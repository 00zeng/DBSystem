﻿using Base.Code;
using MySqlSugar;
using NPOI.HSSF.UserModel;
using ProjectShare.Database;
using ProjectShare.Models;
using ProjectShare.Process;
using ProjectWeb.Areas.FinancialAccounting.Application;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace ProjectWeb.Areas.SalaryCalculate.Application
{
    public class PayrollOfficeApp
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
                var qable = db.Queryable<daoben_payroll_office>()
                            .JoinTable<daoben_hr_emp_job>((a, b) => a.emp_id == b.id)
                            .JoinTable<daoben_payroll_office_sub>((a, c) => c.main_id == a.id);
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
                            .Select("SQL_CALC_FOUND_ROWS SUM(c.amount) AS sub_amount,a.*,b.work_number,b.name_v2,b.position_name,b.grade,b.dept_name,b.company_linkname,b.emp_category,b.entry_date,b.status")
                            .GroupBy("a.id")
                            .OrderBy(pagination.sidx + " " + pagination.sord)
                            .ToJsonPage(pagination.page, pagination.rows, ref records);
                pagination.records = db.SqlQuery<int>("SELECT FOUND_ROWS() as count").FirstOrDefault();
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
                var qable = db.Queryable<daoben_payroll_office>()
                            .JoinTable<daoben_hr_emp_job>((a, b) => a.emp_id == b.id)
                            .JoinTable<daoben_payroll_office_sub>((a, c) => c.main_id == a.id);

                //工资结算-行政工资
                string str = "b.name,b.work_number,DATE_FORMAT(a.month,'%Y-%m'),b.position_name,a.position_standard_salary,b.grade,DATE_FORMAT(b.entry_date,'%Y-%m-%d'),a.work_days,a.onduty_day,a.base_salary, a.position_salary,a.house_subsidy,a.attendance_reward,a.kpi,a.seniority_salary,a.salary_subsidy,a.salary,a.leaving_deduction,a.insurance_fee, c.amount as sub_amount,a.resign_deposit,a.actual_salary,a.note";
                var listDt = qable.OrderBy(pagination.sidx + " " + pagination.sord)
                    .GroupBy("a.id")
                    .Select(str)
                    .ToDataTable();
                int rowsCount = listDt.Rows.Count;
                string[] headerArr = new string[]
                { "姓名","工号","结算月份","岗位","岗位工资标准","职等","入职时间","考勤天数","实际天数","基本工资","岗位工资","食宿补助",
            "全勤奖","KPI","工龄工资","特聘补贴","应发工资","请假扣款","社保","其他","风险金","实发金额","备注",
                };

                int[] colWidthArr = new int[] { 15, 15, 15, 15, 15, 15, 15, 15, 15, 15, 15, 15, 15, 15, 15, 15, 15, 15, 15, 15, 16, 15, 15, 25 };

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
                object list = db.Queryable<daoben_payroll_office>()  // todo
                            .Where(a => a.approve_status == 0)
                            .OrderBy(pagination.sidx + " " + pagination.sord)
                            .ToJsonPage(pagination.page, pagination.rows, ref records);
                pagination.records = records;
                return list;
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
            pagination.sidx = string.IsNullOrEmpty(pagination.sidx) ? "company_id" : pagination.sidx;
            pagination.sord = string.IsNullOrEmpty(pagination.sord) ? "desc" : pagination.sord;
            CompanyInfo myCompanyInfo = LoginInfo.companyInfo;
            PositionInfo myPositionInfo = LoginInfo.positionInfo;
            DateTime now = DateTime.Now.Date;
            using (var db = SugarDao.GetInstance())
            {
                // 获取工资结算时间，实际是结算上一月
                daoben_payroll_setting payrollMonth = new PayrollSettingApp().GetPayrollMonth(myCompanyInfo.id, db, true);
                if (payrollMonth == null)
                    return "系统出错：工资结算周期信息不存在";
                DateTime curPayrollMonth = payrollMonth.month;
                var qable = db.Queryable<daoben_hr_emp_job>()
                        .JoinTable<daoben_payroll_office>((a, b) => a.id == b.emp_id && b.month == curPayrollMonth)
                        .Where(a => a.position_type <= ConstData.POSITION_OFFICE_NORMAL);
                if (myCompanyInfo.category == "事业部")
                    qable.Where(a => (a.company_id_parent == myCompanyInfo.id || a.company_id == myCompanyInfo.id));
                else if (myCompanyInfo.category == "分公司")
                    qable.Where(a => a.company_id == myCompanyInfo.id);
                if (!showAll)
                    qable.Where<daoben_payroll_office>((a, b) => b.id == null && a.entry_date < curPayrollMonth.AddDays(16));
                if (queryInfo != null)
                {
                    if (!string.IsNullOrEmpty(queryInfo.name))
                        qable.Where(a => a.name.Contains(queryInfo.name));
                    if (!string.IsNullOrEmpty(queryInfo.position_name))
                        qable.Where(a => a.position_name.Contains(queryInfo.position_name));
                    if (queryInfo.company_id > 0)
                        qable.Where(a => a.company_id == queryInfo.company_id);
                    if (queryInfo.dept_id > 0)
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
        /// 获取工资信息，用于结算页面
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
            if (myCompanyInfo.category != "事业部")
                return "权限错误：工资结算需由事业部人员操作";

            int companyId = myCompanyInfo.id;
            DateTime now = DateTime.Now.Date;
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

                daoben_payroll_office payrollInfo = new daoben_payroll_office
                {
                    month = payrollMonth.month,
                    start_date = payrollMonth.start_date,
                    end_date = (DateTime)payrollMonth.end_date
                };
                #region 福利、奖罚
                List<daoben_payroll_office_sub> payrollSubList = new List<daoben_payroll_office_sub>();
                GetEmpBenefitList(empInfo.id, payrollSubList, payrollInfo.month, db);
                GetEmpRewardList(empInfo.id, payrollSubList, payrollInfo.month, db);
                #endregion

                #region 个人专有工资信息
                daoben_salary_emp_general seGeneral = db.Queryable<daoben_salary_emp_general>()
                        .JoinTable<daoben_salary_emp>((a, b) => b.id == a.main_id)
                        .Where<daoben_salary_emp>((a, b) => b.emp_id == empInfo.id && b.effect_status == 1 && b.category == 1)
                        .Select("a.resign_deposit,a.traffic_subsidy,a.intern_salary_type,a.intern_fix_salary,a.intern_ratio_salary")
                        .SingleOrDefault();
                if (seGeneral == null)   // 无个人专有的工资信息
                {
                    seGeneral = db.Queryable<daoben_salary_position_other>()
                            .JoinTable<daoben_salary_position>((a, b) => a.salary_position_id == b.id)
                            .Where<daoben_salary_position>((a, b) => b.company_id == empInfo.company_id && b.effect_status == 1 && b.category == 3)
                            .Select<daoben_salary_emp_general>("a.salary_position_id as main_id, a.resign_deposit,a.intern_salary_type,a.intern_fix_salary,a.intern_ratio_salary")
                            .SingleOrDefault();
                    if (seGeneral == null)
                        return "noOfficeSalarySetting";
                    seGeneral.traffic_subsidy = db.Queryable<daoben_salary_position_traffic>()
                            .Where(a => a.salary_position_id == seGeneral.main_id)
                            .Select(a => a.traffic_subsidy).SingleOrDefault();
                }
                payrollInfo.resign_deposit = seGeneral.resign_deposit < 0 ? seGeneral.resign_deposit : 0 - seGeneral.resign_deposit;
                payrollInfo.traffic_subsidy = seGeneral.traffic_subsidy;
                if (empInfo.emp_category == "实习生" && seGeneral.intern_salary_type == 2) // 实习生固定工资
                {
                    payrollInfo.intern_salary_type = seGeneral.intern_salary_type;
                    payrollInfo.intern_salary = payrollInfo.salary = seGeneral.intern_fix_salary;
                    object retInternObj = new
                    {
                        empInfo = empInfo,                  // 员工信息
                        payrollInfo = payrollInfo,          // 工资信息
                        payrollSubList = payrollSubList,    // 福利奖罚等
                    };
                    return retInternObj.ToJson();
                }
                else if (empInfo.emp_category == "实习生")
                {
                    payrollInfo.intern_salary_type = 1;
                    payrollInfo.intern_salary = seGeneral.intern_ratio_salary; //实习工资按比例发放，结算前占用该字段存放比例
                }
                //  payrollInfo.salary += payrollInfo.resign_deposit;
                payrollInfo.salary += (payrollInfo.traffic_subsidy > 0 ? payrollInfo.traffic_subsidy : 0);

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
                //1.25 18：58
                //payrollInfo.seniority_salary = db.Queryable<daoben_salary_position_seniority>()
                //        .JoinTable<daoben_salary_position>((a, b) => a.salary_position_id == b.id)
                //        .Where<daoben_salary_position>((a, b) => b.company_id == empInfo.company_id && b.effect_status == 1 && b.category == 3
                //        && a.year_min <= seniorityY && a.year_max >= seniorityY)
                //        .Select<decimal>("a.salary").SingleOrDefault();
                List<daoben_salary_position_seniority> seniorityList = db.Queryable<daoben_salary_position_seniority>()
                        .JoinTable<daoben_salary_position>((a, b) => a.salary_position_id == b.id)
                        .Where<daoben_salary_position>((a, b) => b.company_id == empInfo.company_id && b.effect_status == 1 && b.category == 3
                            && a.year_min <= seniorityY)
                        .Select("a.*").ToList();
                daoben_salary_position_seniority tempSeniority = seniorityList.OrderByDescending(t => t.year_min).FirstOrDefault();
                if (tempSeniority != null)
                    payrollInfo.seniority_salary = tempSeniority.salary;
                //1.25 19：16
                payrollInfo.salary += payrollInfo.seniority_salary;
                #endregion

                #region KPI
                payrollInfo.kpi_standard = db.Queryable<daoben_salary_position_dept>()
                            .JoinTable<daoben_salary_position>((a, b) => a.salary_position_id == b.id)
                            .Where<daoben_salary_position>((a, b) => b.category == 13 && b.effect_status == 1
                            && b.dept_id == empInfo.dept_id && a.grade == empInfo.grade)
                            .Select<decimal>("a.kpi_standard").SingleOrDefault();
                daoben_salary_emp_kpi_subsidy seKpiSubsidy = db.Queryable<daoben_salary_emp_kpi_subsidy>()
                        .JoinTable<daoben_salary_emp>((a, b) => b.id == a.main_id)
                        .Where<daoben_salary_emp>((a, b) => b.emp_id == empInfo.id && b.effect_status == 1 && b.category == 2)
                        .Select("a.kpi_subsidy,a.kpi_subsidy_full,a.kpi_subsidy_note")
                        .SingleOrDefault();
                daoben_salary_kpi kpiScore = db.Queryable<daoben_salary_kpi>()
                            .Where(a => a.category == 1 && a.approve_status == 100 && a.emp_id == id && a.month == payrollInfo.month)
                            .Select("kpi,kpi_subsidy").SingleOrDefault();
                if (kpiScore != null)
                {
                    payrollInfo.kpi_standard_score = kpiScore.kpi;
                    payrollInfo.kpi = payrollInfo.kpi_standard * kpiScore.kpi / 100;
                }
                if (seKpiSubsidy != null)
                {
                    payrollInfo.kpi_subsidy = seKpiSubsidy.kpi_subsidy;
                    if (seKpiSubsidy.kpi_subsidy_full)
                    {
                        payrollInfo.kpi += payrollInfo.kpi_subsidy;
                        payrollInfo.kpi_subsidy_score = -1;
                    }
                    else
                    {
                        payrollInfo.kpi_subsidy_score = (kpiScore != null ? kpiScore.kpi_subsidy : payrollInfo.kpi_standard_score);
                        payrollInfo.kpi += (payrollInfo.kpi_subsidy * payrollInfo.kpi_subsidy_score / 100);
                    }
                }
                payrollInfo.salary += payrollInfo.kpi;
                #endregion

                #region 其他补助
                payrollInfo.salary_subsidy = db.Queryable<daoben_salary_emp_subsidy>()
                       .JoinTable<daoben_salary_emp>((a, b) => b.id == a.main_id)
                       .Where<daoben_salary_emp>((a, b) => b.emp_id == empInfo.id && b.effect_status == 1 && b.category == 3)
                       .Select<decimal>("a.subsidy").SingleOrDefault();
                payrollInfo.salary += payrollInfo.salary_subsidy;
                #endregion
                #region 留守补贴
                daoben_salary_staysubsidy staySubsidy = db.Queryable<daoben_salary_staysubsidy_emp>()
                            .JoinTable<daoben_salary_staysubsidy>((a, b) => b.id == a.main_id)
                            .Where<daoben_salary_staysubsidy>((a, b) => a.emp_id == id && b.approve_status == 100 && b.month == payrollInfo.month)
                            .Select<daoben_salary_staysubsidy>("b.company_amount,b.emp_amount").FirstOrDefault();
                #endregion
#if false
                #region 出勤
                daoben_hr_attendance attendanceInfo = db.Queryable<daoben_hr_attendance>()
                           .JoinTable<daoben_hr_attendance_approve>((a, b) => a.import_file_id == b.id)
                           .Where<daoben_hr_attendance_approve>((a, b) => a.emp_name == empInfo.name && b.status == 100
                           && a.month == monthInfo.month)
                           .Select("a.work_days, a.attendance, a.attendance_rate").SingleOrDefault();
                if (attendanceInfo != null)
                {
                    getInfo.work_days = attendanceInfo.work_days;
                    getInfo.onduty_day = attendanceInfo.attendance;
                    if (attendanceInfo.attendance_rate < 100)
                        getInfo.attendance_reward = 0;
                    else
                        getInfo.attendance_reward = payrollTemp.attendance_reward;
                }
                else
                    getInfo.attendance_reward = -1;
                getInfo.seniority_salary = payrollTemp.seniority_salary;
                getInfo.traffic_subsidy = payrollTemp.traffic_subsidy;
                getInfo.salary_subsidy = payrollTemp.salary_subsidy;
                getInfo.resign_deposit = payrollTemp.resign_deposit;

                getInfo.salary = getInfo.base_salary + getInfo.position_salary + getInfo.house_subsidy + getInfo.attendance_reward
                            + getInfo.seniority_salary + getInfo.traffic_subsidy + getInfo.salary_subsidy + getInfo.kpi;
                #endregion

#endif
                object retObj = new
                {
                    empInfo = empInfo,                  // 员工信息
                    payrollInfo = payrollInfo,          // 工资信息
                    payrollSubList = payrollSubList,    // 福利奖罚等
                    staySubsidy = staySubsidy,          //留守补助
                    seniorityStr = seniorityY.ToString() + "年" + seniorityM.ToString() + "个月",       // 工龄
                    seKpiSubsidy = seKpiSubsidy
                };
                return retObj.ToJson();
            }

        }

        /// <summary>
        /// 获取工资信息，用于结算页面
        /// </summary>
        /// <param name="id">员工ID</param>
        /// <returns></returns>
        public string GetSettingInfoList(int deptId)
        {
            OperatorModel LoginInfo = OperatorProvider.Provider.GetCurrent();
            if (LoginInfo == null)
                throw new Exception("用户登陆过期，请重新登录");
            if (deptId < 1)
                return "信息错误：部门ID不正确";
            CompanyInfo myCompanyInfo = LoginInfo.companyInfo;
            if (myCompanyInfo.category != "事业部")
                return "权限错误：工资结算需由事业部人员操作";

            int companyId = myCompanyInfo.id;
            DateTime now = DateTime.Now.Date;
            List<object> resList = new List<object>();
            using (var db = SugarDao.GetInstance())
            {
                // 获取工资结算时间
                daoben_payroll_setting payrollMonth = new PayrollSettingApp().GetPayrollMonth(companyId, db, true);
                if (payrollMonth == null)
                    return "系统出错：工资结算周期信息不存在";
                List<daoben_hr_emp_job> empList = db.Queryable<daoben_hr_emp_job>().Where(a => a.dept_id == deptId).ToList();
                if(empList == null || empList.Count < 1)
                    return "该部门无员工";
                foreach(var empInfo in empList)
                {
                    object resInfo = new
                    {
                        empInfo = empInfo,                  // 员工信息
                    };
                    resList.Add(resInfo);
                }
                object result = new
                {
                    payrollMonth = payrollMonth,
                    resList = resList
                };
                return result.ToJson();
#if false
                daoben_hr_emp_job empInfo = db.Queryable<daoben_hr_emp_job>().SingleOrDefault(a => a.id == id);
                if (empInfo == null)
                    return "系统出错：员工信息不存在";

                
                if (empInfo.entry_date > payrollMonth.month.AddDays(15))
                    return "该员工在本月15之后入职，本月无工资";

                daoben_payroll_office payrollInfo = new daoben_payroll_office
                {
                    month = payrollMonth.month,
                    start_date = payrollMonth.start_date,
                    end_date = (DateTime)payrollMonth.end_date
                };
                #region 福利、奖罚
                List<daoben_payroll_office_sub> payrollSubList = new List<daoben_payroll_office_sub>();
                GetEmpBenefitList(empInfo.id, payrollSubList, payrollInfo.month, db);
                GetEmpRewardList(empInfo.id, payrollSubList, payrollInfo.month, db);
                #endregion

                #region 个人专有工资信息
                daoben_salary_emp_general seGeneral = db.Queryable<daoben_salary_emp_general>()
                        .JoinTable<daoben_salary_emp>((a, b) => b.id == a.main_id)
                        .Where<daoben_salary_emp>((a, b) => b.emp_id == empInfo.id && b.effect_status == 1 && b.category == 1)
                        .Select("a.resign_deposit,a.traffic_subsidy,a.intern_salary_type,a.intern_fix_salary,a.intern_ratio_salary")
                        .SingleOrDefault();
                if (seGeneral == null)   // 无个人专有的工资信息
                {
                    seGeneral = db.Queryable<daoben_salary_position_other>()
                            .JoinTable<daoben_salary_position>((a, b) => a.salary_position_id == b.id)
                            .Where<daoben_salary_position>((a, b) => b.company_id == empInfo.company_id && b.effect_status == 1 && b.category == 3)
                            .Select<daoben_salary_emp_general>("a.salary_position_id as main_id, a.resign_deposit,a.intern_salary_type,a.intern_fix_salary,a.intern_ratio_salary")
                            .SingleOrDefault();
                    if (seGeneral == null)
                        return "noOfficeSalarySetting";
                    seGeneral.traffic_subsidy = db.Queryable<daoben_salary_position_traffic>()
                            .Where(a => a.salary_position_id == seGeneral.main_id)
                            .Select(a => a.traffic_subsidy).SingleOrDefault();
                }
                payrollInfo.resign_deposit = seGeneral.resign_deposit < 0 ? seGeneral.resign_deposit : 0 - seGeneral.resign_deposit;
                payrollInfo.traffic_subsidy = seGeneral.traffic_subsidy;
                if (empInfo.emp_category == "实习生" && seGeneral.intern_salary_type == 2) // 实习生固定工资
                {
                    payrollInfo.intern_salary_type = seGeneral.intern_salary_type;
                    payrollInfo.intern_salary = payrollInfo.salary = seGeneral.intern_fix_salary;
                    object retInternObj = new
                    {
                        empInfo = empInfo,                  // 员工信息
                        payrollInfo = payrollInfo,          // 工资信息
                        payrollSubList = payrollSubList,    // 福利奖罚等
                    };
                    return retInternObj.ToJson();
                }
                else if (empInfo.emp_category == "实习生")
                {
                    payrollInfo.intern_salary_type = 1;
                    payrollInfo.intern_salary = seGeneral.intern_ratio_salary; //实习工资按比例发放，结算前占用该字段存放比例
                }
                //  payrollInfo.salary += payrollInfo.resign_deposit;
                payrollInfo.salary += (payrollInfo.traffic_subsidy > 0 ? payrollInfo.traffic_subsidy : 0);

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
                //1.25 18：58
                //payrollInfo.seniority_salary = db.Queryable<daoben_salary_position_seniority>()
                //        .JoinTable<daoben_salary_position>((a, b) => a.salary_position_id == b.id)
                //        .Where<daoben_salary_position>((a, b) => b.company_id == empInfo.company_id && b.effect_status == 1 && b.category == 3
                //        && a.year_min <= seniorityY && a.year_max >= seniorityY)
                //        .Select<decimal>("a.salary").SingleOrDefault();
                List<daoben_salary_position_seniority> seniorityList = db.Queryable<daoben_salary_position_seniority>()
                        .JoinTable<daoben_salary_position>((a, b) => a.salary_position_id == b.id)
                        .Where<daoben_salary_position>((a, b) => b.company_id == empInfo.company_id && b.effect_status == 1 && b.category == 3
                            && a.year_min <= seniorityY)
                        .Select("a.*").ToList();
                daoben_salary_position_seniority tempSeniority = seniorityList.OrderByDescending(t => t.year_min).FirstOrDefault();
                if (tempSeniority != null)
                    payrollInfo.seniority_salary = tempSeniority.salary;
                //1.25 19：16
                payrollInfo.salary += payrollInfo.seniority_salary;
                #endregion

                #region KPI
                payrollInfo.kpi_standard = db.Queryable<daoben_salary_position_dept>()
                            .JoinTable<daoben_salary_position>((a, b) => a.salary_position_id == b.id)
                            .Where<daoben_salary_position>((a, b) => b.category == 13 && b.effect_status == 1
                            && b.dept_id == empInfo.dept_id && a.grade == empInfo.grade)
                            .Select<decimal>("a.kpi_standard").SingleOrDefault();
                daoben_salary_emp_kpi_subsidy seKpiSubsidy = db.Queryable<daoben_salary_emp_kpi_subsidy>()
                        .JoinTable<daoben_salary_emp>((a, b) => b.id == a.main_id)
                        .Where<daoben_salary_emp>((a, b) => b.emp_id == empInfo.id && b.effect_status == 1 && b.category == 2)
                        .Select("a.kpi_subsidy,a.kpi_subsidy_full,a.kpi_subsidy_note")
                        .SingleOrDefault();
                daoben_salary_kpi kpiScore = db.Queryable<daoben_salary_kpi>()
                            .Where(a => a.category == 1 && a.approve_status == 100 && a.emp_id == id && a.month == payrollInfo.month)
                            .Select("kpi,kpi_subsidy").SingleOrDefault();
                if (kpiScore != null)
                {
                    payrollInfo.kpi_standard_score = kpiScore.kpi;
                    payrollInfo.kpi = payrollInfo.kpi_standard * kpiScore.kpi / 100;
                }
                if (seKpiSubsidy != null)
                {
                    payrollInfo.kpi_subsidy = seKpiSubsidy.kpi_subsidy;
                    if (seKpiSubsidy.kpi_subsidy_full)
                    {
                        payrollInfo.kpi += payrollInfo.kpi_subsidy;
                        payrollInfo.kpi_subsidy_score = -1;
                    }
                    else
                    {
                        payrollInfo.kpi_subsidy_score = (kpiScore != null ? kpiScore.kpi_subsidy : payrollInfo.kpi_standard_score);
                        payrollInfo.kpi += (payrollInfo.kpi_subsidy * payrollInfo.kpi_subsidy_score / 100);
                    }
                }
                payrollInfo.salary += payrollInfo.kpi;
                #endregion

                #region 其他补助
                payrollInfo.salary_subsidy = db.Queryable<daoben_salary_emp_subsidy>()
                       .JoinTable<daoben_salary_emp>((a, b) => b.id == a.main_id)
                       .Where<daoben_salary_emp>((a, b) => b.emp_id == empInfo.id && b.effect_status == 1 && b.category == 3)
                       .Select<decimal>("a.subsidy").SingleOrDefault();
                payrollInfo.salary += payrollInfo.salary_subsidy;
                #endregion
                #region 留守补贴
                daoben_salary_staysubsidy staySubsidy = db.Queryable<daoben_salary_staysubsidy_emp>()
                            .JoinTable<daoben_salary_staysubsidy>((a, b) => b.id == a.main_id)
                            .Where<daoben_salary_staysubsidy>((a, b) => a.emp_id == id && b.approve_status == 100 && b.month == payrollInfo.month)
                            .Select<daoben_salary_staysubsidy>("b.company_amount,b.emp_amount").FirstOrDefault();
                #endregion
//#if false
                #region 出勤
                daoben_hr_attendance attendanceInfo = db.Queryable<daoben_hr_attendance>()
                           .JoinTable<daoben_hr_attendance_approve>((a, b) => a.import_file_id == b.id)
                           .Where<daoben_hr_attendance_approve>((a, b) => a.emp_name == empInfo.name && b.status == 100
                           && a.month == monthInfo.month)
                           .Select("a.work_days, a.attendance, a.attendance_rate").SingleOrDefault();
                if (attendanceInfo != null)
                {
                    getInfo.work_days = attendanceInfo.work_days;
                    getInfo.onduty_day = attendanceInfo.attendance;
                    if (attendanceInfo.attendance_rate < 100)
                        getInfo.attendance_reward = 0;
                    else
                        getInfo.attendance_reward = payrollTemp.attendance_reward;
                }
                else
                    getInfo.attendance_reward = -1;
                getInfo.seniority_salary = payrollTemp.seniority_salary;
                getInfo.traffic_subsidy = payrollTemp.traffic_subsidy;
                getInfo.salary_subsidy = payrollTemp.salary_subsidy;
                getInfo.resign_deposit = payrollTemp.resign_deposit;

                getInfo.salary = getInfo.base_salary + getInfo.position_salary + getInfo.house_subsidy + getInfo.attendance_reward
                            + getInfo.seniority_salary + getInfo.traffic_subsidy + getInfo.salary_subsidy + getInfo.kpi;
                #endregion

#endif
                //object retObj = new
                //{
                //    empInfo = empInfo,                  // 员工信息
                //    payrollInfo = payrollInfo,          // 工资信息
                //    payrollSubList = payrollSubList,    // 福利奖罚等
                //    staySubsidy = staySubsidy,          //留守补助
                //    seniorityStr = seniorityY.ToString() + "年" + seniorityM.ToString() + "个月",       // 工龄
                //    seKpiSubsidy = seKpiSubsidy
                //};
                //return retObj.ToJson();
            }

        }

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
                    daoben_payroll_office mainInfo = db.Queryable<daoben_payroll_office>().SingleOrDefault(a => a.id == id);
                    if (mainInfo == null)
                        return "信息错误：指定的信息不存在";
                    daoben_hr_emp_job empInfo = db.Queryable<daoben_hr_emp_job>().SingleOrDefault(a => a.id == mainInfo.emp_id);
                    List<daoben_payroll_office_approve> approveList = db.Queryable<daoben_payroll_office_approve>().Where(a => a.main_id == id).ToList();
                    List<daoben_payroll_office_sub> subList = db.Queryable<daoben_payroll_office_sub>().Where(a => a.main_id == id).ToList();
                    daoben_salary_emp_kpi_subsidy seKpiSubsidy = db.Queryable<daoben_salary_emp_kpi_subsidy>()
                                    .JoinTable<daoben_salary_emp>((a, b) => b.id == a.main_id)
                                    .Where<daoben_salary_emp>((a, b) => b.emp_id == empInfo.id && b.effect_status == 1 && b.category == 2)
                                    .Select("a.kpi_subsidy,a.kpi_subsidy_full,a.kpi_subsidy_note")
                                    .SingleOrDefault();
                    object resultObj = new
                    {
                        mainInfo = mainInfo,
                        empInfo = empInfo,
                        approveList = approveList,
                        subList = subList,
                        seKpiSubsidy = seKpiSubsidy
                    };
                    return resultObj.ToJson();
                }
                catch (Exception ex)
                {
                    return ex.Message;
                }
            }
        }
        public string Add(daoben_payroll_office addInfo, List<daoben_payroll_office_sub> subList)
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
                    addInfo.name = empInfo.name;
                    //addInfo.job_history_id = jobHistory.id;
                    addInfo.approve_status = 0;
                    addInfo.creator_job_history_id = LoginInfo.jobHistoryId;
                    addInfo.creator_name = LoginInfo.empName;
                    addInfo.create_time = DateTime.Now;
                    db.CommandTimeOut = 30;
                    db.BeginTran();
                    db.Insert(addInfo);
                    db.DisableInsertColumns = new string[] { "id" };
                    db.InsertRange(subList);
                    db.CommitTran();

                }
                catch (Exception ex)
                {
                    db.RollbackTran();
                    return "系统出错：" + ex.Message;
                }
                //1.2
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
                db.Update<daoben_payroll_office>(upObj, a => a.id == id);
            }
        }

        /// <summary>
        /// 获取员工福利列表
        /// </summary>
        /// <param name="empId"></param>
        /// <param name="payrollSubList"></param>
        /// <param name="month"></param>
        /// <param name="db"></param>
        private void GetEmpBenefitList(string empId, List<daoben_payroll_office_sub> payrollSubList, DateTime? month, SqlSugarClient db)
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
                    daoben_payroll_office_sub infoSub = new daoben_payroll_office_sub
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
        private void GetEmpRewardList(string empId, List<daoben_payroll_office_sub> payrollSubList, DateTime? month, SqlSugarClient db)
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
                    daoben_payroll_office_sub infoSub = new daoben_payroll_office_sub
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
    }
}