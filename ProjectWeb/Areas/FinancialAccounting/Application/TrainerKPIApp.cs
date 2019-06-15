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
    public class TrainerKPIApp
    {
        /// <summary>
        /// 本月待考核列表
        /// </summary>
        /// <param name="pagination"></param>
        /// <param name="queryInfo"></param>
        /// <param name="showAll">所有人</param>
        /// <returns></returns>
        public object GetListKPI(Pagination pagination, daoben_hr_emp_job queryInfo, bool showAll)
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
            pagination.sord = string.IsNullOrEmpty(pagination.sord) ? "asc" : pagination.sord;
            CompanyInfo myCompanyInfo = LoginInfo.companyInfo;
            PositionInfo myPositionInfo = LoginInfo.positionInfo;
            DateTime now = DateTime.Now.Date;
            using (var db = SugarDao.GetInstance())
            {
                // 获取工资结算时间
                daoben_payroll_setting payrollMonth = new PayrollSettingApp().GetPayrollMonth(myCompanyInfo.id, db, true);
                if (payrollMonth == null)
                    return "系统出错：工资结算周期信息不存在";
                DateTime curPayrollMonth = payrollMonth.month; // 实际是结算上一月
                var qable = db.Queryable<daoben_hr_emp_job>()
                        .JoinTable<daoben_kpi_trainer>((a, b) => a.id == b.emp_id && b.month == curPayrollMonth && b.is_del == false)
                        .Where(a => (a.position_type == ConstData.POSITION_TRAINER || a.position_type == ConstData.POSITION_TRAINERMANAGER));
                if (myCompanyInfo.category == "事业部")
                    qable.Where(a => a.company_id_parent == myCompanyInfo.id);
                else if (myCompanyInfo.category == "分公司")
                    qable.Where(a => a.company_id == myCompanyInfo.id);
                if (!showAll)
                    qable.Where<daoben_kpi_trainer>((a, b) => b.id == null && a.entry_date < curPayrollMonth.AddDays(16));
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
                        .Select("a.id,a.name,a.name_v2,a.work_number,a.position_name,a.grade,a.dept_name,a.company_linkname,a.emp_category,a.entry_date,a.status,b.id as calculated")
                        .ToJsonPage(pagination.page, pagination.rows, ref records);
                pagination.records = records;
                if (string.IsNullOrEmpty(listStr) || listStr == "[]")
                    return null;
                return listStr.ToJson();

            }
        }

        /// <summary>
        /// KPI历史
        /// </summary>
        /// <param name="pagination"></param>
        /// <param name="queryInfo"></param>
        /// <returns></returns>
        public object GetListHistory(Pagination pagination)
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
                string queryStr = db.Queryable<daoben_kpi_trainer>()
                            .JoinTable<daoben_hr_emp_job>((a, b) => a.emp_id == b.id)
                            .Where(a => a.is_del == false)
                            .Select("a.id,a.emp_id,a.kpi_total,a.month, a.status,b.work_number,b.name as name,b.name_v2,b.position_name,b.grade,b.dept_name,b.company_linkname,b.emp_category,b.entry_date")
                            .OrderBy(pagination.sidx + " " + pagination.sord)
                            .ToJsonPage(pagination.page, pagination.rows, ref records);
                pagination.records = records;
                if (string.IsNullOrEmpty(queryStr) || queryStr == "[]")
                    return null;
                return queryStr.ToJson();
            }
        }

        /// <summary>
        /// TODO
        /// </summary>
        /// <param name="pagination"></param>
        /// <returns></returns>
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
                string listStr = db.Queryable<daoben_kpi_trainer>()
                            .JoinTable<daoben_hr_emp_job>((a, b) => a.emp_id == b.id)
                            .Select("a.id,a.emp_id,a.kpi_total,a.month, a.status,b.work_number,b.name_v2,b.position_name,b.grade,b.dept_name,b.company_linkname,b.emp_category,b.entry_date")
                            .OrderBy(pagination.sidx + " " + pagination.sord)
                            .ToJsonPage(pagination.page, pagination.rows, ref records);
                pagination.records = records;
                if (string.IsNullOrEmpty(listStr) || listStr == "[]")
                    return null;
                return listStr.ToJson();
            }
        }

        public object GetGuideSaleList(Pagination pagination, daoben_product_sn queryInfo, DateTime month, string emp_id, string queryName)
        {
            OperatorModel LoginInfo = OperatorProvider.Provider.GetCurrent();
            if (LoginInfo == null)
                throw new Exception("用户登陆过期，请重新登录");
            if (emp_id == null || month == null)
                return "信息错误，获取导购销售详情失败";
            int records = 0;
            if (pagination == null)
                pagination = new Pagination();
            pagination.page = pagination.page > 0 ? pagination.page : 1;
            pagination.rows = pagination.rows > 0 ? pagination.rows : 20;
            pagination.sidx = string.IsNullOrEmpty(pagination.sidx) ? "sale_time" : pagination.sidx;
            pagination.sord = string.IsNullOrEmpty(pagination.sord) ? "desc" : pagination.sord;
            CompanyInfo myCompanyInfo = LoginInfo.companyInfo;

            using (var db = SugarDao.GetInstance())
            {
                daoben_hr_emp_job empInfo = db.Queryable<daoben_hr_emp_job>().SingleOrDefault(t => t.id == emp_id);
                if (empInfo == null)
                    return null;
                daoben_payroll_setting payrollMonth = new PayrollSettingApp().GetPayrollMonth(empInfo.company_id_parent, db, true, month);
                if (payrollMonth == null)
                    return null;
                DateTime startDate = payrollMonth.start_date;
                DateTime endDate = payrollMonth.end_date.ToDate().AddDays(1);

                var qable = db.Queryable<daoben_product_sn>()
                    .JoinTable<daoben_hr_emp_job>((a, b) => a.reporter_id == b.id)
                    .Where<daoben_hr_emp_job>((a, b) => a.reporter_type == 1 && a.sale_time >= startDate && a.sale_time < endDate
                    && b.company_id == empInfo.company_id);
                //todo:匹配area_l1_id 

                if (queryInfo != null) //匹配查询参数
                {
                    if (!string.IsNullOrEmpty(queryInfo.phone_sn))
                        qable.Where(a => a.phone_sn.Contains(queryInfo.phone_sn));
                    if (!string.IsNullOrEmpty(queryInfo.model))
                        qable.Where(a => a.model.Contains(queryInfo.model));
                }
                if (!string.IsNullOrEmpty(queryName))
                    qable.Where<daoben_hr_emp_job>((a, b) => b.name.Contains(queryName));

                string fieldStr = ("b.name, a.phone_sn, a.model, a.color, a.price_wholesale, a.price_sale, a.sale_time");
                var listStr = qable.Select(fieldStr)
                                   .OrderBy(pagination.sidx + " " + pagination.sord)
                                   .ToJsonPage(pagination.page, pagination.rows, ref records);
                pagination.records = records;
                if (string.IsNullOrEmpty(listStr) || listStr == "[]")
                    return null;
                return listStr.ToJson();

            }

        }

        public object GetHighLevelSaleList(Pagination pagination, daoben_product_sn queryInfo, DateTime month, string emp_id, int high_level = 0)
        {
            OperatorModel LoginInfo = OperatorProvider.Provider.GetCurrent();
            if (LoginInfo == null)
                throw new Exception("用户登陆过期，请重新登录");
            if (emp_id == null || month == null)
                return "信息错误，获取导购销售详情失败";
            int records = 0;
            if (pagination == null)
                pagination = new Pagination();
            pagination.page = pagination.page > 0 ? pagination.page : 1;
            pagination.rows = pagination.rows > 0 ? pagination.rows : 20;
            pagination.sidx = string.IsNullOrEmpty(pagination.sidx) ? "sale_time" : pagination.sidx;
            pagination.sord = string.IsNullOrEmpty(pagination.sord) ? "desc" : pagination.sord;
            CompanyInfo myCompanyInfo = LoginInfo.companyInfo;

            using (var db = SugarDao.GetInstance())
            {
                daoben_hr_emp_job empInfo = db.Queryable<daoben_hr_emp_job>().SingleOrDefault(t => t.id == emp_id);
                if (empInfo == null)
                    return null;
                daoben_payroll_setting payrollMonth = new PayrollSettingApp().GetPayrollMonth(empInfo.company_id_parent, db, true, month);
                if (payrollMonth == null)
                    return null;
                DateTime startDate = payrollMonth.start_date;
                DateTime endDate = payrollMonth.end_date.ToDate().AddDays(1);
                //todo:匹配area_l1_id 
                var qable = db.Queryable<daoben_product_sn>()
                    .JoinTable<daoben_hr_emp_job>((a, b) => a.sales_id == b.id)
                    .Where<daoben_hr_emp_job>((a, b) => b.company_id == empInfo.company_id && a.sale_time >= startDate && a.sale_time < endDate);
                  
                if (queryInfo != null) //匹配查询参数
                {
                    if (!string.IsNullOrEmpty(queryInfo.phone_sn))
                        qable.Where(a => a.phone_sn.Contains(queryInfo.phone_sn));
                    if (!string.IsNullOrEmpty(queryInfo.model))
                        qable.Where(a => a.model.Contains(queryInfo.model));
                }
                if(high_level == 1)
                    qable.Where(a => a.high_level == true);
                else if (high_level == -1)
                    qable.Where(a => a.high_level == false);

                string fieldStr = ("a.phone_sn, a.model, a.color, a.high_level, a.price_wholesale, a.price_sale, a.sale_time");
                var listStr = qable.Select(fieldStr)
                                   .OrderBy(pagination.sidx + " " + pagination.sord)
                                   .ToJsonPage(pagination.page, pagination.rows, ref records);
                pagination.records = records;
                if (string.IsNullOrEmpty(listStr) || listStr == "[]")
                    return null;
                return listStr.ToJson();

            }

        }
        /// <summary>
        /// 获取KPI信息，用于评分页面
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
                daoben_hr_emp_job empInfo = db.Queryable<daoben_hr_emp_job>().Where(a => a.id == id).SingleOrDefault();
                if (empInfo == null)
                    return "系统出错：员工信息不存在";
                // 获取工资结算时间
                daoben_payroll_setting payrollMonth = new PayrollSettingApp().GetPayrollMonth(myCompanyInfo.id, db, true);
                if (payrollMonth == null)
                {
                    return "系统出错：工资结算周期信息不存在";
                }

                List<daoben_kpi_trainer_detail> detailList = GetCalculateRatio(db, empInfo.id, payrollMonth.month);

                List<daoben_salary_emp_trainer> empKPIList = db.Queryable<daoben_salary_emp_trainer>()
                            .JoinTable<daoben_salary_emp>((a, b) => a.main_id == b.id)
                            .Where<daoben_salary_emp>((a, b) => b.effect_status == 1 && b.emp_id == empInfo.id)
                            .Select("a.id, a.kpi_type,a.is_included,a.area_l1_id,a.area_l1_name")
                            .ToList();
                if (empKPIList == null || empKPIList.Count < 1)
                    return "noKPISetting";
                if (empInfo.position_type == ConstData.POSITION_TRAINER)
                {
                    daoben_salary_position_trainer positionKPI = db.Queryable<daoben_salary_position_trainer>()
                                .JoinTable<daoben_salary_position>((a, b) => a.salary_position_id == b.id)
                                .Where<daoben_salary_position>((a, b) => b.effect_status == 1 && b.position_id == empInfo.position_id)
                                .Select("a.*").SingleOrDefault();
                    if (positionKPI == null)
                        return "noPositionKPISetting";
                    object retObj = new
                    {
                        empInfo = empInfo,
                        positionKPI = positionKPI,
                        empKPIList = empKPIList,
                        month = payrollMonth.month,
                        detailList = detailList
                    };
                    return retObj.ToJson();
                }
                else
                {
                    daoben_salary_position_trainermanager positionKPI = db.Queryable<daoben_salary_position_trainermanager>()
                                .JoinTable<daoben_salary_position>((a, b) => a.salary_position_id == b.id)
                                .Where<daoben_salary_position>((a, b) => b.effect_status == 1 && b.position_id == empInfo.position_id)
                                .Select("a.*").SingleOrDefault();
                    if (positionKPI == null)
                        return "noPositionKPISetting";
                    object retObj = new
                    {
                        empInfo = empInfo,
                        positionKPI = positionKPI,
                        empKPIList = empKPIList,
                        month = payrollMonth.month,
                        detailList = detailList
                    };
                    return retObj.ToJson();
                }
            }

        }

        public string Add(daoben_kpi_trainer addInfo, List<daoben_kpi_trainer_detail> detailList)
        {
            var LoginInfo = OperatorProvider.Provider.GetCurrent();
            if (LoginInfo == null)
                return "登录超时，请重新登录";
            if (addInfo == null || detailList == null || detailList.Count < 1)
                return "信息错误";
            if (addInfo.month == null)
                return "信息错误：需要指定考核月份";
            CompanyInfo myCompanyInfo = LoginInfo.companyInfo;
            PositionInfo myPositionInfo = LoginInfo.positionInfo;
            addInfo.id = Common.GuId();
            detailList.ForEach(a => a.main_id = addInfo.id);
            using (var db = SugarDao.GetInstance())
            {
                try
                {
                    //daoben_kpi_trainer origInfo = db.Queryable<daoben_kpi_trainer>()
                    //        .Where(a => a.emp_id == addInfo.emp_id && a.month == addInfo.month).SingleOrDefault();
                    //if (origInfo != null && origInfo.status != 0)
                    //    return "信息错误：本月KPI考核已提交且已审批";

                    daoben_hr_emp_job empJob = db.Queryable<daoben_hr_emp_job>()
                                .Where(a => a.id == addInfo.emp_id).SingleOrDefault();
                    if (empJob == null)
                        return "信息错误：指定的员工信息不存在";
                    //if (empJob.position_type == ConstData.POSITION_TRAINER)
                    //{
                    //    if (detailList.Count != 9)
                    //        return "信息错误：需要指定完整的kpi信息";
                    //}
                    //else
                    //{
                    //    if (detailList.Count != 3)
                    //        return "信息错误：需要指定完整的kpi信息";
                    //}
                    addInfo.emp_job_history_id = empJob.cur_job_history_id; //TODO
                    addInfo.position_type = empJob.position_type;
                    addInfo.creator_job_history_id = LoginInfo.jobHistoryId;
                    addInfo.creator_position_name = myPositionInfo.name;
                    addInfo.creator_name = LoginInfo.empName;
                    addInfo.create_time = DateTime.Now;
                    addInfo.status = 100;                    // TODO 暂时不用审批
                    addInfo.is_del = false;
                    addInfo.company_id = empJob.company_id;
                    addInfo.company_id_parent = empJob.company_id_parent;
                    addInfo.company_linkname = empJob.company_linkname;
                    db.CommandTimeOut = 30;
                    db.BeginTran();
                    // 删除已有考评
                    db.Update<daoben_kpi_trainer>(new { is_del = true },
                                a => a.emp_id == addInfo.emp_id && a.month == addInfo.month && a.is_del == false);
                    db.Insert(addInfo);
                    db.InsertRange(detailList);
                    db.CommitTran();
                }
                catch (Exception ex)
                {
                    db.RollbackTran();
                    return "系统错误：" + ex.Message;
                }
                return "success";
            }
        }

        /// <summary>
        /// 获取kpi信息
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
                daoben_kpi_trainer mainInfo = db.Queryable<daoben_kpi_trainer>().SingleOrDefault(a => a.id == id);
                if (mainInfo == null)
                    return "信息错误：指定KPI信息不存在";
                daoben_hr_emp_job empInfo = db.Queryable<daoben_hr_emp_job>().Where(a => a.id == mainInfo.emp_id)
                        .Select("id, name, grade,position_id, position_name,position_type, company_id,emp_category,entry_date").SingleOrDefault();
                if (empInfo == null)
                    return "信息错误:指定的员工不存在";
                List<daoben_kpi_trainer_detail> detailList = db.Queryable<daoben_kpi_trainer_detail>()
                        .Where(a => a.main_id == id).ToList();
                List<daoben_kpi_trainer_approve> approvelist = db.Queryable<daoben_kpi_trainer_approve>()
                        .Where(a => a.main_id == id).ToList();

                List<daoben_salary_emp_trainer> empKPIList = db.Queryable<daoben_salary_emp_trainer>()
                           .JoinTable<daoben_salary_emp>((a, b) => a.main_id == b.id)
                           .Where<daoben_salary_emp>((a, b) => b.effect_status == 1 && b.emp_id == empInfo.id)
                           .Select("a.id, a.kpi_type,a.is_included,a.area_l1_id,a.area_l1_name")
                           .ToList();

                if (empInfo.position_type == ConstData.POSITION_TRAINER)
                {
                    daoben_salary_position_trainer positionKPI = db.Queryable<daoben_salary_position_trainer>()
                                .JoinTable<daoben_salary_position>((a, b) => a.salary_position_id == b.id)
                                .Where<daoben_salary_position>((a, b) => b.effect_status == 1 && b.position_id == empInfo.position_id)
                                .Select("a.*").SingleOrDefault();
                    object resultObj = new
                    {
                        empInfo = empInfo,
                        mainInfo = mainInfo,
                        detailList = detailList,
                        empKPIList = empKPIList,
                        positionKPI = positionKPI,
                        approvelistInfo = approvelist
                    };
                    return resultObj.ToJson();
                }
                else
                {
                    daoben_salary_position_trainermanager positionKPI = db.Queryable<daoben_salary_position_trainermanager>()
                                .JoinTable<daoben_salary_position>((a, b) => a.salary_position_id == b.id)
                                .Where<daoben_salary_position>((a, b) => b.effect_status == 1 && b.position_id == empInfo.position_id)
                                .Select("a.*").SingleOrDefault();
                    object resultObj = new
                    {
                        empInfo = empInfo,
                        mainInfo = mainInfo,
                        detailList = detailList,
                        empKPIList = empKPIList,
                        positionKPI = positionKPI,
                        approvelistInfo = approvelist
                    };
                    return resultObj.ToJson();
                }
            }
        }

        public List<daoben_kpi_trainer_detail> CalculateRatio(string emp_id, DateTime month)
        {

            using (var db = SugarDao.GetInstance())
            {
                return GetCalculateRatio(db, emp_id, month);

            }
        }

        public List<daoben_kpi_trainer_detail> GetCalculateRatio(SqlSugarClient db, string emp_id, DateTime month)
        {
            List<daoben_kpi_trainer_detail> detailList = new List<daoben_kpi_trainer_detail>();
            //计算高端机占比 实销
            daoben_hr_emp_job empInfo = db.Queryable<daoben_hr_emp_job>()
                .SingleOrDefault(t => t.id == emp_id);
            if (empInfo == null)
                return null;
            daoben_payroll_setting payrollMonth = new PayrollSettingApp().GetPayrollMonth(empInfo.company_id_parent, db, true, month);
            if (payrollMonth == null)
                return null;
            DateTime startDate = payrollMonth.start_date;
            DateTime endDate = payrollMonth.end_date.ToDate().AddDays(1);
            if (empInfo.position_type == ConstData.POSITION_TRAINER)
            {
                List<daoben_salary_emp_trainer> empKPIList = db.Queryable<daoben_salary_emp_trainer>()
                        .JoinTable<daoben_salary_emp>((a, b) => a.main_id == b.id)
                        .Where<daoben_salary_emp>((a, b) => b.effect_status == 1 && b.emp_id == empInfo.id && (a.kpi_type == 3 || a.kpi_type == 5))
                        .Select("a.id, a.kpi_type,a.is_included,a.area_l1_id,a.area_l1_name")
                        .ToList();
                daoben_salary_emp_trainer highRatio = empKPIList.Where(t => t.kpi_type == 5).FirstOrDefault();
                if (highRatio == null)
                    return null;
                List<daoben_product_sn> snList = db.Queryable<daoben_product_sn>()
                        .JoinTable<daoben_distributor_info>((a, b) => a.sale_distributor_id == b.id)
                        .Where<daoben_distributor_info>((a, b) => a.sale_type > 0 && a.sale_time >= startDate && a.sale_time < endDate
                        && b.area_l1_id == highRatio.area_l1_id && a.reporter_type == 1)
                        .Select("a.*").ToList();
                int normalCount = snList.Count();
                int highCount = snList.Where(t => t.high_level).ToList().Count();
                daoben_kpi_trainer_detail highRatiokpi = new daoben_kpi_trainer_detail
                {
                    kpi_score = normalCount == 0 ? 0 : Math.Round(((decimal)highCount / normalCount * 100), 2),
                    kpi_type = 5,
                };
                detailList.Add(highRatiokpi);

                daoben_salary_emp_trainer guideRatio = empKPIList.Where(t => t.kpi_type == 3).FirstOrDefault();
                if (guideRatio == null)
                    return null;
                int guideCount = 0;
                guideCount = db.Queryable<daoben_hr_emp_job>()  // TODO 应以结算日为准，查找历史导购 -- JIANG
                    .Where(a => a.position_type == ConstData.POSITION_GUIDE)
                    .Where(a => a.area_l1_id == guideRatio.area_l1_id).Count();
                daoben_kpi_trainer_detail guideRatiokpi = new daoben_kpi_trainer_detail
                {
                    kpi_score = guideCount == 0 ? 0 : Math.Round(((decimal)normalCount / guideCount), 2),
                    kpi_type = 3,
                    guide_amount = normalCount,
                    guide_count = guideCount,
                };
                detailList.Add(guideRatiokpi);
                return detailList;
            }
            else if (empInfo.position_type == ConstData.POSITION_TRAINERMANAGER)
            {
                List<daoben_product_sn> snList = db.Queryable<daoben_product_sn>()
                        .JoinTable<daoben_distributor_info>((a, b) => a.sale_distributor_id == b.id)
                        .Where<daoben_distributor_info>((a, b) => a.sale_type > 0 && a.sale_time >= startDate && a.sale_time < endDate
                        && b.company_id == empInfo.company_id && a.reporter_type == 1)
                        .Select("a.*").ToList();
                int normalCount = snList.Count();
                int highCount = snList.Where(t => t.high_level).ToList().Count();
                daoben_kpi_trainer_detail highRatiokpi = new daoben_kpi_trainer_detail
                {
                    kpi_score = highCount == 0 ? 0 : Math.Round(((decimal)highCount / normalCount * 100), 2),
                    kpi_type = 53,
                };
                detailList.Add(highRatiokpi);


                int guideCount = 0;
                guideCount = db.Queryable<daoben_hr_emp_job>().Where(a => a.company_id == empInfo.company_id)
                    .Where(a => a.position_type == ConstData.POSITION_GUIDE).Count();
                daoben_kpi_trainer_detail guideRatiokpi = new daoben_kpi_trainer_detail
                {
                    kpi_score = guideCount == 0 ? 0 : Math.Round(((decimal)normalCount / guideCount), 2),
                    kpi_type = 51,
                    guide_amount = normalCount,
                    guide_count = guideCount,
                };
                detailList.Add(guideRatiokpi);
                return detailList;
            }
            else return null;
        }
#if false
        // TODO
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
#endif

        internal string Delete(string id)
        {
            using (var db = SugarDao.GetInstance())
            {
                try
                {
                    daoben_kpi_trainer mainInfo = db.Queryable<daoben_kpi_trainer>().SingleOrDefault(a => a.id == id);
                    if (mainInfo == null || mainInfo.status != 0)
                        //审批过后，将不能撤回，但能重新提交评分
                        return "撤回失败：指定信息不存在或已被审批";
                    else
                    {
                        db.CommandTimeOut = 30;
                        db.BeginTran();
                        db.Delete<daoben_kpi_trainer_detail>(a => a.main_id == id);
                        db.Delete<daoben_kpi_trainer>(a => a.id == id);
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
    }
}
