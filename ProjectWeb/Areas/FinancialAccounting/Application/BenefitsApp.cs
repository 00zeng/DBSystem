using Base.Code;
using MySqlSugar;
using ProjectShare.Database;
using ProjectShare.Models;
using ProjectShare.Process;
using ProjectWeb.Areas.SalaryCalculate.Application;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ProjectWeb.Areas.FinancialAccounting.Application
{
    public class BenefitsApp
    {
        public object GetList(Pagination pagination, daoben_hr_emp_job queryInfo, int approve_status)
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
                //查看全部
                var qable = db.Queryable<daoben_salary_benefit>();
                //if (LoginInfo.roleId != ConstData.ROLE_ID_ADMIN)
                //{
                //    if (LoginInfo.roleId == ConstData.ROLE_ID_FINANCIALMANAGER
                //                || LoginInfo.roleId == ConstData.ROLE_ID_FINANCIAL
                //                || LoginInfo.roleId == ConstData.ROLE_ID_HR
                //                || LoginInfo.roleId == ConstData.ROLE_ID_HRMANAGER)
                //        qable.Where(a => (a.company_id == myCompanyInfo.id) || (a.company_id_parent == myCompanyInfo.id));
                //    else if (myPositionInfo.positionType == ConstData.POSITION_GM1 || myPositionInfo.positionType == ConstData.POSITION_GM_ASSISTANT1)
                //        qable.Where(a => (a.company_id == myCompanyInfo.id) || (a.company_id_parent == myCompanyInfo.id));
                //    else if (myPositionInfo.positionType == ConstData.POSITION_GM2 || myPositionInfo.positionType == ConstData.POSITION_GM_ASSISTANT2)
                //        qable.Where(a => a.company_id == myCompanyInfo.id);
                //    else return null;
                //}
                if (myCompanyInfo.category == "事业部")
                    qable.Where(a => (a.company_id == myCompanyInfo.id) || (a.company_id_parent == myCompanyInfo.id));
                else if (myCompanyInfo.category == "分公司")
                    qable.Where(a => a.company_id == myCompanyInfo.id);

                // 机构 审批状态 

                if (queryInfo.company_id > 0)
                    qable.Where(a => a.company_id == queryInfo.company_id);

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

                string listStr = qable.OrderBy(pagination.sidx + " " + pagination.sord)
                    .ToJsonPage(pagination.page, pagination.rows, ref records);
                pagination.records = records;
                if (string.IsNullOrEmpty(listStr) || listStr == "[]")
                    return null;
                return listStr.ToJson();
            }
        }

        public object GetListMyApprove(Pagination pagination, int company_id)
        {
            var LoginInfo = OperatorProvider.Provider.GetCurrent();
            if (LoginInfo == null)
                return null;
            int records = 0;
            if (pagination == null)
                pagination = new Pagination();
            pagination.page = pagination.page > 0 ? pagination.page : 1;
            pagination.rows = pagination.rows > 0 ? pagination.rows : 20;
            pagination.sidx = string.IsNullOrEmpty(pagination.sidx) ? "id" : pagination.sidx;
            pagination.sord = string.IsNullOrEmpty(pagination.sord) ? "asc" : pagination.sord;
            CompanyInfo myCompanyInfo = LoginInfo.companyInfo;
            PositionInfo myPositionInfo = LoginInfo.positionInfo;
            using (var db = SugarDao.GetInstance())
            {
                //我的审批-分公司福利：由分公司助理录入0-分公司总经理审批1-总经理2-财务3
                //我的审批-事业部福利：由人事录入0 - 总经理1 - 财务2
                var qable = db.Queryable<daoben_salary_benefit>();
                if (LoginInfo.roleId != ConstData.ROLE_ID_ADMIN)
                {
                    if (LoginInfo.roleId == ConstData.ROLE_ID_FINANCIALMANAGER)//财务经理
                        qable.Where(a => (a.approve_status == 1 && a.company_id == myCompanyInfo.id)
                                    || (a.approve_status == 2 && a.company_id_parent == myCompanyInfo.id));
                    else if (myPositionInfo.positionType == ConstData.POSITION_GM1)//事业部总经理
                        qable.Where(a => (a.approve_status == 0 && a.company_id == myCompanyInfo.id)
                                    || (a.approve_status == 1 && a.company_id_parent == myCompanyInfo.id));
                    else if (myPositionInfo.positionType == ConstData.POSITION_GM2)//分公司总经理
                        qable.Where(a => a.approve_status == 0 && a.company_id == myCompanyInfo.id);
                    else
                        return null;
                }

                // 机构  
                if (company_id > 0)
                    qable.Where(a => a.company_id == company_id);

                string listStr = qable.OrderBy(pagination.sidx + " " + pagination.sord)
                                .ToJsonPage(pagination.page, pagination.rows, ref records);
                pagination.records = records;
                if (string.IsNullOrEmpty(listStr) || listStr == "[]")
                    return null;
                return listStr.ToJson();
            }
        }

        public string Add(daoben_salary_benefit addInfo, List<daoben_salary_benefit_emp> empInfoList,
                    List<daoben_salary_benefit_detail> addBenefitsList)
        {
            var LoginInfo = OperatorProvider.Provider.GetCurrent();
            if (LoginInfo == null)
                return null;
            CompanyInfo myCompanyInfo = LoginInfo.companyInfo;
            PositionInfo myPositionInfo = LoginInfo.positionInfo;
            using (var db = SugarDao.GetInstance())
            {
                try
                {
                    daoben_org_company companyInfo = db.Queryable<daoben_org_company>()
                        .SingleOrDefault(a => a.id == addInfo.company_id);
                    if (companyInfo != null)
                        addInfo.company_name = companyInfo.link_name;
                    else
                        return "信息错误：指定的信息不存在";
                    addInfo.approve_status = 0;
                    addInfo.company_id = myCompanyInfo.id;
                    addInfo.company_id_parent = myCompanyInfo.parentId;
                    addInfo.creator_job_history_id = LoginInfo.jobHistoryId;
                    addInfo.creator_name = LoginInfo.empName;
                    addInfo.create_time = DateTime.Now;

                    db.CommandTimeOut = 30;
                    db.BeginTran();
                    db.Insert(addInfo);
                    db.DisableInsertColumns = new string[] { "id" };
                    if (empInfoList != null)
                        db.InsertRange(empInfoList);
                    if (addBenefitsList != null)
                        db.InsertRange(addBenefitsList);
                    db.CommitTran();
                    
                }
                catch (Exception ex)
                {
                    db.RollbackTran();
                    return "系统出错：" + ex.Message;
                }
                //12.29
                approveTemp(addInfo.id);
                return "success";
            }
        }

        public string Approve(daoben_salary_benefit_approve approveInfo)
        {
            var LoginInfo = OperatorProvider.Provider.GetCurrent();
            if (LoginInfo == null)
                return "登录超时，请重新登录";
            CompanyInfo myCompanyInfo = LoginInfo.companyInfo;
            PositionInfo myPositionInfo = LoginInfo.positionInfo;
            object upObj = null;
            daoben_sys_cron cronInfo = null;
            using (var db = SugarDao.GetInstance())
            {
                try
                {
                    //由 分公司助理录入 - 分公司总经理审批 - 总经理 - 财务经理
                    //事业部福利：由人事录入 - 总经理 - 财务经理
                    daoben_salary_benefit mainInfo = db.Queryable<daoben_salary_benefit>().InSingle(approveInfo.main_id);
                    if (mainInfo == null)
                        return "信息错误：指定的申请信息不存在";
                    if (LoginInfo.roleId == ConstData.ROLE_ID_FINANCIALMANAGER && approveInfo.status > 0)
                    {
                        approveInfo.status = 100;
                        upObj = new { approve_status = 100 };
                        // 加入定时任务列表
                        PayrollSettingApp payRollApp = new PayrollSettingApp();
                        DateTime month = payRollApp.GetInfo(0).month;
                        cronInfo = new daoben_sys_cron { main_id = mainInfo.id, month = month, is_finished = false, category = 11, create_time = DateTime.Now };
                    }
                    else
                    {
                        if (approveInfo.status > 0)
                        {
                            upObj = new { approve_status = (1 + mainInfo.approve_status) };
                            approveInfo.status = 1 + mainInfo.approve_status;
                        }
                        else
                        {
                            upObj = new { approve_status = (0 - 1 - mainInfo.approve_status) };
                            approveInfo.status = 1 + mainInfo.approve_status;
                        }
                    }
                    approveInfo.approve_name = LoginInfo.empName;
                    approveInfo.approve_position_name = myPositionInfo.name;
                    approveInfo.approve_position_id = myPositionInfo.id;
                    approveInfo.approve_time = DateTime.Now;
                    db.CommandTimeOut = 30;
                    db.BeginTran();
                    db.DisableInsertColumns = new string[] { "id" };
                    db.Update<daoben_salary_benefit>(upObj, a => a.id == mainInfo.id);
                    db.Insert(approveInfo);
                    if (cronInfo != null)
                        db.Insert(cronInfo);
                    db.CommitTran();
                    return "success";
                }
                catch (Exception ex)
                {
                    db.RollbackTran();
                    return "系统出错：" + ex.Message;
                }
            }
        }

        public string GetInfo(string id)
        {
            var LoginInfo = OperatorProvider.Provider.GetCurrent();
            if (LoginInfo == null)
                return "登录超时，请重新登录";
            CompanyInfo myCompanyInfo = LoginInfo.companyInfo;
            //string selectStr = null;
            using (var db = SugarDao.GetInstance())
            {
                try
                {
                    daoben_salary_benefit benefitsInfo = db.Queryable<daoben_salary_benefit>().InSingle(id);
                    if (benefitsInfo == null)
                        return "信息错误：指定的申请信息不存在";
                    List<daoben_salary_benefit_approve> ApprovelistInfo = db.Queryable<daoben_salary_benefit_approve>()
                        .Where(a => a.main_id == id).ToList();
                    List<daoben_salary_benefit_detail> benefitslistInfo = db.Queryable<daoben_salary_benefit_detail>()
                        .Where(a => a.main_id == id).ToList();
                    //if (myCompanyInfo.category == "分公司")
                    //    selectStr = "a.id, a.name,a.position_name,a.dept_name,a.area_l2_name, CONCAT(a.name,' (',a.position_name,' - ', IFNULL(a.area_l1_name,'无所属区域'), ')') as display_info";
                    //else
                    //    selectStr = "a.id, a.name,a.position_name,a.dept_name,a.area_l2_name, CONCAT(a.name,' (',a.position_name,' - ', IFNULL(a.dept_name,'无所属部门'), ')') as display_info";
                    //var empQable = db.Queryable<daoben_hr_emp_job>()
                    //    .JoinTable<daoben_salary_benefit_emp>((a, b) => b.main_id == id && a.id == b.emp_id)
                    //    .Select(selectStr);
                    //string empList = empQable.Select(selectStr).OrderBy("entry_date desc").ToJson();                   
                    //

                    List<daoben_hr_emp_job> empList = db.Queryable<daoben_hr_emp_job>()
                        .JoinTable<daoben_salary_benefit_emp>((a, b) => a.id == b.emp_id)
                        .Where<daoben_salary_benefit_emp>((a, b) => b.main_id == benefitsInfo.id)
                        .ToList();

                    string creator_position_name = null;
                    daoben_hr_emp_job_history creatorInfo = db.Queryable<daoben_hr_emp_job_history>()
                        .Where(a => a.id == benefitsInfo.creator_job_history_id).Select("position_name").SingleOrDefault();
                    if (creatorInfo != null)
                        creator_position_name = creatorInfo.position_name;
                    object resultObj = new
                    {
                        benefitsInfo = benefitsInfo,
                        benefitslistInfo = benefitslistInfo,
                        approvelistInfo = ApprovelistInfo,
                        empList = empList,
                        creator_position_name = creator_position_name,
                    };
                    return resultObj.ToJson();
                }
                catch (Exception ex)
                {
                    return "系统出错：" + ex.Message;
                }
            }
        }
        internal string Delete(string id)
        {
            using (var db = SugarDao.GetInstance())
            {
                try
                {
                    daoben_salary_benefit mainInfo = db.Queryable<daoben_salary_benefit>().InSingle(id);
                    if (mainInfo == null || mainInfo.approve_status != 0)
                        //审批过后，将不能撤回
                        return "撤回失败：指定的申请信息不存在或者已被审批";
                    else
                    {
                        db.CommandTimeOut = 30;
                        db.BeginTran();
                        db.Delete<daoben_salary_benefit>(a => a.id == id);
                        db.Delete<daoben_salary_benefit_detail>(a => a.main_id == id);
                        db.Delete<daoben_salary_benefit_emp>(a => a.main_id == id);
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
                
                db.Update<daoben_salary_benefit>(upObj, a => a.id==id);
            }
        }

    }
}
