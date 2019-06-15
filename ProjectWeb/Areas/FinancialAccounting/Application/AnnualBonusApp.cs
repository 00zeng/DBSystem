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
    public class AnnualBonusApp
    {

        public object GetGridJson(Pagination pagination, daoben_hr_emp_job queryInfo, int approve_status, QueryTime queryTime)
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
                var qable = db.Queryable<daoben_salary_annualbonus>()
                    .JoinTable<daoben_hr_emp_job>((a, b) => a.emp_id == b.id);
                if (myPositionInfo.positionType == ConstData.POSITION_GM1 || myPositionInfo.positionType == ConstData.POSITION_GM_ASSISTANT1)//事业部
                    qable.Where(a => a.company_id == myCompanyInfo.id || a.company_id_parent == myCompanyInfo.id);
                else if (myPositionInfo.positionType == ConstData.POSITION_GM2 || myPositionInfo.positionType == ConstData.POSITION_GM_ASSISTANT2)//分公司
                    qable.Where(a => a.company_id == myCompanyInfo.id);
                else if (LoginInfo.roleId == ConstData.ROLE_ID_FINANCIALMANAGER)//财务经理
                {
                    qable.Where(a => a.company_id == myCompanyInfo.id || a.company_id_parent == myCompanyInfo.id);
                }
                else
                    qable.Where(a => a.company_id == myCompanyInfo.id || a.company_id_parent == myCompanyInfo.id);
                //姓名 机构 部门 职位名 审批状态 入职时间 * 2
                if (queryInfo != null)
                {
                    if (!string.IsNullOrEmpty(queryInfo.name))
                        qable.Where(a => a.emp_name.Contains(queryInfo.name));
                    if (queryInfo.company_id > 0)
                        qable.Where(a => a.company_id == queryInfo.company_id);
                    if (queryInfo.dept_id > 0)
                        qable.Where(a => a.dept_id == queryInfo.dept_id);
                    if (!string.IsNullOrEmpty(queryInfo.position_name))
                        qable.Where(a => a.position_name.Contains(queryInfo.position_name));
                }                 
                if (approve_status != 0)  // 0表示查找全部
                {
                    int status = approve_status;
                    if (status == 100)   // 已审批
                        qable.Where(a => a.approve_status == 100);
                    else if (status == -100)    // 审批不通过
                        qable.Where(a => a.approve_status < 0);
                    else if (status == 1)    // 审批中
                        qable.Where(a => a.approve_status > 0 && a.approve_status < 100);
                    else if (status == -1)    // 未审批
                        qable.Where(a => a.approve_status == 0);
                }

                if (queryTime != null)
                {
                    if (queryTime.startTime1 != null)
                        qable.Where(a => a.bonus_year >= queryTime.startTime1.ToDate().Year);
                    if (queryTime.startTime2 != null)
                        qable.Where(a => a.bonus_year <= queryTime.startTime2.ToDate().Year);
                }

                string listStr = qable.OrderBy(pagination.sidx + " " + pagination.sord)
                       .Select("a.*,b.entry_date as entry_date")
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
            pagination.sord = string.IsNullOrEmpty(pagination.sord) ? "asc" : pagination.sord;
            using (var db = SugarDao.GetInstance())
            {
                // 我的审批 
                var qable = db.Queryable<daoben_salary_annualbonus>();
                if (myPositionInfo.positionType == ConstData.POSITION_GM1 || myPositionInfo.positionType == ConstData.POSITION_GM_ASSISTANT1)
                {
                    qable.Where(a => ((a.approve_status == 0 && a.company_id == myCompanyInfo.id)
                                     || (a.approve_status == 1 && a.company_id_parent == myCompanyInfo.id)));
                }
                else if (myPositionInfo.positionType == ConstData.POSITION_GM2 || myPositionInfo.positionType == ConstData.POSITION_GM_ASSISTANT2)
                    qable.Where(a => a.approve_status == 0 && a.company_id == myCompanyInfo.id);
                else if (LoginInfo.roleId == ConstData.ROLE_ID_FINANCIALMANAGER)
                {
                    qable.Where(a => ((a.approve_status == 1 && a.company_id == myCompanyInfo.id)
                                   || (a.approve_status == 2 && a.company_id_parent == myCompanyInfo.id)));
                }
                else
                    return null;
                if(queryInfo!=null)
                {
                    if (!string.IsNullOrEmpty(queryInfo.name))
                        qable.Where(a => a.emp_name.Contains(queryInfo.name));
                    if (!string.IsNullOrEmpty(queryInfo.position_name))
                        qable.Where(a => a.position_name.Contains(queryInfo.position_name));
                    if (queryInfo.dept_id > 0)
                        qable.Where(a => a.dept_id == queryInfo.dept_id);
                    if (queryInfo.company_id > 0)
                        qable.Where(a => a.company_id == queryInfo.company_id);
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
            pagination.sord = string.IsNullOrEmpty(pagination.sord) ? "asc" : pagination.sord;
            using (var db = SugarDao.GetInstance())
            {
                //查看全部员工(导购员除外)
                var qable = db.Queryable<daoben_hr_emp_job>()
                    .Where(a => a.position_type != ConstData.POSITION_GUIDE);

                if (LoginInfo.roleId != ConstData.ROLE_ID_ADMIN)
                {
                    if (LoginInfo.roleId == ConstData.ROLE_ID_FINANCIAL
                                    || LoginInfo.roleId == ConstData.ROLE_ID_FINANCIALMANAGER)
                        qable.Where(a => (a.company_id_parent == myCompanyInfo.id || a.company_id == myCompanyInfo.id));
                    else
                        return null;
                }
                if (queryInfo != null)
                {
                    if (!string.IsNullOrEmpty(queryInfo.name))
                        qable.Where(a => a.name.Contains(queryInfo.name));
                    if (!string.IsNullOrEmpty(queryInfo.position_name))
                        qable.Where(a => a.position_name.Contains(queryInfo.position_name));
                    if (queryInfo.dept_id > 0)
                        qable.Where(a => a.dept_id == queryInfo.dept_id);
                    if (queryInfo.company_id > 0)
                        qable.Where(a => a.company_id == queryInfo.company_id);
                }
                string listStr = qable.OrderBy(pagination.sidx + " " + pagination.sord)
                       .ToJsonPage(pagination.page, pagination.rows, ref records);
                pagination.records = records;
                if (string.IsNullOrEmpty(listStr) || listStr == "[]")
                    return null;
                return listStr.ToJson();
            }
        }
        public string Add(daoben_salary_annualbonus addInfo)
        {
            var LoginInfo = OperatorProvider.Provider.GetCurrent();
            if (LoginInfo == null)
                return "登录超时，请重新登录";
            CompanyInfo myCompanyInfo = LoginInfo.companyInfo;
            PositionInfo myPositionInfo = LoginInfo.positionInfo;
            DateTime now = DateTime.Now;
            using (var db = SugarDao.GetInstance())
            {
                try
                {
                    daoben_hr_emp_job jobInfo = db.Queryable<daoben_hr_emp_job>()
                        .SingleOrDefault(a => a.id == addInfo.emp_id);
                    if (jobInfo == null)
                        return "信息错误：指定的员工信息不存在";
                    addInfo.id = Common.GuId();
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
                    addInfo.work_number = jobInfo.work_number;
                    addInfo.creator_job_history_id = LoginInfo.jobHistoryId;
                    addInfo.creator_name = LoginInfo.empName;
                    addInfo.create_time = DateTime.Now;

                    #region 待办事项 财务
                    //待办事项 收件人
                    string tempStr = "年终奖申请待审批";
                    List<string> idList = db.Queryable<daoben_hr_emp_job>()
                        .JoinTable<daoben_ms_account>((a, b) => a.id == b.employee_id)
                        .Where<daoben_ms_account>((a, b) => b.role_id == ConstData.ROLE_ID_FINANCIALMANAGER)
                        .Where(a => a.company_id == addInfo.company_id)
                        .Select<string>("a.id as id").ToList();
                    //待办事项 生成列表
                    List<daoben_sys_task> taskList = idList
                        .Select(a1 => new daoben_sys_task
                        {
                            emp_id = a1,
                            category = 1,
                            main_id = addInfo.id,
                            main_url = "/SaleManage/AnnualBonus/Approve?id=" + addInfo.id,
                            title = tempStr,
                            content_abstract = addInfo.note,
                            recipient_type = 1,
                            create_time = now,
                            status = 1
                        }).ToList();
                    #endregion
                    db.Insert(addInfo);
                    if (taskList != null && taskList.Count > 25)
                        db.SqlBulkCopy(taskList);
                    else if (taskList != null && taskList.Count > 0)
                        db.InsertRange(taskList);

                }
                catch (Exception ex)
                {
                    return ex.Message;
                }
                //
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
                    daoben_salary_annualbonus annualbonusInfo = db.Queryable<daoben_salary_annualbonus>().InSingle(id);
                    if (annualbonusInfo == null)
                        return "信息错误：指定的申请信息不存在";
                    List<daoben_salary_annualbonus_approve> listApproveInfo = db.Queryable<daoben_salary_annualbonus_approve>()
                        .Where(a => a.main_id == id).ToList();
                    object resultObj = new
                    {
                        annualbonusInfo = annualbonusInfo,
                        listApproveInfo = listApproveInfo
                    };
                    return resultObj.ToJson();

                }
                catch (Exception ex)
                {
                    return ex.Message;
                }
            }
        }

        public string Approve(daoben_salary_annualbonus_approve approveInfo)
        {
            var LoginInfo = OperatorProvider.Provider.GetCurrent();
            if (LoginInfo == null)
                return "登录超时，请重新登录";
            CompanyInfo myCompanyInfo = LoginInfo.companyInfo;
            PositionInfo myPositionInfo = LoginInfo.positionInfo;
            DateTime now = DateTime.Now;
            using (var db = SugarDao.GetInstance())
            {
                try
                {
                    daoben_salary_annualbonus annualbonusInfo = db.Queryable<daoben_salary_annualbonus>()
                        .InSingle(approveInfo.main_id);
                    if (annualbonusInfo == null)
                        return "信息错误：指定的年终奖信息不存在";
                    if (LoginInfo.roleId == ConstData.ROLE_ID_FINANCIALMANAGER && approveInfo.status > 0)
                        annualbonusInfo.approve_status = 100;
                    else
                    {
                        if (approveInfo.status > 0)
                            annualbonusInfo.approve_status = 0 + 1 + annualbonusInfo.approve_status;
                        else
                            annualbonusInfo.approve_status = 0 - 1 - annualbonusInfo.approve_status;
                    }
                    approveInfo.status = annualbonusInfo.approve_status;
                    approveInfo.approve_name = LoginInfo.empName;
                    approveInfo.approve_position_name = myPositionInfo.name;
                    approveInfo.approve_position_id = myPositionInfo.id;
                    approveInfo.approve_time = DateTime.Now;
                    //TODO 消息通知——通知申请者 待测试
                    #region 消息通知 创建人
                    //消息通知 收件人id列表
                    string tempStr = null;
                    tempStr = "年终奖信息申请" + (approveInfo.status == 100 ? "通过" : "不通过");
                    List<string> idList = db.Queryable<daoben_hr_emp_job_history>()
                        .Where(a => a.company_id == myCompanyInfo.id)
                        .Where(a => a.id == annualbonusInfo.creator_job_history_id).Select<string>("emp_id as id").ToList();
                    //消息通知 生成列表
                    List<daoben_sys_notification> newsList = idList
                        .Select(a1 => new daoben_sys_notification
                        {
                            emp_id = a1,
                            category = 2,
                            main_id = annualbonusInfo.id,
                            main_url = "/SaleManage/AnnualBonus/Show?id=" + annualbonusInfo.id,
                            title = tempStr,
                            content_abstract = annualbonusInfo.note,
                            recipient_type = 1,
                            create_time = now,
                            status = 1
                        }).ToList();
                    #endregion
                    db.CommandTimeOut = 30;
                    db.BeginTran();
                    db.Update<daoben_salary_annualbonus>(new { approve_status = annualbonusInfo.approve_status }, a => a.id == annualbonusInfo.id);
                    db.Insert(approveInfo);
                    //代办事项 置为 已处理
                    List<daoben_sys_task> origTask = db.Queryable<daoben_sys_task>().Where(a => a.main_id == approveInfo.main_id).ToList();
                    object upObjForTask = new
                    {
                        status = 3,
                        finished_time = now
                    };
                    if (origTask != null)
                        db.Update<daoben_sys_task>(upObjForTask, a => a.main_id == approveInfo.main_id);
                    if (newsList != null && newsList.Count > 25)
                        db.SqlBulkCopy(newsList);
                    else if (newsList != null && newsList.Count > 0)
                        db.InsertRange(newsList);

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
                    daoben_salary_annualbonus mainInfo = db.Queryable<daoben_salary_annualbonus>().SingleOrDefault(a => a.id == id);
                    if (mainInfo == null || mainInfo.approve_status != 0)
                        //审批过后，将不能撤回
                        return "撤回失败：指定的申请信息不存在或者已被审批";
                    else
                    {
                        db.Delete<daoben_salary_annualbonus>(a => a.id == id);
                        //删除所有待办事项
                        db.Delete<daoben_sys_task>(a => a.main_id == id);
                        db.Delete<daoben_sys_notification>(a => a.main_id == id);
                        return "success";
                    }
                }
                catch (Exception ex)
                {
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

                db.Update<daoben_salary_annualbonus>(upObj, a => a.id == id);
            }
        }

    }
}
