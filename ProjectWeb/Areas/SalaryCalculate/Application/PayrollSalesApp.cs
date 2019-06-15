using Base.Code;
using MySqlSugar;
using NPOI.HSSF.UserModel;
using ProjectShare.Database;
using ProjectShare.Models;
using ProjectShare.Process;
using ProjectWeb.Application;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace ProjectWeb.Areas.SalaryCalculate.Application
{
    public class PayrollSalesApp
    {
        public object GetListHistory(Pagination pagination, daoben_hr_emp_job queryInfo)
        {
            OperatorModel LoginInfo = OperatorProvider.Provider.GetCurrent();
            if (LoginInfo == null)
                throw new Exception("用户登陆过期，请重新登录");

            int records = 0;
            if (pagination == null)
                pagination = new Pagination();
            pagination.page = pagination.page > 0 ? pagination.page : 1;
            pagination.rows = pagination.rows > 0 ? pagination.rows : 20;
            pagination.sidx = string.IsNullOrEmpty(pagination.sidx) ? "month" : pagination.sidx;
            pagination.sord = string.IsNullOrEmpty(pagination.sord) ? "desc" : pagination.sord;
            CompanyInfo myCompanyInfo = LoginInfo.companyInfo;
            using (var db = SugarDao.GetInstance())
            {
                var qable = db.Queryable<daoben_payroll_sales>()
                            .JoinTable<daoben_hr_emp_job>((a, b) => b.id == a.emp_id)
                            .JoinTable<daoben_payroll_sales_sub>((a, c) => a.id == c.main_id);

                if (!string.IsNullOrEmpty(queryInfo.name))
                {
                    qable.Where<daoben_hr_emp_job>((a, b) => b.name.Contains(queryInfo.name));
                }
                if (!string.IsNullOrEmpty(queryInfo.position_name))
                {
                    qable.Where<daoben_hr_emp_job>((a, b) => b.position_name.Contains(queryInfo.position_name));
                }
                if (queryInfo.company_id > 0)
                {
                    qable.Where<daoben_hr_emp_job>((a, b) => b.company_id == queryInfo.company_id);
                }
                string list = qable
                            .Select("SQL_CALC_FOUND_ROWS SUM(c.amount) AS sub_amount,a.id,a.name,a.month,a.base_salary,a.position_salary,a.house_subsidy,a.attendance_reward,a.seniority_salary,a.traffic_subsidy,a.salary,a.insurance_fee,a.resign_deposit,a.leaving_deduction,a.proxy_subsidy,a.actual_salary,a.note,b.name as emp_name,b.work_number,b.name_v2,b.position_name as position_name,b.area_l1_name,b.area_l2_name,b.grade as grade,b.dept_name,b.company_linkname,b.emp_category,b.entry_date as entry_date,b.status")
                            .GroupBy("a.id")
                            .OrderBy(pagination.sidx + " " + pagination.sord)
                            .ToJsonPage(pagination.page, pagination.rows, ref records);
                pagination.records = db.SqlQuery<int>("SELECT FOUND_ROWS() as count").FirstOrDefault();
                if (string.IsNullOrEmpty(list) || list == "[]")
                    return null;
                return list.ToJson();
            }
        }
        /// <summary>
        /// 导出
        /// </summary>
        public MemoryStream ExportExcel(Pagination pagination)
        {
            OperatorModel LoginInfo = OperatorProvider.Provider.GetCurrent();
            if (LoginInfo == null)
                throw new Exception("用户登陆过期，请重新登录");

            if (pagination == null)
                pagination = new Pagination();
            pagination.sidx = string.IsNullOrEmpty(pagination.sidx) ? "month" : pagination.sidx;
            pagination.sord = string.IsNullOrEmpty(pagination.sord) ? "desc" : pagination.sord;
            CompanyInfo myCompanyInfo = LoginInfo.companyInfo;
            using (var db = SugarDao.GetInstance())
            {
                var qable = db.Queryable<daoben_payroll_sales>()
                             .JoinTable<daoben_hr_emp_job>((a, b) => b.id == a.emp_id)
                             .JoinTable<daoben_payroll_sales_sub>((a, c) => a.id == c.main_id);

                //工资结算-业务      
                string str = "b.name,b.work_number,DATE_FORMAT(a.month,'%Y-%m'),b.position_name,b.grade,DATE_FORMAT(b.entry_date,'%Y-%m-%d'),a.base_salary,a.position_salary,a.house_subsidy,a.attendance_reward,a.seniority_salary,a.traffic_subsidy,a.salary,a.insurance_fee,a.resign_deposit,a.leaving_deduction,a.proxy_subsidy,a.actual_salary,a.note";
                var listDt = qable.OrderBy(pagination.sidx + " " + pagination.sord)
                    .GroupBy("a.id")
                    .Select(str)
                    .ToDataTable();
                int rowsCount = listDt.Rows.Count;
                string[] headerArr = new string[]
                {"姓名","工号","结算月份","岗位","职等","入职时间","基本工资","岗位工资","住房补助","全勤奖","工龄工资","交通补贴","应发工资",
            "扣社保","扣风险金","请假扣款","区域经理代管补助","实发金额","备注",
                };
                int[] colWidthArr = new int[] { 15, 15, 15, 15, 15, 15, 15, 15, 15, 15, 15, 15, 15, 15, 15, 15, 16, 15, 25 };

                //建立空白工作簿
                HSSFWorkbook book = new HSSFWorkbook();
                if (rowsCount < ConstData.EXPORT_SHEET_LEN)
                    ExcelApi.ExcelExport(book, "sheet1", headerArr, colWidthArr, listDt, 0, rowsCount);
                else
                {
                    int page = rowsCount / ConstData.EXPORT_SHEET_LEN;
                    for (int i = 0; i < page; i++)
                    {
                        int start = i * ConstData.EXPORT_SHEET_LEN;
                        int end = (i * ConstData.EXPORT_SHEET_LEN) + ConstData.EXPORT_SHEET_LEN - 1;
                        string sheetName = "sheet" + (i + 1).ToString();

                        ExcelApi.ExcelExport(book, sheetName, headerArr, colWidthArr, listDt, start, end);
                    }
                    int lastCount = rowsCount % ConstData.EXPORT_SHEET_LEN;
                    string lastName = "sheet" + (page + 1).ToString();

                    ExcelApi.ExcelExport(book, lastName, headerArr, colWidthArr, listDt, rowsCount - lastCount, lastCount);
                }

                // 写入到客户端 
                MemoryStream ms = new MemoryStream();
                {
                    book.Write(ms);
                    ms.Seek(0, SeekOrigin.Begin);
                    return ms;
                }
            }
        }
        public object GetListApprove(Pagination pagination)
        {
            OperatorModel LoginInfo = OperatorProvider.Provider.GetCurrent();
            if (LoginInfo == null)
                throw new Exception("用户登陆过期，请重新登录");

            int records = 0;
            if (pagination == null)
                pagination = new Pagination();
            pagination.page = pagination.page > 0 ? pagination.page : 1;
            pagination.rows = pagination.rows > 0 ? pagination.rows : 20;
            pagination.sidx = string.IsNullOrEmpty(pagination.sidx) ? "month" : pagination.sidx;
            pagination.sord = string.IsNullOrEmpty(pagination.sord) ? "desc" : pagination.sord;
            CompanyInfo myCompanyInfo = LoginInfo.companyInfo;
            using (var db = SugarDao.GetInstance())
            {
                string listStr = db.Queryable<daoben_payroll_sales>()
                            .JoinTable<daoben_hr_emp_job>((a, b) => b.id == a.emp_id)
                            .JoinTable<daoben_payroll_sales_sub>((a, c) => a.id == c.main_id)
                            .Select("SUM(c.amount) AS sub_amount,a.*,b.name as emp_name,b.work_number,b.name_v2,b.position_name,b.area_l1_name,b.area_l2_name,b.grade,b.dept_name,b.company_linkname,b.emp_category,b.entry_date,b.status")
                            .GroupBy("a.id")
                            .OrderBy(pagination.sidx + " " + pagination.sord)
                            .ToJsonPage(pagination.page, pagination.rows, ref records);
                pagination.records = records;
                if (string.IsNullOrEmpty(listStr) || listStr == "[]")
                    return null;
                return listStr.ToJson();
            }
        }

        public object GetListCalculate(Pagination pagination, daoben_hr_emp_job queryInfo, bool showAll)
        {
            OperatorModel LoginInfo = OperatorProvider.Provider.GetCurrent();
            if (LoginInfo == null)
                throw new Exception("用户登陆过期，请重新登录");

            int records = 0;
            if (pagination == null)
                pagination = new Pagination();
            pagination.page = pagination.page > 0 ? pagination.page : 1;
            pagination.rows = pagination.rows > 0 ? pagination.rows : 20;
            pagination.sidx = string.IsNullOrEmpty(pagination.sidx) ? "a.company_id" : pagination.sidx;
            pagination.sord = string.IsNullOrEmpty(pagination.sord) ? "desc" : pagination.sord;
            CompanyInfo myCompanyInfo = LoginInfo.companyInfo;
            PositionInfo myPositionInfo = LoginInfo.positionInfo;
            using (var db = SugarDao.GetInstance())
            {
                // 获取工资结算时间，实际是结算上一月
                daoben_payroll_setting payrollMonth = new PayrollSettingApp().GetPayrollMonth(myCompanyInfo.id, db, true);
                if (payrollMonth == null)
                    return "系统出错：工资结算周期信息不存在";
                DateTime curPayrollMonth = payrollMonth.month;

                var qable = db.Queryable<daoben_hr_emp_job>()
                        .JoinTable<daoben_payroll_sales>((a, b) => a.id == b.emp_id && b.month == curPayrollMonth)
                        .Where(a => (a.position_type == ConstData.POSITION_SALES || a.position_type == ConstData.POSITION_SALESMANAGER));
                if (myCompanyInfo.category == "事业部")
                    qable.Where(a => a.company_id_parent == myCompanyInfo.id);
                else if (myCompanyInfo.category == "分公司")
                    qable.Where(a => a.company_id == myCompanyInfo.id);

                if (!showAll)
                    qable.Where<daoben_payroll_sales>((a, b) => b.id == null && a.entry_date < curPayrollMonth.AddDays(16));
                if (queryInfo != null)
                {
                    if (!string.IsNullOrEmpty(queryInfo.name))
                        qable.Where(a => a.name.Contains(queryInfo.name));
                    if (!string.IsNullOrEmpty(queryInfo.position_name))
                        qable.Where(a => a.position_name.Contains(queryInfo.position_name));
                    if (queryInfo.company_id != 0)
                        qable.Where(a => a.company_id == queryInfo.company_id);
                    if (queryInfo.dept_id != 0)
                        qable.Where(a => a.dept_id == queryInfo.dept_id);
                }

                string listStr = qable.OrderBy(pagination.sidx + " " + pagination.sord)
                        .Select("a.id,a.name,a.name_v2,a.work_number,a.position_name,a.grade,a.dept_name,a.company_name,a.emp_category,a.entry_date,a.status,b.id as calculated")
                        .ToJsonPage(pagination.page, pagination.rows, ref records);
                pagination.records = records;
                if (string.IsNullOrEmpty(listStr) || listStr == "[]")
                    return null;
                return listStr.ToJson();

            }
        }
        /// <summary>
        /// 获取工资信息，用于结算页面  yewuyuan
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
            PositionInfo myPositionInfo = LoginInfo.positionInfo;
            if (myCompanyInfo.category != "事业部")
                return "权限错误：工资结算需由事业部人员操作";

            int companyId = myCompanyInfo.id;
            DateTime now = DateTime.Now.Date;
            DateTime startTime, endTime;

            using (var db = SugarDao.GetInstance())
            {
                daoben_hr_emp_job empInfo = db.Queryable<daoben_hr_emp_job>().SingleOrDefault(a => a.id == id);
                if (empInfo == null)
                    return "系统出错：员工信息不存在";
                // 获取工资结算时间
                daoben_payroll_setting payrollMonth = new PayrollSettingApp().GetPayrollMonth(companyId, db, true);
                if (payrollMonth == null)
                    return "系统出错：工资结算周期信息不存在";
                if (empInfo.entry_date > payrollMonth.month.AddDays(15))
                    return "该员工在本月15之后入职，本月无工资";
                daoben_payroll_sales payrollInfo = new daoben_payroll_sales
                {
                    emp_id = empInfo.id,
                    month = payrollMonth.month,
                    start_date = payrollMonth.start_date,
                    end_date = (DateTime)payrollMonth.end_date,
                    create_time = now,
                    creator_job_history_id = LoginInfo.jobHistoryId,
                    creator_name = LoginInfo.empName,
                    approve_status = 0,
                    paid_status = -1,
                    company_id = empInfo.company_id,
                    company_id_parent = empInfo.company_id_parent,
                    actual_salary = 0,
                    company_name = empInfo.company_name,
                    entry_date = empInfo.entry_date.ToDate(),
                    grade = empInfo.grade,
                    name = empInfo.name,
                    position_id = empInfo.position_id,
                    position_name = empInfo.position_name,
                    deposit_total = 0,//个人薪资
                };
                List<daoben_payroll_sales_sub> totalSub = new List<daoben_payroll_sales_sub>();

                payrollInfo.name = empInfo.name;
                startTime = payrollInfo.start_date;
                endTime = payrollInfo.end_date.Date.AddDays(1);

                daoben_sale_month_emp totalInfo = new daoben_sale_month_emp();
                #region 个人专有工资信息
                daoben_salary_emp_sales seGeneral = db.Queryable<daoben_salary_emp_sales>()
                        .JoinTable<daoben_salary_emp>((a, b) => b.id == a.salary_position_id)
                        .Where<daoben_salary_emp>((a, b) => b.emp_id == empInfo.id && b.effect_status == 1 && b.category == 4)
                        .Select("a.resign_deposit,a.traffic_subsidy,a.intern_salary_type,a.intern_fix_salary,a.intern_ratio_salary")
                        .SingleOrDefault();
                if (seGeneral == null)   // 无个人专有的工资信息
                {
                    seGeneral = db.Queryable<daoben_salary_position_other>()
                            .JoinTable<daoben_salary_position>((a, b) => a.salary_position_id == b.id)
                            .Where<daoben_salary_position>((a, b) => b.company_id == empInfo.company_id && b.effect_status == 1 && b.category == 3)
                            .Select<daoben_salary_emp_sales>("a.salary_position_id, a.resign_deposit,a.intern_salary_type,a.intern_fix_salary,a.intern_ratio_salary")
                            .SingleOrDefault();
                    if (seGeneral == null)
                        return "noSalesSalarySetting";
                    seGeneral.traffic_subsidy = db.Queryable<daoben_salary_position_traffic>()
                            .JoinTable<daoben_salary_position>((a, b) => a.salary_position_id == b.id)
                            .Where<daoben_salary_position>((a, b) => b.position_id == empInfo.position_id && b.effect_status == 1
                            && b.category == empInfo.position_type) // category 与 jobInfo.position_type 相对应（仅业务员/业务经理）
                            .Select<decimal>("traffic_subsidy").SingleOrDefault();
                }
                payrollInfo.resign_deposit = seGeneral.resign_deposit < 0 ? seGeneral.resign_deposit : 0 - seGeneral.resign_deposit;
                payrollInfo.traffic_subsidy = seGeneral.traffic_subsidy;
                if (empInfo.emp_category == "实习生" && seGeneral.intern_salary_type == 2) // 实习生固定工资
                {
                    payrollInfo.intern_salary_type = seGeneral.intern_salary_type;
                    payrollInfo.intern_salary = payrollInfo.salary = seGeneral.intern_fix_salary;
                }
                else if (empInfo.emp_category == "实习生")
                {
                    payrollInfo.intern_salary_type = 1;
                    payrollInfo.intern_salary = seGeneral.intern_ratio_salary; //实习工资按比例发放，结算前占用该字段存放比例
                }
                //  payrollInfo.salary += payrollInfo.resign_deposit;
                payrollInfo.salary += (payrollInfo.traffic_subsidy > 0 ? payrollInfo.traffic_subsidy : 0);
                #endregion


                #region 福利、奖罚
                GetEmpBenefitList(empInfo.id, totalSub, payrollInfo.month, db);
                GetEmpRewardList(empInfo.id, totalSub, payrollInfo.month, db);
                #endregion

                #region 职等薪资
                int gradeCagetory = 0;
                if (empInfo.dept_id == 0)
                    gradeCagetory = 1; // 无部门时，默认为 1-行政管理
                else
                    gradeCagetory = db.Queryable<daoben_org_dept>()
                            .Where(a => a.id == empInfo.dept_id).Select(a => a.grade_category).SingleOrDefault();
                if (gradeCagetory == 0)
                    return "信息错误：该员工职等类型有误，请联系系统管理员";

                daoben_salary_position_grade spGrade = db.Queryable<daoben_salary_position_grade>()
                            .JoinTable<daoben_salary_position>((a, b) => a.salary_position_id == b.id)
                            .Where<daoben_salary_position>((a, b) => b.category == 4 && b.effect_status == 1
                            && b.company_id == empInfo.company_id && a.grade_category == gradeCagetory && a.grade == empInfo.grade)
                            .Select("a.base_salary, a.position_salary, a.house_subsidy, a.attendance_reward, a.kpi_advice,a.total").SingleOrDefault();
                if (spGrade == null)
                    return "信息错误：无该员工对应的职等工资信息，请先导入";
                payrollInfo.base_salary = spGrade.base_salary;
                payrollInfo.position_salary = spGrade.position_salary;
                payrollInfo.house_subsidy = spGrade.house_subsidy;
                payrollInfo.attendance_reward = spGrade.attendance_reward;
                payrollInfo.salary += payrollInfo.base_salary;
                payrollInfo.salary += payrollInfo.position_salary;
                payrollInfo.salary += payrollInfo.house_subsidy;
                payrollInfo.salary += payrollInfo.attendance_reward;
                payrollInfo.position_standard_salary = spGrade.total;
                #endregion

                #region 工龄工资
                DateTime entryDate = (DateTime)empInfo.entry_date;
                int seniorityM = (payrollInfo.month.Year - entryDate.Year) * 12 + (payrollInfo.month.Month - entryDate.Month);
                if (entryDate.Day > 15)   // 15号以后入职的不计当月工龄
                    seniorityM--;
                int seniorityY = seniorityM / 12;
                seniorityM = seniorityM % 12;
                payrollInfo.seniority_salary = db.Queryable<daoben_salary_position_seniority>()
                        .JoinTable<daoben_salary_position>((a, b) => a.salary_position_id == b.id)
                        .Where<daoben_salary_position>((a, b) => b.company_id == empInfo.company_id && b.effect_status == 1 && b.category == 3
                        && a.year_min <= seniorityY && a.year_max >= seniorityY)
                        .Select<decimal>("a.salary").SingleOrDefault();
                payrollInfo.salary += payrollInfo.seniority_salary;
                #endregion

                #region 其他


                payrollInfo.salary += payrollInfo.traffic_subsidy;
                //payrollInfo.deposit_total = salesSalaryInfo.resign_deposit>0?0-sa;TODO 累计风险
                decimal deposit_total = db.Queryable<daoben_payroll_sales>()
                        .Where(t => t.emp_id == payrollInfo.emp_id)
                        .Select<decimal>("SUM(resign_deposit)").ToDecimal();
                payrollInfo.deposit_total = deposit_total > 0 ? 0 - deposit_total : deposit_total;
                #endregion
                #region 月度考核
                daoben_kpi_sales salesKPI = null;
                List<daoben_kpi_sales_sn_temp> snList = null;
                if (empInfo.emp_category != "实习生")//实习生没有月度考核
                {
                    salesKPI = db.Queryable<daoben_kpi_sales>()
                            .SingleOrDefault(t => t.emp_id == empInfo.id && t.month == payrollInfo.month);
                    if (salesKPI == null)
                        return "noSalesKPI";

                    daoben_payroll_sales_sub tempMonthSub = new daoben_payroll_sales_sub()
                    {
                        amount = salesKPI.kpi_total_a,
                        category = 6,
                        category_id = salesKPI.id,
                        sub_name = empInfo.name,
                        sub_note = "月度考核",
                    };
                    totalSub.Add(tempMonthSub);
                }
                #endregion


                #region 达量活动 主推活动 业务考核(月度考核之外) pk比赛 排名比赛 

                //达量活动
                List<daoben_activity_attaining> attainingActivity = db.Queryable<daoben_activity_attaining>()
                    .Where(t => (t.emp_category == 21 || t.emp_category == 20) && t.end_date < endTime && t.end_date >= startTime)
                    .Where(t => t.company_id == empInfo.company_id).ToList();
                List<daoben_activity_attaining_emp> attainingEmpList = db.Queryable<daoben_activity_attaining_emp>()
                    .JoinTable<daoben_activity_attaining>((a, b) => a.main_id == b.id)
                    .Where<daoben_activity_attaining>((a, b) => b.end_date < endTime && b.end_date >= startTime)
                    .Where(a => a.emp_id == empInfo.id)
                    .Select("a.*").ToList();
                foreach (var a in attainingEmpList)
                {
                    daoben_activity_attaining tempActivity = attainingActivity.Where(t => t.id == a.main_id).SingleOrDefault();
                    if (tempActivity != null)
                    {
                        daoben_payroll_sales_sub tempSub = new daoben_payroll_sales_sub()
                        {
                            amount = a.total_reward,
                            category = 2,
                            category_id = tempActivity.id,
                            sub_name = tempActivity.name,
                            sub_note = "达量活动",
                        };
                        totalSub.Add(tempSub);
                    }
                }
                //主推活动
                List<daoben_activity_recommendation> recomActivity = db.Queryable<daoben_activity_recommendation>()
                    .Where(t => (t.emp_category == 21 || t.emp_category == 20) && t.end_date < endTime && t.end_date >= startTime)
                    .Where(t => t.company_id == empInfo.company_id).ToList();
                List<daoben_activity_recommendation_emp> recomEmpList = db.Queryable<daoben_activity_recommendation_emp>()
                    .JoinTable<daoben_activity_recommendation>((a, b) => a.main_id == b.id)
                    .Where<daoben_activity_recommendation>((a, b) => b.end_date < endTime && b.end_date >= startTime)
                    .Where(a => a.emp_id == empInfo.id)
                    .Select("a.*").ToList();
                foreach (var a in recomEmpList)
                {
                    daoben_activity_recommendation tempActivity = recomActivity.Where(t => t.id == a.main_id).SingleOrDefault();
                    if (tempActivity != null)
                    {
                        daoben_payroll_sales_sub tempSub = new daoben_payroll_sales_sub()
                        {
                            amount = a.total_reward,
                            category = 1,
                            category_id = tempActivity.id,
                            sub_name = tempActivity.name,
                            sub_note = "主推",
                        };
                        totalSub.Add(tempSub);//temp.note
                    }
                }
                //排名活动
                List<daoben_activity_ranking> rankActivity = db.Queryable<daoben_activity_ranking>()
                    .Where(t => (t.emp_category == 21 || t.emp_category == 20) && t.end_date < endTime && t.end_date >= startTime)
                    .Where(t => t.company_id == empInfo.company_id).ToList();
                List<daoben_activity_ranking_emp> rankEmpList = db.Queryable<daoben_activity_ranking_emp>()
                    .JoinTable<daoben_activity_ranking>((a, b) => a.main_id == b.id)
                    .Where<daoben_activity_ranking>((a, b) => b.end_date < endTime && b.end_date >= startTime)
                    .Where(a => a.emp_id == empInfo.id)
                    .Select("a.*").ToList();
                foreach (var a in rankEmpList)
                {
                    daoben_activity_ranking tempActivity = rankActivity.Where(t => t.id == a.main_id).SingleOrDefault();
                    if (tempActivity != null)
                    {
                        daoben_payroll_sales_sub tempSub = new daoben_payroll_sales_sub()
                        {
                            amount = a.reward,
                            category = 3,
                            category_id = tempActivity.id,
                            sub_name = tempActivity.name,
                            sub_note = "排名" + a.final_place,
                        };
                        totalSub.Add(tempSub);//temp.note
                    }
                }
                //PK活动
                List<daoben_activity_pk> pkActivity = db.Queryable<daoben_activity_pk>()
                    .Where(t => (t.emp_category == 21 || t.emp_category == 20) && t.end_date < endTime && t.end_date >= startTime)
                    .Where(t => t.company_id == empInfo.company_id).ToList();
                List<daoben_activity_pk_emp> pkEmpList = db.Queryable<daoben_activity_pk_emp>()
                    .JoinTable<daoben_activity_pk>((a, b) => a.main_id == b.id)
                    .Where<daoben_activity_pk>((a, b) => b.end_date < endTime && b.end_date >= startTime)
                    .Where(a => a.emp_id == empInfo.id)
                    .Select("a.*").ToList();
                foreach (var a in pkEmpList)
                {
                    daoben_activity_pk tempActivity = pkActivity.Where(t => t.id == a.main_id).SingleOrDefault();
                    if (tempActivity != null)
                    {
                        daoben_payroll_sales_sub tempSub = new daoben_payroll_sales_sub()
                        {
                            amount = a.total_amount,
                            category = 4,
                            category_id = tempActivity.id,
                            sub_name = tempActivity.name,
                            sub_note = "PK比赛",
                        };
                        totalSub.Add(tempSub);
                    }
                }

                //业务考核
                List<daoben_payroll_sales_sub> perfSubList = db.Queryable<daoben_activity_sales_perf_emp>()
                        .JoinTable<daoben_activity_sales_perf>((a, b) => a.main_id == b.id)
                        .Where<daoben_activity_sales_perf>((a, b) => b.end_date < endTime && b.end_date >= startTime
                        && a.emp_id == empInfo.id && b.activity_status == 2 && b.category > 1)
                        .Select<daoben_payroll_sales_sub>("a.total_reward as amount, 5 as category, b.id as category_id, b.name as sub_name, IF(b.category=2,'销量考核','导购人数考核') as sub_note")
                        .ToList();
                if (perfSubList != null && perfSubList.Count > 0)
                    totalSub.AddRange(perfSubList);
                
                #endregion

                //if (origsalesKPI != null)
                //    db.Delete<daoben_sale_month_emp>(t => t.id == origsalesKPI.id);
                //db.Insert(salesKPI);
                if (empInfo.emp_category == "实习生")
                {
                    if (payrollInfo.intern_salary_type == 2)
                        payrollInfo.salary = payrollInfo.intern_salary;
                }
                payrollInfo.salary = 0;//前端重新计算
                #region 留守补贴
                daoben_salary_staysubsidy staySubsidy = db.Queryable<daoben_salary_staysubsidy_emp>()
                            .JoinTable<daoben_salary_staysubsidy>((a, b) => b.id == a.main_id)
                            .Where<daoben_salary_staysubsidy>((a, b) => a.emp_id == id && b.approve_status == 100 && b.month == payrollInfo.month)
                            .Select<daoben_salary_staysubsidy>("b.company_amount,b.emp_amount").FirstOrDefault();
                #endregion
                object retObj = new
                {
                    empInfo = empInfo,
                    payrollInfo = payrollInfo,
                    //empSalaryInfo = empSalaryInfo,
                    //posSlaryInfo = posSlaryInfo,
                    staySubsidy = staySubsidy,          //留守补助
                    seniorityStr = seniorityY.ToString() + "年" + seniorityM.ToString() + "个月",       // 工龄
                    payrollSubList = totalSub,
                    salesKPI = salesKPI,
                    snList = snList
                };

                return retObj.ToJson();
            }

        }

#if false
        public List<daoben_kpi_sales_sn_temp> figureKPI(SqlSugarClient db, daoben_payroll_setting payrollMonth,
                daoben_hr_emp_job empInfo, daoben_kpi_sales salesKPI, string main_id = null)
        {
            DateTime startTime = payrollMonth.start_date;
            DateTime endTime = payrollMonth.end_date.ToDate().AddDays(1);

            daoben_salary_emp empSalaryInfo = db.Queryable<daoben_salary_emp>()
                    .SingleOrDefault(t => t.emp_id == empInfo.id && t.effect_status == 1 && t.approve_status == 100 && t.category == 4);
            daoben_salary_position posSlaryInfo = db.Queryable<daoben_salary_position>()
                    .SingleOrDefault(t => t.position_id == empInfo.position_id && t.effect_status == 1 && t.approve_status == 100
                    && t.category == empInfo.position_type);// category 与 jobInfo.position_type 相对应（仅业务员/业务经理）);
            if (empSalaryInfo == null && posSlaryInfo == null)
                return null;
            daoben_salary_emp_sales salesSalaryInfo = new daoben_salary_emp_sales();
            List<daoben_salary_emp_sales_sub> subList = null;
            if (empSalaryInfo != null)
            {
#region 个人薪资方案
                salesSalaryInfo = db.Queryable<daoben_salary_emp_sales>()
                        .SingleOrDefault(a => a.salary_position_id == empSalaryInfo.id);
                subList = db.Queryable<daoben_salary_emp_sales_sub>()
                        .Where(a => a.main_id == salesSalaryInfo.id).ToList();
                if (salesSalaryInfo == null || subList == null || subList.Count < 1)
                    return null;
#endregion
            }
            else
            {
#region 无个人薪资方案，使用岗位薪资
                //salesSalaryInfo = db.Queryable<daoben_salary_position_sales>()
                //        .Where(a => a.salary_position_id == posSlaryInfo.id)
                //        .Select<daoben_salary_emp_sales>("id,salary_position_id,activity_target,target_content,target_mode,normal_rebate_mode,buyout_rebate_mode")
                //        .SingleOrDefault();
                daoben_salary_position_sales tempInfo = db.Queryable<daoben_salary_position_sales>()
                            .Where(a => a.salary_position_id == posSlaryInfo.id)
                            .SingleOrDefault();
                salesSalaryInfo.id = tempInfo.id;
                salesSalaryInfo.salary_position_id = tempInfo.salary_position_id;
                salesSalaryInfo.activity_target = tempInfo.activity_target;
                salesSalaryInfo.target_content = tempInfo.target_content;
                salesSalaryInfo.target_mode = tempInfo.target_mode;
                salesSalaryInfo.normal_rebate_mode = tempInfo.normal_rebate_mode;
                salesSalaryInfo.buyout_rebate_mode = tempInfo.buyout_rebate_mode;
                if (salesSalaryInfo == null)
                    return null;
                subList = db.Queryable<daoben_salary_position_sales_sub>()
                        .Where(a => a.main_id == salesSalaryInfo.id)
                        .Select<daoben_salary_emp_sales_sub>("*").ToList();
                if (subList == null || subList.Count < 1)
                    return null;
#endregion
            }
#region 所有销售串码及统计

            salesKPI = new daoben_kpi_sales();
            salesKPI.emp_id = empInfo.id;
            salesKPI.position_type = empInfo.position_type;
            salesKPI.month = payrollMonth.month;
            salesKPI.company_id = empInfo.company_id;
            salesKPI.company_id_parent = empInfo.company_id_parent;
            salesKPI.company_linkname = empInfo.company_linkname;
            salesKPI.status = -200;
            salesKPI.is_del = false;
            salesKPI.id = main_id == null ? Common.GuId() : main_id;
            string selStr = "'{0}' as main_id, phone_sn,model, color,sale_type, price_wholesale, price_sale, {1} as time";
            var qable = db.Queryable<daoben_product_sn>();
            if (empInfo.position_type == ConstData.POSITION_SALES)
            {
                if (salesSalaryInfo.target_content == 1)
                {
                    qable.Where(a => a.sales_id == empInfo.id && a.sale_type > 0
                                && a.sale_time >= startTime && a.sale_time < endTime);
                    selStr = string.Format(selStr, main_id, "sale_time");
                }
                else
                {
                    qable.Where(a => a.out_sales_id == empInfo.id && a.sale_type > 1
                                && a.outstorage_time >= startTime && a.outstorage_time < endTime);
                    selStr = string.Format(selStr, main_id, "outstorage_time");

                }
            }
            else if (empInfo.position_type == ConstData.POSITION_SALESMANAGER)
            {
                if (salesSalaryInfo.target_content == 1)
                {
                    qable.Where(a => a.sales_m_id == empInfo.id && a.sale_type > 0
                                && a.sale_time >= startTime && a.sale_time < endTime);
                    selStr = string.Format(selStr, main_id, "sale_time");
                }
                else
                {
                    qable.Where(a => a.out_sales_m_id == empInfo.id && a.sale_type > 1
                                && a.outstorage_time >= startTime && a.outstorage_time < endTime);
                    selStr = string.Format(selStr, main_id, "outstorage_time");
                }
            }
            List<daoben_kpi_sales_sn_temp> snList = qable.Select<daoben_kpi_sales_sn_temp>(selStr).ToList();
            if (snList == null || snList.Count < 1)
                return null;
            snList.ForEach(a =>
            {
                if (a.sale_type == 1 || a.sale_type == 3)
                {
                    salesKPI.normal_count++;
                    salesKPI.normal_amount_r += a.price_sale;
                    salesKPI.normal_amount_w += a.price_wholesale;
                }
                else if (a.sale_type == 2)
                {
                    salesKPI.buyout_count++;
                    salesKPI.buyout_amount += a.price_sale;
                    salesKPI.buyout_amount_w += a.price_wholesale;
                }
            });
            salesKPI.total_count = salesKPI.normal_count + salesKPI.buyout_count;
            salesKPI.total_amount_r = salesKPI.normal_amount_r + salesKPI.buyout_amount;
            salesKPI.total_amount_w = salesKPI.normal_amount_w + salesKPI.buyout_amount_w;
            List<daoben_kpi_sales_sn_temp> normalSnList = snList.Where(a => a.sale_type != 2).ToList();
            List<daoben_kpi_sales_sn_temp> buyoutSnList = snList.Where(a => a.sale_type == 2).ToList();
#endregion
#region 月度考核
            if (salesSalaryInfo.target_mode == 1 || salesSalaryInfo.target_mode == 2)
            {
#region 按完成率/按台数 
                daoben_salary_emp_sales_sub rebateModel = null;
                if (salesSalaryInfo.target_mode == 1)
                {
                    if (salesSalaryInfo.activity_target == 0)
                        salesKPI.total_ratio = 100;
                    else
                        salesKPI.total_ratio = (((decimal)snList.Count()) / salesSalaryInfo.activity_target) * 100;
                    rebateModel = subList.Where(a => salesKPI.total_ratio >= a.target_min)
                                .OrderByDescending(a => a.target_min).FirstOrDefault();
                }
                else
                    rebateModel = subList.Where(a => salesKPI.total_count >= a.target_min)
                            .OrderByDescending(a => a.target_min).First();

                switch (salesSalaryInfo.normal_rebate_mode)
                {
                    case 1:
                        salesKPI.kpi_total = salesKPI.normal_count * rebateModel.sale_rebate;
                        normalSnList.ForEach(ns => ns.outlay = rebateModel.sale_rebate);
                        break;
                    case 2:
                        salesKPI.kpi_total = salesKPI.normal_amount_w * rebateModel.sale_rebate / 100;
                        normalSnList.ForEach(ns => ns.outlay = ns.price_wholesale * rebateModel.sale_rebate / 100);
                        break;
                    case 3:
                        salesKPI.kpi_total = salesKPI.normal_amount_r * rebateModel.sale_rebate / 100;
                        normalSnList.ForEach(ns => ns.outlay = ns.price_sale * rebateModel.sale_rebate / 100);
                        break;
                    case 4:
                        salesKPI.kpi_total = rebateModel.sale_rebate;
                        decimal perCommission = rebateModel.buyout_rebate / salesKPI.normal_count; // 均摊
                        normalSnList.ForEach(ns => ns.outlay = perCommission);
                        break;
                    default:
                        break;
                }
                switch (salesSalaryInfo.buyout_rebate_mode)
                {
                    case 1:
                        salesKPI.kpi_total += salesKPI.buyout_count * rebateModel.buyout_rebate;
                        buyoutSnList.ForEach(bs => bs.outlay = rebateModel.buyout_rebate);
                        break;
                    case 2:
                        salesKPI.kpi_total += salesKPI.buyout_amount_w * rebateModel.buyout_rebate / 100;
                        buyoutSnList.ForEach(bs => bs.outlay = bs.price_wholesale * rebateModel.buyout_rebate / 100);
                        break;
                    case 3:
                        salesKPI.kpi_total += salesKPI.buyout_amount * rebateModel.buyout_rebate / 100;
                        buyoutSnList.ForEach(bs => bs.commission = bs.price_sale * rebateModel.buyout_rebate / 100);
                        break;
                    case 4:
                        salesKPI.kpi_total += rebateModel.buyout_rebate;
                        decimal perCommission = rebateModel.buyout_rebate / salesKPI.buyout_count; // 均摊
                        buyoutSnList.ForEach(bs => bs.commission = perCommission);
                        break;
                    default:
                        break;
                }
#endregion
            }
            else if (salesSalaryInfo.target_mode == 3 || salesSalaryInfo.target_mode == 5)
            {
#region 按零售价/批发价
                subList.ForEach(a =>
                {
                    int max = a.target_max == -1 ? 100000000 : a.target_max + 1; // 用1亿替代“以上”
                    List<daoben_kpi_sales_sn_temp> subNormalSnList, subBuyoutSnList;
                    if (salesSalaryInfo.target_mode == 3)
                    {
                        subNormalSnList = normalSnList.Where(n => n.price_wholesale >= a.target_min && n.price_wholesale < max).ToList();
                        subBuyoutSnList = buyoutSnList.Where(n => n.price_wholesale >= a.target_min && n.price_wholesale < max).ToList();
                    }
                    else
                    {
                        subNormalSnList = normalSnList.Where(n => n.price_sale >= a.target_min && n.price_sale < max).ToList();
                        subBuyoutSnList = buyoutSnList.Where(n => n.price_sale >= a.target_min && n.price_sale < max).ToList();
                    }
                    if (salesSalaryInfo.normal_rebate_mode == 1)
                    {
                        salesKPI.kpi_total += subNormalSnList.Count * a.sale_rebate;
                        subNormalSnList.ForEach(ns => ns.commission = a.sale_rebate);
                    }
                    else if (salesSalaryInfo.normal_rebate_mode == 2)
                    {
                        salesKPI.kpi_total += subNormalSnList.Sum(n => n.price_wholesale) * a.sale_rebate / 100;
                        subNormalSnList.ForEach(ns => ns.commission = ns.price_wholesale * a.sale_rebate / 100);

                    }
                    else if (salesSalaryInfo.normal_rebate_mode == 3)
                    {
                        salesKPI.kpi_total += subNormalSnList.Sum(n => n.price_sale) * a.sale_rebate / 100;
                        subNormalSnList.ForEach(ns => ns.commission = ns.price_sale * a.sale_rebate / 100);
                    }

                    if (salesSalaryInfo.buyout_rebate_mode == 1)
                    {
                        salesKPI.kpi_total += subBuyoutSnList.Count * a.buyout_rebate;
                        subBuyoutSnList.ForEach(bs => bs.commission = a.buyout_rebate);
                    }
                    else if (salesSalaryInfo.buyout_rebate_mode == 2)
                    {
                        salesKPI.kpi_total += subBuyoutSnList.Sum(n => n.price_wholesale) * a.buyout_rebate / 100;
                        subBuyoutSnList.ForEach(bs => bs.commission = bs.price_wholesale * a.buyout_rebate / 100);
                    }
                    else if (salesSalaryInfo.buyout_rebate_mode == 3)
                    {
                        salesKPI.kpi_total += subBuyoutSnList.Sum(n => n.price_sale) * a.buyout_rebate / 100;
                        subBuyoutSnList.ForEach(bs => bs.commission = bs.price_sale * a.buyout_rebate / 100);
                    }
                });
#endregion
            }
            salesKPI.kpi_total_a = salesKPI.kpi_total;
#endregion
            return snList;

        }
#endif

        /// <summary>
        /// 获取工资信息，用于查看页面
        /// </summary>
        /// <returns></returns>
        public string GetInfo(string id)
        {
            var LoginInfo = OperatorProvider.Provider.GetCurrent();
            if (LoginInfo == null)
                return "登录超时，请重新登录";
            if (string.IsNullOrEmpty(id))
                return "信息错误：ID不能为空";
            using (var db = SugarDao.GetInstance())
            {
                try
                {
                    daoben_payroll_sales mainInfo = db.Queryable<daoben_payroll_sales>().SingleOrDefault(a => a.id == id);
                    if (mainInfo == null)
                        return "信息错误：指定的信息不存在";
                    daoben_hr_emp_job empInfo = db.Queryable<daoben_hr_emp_job>().SingleOrDefault(a => a.id == mainInfo.emp_id);
                    List<daoben_payroll_sales_approve> approveList = db.Queryable<daoben_payroll_sales_approve>().Where(a => a.main_id == id).ToList();
                    List<daoben_payroll_sales_sub> subList = db.Queryable<daoben_payroll_sales_sub>().Where(a => a.main_id == id).ToList();
                    daoben_kpi_sales origsalesKPI = db.Queryable<daoben_kpi_sales>()
                            .Where(a => a.emp_id == empInfo.id && a.month == mainInfo.month && a.is_del == false)
                            .OrderBy("status DESC")
                            .FirstOrDefault();
                    object resultObj = new
                    {
                        mainInfo = mainInfo,
                        empInfo = empInfo,
                        approveList = approveList,
                        subList = subList,
                        salesKPI = origsalesKPI,
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
        /// 获取员工福利列表
        /// </summary>
        /// <param name="empId"></param>
        /// <param name="payrollSubList"></param>
        /// <param name="month"></param>
        /// <param name="db"></param>
        private void GetEmpBenefitList(string empId, List<daoben_payroll_sales_sub> payrollSubList, DateTime? month, SqlSugarClient db)
        {
            List<daoben_salary_benefit_detail> benefitList = db.Queryable<daoben_salary_benefit_detail>()
                           .JoinTable<daoben_salary_benefit>((a, b) => a.main_id == b.id)
                           .JoinTable<daoben_salary_benefit, daoben_salary_benefit_emp>((a, b, c) => c.main_id == b.id)
                           .Where<daoben_salary_benefit, daoben_salary_benefit_emp>((a, b, c) => c.emp_id == empId
                           && b.approve_status == 100 && a.paid_type == 1 && a.paid_date == month)
                           .Select("a.main_id, a.name, a.amount, a.note").ToList();
            if (benefitList != null)
            {
                benefitList.ForEach(a =>
                {
                    daoben_payroll_sales_sub infoSub = new daoben_payroll_sales_sub
                    {
                        category = 12,
                        category_id = a.main_id,
                        sub_name = a.name,
                        sub_note = a.note,
                        amount = a.amount
                    };
                    payrollSubList.Add(infoSub);
                });
            }
        }

        /// <summary>
        /// 获取员工奖罚列表
        /// </summary>
        /// <param name="empId"></param>
        /// <param name="payrollSubList"></param>
        /// <param name="month"></param>
        /// <param name="db"></param>
        private void GetEmpRewardList(string empId, List<daoben_payroll_sales_sub> payrollSubList, DateTime? month, SqlSugarClient db)
        {
            List<daoben_salary_reward_detail> rewardList = db.Queryable<daoben_salary_reward_detail>()
                          .JoinTable<daoben_salary_reward>((a, b) => a.main_id == b.id)
                          .Where<daoben_salary_reward>((a, b) => b.emp_id == empId
                          && b.approve_status == 100 && b.month == month)
                          .Select("a.main_id, a.detail_name, a.amount, a.note").ToList();
            if (rewardList != null)
            {
                rewardList.ForEach(a =>
                {
                    daoben_payroll_sales_sub infoSub = new daoben_payroll_sales_sub
                    {
                        category = 11,
                        category_id = a.main_id,
                        sub_name = a.detail_name,
                        sub_note = a.note,
                        amount = a.amount
                    };
                    payrollSubList.Add(infoSub);
                });
            }
        }

        public string Add(daoben_payroll_sales addInfo, List<daoben_payroll_sales_sub> subList, daoben_kpi_sales salesKPI)
        {
            var LoginInfo = OperatorProvider.Provider.GetCurrent();
            if (LoginInfo == null)
                return null;
            CompanyInfo myCompanyInfo = LoginInfo.companyInfo;
            PositionInfo myPositionInfo = LoginInfo.positionInfo;
            addInfo.id = Common.GuId();
            subList.ForEach(a => a.main_id = addInfo.id);

            using (var db = SugarDao.GetInstance())
            {
                try
                {
                    daoben_hr_emp_job empInfo = db.Queryable<daoben_hr_emp_job>().SingleOrDefault(a => a.id == addInfo.emp_id);
                    if (empInfo == null)
                    {
                        return "信息错误：指定的员工信息不存在";
                    }

                    List<daoben_kpi_sales> origKpiList = db.Queryable<daoben_kpi_sales>()
                            .Where(t => t.emp_id == empInfo.id && t.month == addInfo.month).ToList();

                    addInfo.name = empInfo.name;
                    //addInfo.job_history_id = jobHistory.id;
                    addInfo.approve_status = 0;
                    addInfo.creator_job_history_id = LoginInfo.jobHistoryId;
                    addInfo.creator_name = LoginInfo.empName;
                    addInfo.create_time = DateTime.Now;
                    //daoben_kpi_sales origsalesKPI = db.Queryable<daoben_kpi_sales>()
                    //    .Where(a => a.emp_id == salesKPI.emp_id && a.month == salesKPI.month).FirstOrDefault();
                    //if (origsalesKPI == null)
                    //    db.Insert(salesKPI);
                    db.CommandTimeOut = 30;
                    db.BeginTran();
                    db.Insert(addInfo);
                    if (origKpiList != null && origKpiList.Count > 0)
                    {
                        if (origKpiList.Count == 1)
                        {
                            if (origKpiList[0].status == -200)
                                db.Update<daoben_kpi_sales>(new { status = 0 }, t => t.id == origKpiList[0].id);
                        }
                        else if (origKpiList.Exists(a => a.status >= 0))   // 已提交了月度考核
                        {
                            db.Delete<daoben_kpi_sales>(a => a.emp_id == empInfo.id && a.month == addInfo.month && a.status == -200);
                        }
                    }

                    db.DisableInsertColumns = new string[] { "id" };
                    db.InsertRange(subList);
                    db.CommitTran();

                }
                catch (Exception ex)
                {
                    db.RollbackTran();
                    return "系统出错：" + ex.Message;
                }
                //1.11
                approveTemp(addInfo.id);
                return "success";
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
                db.Update<daoben_payroll_sales>(upObj, a => a.id == id);
            }
        }
    }
}
