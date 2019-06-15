using Base.Code;
using Base.Code.Security;
using System.Linq;
using System;
using MySqlSugar;
using ProjectShare.Database;
using ProjectShare.Process;
using ProjectShare.Models;

namespace ProjectWeb.Application
{
    public class AccountApp
    {
        #region 用户登录
        public LoginStat LoginCheck(daoben_ms_account accountInfo, bool rememberMe)
        {
            string password = accountInfo.password;
            string account = accountInfo.account;
            try
            {
                using (var db = SugarDao.GetInstance())
                {
                    accountInfo.last_login = DateTime.Now;
                    accountInfo = GetAccountInfo(account, db);
                    if (accountInfo == null)
                        return LoginStat.Failure;
                    if (accountInfo.lockout == true && SetUserLockOut(accountInfo, db, true) == true)
                        return LoginStat.LockedOut;
                    //判断是否禁用
                    if (accountInfo.inactive)
                        return LoginStat.Forbidden;

                    if (PasswordStorage.VerifyPassword(password, accountInfo.password))
                    {
                        var updateList = new { last_login = DateTime.Now };
                        AccountUpdate(updateList, accountInfo.id, db);
                        //cookie赋值
                        SaveCookie(accountInfo);
                        return LoginStat.Success;
                    }

                    if (SetUserLockOut(accountInfo, db, false) == true)
                        return LoginStat.LockedOut;
                }
                return LoginStat.Failure;
            }
            catch (Exception ex)
            {
                return LoginStat.SysError;
            }
        }

        #region cookie保存
        /// <summary>
        /// 
        /// </summary>
        /// <param name="accountInfo">账户表</param>
        private void SaveCookie(daoben_ms_account accountInfo)
        {
            string guid = Guid.NewGuid().ToString();
            OperatorModel model = new OperatorModel();
            model.accountId = accountInfo.id;
            model.account = accountInfo.account;
            model.roleId = accountInfo.role_id;
            model.roleName = accountInfo.role_name;
            model.empId = accountInfo.employee_id;
            model.empName = accountInfo.employee_name;
            model.empType = accountInfo.employee_type;
            model.loginTime = DateTime.Now;
            //通过账户表字段 employee_id 获取职位信息
            CompanyInfo myCompanyInfo = new CompanyInfo();
            PositionInfo myPositionInfo = new PositionInfo();
            if (!string.IsNullOrEmpty(accountInfo.employee_id))
            {
                try
                {
                    using (var db = SugarDao.GetInstance())
                    {
                        int companyId = 0;
                        if (accountInfo.employee_type == 0)          //员工账户
                        {

                            daoben_hr_emp_job jobInfo = db.Queryable<daoben_hr_emp_job>()
                                        .Where(a => a.id == accountInfo.employee_id).SingleOrDefault();
                            daoben_hr_emp_job_history jobHistory = null;
                            if (jobInfo!=null)
                                jobHistory = db.Queryable<daoben_hr_emp_job_history>()
                                            .Where(a => a.id == jobInfo.cur_job_history_id).SingleOrDefault();
                            if (jobInfo != null && jobHistory != null)
                            {
                                daoben_org_position positionInfo = db.Queryable<daoben_org_position>().InSingle(jobInfo.position_id);
                                companyId = jobHistory.company_id;
                                model.jobHistoryId = jobHistory.id;
                                myPositionInfo.id = jobHistory.position_id;
                                myPositionInfo.name = jobHistory.position_name;
                                myPositionInfo.grade = jobHistory.grade;
                                myPositionInfo.positionType = jobHistory.position_type;
                                myPositionInfo.deptId = jobHistory.dept_id;
                                myPositionInfo.deptName = jobHistory.dept_name;
                                myPositionInfo.areaL1Id = jobHistory.area_l1_id;
                                myPositionInfo.areaL1Name = jobHistory.area_l1_name;
                                myPositionInfo.areaL2Id = jobHistory.area_l2_id;
                                myPositionInfo.areaL2Name = jobHistory.area_l2_name;
                                model.work_number = jobInfo.work_number;
                                myPositionInfo.supervisorId = jobInfo.supervisor_id;
                                myPositionInfo.supervisorName = jobInfo.supervisor_name;
                            }
                        }
                        else
                        {
                            string distributorId = null;
                            if (accountInfo.employee_type == 1)       // 经销商账户
                                distributorId = accountInfo.employee_id;
                            else if (accountInfo.employee_type == 2)       // 店员账户
                            {
                                daoben_distributor_clerk clerkInfo = db.Queryable<daoben_distributor_clerk>().InSingle(accountInfo.employee_id);
                                distributorId = clerkInfo.distributor_id;
                            }
                            if (!string.IsNullOrEmpty(distributorId))
                            {
                                daoben_distributor_info distributorInfo = db.Queryable<daoben_distributor_info>()
                                                .InSingle(distributorId);
                                if (distributorInfo != null)
                                {
                                    companyId = distributorInfo.company_id;
                                    myPositionInfo.areaL1Id = distributorInfo.area_l1_id;
                                    myPositionInfo.areaL1Name = distributorInfo.area_l1_name;
                                    myPositionInfo.areaL2Id = distributorInfo.area_l2_id;
                                    myPositionInfo.areaL2Name = distributorInfo.area_l2_name;
                                }
                            }
                        }
                        if (companyId > 0)
                        {
                            daoben_org_company companyInfo = db.Queryable<daoben_org_company>().InSingle(companyId);
                            if (companyInfo != null)
                            {
                                myCompanyInfo.id = companyInfo.id;
                                myCompanyInfo.name = companyInfo.name;
                                myCompanyInfo.linkName = companyInfo.link_name;
                                myCompanyInfo.category = companyInfo.category;
                                myCompanyInfo.parentId = companyInfo.parent_id;
                                myCompanyInfo.parentName = companyInfo.parent_name;
                            }
                        }
                    }
                }
                catch (Exception ex) { return; }
            }
            model.companyInfo = myCompanyInfo;
            model.positionInfo = myPositionInfo;
            OperatorProvider.Provider.AddCurrent(model);

        }
        #endregion

        private bool SetUserLockOut(daoben_ms_account accountInfo, SqlSugarClient db, bool to_unlock = false)
        {
            if (accountInfo.last_attempt != null)
            {
                if (DateTime.Now.Subtract((DateTime)accountInfo.last_attempt).Minutes
                        > DefaultAccountSetup.LockoutTimeSpan_FromMinutes)
                {
                    // Reset the failed login attempt count.
                    accountInfo.attempt_login = 0;
                    accountInfo.last_attempt = null;
                    accountInfo.lockout = false;
                }
                else
                {
                    if (to_unlock)
                        return accountInfo.lockout;
                }
            }

            if (!to_unlock)
            {
                // if account is locked out, attempt_login has reached the max count.
                accountInfo.attempt_login++;
                accountInfo.last_attempt = DateTime.Now;

                if (accountInfo.attempt_login == DefaultAccountSetup.MaxAttemptsBeforeLockout)
                {
                    accountInfo.lockout = true;
                }
            }
            var updateList = new
            {
                attempt_login = accountInfo.attempt_login,
                last_attempt = accountInfo.last_attempt,
                lockout = accountInfo.lockout
            };

            if (AccountUpdate(updateList, accountInfo.id, db) != "success")
                return false;
            return accountInfo.lockout;
        }

        #endregion

        #region 根据登录名获取用户
        public daoben_ms_account GetAccountInfo(string account, SqlSugarClient db)
        {
            daoben_ms_account accountInfo = db.Queryable<daoben_ms_account>().SingleOrDefault(t => t.account == account);
            if (accountInfo == null)
                return null;
            return accountInfo;
        }
        #endregion

        #region 更新用户信息
        /// <summary>
        /// 更新用户信息
        /// </summary>
        /// <param name="updateList"></param>
        /// <returns></returns>
        public string AccountUpdate(object updateList, int id, SqlSugarClient db)
        {
            if (updateList == null)
                return "success";
            try
            {
                if (id < 1)
                    return "ID有误";
                if (db.Update<daoben_ms_account>(updateList, t => t.id == id))
                    return "success";
                return "系统出错";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }
        #endregion

        #region 修改密码
        public string UpdatePassword(string password, string newPassword)
        {
            OperatorModel LoginInfo = OperatorProvider.Provider.GetCurrent();
            if (LoginInfo == null)
                throw new Exception("用户登陆过期，请重新登录");
            if (string.IsNullOrEmpty(newPassword))
                return "信息错误，删除失败！";
            try
            {
                using (var db = SugarDao.GetInstance())
                {
                    daoben_ms_account accountInfo = db.Queryable<daoben_ms_account>().InSingle(LoginInfo.accountId);
                    if (accountInfo == null)
                        return "账号不存在，修改失败！";
                    if (!PasswordStorage.VerifyPassword(password, accountInfo.password))
                    {
                        return "原始密码输入不正确！";
                    }
                    if (password == newPassword)
                        return "success";
                    newPassword = PasswordStorage.CreateHash(newPassword);//修改的密码加密
                    db.Update<daoben_ms_account>(new { password = newPassword }, a => a.id == LoginInfo.accountId);
                }
                return "success";
            }
            catch (Exception ex)
            {
                throw new Exception("系统出错：" + ex.Message);
            }
        }
        #endregion

    }
}
