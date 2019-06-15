using Base.Code;
using MySqlSugar;
using ProjectShare.Database;
using ProjectShare.Models;
using ProjectShare.Process;
using ProjectWeb.Application;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ProjectWeb.Areas.SalaryCalculate.Application
{
    public class PayrollSettingApp
    {
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
            int companyId = 0;
            if (myCompanyInfo.category == "事业部")
                companyId = myCompanyInfo.id;
            else if (myCompanyInfo.category == "分公司")
                companyId = myCompanyInfo.parentId;
            using (var db = SugarDao.GetInstance())
            {
                var qable = db.Queryable<daoben_payroll_setting>();
                if (companyId == 0)
                {
                    qable.Where(a => a.start_date < DateTime.Now)
                            .Select("id, month, start_date, end_date, creator_name, note, company_name");
                }
                else
                {
                    qable.JoinTable<daoben_payroll_setting>((a, b) => b.month == a.month && b.company_id == companyId && b.approve_status == 100)
                            .Where(a => a.company_id == 0 && a.start_date < DateTime.Now)
                            .Select("ifnull(b.id,a.id) as id, a.month, ifnull(b.start_date,a.start_date) as start_date, ifnull(b.end_date,a.end_date) as end_date,ifnull(b.creator_name,a.creator_name) as creator_name, b.note, b.company_name as company_name");
                }
                string listStr = qable.OrderBy(pagination.sidx + " " + pagination.sord)
                            .ToJsonPage(pagination.page, pagination.rows, ref records);
                pagination.records = records;
                if (string.IsNullOrEmpty(listStr) || listStr == "[]")
                    return null;
                return listStr.ToJson();
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
            if (myCompanyInfo.category != "事业部")
                return "权限错误：需由事业部人员操作!";
            using (var db = SugarDao.GetInstance())
            {
                string listStr = db.Queryable<daoben_payroll_setting>()
                            .Where(a => a.approve_status == 0 && a.company_id == myCompanyInfo.id)
                            .OrderBy(pagination.sidx + " " + pagination.sord)
                            .ToJsonPage(pagination.page, pagination.rows, ref records);
                pagination.records = records;
                if (string.IsNullOrEmpty(listStr) || listStr == "[]")
                    return null;
                return listStr.ToJson();
            }
        }

        /// <summary>
        /// 获取结算月份信息
        /// </summary>
        /// <param name="id">0-当前日期所属月份</param>
        /// <returns></returns>
        public daoben_payroll_setting GetInfo(int id)
        {
            DateTime now = DateTime.Now.Date;

            daoben_payroll_setting getInfo = null;
            using (var db = SugarDao.GetInstance())
            {
                if (id < 1)
                    getInfo = db.Queryable<daoben_payroll_setting>().Where(a => a.end_date >= now && a.start_date <= now).SingleOrDefault();
                else
                    getInfo = db.Queryable<daoben_payroll_setting>().Where(a => a.id == id).SingleOrDefault();

                if (getInfo != null)
                    return getInfo;
                else
                {
                    DateTime curMonth = now.AddDays(1 - now.Day);
                    daoben_payroll_setting obj = new daoben_payroll_setting { month = curMonth, end_date = curMonth.AddMonths(1).AddDays(-1) };
                    return obj;
                }
            }
        }

        /// <summary>
        /// 获取结算月份信息 
        /// add by yajun
        /// </summary>
        /// <param name="id">0-当前日期所属月份</param>
        /// <returns></returns>
        public object GetInfoByID(int id)
        {
            using (var db = SugarDao.GetInstance())
            {
                daoben_payroll_setting getInfo = db.Queryable<daoben_payroll_setting>().Where(a => a.id == id).SingleOrDefault();
                return getInfo;
            }
        }
        /// <summary>
        /// 系统函数，添加200个月内的月份信息
        /// </summary>
        /// <returns></returns>
        public void Add()
        {
            DateTime now = DateTime.Now;
            List<daoben_payroll_setting> list = new List<daoben_payroll_setting>();
            DateTime month = now.Date.AddDays(1 - now.Day);
            for (int i = 0; i < 200; i++)
            {
                daoben_payroll_setting addInfo = new daoben_payroll_setting();
                addInfo.month = month.AddMonths(i);
                addInfo.start_date = addInfo.month;
                addInfo.end_date = addInfo.month.AddMonths(1).AddDays(-1);
                addInfo.approve_status = 100;
                addInfo.creator_name = "系统默认";
                addInfo.create_time = now;
                list.Add(addInfo);
            }
            using (var db = SugarDao.GetInstance())
            {
                db.DisableInsertColumns = new string[] { "id" };
                db.SqlBulkCopy(list);
            }
        }

        public string Edit(daoben_payroll_setting editInfo)
        {
            OperatorModel LoginInfo = OperatorProvider.Provider.GetCurrent();
            if (LoginInfo == null)
                throw new Exception("用户登陆过期，请重新登录");
            CompanyInfo myCompanyInfo = LoginInfo.companyInfo;
            PositionInfo myPositionInfo = LoginInfo.positionInfo;

            if (myCompanyInfo.category != "事业部")
                return "权限错误，需由事业部人员操作!";
            DateTime now = DateTime.Now;
            if (editInfo == null || editInfo.end_date < now)
                return "信息错误，操作失败!";

            try
            {
                using (var db = SugarDao.GetInstance())
                {
                    daoben_payroll_setting editOrig = GetPayrollMonth(myCompanyInfo.id, db);
                    if (editOrig == null)
                        return "系统错误，原始结算月份信息不存在!";
                    editInfo.month = editOrig.month;
                    editInfo.start_date = editOrig.start_date;
                    editInfo.note = editInfo.note;
                    editInfo.creator_job_history_id = LoginInfo.jobHistoryId;
                    editInfo.creator_position_name = myPositionInfo.name;
                    editInfo.creator_name = LoginInfo.empName;
                    editInfo.create_time = now;
                    editInfo.approve_status = 0;
                    editInfo.company_id = myCompanyInfo.id;
                    editInfo.company_name = myCompanyInfo.name;
                    db.DisableInsertColumns = new string[] { "id" };
                    db.Insert(editInfo);
                    return "success";
                }
            }
            catch (Exception ex)
            {
                return "系统出错：" + ex.Message;
            }
        }

        public string Approve(daoben_payroll_setting approveInfo)
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
                    daoben_payroll_setting mainInfo = db.Queryable<daoben_payroll_setting>().SingleOrDefault(a => a.id == approveInfo.id);
                    if (mainInfo == null)
                        return "信息错误：信息不存在";
                    if (mainInfo.approve_status < 0 || mainInfo.approve_status == 100)
                        return "信息错误：已被审批";
                    if (LoginInfo.roleId == ConstData.ROLE_ID_FINANCIAL || LoginInfo.roleId == ConstData.ROLE_ID_FINANCIALMANAGER)
                    {
                        if (approveInfo.approve_status > 0)
                            mainInfo.approve_status = 100;
                        else
                            mainInfo.approve_status = -100;
                    }
                    else
                        mainInfo.approve_status = -1;
                    mainInfo.approve_job_history_id = LoginInfo.jobHistoryId;
                    mainInfo.approve_name = LoginInfo.empName;
                    mainInfo.approve_position_name = myPositionInfo.name;
                    mainInfo.approve_time = DateTime.Now;
                    mainInfo.approve_note = approveInfo.approve_note;

                    if (mainInfo.approve_status == 100)
                    {
                        DateTime nextMonth = mainInfo.month.AddMonths(1);
                        db.Update<daoben_payroll_setting>(new { approve_status = -101 }, a => a.month == mainInfo.month && a.company_id == mainInfo.company_id);
                        #region 下一个月的起始时间
                        daoben_payroll_setting nextSetting = db.Queryable<daoben_payroll_setting>()
                                    .Where(a => a.company_id == mainInfo.company_id && a.month == mainInfo.month.AddMonths(1) && a.approve_status == 100)
                                    .SingleOrDefault();
                        if (nextSetting == null)
                        {
                            nextSetting = new daoben_payroll_setting
                            {
                                month = mainInfo.month.AddMonths(1),
                                end_date = mainInfo.month.AddMonths(2).AddDays(-1),
                                company_id = myCompanyInfo.id,
                                company_name = myCompanyInfo.name,
                                approve_status = 100
                            };
                        }
                        nextSetting.start_date = ((DateTime)mainInfo.end_date).AddDays(1);
                        nextSetting.creator_job_history_id = mainInfo.creator_job_history_id;
                        nextSetting.creator_position_name = mainInfo.approve_position_name;
                        nextSetting.creator_name = mainInfo.creator_name;
                        nextSetting.create_time = mainInfo.approve_time;
                        nextSetting.approve_job_history_id = LoginInfo.jobHistoryId;
                        nextSetting.approve_name = LoginInfo.empName;
                        nextSetting.approve_position_name = myPositionInfo.name;
                        nextSetting.approve_time = mainInfo.approve_time;
                        if (nextSetting.id != 0)
                            db.Update(nextSetting);
                        else
                        {
                            db.DisableInsertColumns = new string[] { "id" };
                            db.Insert(nextSetting);
                        }
                        #endregion
                    }
                    db.Update(mainInfo);
                    approveInfo.end_date = mainInfo.end_date;
                    return "success";
                }
                catch (Exception ex)
                {
                    return ex.Message;
                }
            }
        }

        public daoben_payroll_setting GetPayrollMonth(int companyId = 0, SqlSugarClient db = null, bool lastMonth = false, DateTime? month = null)
        {
            string whereStr = "";
            if (companyId == 0)
            {
                var LoginInfo = OperatorProvider.Provider.GetCurrent();
                if (LoginInfo == null)
                    throw new Exception("登录超时，请重新登录");
                CompanyInfo myCompanyInfo = LoginInfo.companyInfo;
                if (myCompanyInfo.category == "事业部")
                    whereStr = string.Format(" (company_id={0} OR company_id=0) ", myCompanyInfo.id);
                else if (myCompanyInfo.category == "分公司")
                    whereStr = string.Format(" (company_id={0} OR company_id=0) ", myCompanyInfo.parentId);
                else
                    whereStr = " company_id=0 ";
            }
            else
                whereStr = string.Format(" (company_id={0} OR company_id=0) ", companyId);
            DateTime now = DateTime.Now;
            bool toDispose = false;
            if (db == null)
            {
                db = SugarDao.GetInstance();
                toDispose = true;
            }
            daoben_payroll_setting monthInfo = null;
            if (month == null)
            {
                // 获取工资结算时间
                monthInfo = db.Queryable<daoben_payroll_setting>()
                        .Where(a => a.end_date >= now.Date && a.start_date <= now && a.approve_status == 100).Where(whereStr)
                        .OrderBy(a => a.id, OrderByType.Desc)
                        .Select("month, start_date, end_date, company_id, approve_status, note").FirstOrDefault();
                if (lastMonth)
                {
                    monthInfo = db.Queryable<daoben_payroll_setting>()
                        .Where(a => a.month == monthInfo.month.AddMonths(-1) && a.approve_status == 100).Where(whereStr)
                        .OrderBy(a => a.id, OrderByType.Desc)
                        .Select("month, start_date, end_date, company_id, approve_status, note").FirstOrDefault();
                }
            }
            else
            {
                monthInfo = db.Queryable<daoben_payroll_setting>()
                        .Where(a => a.month == month && a.approve_status == 100).Where(whereStr)
                        .OrderBy(a => a.id, OrderByType.Desc)
                        .Select("month, start_date, end_date, company_id, approve_status, note").FirstOrDefault();
            }
            if (toDispose)
                db.Dispose();
            return monthInfo;
        }

    }
}
