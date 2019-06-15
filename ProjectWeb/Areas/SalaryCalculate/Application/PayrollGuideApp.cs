using Base.Code;
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
    public class PayrollGuideApp
    {
        public object GetListHistory(Pagination pagination, daoben_hr_emp_job queryInfo, QueryTime queryTime)
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
                var qable = db.Queryable<daoben_payroll_guide>()
                            .JoinTable<daoben_hr_emp_job>((a, b) => b.id == a.emp_id)
                            .JoinTable<daoben_payroll_guide_sub>((a, c) => a.id == c.main_id);
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
                if (queryTime.endTime1 != null)//月份
                {
                    DateTime tempTime = queryTime.endTime1.ToDate().AddMonths(1);
                    qable.Where(a => a.month >= queryTime.endTime1 && a.month < tempTime);
                }
                if (queryTime.startTime1 != null)
                    qable.Where<daoben_hr_emp_job>((a, b) => b.entry_date >= queryTime.startTime1);
                if (queryTime.startTime2 != null)
                {
                    DateTime tempTime = queryTime.startTime2.ToDate().AddDays(1);
                    qable.Where<daoben_hr_emp_job>((a, b) => b.entry_date < tempTime);
                }
                if (!string.IsNullOrEmpty(queryInfo.guide_category))
                    qable.Where<daoben_hr_emp_job>((a, b) => b.guide_category == queryInfo.guide_category);
                string list = qable
                            .Select("SQL_CALC_FOUND_ROWS SUM(c.amount) AS sub_amount,a.id ,a.name ,a.month,a.sale_count,a.sale_commission,a.base_salary,a.buyout_commission,a.increase_reward,a.insurance_fee,a.salary,a.resign_deposit,a.leaving_deduction,a.actual_salary,a.note,b.name as emp_name,b.work_number,b.name_v2,b.position_name,b.area_l1_name,b.area_l2_name,b.grade,b.dept_name,b.company_linkname,b.emp_category,b.entry_date,b.status")
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
                var qable = db.Queryable<daoben_payroll_guide>()
                            .JoinTable<daoben_hr_emp_job>((a, b) => a.emp_id == b.id)
                            .JoinTable<daoben_payroll_guide_sub>((a, c) => a.id == c.main_id);

                //工资结算-导购
                string str = "b.area_l2_name,b.name,b.name_v2,b.work_number,DATE_FORMAT(a.month,'%Y-%m'),a.sale_count,a.sale_commission,a.base_salary,a.buyout_commission,a.increase_reward,a.salary,a.resign_deposit, c.amount as sub_amount, a.actual_salary,a.note";
                var listDt = qable.OrderBy(pagination.sidx + " " + pagination.sord)
                    .GroupBy("a.id")
                    .Select(str)
                    .ToDataTable();
                int rowsCount = listDt.Rows.Count;
                string[] headerArr = new string[]
                { "业务片区","姓名","v2姓名","工号","结算月份","当月实销","实销提成","底薪","买断机提成","介绍费奖励","应发工资",
            "风险金","其他","实发工资","其他款备注",
                };
                int[] colWidthArr = new int[] { 20, 15, 15, 15, 15, 15, 15, 15, 15, 15, 15, 15, 15, 15, 15, 25 };

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
                object list = db.Queryable<daoben_payroll_guide>()   // todo
                            .Where(a => a.approve_status == 0)
                            .OrderBy(pagination.sidx + " " + pagination.sord)
                            .ToJsonPage(pagination.page, pagination.rows, ref records);
                pagination.records = records;
                return list;
            }
        }

        public object GetListCalculate(Pagination pagination, daoben_hr_emp_job queryInfo, QueryTime queryTime, bool showAll)
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
                        .JoinTable<daoben_payroll_guide>((a, b) => a.id == b.emp_id && b.month == curPayrollMonth)
                        .Where(a => a.position_type == ConstData.POSITION_GUIDE);
                if (myCompanyInfo.category == "事业部")
                    qable.Where(a => a.company_id_parent == myCompanyInfo.id);
                else if (myCompanyInfo.category == "分公司")
                    qable.Where(a => a.company_id == myCompanyInfo.id);
                if (!showAll)
                    qable.Where<daoben_payroll_guide>((a, b) => b.id == null && a.entry_date < curPayrollMonth.AddDays(16));
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
                    if (!string.IsNullOrEmpty(queryInfo.guide_category))
                        qable.Where(a => a.guide_category == queryInfo.guide_category);
                    if (queryTime.startTime1 != null)
                        qable.Where(a => a.entry_date >= queryTime.startTime1);
                    if (queryTime.startTime2 != null)
                    {
                        DateTime tempTime = queryTime.startTime2.ToDate().AddDays(1);
                        qable.Where(a => a.entry_date < tempTime);
                    }
                }
                string listStr = qable.OrderBy(pagination.sidx + " " + pagination.sord)
                       .Select("a.id,a.name,a.name_v2,a.work_number,a.position_name,a.grade,a.area_l1_name,a.area_l2_name,a.dept_name,a.company_name,a.emp_category,a.entry_date,a.status,b.id as calculated")
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
                //月度销售总额
                daoben_sale_month_emp origMonthInfo = db.Queryable<daoben_sale_month_emp>()
                    .SingleOrDefault(a => a.emp_id == empInfo.id && a.month == payrollMonth.month);

                daoben_payroll_guide payrollInfo = new daoben_payroll_guide
                {
                    month = payrollMonth.month,
                    start_date = payrollMonth.start_date,
                    end_date = (DateTime)payrollMonth.end_date,
                    create_time = now,
                    creator_job_history_id = LoginInfo.jobHistoryId,
                    creator_name = LoginInfo.empName,
                    approve_status = 0,
                };
                payrollInfo.name = empInfo.name;
                //payrollInfo.
                //payrollInfo.job_history_id
                startTime = payrollInfo.start_date;
                endTime = payrollInfo.end_date.Date.AddDays(1);
                #region 福利、奖罚
                List<daoben_payroll_guide_sub> payrollSubList = new List<daoben_payroll_guide_sub>();
                GetEmpBenefitList(empInfo.id, payrollSubList, payrollInfo.month, db);
                GetEmpRewardList(empInfo.id, payrollSubList, payrollInfo.month, db);
                #endregion

                #region 实销提成 买断提成 包销提成
                //提成规则
                List<daoben_product_commission_detail> commDetailList = db.Queryable<daoben_product_commission_detail>()
                                .Where(a => a.company_id == empInfo.company_id
                                && a.effect_date <= endTime && a.expire_date >= startTime && a.effect_status > 0)
                                .Select("id, model, color, effect_date, expire_date,salary_include")
                                .ToList();
                List<daoben_product_sn_outlay> guideSnList = db.Queryable<daoben_product_sn_outlay>()
                        .JoinTable<daoben_product_sn>((a, b) => a.phone_sn == b.phone_sn)
                        .Where<daoben_product_sn>((a, b) => a.category == 31 && a.outlay_type == 1
                        && a.time >= startTime && a.time < endTime && b.reporter_id == empInfo.id)
                        .Select("a.phone_sn,a.outlay, b.sale_type as outlay_type, a.time, b.model, b.color") // 占用outlay_type作sale_type
                        .ToList();  // 同一串码最多只能有一个导购提成
                payrollInfo.buyout_commission = 0;//买断提成
                payrollInfo.exclusive_commission = 0;//包销提成  
                payrollInfo.sale_commission = 0; //实销提成
                payrollInfo.commission_basesalary = 0; //核算底薪的提成
                foreach (var a in guideSnList)
                {
                    DateTime saleDate = a.time.ToDate().Date;
                    daoben_product_commission_detail commissionModel = commDetailList
                                .Where(c => c.model == a.model && c.color == a.color
                                && c.effect_date <= saleDate && c.expire_date >= saleDate)
                                .OrderByDescending(c => c.effect_date).FirstOrDefault();//按照生效时间倒序 取最大值
                    if (commissionModel == null || !commissionModel.salary_include)
                        payrollInfo.commission_basesalary += a.outlay; // 默认核算底薪
                    if (a.outlay_type == 1) // 占用outlay_type作sale_type
                    {
                        payrollInfo.sale_commission += a.outlay;
                        payrollInfo.sale_count++;
                    }
                    else if (a.outlay_type == 2)
                    {
                        payrollInfo.buyout_commission += a.outlay;
                        payrollInfo.buyout_count++;
                    }
                    else if (a.outlay_type == 3)
                    {
                        payrollInfo.exclusive_commission += a.outlay;
                        payrollInfo.exclusive_count++;
                    }
                }

                #endregion

                #region 导购员底薪
                daoben_salary_emp_general seGeneral = db.Queryable<daoben_salary_emp_general>()
                        .JoinTable<daoben_salary_emp>((a, b) => b.id == a.main_id)
                        .Where<daoben_salary_emp>((a, b) => b.emp_id == empInfo.id && b.effect_status == 1 && b.category == 1)
                        .Select("a.resign_deposit,a.traffic_subsidy,a.intern_salary_type,a.intern_fix_salary,a.intern_ratio_salary,a.guide_base_type,a.guide_salary_base,a.guide_standard_salary,a.guide_standard_commission,a.guide_annualbonus_type")
                        .SingleOrDefault();
                daoben_salary_position_guide guideInfo = db.Queryable<daoben_salary_position_guide>()
                       .JoinTable<daoben_salary_position>((a, b) => a.salary_position_id == b.id)
                       .Where<daoben_salary_position>((a, b) => b.position_id == empInfo.position_id && b.effect_status == 1)
                       .Select("a.*").SingleOrDefault();
                if (guideInfo == null)
                    return "noGuidesSalarySetting";

                if (seGeneral == null)   // 没有配置 员工薪资设置
                {
                    seGeneral = new daoben_salary_emp_general();
                    //风险金/离职押金
                    seGeneral.resign_deposit = db.Queryable<daoben_salary_position_other>()
                            .JoinTable<daoben_salary_position>((a, b) => a.salary_position_id == b.id)
                            .Where<daoben_salary_position>((a, b) => b.company_id == empInfo.company_id && b.effect_status == 1 && b.category == 3)
                            .Select<int>("a.resign_deposit")
                            .SingleOrDefault();
                    List<daoben_salary_position_guide_sub> subList = db.Queryable<daoben_salary_position_guide_sub>()
                        .Where(a => a.main_id == guideInfo.id).ToList();
                    seGeneral.guide_base_type = guideInfo.guide_base_type;
                    seGeneral.guide_salary_base = guideInfo.guide_salary_base;
                    seGeneral.guide_standard_salary = guideInfo.standard_salary;
                    seGeneral.guide_standard_commission = guideInfo.standard_commission;
                    seGeneral.guide_annualbonus_type = guideInfo.guide_annualbonus_type;
                }
                payrollInfo.resign_deposit = seGeneral.resign_deposit < 0 ? seGeneral.resign_deposit : 0 - seGeneral.resign_deposit;
                //社保 请假扣款
                payrollInfo.insurance_fee = 0;
                payrollInfo.leaving_deduction = 0;
                //payrollInfo.deposit_total  TODO 累计
                decimal deposit_total = db.Queryable<daoben_payroll_guide>()
                        .Where(t => t.emp_id == payrollInfo.emp_id)
                        .Select<decimal>("SUM(resign_deposit)").ToDecimal();
                payrollInfo.deposit_total = deposit_total > 0 ? 0 - deposit_total : deposit_total;
                payrollInfo.base_type = seGeneral.guide_base_type;
                if (payrollInfo.base_type == 0)//0底薪
                {
                    payrollInfo.base_salary = 0;
                }
                else if (payrollInfo.base_type == 1)//达标底薪
                {
                    if (payrollInfo.commission_basesalary >= seGeneral.guide_standard_commission)
                        payrollInfo.base_salary = seGeneral.guide_standard_salary;
                    else
                        payrollInfo.base_salary = 0;
                }
                else if (payrollInfo.base_type == 2)
                {
                    payrollInfo.base_salary = 0;
                    //星级制
                    List<daoben_salary_position_guide_sub> subTempList = db.Queryable<daoben_salary_position_guide_sub>()
                        .JoinTable<daoben_salary_position_guide>((a, b) => a.main_id == b.id)
                        .JoinTable<daoben_salary_position_guide, daoben_salary_position>((a, b, c) => b.salary_position_id == c.id)
                        .Where<daoben_salary_position_guide, daoben_salary_position>((a, b, c) => c.effect_status == 1 && c.position_id == empInfo.position_id && a.category == 1)
                        .Select("a.*")
                        .ToList();
                    if (subTempList != null)
                    {
                        daoben_salary_position_guide_sub subTempInfo = subTempList.SingleOrDefault(t => t.level == empInfo.grade);
                        if (subTempInfo != null)
                            payrollInfo.base_salary = subTempInfo.amount;
                    }
                }
                else if (payrollInfo.base_type == 3)
                {
                    payrollInfo.base_salary = 0;
                    //浮动底薪
                    List<daoben_salary_position_guide_sub> subTempList = db.Queryable<daoben_salary_position_guide_sub>()
                        .JoinTable<daoben_salary_position_guide>((a, b) => a.main_id == b.id)
                        .JoinTable<daoben_salary_position_guide, daoben_salary_position>((a, b, c) => b.salary_position_id == c.id)
                        .Where<daoben_salary_position_guide, daoben_salary_position>((a, b, c) => c.effect_status == 1 && c.position_id == empInfo.position_id && a.category == 2)
                        .Select("a.*")
                        .ToList();
                    if (subTempList != null)
                    {
                        subTempList = subTempList.OrderByDescending(t => t.target_min).ToList();

                        daoben_salary_position_guide_sub subTempInfo = subTempList.Where(a => payrollInfo.sale_count >= a.target_min).First();
                        if (subTempInfo != null)
                            payrollInfo.base_salary = subTempInfo.amount;
                    }
                }
                else if (payrollInfo.base_type == 4)
                {
                    payrollInfo.base_salary = seGeneral.guide_salary_base;
                }

                #endregion

                //TODO monthInfo
                #region 增员奖 
                daoben_salary_position_guide guidePosInfo = db.Queryable<daoben_salary_position_guide>()
                        .JoinTable<daoben_salary_position>((a, b) => a.salary_position_id == b.id)
                        .Where<daoben_salary_position>((a, b) => b.position_id == empInfo.position_id && b.effect_status == 1)
                        .Select("a.*").SingleOrDefault();
                if (guidePosInfo.increase_award_status == 0)
                {
                    payrollInfo.increase_commission = 0;
                    payrollInfo.increase_count = 0;
                    payrollInfo.increase_reward = 0;
                    payrollInfo.increase_sale_count = 0;
                }
                else if (guidePosInfo.increase_award_status == 1)
                {
                    List<daoben_hr_emp_job> empIncreaseList = db.Queryable<daoben_hr_emp_job>()
                        .Where(a => a.introducer_id == empInfo.id && a.entry_date <= guidePosInfo.increase_effect_time)
                        .ToList();

                    List<daoben_hr_emp_resigned_job> empResignedList = db.Queryable<daoben_hr_emp_resigned_job>()
                        .JoinTable<daoben_hr_emp_job>((a, b) => a.id == b.id)
                        .Where<daoben_hr_emp_job>((a, b) => b.introducer_id == empInfo.id && b.entry_date <= guidePosInfo.increase_effect_time)
                        .Select("a.*")
                        .ToList();
                    if (empResignedList != null && empResignedList.Count() > 0)//移除在保护期之内入职的导购员
                    {
                        foreach (var a in empResignedList)
                        {
                            daoben_hr_emp_job tempJob = empIncreaseList.Where(t => t.id == a.id).SingleOrDefault();
                            if (tempJob != null)
                            {
                                DateTime resignDate = a.resign_date.ToDate();
                                if (resignDate.AddMonths(guidePosInfo.increase_protect) >= tempJob.entry_date)
                                {
                                    empIncreaseList.Remove(tempJob);
                                }
                            }
                        }
                    }
                    payrollInfo.increase_count = empIncreaseList.Count();
                    payrollInfo.increase_reward = 0;
                    List<string> idList = empIncreaseList.Select(t => t.id).ToList();
                    List<daoben_sale_salesinfo> increaseSaleList = db.Queryable<daoben_sale_salesinfo>()
                        .Where(t => idList.Contains(t.reporter_id)).ToList();
                    payrollInfo.increase_sale_count = increaseSaleList.Count();
                    payrollInfo.increase_commission = payrollInfo.increase_sale_count * guidePosInfo.increase_commission;
                    foreach (var a in empIncreaseList)
                    {
                        if (a.entry_date > now.AddMonths(guidePosInfo.increase_month))
                        {
                            payrollInfo.increase_reward += guidePosInfo.increase_salary;
                        }
                    }
                }

                #endregion

                #region 达量活动 主推活动  pk比赛 排名比赛 

                List<daoben_payroll_guide_sub> totalSub = new List<daoben_payroll_guide_sub>();
                //达量活动
                List<daoben_activity_attaining> attainingActivity = db.Queryable<daoben_activity_attaining>()
                    .Where(t => t.emp_category == 3 && t.end_date < endTime && t.end_date >= startTime)
                    .Where(t => t.company_id == empInfo.company_id).ToList();
                List<daoben_activity_attaining_emp> attainingEmpList = db.Queryable<daoben_activity_attaining_emp>()
                    .JoinTable<daoben_activity_attaining>((a, b) => a.main_id == b.id)
                    .Where<daoben_activity_attaining>((a, b) => b.emp_category == 3 && b.end_date < endTime && b.end_date >= startTime)
                    .Where(a => a.emp_id == empInfo.id)
                    .Select("a.*").ToList();
                foreach (var a in attainingEmpList)
                {
                    daoben_activity_attaining tempActivity = attainingActivity.Where(t => t.id == a.main_id).SingleOrDefault();
                    if (tempActivity != null)
                    {
                        daoben_payroll_guide_sub tempSub = new daoben_payroll_guide_sub()
                        {
                            amount = a.total_reward,
                            category = 2,
                            category_id = tempActivity.id,
                            sub_name = tempActivity.name,
                            sub_note = "达量活动",
                        };
                        totalSub.Add(tempSub);//temp.note
                    }
                }
                //主推活动
                List<daoben_activity_recommendation> recomActivity = db.Queryable<daoben_activity_recommendation>()
                    .Where(t => t.emp_category == 3 && t.end_date < endTime && t.end_date >= startTime)
                    .Where(t => t.company_id == empInfo.company_id).ToList();
                List<daoben_activity_recommendation_emp> recomEmpList = db.Queryable<daoben_activity_recommendation_emp>()
                    .JoinTable<daoben_activity_recommendation>((a, b) => a.main_id == b.id)
                    .Where<daoben_activity_recommendation>((a, b) => b.emp_category == 3 && b.end_date < endTime && b.end_date >= startTime)
                    .Where(a => a.emp_id == empInfo.id)
                    .Select("a.*").ToList();
                foreach (var a in recomEmpList)
                {
                    daoben_activity_recommendation tempActivity = recomActivity.Where(t => t.id == a.main_id).SingleOrDefault();
                    if (tempActivity != null)
                    {
                        daoben_payroll_guide_sub tempSub = new daoben_payroll_guide_sub()
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
                    .Where(t => t.emp_category == 3 && t.end_date < endTime && t.end_date >= startTime)
                    .Where(t => t.company_id == empInfo.company_id).ToList();
                List<daoben_activity_ranking_emp> rankEmpList = db.Queryable<daoben_activity_ranking_emp>()
                    .JoinTable<daoben_activity_ranking>((a, b) => a.main_id == b.id)
                    .Where<daoben_activity_ranking>((a, b) => b.emp_category == 3 && b.end_date < endTime && b.end_date >= startTime)
                    .Where(a => a.emp_id == empInfo.id)
                    .Select("a.*").ToList();
                foreach (var a in rankEmpList)
                {
                    daoben_activity_ranking tempActivity = rankActivity.Where(t => t.id == a.main_id).SingleOrDefault();
                    if (tempActivity != null)
                    {
                        daoben_payroll_guide_sub tempSub = new daoben_payroll_guide_sub()
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
                    .Where(t => t.emp_category == 3 && t.end_date < endTime && t.end_date >= startTime)
                    .Where(t => t.company_id == empInfo.company_id).ToList();
                List<daoben_activity_pk_emp> pkEmpList = db.Queryable<daoben_activity_pk_emp>()
                    .JoinTable<daoben_activity_pk>((a, b) => a.main_id == b.id)
                    .Where<daoben_activity_pk>((a, b) => b.emp_category == 3 && b.end_date < endTime && b.end_date >= startTime)
                    .Where(a => a.emp_id == empInfo.id)
                    .Select("a.*").ToList();
                foreach (var a in pkEmpList)
                {
                    daoben_activity_pk tempActivity = pkActivity.Where(t => t.id == a.main_id).SingleOrDefault();
                    if (tempActivity != null)
                    {
                        daoben_payroll_guide_sub tempSub = new daoben_payroll_guide_sub()
                        {
                            amount = a.total_reward,
                            category = 4,
                            category_id = tempActivity.id,
                            sub_name = tempActivity.name,
                            sub_note = "PK比赛",
                        };
                        totalSub.Add(tempSub);//temp.note
                    }
                }


                #endregion

                #region 年终奖TODO
                if (payrollInfo.month.Month == 12)
                {
                    List<daoben_salary_position_guide_sub> subList = db.Queryable<daoben_salary_position_guide_sub>()
                            .Where(a => a.main_id == guideInfo.id).ToList();
                    decimal rewardAnnualbonus = 0;
                    if (seGeneral.guide_annualbonus_type == 1)
                    {
                        DateTime tempStartTime = payrollInfo.end_date.AddYears(-1);
                        int wholeYearCount = db.Queryable<daoben_product_sn>()
                        .Where(t => t.reporter_id == payrollInfo.emp_id && t.sale_time < endTime && t.sale_time >= tempStartTime)
                        .Select<int>("COUNT(DISTINCT phone_sn)").ToInt();
                        //销量
                        daoben_salary_position_guide_sub modelSub = subList.Where(t => wholeYearCount >= t.target_min && t.category == 3)
                                .OrderByDescending(t => t.target_min).FirstOrDefault();
                        if (modelSub != null)
                            rewardAnnualbonus = modelSub.amount;
                    }
                    else
                    {
                        //星级
                        daoben_salary_position_guide_sub modelSub = subList.FirstOrDefault(t => empInfo.grade == t.level && t.category == 4);
                        if (modelSub != null)
                            rewardAnnualbonus = modelSub.amount;
                    }
                    daoben_payroll_guide_sub annualbonusSub = new daoben_payroll_guide_sub()
                    {
                        amount = rewardAnnualbonus,
                        category = 13,
                        sub_name = "导购员年终奖",
                        sub_note = "导购员年终奖",
                    };
                    payrollSubList.Add(annualbonusSub);
                }
                #endregion
                totalSub.AddRange(payrollSubList);
                #region 留守补贴
                daoben_salary_staysubsidy staySubsidy = db.Queryable<daoben_salary_staysubsidy_emp>()
                            .JoinTable<daoben_salary_staysubsidy>((a, b) => b.id == a.main_id)
                            .Where<daoben_salary_staysubsidy>((a, b) => a.emp_id == id && b.approve_status == 100 && b.month == payrollInfo.month)
                            .Select<daoben_salary_staysubsidy>("b.company_amount,b.emp_amount").FirstOrDefault();
                #endregion

                object retObj = new
                {
                    empInfo = empInfo,                  // 员工信息
                    payrollInfo = payrollInfo,          // 工资信息
                    //annualbonusList = annualbonusList,  //年终奖 todo
                    staySubsidy = staySubsidy,          //留守补助
                    payrollSubList = totalSub,          // 活动(排名、主推、达量) 福利 奖罚
                    increase = guideInfo.increase_award_status, // 0-关闭 1-开启
                };
                return retObj.ToJson();
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
                    daoben_payroll_guide mainInfo = db.Queryable<daoben_payroll_guide>().SingleOrDefault(a => a.id == id);
                    if (mainInfo == null)
                        return "信息错误：指定的信息不存在";
                    daoben_hr_emp_job empInfo = db.Queryable<daoben_hr_emp_job>().SingleOrDefault(a => a.id == mainInfo.emp_id);
                    List<daoben_payroll_guide_approve> approveList = db.Queryable<daoben_payroll_guide_approve>().Where(a => a.main_id == id).ToList();
                    List<daoben_payroll_guide_sub> subList = db.Queryable<daoben_payroll_guide_sub>().Where(a => a.main_id == id).ToList();
                    daoben_salary_position_guide guidePosInfo = db.Queryable<daoben_salary_position_guide>()
                        .JoinTable<daoben_salary_position>((a, b) => a.salary_position_id == b.id)
                        .Where<daoben_salary_position>((a, b) => b.position_id == empInfo.position_id && b.effect_status == 1)
                        .Select("a.*").SingleOrDefault();
                    int increase_award_status = 0;
                    if (guidePosInfo != null)
                        increase_award_status = guidePosInfo.increase_award_status;
                    object resultObj = new
                    {
                        mainInfo = mainInfo,
                        empInfo = empInfo,
                        approveList = approveList,
                        subList = subList,
                        increase = increase_award_status, // 0-关闭 1-开启
                    };
                    return resultObj.ToJson();
                }
                catch (Exception ex)
                {
                    return ex.Message;
                }
            }
        }

        public string Add(daoben_payroll_guide addInfo, List<daoben_payroll_guide_sub> subList)
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
                    daoben_hr_emp_job empJob = db.Queryable<daoben_hr_emp_job>().SingleOrDefault(a => a.id == addInfo.emp_id);
                    if (empJob == null)
                    {
                        return "信息错误：指定的员工信息不存在";//TODO delete
                    }
                    else
                    {
                        addInfo.name = empJob.name;
                    }
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
                db.Update<daoben_payroll_guide>(upObj, a => a.id == id);
            }
        }

        /// <summary>
        /// 获取员工福利列表
        /// </summary>
        /// <param name="empId"></param>
        /// <param name="payrollSubList"></param>
        /// <param name="month"></param>
        /// <param name="db"></param>
        private void GetEmpBenefitList(string empId, List<daoben_payroll_guide_sub> payrollSubList, DateTime? month, SqlSugarClient db)
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
                    daoben_payroll_guide_sub infoSub = new daoben_payroll_guide_sub
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
        private void GetEmpRewardList(string empId, List<daoben_payroll_guide_sub> payrollSubList, DateTime? month, SqlSugarClient db)
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
                    daoben_payroll_guide_sub infoSub = new daoben_payroll_guide_sub
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
