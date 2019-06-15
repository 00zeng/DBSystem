using Base.Code;
using MySqlSugar;
using ProjectShare.Database;
using ProjectShare.Models;
using ProjectShare.Process;
using System;
using System.Collections.Generic;
using System.Linq;
using ProjectWeb.Areas.SalaryCalculate.Application;

namespace ProjectWeb.Areas.FinancialAccounting.Application
{
    public class StaySubsidyApp
    {
        public object GetListPage(Pagination pagination, daoben_salary_staysubsidy queryInfo)
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
            pagination.sidx = string.IsNullOrEmpty(pagination.sidx) ? "month" : pagination.sidx;
            pagination.sord = string.IsNullOrEmpty(pagination.sord) ? "desc" : pagination.sord;
            using (var db = SugarDao.GetInstance())
            {
                //查看全部
                var qable = db.Queryable<daoben_salary_staysubsidy>();
                if (myCompanyInfo.category == "事业部")
                    qable.Where(a => (a.company_id == myCompanyInfo.id || a.company_id_parent == myCompanyInfo.id));
                else if (myCompanyInfo.category == "分公司")
                    qable.Where(a => a.company_id == myCompanyInfo.parentId);
                if (queryInfo != null)
                {
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
        public object GetListMyApprove(Pagination pagination)
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
                //我的审批
                var qable = db.Queryable<daoben_salary_staysubsidy>();
                if (LoginInfo.roleId != ConstData.ROLE_ID_ADMIN)
                {
                    if (LoginInfo.roleId == ConstData.ROLE_ID_FINANCIALMANAGER)
                        qable.Where(a => a.approve_status == 1 && a.company_id == myCompanyInfo.id);
                    else if (myPositionInfo.positionType == ConstData.POSITION_GM1)
                        qable.Where(a => a.approve_status == 0 && a.company_id == myCompanyInfo.id);
                    else
                        return null;
                }
                string listStr = qable.OrderBy(pagination.sidx + " " + pagination.sord)
                            .ToJsonPage(pagination.page, pagination.rows, ref records);
                pagination.records = records;
                if (string.IsNullOrEmpty(listStr) || listStr == "[]")
                    return null;
                return listStr.ToJson();
            }
        }
        public string Add(daoben_salary_staysubsidy addInfo, List<daoben_salary_staysubsidy_emp> listEmp)
        {
            if (listEmp == null || listEmp.Count == 0)
                return "信息错误：请至少选择一名员工";

            var LoginInfo = OperatorProvider.Provider.GetCurrent();
            if (LoginInfo == null)
                return "登录超时，请重新登录";
            addInfo.id = Common.GuId();
            foreach (var a in listEmp)
            {
                a.main_id = addInfo.id;
            }
            CompanyInfo myCompanyInfo = LoginInfo.companyInfo;
            PositionInfo myPositionInfo = LoginInfo.positionInfo;
            using (var db = SugarDao.GetInstance())
            {
                try
                {
                    if(addInfo.company_id == myCompanyInfo.id)
                    {
                        addInfo.company_linkname = myCompanyInfo.linkName;
                        addInfo.company_id_parent = myCompanyInfo.parentId;
                    }
                    else
                    {
                        var companyInfo = db.Queryable<daoben_org_company>().Where(a => a.id == addInfo.company_id)
                                .Select("parent_id, link_name").SingleOrDefault();
                        if (companyInfo == null)
                            return "数据错误：机构信息不存在";
                        addInfo.company_linkname = companyInfo.link_name;
                        addInfo.company_id_parent = companyInfo.parent_id;
                    }
                    addInfo.count = listEmp.Count();
                    addInfo.approve_status = 0;
                    addInfo.creator_job_history_id = LoginInfo.jobHistoryId;
                    addInfo.creator_name = LoginInfo.empName;
                    addInfo.create_time = DateTime.Now;
                    db.CommandTimeOut = 30;
                    db.BeginTran();
                    db.Insert(addInfo);
                    db.InsertRange(listEmp);
                    db.CommitTran();
                }
                catch (Exception ex)
                {
                    db.RollbackTran();
                    return ex.Message;
                }
                approveTemp(addInfo.id);
                return "success";
            }
        }
        public string Approve(daoben_salary_staysubsidy_approve approveInfo)
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
                    daoben_salary_staysubsidy mainInfo = db.Queryable<daoben_salary_staysubsidy>().InSingle(approveInfo.main_id);
                    if (mainInfo == null)
                        return "信息错误：指定的申请信息不存在";
                    if (LoginInfo.roleId == ConstData.ROLE_ID_FINANCIALMANAGER || LoginInfo.roleId == ConstData.ROLE_ID_FINANCIAL)
                    {
                        if (approveInfo.status > 0)
                            approveInfo.status = 100;
                        upObj = new { approve_status = 100 };
                        // 加入定时任务列表
                        PayrollSettingApp payRollApp = new PayrollSettingApp();
                        DateTime month = payRollApp.GetInfo(0).month;
                        cronInfo = new daoben_sys_cron { main_id = mainInfo.id, month = month, is_finished = false, category = 12, create_time = DateTime.Now };
                    }
                    else
                    {
                        if (approveInfo.status > 0)
                            upObj = new { approve_status = (1 + mainInfo.approve_status) };
                        else
                            upObj = new { approve_status = (0 - 1 - mainInfo.approve_status) };
                    }
                    approveInfo.approve_id = LoginInfo.accountId;
                    approveInfo.approve_name = LoginInfo.empName;
                    approveInfo.approve_time = DateTime.Now;
                    approveInfo.approve_position_id = myPositionInfo.id;
                    approveInfo.approve_position_name = myPositionInfo.name;

                    db.CommandTimeOut = 30;
                    db.BeginTran();
                    db.DisableInsertColumns = new string[] { "id" };

                    db.Update<daoben_salary_staysubsidy>(upObj, a => a.id == mainInfo.id);
                    db.Insert(approveInfo);
                    if (cronInfo != null)
                        db.Insert(cronInfo);
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
        public object GetInfo(string id)
        {
            OperatorModel LoginInfo = OperatorProvider.Provider.GetCurrent();
            if (LoginInfo == null)
                throw new Exception("用户登陆过期，请重新登录");
            if (string.IsNullOrEmpty(id))
                return null;
            CompanyInfo myCompanyInfo = LoginInfo.companyInfo;
            using (var db = SugarDao.GetInstance())
            {
                daoben_salary_staysubsidy mainInfo = db.Queryable<daoben_salary_staysubsidy>().InSingle(id);
                if (mainInfo == null)
                    return null;
                List<daoben_salary_staysubsidy_emp> listEmp = db.Queryable<daoben_salary_staysubsidy_emp>()
                    .Where(a => a.main_id == mainInfo.id).ToList();
                List<daoben_salary_staysubsidy_approve> approveList = db.Queryable<daoben_salary_staysubsidy_approve>()
                            .Where(a => a.main_id == id).ToList();
                object retObj = new
                {
                    mainInfo = mainInfo,
                    listEmp = listEmp,
                    approveList = approveList,
                };
                return retObj;
            }
        }

        internal string Delete(string id)
        {
            using (var db = SugarDao.GetInstance())
            {
                try
                {
                    daoben_salary_staysubsidy mainInfo = db.Queryable<daoben_salary_staysubsidy>().InSingle(id);
                    if (mainInfo == null || mainInfo.approve_status != 0)
                        //审批过后，将不能撤回
                        return "撤回失败：指定的申请信息不存在或者已被审批";
                    else
                    {
                        db.CommandTimeOut = 30;
                        db.BeginTran();
                        db.Delete<daoben_salary_staysubsidy_emp>(a => a.main_id == id);
                        db.Delete<daoben_salary_staysubsidy>(a => a.id == id);
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

                db.Update<daoben_salary_staysubsidy>(upObj, a => a.id == id);
            }
        }
    }
}
