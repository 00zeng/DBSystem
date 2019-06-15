using System;
using System.Linq;
using MySqlSugar;
using Base.Code;
using Base.Code.Security;
using ProjectShare.Database;
using ProjectShare.Process;

namespace ProjectWeb.Areas.SystemManage.Application
{
    public class MsAccountApp
    {
        #region 账户列表查询
        /// <summary>
        /// 账户列表查询
        /// </summary>
        /// <param name="pagination"></param>
        /// <param name="accountInfo"></param>
        /// <param name="accountName">前端account有冲突，改为accountName</param>
        /// <returns></returns>
        public object GetList(Pagination pagination, daoben_ms_account accountInfo, string accountName)
        {
            OperatorModel LoginInfo = OperatorProvider.Provider.GetCurrent();
            if (LoginInfo == null)
                throw new Exception("用户登陆过期，请重新登录");
            int records = 0;
            if (pagination == null)
                pagination = new Pagination();
            pagination.page = pagination.page > 0 ? pagination.page : 1;
            pagination.rows = pagination.rows > 0 ? pagination.rows : 20;
            pagination.sidx = string.IsNullOrEmpty(pagination.sidx) ? "reg_time" : pagination.sidx;
            pagination.sord = string.IsNullOrEmpty(pagination.sord) ? "desc" : pagination.sord;

            bool inactive = false;
            string account = accountName;
            string empName = null;
            string roleName = null;
            if (accountInfo != null)
            {
                inactive = accountInfo.inactive;
                empName = accountInfo.employee_name;
                roleName = accountInfo.role_name;
            }
            using (var db = SugarDao.GetInstance())
            {
                var qable = db.Queryable<daoben_ms_account>().Where(a => a.inactive == inactive);

                if (!string.IsNullOrEmpty(account))
                    qable.Where(a => a.account.Contains(account));
                if (!string.IsNullOrEmpty(empName))
                    qable.Where(a => a.employee_name.Contains(empName));
                if (!string.IsNullOrEmpty(roleName))
                    qable.Where(a => a.employee_name.Contains(roleName));

                string listStr = qable.OrderBy(pagination.sidx + " " + pagination.sord)
                        .ToJsonPage(pagination.page, pagination.rows, ref records);
                pagination.records = records;
                if (string.IsNullOrEmpty(listStr) || listStr == "[]")
                    return null;
                return listStr.ToJson();
            }
        }
        #endregion

        #region 获取员工信息
        /// <summary>
        /// 获取员工信息
        /// </summary>     
        /// <returns></returns>
        //public object GetEmployeeInfo(string employeeID)
        //{

        //}
        #endregion

        #region 新增账户
        public string AddAccount(daoben_ms_account accountInfo)
        {
            OperatorModel LoginInfo = OperatorProvider.Provider.GetCurrent();
            if (LoginInfo == null)
                throw new Exception("用户登陆过期，请重新登录");
            if (accountInfo == null || string.IsNullOrEmpty(accountInfo.account))
                return "信息错误，操作失败!";

            if (string.IsNullOrEmpty(accountInfo.employee_id) || string.IsNullOrEmpty(accountInfo.employee_name))
            {
                accountInfo.employee_id = null;
                accountInfo.employee_name = accountInfo.account;
            }
            //初始化账户信息
            accountInfo.inactive = false;
            accountInfo.inactive_id = 0;
            accountInfo.inactive_name = null;
            accountInfo.inactive_time = null;
            accountInfo.reg_time = DateTime.Now;
            accountInfo.creator_id = LoginInfo.accountId;
            accountInfo.creator_name = LoginInfo.empName;
            accountInfo.password = PasswordStorage.CreateHash(accountInfo.password);
            try
            {
                using (var db = SugarDao.GetInstance())
                {
                    daoben_ms_account accountOrig = db.Queryable<daoben_ms_account>()
                            .Where(a => a.account == accountInfo.account).FirstOrDefault();
                    if (accountOrig != null)
                    {
                        if (accountOrig.inactive)
                            return "该账户名称已存在(已注销)，操作失败！";
                        else
                            return "该账户名称已存在，操作失败！";
                    }
                    if (accountInfo.role_id > 0)
                    {
                        daoben_ms_role roleInfo = db.Queryable<daoben_ms_role>().InSingle(accountInfo.role_id);
                        if (roleInfo == null)
                            return "指定角色不存在，操作失败!";
                        accountInfo.role_name = roleInfo.name;
                    }
                    else
                    {
                        accountInfo.role_id = 0;
                        accountInfo.role_name = null;
                    }
                    db.DisableInsertColumns = new string[] { "id" };
                    accountInfo.id = db.Insert(accountInfo).ToInt();
                }
                return "success";
            }
            catch (Exception ex)
            {
                return "系统出错：" + ex.Message;
            }
        }
        #endregion
        #region 变更角色
        /// <summary>
        /// 变更角色
        /// </summary>     
        /// <returns></returns>
        /// 
        public string UpdateRole(int id, int roleId)
        {
            OperatorModel LoginInfo = OperatorProvider.Provider.GetCurrent();
            if (LoginInfo == null)
                throw new Exception("用户登陆过期，请重新登录");
            if (id == 0 || roleId == 0)
                return "信息错误，操作失败！";
            try
            {
                using (var db = SugarDao.GetInstance())
                {
                    daoben_ms_account accountInfo = db.Queryable<daoben_ms_account>().InSingle(id);
                    if (accountInfo == null || accountInfo.inactive)
                        return "账户不存在或已注销，操作失败！";

                    daoben_ms_role roleInfo = db.Queryable<daoben_ms_role>().InSingle(roleId);
                    if (roleInfo == null || roleInfo.inactive)
                        return "指定角色不存在或已注销，操作失败!";

                    if (accountInfo.role_id == roleId)
                        return "success";
                    object obj = new
                    {
                        role_id = roleId,
                        role_name = roleInfo.name
                    };
                    db.Update<daoben_ms_account>(obj, a => a.id == id);
                }
                return "success";
            }
            catch (Exception ex)
            {
                return "系统出错：" + ex.Message;
            }
        }
        #endregion

        #region 修改密码
        public string ResetPassword(int id, string newPassword)
        {
            OperatorModel LoginInfo = OperatorProvider.Provider.GetCurrent();
            if (LoginInfo == null)
                throw new Exception("用户登陆过期，请重新登录");
            if (id == 0)
                return "信息错误，操作失败！";
            try
            {
                using (var db = SugarDao.GetInstance())
                {
                    daoben_ms_account accountInfo = db.Queryable<daoben_ms_account>().InSingle(id);
                    if (accountInfo == null)
                        return "账号不存在，操作失败！";

                    newPassword = PasswordStorage.CreateHash(newPassword);//修改的密码加密
                    if (accountInfo.password == newPassword)
                        return "success";
                    db.Update<daoben_ms_account>(new { password = newPassword }, a => a.id == id);
                }
                return "success";
            }
            catch (Exception ex)
            {
                return "系统出错：" + ex.Message;
            }
        }
        #endregion

        #region 启用/注销账户
        /// <summary>
        /// 启用/注销账户
        /// </summary>
        /// <returns></returns>


        public string ActiveAccount(int id)
        {
            OperatorModel LoginInfo = OperatorProvider.Provider.GetCurrent();
            if (LoginInfo == null)
                throw new Exception("用户登陆过期，请重新登录");
            if (id == 0)
                return "信息错误，操作失败！";
            try
            {
                using (var db = SugarDao.GetInstance())
                {
                    daoben_ms_account accountInfo = db.Queryable<daoben_ms_account>().InSingle(id);
                    if (accountInfo == null)
                        return "账户不存在，操作失败！";
                    object obj = new
                    {
                        inactive = !accountInfo.inactive,
                        inactive_id = LoginInfo.accountId,
                        inactive_name = LoginInfo.empName,
                        inactive_time = DateTime.Now
                    };
                    db.Update<daoben_ms_account>(obj, a => a.id == id);
                }
                return "success";
            }

            catch (Exception ex)
            {
                return "系统出错：" + ex.Message;
            }
        }
        #endregion

        #region 删除账户
        /// <summary>
        /// 删除账户
        /// </summary>
        /// <returns></returns>
        public string DeleteAccount(int id)
        {
            OperatorModel LoginInfo = OperatorProvider.Provider.GetCurrent();
            if (LoginInfo == null)
                throw new Exception("用户登陆过期，请重新登录");
            if (id == 0)
                return "信息错误，操作失败！";
            try
            {
                using (var db = SugarDao.GetInstance())
                {
                    daoben_ms_account accountInfo = db.Queryable<daoben_ms_account>().InSingle(id);
                    if (accountInfo == null)
                        return "账号不存在，操作失败！";
                    if(!accountInfo.inactive)
                        return "账号未注销，请先注销！";
                    if (accountInfo.employee_id != null)
                        return "该账户有对应的员工信息，操作失败！";
                    db.Delete<daoben_ms_account>(a => a.id == id);                    
                }
                return "success";
            }
            catch (Exception ex)
            {
                return "系统出错：" + ex.Message;
            }
        }
        #endregion
    }
}
