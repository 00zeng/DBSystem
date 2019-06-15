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
using ProjectWeb.Areas.SalaryCalculate.Application;

namespace ProjectWeb.Areas.FinancialAccounting.Application
{
    public class OfficeKPIApp
    {

        /// <summary>
        /// 行政KPI历史
        /// </summary>
        /// <param name="pagination"></param>
        /// <param name="queryInfo"></param>
        /// <returns></returns>
        public object GetListHistory(Pagination pagination, daoben_hr_emp_job queryInfo, int approve_status, QueryTime queryTime)
        {
            var LoginInfo = OperatorProvider.Provider.GetCurrent();
            if (LoginInfo == null)
                return null;
            int records = 0;
            if (pagination == null)
                pagination = new Pagination();
            pagination.page = pagination.page > 0 ? pagination.page : 1;
            pagination.rows = pagination.rows > 0 ? pagination.rows : 20;
            pagination.sidx = string.IsNullOrEmpty(pagination.sidx) ? "a.month" : pagination.sidx;
            pagination.sord = string.IsNullOrEmpty(pagination.sord) ? "desc" : pagination.sord;
            CompanyInfo myCompanyInfo = LoginInfo.companyInfo;
            PositionInfo myPositionInfo = LoginInfo.positionInfo;
            using (var db = SugarDao.GetInstance())
            {
                var qable = db.Queryable<daoben_salary_kpi>()
                            .JoinTable<daoben_hr_emp_job>((a, b) => a.emp_id == b.id);
                if (LoginInfo.roleId != ConstData.ROLE_ID_ADMIN)
                {
                    if (LoginInfo.roleId == ConstData.ROLE_ID_FINANCIAL || LoginInfo.roleId == ConstData.ROLE_ID_FINANCIALMANAGER)
                    {
                        qable.Where(a => (a.company_id_parent == myCompanyInfo.id || a.company_id == myCompanyInfo.id));
                    }
                    else
                    {
                        if (myPositionInfo.positionType == ConstData.POSITION_GM1 || myPositionInfo.positionType == ConstData.POSITION_GM_ASSISTANT1)   // 事业部
                            qable.Where(a => (a.company_id_parent == myCompanyInfo.id || a.company_id == myCompanyInfo.id));
                        else if (myPositionInfo.positionType == ConstData.POSITION_GM2 || myPositionInfo.positionType == ConstData.POSITION_GM_ASSISTANT2)   // 分公司
                            qable.Where(a => a.company_id == myCompanyInfo.id);
                        else if (myPositionInfo.positionType == ConstData.POSITION_DEPT_M)
                            qable.Where(a => a.dept_id == myPositionInfo.deptId);
                        else
                            return null;
                    }
                }
                //姓名 机构 部门 职位名 月份 * 2 审批状态
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
                }
                if (queryTime != null)
                {
                    if (queryTime.startTime1 != null)
                        qable.Where(a => a.month.ToInt() >= queryTime.startTime1.ToDate().Month);
                    if (queryTime.startTime2 != null)
                        qable.Where(a => a.month.ToInt() <= queryTime.startTime2.ToDate().Month);
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

                string listStr = qable
                    .OrderBy(pagination.sidx + " " + pagination.sord)
                    .Select("a.*,b.entry_date as entry_date,b.work_number as work_number")
                    .ToJsonPage(pagination.page, pagination.rows, ref records);
                pagination.records = records;
                if (string.IsNullOrEmpty(listStr) || listStr == "[]")
                    return null;
                return listStr.ToJson();
            }
        }
        /// <summary>
        /// 行政KPI审批
        /// </summary>
        /// <param name="pagination"></param>
        /// <param name="queryName"></param>
        /// <returns></returns>
        public object GetListApprove(Pagination pagination, daoben_hr_emp_job queryInfo, QueryTime queryTime)
        {
            var LoginInfo = OperatorProvider.Provider.GetCurrent();
            if (LoginInfo == null)
                return null;
            int records = 0;
            if (pagination == null)
                pagination = new Pagination();
            pagination.page = pagination.page > 0 ? pagination.page : 1;
            pagination.rows = pagination.rows > 0 ? pagination.rows : 20;
            pagination.sidx = string.IsNullOrEmpty(pagination.sidx) ? "a.create_time" : pagination.sidx;
            pagination.sord = string.IsNullOrEmpty(pagination.sord) ? "desc" : pagination.sord;
            CompanyInfo myCompanyInfo = LoginInfo.companyInfo;
            PositionInfo myPositionInfo = LoginInfo.positionInfo;
            using (var db = SugarDao.GetInstance())
            {
                var qable = db.Queryable<daoben_salary_kpi>()
                            .JoinTable<daoben_hr_emp_job>((a, b) => a.emp_id == b.id);
                if (LoginInfo.roleId != ConstData.ROLE_ID_ADMIN)
                    if (LoginInfo.roleId == ConstData.ROLE_ID_FINANCIAL || LoginInfo.roleId == ConstData.ROLE_ID_FINANCIALMANAGER)
                    {
                        //财务部
                        qable.Where(a => (a.position_type != 0 && a.approve_status == 0)
                                    || (a.position_type == 0 && a.approve_status == 1));
                        qable.Where(a => a.company_id == myCompanyInfo.id || a.company_id_parent == myCompanyInfo.id);
                    }
                    else if (myPositionInfo.positionType == ConstData.POSITION_GM1)
                    {
                        //事业部总经理:审批普通事业部员工 + 分公司培训师
                        qable.Where(a => (a.company_id == myCompanyInfo.id && a.position_type == ConstData.POSITION_OFFICE_NORMAL)
                                    || (a.company_id_parent == myCompanyInfo.id && a.category == 2));
                        qable.Where(a => a.approve_status == 0);
                    }
                    else if (myPositionInfo.positionType == ConstData.POSITION_GM2)
                    {
                        //分公司经理:审批普通分公司员工
                        qable.Where(a => a.company_id == myCompanyInfo.id);
                        qable.Where(a => a.position_type == ConstData.POSITION_OFFICE_NORMAL && a.approve_status == 0 && a.category == 1);
                    }
                    else
                        return null;
                //姓名 机构 部门 职位名 月份 * 2
                if (!string.IsNullOrEmpty(queryInfo.name))
                    qable.Where(a => a.emp_name.Contains(queryInfo.name));
               if(queryInfo!=null)
                {
                    if (queryInfo.company_id > 0)
                        qable.Where(a => a.company_id == queryInfo.company_id);
                    if (queryInfo.dept_id > 0)
                        qable.Where(a => a.dept_id == queryInfo.dept_id);
                    if (queryInfo.position_id > 0)
                        qable.Where(a => a.position_id == queryInfo.position_id);
                }               
                if (queryTime != null)
                {
                    if (queryTime.startTime1 != null)
                        qable.Where(a => a.month.ToInt() >= queryTime.startTime1.ToDate().Month);
                    if (queryTime.startTime2 != null)
                        qable.Where(a => a.month.ToInt() <= queryTime.startTime2.ToDate().Month);
                }               
                string listStr = qable
                    .OrderBy(pagination.sidx + " " + pagination.sord)
                    .Select("a.*,b.entry_date as entry_date,b.work_number as work_number")
                    .ToJsonPage(pagination.page, pagination.rows, ref records);
                pagination.records = records;
                if (string.IsNullOrEmpty(listStr) || listStr == "[]")
                    return null;
                return listStr.ToJson();
            }
        }

        /// <summary>
        /// KPI评分
        /// </summary>
        /// <param name="pagination"></param>
        /// <param name="queryName"></param>
        /// <returns></returns>
        public object GetList(Pagination pagination, daoben_hr_emp_job queryInfo, QueryTime queryTime, bool showAll)
        {
            var LoginInfo = OperatorProvider.Provider.GetCurrent();
            if (LoginInfo == null)
                return null;
            int records = 0;
            if (pagination == null)
                pagination = new Pagination();
            pagination.page = pagination.page > 0 ? pagination.page : 1;
            pagination.rows = pagination.rows > 0 ? pagination.rows : 20;
            pagination.sidx = string.IsNullOrEmpty(pagination.sidx) ? "a.entry_date" : pagination.sidx;
            pagination.sord = string.IsNullOrEmpty(pagination.sord) ? "desc" : pagination.sord;
            CompanyInfo myCompanyInfo = LoginInfo.companyInfo;
            PositionInfo myPositionInfo = LoginInfo.positionInfo;
            using (var db = SugarDao.GetInstance())
            {
                // 获取工资结算时间
                daoben_payroll_setting payrollMonth = new PayrollSettingApp().GetPayrollMonth(myCompanyInfo.id, db, true);
                if (payrollMonth == null)
                    return "系统出错：工资结算周期信息不存在";
                DateTime curPayrollMonth = payrollMonth.month; // 实际是结算上一月

                var qable = db.Queryable<daoben_hr_emp_job>()
                        .JoinTable<daoben_salary_kpi>((a, b) => a.id == b.emp_id && b.month == curPayrollMonth)
                        .Where(a => a.position_type <= ConstData.POSITION_OFFICE_NORMAL);
                if (!showAll)
                    qable.Where<daoben_kpi_sales>((a, b) => b.id == null && a.entry_date < curPayrollMonth.AddDays(16));

                if (LoginInfo.roleId != ConstData.ROLE_ID_ADMIN)
                {
                    if (LoginInfo.roleId == ConstData.ROLE_ID_FINANCIALMANAGER)
                        qable.Where(a => (a.company_id == myCompanyInfo.id || a.company_id_parent == myCompanyInfo.id));
                    else if (LoginInfo.roleId == ConstData.ROLE_ID_TERMINALMANAGER)//终端经理-培训
                        qable.Where(a => a.company_id == myCompanyInfo.id && a.position_type == ConstData.POSITION_TRAINER);
                    else
                    {
                        if (myPositionInfo.positionType == ConstData.POSITION_GM1)
                        {
                            //事业部经理-分公司经理（助理）+事业部部门经理
                            qable.Where(a => a.position_type < ConstData.POSITION_OFFICE_NORMAL
                                        && (a.company_id == myCompanyInfo.id || a.company_id_parent == myCompanyInfo.id));
                        }
                        else if (myPositionInfo.positionType == ConstData.POSITION_GM2)
                        {
                            //分公司经理-分公司总经理助理，部门经理
                            qable.Where(a => a.position_type >= ConstData.POSITION_GM_ASSISTANT2 && a.position_type < ConstData.POSITION_OFFICE_NORMAL
                                        && a.company_id == myCompanyInfo.id);
                        }
                        else if (myPositionInfo.positionType == ConstData.POSITION_DEPT_M)
                            qable.Where(a => a.dept_id == myPositionInfo.deptId && a.position_type == ConstData.POSITION_OFFICE_NORMAL);
                        else
                            return null;
                    }
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
                }
                if(queryTime!=null)
                {
                    if (queryTime.startTime1 != null)
                        qable.Where(a => a.entry_date >= queryTime.startTime1);
                    if (queryTime.startTime2 != null)
                    {
                        queryTime.startTime2 = queryTime.startTime2.ToDate().AddDays(1);
                        qable.Where(a => a.entry_date < queryTime.startTime2);
                    }
                }
               
                string listStr = qable
                    .OrderBy(pagination.sidx + " " + pagination.sord)
                    .Select("a.id,a.name,a.work_number,a.position_name,a.grade,a.dept_name,a.company_linkname,a.emp_category,a.entry_date,a.status,b.id as calculated")
                    .ToJsonPage(pagination.page, pagination.rows, ref records);
                pagination.records = records;
                if (string.IsNullOrEmpty(listStr) || listStr == "[]")
                    return null;
                return listStr.ToJson();
            }
        }
        public string Add(daoben_salary_kpi addInfo)
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
                    addInfo.id = Common.GuId();
                    daoben_hr_emp_job jobInfo = db.Queryable<daoben_hr_emp_job>().InSingle(addInfo.emp_id);
                    if (jobInfo == null)
                        return "信息错误：指定的员工信息不存在";
                    if (addInfo.month == null)
                        return "信息错误：需要指定考核月份";
                    addInfo.emp_name = jobInfo.name;
                    addInfo.work_number = jobInfo.work_number;
                    addInfo.emp_category = jobInfo.emp_category;
                    addInfo.position_id = jobInfo.position_id;
                    addInfo.position_name = jobInfo.position_name;
                    addInfo.position_type = jobInfo.position_type;
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
                    if (jobInfo.position_type <= ConstData.POSITION_OFFICE_NORMAL)
                        addInfo.category = 1;
                    else if (jobInfo.position_type == ConstData.POSITION_TRAINERMANAGER)
                        addInfo.category = 3;
                    else if (jobInfo.position_type == ConstData.POSITION_TRAINER)
                        addInfo.category = 2;
                    else
                        return "信息错误：错误的KPI评分类型";
                    addInfo.approve_status = 100;
                    //不用审批了
                    //TODO 开始周期结束周期
                    addInfo.creator_job_history_id = LoginInfo.jobHistoryId;
                    addInfo.creator_name = LoginInfo.empName;
                    addInfo.create_time = DateTime.Now;
                    List<daoben_salary_kpi> origInfoList = db.Queryable<daoben_salary_kpi>()
                        .Where(a => a.emp_id == addInfo.emp_id && a.month == addInfo.month).ToList();
                    if (origInfoList != null && origInfoList.Count() > 0)
                    {
                        List<string> idList = origInfoList.Select(t => t.id).ToList();
                        db.Delete<daoben_salary_kpi>(a => idList.Contains(a.id) == true);
                    }
                    db.Insert(addInfo);


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
        public string Approve(daoben_salary_kpi_approve approveInfo)
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
                    daoben_salary_kpi origInfo = db.Queryable<daoben_salary_kpi>().InSingle(approveInfo.main_id);
                    if (origInfo == null)
                        return "信息错误：指定的申请信息不存在";
                    if ((LoginInfo.roleId == ConstData.ROLE_ID_FINANCIAL || LoginInfo.roleId == ConstData.ROLE_ID_FINANCIALMANAGER) && approveInfo.status > 0)
                        origInfo.approve_status = 100;
                    else
                    {
                        if (approveInfo.status > 0)
                            origInfo.approve_status = 0 + 1 + origInfo.approve_status;
                        else
                            origInfo.approve_status = 0 - 1 - origInfo.approve_status;
                    }
                    approveInfo.approve_id = LoginInfo.accountId;
                    approveInfo.approve_name = LoginInfo.empName;
                    approveInfo.approve_time = DateTime.Now;
                    approveInfo.approve_position_id = myPositionInfo.id;
                    approveInfo.approve_position_name = myPositionInfo.name;

                    db.CommandTimeOut = 30;
                    db.BeginTran();

                    db.Update<daoben_salary_kpi>(new { approve_status = origInfo.approve_status, }, a => a.id == origInfo.id);
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

        /// <summary>
        /// 获取KPI补贴信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public string GetKpiSubInfo(string id)
        {
            var LoginInfo = OperatorProvider.Provider.GetCurrent();
            if (LoginInfo == null)
                return "登录超时，请重新登录";
            using (var db = SugarDao.GetInstance())
            {
                try
                {
                    daoben_hr_emp_job jobInfo = db.Queryable<daoben_hr_emp_job>().InSingle(id);
                    if (jobInfo == null)
                        return "信息错误：指定的员工信息不存在";
                    daoben_salary_emp salaryInfo = db.Queryable<daoben_salary_emp>().SingleOrDefault(a => a.emp_id == id);
                    daoben_salary_emp_kpi_subsidy kpiSubInfo = new daoben_salary_emp_kpi_subsidy();
                    if (!string.IsNullOrEmpty(salaryInfo.id))
                        kpiSubInfo = db.Queryable<daoben_salary_emp_kpi_subsidy>().SingleOrDefault(a => a.main_id == salaryInfo.id);
                    object resultObj = new
                    {
                        jobInfo = jobInfo,
                        kpiSubInfo = kpiSubInfo
                    };
                    return resultObj.ToJson();

                }
                catch (Exception ex)
                {
                    return ex.Message;
                }
            }
        }
        /// <summary>
        /// 获取kpi设置信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public string GetInfo(string id)
        {
            var LoginInfo = OperatorProvider.Provider.GetCurrent();
            if (LoginInfo == null)
                return "登录超时，请重新登录";
            using (var db = SugarDao.GetInstance())
            {
                try
                {
                    daoben_salary_kpi kpiscoreInfo = db.Queryable<daoben_salary_kpi>().InSingle(id);
                    if (kpiscoreInfo == null)
                        return "信息错误：指定的KPI设置信息不存在";
                    List<daoben_salary_kpi_approve> ApprovelistInfo = db.Queryable<daoben_salary_kpi_approve>()
                        .Where(a => a.main_id == id).ToList();
                    object resultObj = new
                    {
                        kpiscoreInfo = kpiscoreInfo,
                        ApprovelistInfo = ApprovelistInfo
                    };
                    return resultObj.ToJson();

                }
                catch (Exception ex)
                {
                    return ex.Message;
                }
            }
        }


        public string GetOrigInfo(string empId, DateTime month)
        {
            var LoginInfo = OperatorProvider.Provider.GetCurrent();
            if (LoginInfo == null)
                return "登录超时，请重新登录";
            using (var db = SugarDao.GetInstance())
            {
                try
                {
                    daoben_salary_kpi kpiInfo = db.Queryable<daoben_salary_kpi>()
                        .Where(a => a.emp_id == empId && a.month == month).SingleOrDefault();
                    if (kpiInfo != null)
                        return kpiInfo.ToJson();
                    else return null;
                }
                catch (Exception ex)
                {
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
                    daoben_salary_kpi mainInfo = db.Queryable<daoben_salary_kpi>().SingleOrDefault(a => a.id == id);
                    if (mainInfo == null || mainInfo.approve_status != 0)
                        //审批过后，将不能撤回
                        return "撤回失败：指定的申请信息不存在或者已被审批";
                    else
                    {
                        db.CommandTimeOut = 30;
                        db.BeginTran();
                        db.Delete<daoben_salary_kpi>(a => a.id == id);
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

                db.Update<daoben_salary_kpi>(upObj, a => a.id == id);
            }
        }

        /// <summary>
        /// 获取月份信息，用于评分页面
        /// </summary>
        /// <param name="id">员工ID</param>
        /// <returns></returns>
        public string GetSettingInfo(string id)
        {
            OperatorModel LoginInfo = OperatorProvider.Provider.GetCurrent();
            if (LoginInfo == null)
                throw new Exception("用户登陆过期，请重新登录");
            if (string.IsNullOrEmpty(id))
                return "信息错误：ID不能为空";
            CompanyInfo myCompanyInfo = LoginInfo.companyInfo;
            if (myCompanyInfo.category != "事业部" && myCompanyInfo.category != "分公司")
                return "权限错误：KPI需由事业部或分公司人员操作";
            using (var db = SugarDao.GetInstance())
            {
                daoben_hr_emp_job empInfo = db.Queryable<daoben_hr_emp_job>().SingleOrDefault(a => a.id == id);
                if (empInfo == null)
                    return "系统出错：员工信息不存在";

                // 获取工资结算时间
                daoben_payroll_setting payrollMonth = new PayrollSettingApp().GetPayrollMonth(myCompanyInfo.id, db, true);
                if (payrollMonth == null)
                    return "系统出错：工资结算周期信息不存在";
                if (empInfo.entry_date > payrollMonth.month.AddDays(15))
                    return "该员工在本月15之后入职，本月无工资";
                PayrollSalesApp salesApp = new PayrollSalesApp();

                object retObj = new
                {
                    empInfo = empInfo,
                    month = payrollMonth.month,
                };
                return retObj.ToJson();
            }

        }
    }
}
