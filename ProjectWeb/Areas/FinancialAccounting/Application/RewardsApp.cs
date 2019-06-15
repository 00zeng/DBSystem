using Base.Code;
using MySqlSugar;
using ProjectShare.Database;
using ProjectShare.Models;
using ProjectShare.Process;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ProjectWeb.Areas.SystemManage.Application;

namespace ProjectWeb.Areas.FinancialAccounting.Application
{
    public class RewardsApp
    {
        public object GetGridJson(Pagination pagination, daoben_hr_emp_job queryInfo, QueryTime queryTime)
        {
            var LoginInfo = OperatorProvider.Provider.GetCurrent();
            if (LoginInfo == null)
                return null;
            CompanyInfo myCompanyInfo = LoginInfo.companyInfo;
            PositionInfo myPositionInfo = LoginInfo.positionInfo;

            int records = 0;
            if (pagination == null)
                pagination = new Pagination();
            pagination.page = pagination.page > 0 ? pagination.page : 1;
            pagination.rows = pagination.rows > 0 ? pagination.rows : 20;
            pagination.sidx = string.IsNullOrEmpty(pagination.sidx) ? "a.month" : pagination.sidx;
            pagination.sord = string.IsNullOrEmpty(pagination.sord) ? "desc" : pagination.sord;
            using (var db = SugarDao.GetInstance())
            {
                //查看全部奖罚数据
                var qable = db.Queryable<daoben_salary_reward>()
                    .JoinTable<daoben_hr_emp_job>((a, b) => a.emp_id == b.id);

                if (LoginInfo.roleId != ConstData.ROLE_ID_ADMIN)
                {
                    if (myPositionInfo.positionType == ConstData.POSITION_GM1
                                    || myPositionInfo.positionType == ConstData.POSITION_GM_ASSISTANT1
                                    || LoginInfo.roleId == ConstData.ROLE_ID_FINANCIAL
                                    || LoginInfo.roleId == ConstData.ROLE_ID_FINANCIALMANAGER)
                    {
                        qable.Where(a => (a.company_id_parent == myCompanyInfo.id || a.company_id == myCompanyInfo.id));
                    }
                    else if (myPositionInfo.positionType == ConstData.POSITION_GM2 || myPositionInfo.positionType == ConstData.POSITION_GM_ASSISTANT2)
                        qable.Where(a => a.company_id == myCompanyInfo.id);
                    else if (myPositionInfo.positionType == ConstData.POSITION_DEPT_M)
                        qable.Where(a => a.dept_id == myPositionInfo.id);
                    else
                        qable.Where(a => a.emp_id == LoginInfo.empId);
                }
                if(queryInfo!=null)
                {
                    if (!string.IsNullOrEmpty(queryInfo.name))
                        qable.Where(a => a.emp_name.Contains(queryInfo.name));
                    if (queryInfo.company_id > 0)
                        qable.Where(a => a.company_id == queryInfo.company_id);
                    if (queryInfo.dept_id > 0)
                        qable.Where(a => a.dept_id == queryInfo.dept_id);
                    if (queryInfo.position_id > 0)
                        qable.Where(a => a.position_id == queryInfo.position_id);
                    if (!string.IsNullOrEmpty(queryInfo.position_name))
                        qable.Where(a => a.position_name.Contains(queryInfo.position_name));
                }
                
                if (queryTime != null)
                {
                    if (queryTime.startTime1 != null)
                        qable.Where(a => a.month >= queryTime.startTime1);
                    if (queryTime.startTime2 != null)
                    {
                        DateTime startTime2 = queryTime.startTime2.ToDate().AddMonths(1);
                        qable.Where(a => a.month < startTime2);
                    }
                }
                string listStr = qable.OrderBy(pagination.sidx + " " + pagination.sord)
                       .Select("a.month,b.name,b.work_number,b.position_name,b.grade,b.emp_category,b.area_l1_name,b.area_l2_name,b.dept_name,b.company_name,b.entry_date,a.id,a.creator_name,a.create_time,a.approve_status")
                   .ToJsonPage(pagination.page, pagination.rows, ref records);
                pagination.records = records;
                if (string.IsNullOrEmpty(listStr) || listStr == "[]")
                    return null;
                return listStr.ToJson();
            }
        }

        public object GetGridJson2(Pagination pagination, daoben_hr_emp_job queryInfo)
        {
            var LoginInfo = OperatorProvider.Provider.GetCurrent();
            if (LoginInfo == null)
                return null;
            CompanyInfo myCompanyInfo = LoginInfo.companyInfo;
            PositionInfo myPositionInfo = LoginInfo.positionInfo;
            int records = 0;
            if (pagination == null)
                pagination = new Pagination();
            pagination.page = pagination.page > 0 ? pagination.page : 1;
            pagination.rows = pagination.rows > 0 ? pagination.rows : 20;
            pagination.sidx = string.IsNullOrEmpty(pagination.sidx) ? "create_time" : pagination.sidx;
            pagination.sord = string.IsNullOrEmpty(pagination.sord) ? "desc" : pagination.sord;
            using (var db = SugarDao.GetInstance())
            {
                var qable = db.Queryable<daoben_salary_reward>();
                if (myPositionInfo.positionType == ConstData.POSITION_GM1 || myPositionInfo.positionType == ConstData.POSITION_GM_ASSISTANT1)
                    qable.Where(a => (a.approve_status == 0 && a.company_id == myCompanyInfo.id)
                                    || (a.approve_status == 1 && a.company_id_parent == myCompanyInfo.id));
                else if (myPositionInfo.positionType == ConstData.POSITION_GM2 || myPositionInfo.positionType == ConstData.POSITION_GM_ASSISTANT2)
                    qable.Where(a => a.approve_status == 0 && a.company_id == myCompanyInfo.id);
                else if (LoginInfo.roleId == ConstData.ROLE_ID_FINANCIALMANAGER)
                {
                    qable.Where(a => (a.approve_status == 1 && a.company_id == myCompanyInfo.id)
                                    || (a.approve_status == 2 && a.company_id_parent == myCompanyInfo.id));
                }
                else
                    return null;

                if (queryInfo != null)
                {
                    if (!string.IsNullOrEmpty(queryInfo.name))
                        qable.Where(a => a.emp_name.Contains(queryInfo.name));
                    if (queryInfo.company_id > 0)
                        qable.Where(a => a.company_id == queryInfo.company_id);
                    if (queryInfo.dept_id > 0)
                        qable.Where(a => a.dept_id == queryInfo.dept_id);
                    if (queryInfo.position_id > 0)
                        qable.Where(a => a.position_id == queryInfo.position_id);
                    if (!string.IsNullOrEmpty(queryInfo.position_name))
                        qable.Where(a => a.position_name.Contains(queryInfo.position_name));
                }
                    
                string listStr = qable.OrderBy(pagination.sidx + " " + pagination.sord)
                       .ToJsonPage(pagination.page, pagination.rows, ref records);
                pagination.records = records;
                if (string.IsNullOrEmpty(listStr) || listStr == "[]")
                    return null;
                return listStr.ToJson();
            }
        }

        public object GetGridJson3(Pagination pagination, daoben_hr_emp_job queryInfo)
        {
            var LoginInfo = OperatorProvider.Provider.GetCurrent();
            if (LoginInfo == null)
                return null;
            CompanyInfo myCompanyInfo = LoginInfo.companyInfo;
            PositionInfo myPositionInfo = LoginInfo.positionInfo;

            int records = 0;
            if (pagination == null)
                pagination = new Pagination();
            pagination.page = pagination.page > 0 ? pagination.page : 1;
            pagination.rows = pagination.rows > 0 ? pagination.rows : 20;
            pagination.sidx = string.IsNullOrEmpty(pagination.sidx) ? "create_time" : pagination.sidx;
            pagination.sord = string.IsNullOrEmpty(pagination.sord) ? "desc" : pagination.sord;
            using (var db = SugarDao.GetInstance())
            {
                //查看全部员工(导购员除外)
                var qable = db.Queryable<daoben_hr_emp_job>();

                if (LoginInfo.roleId != ConstData.ROLE_ID_ADMIN)
                {
                    if (LoginInfo.roleId == ConstData.ROLE_ID_FINANCIAL
                                    || LoginInfo.roleId == ConstData.ROLE_ID_FINANCIALMANAGER)
                    {
                        qable.Where(a => (a.company_id_parent == myCompanyInfo.id || a.company_id == myCompanyInfo.id));
                    }
                    else
                        return null;
                }

                //姓名 机构 部门 职位名 
                if (queryInfo != null)
                {
                    if (!string.IsNullOrEmpty(queryInfo.name))
                        qable.Where(a => a.name.Contains(queryInfo.name));
                    if (queryInfo.company_id > 0)
                        qable.Where(a => a.company_id == queryInfo.company_id);
                    if (queryInfo.dept_id > 0)
                        qable.Where(a => a.dept_id == queryInfo.dept_id);
                    if (queryInfo.position_id > 0)
                        qable.Where(a => a.position_id == queryInfo.position_id);
                    if (!string.IsNullOrEmpty(queryInfo.position_name))
                        qable.Where(a => a.position_name.Contains(queryInfo.position_name));
                }                  

                string listStr = qable.OrderBy(pagination.sidx + " " + pagination.sord)
                       .ToJsonPage(pagination.page, pagination.rows, ref records);
                pagination.records = records;
                if (string.IsNullOrEmpty(listStr) || listStr == "[]")
                    return null;
                return listStr.ToJson();
            }
        }

        public string Add(daoben_salary_reward addInfo, List<daoben_salary_reward_detail> rewardAddList)
        {
            var LoginInfo = OperatorProvider.Provider.GetCurrent();
            if (LoginInfo == null)
                return "登录超时，请重新登录";
            CompanyInfo myCompanyInfo = LoginInfo.companyInfo;
            PositionInfo myPositionInfo = LoginInfo.positionInfo;
            using (var db = SugarDao.GetInstance())
            {
                try
                {
                    daoben_hr_emp_job jobInfo = db.Queryable<daoben_hr_emp_job>()
                        .SingleOrDefault(a => a.id == addInfo.emp_id);
                    if (jobInfo == null)
                        return "信息错误：指定的员工信息不存在";
                    addInfo.id = Common.GuId();
                    addInfo.work_number = jobInfo.work_number;
                    addInfo.entry_date = jobInfo.entry_date;
                    addInfo.emp_name = jobInfo.name;
                    addInfo.emp_category = jobInfo.emp_category;
                    addInfo.approve_status = 0;
                    addInfo.position_id = jobInfo.position_id;
                    addInfo.position_name = jobInfo.position_name;
                    addInfo.grade = jobInfo.grade;
                    addInfo.dept_id = jobInfo.dept_id;
                    addInfo.dept_name = jobInfo.dept_name;
                    addInfo.area_l1_id = jobInfo.area_l1_id;
                    addInfo.area_l1_name = jobInfo.area_l1_name;
                    addInfo.area_l2_id = jobInfo.area_l2_id;
                    addInfo.area_l2_name = jobInfo.area_l2_name;
                    addInfo.company_id = jobInfo.company_id;
                    addInfo.company_name = jobInfo.company_name;
                    addInfo.company_id_parent = jobInfo.company_id_parent;
                    //TODO 考核周期 
                    //addInfo.start_date 
                    //addInfo.end_date 

                    addInfo.create_time = DateTime.Now;
                    addInfo.creator_job_history_id = LoginInfo.jobHistoryId;
                    addInfo.creator_name = LoginInfo.empName;
                    addInfo.creator_position_name = myPositionInfo.name;
                    addInfo.create_time = DateTime.Now;
                    if (rewardAddList == null || rewardAddList.Count() == 0)
                        return "没有数据";
                    rewardAddList.ForEach(a =>
                    {
                        a.main_id = addInfo.id;
                    });
                    db.CommandTimeOut = 300;
                    db.BeginTran();

                    db.Insert(addInfo);
                    db.InsertRange(rewardAddList);

                    db.CommitTran();

                }
                catch (Exception ex)
                {
                    db.RollbackTran();
                    return ex.Message;
                }
                //12.29
                approveTemp(addInfo.id);
                return "success";
            }
        }

        public string GetInfo(string id)
        {
            var LoginInfo = OperatorProvider.Provider.GetCurrent();
            if (LoginInfo == null)
                return "登录超时，请重新登录";
            using (var db = SugarDao.GetInstance())
            {
                try
                {
                    daoben_salary_reward rewardInfo = db.Queryable<daoben_salary_reward>().InSingle(id);
                    List<daoben_salary_reward_detail> rewardAddList = db.Queryable<daoben_salary_reward_detail>()
                        .Where(a => a.main_id == id).ToList();
                    List<daoben_salary_reward_approve> approveList = db.Queryable<daoben_salary_reward_approve>()
                        .Where(a => a.main_id == id).ToList();
                    object resultObj = new
                    {
                        rewardInfo = rewardInfo,
                        rewardAddList = rewardAddList,
                        approveList = approveList
                    };
                    return resultObj.ToJson();

                }
                catch (Exception ex)
                {
                    return ex.Message;
                }
            }
        }

        public string Approve(daoben_salary_reward_approve approveInfo)
        {
            var LoginInfo = OperatorProvider.Provider.GetCurrent();
            if (LoginInfo == null)
                return "登录超时，请重新登录";
            using (var db = SugarDao.GetInstance())
            {
                try
                {
                    //申请表ID
                    daoben_salary_reward rewardInfo = db.Queryable<daoben_salary_reward>().InSingle(approveInfo.main_id);
                    if (rewardInfo == null)
                        return "信息错误：指定的员工信息不存在";
                    CompanyInfo myCompanyInfo = LoginInfo.companyInfo;
                    PositionInfo myPositionInfo = LoginInfo.positionInfo;
                    //审核者的信息
                    daoben_hr_emp_job jobInfo = db.Queryable<daoben_hr_emp_job>().InSingle(LoginInfo.empId);
                    approveInfo.approve_id = LoginInfo.accountId;
                    approveInfo.approve_name = LoginInfo.empName;
                    approveInfo.approve_time = DateTime.Now;
                    approveInfo.approve_position_id = myPositionInfo.id;
                    approveInfo.approve_position_name = myPositionInfo.name;

                    if (LoginInfo.roleId == ConstData.ROLE_ID_FINANCIALMANAGER && approveInfo.status > 0)
                    {
                        rewardInfo.approve_status = 100;
                        approveInfo.status = 100;
                    }
                    //TODO 奖罚申请通过之后~~
                    else
                    {
                        if (approveInfo.status > 0)
                            rewardInfo.approve_status = 0 + 1 + rewardInfo.approve_status;
                        else
                            rewardInfo.approve_status = 0 - 1 - rewardInfo.approve_status;
                        approveInfo.status = rewardInfo.approve_status;
                    }


                    db.CommandTimeOut = 300;
                    db.BeginTran();

                    db.Update<daoben_salary_reward>(new { approve_status = rewardInfo.approve_status }, a => a.id == rewardInfo.id);
                    db.Insert(approveInfo);

                    db.CommitTran();

                    return "success";
                }
                catch (Exception ex)
                {
                    db.RollbackTran();
                    return ex.Message;
                }
            }
        }

        internal string Delete(string id)
        {
            using (var db = SugarDao.GetInstance())
            {
                try
                {
                    daoben_salary_reward mainInfo = db.Queryable<daoben_salary_reward>().SingleOrDefault(a => a.id == id);
                    if (mainInfo == null || mainInfo.approve_status != 0)
                        //审批过后，将不能撤回
                        return "撤回失败：指定的申请信息不存在或者已被审批";
                    else
                    {
                        db.CommandTimeOut = 30;
                        db.BeginTran();
                        db.Delete<daoben_salary_reward_detail>(a => a.main_id == id);
                        db.Delete<daoben_salary_reward>(a => a.id == id);
                        db.CommitTran();
                        return "success";
                    }

                }
                catch (Exception ex)
                {
                    db.RollbackTran();
                    return "系统出错：" + ex.Message;
                }
            }
        }

        private void approveTemp(string id)
        {
            using (var db = SugarDao.GetInstance())
            {
                object upObj = new
                {
                    approve_status = 100
                };

                db.Update<daoben_salary_reward>(upObj, a => a.id == id);
            }
        }


    }
}
