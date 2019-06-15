using Base.Code;
using MySqlSugar;
using NPOI.HSSF.UserModel;
using ProjectShare.Database;
using ProjectShare.Models;
using ProjectShare.Process;
using System;
using System.IO;
using System.Linq;

namespace ProjectWeb.Areas.ActivityManage.Application
{

    public class CalculateApp
    {

        #region 达量活动
        public object AttainingMainList(Pagination pagination, QueryTime queryTime, int company_id, string emp_name, int? emp_category = 0)
        {
            var LoginInfo = OperatorProvider.Provider.GetCurrent();
            if (LoginInfo == null)
                return null;
            if (pagination == null)
                pagination = new Pagination();
            pagination.page = pagination.page > 0 ? pagination.page : 1;
            pagination.rows = pagination.rows > 0 ? pagination.rows : 20;
            pagination.sidx = string.IsNullOrEmpty(pagination.sidx) ? "a.emp_id" : pagination.sidx;
            pagination.sord = string.IsNullOrEmpty(pagination.sord) ? "desc" : pagination.sord;
            CompanyInfo myCompanyInfo = LoginInfo.companyInfo;
            PositionInfo myPositionInfo = LoginInfo.positionInfo;

            using (var db = SugarDao.GetInstance())
            {
                var qable = db.Queryable<daoben_activity_attaining_emp>()
                        .JoinTable<daoben_hr_emp_job>((a, b) => b.id == a.emp_id)
                        .JoinTable<daoben_activity_attaining>((a, c) => a.main_id == c.id)
                        .Where<daoben_activity_attaining>((a, c) => c.activity_status == 2);       // 只统计已结束的活动

                if (myCompanyInfo.category == "事业部")
                    qable.Where<daoben_activity_attaining>((a, c) => c.company_id_parent == myCompanyInfo.id);
                else if (myCompanyInfo.category == "分公司")
                    qable.Where<daoben_activity_attaining>((a, c) => c.company_id == myCompanyInfo.id);
                if (company_id > 0)
                    qable.Where<daoben_activity_attaining>((a, c) => c.company_id == company_id);

                if (queryTime.startTime1 != null)
                    qable.Where<daoben_activity_attaining>((a, c) => c.end_date >= queryTime.startTime1);
                if (queryTime.startTime2 != null)
                {
                    queryTime.startTime2 = queryTime.startTime2.ToDate().AddDays(1);
                    qable.Where<daoben_activity_attaining>((a, c) => c.end_date < queryTime.startTime2);
                }
                if (!string.IsNullOrEmpty(emp_name))
                    qable.Where(a => a.emp_name.Contains(emp_name));


                if (emp_category == 3)
                    qable.Where<daoben_hr_emp_job>((a, b) => b.position_type == ConstData.POSITION_GUIDE);
                else if (emp_category == 20)
                    qable.Where<daoben_hr_emp_job>((a, b) => b.position_type == ConstData.POSITION_SALES);
                else if (emp_category == 21)
                    qable.Where<daoben_hr_emp_job>((a, b) => b.position_type == ConstData.POSITION_SALESMANAGER);

                string listStr = qable
                        .OrderBy(pagination.sidx + " " + pagination.sord)
                        .GroupBy("a.emp_id")
                        .Select("SQL_CALC_FOUND_ROWS COUNT(a.id) AS count,SUM(total_count) AS total_count,SUM(total_amount) AS total_amount,SUM(a.total_reward) AS total_reward,a.emp_name,a.emp_id as id,c.emp_category,a.area_l2_name,b.company_linkname")
                        .ToJsonPage(pagination.page, pagination.rows);
                pagination.records = db.SqlQuery<int>("SELECT FOUND_ROWS() as count").FirstOrDefault();
                if (string.IsNullOrEmpty(listStr) || listStr == "[]")
                    return null;
                return listStr.ToJson();
            }
        }
        public object AttainingList(Pagination pagination, QueryTime queryTime, string emp_id)
        {
            var LoginInfo = OperatorProvider.Provider.GetCurrent();
            if (LoginInfo == null)
                return null;
            int records = 0;
            if (pagination == null)
                pagination = new Pagination();
            pagination.page = pagination.page > 0 ? pagination.page : 1;
            pagination.rows = pagination.rows > 0 ? pagination.rows : 20;
            pagination.sidx = string.IsNullOrEmpty(pagination.sidx) ? "c.name" : pagination.sidx;
            pagination.sord = string.IsNullOrEmpty(pagination.sord) ? "desc" : pagination.sord;
            CompanyInfo myCompanyInfo = LoginInfo.companyInfo;
            PositionInfo myPositionInfo = LoginInfo.positionInfo;

            using (var db = SugarDao.GetInstance())
            {
                var qable = db.Queryable<daoben_activity_attaining_emp>()
                    .JoinTable<daoben_hr_emp_job>((a, b) => a.emp_id == b.id)
                    .JoinTable<daoben_activity_attaining>((a, c) => a.main_id == c.id)
                    .Where<daoben_activity_attaining>((a, c) => c.activity_status == 2 && a.emp_id == emp_id);       // 只统计已结束的活动

                if (queryTime.startTime1 != null)
                    qable.Where<daoben_activity_attaining>((a, c) => c.end_date >= queryTime.startTime1);
                if (queryTime.startTime2 != null)
                {
                    queryTime.startTime2 = queryTime.startTime2.ToDate().AddDays(1);
                    qable.Where<daoben_activity_attaining>((a, c) => c.end_date < queryTime.startTime2);
                }
                string listStr = qable
                        .OrderBy(pagination.sidx + " " + pagination.sord)
                        .Select("c.id,c.name,c.start_date,c.end_date,a.total_amount,a.total_count,a.total_ratio,a.total_reward,c.target_mode,c.activity_target,c.target_content")
                        .ToJsonPage(pagination.page, pagination.rows, ref records);
                pagination.records = records;
                if (string.IsNullOrEmpty(listStr) || listStr == "[]")
                    return null;
                return listStr.ToJson();
            }
        }

        public MemoryStream ExportExcelAttaining(Pagination pagination, QueryTime queryTime, int? company_id, string emp_name, int? emp_category)
        {
            var LoginInfo = OperatorProvider.Provider.GetCurrent();
            if (LoginInfo == null)
                return null;
            if (pagination == null)
                pagination = new Pagination();
            pagination.sidx = string.IsNullOrEmpty(pagination.sidx) ? "c.name" : pagination.sidx;
            pagination.sord = string.IsNullOrEmpty(pagination.sord) ? "desc" : pagination.sord;
            CompanyInfo myCompanyInfo = LoginInfo.companyInfo;
            PositionInfo myPositionInfo = LoginInfo.positionInfo;

            using (var db = SugarDao.GetInstance())
            {
                var qable = db.Queryable<daoben_activity_attaining_emp>()
                    .JoinTable<daoben_hr_emp_job>((a, b) => a.emp_id == b.id)
                    .JoinTable<daoben_activity_attaining>((a, c) => a.main_id == c.id)
                    .Where<daoben_activity_attaining>((a, c) => c.activity_status == 2);       // 只统计已结束的活动

                if (myCompanyInfo.category == "事业部")
                    qable.Where<daoben_activity_attaining>((a, c) => c.company_id_parent == myCompanyInfo.id);
                else if (myCompanyInfo.category == "分公司")
                    qable.Where<daoben_activity_attaining>((a, c) => c.company_id == myCompanyInfo.id);
                if (company_id > 0)
                    qable.Where<daoben_activity_attaining>((a, c) => c.company_id == company_id);

                if (queryTime.startTime1 != null)
                    qable.Where<daoben_activity_attaining>((a, c) => c.end_date >= queryTime.startTime1);
                if (queryTime.startTime2 != null)
                {
                    queryTime.startTime2 = queryTime.startTime2.ToDate().AddDays(1);
                    qable.Where<daoben_activity_attaining>((a, c) => c.end_date < queryTime.startTime2);
                }
                if (!string.IsNullOrEmpty(emp_name))
                    qable.Where(a => a.emp_name.Contains(emp_name));

                if (emp_category == 3)
                    qable.Where<daoben_hr_emp_job>((a, b) => b.position_type == ConstData.POSITION_GUIDE);
                else if (emp_category == 20)
                    qable.Where<daoben_hr_emp_job>((a, b) => b.position_type == ConstData.POSITION_SALES);
                else if (emp_category == 21)
                    qable.Where<daoben_hr_emp_job>((a, b) => b.position_type == ConstData.POSITION_SALESMANAGER);


                string selStr = "b.company_linkname,b.name as name,b.position_name,c.name as activity_name,a.total_amount,DATE_FORMAT(c.start_date,'%Y-%m-%d'),DATE_FORMAT(c.end_date,'%Y-%m-%d'),a.total_reward";
                var listDt = qable.OrderBy(pagination.sidx + " " + pagination.sord)
                        .OrderBy("a.emp_id,c.end_date")
                        .Select(selStr).ToDataTable();
                int rowsCount = listDt.Rows.Count;

                string[] headerArr = new string[] { "所属机构", "员工名称", "职位", "活动名称", "销量总金额", "活动开始时间", "活动结束时间", "奖励金额" };
                int[] colWidthArr = new int[] { 18, 20, 15, 28, 18, 20, 20, 18 };

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

        public object GetTotalAttaining(QueryTime queryTime, int? company_id, string emp_name, int? emp_category = 0)
        {
            OperatorModel LoginInfo = OperatorProvider.Provider.GetCurrent();
            if (LoginInfo == null)
                throw new Exception("用户登陆过期，请重新登录");
            CompanyInfo myCompanyInfo = LoginInfo.companyInfo;

            DateTime now = DateTime.Now;
            DateTime firstDayOfThisYear = new DateTime(now.Year, 1, 1);
            using (var db = SugarDao.GetInstance())
            {
                var qable = db.Queryable<daoben_activity_attaining_emp>()
                        .JoinTable<daoben_hr_emp_job>((a, b) => b.id == a.emp_id)
                        .JoinTable<daoben_activity_attaining>((a, c) => a.main_id == c.id)
                        .Where<daoben_activity_attaining>((a, c) => c.activity_status == 2);       // 只统计已结束的活动

                if (myCompanyInfo.category == "事业部")
                    qable.Where<daoben_activity_attaining>((a, c) => c.company_id_parent == myCompanyInfo.id);
                else if (myCompanyInfo.category == "分公司")
                    qable.Where<daoben_activity_attaining>((a, c) => c.company_id == myCompanyInfo.id);
                if (company_id > 0)
                    qable.Where<daoben_activity_attaining>((a, c) => c.company_id == company_id);

                if (queryTime.startTime1 != null)
                    qable.Where<daoben_activity_attaining>((a, c) => c.end_date >= queryTime.startTime1);
                if (queryTime.startTime2 != null)
                {
                    queryTime.startTime2 = queryTime.startTime2.ToDate().AddDays(1);
                    qable.Where<daoben_activity_attaining>((a, c) => c.end_date < queryTime.startTime2);
                }
                if (!string.IsNullOrEmpty(emp_name))
                    qable.Where(a => a.emp_name.Contains(emp_name));

                if (emp_category == 3)
                    qable.Where<daoben_hr_emp_job>((a, b) => b.position_type == ConstData.POSITION_GUIDE);
                else if (emp_category == 20)
                    qable.Where<daoben_hr_emp_job>((a, b) => b.position_type == ConstData.POSITION_SALES);
                else if (emp_category == 21)
                    qable.Where<daoben_hr_emp_job>((a, b) => b.position_type == ConstData.POSITION_SALESMANAGER);
                totalInfo totalInfo = qable.Select<totalInfo>("SUM(a.total_reward) as total_reward, COUNT(*) as total_activity")
                        .SingleOrDefault();
                if (totalInfo == null)
                    return "系统错误：无法获取总数";
                return totalInfo;
            }
        }
        #endregion

        #region 主推活动
        public object RecomMainList(Pagination pagination, QueryTime queryTime, int company_id, string emp_name, int? emp_category)
        {
            var LoginInfo = OperatorProvider.Provider.GetCurrent();
            if (LoginInfo == null)
                return null;
            if (pagination == null)
                pagination = new Pagination();
            pagination.page = pagination.page > 0 ? pagination.page : 1;
            pagination.rows = pagination.rows > 0 ? pagination.rows : 20;
            pagination.sidx = string.IsNullOrEmpty(pagination.sidx) ? "a.emp_id" : pagination.sidx;
            pagination.sord = string.IsNullOrEmpty(pagination.sord) ? "desc" : pagination.sord;
            CompanyInfo myCompanyInfo = LoginInfo.companyInfo;
            PositionInfo myPositionInfo = LoginInfo.positionInfo;

            using (var db = SugarDao.GetInstance())
            {
                var qable = db.Queryable<daoben_activity_recommendation_emp>()
                    .JoinTable<daoben_hr_emp_job>((a, b) => b.id == a.emp_id)
                    .JoinTable<daoben_activity_recommendation>((a, c) => a.main_id == c.id)
                    .Where<daoben_activity_recommendation>((a, c) => c.activity_status == 2);       // 只统计已结束的活动

                if (myCompanyInfo.category == "事业部")
                    qable.Where<daoben_activity_recommendation>((a, c) => c.company_id_parent == myCompanyInfo.id);
                else if (myCompanyInfo.category == "分公司")
                    qable.Where<daoben_activity_recommendation>((a, c) => c.company_id == myCompanyInfo.id);
                if (company_id > 0)
                    qable.Where<daoben_activity_recommendation>((a, c) => c.company_id == company_id);

                if (queryTime.startTime1 != null)
                    qable.Where<daoben_activity_recommendation>((a, c) => c.end_date >= queryTime.startTime1);
                if (queryTime.startTime2 != null)
                {
                    queryTime.startTime2 = queryTime.startTime2.ToDate().AddDays(1);
                    qable.Where<daoben_activity_recommendation>((a, c) => c.end_date < queryTime.startTime2);
                }
                if (!string.IsNullOrEmpty(emp_name))
                    qable.Where(a => a.emp_name.Contains(emp_name));

                if (emp_category > 0)
                {
                    if (emp_category == 3)
                        qable.Where<daoben_hr_emp_job>((a, b) => b.position_type == ConstData.POSITION_GUIDE);
                    else if (emp_category == 20)
                        qable.Where<daoben_hr_emp_job>((a, b) => b.position_type == ConstData.POSITION_SALES);
                    else if (emp_category == 21)
                        qable.Where<daoben_hr_emp_job>((a, b) => b.position_type == ConstData.POSITION_SALESMANAGER);
                }
                string listStr = qable
                        .OrderBy(pagination.sidx + " " + pagination.sord)
                        .GroupBy("a.emp_id")
                        .Select("SQL_CALC_FOUND_ROWS COUNT(a.id) AS count,SUM(total_count) AS total_count,SUM(total_amount) AS total_amount,SUM(a.total_reward) AS total_reward,a.emp_name,a.emp_id as id,b.area_l2_name,b.company_linkname")
                        .ToJsonPage(pagination.page, pagination.rows);
                pagination.records = db.SqlQuery<int>("SELECT FOUND_ROWS() as count").FirstOrDefault();
                if (string.IsNullOrEmpty(listStr) || listStr == "[]")
                    return null;
                return listStr.ToJson();
            }
        }
        public object RecomList(Pagination pagination, QueryTime queryTime, string emp_id)
        {
            var LoginInfo = OperatorProvider.Provider.GetCurrent();
            if (LoginInfo == null)
                return null;
            int records = 0;
            if (pagination == null)
                pagination = new Pagination();
            pagination.page = pagination.page > 0 ? pagination.page : 1;
            pagination.rows = pagination.rows > 0 ? pagination.rows : 20;
            pagination.sidx = string.IsNullOrEmpty(pagination.sidx) ? "c.name" : pagination.sidx;
            pagination.sord = string.IsNullOrEmpty(pagination.sord) ? "desc" : pagination.sord;

            using (var db = SugarDao.GetInstance())
            {
                var qable = db.Queryable<daoben_activity_recommendation_emp>()
                    .JoinTable<daoben_hr_emp_job>((a, b) => a.emp_id == b.id)
                    .JoinTable<daoben_activity_recommendation>((a, c) => a.main_id == c.id)
                    .Where<daoben_activity_recommendation>((a, c) => c.activity_status == 2 && a.emp_id == emp_id);       // 只统计已结束的活动

                if (queryTime.startTime1 != null)
                    qable.Where<daoben_activity_recommendation>((a, c) => c.end_date >= queryTime.startTime1);
                if (queryTime.startTime2 != null)
                {
                    queryTime.startTime2 = queryTime.startTime2.ToDate().AddDays(1);
                    qable.Where<daoben_activity_recommendation>((a, c) => c.end_date < queryTime.startTime2);
                }
                string listStr = qable
                        .OrderBy(pagination.sidx + " " + pagination.sord)
                        .Select("c.id,c.name,c.start_date,c.end_date,a.total_amount,a.total_count,a.total_ratio,a.total_reward,c.target_mode,c.activity_target,c.target_content")
                        .ToJsonPage(pagination.page, pagination.rows);
                pagination.records = records;
                if (string.IsNullOrEmpty(listStr) || listStr == "[]")
                    return null;
                return listStr.ToJson();
            }
        }

        public MemoryStream ExportExcelRecom(Pagination pagination, QueryTime queryTime, int? company_id, string emp_name, int? emp_category)
        {
            var LoginInfo = OperatorProvider.Provider.GetCurrent();
            if (LoginInfo == null)
                return null;
            if (pagination == null)
                pagination = new Pagination();
            pagination.sidx = string.IsNullOrEmpty(pagination.sidx) ? "c.name" : pagination.sidx;
            pagination.sord = string.IsNullOrEmpty(pagination.sord) ? "desc" : pagination.sord;
            CompanyInfo myCompanyInfo = LoginInfo.companyInfo;
            PositionInfo myPositionInfo = LoginInfo.positionInfo;
            using (var db = SugarDao.GetInstance())
            {
                var qable = db.Queryable<daoben_activity_recommendation_emp>()
                    .JoinTable<daoben_hr_emp_job>((a, b) => a.emp_id == b.id)
                    .JoinTable<daoben_activity_recommendation>((a, c) => a.main_id == c.id)
                    .Where<daoben_activity_recommendation>((a, c) => c.activity_status == 2);       // 只统计已结束的活动

                if (myCompanyInfo.category == "事业部")
                    qable.Where<daoben_activity_recommendation>((a, c) => c.company_id_parent == myCompanyInfo.id);
                else if (myCompanyInfo.category == "分公司")
                    qable.Where<daoben_activity_recommendation>((a, c) => c.company_id == myCompanyInfo.id);
                if (company_id > 0)
                    qable.Where<daoben_activity_recommendation>((a, c) => c.company_id == company_id);

                if (queryTime.startTime1 != null)
                    qable.Where<daoben_activity_recommendation>((a, c) => c.end_date >= queryTime.startTime1);
                if (queryTime.startTime2 != null)
                {
                    queryTime.startTime2 = queryTime.startTime2.ToDate().AddDays(1);
                    qable.Where<daoben_activity_recommendation>((a, c) => c.end_date < queryTime.startTime2);
                }
                if (!string.IsNullOrEmpty(emp_name))
                    qable.Where(a => a.emp_name.Contains(emp_name));

                if (emp_category > 0)
                {
                    if (emp_category == 3)
                        qable.Where<daoben_hr_emp_job>((a, b) => b.position_type == ConstData.POSITION_GUIDE);
                    else if (emp_category == 20)
                        qable.Where<daoben_hr_emp_job>((a, b) => b.position_type == ConstData.POSITION_SALES);
                    else if (emp_category == 21)
                        qable.Where<daoben_hr_emp_job>((a, b) => b.position_type == ConstData.POSITION_SALESMANAGER);
                }

                string selStr = "b.company_linkname,b.name as name,b.position_name,c.name as activity_name,a.total_amount,DATE_FORMAT(c.start_date,'%Y-%m-%d'),DATE_FORMAT(c.end_date,'%Y-%m-%d'),a.total_reward";
                var listDt = qable.OrderBy(pagination.sidx + " " + pagination.sord)
                        .OrderBy("a.emp_id,c.end_date")
                        .Select(selStr).ToDataTable();
                int rowsCount = listDt.Rows.Count;

                string[] headerArr = new string[] { "所属机构", "员工名称", "职位", "活动名称", "销量总金额", "活动开始时间", "活动结束时间", "奖励金额" };
                int[] colWidthArr = new int[] { 18, 20, 15, 28, 18, 20, 20, 18 };

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

        public object GetTotalRecom(QueryTime queryTime, int? company_id, string emp_name, int? emp_category = 0)
        {
            OperatorModel LoginInfo = OperatorProvider.Provider.GetCurrent();
            if (LoginInfo == null)
                throw new Exception("用户登陆过期，请重新登录");
            CompanyInfo myCompanyInfo = LoginInfo.companyInfo;
            using (var db = SugarDao.GetInstance())
            {
                var qable = db.Queryable<daoben_activity_recommendation_emp>()
                        .JoinTable<daoben_hr_emp_job>((a, b) => b.id == a.emp_id)
                        .JoinTable<daoben_activity_recommendation>((a, c) => a.main_id == c.id)
                        .Where<daoben_activity_recommendation>((a, c) => c.activity_status == 2);       // 只统计已结束的活动
                if (myCompanyInfo.category == "事业部")
                    qable.Where<daoben_activity_recommendation>((a, c) => c.company_id_parent == myCompanyInfo.id);
                else if (myCompanyInfo.category == "分公司")
                    qable.Where<daoben_activity_recommendation>((a, c) => c.company_id == myCompanyInfo.id);
                if (company_id > 0)
                    qable.Where<daoben_activity_recommendation>((a, c) => c.company_id == company_id);

                if (queryTime.startTime1 != null)
                    qable.Where<daoben_activity_recommendation>((a, c) => c.end_date >= queryTime.startTime1);
                if (queryTime.startTime2 != null)
                {
                    queryTime.startTime2 = queryTime.startTime2.ToDate().AddDays(1);
                    qable.Where<daoben_activity_recommendation>((a, c) => c.end_date < queryTime.startTime2);
                }
                if (!string.IsNullOrEmpty(emp_name))
                    qable.Where(a => a.emp_name.Contains(emp_name));

                if (emp_category == 3)
                    qable.Where<daoben_hr_emp_job>((a, b) => b.position_type == ConstData.POSITION_GUIDE);
                else if (emp_category == 20)
                    qable.Where<daoben_hr_emp_job>((a, b) => b.position_type == ConstData.POSITION_SALES);
                else if (emp_category == 21)
                    qable.Where<daoben_hr_emp_job>((a, b) => b.position_type == ConstData.POSITION_SALESMANAGER);

                totalInfo totalInfo = qable.Select<totalInfo>("SUM(a.total_reward) as total_reward, COUNT(*) as total_activity")
                        .SingleOrDefault();
                if (totalInfo == null)
                    return "系统错误：无法获取总数";
                return totalInfo;
            }
        }
        #endregion

        #region 排名活动
        public object RankMainList(Pagination pagination, QueryTime queryTime, int company_id, string emp_name, int? emp_category)
        {
            var LoginInfo = OperatorProvider.Provider.GetCurrent();
            if (LoginInfo == null)
                return null;
            if (pagination == null)
                pagination = new Pagination();
            pagination.page = pagination.page > 0 ? pagination.page : 1;
            pagination.rows = pagination.rows > 0 ? pagination.rows : 20;
            pagination.sidx = string.IsNullOrEmpty(pagination.sidx) ? "a.emp_id" : pagination.sidx;
            pagination.sord = string.IsNullOrEmpty(pagination.sord) ? "desc" : pagination.sord;
            CompanyInfo myCompanyInfo = LoginInfo.companyInfo;
            PositionInfo myPositionInfo = LoginInfo.positionInfo;
            using (var db = SugarDao.GetInstance())
            {
                var qable = db.Queryable<daoben_activity_ranking_emp>()
                        .JoinTable<daoben_hr_emp_job>((a, b) => b.id == a.emp_id)
                        .JoinTable<daoben_activity_ranking>((a, c) => a.main_id == c.id)
                        .Where<daoben_activity_ranking>((a, c) => c.activity_status == 2);       // 只统计已结束的活动

                if (company_id > 0)
                    qable.Where<daoben_activity_ranking>((a, c) => c.company_id == company_id);
                else
                {
                    if (myCompanyInfo.category == "事业部")
                        qable.Where<daoben_activity_ranking>((a, c) => c.company_id_parent == myCompanyInfo.id);
                    else if (myCompanyInfo.category == "分公司")
                        qable.Where<daoben_activity_ranking>((a, c) => c.company_id == myCompanyInfo.id);
                }
                if (queryTime.startTime1 != null)
                    qable.Where<daoben_activity_ranking>((a, c) => c.end_date >= queryTime.startTime1);
                if (queryTime.startTime2 != null)
                {
                    queryTime.startTime2 = queryTime.startTime2.ToDate().AddDays(1);
                    qable.Where<daoben_activity_ranking>((a, c) => c.end_date < queryTime.startTime2);
                }
                if (!string.IsNullOrEmpty(emp_name))
                    qable.Where(a => a.emp_name.Contains(emp_name));

                if (emp_category == 3)
                    qable.Where<daoben_hr_emp_job>((a, b) => b.position_type == ConstData.POSITION_GUIDE);
                else if (emp_category == 20)
                    qable.Where<daoben_hr_emp_job>((a, b) => b.position_type == ConstData.POSITION_SALES);
                else if (emp_category == 21)
                    qable.Where<daoben_hr_emp_job>((a, b) => b.position_type == ConstData.POSITION_SALESMANAGER);
                string listStr = qable.OrderBy(pagination.sidx + " " + pagination.sord)
                        .GroupBy("a.emp_id")
                        .Select("SQL_CALC_FOUND_ROWS COUNT(a.id) AS count,SUM(counting_count) AS total_count,SUM(counting_amount) AS total_amount,SUM(a.reward) AS total_reward,a.emp_name,a.emp_id as id,c.emp_category,a.area_l2_name,b.company_linkname")
                        .ToJsonPage(pagination.page, pagination.rows);
                pagination.records = db.SqlQuery<int>("SELECT FOUND_ROWS() as count").FirstOrDefault();
                if (string.IsNullOrEmpty(listStr) || listStr == "[]")
                    return null;
                return listStr.ToJson();
            }
        }
        public object RankList(Pagination pagination, QueryTime queryTime, string emp_id)
        {
            var LoginInfo = OperatorProvider.Provider.GetCurrent();
            if (LoginInfo == null)
                return null;
            int records = 0;
            if (pagination == null)
                pagination = new Pagination();
            pagination.page = pagination.page > 0 ? pagination.page : 1;
            pagination.rows = pagination.rows > 0 ? pagination.rows : 20;
            pagination.sidx = string.IsNullOrEmpty(pagination.sidx) ? "c.end_date" : pagination.sidx;
            pagination.sord = string.IsNullOrEmpty(pagination.sord) ? "desc" : pagination.sord;

            using (var db = SugarDao.GetInstance())
            {
                var qable = db.Queryable<daoben_activity_ranking_emp>()
                        .JoinTable<daoben_hr_emp_job>((a, b) => a.emp_id == b.id)
                        .JoinTable<daoben_activity_ranking>((a, c) => a.main_id == c.id)
                        .Where<daoben_activity_ranking>((a, c) => c.activity_status == 2 && a.emp_id == emp_id);

                if (queryTime.startTime1 != null)
                    qable.Where<daoben_activity_ranking>((a, c) => c.end_date >= queryTime.startTime1);
                if (queryTime.startTime2 != null)
                {
                    queryTime.startTime2 = queryTime.startTime2.ToDate().AddDays(1);
                    qable.Where<daoben_activity_ranking>((a, c) => c.end_date < queryTime.startTime2);
                }
                string listStr = qable.OrderBy(pagination.sidx + " " + pagination.sord)
                        .Select("c.id,c.name,c.start_date,c.end_date,a.counting_amount,a.counting_count,a.counting_palce,a.reward,c.emp_category,c.ranking_content")
                        .ToJsonPage(pagination.page, pagination.rows, ref records);
                pagination.records = records;
                if (string.IsNullOrEmpty(listStr) || listStr == "[]")
                    return null;
                return listStr.ToJson();
            }
        }
        public MemoryStream ExportExcelRank(Pagination pagination, QueryTime queryTime, int? company_id, string emp_name, int? emp_category)
        {
            var LoginInfo = OperatorProvider.Provider.GetCurrent();
            if (LoginInfo == null)
                return null;
            if (pagination == null)
                pagination = new Pagination();
            pagination.sidx = string.IsNullOrEmpty(pagination.sidx) ? "c.name" : pagination.sidx;
            pagination.sord = string.IsNullOrEmpty(pagination.sord) ? "desc" : pagination.sord;
            CompanyInfo myCompanyInfo = LoginInfo.companyInfo;
            PositionInfo myPositionInfo = LoginInfo.positionInfo;
            using (var db = SugarDao.GetInstance())
            {
                var qable = db.Queryable<daoben_activity_ranking_emp>()
                        .JoinTable<daoben_hr_emp_job>((a, b) => a.emp_id == b.id)
                        .JoinTable<daoben_activity_ranking>((a, c) => a.main_id == c.id)
                        .Where<daoben_activity_ranking>((a, c) => c.activity_status == 2);       // 只统计已结束的活动

                if (company_id > 0)
                    qable.Where<daoben_activity_ranking>((a, c) => c.company_id == company_id);
                else
                {
                    if (myCompanyInfo.category == "事业部")
                        qable.Where<daoben_activity_ranking>((a, c) => c.company_id_parent == myCompanyInfo.id);
                    else if (myCompanyInfo.category == "分公司")
                        qable.Where<daoben_activity_ranking>((a, c) => c.company_id == myCompanyInfo.id);
                }

                if (queryTime.startTime1 != null)
                    qable.Where<daoben_activity_ranking>((a, c) => c.end_date >= queryTime.startTime1);
                if (queryTime.startTime2 != null)
                {
                    queryTime.startTime2 = queryTime.startTime2.ToDate().AddDays(1);
                    qable.Where<daoben_activity_ranking>((a, c) => c.end_date < queryTime.startTime2);
                }
                if (!string.IsNullOrEmpty(emp_name))
                    qable.Where(a => a.emp_name.Contains(emp_name));

                if (emp_category == 3)
                    qable.Where<daoben_hr_emp_job>((a, b) => b.position_type == ConstData.POSITION_GUIDE);
                else if (emp_category == 20)
                    qable.Where<daoben_hr_emp_job>((a, b) => b.position_type == ConstData.POSITION_SALES);
                else if (emp_category == 21)
                    qable.Where<daoben_hr_emp_job>((a, b) => b.position_type == ConstData.POSITION_SALESMANAGER);

                string selStr = "b.company_linkname,b.name as name,b.position_name,c.name as activity_name,a.counting_count,a.counting_amount,DATE_FORMAT(c.start_date,'%Y-%m-%d'),DATE_FORMAT(c.end_date,'%Y-%m-%d'),a.final_place,a.reward";
                var listDt = qable.OrderBy(pagination.sidx + " " + pagination.sord)
                        .OrderBy("a.emp_id,c.end_date")
                        .Select(selStr).ToDataTable();
                int rowsCount = listDt.Rows.Count;

                string[] headerArr = new string[] { "所属机构", "员工名称", "职位", "活动名称", "统计销量", "统计金额", "活动开始时间", "活动结束时间", "排名", "奖励金额" };
                int[] colWidthArr = new int[] { 18, 20, 15, 28, 18, 18, 20, 20, 18, 18 };

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
        public object GetTotalRank(QueryTime queryTime, int company_id, string emp_name, int emp_category = 0)
        {
            OperatorModel LoginInfo = OperatorProvider.Provider.GetCurrent();
            if (LoginInfo == null)
                throw new Exception("用户登陆过期，请重新登录");
            CompanyInfo myCompanyInfo = LoginInfo.companyInfo;

            using (var db = SugarDao.GetInstance())
            {
                var qable = db.Queryable<daoben_activity_ranking_emp>()
                        .JoinTable<daoben_hr_emp_job>((a, b) => b.id == a.emp_id)
                        .JoinTable<daoben_activity_ranking>((a, c) => a.main_id == c.id)
                        .Where<daoben_activity_ranking>((a, c) => c.activity_status == 2);       // 只统计已结束的活动

                if (company_id > 0)
                    qable.Where<daoben_activity_ranking>((a, c) => c.company_id == company_id);
                else
                {
                    if (myCompanyInfo.category == "事业部")
                        qable.Where<daoben_activity_ranking>((a, c) => c.company_id_parent == myCompanyInfo.id);
                    else if (myCompanyInfo.category == "分公司")
                        qable.Where<daoben_activity_ranking>((a, c) => c.company_id == myCompanyInfo.id);
                }
                if (emp_category == 3)
                    qable.Where<daoben_hr_emp_job>((a, b) => b.position_type == ConstData.POSITION_GUIDE);
                else if (emp_category == 20)
                    qable.Where<daoben_hr_emp_job>((a, b) => b.position_type == ConstData.POSITION_SALES);
                else if (emp_category == 21)
                    qable.Where<daoben_hr_emp_job>((a, b) => b.position_type == ConstData.POSITION_SALESMANAGER);

                if (queryTime.startTime1 != null)
                    qable.Where<daoben_activity_ranking>((a, c) => c.end_date >= queryTime.startTime1);
                if (queryTime.startTime2 != null)
                {
                    queryTime.startTime2 = queryTime.startTime2.ToDate().AddDays(1);
                    qable.Where<daoben_activity_ranking>((a, c) => c.end_date < queryTime.startTime2);
                }
                if (!string.IsNullOrEmpty(emp_name))
                    qable.Where<daoben_hr_emp_job>((a, b) => b.name.Contains(emp_name));

                totalInfo totalInfo = qable.Select<totalInfo>("SUM(a.reward) as total_reward, COUNT(*) as total_activity")
                        .SingleOrDefault();
                if (totalInfo == null)
                    return "系统错误：无法获取总数";
                return totalInfo;
            }
        }
        #endregion

        #region PK活动
        public object PKMainList(Pagination pagination, QueryTime queryTime, int company_id, string emp_name, int? emp_category)
        {
            var LoginInfo = OperatorProvider.Provider.GetCurrent();
            if (LoginInfo == null)
                return null;
            if (pagination == null)
                pagination = new Pagination();
            pagination.page = pagination.page > 0 ? pagination.page : 1;
            pagination.rows = pagination.rows > 0 ? pagination.rows : 20;
            pagination.sidx = string.IsNullOrEmpty(pagination.sidx) ? "a.emp_id" : pagination.sidx;
            pagination.sord = string.IsNullOrEmpty(pagination.sord) ? "desc" : pagination.sord;
            CompanyInfo myCompanyInfo = LoginInfo.companyInfo;
            PositionInfo myPositionInfo = LoginInfo.positionInfo;

            using (var db = SugarDao.GetInstance())
            {
                var qable = db.Queryable<daoben_activity_pk_emp>()
                    .JoinTable<daoben_hr_emp_job>((a, b) => b.id == a.emp_id)
                    .JoinTable<daoben_activity_pk>((a, c) => a.main_id == c.id)
                    .Where<daoben_activity_pk>((a, c) => c.activity_status == 2);       // 只统计已结束的活动

                if (myCompanyInfo.category == "事业部")
                    qable.Where<daoben_activity_pk>((a, c) => c.company_id_parent == myCompanyInfo.id);
                else if (myCompanyInfo.category == "分公司")
                    qable.Where<daoben_activity_pk>((a, c) => c.company_id == myCompanyInfo.id);
                if (company_id > 0)
                    qable.Where<daoben_hr_emp_job>((a, b) => b.company_id == company_id);

                if (queryTime.startTime1 != null)
                    qable.Where<daoben_activity_pk>((a, c) => c.end_date >= queryTime.startTime1);
                if (queryTime.startTime2 != null)
                {
                    queryTime.startTime2 = queryTime.startTime2.ToDate().AddDays(1);
                    qable.Where<daoben_activity_pk>((a, c) => c.end_date < queryTime.startTime2);
                }

                if (!string.IsNullOrEmpty(emp_name))
                    qable.Where(a => a.emp_name.Contains(emp_name));

                if (emp_category == 3)
                    qable.Where<daoben_hr_emp_job>((a, b) => b.position_type == ConstData.POSITION_GUIDE);
                else if (emp_category == 20)
                    qable.Where<daoben_hr_emp_job>((a, b) => b.position_type == ConstData.POSITION_SALES);
                else if (emp_category == 21)
                    qable.Where<daoben_hr_emp_job>((a, b) => b.position_type == ConstData.POSITION_SALESMANAGER);

                string listStr = qable
                        .OrderBy(pagination.sidx + " " + pagination.sord)
                        .GroupBy("a.emp_id")
                        .Select("SQL_CALC_FOUND_ROWS COUNT(a.id) AS count,SUM(total_count) AS total_count,SUM(total_amount) AS total_amount,SUM(a.total_reward) AS total_reward,a.emp_name,a.emp_id as id,c.emp_category,a.area_l2_name,b.company_linkname")
                        .ToJsonPage(pagination.page, pagination.rows);
                pagination.records = db.SqlQuery<int>("SELECT FOUND_ROWS() as count").FirstOrDefault();
                if (string.IsNullOrEmpty(listStr) || listStr == "[]")
                    return null;
                return listStr.ToJson();
            }
        }
        public object PKList(Pagination pagination, QueryTime queryTime, string emp_id)
        {
            var LoginInfo = OperatorProvider.Provider.GetCurrent();
            if (LoginInfo == null)
                return null;
            int records = 0;
            if (pagination == null)
                pagination = new Pagination();
            pagination.page = pagination.page > 0 ? pagination.page : 1;
            pagination.rows = pagination.rows > 0 ? pagination.rows : 20;
            pagination.sidx = string.IsNullOrEmpty(pagination.sidx) ? "c.name" : pagination.sidx;
            pagination.sord = string.IsNullOrEmpty(pagination.sord) ? "desc" : pagination.sord;

            using (var db = SugarDao.GetInstance())
            {
                var qable = db.Queryable<daoben_activity_pk_emp>()
                    //.JoinTable<daoben_hr_emp_job>((a, b) => a.emp_id == b.id)
                    .JoinTable<daoben_activity_pk>((a, c) => a.main_id == c.id)
                    .Where<daoben_activity_pk>((a, c) => c.activity_status == 2 && a.emp_id == emp_id);

                if (queryTime.startTime1 != null)
                    qable.Where<daoben_activity_pk>((a, c) => c.end_date >= queryTime.startTime1);
                if (queryTime.startTime2 != null)
                {
                    queryTime.startTime2 = queryTime.startTime2.ToDate().AddDays(1);
                    qable.Where<daoben_activity_pk>((a, c) => c.end_date < queryTime.startTime2);
                }
                string listStr = qable
                        .OrderBy(pagination.sidx + " " + pagination.sord)
                        .Select("c.id,c.name,c.start_date,c.end_date,a.total_amount,a.activity_target,a.total_count,a.winner,a.total_ratio,a.total_reward,c.emp_category")
                        .ToJsonPage(pagination.page, pagination.rows, ref records);
                pagination.records = records;
                if (string.IsNullOrEmpty(listStr) || listStr == "[]")
                    return null;
                return listStr.ToJson();
            }
        }
        public MemoryStream ExportExcelPK(Pagination pagination, QueryTime queryTime, int? company_id, string emp_name, int? emp_category)
        {
            var LoginInfo = OperatorProvider.Provider.GetCurrent();
            if (LoginInfo == null)
                return null;
            if (pagination == null)
                pagination = new Pagination();
            pagination.sidx = string.IsNullOrEmpty(pagination.sidx) ? "c.name" : pagination.sidx;
            pagination.sord = string.IsNullOrEmpty(pagination.sord) ? "desc" : pagination.sord;
            CompanyInfo myCompanyInfo = LoginInfo.companyInfo;
            PositionInfo myPositionInfo = LoginInfo.positionInfo;

            using (var db = SugarDao.GetInstance())
            {
                var qable = db.Queryable<daoben_activity_pk_emp>()
                    .JoinTable<daoben_hr_emp_job>((a, b) => a.emp_id == b.id)
                    .JoinTable<daoben_activity_pk>((a, c) => a.main_id == c.id)
                    .Where<daoben_activity_pk>((a, c) => c.activity_status == 2);       // 只统计已结束的活动

                if (myCompanyInfo.category == "事业部")
                    qable.Where<daoben_activity_pk>((a, c) => c.company_id_parent == myCompanyInfo.id);
                else if (myCompanyInfo.category == "分公司")
                    qable.Where<daoben_activity_pk>((a, c) => c.company_id == myCompanyInfo.id);
                if (company_id > 0)
                    qable.Where<daoben_hr_emp_job>((a, b) => b.company_id == company_id);

                if (queryTime.startTime1 != null)
                    qable.Where<daoben_activity_pk>((a, c) => c.end_date >= queryTime.startTime1);
                if (queryTime.startTime2 != null)
                {
                    queryTime.startTime2 = queryTime.startTime2.ToDate().AddDays(1);
                    qable.Where<daoben_activity_pk>((a, c) => c.end_date < queryTime.startTime2);
                }
                if (!string.IsNullOrEmpty(emp_name))
                    qable.Where(a => a.emp_name.Contains(emp_name));

                if (emp_category == 3)
                    qable.Where<daoben_hr_emp_job>((a, b) => b.position_type == ConstData.POSITION_GUIDE);
                else if (emp_category == 20)
                    qable.Where<daoben_hr_emp_job>((a, b) => b.position_type == ConstData.POSITION_SALES);
                else if (emp_category == 21)
                    qable.Where<daoben_hr_emp_job>((a, b) => b.position_type == ConstData.POSITION_SALESMANAGER);

                string selStr = "b.company_linkname,b.name as name,b.position_name,c.name as activity_name,a.total_count,a.activity_target,a.total_ratio,DATE_FORMAT(c.start_date,'%Y-%m-%d'),DATE_FORMAT(c.end_date,'%Y-%m-%d'),(CASE WHEN a.winner=1 THEN '赢' ELSE '输' END),a.total_reward";
                var listDt = qable.OrderBy(pagination.sidx + " " + pagination.sord)
                        .OrderBy("a.emp_id,c.end_date")
                        .Select(selStr).ToDataTable();
                int rowsCount = listDt.Rows.Count;

                string[] headerArr = new string[] { "所属机构", "员工名称", "职位", "活动名称", "统计销量", "目标销量", "完成率", "活动开始时间", "活动结束时间", "输赢", "金额" };
                int[] colWidthArr = new int[] { 18, 20, 15, 28, 18, 18, 20, 20, 18, 18, 18 };

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

        public object GetTotalPK(QueryTime queryTime, int? company_id, string emp_name, int? emp_category = 0)
        {
            OperatorModel LoginInfo = OperatorProvider.Provider.GetCurrent();
            if (LoginInfo == null)
                throw new Exception("用户登陆过期，请重新登录");
            CompanyInfo myCompanyInfo = LoginInfo.companyInfo;

            DateTime now = DateTime.Now;
            DateTime firstDayOfThisYear = new DateTime(now.Year, 1, 1);
            DateTime firstDayOfThisMonth = new DateTime(now.Year, now.Month, 1);
            using (var db = SugarDao.GetInstance())
            {
                var qable = db.Queryable<daoben_activity_pk_emp>()
                        .JoinTable<daoben_hr_emp_job>((a, b) => b.id == a.emp_id)
                        .JoinTable<daoben_activity_pk>((a, c) => a.main_id == c.id)
                        .Where<daoben_activity_pk>((a, c) => c.activity_status == 2);       // 只统计已结束的活动

                if (!string.IsNullOrEmpty(emp_name))
                    qable.Where<daoben_hr_emp_job>((a, b) => b.name.Contains(emp_name));

                if (myCompanyInfo.category == "事业部")
                    qable.Where<daoben_activity_pk>((a, c) => c.company_id_parent == myCompanyInfo.id);
                else if (myCompanyInfo.category == "分公司")
                    qable.Where<daoben_activity_pk>((a, c) => c.company_id == myCompanyInfo.id);
                if (company_id > 0)
                    qable.Where<daoben_hr_emp_job>((a, b) => b.company_id == company_id);

                if (emp_category == 3)
                    qable.Where<daoben_hr_emp_job>((a, b) => b.position_type == ConstData.POSITION_GUIDE);
                else if (emp_category == 20)
                    qable.Where<daoben_hr_emp_job>((a, b) => b.position_type == ConstData.POSITION_SALES);
                else if (emp_category == 21)
                    qable.Where<daoben_hr_emp_job>((a, b) => b.position_type == ConstData.POSITION_SALESMANAGER);

                if (queryTime.startTime1 != null)
                    qable.Where<daoben_activity_pk>((a, c) => c.end_date >= queryTime.startTime1);
                if (queryTime.startTime2 != null)
                {
                    queryTime.startTime2 = queryTime.startTime2.ToDate().AddDays(1);
                    qable.Where<daoben_activity_pk>((a, c) => c.end_date < queryTime.startTime2);
                }

                totalInfo totalInfo = qable.Select<totalInfo>("SUM(a.total_reward) as total_reward, COUNT(*) as total_activity")
                        .SingleOrDefault();
                if (totalInfo == null)
                    return "系统错误：无法获取总数";
                return totalInfo;
            }
        }
        #endregion

        #region 业务考核
        public object PerfMainList(Pagination pagination, QueryTime queryTime, int company_id, string emp_name, int? emp_category)
        {
            var LoginInfo = OperatorProvider.Provider.GetCurrent();
            if (LoginInfo == null)
                return null;
            if (pagination == null)
                pagination = new Pagination();
            pagination.page = pagination.page > 0 ? pagination.page : 1;
            pagination.rows = pagination.rows > 0 ? pagination.rows : 20;
            pagination.sidx = string.IsNullOrEmpty(pagination.sidx) ? "a.emp_id" : pagination.sidx;
            pagination.sord = string.IsNullOrEmpty(pagination.sord) ? "desc" : pagination.sord;
            CompanyInfo myCompanyInfo = LoginInfo.companyInfo;
            PositionInfo myPositionInfo = LoginInfo.positionInfo;
            using (var db = SugarDao.GetInstance())
            {
                var qable = db.Queryable<daoben_activity_sales_perf_emp>()
                    .JoinTable<daoben_hr_emp_job>((a, b) => b.id == a.emp_id)
                    .JoinTable<daoben_activity_sales_perf>((a, c) => a.main_id == c.id)
                    .Where<daoben_activity_sales_perf>((a, c) => c.activity_status == 2);       // 只统计已结束的活动

                if (myCompanyInfo.category == "事业部")
                    qable.Where<daoben_activity_sales_perf>((a, c) => c.company_id_parent == myCompanyInfo.id);
                else if (myCompanyInfo.category == "分公司")
                    qable.Where<daoben_activity_sales_perf>((a, c) => c.company_id == myCompanyInfo.id);
                if (company_id > 0)
                    qable.Where<daoben_hr_emp_job>((a, b) => b.company_id == company_id);

                if (queryTime.startTime1 != null)
                    qable.Where<daoben_activity_sales_perf>((a, c) => c.end_date >= queryTime.startTime1);
                if (queryTime.startTime2 != null)
                {
                    queryTime.startTime2 = queryTime.startTime2.ToDate().AddDays(1);
                    qable.Where<daoben_activity_sales_perf>((a, c) => c.end_date < queryTime.startTime2);
                }

                if (!string.IsNullOrEmpty(emp_name))
                    qable.Where(a => a.emp_name.Contains(emp_name));

                if (emp_category == 3)
                    qable.Where<daoben_hr_emp_job>((a, b) => b.position_type == ConstData.POSITION_GUIDE);
                else if (emp_category == 20)
                    qable.Where<daoben_hr_emp_job>((a, b) => b.position_type == ConstData.POSITION_SALES);
                else if (emp_category == 21)
                    qable.Where<daoben_hr_emp_job>((a, b) => b.position_type == ConstData.POSITION_SALESMANAGER);

                string listStr = qable
                        .OrderBy(pagination.sidx + " " + pagination.sord)
                        .GroupBy("a.emp_id")
                        .Select("SQL_CALC_FOUND_ROWS COUNT(a.id) AS count,SUM(a.total_count) AS total_count,SUM(a.total_amount) AS total_amount,SUM(a.total_reward) AS total_reward,a.emp_name,a.emp_id as id,c.emp_category,a.area_l2_name,b.company_linkname")
                        .ToJsonPage(pagination.page, pagination.rows);
                pagination.records = db.SqlQuery<int>("SELECT FOUND_ROWS() as count").FirstOrDefault();
                if (string.IsNullOrEmpty(listStr) || listStr == "[]")
                    return null;
                return listStr.ToJson();
            }
        }
        public object PerfList(Pagination pagination, QueryTime queryTime, string emp_id)
        {
            var LoginInfo = OperatorProvider.Provider.GetCurrent();
            if (LoginInfo == null)
                return null;
            int records = 0;
            if (pagination == null)
                pagination = new Pagination();
            pagination.page = pagination.page > 0 ? pagination.page : 1;
            pagination.rows = pagination.rows > 0 ? pagination.rows : 20;
            pagination.sidx = string.IsNullOrEmpty(pagination.sidx) ? "c.name" : pagination.sidx;
            pagination.sord = string.IsNullOrEmpty(pagination.sord) ? "desc" : pagination.sord;
            using (var db = SugarDao.GetInstance())
            {
                var qable = db.Queryable<daoben_activity_sales_perf_emp>()
                    .JoinTable<daoben_hr_emp_job>((a, b) => a.emp_id == b.id)
                    .JoinTable<daoben_activity_sales_perf>((a, c) => a.main_id == c.id)
                    .Where<daoben_activity_sales_perf>((a, c) => a.emp_id == emp_id && c.activity_status == 2);       // 只统计已结束的活动;

                if (queryTime.startTime1 != null)
                    qable.Where<daoben_activity_sales_perf>((a, c) => c.end_date >= queryTime.startTime1);
                if (queryTime.startTime2 != null)
                {
                    queryTime.startTime2 = queryTime.startTime2.ToDate().AddDays(1);
                    qable.Where<daoben_activity_sales_perf>((a, c) => c.end_date < queryTime.startTime2);
                }
                string listStr = qable
                        .OrderBy(pagination.sidx + " " + pagination.sord)
                        .Select("c.id,c.name,c.start_date,c.end_date,a.total_amount,a.total_count,a.total_ratio,a.total_reward,c.emp_category,c.target_content,c.target_mode")
                        .ToJsonPage(pagination.page, pagination.rows, ref records);
                pagination.records = records;
                if (string.IsNullOrEmpty(listStr) || listStr == "[]")
                    return null;
                return listStr.ToJson();
            }
        }
        public MemoryStream ExportExcelSalesPer(Pagination pagination, QueryTime queryTime, int? company_id, string emp_name, int? emp_category)
        {
            var LoginInfo = OperatorProvider.Provider.GetCurrent();
            if (LoginInfo == null)
                return null;
            if (pagination == null)
                pagination = new Pagination();
            pagination.sidx = string.IsNullOrEmpty(pagination.sidx) ? "c.name" : pagination.sidx;
            pagination.sord = string.IsNullOrEmpty(pagination.sord) ? "desc" : pagination.sord;
            CompanyInfo myCompanyInfo = LoginInfo.companyInfo;
            PositionInfo myPositionInfo = LoginInfo.positionInfo;
            DateTime now = DateTime.Now;
            DateTime firstDayOfThisYear = new DateTime(now.Year, 1, 1);
            using (var db = SugarDao.GetInstance())
            {
                var qable = db.Queryable<daoben_activity_sales_perf_emp>()
                    .JoinTable<daoben_hr_emp_job>((a, b) => a.emp_id == b.id)
                    .JoinTable<daoben_activity_sales_perf>((a, c) => a.main_id == c.id)
                    .Where<daoben_activity_sales_perf>((a, c) => c.activity_status == 2);       // 只统计已结束的活动
                if (myCompanyInfo.category == "事业部")
                    qable.Where<daoben_activity_sales_perf>((a, c) => c.company_id_parent == myCompanyInfo.id);
                else if (myCompanyInfo.category == "分公司")
                    qable.Where<daoben_activity_sales_perf>((a, c) => c.company_id == myCompanyInfo.id);
                if (company_id > 0)
                    qable.Where<daoben_hr_emp_job>((a, b) => b.company_id == company_id);

                if (queryTime.startTime1 != null)
                    qable.Where<daoben_activity_sales_perf>((a, c) => c.end_date >= queryTime.startTime1);
                if (queryTime.startTime2 != null)
                {
                    queryTime.startTime2 = queryTime.startTime2.ToDate().AddDays(1);
                    qable.Where<daoben_activity_sales_perf>((a, c) => c.end_date < queryTime.startTime2);
                }
                if (!string.IsNullOrEmpty(emp_name))
                    qable.Where(a => a.emp_name.Contains(emp_name));

                if (emp_category == 3)
                    qable.Where<daoben_hr_emp_job>((a, b) => b.position_type == ConstData.POSITION_GUIDE);
                else if (emp_category == 20)
                    qable.Where<daoben_hr_emp_job>((a, b) => b.position_type == ConstData.POSITION_SALES);
                else if (emp_category == 21)
                    qable.Where<daoben_hr_emp_job>((a, b) => b.position_type == ConstData.POSITION_SALESMANAGER);


                string selStr = "b.company_linkname,b.name as name,b.position_name,c.name as activity_name,a.total_amount,DATE_FORMAT(c.start_date,'%Y-%m-%d'),DATE_FORMAT(c.end_date,'%Y-%m-%d'),a.total_reward";
                var listDt = qable.OrderBy(pagination.sidx + " " + pagination.sord)
                        .OrderBy("a.emp_id,c.end_date")
                        .Select(selStr).ToDataTable();
                int rowsCount = listDt.Rows.Count;

                string[] headerArr = new string[] { "所属机构", "员工名称", "职位", "活动名称", "销量总金额", "活动开始时间", "活动结束时间", "奖励金额" };
                int[] colWidthArr = new int[] { 18, 20, 15, 28, 18, 20, 20, 18 };

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

        public object GetTotalSalesPer(QueryTime queryTime, int? company_id, string emp_name, int? emp_category = 0)
        {
            OperatorModel LoginInfo = OperatorProvider.Provider.GetCurrent();
            if (LoginInfo == null)
                throw new Exception("用户登陆过期，请重新登录");
            CompanyInfo myCompanyInfo = LoginInfo.companyInfo;
            using (var db = SugarDao.GetInstance())
            {
                var qable = db.Queryable<daoben_activity_sales_perf_emp>()
                        .JoinTable<daoben_hr_emp_job>((a, b) => b.id == a.emp_id)
                        .JoinTable<daoben_activity_sales_perf>((a, c) => a.main_id == c.id)
                        .Where<daoben_activity_sales_perf>((a, c) => c.activity_status == 2);       // 只统计已结束的活动
                if (!string.IsNullOrEmpty(emp_name))
                    qable.Where<daoben_hr_emp_job>((a, b) => b.name.Contains(emp_name));

                if (myCompanyInfo.category == "事业部")
                    qable.Where<daoben_activity_sales_perf>((a, c) => c.company_id_parent == myCompanyInfo.id);
                else if (myCompanyInfo.category == "分公司")
                    qable.Where<daoben_activity_sales_perf>((a, c) => c.company_id == myCompanyInfo.id);
                if (company_id > 0)
                    qable.Where<daoben_hr_emp_job>((a, b) => b.company_id == company_id);

                if (queryTime.startTime1 != null)
                    qable.Where<daoben_activity_sales_perf>((a, c) => c.end_date >= queryTime.startTime1);
                if (queryTime.startTime2 != null)
                {
                    queryTime.startTime2 = queryTime.startTime2.ToDate().AddDays(1);
                    qable.Where<daoben_activity_sales_perf>((a, c) => c.end_date < queryTime.startTime2);
                }
                if (!string.IsNullOrEmpty(emp_name))
                    qable.Where(a => a.emp_name.Contains(emp_name));

                if (emp_category == 3)
                    qable.Where<daoben_hr_emp_job>((a, b) => b.position_type == ConstData.POSITION_GUIDE);
                else if (emp_category == 20)
                    qable.Where<daoben_hr_emp_job>((a, b) => b.position_type == ConstData.POSITION_SALES);
                else if (emp_category == 21)
                    qable.Where<daoben_hr_emp_job>((a, b) => b.position_type == ConstData.POSITION_SALESMANAGER);


                totalInfo totalInfo = qable.Select<totalInfo>("SUM(a.total_reward) as total_reward, COUNT(*) as total_activity")
                        .SingleOrDefault();
                if (totalInfo == null)
                    return "系统错误：无法获取总数";
                return totalInfo;
            }
        }

        #endregion
    }
}
