using Base.Code;
using MySqlSugar;
using NPOI.HSSF.UserModel;
using ProjectShare.Database;
using ProjectShare.Models;
using ProjectShare.Process;
using ProjectWeb.Areas.SalaryCalculate.Application;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;


namespace ProjectWeb.Areas.FinancialAccounting.Application
{
    public class SalesKPIApp
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
            pagination.sidx = string.IsNullOrEmpty(pagination.sidx) ? "a.entry_date" : pagination.sidx;
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
                        .JoinTable<daoben_kpi_sales>((a, b) => a.id == b.emp_id && b.month == curPayrollMonth && b.is_del == false)
                        .Where(a => (a.position_type == ConstData.POSITION_SALES || a.position_type == ConstData.POSITION_SALESMANAGER)
                        && a.emp_category != "实习生");
                if (myCompanyInfo.category == "事业部")
                    qable.Where(a => a.company_id_parent == myCompanyInfo.id);
                else if (myCompanyInfo.category == "分公司")
                    qable.Where(a => a.company_id == myCompanyInfo.id);
                if (!showAll)
                    qable.Where<daoben_kpi_sales>((a, b) => b.id == null && a.entry_date < curPayrollMonth.AddDays(16));
                if (queryInfo != null)
                {
                    if (!string.IsNullOrEmpty(queryInfo.name))
                        qable.Where(a => a.name.Contains(queryInfo.name));
                    if (!string.IsNullOrEmpty(queryInfo.position_name))
                        qable.Where(a => a.position_name.Contains(queryInfo.position_name));
                    if (queryInfo.position_id > 0)
                        qable.Where(t => t.position_id == queryInfo.position_id);
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
                var qable = db.Queryable<daoben_kpi_sales>()
                            .JoinTable<daoben_hr_emp_job>((a, b) => a.emp_id == b.id);
                if (queryInfo != null)
                {
                    if (!string.IsNullOrEmpty(queryInfo.name))
                        qable.Where<daoben_hr_emp_job>((a, b) => b.name.Contains(queryInfo.name));
                    if (!string.IsNullOrEmpty(queryInfo.position_name))
                        qable.Where<daoben_hr_emp_job>((a, b) => b.position_name.Contains(queryInfo.position_name));
                    if (queryInfo.position_id > 0)
                        qable.Where<daoben_hr_emp_job>((a, b) => b.position_id == queryInfo.position_id);
                    if (queryInfo.company_id != 0)
                        qable.Where<daoben_hr_emp_job>((a, b) => b.company_id == queryInfo.company_id);
                    if (queryInfo.dept_id != 0)
                        qable.Where<daoben_hr_emp_job>((a, b) => b.dept_id == queryInfo.dept_id);
                }
                if (queryTime.startTime1 != null)
                    qable.Where<daoben_hr_emp_job>((a, b) => b.entry_date >= queryTime.startTime1);
                if (queryTime.startTime2 != null)
                {
                    DateTime tempTime = queryTime.startTime2.ToDate().AddDays(1);
                    qable.Where<daoben_hr_emp_job>((a, b) => b.entry_date < tempTime);
                }
                string queryStr = qable
                            .Where(a => a.is_del == false)
                            .Select("a.create_time,a.id,a.emp_id,IF(a.is_adjust=1, a.kpi_total_a, a.kpi_total) AS kpi_total,a.month, a.status,b.work_number,b.name as name,b.name_v2,b.position_name,b.grade,b.dept_name,b.company_linkname,b.emp_category,b.entry_date")
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
                string listStr = db.Queryable<daoben_kpi_sales>()
                            .JoinTable<daoben_hr_emp_job>((a, b) => a.emp_id == b.id)
                            .Select("a.create_time,a.id,a.emp_id,a.kpi_total,a.kpi_total_a,a.month,a.status,b.work_number,b.name_v2,b.position_name,b.grade,b.dept_name,b.company_linkname,b.emp_category,b.entry_date")
                            .OrderBy(pagination.sidx + " " + pagination.sord)
                            .ToJsonPage(pagination.page, pagination.rows, ref records);
                pagination.records = records;
                if (string.IsNullOrEmpty(listStr) || listStr == "[]")
                    return null;
                return listStr.ToJson();
            }
        }

        /// <summary>
        /// 从串码表获取具体销售信息
        /// </summary>
        /// <param name="pagination"></param>
        /// <param name="queryInfo"></param>
        /// <param name="origin">1-查看页面（~sn）；2-提交页面（~sn_temp）</param>
        /// <returns></returns>
        public object GetSaleList(Pagination pagination, string mainId, daoben_product_sn queryInfo, int origin)
        {
            OperatorModel LoginInfo = OperatorProvider.Provider.GetCurrent();
            if (LoginInfo == null)
                throw new Exception("用户登陆过期，请重新登录");
            if (string.IsNullOrEmpty(mainId))
                return null;
            int records = 0;
            if (pagination == null)
                pagination = new Pagination();
            pagination.page = pagination.page > 0 ? pagination.page : 1;
            pagination.rows = pagination.rows > 0 ? pagination.rows : 20;
            pagination.sidx = string.IsNullOrEmpty(pagination.sidx) ? "time" : pagination.sidx;
            pagination.sord = string.IsNullOrEmpty(pagination.sord) ? "desc" : pagination.sord;
            CompanyInfo myCompanyInfo = LoginInfo.companyInfo;
            string listStr = null;
            using (var db = SugarDao.GetInstance())
            {
                if (origin == 2)
                {
                    var qable = db.Queryable<daoben_kpi_sales_sn_temp>()
                            .Where(a => a.main_id == mainId);
                    if (queryInfo != null)
                    {
                        if (queryInfo.sale_type == 0) // 已出库
                            qable.Where(a => a.sale_type == 0);
                        else if (queryInfo.sale_type == 1) // 正常机
                            qable.Where(a => (a.sale_type == 1 || a.sale_type == 3));
                        else if (queryInfo.sale_type == 2) //买断机
                            qable.Where(a => a.sale_type == 2);

                        if (!string.IsNullOrEmpty(queryInfo.phone_sn))
                            qable.Where(a => a.phone_sn.Contains(queryInfo.phone_sn));
                        if (!string.IsNullOrEmpty(queryInfo.model))
                            qable.Where(a => a.model.Contains(queryInfo.model));
                    }
                    listStr = qable.OrderBy(pagination.sidx + " " + pagination.sord)
                                 .ToJsonPage(pagination.page, pagination.rows, ref records);
                }
                else
                {
                    var qable = db.Queryable<daoben_product_sn>()
                            .JoinTable<daoben_product_sn_outlay>((a, b)=>a.phone_sn == b.phone_sn)
                            .Where<daoben_product_sn_outlay>((a, b) => b.main_id == mainId);
                    if (queryInfo != null)
                    {
                        if (queryInfo.sale_type == 0) // 已出库
                            qable.Where(a => a.sale_type == 0);
                        else if (queryInfo.sale_type == 1) // 正常机
                            qable.Where(a => (a.sale_type == 1 || a.sale_type == 3));
                        else if (queryInfo.sale_type == 2) //买断机
                            qable.Where(a => a.sale_type == 2);

                        if (!string.IsNullOrEmpty(queryInfo.phone_sn))
                            qable.Where(a => a.phone_sn.Contains(queryInfo.phone_sn));
                        if (!string.IsNullOrEmpty(queryInfo.model))
                            qable.Where(a => a.model.Contains(queryInfo.model));
                    }
                    listStr = qable.OrderBy(pagination.sidx + " " + pagination.sord)
                                .Select("a.phone_sn, a.model, a.color,a.sale_type, b.outlay,b.outlay_type,a.price_wholesale, a.price_sale, b.time")
                                .ToJsonPage(pagination.page, pagination.rows, ref records);
                }
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
        public string GetSettingInfo(string id, DateTime? month)
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
                if (empInfo.emp_category == "实习生")//实习生没有月度考核
                    return "实习生无KPI考核";

                // 获取工资结算时间
                daoben_payroll_setting payrollMonth = null;
                if (month != null && ((DateTime)month).Day == 1)    // 有指定月份
                    payrollMonth = new PayrollSettingApp().GetPayrollMonth(myCompanyInfo.id, db, false, month);
                else
                    payrollMonth = new PayrollSettingApp().GetPayrollMonth(myCompanyInfo.id, db, true);
                if (payrollMonth == null)
                    return "系统出错：工资结算周期信息不存在";
                if (empInfo.entry_date > payrollMonth.month.AddDays(15))
                    return "该员工在本月15日之后入职，本月无工资";
                PayrollSalesApp salesApp = new PayrollSalesApp();
                #region 计算过程
                DateTime startTime = payrollMonth.start_date;
                DateTime endTime = payrollMonth.end_date.ToDate().AddDays(1);

                daoben_salary_emp empSalaryInfo = db.Queryable<daoben_salary_emp>()
                        .SingleOrDefault(t => t.emp_id == empInfo.id && t.effect_status == 1 && t.approve_status == 100 && t.category == 4);
                daoben_salary_position posSalaryInfo = db.Queryable<daoben_salary_position>()
                        .SingleOrDefault(t => t.position_id == empInfo.position_id && t.effect_status == 1 && t.approve_status == 100
                        && t.category == empInfo.position_type);// category 与 jobInfo.position_type 相对应（仅业务员/业务经理）);
                if (empSalaryInfo == null && posSalaryInfo == null)
                    return "尚未配置个人薪资方案或岗位薪资方案，KPI规则未知";

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
                        return "数据错误：员工个人薪资方案出错";
                    #endregion
                }
                else
                {
                    #region 无个人薪资方案，使用岗位薪资
                    daoben_salary_position_sales tempInfo = db.Queryable<daoben_salary_position_sales>()
                            .Where(a => a.salary_position_id == posSalaryInfo.id)
                            .SingleOrDefault();
                    if (tempInfo == null)
                        return "数据错误：岗位薪资方案出错";
                    if (salesSalaryInfo == null)
                        salesSalaryInfo = new daoben_salary_emp_sales();
                    salesSalaryInfo.id = tempInfo.id;
                    salesSalaryInfo.salary_position_id = tempInfo.salary_position_id;
                    salesSalaryInfo.activity_target = tempInfo.activity_target;
                    salesSalaryInfo.target_content = tempInfo.target_content;
                    salesSalaryInfo.target_mode = tempInfo.target_mode;
                    salesSalaryInfo.normal_rebate_mode = tempInfo.normal_rebate_mode;
                    salesSalaryInfo.buyout_rebate_mode = tempInfo.buyout_rebate_mode;
                    subList = db.Queryable<daoben_salary_position_sales_sub>()
                            .Where(a => a.main_id == salesSalaryInfo.id)
                            .Select<daoben_salary_emp_sales_sub>("*").ToList();
                    if (subList == null || subList.Count < 1)
                        return "数据错误：岗位薪资方案出错";
                    #endregion
                }
                #region 所有销售串码及统计
                daoben_kpi_sales salesKPI = new daoben_kpi_sales();
                salesKPI.emp_id = empInfo.id;
                salesKPI.position_type = empInfo.position_type;
                salesKPI.month = payrollMonth.month;
                salesKPI.company_id = empInfo.company_id;
                salesKPI.company_id_parent = empInfo.company_id_parent;
                salesKPI.company_linkname = empInfo.company_linkname;
                salesKPI.is_del = false;
                salesKPI.id = Common.GuId();
                string selStr = "'{0}' as main_id, phone_sn,model, color,sale_type,{1} as outlay_type, price_wholesale, price_sale, {2} as time";
                var qable = db.Queryable<daoben_product_sn>();
                if (empInfo.position_type == ConstData.POSITION_SALES)
                {
                    if (salesSalaryInfo.target_content == 1)
                    {
                        qable.Where(a => a.sales_id == empInfo.id && a.sale_type > 0
                                    && a.sale_time >= startTime && a.sale_time < endTime);
                        selStr = string.Format(selStr, salesKPI.id, "1", "sale_time");
                    }
                    else
                    {
                        qable.Where(a => a.out_sales_id == empInfo.id && a.sale_type > -1
                                    && a.outstorage_time >= startTime && a.outstorage_time < endTime);
                        selStr = string.Format(selStr, salesKPI.id, "2", "outstorage_time");

                    }
                }
                else if (empInfo.position_type == ConstData.POSITION_SALESMANAGER)
                {
                    if (salesSalaryInfo.target_content == 1)
                    {
                        qable.Where(a => a.sales_m_id == empInfo.id && a.sale_type > 0
                                    && a.sale_time >= startTime && a.sale_time < endTime);
                        selStr = string.Format(selStr, salesKPI.id, "1", "sale_time");
                    }
                    else
                    {
                        qable.Where(a => a.out_sales_m_id == empInfo.id && a.sale_type > -1
                                    && a.outstorage_time >= startTime && a.outstorage_time < endTime);
                        selStr = string.Format(selStr, salesKPI.id, "2", "outstorage_time");
                    }
                }
                List<daoben_kpi_sales_sn_temp> snList = qable.Select<daoben_kpi_sales_sn_temp>(selStr).ToList();
                if (snList == null || snList.Count < 1)
                {
                    object nullRetObj = new
                    {
                        empInfo = empInfo,
                        payrollMonth = payrollMonth,
                        salesKPI = salesKPI,
                        salesSalaryInfo = salesSalaryInfo,
                        subList = subList,
                    };
                    return nullRetObj.ToJson();
                }
                #endregion

                snList.ForEach(a =>
                {
                    if (a.sale_type == 2)
                    {
                        salesKPI.buyout_count++;
                        salesKPI.buyout_amount += a.price_sale;
                        salesKPI.buyout_amount_w += a.price_wholesale;
                    }
                    else // 含 sale_type = 0/1/3
                    {
                        salesKPI.normal_count++;
                        salesKPI.normal_amount_r += a.price_sale;
                        salesKPI.normal_amount_w += a.price_wholesale;
                    }
                });
                salesKPI.total_count = salesKPI.normal_count + salesKPI.buyout_count;
                salesKPI.total_amount_r = salesKPI.normal_amount_r + salesKPI.buyout_amount;
                salesKPI.total_amount_w = salesKPI.normal_amount_w + salesKPI.buyout_amount_w;
                List<daoben_kpi_sales_sn_temp> normalSnList = snList.Where(a => a.sale_type != 2).ToList();
                List<daoben_kpi_sales_sn_temp> buyoutSnList = snList.Where(a => a.sale_type == 2).ToList();
                #region KPI计算
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
                            buyoutSnList.ForEach(bs => bs.outlay = bs.price_sale * rebateModel.buyout_rebate / 100);
                            break;
                        case 4:
                            salesKPI.kpi_total += rebateModel.buyout_rebate;
                            decimal perCommission = rebateModel.buyout_rebate / salesKPI.buyout_count; // 均摊
                            buyoutSnList.ForEach(bs => bs.outlay = perCommission);
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
                            subNormalSnList.ForEach(ns => ns.outlay = a.sale_rebate);
                        }
                        else if (salesSalaryInfo.normal_rebate_mode == 2)
                        {
                            salesKPI.kpi_total += subNormalSnList.Sum(n => n.price_wholesale) * a.sale_rebate / 100;
                            subNormalSnList.ForEach(ns => ns.outlay = ns.price_wholesale * a.sale_rebate / 100);

                        }
                        else if (salesSalaryInfo.normal_rebate_mode == 3)
                        {
                            salesKPI.kpi_total += subNormalSnList.Sum(n => n.price_sale) * a.sale_rebate / 100;
                            subNormalSnList.ForEach(ns => ns.outlay = ns.price_sale * a.sale_rebate / 100);
                        }

                        if (salesSalaryInfo.buyout_rebate_mode == 1)
                        {
                            salesKPI.kpi_total += subBuyoutSnList.Count * a.buyout_rebate;
                            subBuyoutSnList.ForEach(bs => bs.outlay = a.buyout_rebate);
                        }
                        else if (salesSalaryInfo.buyout_rebate_mode == 2)
                        {
                            salesKPI.kpi_total += subBuyoutSnList.Sum(n => n.price_wholesale) * a.buyout_rebate / 100;
                            subBuyoutSnList.ForEach(bs => bs.outlay = bs.price_wholesale * a.buyout_rebate / 100);
                        }
                        else if (salesSalaryInfo.buyout_rebate_mode == 3)
                        {
                            salesKPI.kpi_total += subBuyoutSnList.Sum(n => n.price_sale) * a.buyout_rebate / 100;
                            subBuyoutSnList.ForEach(bs => bs.outlay = bs.price_sale * a.buyout_rebate / 100);
                        }
                    });
                    #endregion
                }
                salesKPI.kpi_total_a = salesKPI.kpi_total;
                db.SqlBulkCopy(snList); // 临时存放，KPI提交后移动到daoben_product_sn_outlay，本表定时清空，
                #endregion
                #endregion
                object retObj = new
                {
                    empInfo = empInfo,
                    payrollMonth = payrollMonth,
                    salesKPI = salesKPI,
                    salesSalaryInfo = salesSalaryInfo,
                    subList = subList,
                };
                return retObj.ToJson();
            }
        }

        public string Add(daoben_kpi_sales addInfo)
        {
            var LoginInfo = OperatorProvider.Provider.GetCurrent();
            if (LoginInfo == null)
                return "登录超时，请重新登录";
            if (addInfo == null)
                return "信息错误";
            if(addInfo.month == null || addInfo.month > DateTime.Now)
                return "信息错误：月份不正确!";
            if (string.IsNullOrEmpty(addInfo.id) || addInfo.id.Length != 36)
                return "信息错误：ID不正确!";

            CompanyInfo myCompanyInfo = LoginInfo.companyInfo;
            PositionInfo myPositionInfo = LoginInfo.positionInfo;
            string sqlStr = "insert into daoben_product_sn_outlay (category,main_id,phone_sn,outlay,outlay_type,time) "
                    + "select {0} as category, main_id, phone_sn,outlay,outlay_type,time from daoben_kpi_sales_sn_temp where main_id='{1}'";
            using (var db = SugarDao.GetInstance())
            {
                try
                {
                    daoben_hr_emp_job empJob = db.Queryable<daoben_hr_emp_job>()
                                .Where(a => a.id == addInfo.emp_id).SingleOrDefault();
                    if (empJob == null)
                        return "信息错误：指定的员工信息不存在";
                    if (empJob.position_type == ConstData.POSITION_SALES)
                        sqlStr = string.Format(sqlStr, "1", addInfo.id);
                    else
                        sqlStr = string.Format(sqlStr, "11", addInfo.id);
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
                    List<string> origIdList = db.Queryable<daoben_kpi_sales>()
                            .Where(a => a.emp_id == addInfo.emp_id && a.month == addInfo.month)
                            .Select<string>("id").ToList();
                    if (origIdList != null && origIdList.Count > 0)
                    {   // 存在同人同月份KPI，不管审批与否
                        db.Delete<daoben_product_sn_outlay>(a => origIdList.Contains(a.main_id));
                        db.Delete<daoben_kpi_sales>(a => origIdList.Contains(a.id));
                    }
                    db.Insert(addInfo);
                    db.SqlQuery<int>(sqlStr);
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
        public string GetInfo(string kpiId)
        {
            var LoginInfo = OperatorProvider.Provider.GetCurrent();
            if (LoginInfo == null)
                return "登录超时，请重新登录";
            if (string.IsNullOrEmpty(kpiId))
                return "信息错误";
            using (var db = SugarDao.GetInstance())
            {
                daoben_kpi_sales salesKPI = db.Queryable<daoben_kpi_sales>().SingleOrDefault(a => a.id == kpiId);

                if (salesKPI == null || string.IsNullOrEmpty(salesKPI.emp_id))
                    return "信息错误：指定KPI信息不存在";
                daoben_hr_emp_job empInfo = db.Queryable<daoben_hr_emp_job>().Where(a => a.id == salesKPI.emp_id).SingleOrDefault();
                if (empInfo == null)
                    return "信息错误:指定的员工不存在";
                List<daoben_kpi_sale_approve> approvelist = null;
                if (!string.IsNullOrEmpty(salesKPI.id))
                    approvelist = db.Queryable<daoben_kpi_sale_approve>()
                            .Where(a => a.main_id == salesKPI.id).ToList();


                // 获取工资结算时间
                daoben_payroll_setting payrollMonth = new PayrollSettingApp().GetPayrollMonth(salesKPI.company_id_parent, db, true);
                if (payrollMonth == null)
                    return "系统出错：工资结算周期信息不存在";

                PayrollSalesApp salesApp = new PayrollSalesApp();
                #region
                daoben_salary_emp empSalaryInfo = db.Queryable<daoben_salary_emp>()
                        .SingleOrDefault(t => t.emp_id == empInfo.id && t.effect_status == 1 && t.approve_status == 100 && t.category == 4);
                daoben_salary_position posSalaryInfo = db.Queryable<daoben_salary_position>()
                        .SingleOrDefault(t => t.position_id == empInfo.position_id && t.effect_status == 1 && t.approve_status == 100
                        && t.category == empInfo.position_type);// category 与 jobInfo.position_type 相对应（仅业务员/业务经理）);
                if (empSalaryInfo == null && posSalaryInfo == null)
                    return "尚未配置个人薪资方案或岗位薪资方案，KPI规则未知";
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
                        return "数据错误：员工个人薪资方案出错";
                    #endregion
                }
                else
                {
                    #region 无个人薪资方案，使用岗位薪资
                    daoben_salary_position_sales tempInfo = db.Queryable<daoben_salary_position_sales>()
                            .Where(a => a.salary_position_id == posSalaryInfo.id)
                            .SingleOrDefault();
                    if (tempInfo == null)
                        return "数据错误：岗位薪资方案出错";
                    if (salesSalaryInfo == null)
                        salesSalaryInfo = new daoben_salary_emp_sales();
                    salesSalaryInfo.id = tempInfo.id;
                    salesSalaryInfo.salary_position_id = tempInfo.salary_position_id;
                    salesSalaryInfo.activity_target = tempInfo.activity_target;
                    salesSalaryInfo.target_content = tempInfo.target_content;
                    salesSalaryInfo.target_mode = tempInfo.target_mode;
                    salesSalaryInfo.normal_rebate_mode = tempInfo.normal_rebate_mode;
                    salesSalaryInfo.buyout_rebate_mode = tempInfo.buyout_rebate_mode;
                    subList = db.Queryable<daoben_salary_position_sales_sub>()
                            .Where(a => a.main_id == salesSalaryInfo.id)
                            .Select<daoben_salary_emp_sales_sub>("*").ToList();
                    if (subList == null || subList.Count < 1)
                        return "数据错误：岗位薪资方案出错";
                    #endregion
                }

                #endregion
                object resultObj = new
                {
                    empInfo = empInfo,
                    salesKPI = salesKPI,
                    approvelistInfo = approvelist,
                    salesSalaryInfo = salesSalaryInfo,
                    sublist = subList
                };
                return resultObj.ToJson();
            }
        }

        internal string Delete(string id)
        {
            using (var db = SugarDao.GetInstance())
            {
                try
                {
                    daoben_kpi_sales mainInfo = db.Queryable<daoben_kpi_sales>().SingleOrDefault(a => a.id == id);
                    if (mainInfo == null || mainInfo.status != 0)
                        return "撤回失败：指定信息不存在或已被审批";
                    else
                    {
                        db.CommandTimeOut = 30;
                        db.BeginTran();
                        db.Delete<daoben_product_sn_outlay>(a => a.main_id == id);
                        db.Delete<daoben_kpi_sales>(a => a.id == id);
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
        /// <summary>
        /// 
        /// </summary>
        /// <param name="pagination"></param>
        /// <param name="queryInfo"></param>
        /// <param name="origin">来源：1-daoben_product_sn_outlay；2-daoben_kpi_sales_sn_temp</param>
        /// <returns></returns>
        public MemoryStream ExportExcel(Pagination pagination, daoben_kpi_sales queryInfo, int origin)
        {
            OperatorModel LoginInfo = OperatorProvider.Provider.GetCurrent();
            if (LoginInfo == null)
                throw new Exception("用户登陆过期，请重新登录");

            if (pagination == null)
                pagination = new Pagination();
            pagination.sidx = string.IsNullOrEmpty(pagination.sidx) ? "month" : pagination.sidx;
            pagination.sord = string.IsNullOrEmpty(pagination.sord) ? "desc" : pagination.sord;
            DataTable listDt = null;
            string selStr = string.Format("'{0}' as name, '{1}' as month,", queryInfo.emp_name, queryInfo.month.ToDate().ToString("yyyy-MM"));
            using (var db = SugarDao.GetInstance())
            {
                if(origin == 2)
                {   // daoben_kpi_sales_sn_temp
                    selStr += "phone_sn, model, color,(CASE WHEN sale_type=0 THEN '已出库' WHEN sale_type=2 THEN '买断' ELSE '正常销售' END) as sale_type,"
                                + "outlay, IF(outlay_type=1,'实销','下货'), price_wholesale, price_sale, DATE_FORMAT(time,'%Y-%m-%d')";
                    listDt = db.Queryable<daoben_kpi_sales_sn_temp>().Where(a => a.main_id == queryInfo.id)
                                .OrderBy(pagination.sidx + " " + pagination.sord)
                                .Select(selStr).ToDataTable();
                }
                else
                {   // daoben_product_sn_outlay
                    selStr += "a.phone_sn, b.model, b.color,(CASE WHEN b.sale_type=0 THEN '已出库' WHEN b.sale_type=2 THEN '买断' ELSE '正常销售' END) as sale_type,"
                                + "a.outlay,IF(a.outlay_type=1,'实销','下货'), b.price_wholesale, b.price_sale, DATE_FORMAT(a.time,'%Y-%m-%d')";
                    listDt = db.Queryable<daoben_product_sn_outlay>()
                                .JoinTable<daoben_product_sn>((a,b)=>a.phone_sn == b.phone_sn)
                                .Where(a => a.main_id == queryInfo.id)
                                .OrderBy(pagination.sidx + " " + pagination.sord)
                                .Select(selStr).ToDataTable();
                }
                int rowsCount = listDt.Rows.Count;
                string[] headerArr = new string[]
                {"姓名","月份","串码","型号","颜色","销售状态","提成","类型","批发价","实销价","时间"};
                int[] colWidthArr = new int[] { 15, 15, 18, 20, 15, 15, 15,15, 15, 15, 15 };

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
    }
}
