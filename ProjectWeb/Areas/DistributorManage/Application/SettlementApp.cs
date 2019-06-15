using Base.Code;
using Base.Code.Security;
using MySqlSugar;
using NPOI.HSSF.UserModel;
using ProjectShare.Database;
using ProjectShare.Models;
using ProjectShare.Process;
using ProjectWeb.Application;
using ProjectWeb.Areas.ActivityManage.Application;
using ProjectWeb.Areas.SalaryCalculate.Application;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;

namespace ProjectWeb.Areas.DistributorManage.Application
{
    /// <summary>
    /// 运费信息
    /// Summary（汇总）、Shipping(运费)、Refund(补差)、Rebate（返利）
    /// </summary>
    public class SettlementApp
    {
        #region 运费       
        public object ShippingMainList(Pagination pagination, QueryTime queryTime, int company_id, string distributor_name)
        {
            var LoginInfo = OperatorProvider.Provider.GetCurrent();
            if (LoginInfo == null)
                return null;
            if (pagination == null)
                pagination = new Pagination();
            pagination.page = pagination.page > 0 ? pagination.page : 1;
            pagination.rows = pagination.rows > 0 ? pagination.rows : 20;
            pagination.sidx = string.IsNullOrEmpty(pagination.sidx) ? "a.distributor_name" : pagination.sidx;
            pagination.sord = string.IsNullOrEmpty(pagination.sord) ? "desc" : pagination.sord;
            CompanyInfo myCompanyInfo = LoginInfo.companyInfo;
            PositionInfo myPositionInfo = LoginInfo.positionInfo;

            using (var db = SugarDao.GetInstance())
            {
                var qable = db.Queryable<daoben_distributor_settlement_shipping>()
                    .JoinTable<daoben_distributor_info>((a, b) => a.distributor_id == b.id);

                if (myCompanyInfo.category == "事业部")
                    qable.Where(a => a.company_id_parent == myCompanyInfo.id);
                else if (myCompanyInfo.category == "分公司")
                    qable.Where(a => a.company_id == myCompanyInfo.id);
                if (company_id > 0)
                    qable.Where(a => a.company_id == company_id);

                if (queryTime.startTime1 != null)
                    qable.Where(a => a.shipping_date >= queryTime.startTime1);
                if (queryTime.startTime2 != null)
                {
                    queryTime.startTime2 = queryTime.startTime2.ToDate().AddDays(1);
                    qable.Where(a => a.shipping_date < queryTime.startTime2);
                }
                if (!string.IsNullOrEmpty(distributor_name))
                    qable.Where(a => a.distributor_name.Contains(distributor_name));
                string listStr = qable
                        .OrderBy(pagination.sidx + " " + pagination.sord)
                        .GroupBy("a.distributor_id")
                        .Select("SQL_CALC_FOUND_ROWS COUNT(total_count) AS count,SUM(total_count) AS total_count,SUM(total_amount) AS total_amount,a.distributor_name,a.distributor_id,b.address,b.company_linkname")
                        .ToJsonPage(pagination.page, pagination.rows);
                pagination.records = db.SqlQuery<int>("SELECT FOUND_ROWS() as count").FirstOrDefault();
                if (string.IsNullOrEmpty(listStr) || listStr == "[]")
                    return null;
                return listStr.ToJson();
            }
        }
        public object ShippingList(Pagination pagination, QueryTime queryTime, string distributor_id)
        {
            var LoginInfo = OperatorProvider.Provider.GetCurrent();
            if (LoginInfo == null)
                return null;
            int records = 0;
            if (pagination == null)
                pagination = new Pagination();
            pagination.page = pagination.page > 0 ? pagination.page : 1;
            pagination.rows = pagination.rows > 0 ? pagination.rows : 20;
            pagination.sidx = string.IsNullOrEmpty(pagination.sidx) ? "shipping_date" : pagination.sidx;
            pagination.sord = string.IsNullOrEmpty(pagination.sord) ? "desc" : pagination.sord;
            CompanyInfo myCompanyInfo = LoginInfo.companyInfo;
            PositionInfo myPositionInfo = LoginInfo.positionInfo;

            using (var db = SugarDao.GetInstance())
            {
                var qable = db.Queryable<daoben_distributor_settlement_shipping>()
                    .Where(a => a.distributor_id == distributor_id);

                if (queryTime.startTime1 != null)
                    qable.Where(a => a.shipping_date >= queryTime.startTime1);
                if (queryTime.startTime2 != null)
                {
                    queryTime.startTime2 = queryTime.startTime2.ToDate().AddDays(1);
                    qable.Where(a => a.shipping_date < queryTime.startTime2);
                }

                string listStr = qable
                        .OrderBy(pagination.sidx + " " + pagination.sord)
                        .ToJsonPage(pagination.page, pagination.rows, ref records);
                pagination.records = records;
                if (string.IsNullOrEmpty(listStr) || listStr == "[]")
                    return null;
                return listStr.ToJson();
            }
        }
        public MemoryStream ExportExcelShipping(Pagination pagination, QueryTime queryTime, int? company_id, string distributor_name)
        {
            var LoginInfo = OperatorProvider.Provider.GetCurrent();
            if (LoginInfo == null)
                return null;
            if (pagination == null)
                pagination = new Pagination();
            //pagination.sidx = string.IsNullOrEmpty(pagination.sidx) ? "a.distributor_id" : pagination.sidx;
            //pagination.sord = string.IsNullOrEmpty(pagination.sord) ? "desc" : pagination.sord;
            CompanyInfo myCompanyInfo = LoginInfo.companyInfo;
            PositionInfo myPositionInfo = LoginInfo.positionInfo;

            using (var db = SugarDao.GetInstance())
            {
                var qable = db.Queryable<daoben_distributor_settlement_shipping>()
                    .JoinTable<daoben_distributor_info>((a, b) => a.distributor_id == b.id);

                if (myCompanyInfo.category == "事业部")
                    qable.Where(a => a.company_id_parent == myCompanyInfo.id);
                else if (myCompanyInfo.category == "分公司")
                    qable.Where(a => a.company_id == myCompanyInfo.id);
                if (company_id > 0)
                    qable.Where(a => a.company_id == company_id);

                if (queryTime.startTime1 != null)
                    qable.Where(a => a.shipping_date >= queryTime.startTime1);
                if (queryTime.startTime2 != null)
                {
                    queryTime.startTime2 = queryTime.startTime2.ToDate().AddDays(1);
                    qable.Where(a => a.shipping_date < queryTime.startTime2);
                }
                if (!string.IsNullOrEmpty(distributor_name))
                    qable.Where(a => a.distributor_name.Contains(distributor_name));

                string selStr = "a.distributor_name,a.total_amount,a.total_count,a.shipping_date,a.company_linkname,b.address";
                var listDt = qable
                        .OrderBy("a.distributor_name,a.shipping_date asc")
                        .Select(selStr).ToDataTable();
                int rowsCount = listDt.Rows.Count;

                string[] headerArr = new string[] { "经销商名称", "运费", "数量", "日期", "所属机构", "详细地址" };
                int[] colWidthArr = new int[] { 40, 20, 20, 25, 25, 50 };

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
        public object GetTotalShipping(QueryTime queryTime, int? company_id, string distributor_name)
        {
            OperatorModel LoginInfo = OperatorProvider.Provider.GetCurrent();
            if (LoginInfo == null)
                throw new Exception("用户登陆过期，请重新登录");
            CompanyInfo myCompanyInfo = LoginInfo.companyInfo;

            using (var db = SugarDao.GetInstance())
            {
                var qable = db.Queryable<daoben_distributor_settlement_shipping>();

                if (myCompanyInfo.category == "事业部")
                    qable.Where(a => a.company_id_parent == myCompanyInfo.id);
                else if (myCompanyInfo.category == "分公司")
                    qable.Where(a => a.company_id == myCompanyInfo.id);
                if (company_id > 0)
                    qable.Where(a => a.company_id == company_id);

                if (queryTime.startTime1 != null)
                    qable.Where(a => a.shipping_date >= queryTime.startTime1);
                if (queryTime.startTime2 != null)
                {
                    queryTime.startTime2 = queryTime.startTime2.ToDate().AddDays(1);
                    qable.Where(a => a.shipping_date < queryTime.startTime2);
                }

                if (!string.IsNullOrEmpty(distributor_name))
                    qable.Where(a => a.distributor_name.Contains(distributor_name));

                totalInfo totalInfo = qable.Select<totalInfo>("SUM(total_amount) as total_amount, SUM(total_count) as total_count")
                        .SingleOrDefault();
                if (totalInfo == null)
                    return "系统错误：无法获取总数";
                return totalInfo;
            }
        }
        #endregion

        #region 调价补差
        public object RefundMainList(Pagination pagination, QueryTime queryTime, int? company_id, string distributor_name)
        {
            var LoginInfo = OperatorProvider.Provider.GetCurrent();
            if (LoginInfo == null)
                return null;
            if (pagination == null)
                pagination = new Pagination();
            pagination.page = pagination.page > 0 ? pagination.page : 1;
            pagination.rows = pagination.rows > 0 ? pagination.rows : 20;
            pagination.sidx = string.IsNullOrEmpty(pagination.sidx) ? "a.distributor_name" : pagination.sidx;
            pagination.sord = string.IsNullOrEmpty(pagination.sord) ? "desc" : pagination.sord;
            CompanyInfo myCompanyInfo = LoginInfo.companyInfo;
            PositionInfo myPositionInfo = LoginInfo.positionInfo;

            using (var db = SugarDao.GetInstance())
            {
                var qable = db.Queryable<daoben_sale_refund>()
                    .JoinTable<daoben_distributor_info>((a, b) => a.distributor_id == b.id)
                    .JoinTable<daoben_sale_refund_approve>((a, c) => a.import_file_id == c.id && c.status == 100);

                if (myCompanyInfo.category == "事业部")
                    qable.Where(a => a.company_id_parent == myCompanyInfo.id);
                else if (myCompanyInfo.category == "分公司")
                    qable.Where(a => a.company_id == myCompanyInfo.id);
                if (company_id > 0)
                    qable.Where(a => a.company_id == company_id);

                if (queryTime.startTime1 != null)
                    qable.Where(a => a.accur_time >= queryTime.startTime1);
                if (queryTime.startTime2 != null)
                {
                    queryTime.startTime2 = queryTime.startTime2.ToDate().AddDays(1);
                    qable.Where(a => a.accur_time < queryTime.startTime2);
                }
                if (!string.IsNullOrEmpty(distributor_name))
                    qable.Where(a => a.distributor_name.Contains(distributor_name));
                string listStr = qable
                        .OrderBy(pagination.sidx + " " + pagination.sord)
                        .GroupBy("a.distributor_id")
                        .Select("SQL_CALC_FOUND_ROWS SUM(diff_price) AS total_refund,Count(*) AS count,a.distributor_name,a.distributor_id,b.address,a.company_linkname")
                        .ToJsonPage(pagination.page, pagination.rows);
                pagination.records = db.SqlQuery<int>("SELECT FOUND_ROWS() as count").FirstOrDefault();
                if (string.IsNullOrEmpty(listStr) || listStr == "[]")
                    return null;
                return listStr.ToJson();
            }
        }

        public object RefundList(Pagination pagination, QueryTime queryTime, string distributor_id)
        {
            var LoginInfo = OperatorProvider.Provider.GetCurrent();
            if (LoginInfo == null)
                return null;
            int records = 0;
            if (pagination == null)
                pagination = new Pagination();
            pagination.page = pagination.page > 0 ? pagination.page : 1;
            pagination.rows = pagination.rows > 0 ? pagination.rows : 20;
            pagination.sidx = string.IsNullOrEmpty(pagination.sidx) ? "accur_time" : pagination.sidx;
            pagination.sord = string.IsNullOrEmpty(pagination.sord) ? "desc" : pagination.sord;

            using (var db = SugarDao.GetInstance())
            {
                var qable = db.Queryable<daoben_sale_refund>()
                    .JoinTable<daoben_sale_refund_approve>((a, b) => a.import_file_id == b.id && b.status == 100)
                    .Where(a => a.distributor_id == distributor_id);

                if (queryTime.startTime1 != null)
                    qable.Where(a => a.accur_time >= queryTime.startTime1);
                if (queryTime.startTime2 != null)
                {
                    queryTime.startTime2 = queryTime.startTime2.ToDate().AddDays(1);
                    qable.Where(a => a.accur_time < queryTime.startTime2);
                }

                string listStr = qable
                        .OrderBy(pagination.sidx + " " + pagination.sord)
                        .ToJsonPage(pagination.page, pagination.rows, ref records);
                pagination.records = records;
                if (string.IsNullOrEmpty(listStr) || listStr == "[]")
                    return null;
                return listStr.ToJson();
            }
        }

        public MemoryStream ExportExcelRefund(Pagination pagination, QueryTime queryTime, int? company_id, string distributor_name)
        {
            var LoginInfo = OperatorProvider.Provider.GetCurrent();
            if (LoginInfo == null)
                return null;
            if (pagination == null)
                pagination = new Pagination();
            CompanyInfo myCompanyInfo = LoginInfo.companyInfo;
            PositionInfo myPositionInfo = LoginInfo.positionInfo;

            using (var db = SugarDao.GetInstance())
            {
                var qable = db.Queryable<daoben_sale_refund>()
                    .JoinTable<daoben_distributor_info>((a, b) => a.distributor_id == b.id)
                    .JoinTable<daoben_sale_refund_approve>((a, c) => a.import_file_id == c.id && c.status == 100);                    
                if (myCompanyInfo.category == "事业部")
                    qable.Where(a => a.company_id_parent == myCompanyInfo.id);
                else if (myCompanyInfo.category == "分公司")
                    qable.Where(a => a.company_id == myCompanyInfo.id);
                if (company_id > 0)
                    qable.Where(a => a.company_id == company_id);

                if (queryTime.startTime1 != null)
                    qable.Where(a => a.accur_time >= queryTime.startTime1);
                if (queryTime.startTime2 != null)
                {
                    queryTime.startTime2 = queryTime.startTime2.ToDate().AddDays(1);
                    qable.Where(a => a.accur_time < queryTime.startTime2);
                }

                if (!string.IsNullOrEmpty(distributor_name))
                    qable.Where(a => a.distributor_name.Contains(distributor_name));

                string selStr = "a.distributor_name,a.model,a.phone_sn,a.diff_price,a.orig_price,a.new_price,a.accur_time,a.company_linkname,b.address";
                var listDt = qable
                        .OrderBy("a.distributor_name,a.accur_time asc")
                        .Select(selStr).ToDataTable();
                int rowsCount = listDt.Rows.Count;

                string[] headerArr = new string[] { "经销商名称", "型号", "串码", "补差金额", "原批发价", "新批发价", "时间", "所属机构", "详细地址" };
                int[] colWidthArr = new int[] { 40, 25, 25, 20, 20, 20, 25, 25, 50 };

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
        public object GetTotalRefund(QueryTime queryTime, int? company_id, string distributor_name)
        {
            OperatorModel LoginInfo = OperatorProvider.Provider.GetCurrent();
            if (LoginInfo == null)
                throw new Exception("用户登陆过期，请重新登录");
            CompanyInfo myCompanyInfo = LoginInfo.companyInfo;

            using (var db = SugarDao.GetInstance())
            {
                var qable = db.Queryable<daoben_sale_refund>()
                   .JoinTable<daoben_sale_refund_approve>((a, c) => a.import_file_id == c.id && c.status == 100);

                if (myCompanyInfo.category == "事业部")
                    qable.Where(a => a.company_id_parent == myCompanyInfo.id);
                else if (myCompanyInfo.category == "分公司")
                    qable.Where(a => a.company_id == myCompanyInfo.id);
                if (company_id > 0)
                    qable.Where(a => a.company_id == company_id);

                if (queryTime.startTime1 != null)
                    qable.Where(a => a.accur_time >= queryTime.startTime1);
                if (queryTime.startTime2 != null)
                {
                    queryTime.startTime2 = queryTime.startTime2.ToDate().AddDays(1);
                    qable.Where(a => a.accur_time < queryTime.startTime2);
                }

                if (!string.IsNullOrEmpty(distributor_name))
                    qable.Where(a => a.distributor_name.Contains(distributor_name));

                totalInfo totalInfo = qable.Select<totalInfo>("SUM(diff_price) AS total_amount, COUNT(*) AS total_count")
                        .SingleOrDefault();
                if (totalInfo == null)
                    return "系统错误：无法获取总数";
                return totalInfo;
            }
        }
        #endregion

        #region 买断补差
        public object BuyoutRefundMainList(Pagination pagination, QueryTime queryTime, int? company_id, string distributor_name)
        {
            var LoginInfo = OperatorProvider.Provider.GetCurrent();
            if (LoginInfo == null)
                return null;
            if (pagination == null)
                pagination = new Pagination();
            pagination.page = pagination.page > 0 ? pagination.page : 1;
            pagination.rows = pagination.rows > 0 ? pagination.rows : 20;
            pagination.sidx = string.IsNullOrEmpty(pagination.sidx) ? "a.distributor_name" : pagination.sidx;
            pagination.sord = string.IsNullOrEmpty(pagination.sord) ? "desc" : pagination.sord;
            CompanyInfo myCompanyInfo = LoginInfo.companyInfo;
            PositionInfo myPositionInfo = LoginInfo.positionInfo;

            using (var db = SugarDao.GetInstance())
            {
                var qable = db.Queryable<daoben_sale_buyout_import_temp>()
                    .JoinTable<daoben_distributor_info>((a, b) => a.distributor_id == b.id)
                    .JoinTable<daoben_sale_buyout_import_approve>((a, c) => a.import_file_id == c.id && c.status == 100);
                if (myCompanyInfo.category == "事业部")
                    qable.Where(a => a.company_id_parent == myCompanyInfo.id);
                else if (myCompanyInfo.category == "分公司")
                    qable.Where(a => a.company_id == myCompanyInfo.id);
                if (company_id > 0)
                    qable.Where(a => a.company_id == company_id);

                if (queryTime.startTime1 != null)
                    qable.Where(a => a.accur_time >= queryTime.startTime1);
                if (queryTime.startTime2 != null)
                {
                    queryTime.startTime2 = queryTime.startTime2.ToDate().AddDays(1);
                    qable.Where(a => a.accur_time < queryTime.startTime2);
                }
                if (!string.IsNullOrEmpty(distributor_name))
                    qable.Where(a => a.distributor_name.Contains(distributor_name));
                string listStr = qable
                        //.Where(a => a.buyout_refund > 0)
                        .OrderBy(pagination.sidx + " " + pagination.sord)
                        .GroupBy("a.distributor_id")
                        .Select("SQL_CALC_FOUND_ROWS SUM(buyout_refund) AS total_refund, Count(*) AS count,a.distributor_name,a.distributor_id,b.address,a.company_linkname")
                        .ToJsonPage(pagination.page, pagination.rows);
                pagination.records = db.SqlQuery<int>("SELECT FOUND_ROWS() as count").FirstOrDefault();
                if (string.IsNullOrEmpty(listStr) || listStr == "[]")
                    return null;
                return listStr.ToJson();
            }
        }

        public object BuyoutRefundList(Pagination pagination, QueryTime queryTime, string distributor_id)
        {
            var LoginInfo = OperatorProvider.Provider.GetCurrent();
            if (LoginInfo == null)
                return null;
            int records = 0;
            if (pagination == null)
                pagination = new Pagination();
            pagination.page = pagination.page > 0 ? pagination.page : 1;
            pagination.rows = pagination.rows > 0 ? pagination.rows : 20;
            pagination.sidx = string.IsNullOrEmpty(pagination.sidx) ? "accur_time" : pagination.sidx;
            pagination.sord = string.IsNullOrEmpty(pagination.sord) ? "desc" : pagination.sord;

            using (var db = SugarDao.GetInstance())
            {
                var qable = db.Queryable<daoben_sale_buyout_import_temp>()
                    .Where(a => a.distributor_id == distributor_id );

                if (queryTime.startTime1 != null)
                    qable.Where(a => a.accur_time >= queryTime.startTime1);
                if (queryTime.startTime2 != null)
                {
                    queryTime.startTime2 = queryTime.startTime2.ToDate().AddDays(1);
                    qable.Where(a => a.accur_time < queryTime.startTime2);
                }

                string listStr = qable
                        .Select("model,phone_sn,price_wholesale,price_buyout,buyout_refund,accur_Time")
                        .OrderBy(pagination.sidx + " " + pagination.sord)
                        .ToJsonPage(pagination.page, pagination.rows, ref records);
                pagination.records = records;
                if (string.IsNullOrEmpty(listStr) || listStr == "[]")
                    return null;
                return listStr.ToJson();
            }
        }

        public MemoryStream ExportExcelBuyoutRefund(Pagination pagination, QueryTime queryTime, int? company_id, string distributor_name)
        {
            var LoginInfo = OperatorProvider.Provider.GetCurrent();
            if (LoginInfo == null)
                return null;
            if (pagination == null)
                pagination = new Pagination();
            CompanyInfo myCompanyInfo = LoginInfo.companyInfo;
            PositionInfo myPositionInfo = LoginInfo.positionInfo;

            using (var db = SugarDao.GetInstance())
            {
                var qable = db.Queryable<daoben_sale_buyout_import_temp>()
                    .JoinTable<daoben_distributor_info>((a, b) => a.distributor_id == b.id)
                    .JoinTable<daoben_sale_buyout_import_approve>((a, c) => a.import_file_id == c.id && c.status == 100);
                   
                if (myCompanyInfo.category == "事业部")
                    qable.Where(a => a.company_id_parent == myCompanyInfo.id);
                else if (myCompanyInfo.category == "分公司")
                    qable.Where(a => a.company_id == myCompanyInfo.id);
                if (company_id > 0)
                    qable.Where(a => a.company_id == company_id);

                if (queryTime.startTime1 != null)
                    qable.Where(a => a.accur_time >= queryTime.startTime1);
                if (queryTime.startTime2 != null)
                {
                    queryTime.startTime2 = queryTime.startTime2.ToDate().AddDays(1);
                    qable.Where(a => a.accur_time < queryTime.startTime2);
                }
                if (!string.IsNullOrEmpty(distributor_name))
                    qable.Where(a => a.distributor_name.Contains(distributor_name));

                string selStr = "a.distributor_name,a.model,a.phone_sn,a.buyout_refund,a.price_wholesale,a.price_buyout,a.accur_time,a.company_linkname,b.address";
                var listDt = qable
                        .OrderBy("a.distributor_name,a.accur_time asc")
                        .Select(selStr).ToDataTable();
                int rowsCount = listDt.Rows.Count;

                string[] headerArr = new string[] { "经销商名称", "型号", "串码", "补差金额", "批发价", "买断价", "时间", "所属机构", "详细地址" };
                int[] colWidthArr = new int[] { 40, 25, 25, 20, 20, 20, 25, 25, 50 };

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
        public object GetTotalBuyoutRefund(QueryTime queryTime, int? company_id, string distributor_name)
        {
            OperatorModel LoginInfo = OperatorProvider.Provider.GetCurrent();
            if (LoginInfo == null)
                throw new Exception("用户登陆过期，请重新登录");
            CompanyInfo myCompanyInfo = LoginInfo.companyInfo;

            using (var db = SugarDao.GetInstance())
            {
                var qable = db.Queryable<daoben_sale_buyout_import_temp>()
                              .JoinTable<daoben_sale_buyout_import_approve>((a, c) => a.import_file_id == c.id && c.status == 100);
                              
                if (myCompanyInfo.category == "事业部")
                    qable.Where(a => a.company_id_parent == myCompanyInfo.id);
                else if (myCompanyInfo.category == "分公司")
                    qable.Where(a => a.company_id == myCompanyInfo.id);
                if (company_id > 0)
                    qable.Where(a => a.company_id == company_id);

                if (queryTime.startTime1 != null)
                    qable.Where(a => a.accur_time >= queryTime.startTime1);
                if (queryTime.startTime2 != null)
                {
                    queryTime.startTime2 = queryTime.startTime2.ToDate().AddDays(1);
                    qable.Where(a => a.accur_time < queryTime.startTime2);
                }

                if (!string.IsNullOrEmpty(distributor_name))
                    qable.Where(a => a.distributor_name.Contains(distributor_name));

                totalInfo totalInfo = qable.Select<totalInfo>("SUM(buyout_refund) AS total_amount, COUNT(*) AS total_count")
                        .SingleOrDefault();
                if (totalInfo == null)
                    return "系统错误：无法获取总数";
                return totalInfo;
            }
        }
        #endregion

        #region 包销补差
        public object ExclusiveRefundMainList(Pagination pagination, QueryTime queryTime, int? company_id, string distributor_name)
        {
            var LoginInfo = OperatorProvider.Provider.GetCurrent();
            if (LoginInfo == null)
                return null;
            if (pagination == null)
                pagination = new Pagination();
            pagination.page = pagination.page > 0 ? pagination.page : 1;
            pagination.rows = pagination.rows > 0 ? pagination.rows : 20;
            pagination.sidx = string.IsNullOrEmpty(pagination.sidx) ? "a.distributor_name" : pagination.sidx;
            pagination.sord = string.IsNullOrEmpty(pagination.sord) ? "desc" : pagination.sord;
            CompanyInfo myCompanyInfo = LoginInfo.companyInfo;
            PositionInfo myPositionInfo = LoginInfo.positionInfo;
          
            using (var db = SugarDao.GetInstance())
            {
                var qable = db.Queryable<daoben_sale_exclusive_detail>()
                    .JoinTable<daoben_distributor_info>((a, b) => a.distributor_id == b.id)
                    .JoinTable<daoben_sale_exclusive>((a, c) => a.main_id == c.id && c.status == 100);
                if (myCompanyInfo.category == "事业部")
                    qable.Where(a => a.company_id_parent == myCompanyInfo.id);
                else if (myCompanyInfo.category == "分公司")
                    qable.Where(a => a.company_id == myCompanyInfo.id);
                if (company_id > 0)
                    qable.Where<daoben_distributor_info>((a, b) => b.company_id == company_id);

                if (queryTime.startTime1 != null)
                    qable.Where(a => a.accur_time >= queryTime.startTime1);
                if (queryTime.startTime2 != null)
                {
                    queryTime.startTime2 = queryTime.startTime2.ToDate().AddDays(1);
                    qable.Where(a => a.accur_time < queryTime.startTime2);
                }

                if (!string.IsNullOrEmpty(distributor_name))
                    qable.Where<daoben_distributor_info>((a, b) => b.name.Contains(distributor_name));
                string listStr = qable
                        .OrderBy(pagination.sidx + " " + pagination.sord)
                        .GroupBy("a.distributor_id")
                        .Select("SQL_CALC_FOUND_ROWS SUM(a.refund) AS total_refund ,Count(*) AS count,a.distributor_name,a.distributor_id,b.address,a.company_linkname")
                        .ToJsonPage(pagination.page, pagination.rows);
                pagination.records = db.SqlQuery<int>("SELECT FOUND_ROWS() as count").FirstOrDefault();
                if (string.IsNullOrEmpty(listStr) || listStr == "[]")
                    return null;
                return listStr.ToJson();
            }
        }

        public object ExclusiveRefundList(Pagination pagination, QueryTime queryTime, string distributor_id)
        {
            var LoginInfo = OperatorProvider.Provider.GetCurrent();
            if (LoginInfo == null)
                return null;
            int records = 0;
            if (pagination == null)
                pagination = new Pagination();
            pagination.page = pagination.page > 0 ? pagination.page : 1;
            pagination.rows = pagination.rows > 0 ? pagination.rows : 20;
            pagination.sidx = string.IsNullOrEmpty(pagination.sidx) ? "accur_time" : pagination.sidx;
            pagination.sord = string.IsNullOrEmpty(pagination.sord) ? "desc" : pagination.sord;
            CompanyInfo myCompanyInfo = LoginInfo.companyInfo;
            PositionInfo myPositionInfo = LoginInfo.positionInfo;
            using (var db = SugarDao.GetInstance())
            {
                var qable = db.Queryable<daoben_sale_exclusive_detail>()
                    .JoinTable<daoben_sale_exclusive>((a, b) => a.main_id == b.id && b.status == 100)
                    .Where(a => a.distributor_id == distributor_id);

                if (queryTime.startTime1 != null)
                    qable.Where(a => a.accur_time >= queryTime.startTime1);
                if (queryTime.startTime2 != null)
                {
                    queryTime.startTime2 = queryTime.startTime2.ToDate().AddDays(1);
                    qable.Where(a => a.accur_time < queryTime.startTime2);
                }
                string listStr = qable
                        .Select("a.model,a.phone_sn,a.price_wholesale,a.refund,a.price_exclusive,a.accur_time")
                        .OrderBy(pagination.sidx + " " + pagination.sord)
                        .ToJsonPage(pagination.page, pagination.rows, ref records);
                pagination.records = records;
                if (string.IsNullOrEmpty(listStr) || listStr == "[]")
                    return null;
                return listStr.ToJson();
            }
        }

        public MemoryStream ExportExcelExclusiveRefund(Pagination pagination, QueryTime queryTime, int? company_id, string distributor_name)
        {
            var LoginInfo = OperatorProvider.Provider.GetCurrent();
            if (LoginInfo == null)
                return null;
            if (pagination == null)
                pagination = new Pagination();
            CompanyInfo myCompanyInfo = LoginInfo.companyInfo;
            PositionInfo myPositionInfo = LoginInfo.positionInfo;

            using (var db = SugarDao.GetInstance())
            {
                var qable = db.Queryable<daoben_sale_exclusive_detail>()
                    .JoinTable<daoben_distributor_info>((a, b) => a.distributor_id == b.id)
                    .JoinTable<daoben_sale_exclusive>((a, c) => a.main_id == c.id && c.status == 100);
                   
                if (myCompanyInfo.category == "事业部")
                    qable.Where(a => a.company_id_parent == myCompanyInfo.id);
                else if (myCompanyInfo.category == "分公司")
                    qable.Where(a => a.company_id == myCompanyInfo.id);
                if (company_id > 0)
                    qable.Where(a => a.company_id == company_id);

                if (queryTime.startTime1 != null)
                    qable.Where(a => a.accur_time >= queryTime.startTime1);
                if (queryTime.startTime2 != null)
                {
                    queryTime.startTime2 = queryTime.startTime2.ToDate().AddDays(1);
                    qable.Where(a => a.accur_time < queryTime.startTime2);
                }
                if (!string.IsNullOrEmpty(distributor_name))
                    qable.Where(a => a.distributor_name.Contains(distributor_name));

                string selStr = "a.distributor_name,a.model,a.phone_sn,a.refund,a.price_wholesale,a.price_exclusive,a.accur_time,a.company_linkname,b.address";
                var listDt = qable
                        .OrderBy("a.distributor_name,a.accur_time asc")
                        .Select(selStr).ToDataTable();
                int rowsCount = listDt.Rows.Count;

                string[] headerArr = new string[] { "经销商名称", "型号", "串码", "补差金额", "批发价", "包销价", "时间", "所属机构", "详细地址" };
                int[] colWidthArr = new int[] { 40, 25, 25, 20, 20, 20, 25, 25, 50 };

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
        public object GetTotalExclusiveRefund(QueryTime queryTime, int? company_id, string distributor_name)
        {
            OperatorModel LoginInfo = OperatorProvider.Provider.GetCurrent();
            if (LoginInfo == null)
                throw new Exception("用户登陆过期，请重新登录");
            CompanyInfo myCompanyInfo = LoginInfo.companyInfo;

            using (var db = SugarDao.GetInstance())
            {
                var qable = db.Queryable<daoben_sale_exclusive_detail>()
                             .JoinTable<daoben_sale_exclusive>((a, c) => a.main_id == c.id && c.status == 100);
                if (myCompanyInfo.category == "事业部")
                    qable.Where(a => a.company_id_parent == myCompanyInfo.id);
                else if (myCompanyInfo.category == "分公司")
                    qable.Where(a => a.company_id == myCompanyInfo.id);
                if (company_id > 0)
                    qable.Where(a => a.company_id == company_id);

                if (queryTime.startTime1 != null)
                    qable.Where(a => a.accur_time >= queryTime.startTime1);
                if (queryTime.startTime2 != null)
                {
                    queryTime.startTime2 = queryTime.startTime2.ToDate().AddDays(1);
                    qable.Where(a => a.accur_time < queryTime.startTime2);
                }

                if (!string.IsNullOrEmpty(distributor_name))
                    qable.Where(a => a.distributor_name.Contains(distributor_name));

                totalInfo totalInfo = qable.Select<totalInfo>("SUM(a.refund) AS total_amount, COUNT(*) AS total_count")
                        .SingleOrDefault();
                if (totalInfo == null)
                    return "系统错误：无法获取总数";
                return totalInfo;
            }
        }
        #endregion

        #region 计算
        /// <summary>
        /// 返利计算 type：1出库 2实销 3买断 4退库  5包销
        /// </summary>
        /// <param name="importId"></param>
        /// <param name="type">1出库 2实销 3买断 4退库</param>
        public void Rebate(string importId, int type)
        {
            using (var db = SugarDao.GetInstance())
            {
                try
                {
                    DateTime now = DateTime.Now;
                    QContainer qInfo = new QContainer();

                    if (type == 1)//出库
                    {
                        qInfo = db.Queryable<daoben_sale_outstorage>()
                               .Where(a => a.import_file_id == importId && a.check_status == 1)
                               .Select<QContainer>("MAX(accur_time) as endTime1, MIN(accur_time) as startTime1,company_id_parent as idInt")
                               .SingleOrDefault();
                    }
                    else if (type == 2)//实销
                    {
                        qInfo = db.Queryable<daoben_sale_salesinfo>()
                              .Where(a => a.import_file_id == importId && a.check_status == 1)
                              .Select<QContainer>("MAX(accur_time) as endTime1, MIN(accur_time) as startTime1,company_id_parent as idInt")
                              .SingleOrDefault();
                    }
                    else if (type == 3)//买断
                    {
                        qInfo = db.Queryable<daoben_sale_buyout_import_temp>()
                              .Where(a => a.import_file_id == importId && a.check_status == 1)
                              .Select<QContainer>("MAX(accur_time) as endTime1, MIN(accur_time) as startTime1,company_id_parent as idInt")
                              .SingleOrDefault();
                    }
                    else if (type == 4)//退库
                    {
                        qInfo = db.Queryable<daoben_product_sn>()
                              .JoinTable<daoben_sale_return>((a, b) => a.phone_sn == b.phone_sn)
                              .Where<daoben_sale_return>((a, b) => b.import_file_id == importId && a.status == -101)
                              .Select<QContainer>("MAX(a.outstorage_time) as endTime1, MIN(a.outstorage_time) as startTime1,b.company_id_parent as idInt")
                              .SingleOrDefault();   // TODO 同一串码多次退库的处理
                    }
                    else if (type == 5)//包销
                    {
                        qInfo = db.Queryable<daoben_sale_exclusive_detail>()
                             .Where(a => a.main_id == importId && a.check_status == 1)
                             .Select<QContainer>("MAX(accur_time) as endTime1, MIN(accur_time) as startTime1,company_id_parent as idInt")
                             .SingleOrDefault();
                    }

                    //找出时间有交集的返利活动：达量、主推、形象
                    DateTime startDate = qInfo.startTime1.ToDate();
                    List<daoben_distributor_attaining> attainingList = db.Queryable<daoben_distributor_attaining>()
                                .Where(a => a.start_date <= qInfo.endTime1 && a.end_date >= startDate && a.activity_status > 0
                                && a.company_id_parent == qInfo.idInt).ToList();
                    List<daoben_distributor_recommendation> recomList = db.Queryable<daoben_distributor_recommendation>()
                                .Where(a => a.start_date <= qInfo.endTime1 && a.end_date >= startDate && a.activity_status > 0
                                && a.company_id_parent == qInfo.idInt).ToList();
                    List<daoben_distributor_image> imageList = db.Queryable<daoben_distributor_image>()
                                .Where(a => a.start_date <= qInfo.endTime1 && a.end_date >= startDate && a.activity_status > 0
                                && a.company_id_parent == qInfo.idInt && a.target_mode == 2).ToList();

                    //找出时间有交集的活动管理 达量 主推 排名 业务考核
                    List<daoben_activity_attaining> activityAttaingList = db.Queryable<daoben_activity_attaining>()
                                .Where(a => a.start_date <= qInfo.endTime1 && a.end_date >= startDate && a.activity_status > 0
                                && a.company_id_parent == qInfo.idInt).ToList();
                    List<daoben_activity_recommendation> activityRecomList = db.Queryable<daoben_activity_recommendation>()
                                .Where(a => a.start_date <= qInfo.endTime1 && a.end_date >= startDate && a.activity_status > 0
                                && a.company_id_parent == qInfo.idInt).ToList();
                    List<daoben_activity_ranking> activityRankList = db.Queryable<daoben_activity_ranking>()
                                .Where(a => a.start_date <= qInfo.endTime1 && a.end_date >= startDate && a.activity_status > 0
                                && a.company_id_parent == qInfo.idInt).ToList();

                    List<daoben_activity_pk> activityPKList = db.Queryable<daoben_activity_pk>()
                                .Where(a => a.start_date <= qInfo.endTime1 && a.end_date >= startDate && a.activity_status > 0
                                && a.company_id_parent == qInfo.idInt).ToList();

                    List<daoben_activity_sales_perf> activityPerfList = db.Queryable<daoben_activity_sales_perf>()
                                .Where(a => a.start_date <= qInfo.endTime1 && a.end_date >= startDate && a.activity_status > 0
                                && a.company_id_parent == qInfo.idInt).ToList();

                    //分别对上述活动进行统计
                    //达量
                    new AttainingApp().Statistics(db, attainingList);

                    //主推
                    string disRecomSql = DisRecomRebate(db, recomList);

                    //形象返利
                    List<daoben_distributor_image_res> imageResList = DisImageRebate(db, imageList, qInfo);

                    //活动管理 达量活动
                    string empAttainingSql = EmpAttainingReward(db, activityAttaingList);

                    //活动管理 主推活动
                    string empRecomSql = EmpRecomReward(db, activityRecomList);

                    //活动管理 排名活动
                    string empRankSql = EmpRankReward(db, activityRankList);

                    //活动管理 PK活动
                    new PKApp().Statistics(db, activityPKList);

                    //活动管理 业务考核
                    new SalesPerformanceApp().Statistics(db, activityPerfList);

                    db.CommandTimeOut = 30;
                    db.BeginTran();
                    if (!string.IsNullOrEmpty(disRecomSql))
                        db.SqlQuery<int>(disRecomSql);
                    if (imageResList != null && imageResList.Count > 0)
                    {
                        if (imageResList.Exists(a => a.id == 0))
                            db.InsertRange(imageResList.Where(a => a.id == 0).ToList());
                        if (imageResList.Exists(a => a.id > 0))
                        {
                            StringBuilder sqlUpSb1 = new StringBuilder("insert into daoben_distributor_image_res (`id`,`total_sale_count`,");
                            sqlUpSb1.Append("`total_sale_amount`, `rebate`, `start_date`, `end_date`) values ");
                            foreach (var a in imageResList)
                            {
                                if (a.id > 0)
                                {
                                    sqlUpSb1.AppendFormat("({0},{1},{2},{3},'{4}','{5}'),", a.id, a.total_sale_count, a.total_sale_amount,
                                        a.rebate, a.start_date, a.end_date);
                                }
                            }
                            sqlUpSb1.Remove(sqlUpSb1.Length - 1, 1); // 最后一个逗号
                            sqlUpSb1.Append(" on duplicate key update total_sale_count=values(total_sale_count),total_sale_amount=values(total_sale_amount),");
                            sqlUpSb1.Append(" rebate=values(rebate),start_date=values(start_date),end_date=values(end_date);");
                            db.SqlQuery<int>(sqlUpSb1.ToString());
                        }
                    }
                    if (!string.IsNullOrEmpty(empAttainingSql))
                        db.SqlQuery<int>(empAttainingSql);
                    if (!string.IsNullOrEmpty(empRecomSql))
                        db.SqlQuery<int>(empRecomSql);
                    if (!string.IsNullOrEmpty(empRankSql))
                        db.SqlQuery<int>(empRankSql);
                    db.CommitTran();
                }
                catch (Exception ex)
                {
                    db.RollbackTran();
                    ExceptionApp.WriteLog("SettlementApp(Rebate)：" + ex.Message);
                }
            }

        }

        // TODO delete
        public string DisAttainingRebate(SqlSugarClient db, List<daoben_distributor_attaining> attainingList)
        {
            return null;
#if false
            // todo update
            List<daoben_distributor_attaining_distributor> attainingDistriList = new List<daoben_distributor_attaining_distributor>();
            foreach (var attaining in attainingList)
            {
            #region 达量返利 统计                   

                DateTime startDate = attaining.start_date.ToDate();
                DateTime endDate = attaining.end_date.ToDate().AddDays(1);
                List<daoben_distributor_attaining_distributor> distributorList = db.Queryable<daoben_distributor_attaining_distributor>().Where(a => a.main_id == attaining.id).ToList();
                List<daoben_product_sn> snList = new List<daoben_product_sn>();
                //是否指定机型，实销or下货
                if (attaining.product_scope == 2 && attaining.target_content == 2)
                {
                    snList = db.Queryable<daoben_product_sn>()
                            .JoinTable<daoben_distributor_attaining_distributor>((a, b) => a.out_distributor_id == b.distributor_id)
                            .JoinTable<daoben_distributor_attaining_product>((a, c) => a.model == c.model && a.color == c.color)
                            .Where<daoben_distributor_attaining_distributor, daoben_distributor_attaining_product>((a, b, c) => b.main_id == attaining.id && a.status > 0 && c.main_id == attaining.id && a.outstorage_time >= startDate && a.outstorage_time < endDate)
                            .ToList();
                }
                else if (attaining.product_scope == 2 && attaining.target_content == 1)
                {
                    snList = db.Queryable<daoben_product_sn>()
                            .JoinTable<daoben_distributor_attaining_distributor>((a, b) => a.sale_distributor_id == b.distributor_id)
                            .JoinTable<daoben_distributor_attaining_product>((a, c) => a.model == c.model && a.color == c.color)
                            .Where<daoben_distributor_attaining_distributor, daoben_distributor_attaining_product>((a, b, c) =>
                            b.main_id == attaining.id && a.status > 0 && c.main_id == attaining.id && a.sale_time >= startDate && a.sale_time < endDate)
                            .ToList();
                }
                else if (attaining.product_scope == 1 && attaining.target_content == 1)
                {
                    snList = db.Queryable<daoben_product_sn>()
                            .JoinTable<daoben_distributor_attaining_distributor>((a, b) => a.sale_distributor_id == b.distributor_id)
                            .Where<daoben_distributor_attaining_distributor>((a, b) => b.main_id == attaining.id && a.status > 0 && a.sale_time >= startDate && a.sale_time < endDate)
                            .ToList();
                }
                else if (attaining.product_scope == 1 && attaining.target_content == 2)
                {
                    snList = db.Queryable<daoben_product_sn>()
                            .JoinTable<daoben_distributor_attaining_distributor>((a, b) => a.out_distributor_id == b.distributor_id)
                            .Where<daoben_distributor_attaining_distributor>((a, b) => b.main_id == attaining.id && a.status > 0 && a.outstorage_time >= startDate && a.outstorage_time < endDate)
                            .ToList();
                }

                List<daoben_distributor_attaining_product> productList = db.Queryable<daoben_distributor_attaining_product>()
                        .Where(a => a.main_id == attaining.id).ToList();
                List<daoben_distributor_attaining_rebate> rebateList = db.Queryable<daoben_distributor_attaining_rebate>()
                        .Where(a => a.main_id == attaining.id).ToList();
                List<daoben_distributor_attaining_product_rebate> proRebateList = db.Queryable<daoben_distributor_attaining_product_rebate>()
                        .JoinTable<daoben_distributor_attaining_rebate>((a, b) => a.rebate_id == b.id)
                        .Where<daoben_distributor_attaining_rebate>((a, b) => b.main_id == attaining.id)
                        .Select("a.*")
                        .ToList();
                //大家都一起的
                int all_distri_count = snList.Count();
                decimal all_ratio = 0;
                if (attaining.target_sale == 1)
                    all_ratio = attaining.activity_target == 0 ? 0 : (100 * all_distri_count) / (attaining.activity_target);
                if (attaining.target_sale != 1)
                {
                    if (all_distri_count < attaining.activity_target)
                        return null;
                }

                foreach (var i in distributorList)
                {
                    i.total_rebate = 0;
                    i.total_amount = 0;
                    i.total_count = 0;
                    i.total_normal_count = 0;
                    i.total_ratio = all_ratio;//初始化，重新计算 大家都一起的
                    daoben_distributor_attaining_rebate rebateModel = new daoben_distributor_attaining_rebate();
                    List<daoben_product_sn> distriPhoneList = new List<daoben_product_sn>();
                    //确定规则
                    if (attaining.target_content == 1)//实销
                    {
                        distriPhoneList = snList.Where(t => t.sale_distributor_id == i.distributor_id).ToList();
                        if (attaining.target_sale == 1)
                        {
                            if (attaining.activity_target == 0)
                                i.total_ratio = 0;
                            else
                            {
                                i.total_ratio = distriPhoneList.Count() * 100 / attaining.activity_target;
                            }
                            //rebateModel = rebateList.Where(t => i.total_ratio >= t.target_min).OrderByDescending(t => t.target_min).FirstOrDefault();
                            rebateModel = rebateList.Where(t => all_ratio >= t.target_min).OrderByDescending(t => t.target_min).FirstOrDefault();//大家都一起的
                        }
                        else if (attaining.target_sale == 2)
                        {
                            //rebateModel = rebateList.Where(t => distriPhoneList.Count() >= t.target_min).OrderByDescending(t => t.target_min).FirstOrDefault();
                            rebateModel = rebateList.Where(t => all_distri_count >= t.target_min).OrderByDescending(t => t.target_min).FirstOrDefault();//大家都一起的
                        }
                        i.total_count = distriPhoneList.Count();
                        //经销商达量 包销、买断 算量不算钱
                        if (attaining.money_included == 0)
                            distriPhoneList = distriPhoneList.Where(t => (t.sale_type == 1) && t.special_offer == false && t.status < 10).ToList();
                        else if (attaining.money_included == 1)
                            distriPhoneList = distriPhoneList.Where(t => (t.sale_type == 1) && t.status < 10).ToList();
                        i.total_normal_count = distriPhoneList.Count();
                        i.total_amount = distriPhoneList.Sum(t => t.price_retail);
                        //if (attaining.target_sale != 1) //大家都一起的
                        //{
                        //    if (i.total_count < attaining.activity_target)
                        //        continue;
                        //}
                    }
                    else //出库
                    {
                        distriPhoneList = snList.Where(t => t.out_distributor_id == i.distributor_id).ToList();
                        if (attaining.target_sale == 1)
                        {
                            if (attaining.activity_target == 0)
                                i.total_ratio = 0;
                            else
                            {
                                i.total_ratio = distriPhoneList.Count() * 100 / attaining.activity_target;
                            }
                            //rebateModel = rebateList.Where(t => i.total_ratio >= t.target_min).OrderByDescending(t => t.target_min).FirstOrDefault();
                            rebateModel = rebateList.Where(t => all_ratio >= t.target_min).OrderByDescending(t => t.target_min).FirstOrDefault();//大家都一起的
                        }
                        else if (attaining.target_sale == 2)
                        {
                            //rebateModel = rebateList.Where(t => distriPhoneList.Count() >= t.target_min).OrderByDescending(t => t.target_min).FirstOrDefault();
                            rebateModel = rebateList.Where(t => all_distri_count >= t.target_min).OrderByDescending(t => t.target_min).FirstOrDefault();//大家都一起的
                        }
                        i.total_count = distriPhoneList.Count();
                        //经销商达量 包销、买断 算量不算钱
                        if (attaining.money_included == 0)
                            distriPhoneList = distriPhoneList.Where(t => (t.sale_type == 1) && t.special_offer == false && t.status < 10).ToList();
                        else if (attaining.money_included == 1)
                            distriPhoneList = distriPhoneList.Where(t => (t.sale_type == 1) && t.status < 10).ToList();
                        i.total_normal_count = distriPhoneList.Count();
                        i.total_amount = distriPhoneList.Sum(t => t.price_wholesale);
                        //if (attaining.target_sale != 1) //大家都一起的
                        //{
                        //    if (i.total_count < attaining.activity_target)
                        //        continue;
                        //}
                    }
                    if (attaining.target_mode == 3)
                    {
                        foreach (var a in distriPhoneList)
                        {
                            daoben_distributor_attaining_product_rebate proRebateModel = new daoben_distributor_attaining_product_rebate();
                            if (attaining.target_content == 1)
                                proRebateModel = proRebateList.Where(t => t.rebate_id == rebateModel.id && a.price_retail >= t.target_min && a.sale_time >= t.start_date)
                                            .OrderByDescending(t => t.start_date).ThenByDescending(t => t.target_min).FirstOrDefault();
                            else
                                proRebateModel = proRebateList.Where(t => t.rebate_id == rebateModel.id && a.price_retail >= t.target_min && a.outstorage_time >= t.start_date)
                                            .OrderByDescending(t => t.start_date).ThenByDescending(t => t.target_min).FirstOrDefault();
                            if (proRebateModel == null)
                                continue;
                            if (attaining.rebate_mode == 1)
                                i.total_rebate += proRebateModel.rebate;
                            else if (attaining.rebate_mode == 2)
                                i.total_rebate += a.price_wholesale * proRebateModel.rebate / 100;
                            else if (attaining.rebate_mode == 3)
                                i.total_rebate += a.price_retail * proRebateModel.rebate / 100;
                        }
                    }
                    else if (attaining.target_mode == 5)
                    {
                        foreach (var a in distriPhoneList)
                        {
                            daoben_distributor_attaining_product_rebate proRebateModel = new daoben_distributor_attaining_product_rebate();
                            if (attaining.target_content == 1)
                                proRebateModel = proRebateList.Where(t => t.rebate_id == rebateModel.id && a.price_wholesale >= t.target_min && a.sale_time >= t.start_date)
                                            .OrderByDescending(t => t.start_date).ThenByDescending(t => t.target_min).FirstOrDefault();
                            else
                                proRebateModel = proRebateList.Where(t => t.rebate_id == rebateModel.id && a.price_wholesale >= t.target_min && a.outstorage_time >= t.start_date)
                                            .OrderByDescending(t => t.start_date).ThenByDescending(t => t.target_min).FirstOrDefault();
                            if (proRebateModel == null)
                                continue;
                            if (attaining.rebate_mode == 1)
                                i.total_rebate += proRebateModel.rebate;
                            else if (attaining.rebate_mode == 2)
                                i.total_rebate += a.price_wholesale * proRebateModel.rebate / 100;
                            else if (attaining.rebate_mode == 3)
                                i.total_rebate += a.price_retail * proRebateModel.rebate / 100;
                        }
                    }
                    else if (attaining.target_mode == 4)
                    {
                        foreach (var a in distriPhoneList)
                        {
                            daoben_distributor_attaining_product_rebate proRebateModel = new daoben_distributor_attaining_product_rebate();
                            if (attaining.target_content == 1)
                                proRebateModel = proRebateList.Where(t => t.rebate_id == rebateModel.id && t.model == a.model && t.color == a.color && a.sale_time >= t.start_date)
                                            .OrderByDescending(t => t.start_date).ThenByDescending(t => t.target_min).FirstOrDefault();
                            else
                                proRebateModel = proRebateList.Where(t => t.rebate_id == rebateModel.id && t.model == a.model && t.color == a.color && a.outstorage_time >= t.start_date)
                                            .OrderByDescending(t => t.start_date).ThenByDescending(t => t.target_min).FirstOrDefault();
                            if (proRebateModel == null)
                                continue;
                            if (attaining.rebate_mode == 1)
                                i.total_rebate += proRebateModel.rebate;
                            else if (attaining.rebate_mode == 2)
                                i.total_rebate += a.price_wholesale * proRebateModel.rebate / 100;
                            else if (attaining.rebate_mode == 3)
                                i.total_rebate += a.price_retail * proRebateModel.rebate / 100;
                        }
                    }
                    else if (attaining.target_mode == 6)
                    {
                        daoben_distributor_attaining_product_rebate proRebateModel = proRebateList.Where(t => t.rebate_id == rebateModel.id).OrderByDescending(t => t.start_date).ThenByDescending(t => t.target_min).FirstOrDefault();
                        //todo delete 1.15 20:40 此时 proRebateModel.rebate=0 前端在为0是没有传值过来
                        if (proRebateModel.rebate == 0)
                        {
                            if (rebateModel.rebate != 0)
                                proRebateModel.rebate = rebateModel.rebate;//rebateModel.rebate 已弃用的字段
                        }
                        //todo delete 1.15 20:40
                        if (attaining.rebate_mode == 1)
                            i.total_rebate = distriPhoneList.Count() * proRebateModel.rebate;
                        else if (attaining.rebate_mode == 4)
                            i.total_rebate = proRebateModel.rebate;
                        else if (attaining.rebate_mode == 2)
                            foreach (var a in distriPhoneList)
                            {
                                i.total_rebate += a.price_wholesale * proRebateModel.rebate / 100;
                            }
                        else if (attaining.rebate_mode == 3)
                            foreach (var a in distriPhoneList)
                            {
                                i.total_rebate += a.price_retail * proRebateModel.rebate / 100;
                            }
                    }
                }
            #endregion
                attainingDistriList.AddRange(distributorList);
            }
            if (attainingDistriList.Count() <= 0)
                return null;
            StringBuilder sqlUpSb1 = new StringBuilder("insert into daoben_distributor_attaining_distributor (`id`,`total_count`, `total_normal_count`, `total_amount`, `total_ratio`, `total_rebate`) values ");
            foreach (var a in attainingDistriList)
            {
                sqlUpSb1.AppendFormat("({0},{1},{2},{3},{4},{5}),", a.id, a.total_count, a.total_normal_count, a.total_amount, a.total_ratio, a.total_rebate);
            }
            sqlUpSb1.Remove(sqlUpSb1.Length - 1, 1); // 最后一个逗号
            sqlUpSb1.Append(" on duplicate key update total_count=values(total_count),total_normal_count=values(total_normal_count),total_amount=values(total_amount),total_ratio=values(total_ratio),total_rebate=values(total_rebate);");
            return sqlUpSb1.ToString();
#endif
        }
        public string DisRecomRebate(SqlSugarClient db, List<daoben_distributor_recommendation> recomList)
        {
            //主推
            List<daoben_distributor_recommendation_distributor> recomDistriList = new List<daoben_distributor_recommendation_distributor>();
            foreach (var recom in recomList)
            {
                #region 主推返利 统计                   

                DateTime startDate = recom.start_date.ToDate();
                DateTime endDate = recom.end_date.ToDate().AddDays(1);
                List<daoben_distributor_recommendation_distributor> distributorList = db.Queryable<daoben_distributor_recommendation_distributor>().Where(a => a.main_id == recom.id).ToList();
                List<daoben_product_sn> snList = new List<daoben_product_sn>();
                //是否指定机型，实销or下货
                if (recom.product_scope == 2 && recom.target_content == 2)
                {
                    snList = db.Queryable<daoben_product_sn>()
                    .JoinTable<daoben_distributor_recommendation_distributor>((a, b) => a.out_distributor_id == b.distributor_id)
                    .JoinTable<daoben_distributor_recommendation_product>((a, c) => a.model == c.model && a.color == c.color)
                    .Where<daoben_distributor_recommendation_distributor, daoben_distributor_recommendation_distributor>((a, b, c) => b.main_id == recom.id && a.status > 0 && c.main_id == recom.id && a.outstorage_time >= startDate && a.outstorage_time < endDate)
                    .ToList();
                }
                else if (recom.product_scope == 2 && recom.target_content == 1)
                {
                    snList = db.Queryable<daoben_product_sn>()
                            .JoinTable<daoben_distributor_recommendation_distributor>((a, b) => a.sale_distributor_id == b.distributor_id)
                            .JoinTable<daoben_distributor_recommendation_product>((a, c) => a.model == c.model && a.color == c.color)
                            .Where<daoben_distributor_recommendation_distributor, daoben_distributor_recommendation_product>((a, b, c) =>
                            b.main_id == recom.id && a.status > 0 && c.main_id == recom.id && a.sale_time >= startDate && a.sale_time < endDate)
                            .ToList();
                }
                else if (recom.product_scope == 1 && recom.target_content == 1)
                {
                    snList = db.Queryable<daoben_product_sn>()
                            .JoinTable<daoben_distributor_recommendation_distributor>((a, b) => a.sale_distributor_id == b.distributor_id)
                            .Where<daoben_distributor_recommendation_distributor>((a, b) => b.main_id == recom.id && a.status > 0 && a.sale_time >= startDate && a.sale_time < endDate)
                            .ToList();
                }
                else if (recom.product_scope == 1 && recom.target_content == 2)
                {
                    snList = db.Queryable<daoben_product_sn>()
                            .JoinTable<daoben_distributor_recommendation_distributor>((a, b) => a.out_distributor_id == b.distributor_id)
                            .Where<daoben_distributor_recommendation_distributor>((a, b) => b.main_id == recom.id && a.status > 0 && a.outstorage_time >= startDate && a.outstorage_time < endDate)
                            .ToList();
                }

                List<daoben_distributor_recommendation_product> productList = db.Queryable<daoben_distributor_recommendation_product>()
                        .Where(a => a.main_id == recom.id).ToList();
                List<daoben_distributor_recommendation_rebate> rebateList = db.Queryable<daoben_distributor_recommendation_rebate>()
                        .Where(a => a.main_id == recom.id).ToList();

                //大家都一起的
                int all_distri_count = snList.Count();
                decimal all_ratio = 0;
                if (recom.target_mode == 1)
                    all_ratio = recom.activity_target == 0 ? 100 : (100 * all_distri_count) / (recom.activity_target);


                foreach (var i in distributorList)
                {
                    i.total_rebate = 0;
                    i.total_amount = 0;
                    i.total_count = 0;
                    i.total_normal_count = 0;
                    i.total_ratio = 0;//初始化，重新计算                    
                }

                if (recom.target_mode == 1)//完成率
                {
                    if (recom.activity_target <= 0)
                        continue;
                    foreach (var i in distributorList)
                    {
                        List<daoben_product_sn> distriSnlist = new List<daoben_product_sn>();
                        if (recom.target_content == 2)
                            distriSnlist = snList.Where(a => a.out_distributor_id == i.distributor_id).ToList();
                        else
                            distriSnlist = snList.Where(a => a.sale_distributor_id == i.distributor_id).ToList();
                        i.total_count = distributorList.Count();
                        //i.total_ratio = (i.total_count.ToDecimal() * 100) / recom.activity_target;//保留两位小数
                        i.total_ratio = all_ratio;//大家都一起的
                        //经销商主推  买断 算量不算钱
                        daoben_distributor_recommendation_rebate rebateModel = rebateList.Where(b => i.total_ratio >= b.target_min).OrderByDescending(t => t.target_min).First();
                        distriSnlist = distriSnlist.Where(y => (y.sale_type == 1 || y.sale_type == 3) && y.status < 10 && !y.special_offer).ToList();

                        i.total_normal_count = distriSnlist.Count();

                        if (recom.target_content == 1)
                            i.total_amount = distriSnlist.Sum(t => t.price_retail);
                        else
                            i.total_amount = distriSnlist.Sum(t => t.price_wholesale);
                        if (recom.rebate_mode == 1)
                            i.total_rebate = i.total_normal_count * rebateModel.rebate;
                        else if (recom.rebate_mode == 2)
                        {
                            foreach (var a in distriSnlist)
                            {
                                i.total_rebate = i.total_rebate + a.price_wholesale * rebateModel.rebate / 100;
                            }
                        }
                        else if (recom.rebate_mode == 3)
                        {
                            foreach (var a in distriSnlist)
                            {
                                i.total_rebate = i.total_rebate + a.price_retail * rebateModel.rebate / 100;
                            }
                        }
                        else if (recom.rebate_mode == 4)
                            i.total_rebate = rebateModel.rebate;
                    };
                }
                else if (recom.target_mode == 2)//按照台数
                {
                    foreach (var i in distributorList)
                    {
                        List<daoben_product_sn> distriSnlist = new List<daoben_product_sn>();
                        if (recom.target_content == 2)
                            distriSnlist = snList.Where(a => a.out_distributor_id == i.distributor_id).ToList();
                        else
                            distriSnlist = snList.Where(a => a.sale_distributor_id == i.distributor_id).ToList();
                        i.total_count = distriSnlist.Count();
                        //daoben_distributor_recommendation_rebate rebateModel = rebateList.Where(b => i.total_count >= b.target_min).OrderByDescending(t => t.target_min).First();
                        //distriSnlist = distriSnlist.Where(y => (y.sale_type == 1 || y.sale_type == 3) && y.status < 10 && !y.special_offer).ToList();
                        daoben_distributor_recommendation_rebate rebateModel = rebateList.Where(b => all_distri_count >= b.target_min).OrderByDescending(t => t.target_min).First();
                        distriSnlist = distriSnlist.Where(y => (y.sale_type == 1 || y.sale_type == 3) && y.status < 10 && !y.special_offer).ToList();//大家都一起的
                        i.total_normal_count = distriSnlist.Count();
                        if (recom.target_content == 1)
                            i.total_amount = distriSnlist.Sum(t => t.price_retail);
                        else
                            i.total_amount = distriSnlist.Sum(t => t.price_wholesale);
                        if (recom.rebate_mode == 1)
                            i.total_rebate = i.total_normal_count * rebateModel.rebate;
                        else if (recom.rebate_mode == 2)
                        {
                            foreach (var a in distriSnlist)
                            {
                                i.total_rebate = i.total_rebate + a.price_wholesale * rebateModel.rebate / 100;
                            }
                        }
                        else if (recom.rebate_mode == 3)
                        {
                            foreach (var a in distriSnlist)
                            {
                                i.total_rebate = i.total_rebate + a.price_retail * rebateModel.rebate / 100;
                            }
                        }
                        else if (recom.rebate_mode == 4)
                            i.total_rebate = rebateModel.rebate;
                    };
                }
                else if (recom.target_mode == 3)//按零售价
                {
                    foreach (var i in distributorList)
                    {
                        List<daoben_product_sn> distriSnlist = new List<daoben_product_sn>();
                        if (recom.target_content == 2)
                            distriSnlist = snList.Where(a => a.out_distributor_id == i.distributor_id).ToList();
                        else
                            distriSnlist = snList.Where(a => a.sale_distributor_id == i.distributor_id).ToList();
                        i.total_count = distributorList.Count();
                        distriSnlist = distriSnlist.Where(y => (y.sale_type == 1 || y.sale_type == 3) && y.status < 10 && !y.special_offer).ToList();
                        i.total_normal_count = distriSnlist.Count();
                        if (recom.target_content == 1)
                            i.total_amount = distriSnlist.Sum(t => t.price_retail);
                        else
                            i.total_amount = distriSnlist.Sum(t => t.price_wholesale);
                        foreach (var j in distriSnlist)
                        {
                            daoben_distributor_recommendation_rebate rebateModel = rebateList.Where(b => j.price_retail >= b.target_min).OrderByDescending(t => t.target_min).First();
                            if (recom.rebate_mode == 1)
                                i.total_rebate = i.total_rebate + rebateModel.rebate;
                            else if (recom.rebate_mode == 2)
                                i.total_rebate = i.total_rebate + j.price_wholesale * rebateModel.rebate * (decimal)(0.01);
                            else if (recom.rebate_mode == 3)
                                i.total_rebate = i.total_rebate + j.price_retail * rebateModel.rebate * (decimal)(0.01);
                        }
                    }
                }
                else if (recom.target_mode == 5)//按批发价
                {
                    foreach (var i in distributorList)
                    {
                        List<daoben_product_sn> distriSnlist = new List<daoben_product_sn>();
                        if (recom.target_content == 2)
                            distriSnlist = snList.Where(a => a.out_distributor_id == i.distributor_id).ToList();
                        else
                            distriSnlist = snList.Where(a => a.sale_distributor_id == i.distributor_id).ToList();
                        i.total_count = distributorList.Count();
                        distriSnlist = distriSnlist.Where(y => (y.sale_type == 1 || y.sale_type == 3) && y.status < 10 && !y.special_offer).ToList();
                        i.total_normal_count = distriSnlist.Count();
                        if (recom.target_content == 1)
                            i.total_amount = distriSnlist.Sum(t => t.price_retail);
                        else
                            i.total_amount = distriSnlist.Sum(t => t.price_wholesale);
                        foreach (var j in distriSnlist)
                        {
                            daoben_distributor_recommendation_rebate rebateModel = rebateList.Where(b => j.price_wholesale >= b.target_min).OrderByDescending(t => t.target_min).First();
                            if (recom.rebate_mode == 1)
                                i.total_rebate = i.total_rebate + rebateModel.rebate;
                            else if (recom.rebate_mode == 2)
                                i.total_rebate = i.total_rebate + j.price_wholesale * rebateModel.rebate * (decimal)(0.01);
                            else if (recom.rebate_mode == 3)
                                i.total_rebate = i.total_rebate + j.price_retail * rebateModel.rebate * (decimal)(0.01);

                        }
                    }
                }
                else if (recom.target_mode == 4)//按型号
                {
                    foreach (var i in distributorList)
                    {
                        List<daoben_product_sn> distriSnlist = new List<daoben_product_sn>();
                        if (recom.target_content == 2)
                            distriSnlist = snList.Where(a => a.out_distributor_id == i.distributor_id).ToList();
                        else
                            distriSnlist = snList.Where(a => a.sale_distributor_id == i.distributor_id).ToList();
                        i.total_count = distributorList.Count();
                        i.total_normal_count = distriSnlist.Count();
                        if (recom.target_content == 1)
                            i.total_amount = distriSnlist.Sum(t => t.price_retail);
                        else
                            i.total_amount = distriSnlist.Sum(t => t.price_wholesale);
                        foreach (var j in distriSnlist)
                        {
                            daoben_distributor_recommendation_product rebateModel = productList.Where(a => a.model == j.model && a.color == j.color).SingleOrDefault();
                            if (rebateModel == null)//说明有不符合机型规定的product_sn进入到统计 “全部机型”productList为空不能按型号返利
                                continue;
                            if (recom.rebate_mode == 1)
                                i.total_rebate = i.total_rebate + rebateModel.rebate;
                            else if (recom.rebate_mode == 2)
                                i.total_rebate = i.total_rebate + j.price_wholesale * rebateModel.rebate * (decimal)(0.01);
                            else if (recom.rebate_mode == 3)
                                i.total_rebate = i.total_rebate + j.price_retail * rebateModel.rebate * (decimal)(0.01);
                        }
                    }
                }

                #endregion
                recomDistriList.AddRange(distributorList);
            }
            if (recomDistriList.Count() <= 0)
                return null;
            StringBuilder sqlUpSb2 = new StringBuilder("insert into daoben_distributor_recommendation_distributor (`id`, `total_count`,`total_normal_count`,`total_amount`, `total_ratio`, `total_rebate`) values ");
            foreach (var a in recomDistriList)
            {
                sqlUpSb2.AppendFormat("({0},{1},{2},{3},{4},{5}),", a.id, a.total_count, a.total_normal_count, a.total_amount, a.total_ratio, a.total_rebate);
            }
            sqlUpSb2.Remove(sqlUpSb2.Length - 1, 1);
            sqlUpSb2.Append(" on duplicate key update total_count=values(total_count),total_normal_count=values(total_normal_count),total_amount=values(total_amount),total_ratio=values(total_ratio),total_rebate=values(total_rebate);");
            return sqlUpSb2.ToString();
        }
        public List<daoben_distributor_image_res> DisImageRebate(SqlSugarClient db, List<daoben_distributor_image> imageList, QContainer qInfo)
        {
            DateTime now = DateTime.Now;
            //形象返利
            List<daoben_distributor_image_res> resList = new List<daoben_distributor_image_res>();
            foreach (var image in imageList)
            {
                #region 按销量返利计算
                string whereStrF = null, whereStr = null; ;
                if (image.target_content == 1)
                    whereStrF = "(sale_distributor_id='" + image.distributor_id + "' and sale_time>='{0}' and sale_time<'{1}')";
                else
                    whereStrF = "(out_distributor_id='" + image.distributor_id + "' and outstorage_time>='{0}' and outstorage_time<'{1}')";
                if (image.pay_mode == 2)     // 按月发放
                {
                    List<daoben_distributor_image_res> origResList = db.Queryable<daoben_distributor_image_res>()
                            .Where(a => a.main_id == image.id).ToList();
                    DateTime endDate1 = image.end_date.ToDate().AddDays(1);
                    DateTime selEnd = image.start_date.ToDate();
                    selEnd = selEnd.AddDays(1 - selEnd.Day).AddMonths(1); // 按月查询的结束日
                    DateTime selStart = image.start_date.ToDate();  // 按月查询的起始日
                    while (true)
                    {
                        if (selStart >= qInfo.endTime1)
                            break;
                        if (selEnd >= qInfo.startTime1)  // else 该月无数据更新，继续下一个月
                        {
                            whereStr = string.Format(whereStrF, selStart, selEnd);
                            DateTime month = selStart.AddDays(1 - selStart.Day);
                            daoben_distributor_image_res origRes = origResList.Where(a => a.month == month).SingleOrDefault();
                            daoben_distributor_image_res resInfo = GetRebateInfo(db, whereStr, origRes, image, now);
                            if (resInfo != null)
                            {
                                resInfo.month = month;
                                resList.Add(resInfo);
                            }
                        }
                        if (selEnd >= endDate1)
                            break;
                        selStart = selEnd;
                        selEnd = selEnd.AddMonths(1) > endDate1 ? endDate1 : selEnd.AddMonths(1);
                    }
                }
                else    // 一次性发放
                {
                    daoben_distributor_image_res origRes = db.Queryable<daoben_distributor_image_res>()
                            .SingleOrDefault(a => a.main_id == image.id);
                    whereStr = string.Format(whereStrF, image.start_date, (image.end_date > now ? now : image.end_date.ToDate().AddDays(1)));
                    daoben_distributor_image_res resInfo = GetRebateInfo(db, whereStr, origRes, image, now);
                    if (resInfo != null)
                        resList.Add(resInfo);
                }
                #endregion
            }
            return resList;
        }
        private daoben_distributor_image_res GetRebateInfo(SqlSugarClient db, string whereStr,
                    daoben_distributor_image_res origRes, daoben_distributor_image imageInfo, DateTime now)
        {
            daoben_distributor_image_res resInfo = db.Queryable<daoben_product_sn>()
                            .Where(a => a.sale_type > -1).Where(whereStr)
                            .Select<daoben_distributor_image_res>("count(*) as total_sale_count, sum(price_wholesale) as total_sale_amount")
                            .SingleOrDefault();
            if (resInfo == null)
                return null;
            if (origRes == null)
            {
                resInfo.main_id = imageInfo.id;
                resInfo.start_date = imageInfo.start_date;
                resInfo.end_date = imageInfo.end_date > now ? now : imageInfo.end_date;
                resInfo.rebate = resInfo.total_sale_count >= imageInfo.activity_target ? imageInfo.rebate : 0;
                return resInfo;   // resInfo == 0 为区别点
            }
            else
            {
                origRes.total_sale_count = resInfo.total_sale_count;
                origRes.total_sale_amount = resInfo.total_sale_amount;
                origRes.end_date = imageInfo.end_date > now ? now : imageInfo.end_date;
                origRes.rebate = resInfo.total_sale_count >= imageInfo.activity_target ? imageInfo.rebate : 0;
                return origRes;
            }
        }

        public string EmpAttainingReward(SqlSugarClient db, List<daoben_activity_attaining> activityAttaingList)
        {
            //活动管理 达量活动
            List<daoben_activity_attaining_emp> attainingEmpList = new List<daoben_activity_attaining_emp>();
            foreach (var attaining in activityAttaingList)
            {
                #region 活动管理 达量活动
                DateTime startDate = attaining.start_date.ToDate();
                DateTime endDate = attaining.end_date.ToDate().AddDays(1);
                List<daoben_activity_attaining_emp> empList = db.Queryable<daoben_activity_attaining_emp>().Where(a => a.main_id == attaining.id).ToList();
                List<daoben_product_sn> snList = new List<daoben_product_sn>();
                List<daoben_activity_attaining_reward> rewardList = db.Queryable<daoben_activity_attaining_reward>()
                    .Where(a => a.main_id == attaining.id).ToList();
                List<daoben_activity_attaining_product_reward> proRewardList = db.Queryable<daoben_activity_attaining_product_reward>()
                    .JoinTable<daoben_activity_attaining_reward>((a, b) => a.reward_id == b.id)
                    .Where<daoben_activity_attaining_reward>((a, b) => b.main_id == attaining.id)
                    .Select("a.*")
                    .ToList();
                List<daoben_activity_attaining_product> productList = db.Queryable<daoben_activity_attaining_product>()
                    .Where(a => a.main_id == attaining.id).ToList();
                List<daoben_distributor_info> distriList = db.Queryable<daoben_distributor_info>()
                    .Where(a => a.company_id == attaining.company_id || a.company_id_parent == attaining.company_id).ToList();
                //是否指定机型，实销or下货 业务员，业务经理，导购员
                if (attaining.emp_category == 21)//业务经理
                {
                    if (attaining.product_scope > 1 && attaining.target_content == 2)
                    {
                        snList = db.Queryable<daoben_product_sn>()
                                    .JoinTable<daoben_activity_attaining_product>((a, b) => a.model == b.model && a.color == b.color)
                                    .JoinTable<daoben_activity_attaining_emp>((a, d) => a.out_sales_m_id == d.emp_id)
                                    .Where<daoben_activity_attaining_product, daoben_activity_attaining_emp>((a, b, d) => b.main_id == attaining.id && a.status > 0 &&
                                            d.main_id == attaining.id && a.outstorage_time >= startDate && a.outstorage_time < endDate)
                                    .Select("a.*").ToList();
                    }
                    else if (attaining.product_scope > 1 && attaining.target_content == 1)
                    {
                        snList = db.Queryable<daoben_product_sn>()
                                    .JoinTable<daoben_activity_attaining_product>((a, b) => a.model == b.model && a.color == b.color)
                                    .JoinTable<daoben_activity_attaining_emp>((a, d) => a.sales_m_id == d.emp_id)
                                    .Where<daoben_activity_attaining_product, daoben_activity_attaining_emp>((a, b, d) => b.main_id == attaining.id && a.status > 0 &&
                                            d.main_id == attaining.id && a.sale_time >= startDate && a.sale_time < endDate)
                                    .Select("a.*").ToList();
                    }
                    else if (attaining.product_scope == 1 && attaining.target_content == 1)
                    {
                        snList = db.Queryable<daoben_product_sn>()
                                    .JoinTable<daoben_activity_attaining_emp>((a, c) => c.emp_id == a.sales_m_id)
                                    .Where<daoben_activity_attaining_emp>((a, c) => c.main_id == attaining.id && a.status > 0
                                        && a.sale_time >= startDate && a.sale_time < endDate)
                                    .Select("a.*").ToList();
                    }
                    else if (attaining.product_scope == 1 && attaining.target_content == 2)
                    {
                        snList = db.Queryable<daoben_product_sn>()
                                    .JoinTable<daoben_activity_attaining_emp>((a, c) => c.emp_id == a.out_sales_m_id)
                                    .Where<daoben_activity_attaining_emp>((a, c) => c.main_id == attaining.id && a.status > 0
                                        && a.outstorage_time >= startDate && a.outstorage_time < endDate)
                                    .Select("a.*").ToList();
                    }
                }
                else if (attaining.emp_category == 20)//业务员
                {
                    if (attaining.product_scope > 1 && attaining.target_content == 2)
                    {
                        snList = db.Queryable<daoben_product_sn>()
                                    .JoinTable<daoben_activity_attaining_product>((a, b) => a.model == b.model && a.color == b.color)
                                    .JoinTable<daoben_activity_attaining_emp>((a, d) => a.out_sales_id == d.emp_id)
                                    .Where<daoben_activity_attaining_product, daoben_activity_attaining_emp>((a, b, d) => b.main_id == attaining.id && a.status > 0 &&
                                            d.main_id == attaining.id && a.outstorage_time >= startDate && a.outstorage_time < endDate)
                                    .Select("a.*").ToList();
                    }
                    else if (attaining.product_scope > 1 && attaining.target_content == 1)
                    {
                        snList = db.Queryable<daoben_product_sn>()
                                    .JoinTable<daoben_activity_attaining_product>((a, b) => a.model == b.model && a.color == b.color)
                                    .JoinTable<daoben_activity_attaining_emp>((a, d) => a.sales_id == d.emp_id)
                                    .Where<daoben_activity_attaining_product, daoben_activity_attaining_emp>((a, b, d) => b.main_id == attaining.id && a.status > 0 &&
                                            d.main_id == attaining.id && a.sale_time >= startDate && a.sale_time < endDate)
                                    .Select("a.*").ToList();
                    }
                    else if (attaining.product_scope == 1 && attaining.target_content == 1)
                    {
                        snList = db.Queryable<daoben_product_sn>()
                                    .JoinTable<daoben_activity_attaining_emp>((a, c) => c.emp_id == a.sales_id)
                                    .Where<daoben_activity_attaining_emp>((a, c) => c.main_id == attaining.id && a.status > 0
                                        && a.sale_time >= startDate && a.sale_time < endDate)
                                    .Select("a.*").ToList();
                    }
                    else if (attaining.product_scope == 1 && attaining.target_content == 2)
                    {
                        snList = db.Queryable<daoben_product_sn>()
                                    .JoinTable<daoben_activity_attaining_emp>((a, c) => c.emp_id == a.out_sales_id)
                                    .Where<daoben_activity_attaining_emp>((a, c) => c.main_id == attaining.id && a.status > 0
                                        && a.outstorage_time >= startDate && a.outstorage_time < endDate)
                                    .Select("a.*").ToList();
                    }
                }
                else if (attaining.emp_category == 3)//导购员
                {
                    if (attaining.product_scope > 1 && attaining.target_content == 2)
                    {
                        //snList = db.Queryable<daoben_product_sn>()
                        //            .JoinTable<daoben_activity_attaining_product>((a, b) => a.model == b.model && a.color == b.color)
                        //            .JoinTable<daoben_activity_attaining_emp>((a, c) => a.out_guide_id == c.emp_id)
                        //            .Where<daoben_activity_attaining_product, daoben_activity_attaining_emp>((a, b, c) => b.main_id == attaining.id && a.status > 0 &&
                        //                c.main_id == attaining.id && a.outstorage_time >= startDate && a.outstorage_time < endDate)
                        //            .Select("a.*").ToList();
                    }
                    else if (attaining.product_scope > 1 && attaining.target_content == 1)
                    {
                        snList = db.Queryable<daoben_product_sn>()
                                    .JoinTable<daoben_activity_attaining_product>((a, b) => a.model == b.model && a.color == b.color)
                                    .JoinTable<daoben_activity_attaining_emp>((a, c) => a.reporter_id == c.emp_id)
                                    .Where<daoben_activity_attaining_product, daoben_activity_attaining_emp>((a, b, c) => b.main_id == attaining.id && a.status > 0 &&
                                        c.main_id == attaining.id && a.sale_time >= startDate && a.sale_time < endDate)
                                    .Select("a.*").ToList();
                    }
                    else if (attaining.product_scope == 1 && attaining.target_content == 1)
                    {
                        snList = db.Queryable<daoben_product_sn>()
                                    .JoinTable<daoben_activity_attaining_emp>((a, b) => a.reporter_id == b.emp_id)
                                    .Where<daoben_activity_attaining_emp>((a, b) => b.main_id == attaining.id && a.status > 0
                                        && a.sale_time >= startDate && a.sale_time < endDate)
                                    .Select("a.*").ToList();
                    }
                    else if (attaining.product_scope == 1 && attaining.target_content == 2)
                    {
                        //snList = db.Queryable<daoben_product_sn>()
                        //            .JoinTable<daoben_activity_attaining_emp>((a, b) => a.out_guide_id == b.emp_id)
                        //            .Where<daoben_activity_attaining_emp>((a, b) => b.main_id == attaining.id && a.status > 0
                        //                && a.outstorage_time >= startDate && a.outstorage_time < endDate)
                        //            .Select("a.*").ToList();
                    }
                }
                foreach (var i in empList)
                {
                    i.total_reward = 0;
                    i.total_amount = 0;
                    i.total_count = 0;
                    i.total_ratio = 0;
                    i.total_normal_count = 0;//初始化，重新计算
                    daoben_activity_attaining_reward rewardModel = new daoben_activity_attaining_reward();
                    List<daoben_product_sn> empPhoneList = new List<daoben_product_sn>();
                    //确定规则
                    if (attaining.target_content == 1)//实销
                    {
                        if (attaining.emp_category == 21)
                        {
                            empPhoneList = snList.Where(a => i.emp_id == a.sales_m_id).ToList();
                        }
                        else if (attaining.emp_category == 20)
                        {
                            empPhoneList = snList.Where(a => i.emp_id == a.sales_id).ToList();
                        }
                        else if (attaining.emp_category == 3)
                        {
                            empPhoneList = snList.Where(a => a.reporter_id == i.emp_id).ToList();
                        }
                    }
                    else //出库
                    {
                        if (attaining.emp_category == 21)
                        {
                            empPhoneList = snList.Where(a => i.emp_id == a.out_sales_m_id).ToList();
                        }
                        else if (attaining.emp_category == 20)
                        {
                            empPhoneList = snList.Where(a => i.emp_id == a.out_sales_id).ToList();
                        }
                        else if (attaining.emp_category == 3)
                        {
                            empPhoneList = snList.Where(a => a.reporter_id == i.emp_id).ToList();
                        }
                    }
                    if (attaining.target_sale == 1)
                    {
                        if (attaining.activity_target == 0)
                            i.total_ratio = 100;
                        else
                        {
                            i.total_ratio = empPhoneList.Count() * 100 / attaining.activity_target;
                        }
                        rewardModel = rewardList.Where(t => i.total_ratio >= t.target_min).OrderByDescending(t => t.target_min).FirstOrDefault();
                    }
                    else if (attaining.target_sale == 2)
                    {
                        rewardModel = rewardList.Where(t => empPhoneList.Count() >= t.target_min).OrderByDescending(t => t.target_min).FirstOrDefault();
                    }
                    i.total_count = empPhoneList.Count();
                    //员工达量 买断 算量不算钱
                    if (attaining.money_included == 0)
                        empPhoneList = empPhoneList.Where(t => (t.sale_type == 1 || t.sale_type == 3) && t.special_offer == false && t.status < 10).ToList();
                    else if (attaining.money_included == 1)
                        empPhoneList = empPhoneList.Where(t => (t.sale_type == 1 || t.sale_type == 3) && t.status < 10).ToList();
                    i.total_normal_count = empPhoneList.Count();
                    i.total_amount = empPhoneList.Sum(t => t.price_retail);
                    if (attaining.target_sale != 1)
                    {
                        if (i.total_count < attaining.activity_target)
                            continue;
                    }
                    if (attaining.target_mode == 3)
                    {
                        foreach (var a in empPhoneList)
                        {
                            daoben_activity_attaining_product_reward proRewardModel = new daoben_activity_attaining_product_reward();
                            if (attaining.target_content == 1)
                                proRewardModel = proRewardList.Where(t => t.reward_id == rewardModel.id
                                            && a.price_retail >= t.target_min && a.sale_time >= t.start_date)
                                            .OrderByDescending(t => t.start_date).ThenByDescending(t => t.target_min)
                                            .FirstOrDefault();
                            else
                                proRewardModel = proRewardList.Where(t => t.reward_id == rewardModel.id
                                            && a.price_retail >= t.target_min && a.outstorage_time >= t.start_date)
                                            .OrderByDescending(t => t.start_date).ThenByDescending(t => t.target_min)
                                            .FirstOrDefault();
                            if (proRewardModel == null)
                                continue;
                            if (attaining.rebate_mode == 1)
                                i.total_reward += proRewardModel.reward;
                            else if (attaining.rebate_mode == 2)
                                i.total_reward += a.price_wholesale * proRewardModel.reward / 100;
                            else if (attaining.rebate_mode == 3)
                                i.total_reward += a.price_retail * proRewardModel.reward / 100;
                        }
                    }
                    else if (attaining.target_mode == 5)
                    {
                        foreach (var a in empPhoneList)
                        {
                            daoben_activity_attaining_product_reward proRewardModel = new daoben_activity_attaining_product_reward();
                            if (attaining.target_content == 1)
                                proRewardModel = proRewardList.Where(t => t.reward_id == rewardModel.id
                                            && a.price_wholesale >= t.target_min && a.sale_time >= t.start_date)
                                            .OrderByDescending(t => t.start_date).ThenByDescending(t => t.target_min)
                                            .FirstOrDefault();
                            else
                                proRewardModel = proRewardList.Where(t => t.reward_id == rewardModel.id
                                            && a.price_wholesale >= t.target_min && a.outstorage_time >= t.start_date)
                                            .OrderByDescending(t => t.start_date).ThenByDescending(t => t.target_min)
                                            .FirstOrDefault();
                            if (proRewardModel == null)
                                continue;
                            if (attaining.rebate_mode == 1)
                                i.total_reward += proRewardModel.reward;
                            else if (attaining.rebate_mode == 2)
                                i.total_reward += a.price_wholesale * proRewardModel.reward / 100;
                            else if (attaining.rebate_mode == 3)
                                i.total_reward += a.price_retail * proRewardModel.reward / 100;
                        }
                    }
                    else if (attaining.target_mode == 4)
                    {
                        foreach (var a in empPhoneList)
                        {
                            daoben_activity_attaining_product_reward proRewardModel = new daoben_activity_attaining_product_reward();
                            if (attaining.target_content == 1)
                                proRewardModel = proRewardList.Where(t => t.reward_id == rewardModel.id
                                            && t.model == a.model && t.color == a.color && a.sale_time >= t.start_date)
                                            .OrderByDescending(t => t.start_date).ThenByDescending(t => t.target_min)
                                            .FirstOrDefault();
                            else
                                proRewardModel = proRewardList.Where(t => t.reward_id == rewardModel.id
                                            && t.model == a.model && t.color == a.color && a.outstorage_time >= t.start_date)
                                            .OrderByDescending(t => t.start_date).ThenByDescending(t => t.target_min)
                                            .FirstOrDefault();
                            if (proRewardModel == null)
                                continue;
                            if (attaining.rebate_mode == 1)
                                i.total_reward += proRewardModel.reward;
                            else if (attaining.rebate_mode == 2)
                                i.total_reward += a.price_wholesale * proRewardModel.reward / 100;
                            else if (attaining.rebate_mode == 3)
                                i.total_reward += a.price_retail * proRewardModel.reward / 100;
                        }
                    }
                    else if (attaining.target_mode == 6)
                    {
                        daoben_activity_attaining_product_reward proRewardModel = proRewardList
                                    .Where(t => t.reward_id == rewardModel.id)
                                    .OrderByDescending(t => t.start_date).ThenByDescending(t => t.target_min)
                                    .FirstOrDefault();
                        if (attaining.rebate_mode == 1)
                            i.total_reward = empPhoneList.Count() * proRewardModel.reward;
                        else if (attaining.rebate_mode == 4)
                            i.total_reward = proRewardModel.reward;
                        else if (attaining.rebate_mode == 2)
                            foreach (var a in empPhoneList)
                            {
                                i.total_reward += a.price_wholesale * proRewardModel.reward / 100;
                            }
                        else if (attaining.rebate_mode == 3)
                            foreach (var a in empPhoneList)
                            {
                                i.total_reward += a.price_retail * proRewardModel.reward / 100;
                            }
                    }
                }

                #endregion
                attainingEmpList.AddRange(empList);
            }
            if (attainingEmpList.Count() <= 0)
                return null;
            //活动管理 达量活动
            StringBuilder sqlUpSb4 = new StringBuilder("insert into daoben_activity_attaining_emp (`id`, `total_count`, `total_amount`, `total_ratio`, `total_reward`, `total_normal_count`) values ");
            foreach (var a in attainingEmpList)
            {
                sqlUpSb4.AppendFormat("({0},{1},{2},{3},{4},{5}),", a.id, a.total_count, a.total_amount, a.total_ratio, a.total_reward, a.total_normal_count);
            }
            sqlUpSb4.Remove(sqlUpSb4.Length - 1, 1); // 最后一个逗号
            sqlUpSb4.Append("on duplicate key update total_count=values(total_count),total_amount=values(total_amount),total_ratio=values(total_ratio),total_reward=values(total_reward),total_normal_count=values(total_normal_count);");
            return sqlUpSb4.ToString();
        }
        public string EmpRecomReward(SqlSugarClient db, List<daoben_activity_recommendation> activityRecomList)
        {
            //活动管理 主推活动
            List<daoben_activity_recommendation_emp> recomEmpList = new List<daoben_activity_recommendation_emp>();
            foreach (var recom in activityRecomList)
            {
                #region 活动管理 主推活动
                //主推

                DateTime startDate = recom.start_date.ToDate();
                DateTime endDate = recom.end_date.ToDate().AddDays(1);
                List<daoben_activity_recommendation_emp> empList = db.Queryable<daoben_activity_recommendation_emp>().Where(a => a.main_id == recom.id).ToList();
                List<daoben_product_sn> snList = new List<daoben_product_sn>();
                List<daoben_activity_recommendation_reward> rewardList = db.Queryable<daoben_activity_recommendation_reward>()
                    .Where(a => a.main_id == recom.id).ToList();
                List<daoben_activity_recommendation_product> productList = db.Queryable<daoben_activity_recommendation_product>()
                    .Where(a => a.main_id == recom.id).ToList();
                List<daoben_distributor_info> distriList = db.Queryable<daoben_distributor_info>()
                    .Where(a => a.company_id == recom.company_id || a.company_id_parent == recom.company_id).ToList();
                //是否指定机型，实销or下货 业务员，业务经理，导购员
                if (recom.emp_category == 21)//业务经理
                {
                    if (recom.product_scope > 1 && recom.target_content == 2)
                    {
                        snList = db.Queryable<daoben_product_sn>()
                                    .JoinTable<daoben_activity_recommendation_product>((a, b) => a.model == b.model && a.color == b.color)
                                    .JoinTable<daoben_activity_recommendation_emp>((a, d) => a.out_sales_m_id == d.emp_id)
                                    .Where<daoben_activity_recommendation_product, daoben_activity_recommendation_emp>((a, b, d) => b.main_id == recom.id && a.status > 0 &&
                                        d.main_id == recom.id && a.outstorage_time >= startDate && a.outstorage_time < endDate)
                                    .Select("a.*").ToList();
                    }
                    else if (recom.product_scope > 1 && recom.target_content == 1)
                    {
                        snList = db.Queryable<daoben_product_sn>()
                                    .JoinTable<daoben_activity_recommendation_product>((a, b) => a.model == b.model && a.color == b.color)
                                    .JoinTable<daoben_activity_recommendation_emp>((a, d) => a.sales_m_id == d.emp_id)
                                    .Where<daoben_activity_recommendation_product, daoben_activity_recommendation_emp>((a, b, d) => b.main_id == recom.id && a.status > 0 &&
                                        d.main_id == recom.id && a.sale_time >= startDate && a.sale_time < endDate)
                                    .Select("a.*").ToList();
                    }
                    else if (recom.product_scope == 1 && recom.target_content == 1)
                    {
                        snList = db.Queryable<daoben_product_sn>()
                                    .JoinTable<daoben_activity_recommendation_emp>((a, c) => c.emp_id == a.sales_m_id)
                                    .Where<daoben_activity_recommendation_emp>((a, c) => c.main_id == recom.id && a.status > 0
                                        && a.sale_time >= startDate && a.sale_time < endDate)
                                    .Select("a.*").ToList();
                    }
                    else if (recom.product_scope == 1 && recom.target_content == 2)
                    {
                        snList = db.Queryable<daoben_product_sn>()
                                    .JoinTable<daoben_activity_recommendation_emp>((a, c) => c.emp_id == a.out_sales_m_id)
                                    .Where<daoben_activity_recommendation_emp>((a, c) => c.main_id == recom.id && a.status > 0
                                        && a.outstorage_time >= startDate && a.outstorage_time < endDate)
                                    .Select("a.*").ToList();
                    }
                }
                else if (recom.emp_category == 20)//业务员
                {
                    if (recom.product_scope > 1 && recom.target_content == 2)
                    {
                        snList = db.Queryable<daoben_product_sn>()
                                    .JoinTable<daoben_activity_recommendation_product>((a, b) => a.model == b.model && a.color == b.color)
                                    .JoinTable<daoben_activity_recommendation_emp>((a, d) => a.out_sales_id == d.emp_id)
                                    .Where<daoben_activity_recommendation_product, daoben_activity_recommendation_emp>((a, b, d) => b.main_id == recom.id && a.status > 0 &&
                                        d.main_id == recom.id && a.outstorage_time >= startDate && a.outstorage_time < endDate)
                                    .Select("a.*").ToList();
                    }
                    else if (recom.product_scope > 1 && recom.target_content == 1)
                    {
                        snList = db.Queryable<daoben_product_sn>()
                                    .JoinTable<daoben_activity_recommendation_product>((a, b) => a.model == b.model && a.color == b.color)
                                    .JoinTable<daoben_activity_recommendation_emp>((a, d) => a.sales_id == d.emp_id)
                                    .Where<daoben_activity_recommendation_product, daoben_activity_recommendation_emp>((a, b, d) => b.main_id == recom.id && a.status > 0 &&
                                        d.main_id == recom.id && a.sale_time >= startDate && a.sale_time < endDate)
                                    .Select("a.*").ToList();
                    }
                    else if (recom.product_scope == 1 && recom.target_content == 1)
                    {
                        snList = db.Queryable<daoben_product_sn>()
                                    .JoinTable<daoben_activity_recommendation_emp>((a, c) => c.emp_id == a.sales_id)
                                    .Where<daoben_activity_recommendation_emp>((a, c) => c.main_id == recom.id && a.status > 0
                                        && a.sale_time >= startDate && a.sale_time < endDate)
                                    .Select("a.*").ToList();
                    }
                    else if (recom.product_scope == 1 && recom.target_content == 2)
                    {
                        snList = db.Queryable<daoben_product_sn>()
                                    .JoinTable<daoben_activity_recommendation_emp>((a, c) => c.emp_id == a.out_sales_id)
                                    .Where<daoben_activity_recommendation_emp>((a, c) => c.main_id == recom.id && a.status > 0
                                        && a.outstorage_time >= startDate && a.outstorage_time < endDate)
                                    .Select("a.*").ToList();
                    }
                }
                else if (recom.emp_category == 3)//导购员
                {
                    if (recom.product_scope > 1 && recom.target_content == 2)
                    {
                        //snList = db.Queryable<daoben_product_sn>()
                        //            .JoinTable<daoben_activity_recommendation_product>((a, b) => a.model == b.model && a.color == b.color)
                        //            .JoinTable<daoben_activity_recommendation_emp>((a, c) => a.out_guide_id == c.emp_id)
                        //            .Where<daoben_activity_recommendation_product, daoben_activity_recommendation_emp>((a, b, c) => b.main_id == recom.id && a.status > 0 &&
                        //                c.main_id == recom.id && a.outstorage_time >= startDate && a.outstorage_time < endDate)
                        //            .Select("a.*").ToList();
                    }
                    else if (recom.product_scope > 1 && recom.target_content == 1)
                    {
                        snList = db.Queryable<daoben_product_sn>()
                                    .JoinTable<daoben_activity_recommendation_product>((a, b) => a.model == b.model && a.color == b.color)
                                    .JoinTable<daoben_activity_recommendation_emp>((a, c) => a.reporter_id == c.emp_id)
                                    .Where<daoben_activity_recommendation_product, daoben_activity_recommendation_emp>((a, b, c) => b.main_id == recom.id && a.status > 0 &&
                                        c.main_id == recom.id && a.sale_time >= startDate && a.sale_time < endDate)
                                    .Select("a.*").ToList();
                    }
                    else if (recom.product_scope == 1 && recom.target_content == 1)
                    {
                        snList = db.Queryable<daoben_product_sn>()
                                    .JoinTable<daoben_activity_recommendation_emp>((a, b) => a.reporter_id == b.emp_id)
                                    .Where<daoben_activity_recommendation_emp>((a, b) => b.main_id == recom.id && a.status > 0
                                        && a.sale_time >= startDate && a.sale_time < endDate)
                                    .Select("a.*").ToList();
                    }
                    else if (recom.product_scope == 1 && recom.target_content == 2)
                    {
                        //snList = db.Queryable<daoben_product_sn>()
                        //            .JoinTable<daoben_activity_recommendation_emp>((a, b) => a.out_guide_id == b.emp_id)
                        //            .Where<daoben_activity_recommendation_emp>((a, b) => b.main_id == recom.id && a.status > 0
                        //                && a.outstorage_time >= startDate && a.outstorage_time < endDate)
                        //            .Select("a.*").ToList();
                    }
                }

                if (recom.target_mode == 1)//完成率
                {
                    if (recom.activity_target <= 0)
                        continue;
                    foreach (var i in empList)
                    {
                        i.total_amount = 0;
                        i.total_count = 0;
                        i.total_ratio = 0;
                        i.total_normal_count = 0;
                        i.total_reward = 0;//初始化，重新计算

                        List<daoben_product_sn> snTempList = new List<daoben_product_sn>();
                        if (recom.emp_category == 21)
                        {
                            if (recom.target_content == 1)
                                snTempList = snList.Where(a => i.emp_id == a.sales_m_id).ToList();
                            else
                                snTempList = snList.Where(a => i.emp_id == a.out_sales_m_id).ToList();
                        }
                        else if (recom.emp_category == 20)
                        {
                            if (recom.target_content == 1)
                                snTempList = snList.Where(a => i.emp_id == a.sales_id).ToList();
                            else
                                snTempList = snList.Where(a => i.emp_id == a.out_sales_id).ToList();
                        }
                        else
                        {
                            if (recom.target_content == 1)
                                snTempList = snList.Where(a => a.reporter_id == i.emp_id).ToList();
                            //else
                            //    snTempList = snList.Where(a => a.out_guide_id == i.emp_id).ToList();
                        }

                        i.total_count = snTempList.Count();
                        snTempList = snTempList.Where(y => (y.sale_type == 1 || y.sale_type == 3) && y.status < 10 && !y.special_offer).ToList();
                        i.total_normal_count = snTempList.Count();
                        if (recom.target_content == 1)
                            i.total_amount = snTempList.Sum(t => t.price_retail);
                        else
                            i.total_amount = snTempList.Sum(t => t.price_wholesale);

                        i.total_ratio = (i.total_count.ToDecimal() * 100) / recom.activity_target;//保留两位小数
                        daoben_activity_recommendation_reward rebateModel = rewardList.Where(b => i.total_ratio >= b.target_min).OrderByDescending(t => t.target_min).First();
                        if (recom.rebate_mode == 1)
                            i.total_reward = i.total_normal_count * rebateModel.reward;
                        else if (recom.rebate_mode == 2)
                        {
                            foreach (var a in snTempList)
                            {
                                i.total_reward = i.total_reward + a.price_wholesale * rebateModel.reward * (decimal)(0.01);
                            }
                        }
                        else if (recom.rebate_mode == 3)
                        {
                            foreach (var a in snTempList)
                            {
                                i.total_reward = i.total_reward + a.price_retail * rebateModel.reward * (decimal)(0.01);
                            }
                        }
                        else if (recom.rebate_mode == 4)
                            i.total_reward = rebateModel.reward;
                    };
                }
                else if (recom.target_mode == 2)//按照台数
                {
                    foreach (var i in empList)
                    {
                        i.total_reward = 0;
                        i.total_amount = 0;
                        i.total_count = 0;
                        i.total_ratio = 0;
                        i.total_normal_count = 0;//初始化，重新计算
                        List<daoben_product_sn> snTempList = new List<daoben_product_sn>();
                        if (recom.emp_category == 21)
                        {
                            List<string> distriIdList = distriList.Where(a => a.area_l1_id == i.area_l1_id).Select(t => t.id).ToList();
                            snTempList = snList.Where(a => a.sales_id == i.emp_id).ToList();
                            if (recom.target_content == 1)
                                snTempList = snList.Where(a => distriIdList.Contains(a.sale_distributor_id)).ToList();
                            else
                                snTempList = snList.Where(a => distriIdList.Contains(a.out_distributor_id)).ToList();
                        }
                        else if (recom.emp_category == 20)
                        {
                            List<string> distriIdList = distriList.Where(a => a.area_l2_id == i.area_l2_id).Select(t => t.id).ToList();
                            snTempList = snList.Where(a => a.sales_id == i.emp_id).ToList();
                            if (recom.target_content == 1)
                                snTempList = snList.Where(a => distriIdList.Contains(a.sale_distributor_id)).ToList();
                            else
                                snTempList = snList.Where(a => distriIdList.Contains(a.out_distributor_id)).ToList();
                        }
                        else
                            snTempList = snList.Where(a => a.reporter_id == i.emp_id).ToList();
                        i.total_count = snTempList.Count();
                        snTempList = snTempList.Where(y => (y.sale_type == 1 || y.sale_type == 3) && y.status < 10 && !y.special_offer).ToList();
                        i.total_normal_count = snTempList.Count();
                        if (recom.target_content == 1)
                            i.total_amount = snTempList.Sum(t => t.price_retail);
                        else
                            i.total_amount = snTempList.Sum(t => t.price_wholesale);

                        daoben_activity_recommendation_reward rebateModel = rewardList.Where(b => i.total_count >= b.target_min).OrderByDescending(t => t.target_min).First();
                        if (recom.rebate_mode == 1)
                            i.total_reward = i.total_normal_count * rebateModel.reward;
                        else if (recom.rebate_mode == 2)
                        {
                            foreach (var a in snTempList)
                            {
                                i.total_reward = i.total_reward + a.price_wholesale * rebateModel.reward * (decimal)(0.01);
                            }
                        }
                        else if (recom.rebate_mode == 3)
                        {
                            foreach (var a in snTempList)
                            {
                                i.total_reward = i.total_reward + a.price_retail * rebateModel.reward * (decimal)(0.01);
                            }
                        }
                        else if (recom.rebate_mode == 4)
                            i.total_reward = rebateModel.reward;
                    };
                }
                else if (recom.target_mode == 3)//按零售价
                {
                    foreach (var i in empList)
                    {
                        i.total_reward = 0;
                        i.total_amount = 0;
                        i.total_count = 0;
                        i.total_ratio = 0;
                        i.total_normal_count = 0;//初始化，重新计算
                        List<daoben_product_sn> tempSnlist = new List<daoben_product_sn>();
                        List<string> tempDisTriIdList = new List<string>();
                        if (recom.emp_category == 21)
                        {
                            tempDisTriIdList = distriList.Where(a => a.area_l1_id == i.area_l1_id).Select(t => t.id).ToList();
                            if (recom.target_content == 2)
                                tempSnlist = snList.Where(a => tempDisTriIdList.Contains(a.out_distributor_id)).ToList();
                            else
                                tempSnlist = snList.Where(a => tempDisTriIdList.Contains(a.sale_distributor_id)).ToList();
                        }
                        else if (recom.emp_category == 20)
                        {
                            tempDisTriIdList = distriList.Where(a => a.area_l2_id == i.area_l2_id).Select(t => t.id).ToList();
                            if (recom.target_content == 2)
                                tempSnlist = snList.Where(a => tempDisTriIdList.Contains(a.out_distributor_id)).ToList();
                            else
                                tempSnlist = snList.Where(a => tempDisTriIdList.Contains(a.sale_distributor_id)).ToList();
                        }
                        else if (recom.emp_category == 3)
                        {
                            tempSnlist = snList.Where(a => a.reporter_id == i.emp_id).ToList();
                        }
                        i.total_count = tempSnlist.Count();
                        tempSnlist = tempSnlist.Where(y => (y.sale_type == 1 || y.sale_type == 3) && y.status < 10 && !y.special_offer).ToList();
                        i.total_normal_count = tempSnlist.Count();
                        if (recom.target_content == 1)
                            i.total_amount = tempSnlist.Sum(t => t.price_retail);
                        else
                            i.total_amount = tempSnlist.Sum(t => t.price_wholesale);
                        foreach (var j in tempSnlist)
                        {
                            daoben_activity_recommendation_reward rebateModel = rewardList.Where(b => j.price_retail >= b.target_min).OrderByDescending(t => t.target_min).First();
                            if (recom.rebate_mode == 1)
                                i.total_reward = i.total_reward + rebateModel.reward;
                            else if (recom.rebate_mode == 2)
                                i.total_reward = i.total_reward + j.price_wholesale * rebateModel.reward * (decimal)(0.01);
                            else if (recom.rebate_mode == 3)
                                i.total_reward = i.total_reward + j.price_retail * rebateModel.reward * (decimal)(0.01);

                        }
                    }
                }
                else if (recom.target_mode == 5)//按批发价
                {
                    foreach (var i in empList)
                    {
                        i.total_reward = 0;
                        i.total_amount = 0;
                        i.total_count = 0;
                        i.total_ratio = 0;
                        i.total_normal_count = 0;//初始化，重新计算
                        List<daoben_product_sn> tempSnlist = new List<daoben_product_sn>();
                        List<string> tempDisTriIdList = new List<string>();
                        if (recom.emp_category == 21)
                        {
                            tempDisTriIdList = distriList.Where(a => a.area_l1_id == i.area_l1_id).Select(t => t.id).ToList();
                            if (recom.target_content == 2)
                                tempSnlist = snList.Where(a => tempDisTriIdList.Contains(a.out_distributor_id)).ToList();
                            else
                                tempSnlist = snList.Where(a => tempDisTriIdList.Contains(a.sale_distributor_id)).ToList();
                        }
                        else if (recom.emp_category == 20)
                        {
                            tempDisTriIdList = distriList.Where(a => a.area_l2_id == i.area_l2_id).Select(t => t.id).ToList();
                            if (recom.target_content == 2)
                                tempSnlist = snList.Where(a => tempDisTriIdList.Contains(a.out_distributor_id)).ToList();
                            else
                                tempSnlist = snList.Where(a => tempDisTriIdList.Contains(a.sale_distributor_id)).ToList();
                        }
                        else if (recom.emp_category == 3)
                        {
                            tempSnlist = snList.Where(a => a.reporter_id == i.emp_id).ToList();
                        }
                        i.total_count = tempSnlist.Count();
                        tempSnlist = tempSnlist.Where(y => (y.sale_type == 1 || y.sale_type == 3) && y.status < 10 && !y.special_offer).ToList();
                        i.total_normal_count = tempSnlist.Count();
                        if (recom.target_content == 1)
                            i.total_amount = tempSnlist.Sum(t => t.price_retail);
                        else
                            i.total_amount = tempSnlist.Sum(t => t.price_wholesale);
                        foreach (var j in tempSnlist)
                        {
                            daoben_activity_recommendation_reward rebateModel = rewardList.Where(b => j.price_wholesale >= b.target_min).OrderByDescending(t => t.target_min).First();
                            if (recom.rebate_mode == 1)
                                i.total_reward = i.total_reward + rebateModel.reward;
                            else if (recom.rebate_mode == 2)
                                i.total_reward = i.total_reward + j.price_wholesale * rebateModel.reward * (decimal)(0.01);
                            else if (recom.rebate_mode == 3)
                                i.total_reward = i.total_reward + j.price_retail * rebateModel.reward * (decimal)(0.01);

                        }
                    }
                }
                else if (recom.target_mode == 4)//按型号
                {
                    foreach (var i in empList)
                    {
                        i.total_reward = 0;
                        i.total_amount = 0;
                        i.total_count = 0;
                        i.total_ratio = 0;
                        i.total_normal_count = 0;//初始化，重新计算
                        List<daoben_product_sn> tempSnlist = new List<daoben_product_sn>();
                        List<string> tempDisTriIdList = new List<string>();
                        if (recom.emp_category == 21)
                        {
                            tempDisTriIdList = distriList.Where(a => a.area_l1_id == i.area_l1_id).Select(t => t.id).ToList();
                            if (recom.target_content == 2)
                                tempSnlist = snList.Where(a => tempDisTriIdList.Contains(a.out_distributor_id)).ToList();
                            else
                                tempSnlist = snList.Where(a => tempDisTriIdList.Contains(a.sale_distributor_id)).ToList();
                        }
                        else if (recom.emp_category == 20)
                        {
                            tempDisTriIdList = distriList.Where(a => a.area_l2_id == i.area_l2_id).Select(t => t.id).ToList();
                            if (recom.target_content == 2)
                                tempSnlist = snList.Where(a => tempDisTriIdList.Contains(a.out_distributor_id)).ToList();
                            else
                                tempSnlist = snList.Where(a => tempDisTriIdList.Contains(a.sale_distributor_id)).ToList();
                        }
                        else if (recom.emp_category == 3)
                        {
                            tempSnlist = snList.Where(a => a.reporter_id == i.emp_id).ToList();
                        }
                        if (recom.target_content == 1)
                            i.total_amount = tempSnlist.Sum(t => t.price_retail);
                        else
                            i.total_amount = tempSnlist.Sum(t => t.price_wholesale);
                        i.total_count = tempSnlist.Count();
                        foreach (var j in tempSnlist)
                        {
                            daoben_activity_recommendation_product rewardModel = productList.Where(a => a.model == j.model && a.color == j.color).SingleOrDefault();
                            if (rewardList == null)//说明有不符合机型规定的product_sn进入到统计 “全部机型”productList为空不能按型号返利
                                continue;
                            if (recom.rebate_mode == 1)
                                i.total_reward = i.total_reward + rewardModel.reward;
                            else if (recom.rebate_mode == 2)
                                i.total_reward = i.total_reward + j.price_wholesale * rewardModel.reward * (decimal)(0.01);
                            else if (recom.rebate_mode == 3)
                                i.total_reward = i.total_reward + j.price_retail * rewardModel.reward * (decimal)(0.01);
                        }
                    }
                }

                #endregion
                recomEmpList.AddRange(empList);
            }
            if (recomEmpList.Count() <= 0)
                return null;
            //活动管理 主推活动
            StringBuilder sqlUpSb5 = new StringBuilder("insert into daoben_activity_recommendation_emp (`id`, `total_count`, `total_amount`, `total_ratio`, `total_reward`, `total_normal_count`) values ");
            foreach (var a in recomEmpList)
            {
                sqlUpSb5.AppendFormat("({0},{1},{2},{3},{4},{5}),", a.id, a.total_count, a.total_amount, a.total_ratio, a.total_reward, a.total_normal_count);
            }
            sqlUpSb5.Remove(sqlUpSb5.Length - 1, 1); // 最后一个逗号
            sqlUpSb5.Append("on duplicate key update total_count=values(total_count),total_amount=values(total_amount),total_ratio=values(total_ratio),total_reward=values(total_reward),total_normal_count=values(total_normal_count);");
            return sqlUpSb5.ToString();
        }
        public string EmpRankReward(SqlSugarClient db, List<daoben_activity_ranking> activityRankList)
        {
            //活动管理 排名活动
            List<daoben_activity_ranking_emp> rankEmpList = new List<daoben_activity_ranking_emp>();
            foreach (var ranking in activityRankList)
            {
                #region 活动管理 排名活动
                //排名

                DateTime startDate = ranking.start_date.ToDate();
                DateTime endDate = ranking.end_date.ToDate().AddDays(1);
                List<daoben_activity_ranking_emp> empList = db.Queryable<daoben_activity_ranking_emp>().Where(a => a.main_id == ranking.id).ToList();
                List<daoben_product_sn> snList = new List<daoben_product_sn>();
                List<daoben_activity_ranking_reward> rewardList = db.Queryable<daoben_activity_ranking_reward>()
                    .Where(a => a.main_id == ranking.id).ToList();

                List<daoben_distributor_info> distriList = db.Queryable<daoben_distributor_info>()
                    .Where(a => a.company_id == ranking.company_id || a.company_id_parent == ranking.company_id).ToList();
                //是否指定机型，实销or下货 业务员，业务经理，导购员
                if (ranking.emp_category == 21)//业务经理
                {
                    if (ranking.ranking_content == 2 || ranking.ranking_content == 4)
                    {
                        snList = db.Queryable<daoben_product_sn>()
                                    .JoinTable<daoben_activity_ranking_emp>((a, c) => c.emp_id == a.out_sales_m_id)
                                    .Where<daoben_activity_ranking_emp>((a, c) => c.main_id == ranking.id && a.status > 0
                                        && a.outstorage_time >= startDate && a.outstorage_time < endDate)
                                    .Select("a.*").ToList();

                    }
                    else if (ranking.ranking_content == 1 || ranking.ranking_content == 3)
                    {
                        snList = db.Queryable<daoben_product_sn>()
                                    .JoinTable<daoben_activity_ranking_emp>((a, c) => c.emp_id == a.sales_m_id)
                                    .Where<daoben_activity_ranking_emp>((a, c) => c.main_id == ranking.id && a.status > 0
                                        && a.sale_time >= startDate && a.sale_time < endDate)
                                    .Select("a.*").ToList();
                    }
                }
                else if (ranking.emp_category == 20)//业务员
                {
                    if (ranking.ranking_content == 2 || ranking.ranking_content == 4)
                    {
                        snList = db.Queryable<daoben_product_sn>()
                                    .JoinTable<daoben_activity_ranking_emp>((a, c) => a.out_sales_id == c.emp_id)
                                    .Where<daoben_activity_ranking_emp>((a, c) => c.main_id == ranking.id && a.status > 0
                                        && a.outstorage_time >= startDate && a.outstorage_time < endDate)
                                    .Select("a.*").ToList();

                    }
                    else if (ranking.ranking_content == 1 || ranking.ranking_content == 3)
                    {
                        snList = db.Queryable<daoben_product_sn>()
                                    .JoinTable<daoben_activity_ranking_emp>((a, c) => a.sales_id == c.emp_id)
                                    .Where<daoben_activity_ranking_emp>((a, c) => c.main_id == ranking.id && a.status > 0
                                        && a.sale_time >= startDate && a.sale_time < endDate)
                                    .Select("a.*").ToList();
                    }
                }
                else if (ranking.emp_category == 3)//导购员  
                {
                    if (ranking.ranking_content == 2 || ranking.ranking_content == 4)
                    {
                        //snList = db.Queryable<daoben_product_sn>()
                        //            .JoinTable<daoben_activity_ranking_emp>((a, b) => a.out_guide_id == b.emp_id)
                        //            .Where<daoben_activity_ranking_emp>((a, b) => b.main_id == ranking.id && a.status > 0
                        //                && a.sale_time >= startDate && a.sale_time < endDate)
                        //            .Select("a.*").ToList();
                    }
                    else if (ranking.ranking_content == 1 || ranking.ranking_content == 3)
                    {
                        snList = db.Queryable<daoben_product_sn>()
                                    .JoinTable<daoben_activity_ranking_emp>((a, b) => a.reporter_id == b.emp_id)
                                    .Where<daoben_activity_ranking_emp>((a, b) => b.main_id == ranking.id && a.status > 0
                                        && a.sale_time >= startDate && a.sale_time < endDate)
                                    .Select("a.*").ToList();
                    }
                }

                if (ranking.emp_category == 21)//业务经理
                {
                    foreach (var i in empList)
                    {
                        i.reward = 0;
                        i.counting_palce = 0;
                        i.counting_count = 0;
                        i.counting_amount = 0;//初始化，重新计算
                        List<daoben_product_sn> snTempList = new List<daoben_product_sn>();

                        if (ranking.ranking_content == 1 || ranking.ranking_content == 3)
                            snTempList = snList.Where(a => a.sales_m_id == i.emp_id).ToList();
                        else if (ranking.ranking_content == 2 || ranking.ranking_content == 4)
                            snTempList = snList.Where(a => a.out_sales_m_id == i.emp_id).ToList();
                        if (ranking.ranking_content < 3)
                            i.counting_count = snTempList.Count();
                        else if (ranking.ranking_content == 3)
                        {
                            foreach (var a in snTempList)
                            {
                                i.counting_amount += a.price_retail;
                            }
                        }
                        else if (ranking.ranking_content == 4)
                        {
                            foreach (var a in snTempList)
                            {
                                i.counting_amount += a.price_wholesale;
                            }
                        }
                    }
                }
                else if (ranking.emp_category == 20)//业务员
                {
                    foreach (var i in empList)
                    {
                        i.reward = 0;
                        i.counting_palce = 0;
                        i.counting_count = 0;
                        i.counting_amount = 0;//初始化，重新计算
                        List<daoben_product_sn> snTempList = new List<daoben_product_sn>();

                        if (ranking.ranking_content == 1 || ranking.ranking_content == 3)
                            snTempList = snList.Where(a => a.sales_id == i.emp_id).ToList();
                        else if (ranking.ranking_content == 2 || ranking.ranking_content == 4)
                            snTempList = snList.Where(a => a.out_sales_id == i.emp_id).ToList();
                        if (ranking.ranking_content < 3)
                            i.counting_count = snTempList.Count();
                        else if (ranking.ranking_content == 3)
                        {
                            foreach (var a in snTempList)
                            {
                                i.counting_amount += a.price_retail;
                            }
                        }
                        else if (ranking.ranking_content == 4)
                        {
                            foreach (var a in snTempList)
                            {
                                i.counting_amount += a.price_wholesale;
                            }
                        }
                    }
                }
                else if (ranking.emp_category == 3)//导购员
                {
                    foreach (var i in empList)
                    {
                        i.reward = 0;
                        i.counting_amount = 0;
                        i.counting_count = 0;
                        i.counting_palce = 0;//初始化，重新计算
                        List<daoben_product_sn> snTempList = new List<daoben_product_sn>();
                        if (ranking.ranking_content == 1 || ranking.ranking_content == 3)
                            snTempList = snList.Where(a => a.reporter_id == i.emp_id).ToList();
                        //if (ranking.ranking_content == 2 || ranking.ranking_content == 4)
                        //    snTempList = snList.Where(a => a.out_guide_id == i.emp_id).ToList();
                        if (ranking.ranking_content < 3)
                            i.counting_count = snTempList.Count();
                        else if (ranking.ranking_content == 3)
                        {
                            foreach (var a in snTempList)
                            {
                                i.counting_amount += a.price_retail;
                            }
                        }
                        else if (ranking.ranking_content == 4)
                        {
                            foreach (var a in snTempList)
                            {
                                i.counting_amount += a.price_wholesale;
                            }
                        }
                    }
                }

                countAmountRank[] rankArray = new countAmountRank[empList.Count()];//排名计算
                if (ranking.ranking_content <= 2)
                {
                    empList = empList.OrderByDescending(t => t.counting_count).ToList();
                    int rank = 0;
                    foreach (var a in empList)
                    {
                        a.counting_time = DateTime.Now;
                        if (rank == 0)
                        {
                            a.counting_palce = rank + 1;
                            rankArray[rank] = new countAmountRank();
                            rankArray[rank].count = a.counting_count;
                            rankArray[rank].rank = rank + 1;
                            rank++;
                        }
                        else
                        {
                            if (rankArray[rank - 1].count == a.counting_count)
                            {
                                a.counting_palce = rank;
                            }
                            else
                            {
                                a.counting_palce = rank + 1;
                                rankArray[rank] = new countAmountRank();
                                rankArray[rank].count = a.counting_count;
                                rankArray[rank].rank = rank + 1;
                                rank++;
                            }
                        }
                    }
                }
                else
                {
                    empList = empList.OrderByDescending(t => t.counting_amount).ToList();
                    int rank = 0;
                    foreach (var a in empList)
                    {
                        a.counting_time = DateTime.Now;
                        if (rank == 0)
                        {
                            a.counting_palce = rank + 1;
                            rankArray[rank] = new countAmountRank();
                            rankArray[rank].amount = a.counting_amount;
                            rankArray[rank].rank = rank + 1;
                            rank++;
                        }
                        else
                        {
                            if (rankArray[rank - 1].amount == a.counting_amount)
                            {
                                a.counting_palce = rank;
                            }
                            else
                            {
                                a.counting_palce = rank + 1;
                                rankArray[rank] = new countAmountRank();
                                rankArray[rank].amount = a.counting_amount;
                                rankArray[rank].rank = rank + 1;
                                rank++;
                            }
                        }
                    }
                }
                if (ranking.end_date <= DateTime.Now && empList.OrderBy(t => t.counting_palce).First().final_place == 0)
                {
                    foreach (var a in empList)
                    {
                        a.final_place = a.counting_palce;
                        daoben_activity_ranking_reward rewardTemp = rewardList
                                .Where(t => a.final_place <= t.place_min).OrderBy(t => t.place_min).FirstOrDefault();
                        if (rewardTemp != null)
                        {
                            if (a.counting_count > 0 || a.counting_amount > 0)
                                a.reward = rewardTemp.reward;
                        }

                    }
                }
                #endregion
                rankEmpList.AddRange(empList);
            }
            if (rankEmpList.Count() <= 0)
                return null;
            //活动管理 排名活动
            StringBuilder sqlUpSb6 = new StringBuilder("insert into daoben_activity_ranking_emp (`id`, `counting_amount`, `counting_count`, `counting_palce`,`final_place`,`reward`, `counting_time`) values ");
            foreach (var a in rankEmpList)
            {
                sqlUpSb6.AppendFormat("({0},{1},{2},{3},{4},{5},'{6}'),", a.id, a.counting_amount, a.counting_count, a.counting_palce, a.final_place, a.reward, a.counting_time.ToDate().AddDays(-1));
            }
            sqlUpSb6.Remove(sqlUpSb6.Length - 1, 1); // 最后一个逗号
            sqlUpSb6.Append("on duplicate key update counting_amount=values(counting_amount),counting_count=values(counting_count),counting_palce=values(counting_palce),final_place=values(final_place),reward=values(reward),counting_time=values(counting_time);");
            return sqlUpSb6.ToString();
        }
        public string EmpPKReward(SqlSugarClient db, List<daoben_activity_pk> activityPKList)
        {
            //活动管理 PK活动
            List<daoben_activity_pk_emp> pkEmpList = new List<daoben_activity_pk_emp>();
            foreach (var pkInfo in activityPKList)
            {
                #region 活动管理 PK活动

                //分别对上述活动进行统计
                //PK
                DateTime now = DateTime.Now;
                DateTime startDate = pkInfo.start_date.ToDate();
                DateTime endDate = pkInfo.end_date.ToDate().AddDays(1);
                List<daoben_activity_pk_emp> empList = db.Queryable<daoben_activity_pk_emp>().Where(a => a.main_id == pkInfo.id).ToList();
                List<daoben_product_sn> snList = new List<daoben_product_sn>();
                List<daoben_activity_pk_product> productList = db.Queryable<daoben_activity_pk_product>().Where(a => a.main_id == pkInfo.id).ToList();
                List<daoben_distributor_info> distriList = db.Queryable<daoben_distributor_info>()
                    .Where(a => a.company_id == pkInfo.company_id || a.company_id_parent == pkInfo.company_id).ToList();
                //是否指定机型 DEFAULT实销? 业务员，业务经理，导购员
                if (pkInfo.emp_category == 21)//业务经理
                {
                    snList = db.Queryable<daoben_product_sn>()
                        .JoinTable<daoben_activity_pk_emp>((a, c) => a.sales_m_id == c.emp_id)
                        .Where<daoben_activity_pk_emp>((a, c) => c.main_id == pkInfo.id && a.sale_type > 0
                            && a.sale_time >= startDate && a.sale_time < endDate)
                        .Select("a.*").ToList();
                }
                else if (pkInfo.emp_category == 20)//业务员
                {
                    snList = db.Queryable<daoben_product_sn>()
                        .JoinTable<daoben_activity_pk_emp>((a, c) => a.sales_id == c.emp_id)
                        .Where<daoben_activity_pk_emp>((a, c) => c.main_id == pkInfo.id && a.sale_type > 0
                            && a.sale_time >= startDate && a.sale_time < endDate)
                        .Select("a.*").ToList();
                }
                else if (pkInfo.emp_category == 3)//导购员
                {
                    snList = db.Queryable<daoben_product_sn>()
                        .JoinTable<daoben_activity_pk_emp>((a, b) => a.reporter_id == b.emp_id)
                        .Where<daoben_activity_pk_emp>((a, b) => b.main_id == pkInfo.id && a.sale_type > 0
                            && a.sale_time >= startDate && a.sale_time < endDate)
                        .Select("a.*").ToList();
                }
                if (pkInfo.product_scope > 1)
                    snList = snList.Where(a => productList.Exists(t => t.color == a.color && t.model == a.model)).ToList();

                if (pkInfo.emp_category == 21)//业务经理
                {
                    foreach (var i in empList)
                    {
                        i.total_amount = 0;
                        i.total_count = 0;
                        i.total_ratio = 0;
                        i.total_reward = 0;
                        //初始化，重新计算
                        List<daoben_product_sn> snTempList = new List<daoben_product_sn>();
                        snTempList = snList.Where(a => a.sales_m_id == i.emp_id).ToList();
                        i.total_count = snTempList.Count();
                        i.total_ratio = i.activity_target == 0 ? 100 : i.total_count / i.activity_target;
                    }
                }
                else if (pkInfo.emp_category == 20)//业务员
                {
                    foreach (var i in empList)
                    {
                        i.total_amount = 0;
                        i.total_count = 0;
                        i.total_ratio = 0;
                        i.total_reward = 0;//初始化，重新计算
                        List<daoben_product_sn> snTempList = new List<daoben_product_sn>();
                        snTempList = snList.Where(a => a.sales_id == i.emp_id).ToList();
                        i.total_count = snTempList.Count();
                        i.total_ratio = i.activity_target == 0 ? 0 : i.total_count / i.activity_target;
                    }
                }
                else if (pkInfo.emp_category == 3)//导购员
                {
                    foreach (var i in empList)
                    {
                        i.total_amount = 0;
                        i.total_count = 0;
                        i.total_ratio = 0;
                        i.total_reward = 0;//初始化，重新计算                            
                        List<daoben_product_sn> snTempList = new List<daoben_product_sn>();
                        snTempList = snList.Where(a => a.reporter_id == i.emp_id).ToList();
                        i.total_count = snTempList.Count();
                        i.total_ratio = (i.activity_target == 0 ? 100 : (i.total_count * 100 / i.activity_target));
                    }
                }
                empList = empList.OrderBy(t => t.group_number).ThenByDescending(t => t.total_ratio).ToList();

                daoben_activity_pk_emp[] empArray = empList.ToArray();
                bool isWin = true;
                for (int a = 0; a < empArray.Length; a++)
                {
                    if (isWin)
                    {
                        if (empArray[a].total_ratio == empArray[a + 1].total_ratio)
                            empArray[a].winner = false;
                        else
                            empArray[a].winner = true;
                    }
                    //输赢 考虑平手
                    if (isWin)
                    {
                        if (empArray[a].total_ratio > empArray[a + 1].total_ratio)
                            empArray[a].total_reward = (empArray[a].winner ? pkInfo.win_lose : 0 - pkInfo.lose_win);
                    }
                    else
                    {
                        if (empArray[a].total_ratio < empArray[a - 1].total_ratio)
                            empArray[a].total_reward = (empArray[a].winner ? pkInfo.win_lose : 0 - pkInfo.lose_win);
                    }
                    //公司奖励
                    if (pkInfo.win_company_condition == 2 && isWin && empArray[a].total_ratio >= 100)
                    {
                        empArray[a].total_reward += pkInfo.win_company;
                        empArray[a].company_reward = pkInfo.win_company;
                    }
                    if (pkInfo.win_company_condition == 1 && !isWin && empArray[a].total_ratio >= 100)
                    {
                        empArray[a - 1].total_reward += pkInfo.win_company;
                        empArray[a - 1].company_reward = pkInfo.win_company;
                    }
                    isWin = !isWin;
                }
                if (pkInfo.end_date <= now)
                {
                    for (int a = 0; a < empArray.Length; a++)
                    {
                        if (empArray[a].winner)
                        {
                            empArray[a].total_reward = (empArray[a].total_ratio >= 100 ? empArray[a].total_reward : empArray[a].total_reward - pkInfo.win_penalty);
                            empArray[a].total_penalty = (empArray[a].total_ratio >= 100 ? 0 : pkInfo.win_penalty);
                        }
                        else
                        {
                            empArray[a].total_reward = (empArray[a].total_ratio >= 100 ? empArray[a].total_reward : empArray[a].total_reward - pkInfo.lose_penalty);
                            empArray[a].total_penalty = (empArray[a].total_ratio >= 100 ? 0 : pkInfo.lose_penalty);
                        }
                    }
                }
                #endregion
                pkEmpList.AddRange(empArray.ToList());
            }
            if (pkEmpList.Count() <= 0)
                return null;
            //更新到数据库中 
            StringBuilder sqlUpSb1 = new StringBuilder("insert into daoben_activity_pk_emp (`id`, `total_amount`, `total_count`, `total_ratio`,`total_reward`,`winner`,`total_penalty`,`company_reward`) values ");
            foreach (var a in pkEmpList)
            {
                sqlUpSb1.AppendFormat("({0},{1},{2},{3},{4},{5},{6},{7}),", a.id, a.total_amount, a.total_count, a.total_ratio, a.total_reward, a.winner, a.total_penalty, a.company_reward);
            }
            sqlUpSb1.Remove(sqlUpSb1.Length - 1, 1); // 最后一个逗号
            sqlUpSb1.Append("on duplicate key update total_amount=values(total_amount),total_count=values(total_count),total_ratio=values(total_ratio),total_reward=values(total_reward),winner=values(winner),total_penalty=values(total_penalty),company_reward=values(company_reward);");
            return sqlUpSb1.ToString();
        }

        /// <summary>
        /// 用于 排名比赛 排序
        /// </summary>
        public class countAmountRank
        {
            public int count { get; set; }
            public decimal amount { get; set; }
            public int rank { get; set; }
        }
        #endregion

        #region 达量返利
        public object AttainingMainList(Pagination pagination, QueryTime queryTime, int company_id, string distributor_name)
        {
            var LoginInfo = OperatorProvider.Provider.GetCurrent();
            if (LoginInfo == null)
                return null;
            if (pagination == null)
                pagination = new Pagination();
            pagination.page = pagination.page > 0 ? pagination.page : 1;
            pagination.rows = pagination.rows > 0 ? pagination.rows : 20;
            pagination.sidx = string.IsNullOrEmpty(pagination.sidx) ? "a.distributor_id" : pagination.sidx;
            pagination.sord = string.IsNullOrEmpty(pagination.sord) ? "desc" : pagination.sord;
            CompanyInfo myCompanyInfo = LoginInfo.companyInfo;
            PositionInfo myPositionInfo = LoginInfo.positionInfo;

            using (var db = SugarDao.GetInstance())
            {
                var qable = db.Queryable<daoben_distributor_attaining_distributor>()
                    .JoinTable<daoben_distributor_info>((a, b) => a.distributor_id == b.id)
                    .JoinTable<daoben_distributor_attaining>((a, c) => a.main_id == c.id);

                if (myCompanyInfo.category == "事业部")
                    qable.Where<daoben_distributor_attaining>((a, c) => c.company_id_parent == myCompanyInfo.id);
                else if (myCompanyInfo.category == "分公司")
                    qable.Where<daoben_distributor_attaining>((a, c) => c.company_id == myCompanyInfo.id);
                if (company_id > 0)
                    qable.Where<daoben_distributor_attaining>((a, c) => c.company_id == company_id);

                if (queryTime.startTime1 != null)
                    qable.Where<daoben_distributor_attaining>((a, c) => c.end_date >= queryTime.startTime1);
                if (queryTime.startTime2 != null)
                {
                    queryTime.startTime2 = queryTime.startTime2.ToDate().AddDays(1);
                    qable.Where<daoben_distributor_attaining>((a, c) => c.end_date < queryTime.startTime2);
                }

                if (!string.IsNullOrEmpty(distributor_name))
                    qable.Where(a => a.distributor_name.Contains(distributor_name));

                string listStr = qable
                        .OrderBy(pagination.sidx + " " + pagination.sord)
                        .GroupBy("a.distributor_id")
                        .Select("SQL_CALC_FOUND_ROWS COUNT(total_count) AS count,SUM(total_count) AS total_count,SUM(total_amount) AS total_amount,SUM(a.total_rebate) AS total_rebate,a.distributor_name,a.distributor_id,b.address,b.company_linkname")
                        .ToJsonPage(pagination.page, pagination.rows);
                pagination.records = db.SqlQuery<int>("SELECT FOUND_ROWS() as count").FirstOrDefault();
                if (string.IsNullOrEmpty(listStr) || listStr == "[]")
                    return null;
                return listStr.ToJson();
            }
        }
        public object AttainingList(Pagination pagination, QueryTime queryTime, string distributor_id)
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
                var qable = db.Queryable<daoben_distributor_attaining_distributor>()
                        .JoinTable<daoben_distributor_attaining>((a, c) => a.main_id == c.id)
                        .Where(a => a.distributor_id == distributor_id);

                if (queryTime.startTime1 != null)
                    qable.Where<daoben_distributor_attaining>((a, c) => c.end_date >= queryTime.startTime1);
                if (queryTime.startTime2 != null)
                {
                    queryTime.startTime2 = queryTime.startTime2.ToDate().AddDays(1);
                    qable.Where<daoben_distributor_attaining>((a, c) => c.end_date < queryTime.startTime2);
                }

                string listStr = qable
                        .OrderBy(pagination.sidx + " " + pagination.sord)
                        .Select("c.id,c.name,c.start_date,c.end_date,a.total_amount,a.total_count,a.total_ratio,a.total_rebate,c.target_content,c.activity_target")
                        .ToJsonPage(pagination.page, pagination.rows, ref records);
                pagination.records = records;
                if (string.IsNullOrEmpty(listStr) || listStr == "[]")
                    return null;
                return listStr.ToJson();
            }
        }

        public MemoryStream ExportExcelAttaining(Pagination pagination, QueryTime queryTime, int? company_id, string distributor_name)
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
                var qable = db.Queryable<daoben_distributor_attaining_distributor>()
                    .JoinTable<daoben_distributor_info>((a, b) => a.distributor_id == b.id)
                    .JoinTable<daoben_distributor_attaining>((a, c) => a.main_id == c.id);

                if (myCompanyInfo.category == "事业部")
                    qable.Where<daoben_distributor_attaining>((a, c) => c.company_id_parent == myCompanyInfo.id);
                else if (myCompanyInfo.category == "分公司")
                    qable.Where<daoben_distributor_attaining>((a, c) => c.company_id == myCompanyInfo.id);
                if (company_id > 0)
                    qable.Where<daoben_distributor_attaining>((a, c) => c.company_id == company_id);

                if (queryTime.startTime1 != null)
                    qable.Where<daoben_distributor_attaining>((a, c) => c.end_date >= queryTime.startTime1);
                if (queryTime.startTime2 != null)
                {
                    queryTime.startTime2 = queryTime.startTime2.ToDate().AddDays(1);
                    qable.Where<daoben_distributor_attaining>((a, c) => c.end_date < queryTime.startTime2);
                }

                if (!string.IsNullOrEmpty(distributor_name))
                    qable.Where(a => a.distributor_name.Contains(distributor_name));


                string selStr = "b.company_linkname,b.name as name,c.name as activity_name,(CASE WHEN c.target_content=1 THEN '按实销量' ELSE '按下货量' END),DATE_FORMAT(c.start_date,'%Y-%m-%d'),DATE_FORMAT(c.end_date,'%Y-%m-%d'),a.total_rebate";
                var listDt = qable.OrderBy(pagination.sidx + " " + pagination.sord)
                        .OrderBy("a.distributor_id,c.end_date")
                        .Select(selStr).ToDataTable();
                int rowsCount = listDt.Rows.Count;

                string[] headerArr = new string[] { "所属机构", "经销商名称", "返利名称", "返利类型", "活动开始时间", "活动结束时间", "返利金额" };
                int[] colWidthArr = new int[] { 18, 25, 28, 18, 20, 20, 18 };

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

        public object GetTotalAttaining(QueryTime queryTime, int? company_id, string distributor_name)
        {
            OperatorModel LoginInfo = OperatorProvider.Provider.GetCurrent();
            if (LoginInfo == null)
                throw new Exception("用户登陆过期，请重新登录");
            CompanyInfo myCompanyInfo = LoginInfo.companyInfo;

            DateTime now = DateTime.Now;
            DateTime firstDayOfThisMonth = new DateTime(now.Year, now.Month, 1);
            using (var db = SugarDao.GetInstance())
            {
                var qable = db.Queryable<daoben_distributor_attaining_distributor>()
                       .JoinTable<daoben_distributor_attaining>((a, c) => a.main_id == c.id);

                if (myCompanyInfo.category == "事业部")
                    qable.Where<daoben_distributor_attaining>((a, c) => c.company_id_parent == myCompanyInfo.id);
                else if (myCompanyInfo.category == "分公司")
                    qable.Where<daoben_distributor_attaining>((a, c) => c.company_id == myCompanyInfo.id);
                if (company_id > 0)
                    qable.Where<daoben_distributor_attaining>((a, c) => c.company_id == company_id);

                if (queryTime.startTime1 != null)
                    qable.Where<daoben_distributor_attaining>((a, c) => c.end_date >= queryTime.startTime1);
                if (queryTime.startTime2 != null)
                {
                    queryTime.startTime2 = queryTime.startTime2.ToDate().AddDays(1);
                    qable.Where<daoben_distributor_attaining>((a, c) => c.end_date < queryTime.startTime2);
                }

                if (!string.IsNullOrEmpty(distributor_name))
                    qable.Where(a => a.distributor_name.Contains(distributor_name));

                totalInfo totalInfo = qable.Select<totalInfo>("SUM(a.total_rebate) as total_amount,count(*) as total_count")
                         .SingleOrDefault();
                if (totalInfo == null)
                    return "系统错误：无法获取总数";
                return totalInfo;
            }
        }
        #endregion

        #region 主推返利
        public object RecomMainList(Pagination pagination, QueryTime queryTime, int company_id, string distributor_name)
        {
            var LoginInfo = OperatorProvider.Provider.GetCurrent();
            if (LoginInfo == null)
                return null;
            if (pagination == null)
                pagination = new Pagination();
            pagination.page = pagination.page > 0 ? pagination.page : 1;
            pagination.rows = pagination.rows > 0 ? pagination.rows : 20;
            pagination.sidx = string.IsNullOrEmpty(pagination.sidx) ? "a.distributor_id" : pagination.sidx;
            pagination.sord = string.IsNullOrEmpty(pagination.sord) ? "desc" : pagination.sord;
            CompanyInfo myCompanyInfo = LoginInfo.companyInfo;
            PositionInfo myPositionInfo = LoginInfo.positionInfo;
            DateTime now = DateTime.Now;
            DateTime firstDayOfThisYear = new DateTime(now.Year, 1, 1);
            using (var db = SugarDao.GetInstance())
            {
                var qable = db.Queryable<daoben_distributor_recommendation_distributor>()
                    .JoinTable<daoben_distributor_info>((a, b) => b.id == a.distributor_id)
                    .JoinTable<daoben_distributor_recommendation>((a, c) => a.main_id == c.id);
                if (LoginInfo.roleId != ConstData.ROLE_ID_ADMIN)
                {
                    if (LoginInfo.roleId == ConstData.ROLE_ID_FINANCIALMANAGER)
                        qable.Where<daoben_distributor_info>((a, b) => b.company_id == myCompanyInfo.id || b.company_id_parent == myCompanyInfo.id);
                    else if (myPositionInfo.positionType <= ConstData.POSITION_OFFICE_NORMAL)
                        qable.Where<daoben_distributor_info>((a, b) => b.company_id == myCompanyInfo.id || b.company_id_parent == myCompanyInfo.id);
                    else return null;
                }
                if (queryTime.startTime1 != null)
                    qable.Where<daoben_distributor_recommendation>((a, c) => c.end_date >= queryTime.startTime1);
                if (queryTime.startTime2 != null)
                {
                    queryTime.startTime2 = queryTime.startTime2.ToDate().AddDays(1);
                    qable.Where<daoben_distributor_recommendation>((a, c) => c.end_date < queryTime.startTime2);
                }
                if (company_id > 0)
                    qable.Where<daoben_distributor_info>((a, b) => b.company_id == company_id);
                if (!string.IsNullOrEmpty(distributor_name))
                    qable.Where(a => a.distributor_name.Contains(distributor_name));
                string listStr = qable
                        .OrderBy(pagination.sidx + " " + pagination.sord)
                        .GroupBy("a.distributor_id")
                        .Select("SQL_CALC_FOUND_ROWS COUNT(total_count) AS count,SUM(total_count) AS total_count,SUM(total_amount) AS total_amount,SUM(a.total_rebate) AS total_rebate,a.distributor_name,a.distributor_id,b.address,b.company_linkname")
                        .ToJsonPage(pagination.page, pagination.rows);
                pagination.records = db.SqlQuery<int>("SELECT FOUND_ROWS() as count").FirstOrDefault();
                if (string.IsNullOrEmpty(listStr) || listStr == "[]")
                    return null;
                return listStr.ToJson();
            }
        }
        public object RecomList(Pagination pagination, QueryTime queryTime, string distributor_id)
        {
            var LoginInfo = OperatorProvider.Provider.GetCurrent();
            if (LoginInfo == null)
                return null;
            if (pagination == null)
                pagination = new Pagination();
            pagination.page = pagination.page > 0 ? pagination.page : 1;
            pagination.rows = pagination.rows > 0 ? pagination.rows : 20;
            pagination.sidx = string.IsNullOrEmpty(pagination.sidx) ? "c.name" : pagination.sidx;
            pagination.sord = string.IsNullOrEmpty(pagination.sord) ? "desc" : pagination.sord;
            CompanyInfo myCompanyInfo = LoginInfo.companyInfo;
            PositionInfo myPositionInfo = LoginInfo.positionInfo;
            DateTime now = DateTime.Now;
            DateTime firstDayOfThisYear = new DateTime(now.Year, 1, 1);
            using (var db = SugarDao.GetInstance())
            {
                var qable = db.Queryable<daoben_distributor_recommendation_distributor>()
                    .JoinTable<daoben_distributor_info>((a, b) => a.distributor_id == b.id)
                    .JoinTable<daoben_distributor_recommendation>((a, c) => a.main_id == c.id)
                    .Where(a => a.distributor_id == distributor_id);

                if (LoginInfo.roleId != ConstData.ROLE_ID_ADMIN)
                {
                    if (LoginInfo.roleId == ConstData.ROLE_ID_FINANCIALMANAGER)
                        qable.Where<daoben_distributor_info>((a, b) => b.company_id == myCompanyInfo.id || b.company_id_parent == myCompanyInfo.id);
                    else if (myPositionInfo.positionType <= ConstData.POSITION_OFFICE_NORMAL)
                        qable.Where<daoben_distributor_info>((a, b) => b.company_id == myCompanyInfo.id || b.company_id_parent == myCompanyInfo.id);
                    else return null;
                }
                if (queryTime.startTime1 != null)
                    qable.Where<daoben_distributor_recommendation>((a, c) => c.end_date >= queryTime.startTime1);
                if (queryTime.startTime2 != null)
                {
                    queryTime.startTime2 = queryTime.startTime2.ToDate().AddDays(1);
                    qable.Where<daoben_distributor_recommendation>((a, c) => c.end_date < queryTime.startTime2);
                }

                string listStr = qable
                        .OrderBy(pagination.sidx + " " + pagination.sord)
                        .Select("SQL_CALC_FOUND_ROWS c.id,c.name,c.start_date,c.end_date,a.total_amount,a.total_count,a.total_ratio,a.total_rebate,c.target_mode,c.activity_target,c.target_content")
                        .ToJsonPage(pagination.page, pagination.rows);
                pagination.records = db.SqlQuery<int>("SELECT FOUND_ROWS() as count").FirstOrDefault();
                if (string.IsNullOrEmpty(listStr) || listStr == "[]")
                    return null;
                return listStr.ToJson();
            }
        }

        public MemoryStream ExportExcelRecom(Pagination pagination, QueryTime queryTime, int? company_id, string distributor_name)
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
                var qable = db.Queryable<daoben_distributor_recommendation_distributor>()
                    .JoinTable<daoben_distributor_info>((a, b) => a.distributor_id == b.id)
                    .JoinTable<daoben_distributor_recommendation>((a, c) => a.main_id == c.id);

                if (LoginInfo.roleId != ConstData.ROLE_ID_ADMIN)
                {
                    if (LoginInfo.roleId == ConstData.ROLE_ID_FINANCIALMANAGER)
                        qable.Where<daoben_distributor_info>((a, b) => b.company_id == myCompanyInfo.id || b.company_id_parent == myCompanyInfo.id);
                    else if (myPositionInfo.positionType <= ConstData.POSITION_OFFICE_NORMAL)
                        qable.Where<daoben_distributor_info>((a, b) => b.company_id == myCompanyInfo.id || b.company_id_parent == myCompanyInfo.id);
                    else return null;
                }
                if (company_id > 0)
                    qable.Where<daoben_distributor_info>((a, b) => b.company_id == company_id || b.company_id_parent == company_id);
                if (!string.IsNullOrEmpty(distributor_name))
                    qable.Where(a => a.distributor_name.Contains(distributor_name));
                if (queryTime.startTime1 != null)
                    qable.Where<daoben_distributor_recommendation>((a, c) => c.end_date >= queryTime.startTime1);
                if (queryTime.startTime2 != null)
                {
                    queryTime.startTime2 = queryTime.startTime2.ToDate().AddDays(1);
                    qable.Where<daoben_distributor_recommendation>((a, c) => c.end_date < queryTime.startTime2);
                }


                string selStr = "b.company_linkname,b.name as name,c.name as activity_name,(CASE WHEN c.target_content=1 THEN '按实销量' ELSE '按下货量' END),DATE_FORMAT(c.start_date,'%Y-%m-%d'),DATE_FORMAT(c.end_date,'%Y-%m-%d'),a.total_rebate";
                var listDt = qable.OrderBy(pagination.sidx + " " + pagination.sord)
                        .OrderBy("a.distributor_id,c.end_date")
                        .Select(selStr).ToDataTable();
                int rowsCount = listDt.Rows.Count;

                string[] headerArr = new string[] { "所属机构", "经销商名称", "返利名称", "返利类型", "活动开始时间", "活动结束时间", "返利金额" };
                int[] colWidthArr = new int[] { 18, 25, 28, 18, 20, 20, 18 };

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

        public object GetTotalRecom(QueryTime queryTime, int? company_id, string distributor_name)
        {
            OperatorModel LoginInfo = OperatorProvider.Provider.GetCurrent();
            if (LoginInfo == null)
                throw new Exception("用户登陆过期，请重新登录");
            CompanyInfo myCompanyInfo = LoginInfo.companyInfo;

            using (var db = SugarDao.GetInstance())
            {
                var qable = db.Queryable<daoben_distributor_recommendation_distributor>()
                        .JoinTable<daoben_distributor_info>((a, b) => a.distributor_id == b.id)
                        .JoinTable<daoben_distributor_recommendation>((a, c) => a.main_id == c.id);
                if (!string.IsNullOrEmpty(distributor_name))
                    qable.Where<daoben_distributor_info>((a, b) => b.name.Contains(distributor_name));
                if (company_id > 0)
                    qable.Where<daoben_distributor_info>((a, b) => b.company_id == company_id);
                else
                {
                    qable.Where<daoben_distributor_info>((a, b) => b.company_id == myCompanyInfo.id || b.company_id_parent == myCompanyInfo.id);
                }
                if (queryTime.startTime1 != null)
                    qable.Where<daoben_distributor_recommendation>((a, c) => c.end_date >= queryTime.startTime1);
                if (queryTime.startTime2 != null)
                {
                    queryTime.startTime2 = queryTime.startTime2.ToDate().AddDays(1);
                    qable.Where<daoben_distributor_recommendation>((a, c) => c.end_date < queryTime.startTime2);
                }


                List<daoben_distributor_recommendation_distributor> totalList = qable.Select("a.*").ToList();

                totalInfo totalInfo = new totalInfo()
                {
                    startTime = queryTime.startTime1.ToDate(),
                    endTime = queryTime.startTime2.ToDate().AddDays(-1),
                };
                foreach (var a in totalList)
                {
                    totalInfo.total_reward += a.total_rebate;
                }
                totalInfo.total_activity = totalList.GroupBy(t => t.main_id).Count();

                return totalInfo;
            }
        }
        #endregion

        #region 形象返利
        public object ImageMainList(Pagination pagination, QueryTime queryTime, int company_id, string distributor_name)
        {
            var LoginInfo = OperatorProvider.Provider.GetCurrent();
            if (LoginInfo == null)
                return null;
            if (pagination == null)
                pagination = new Pagination();
            pagination.page = pagination.page > 0 ? pagination.page : 1;
            pagination.rows = pagination.rows > 0 ? pagination.rows : 20;
            pagination.sidx = string.IsNullOrEmpty(pagination.sidx) ? "a.distributor_name" : pagination.sidx;
            pagination.sord = string.IsNullOrEmpty(pagination.sord) ? "desc" : pagination.sord;
            CompanyInfo myCompanyInfo = LoginInfo.companyInfo;
            PositionInfo myPositionInfo = LoginInfo.positionInfo;

            using (var db = SugarDao.GetInstance())
            {
                var qable = db.Queryable<daoben_distributor_image>()
                        .JoinTable<daoben_distributor_info>((a, b) => b.id == a.distributor_id)
                        .JoinTable<daoben_distributor_image_res>((a, c) => c.main_id == a.id);

                if (myCompanyInfo.category == "事业部")
                    qable.Where(a => a.company_id_parent == myCompanyInfo.id);
                else if (myCompanyInfo.category == "分公司")
                    qable.Where(a => a.company_id == myCompanyInfo.id);
                if (company_id > 0)
                    qable.Where(a => a.company_id == company_id);

                if (queryTime.startTime1 != null)
                    qable.Where<daoben_distributor_image_res>((a, c) => c.end_date >= queryTime.startTime1);
                if (queryTime.startTime2 != null)
                {
                    queryTime.startTime2 = queryTime.startTime2.ToDate().AddDays(1);
                    qable.Where<daoben_distributor_image_res>((a, c) => c.end_date < queryTime.startTime2);
                }

                if (!string.IsNullOrEmpty(distributor_name))
                    qable.Where(a => a.distributor_name.Contains(distributor_name));
                string listStr = qable.OrderBy(pagination.sidx + " " + pagination.sord).GroupBy("a.distributor_id")
                        .Select("SQL_CALC_FOUND_ROWS COUNT(total_sale_count) AS count,SUM(c.total_sale_count) AS total_count,SUM(c.total_sale_amount) AS total_amount,SUM(c.rebate) AS total_rebate,a.distributor_name,a.distributor_id,b.address,a.company_name")
                        .ToJsonPage(pagination.page, pagination.rows);
                pagination.records = db.SqlQuery<int>("SELECT FOUND_ROWS() as count").FirstOrDefault();
                if (string.IsNullOrEmpty(listStr) || listStr == "[]")
                    return null;
                return listStr.ToJson();
            }
        }
        public object ImageList(Pagination pagination, QueryTime queryTime, string distributor_id)
        {
            var LoginInfo = OperatorProvider.Provider.GetCurrent();
            if (LoginInfo == null)
                return null;
            int records = 0;
            if (pagination == null)
                pagination = new Pagination();
            pagination.page = pagination.page > 0 ? pagination.page : 1;
            pagination.rows = pagination.rows > 0 ? pagination.rows : 20;
            pagination.sidx = string.IsNullOrEmpty(pagination.sidx) ? "a.start_date" : pagination.sidx;
            pagination.sord = string.IsNullOrEmpty(pagination.sord) ? "desc" : pagination.sord;
            //CompanyInfo myCompanyInfo = LoginInfo.companyInfo;
            //PositionInfo myPositionInfo = LoginInfo.positionInfo;           
            using (var db = SugarDao.GetInstance())
            {
                var qable = db.Queryable<daoben_distributor_image>()
                        .JoinTable<daoben_distributor_image_res>((a, c) => c.main_id == a.id)
                        .Where(a => a.distributor_id == distributor_id);

                if (queryTime.startTime1 != null)
                    qable.Where<daoben_distributor_image_res>((a, c) => c.end_date >= queryTime.startTime1);
                if (queryTime.startTime2 != null)
                {
                    queryTime.startTime2 = queryTime.startTime2.ToDate().AddDays(1);
                    qable.Where<daoben_distributor_image_res>((a, c) => c.end_date < queryTime.startTime2);
                }

                string listStr = qable.OrderBy(pagination.sidx + " " + pagination.sord)
                        .Select("a.id,a.name,a.start_date,a.end_date,c.start_date as pay_start,c.end_date as pay_end,c.total_sale_amount as total_amount,c.total_sale_count as total_count,c.rebate,a.target_content,a.target_mode,a.pay_mode")
                        .ToJsonPage(pagination.page, pagination.rows, ref records);
                pagination.records = records;
                if (string.IsNullOrEmpty(listStr) || listStr == "[]")
                    return null;
                return listStr.ToJson();
            }
        }

        public MemoryStream ExportExcelImage(Pagination pagination, QueryTime queryTime, int? company_id, string distributor_name)
        {
            var LoginInfo = OperatorProvider.Provider.GetCurrent();
            if (LoginInfo == null)
                return null;
            if (pagination == null)
                pagination = new Pagination();
            pagination.sidx = string.IsNullOrEmpty(pagination.sidx) ? "a.distributor_id" : pagination.sidx;
            pagination.sord = string.IsNullOrEmpty(pagination.sord) ? "desc" : pagination.sord;
            CompanyInfo myCompanyInfo = LoginInfo.companyInfo;
            PositionInfo myPositionInfo = LoginInfo.positionInfo;

            using (var db = SugarDao.GetInstance())
            {
                var qable = db.Queryable<daoben_distributor_image>()
                        .JoinTable<daoben_distributor_image_res>((a, c) => c.main_id == a.id);

                if (myCompanyInfo.category == "事业部")
                    qable.Where(a => a.company_id_parent == myCompanyInfo.id);
                else if (myCompanyInfo.category == "分公司")
                    qable.Where(a => a.company_id == myCompanyInfo.id);
                if (company_id > 0)
                    qable.Where(a => a.company_id == company_id);

                if (queryTime.startTime1 != null)
                    qable.Where<daoben_distributor_image_res>((a, c) => c.end_date >= queryTime.startTime1);
                if (queryTime.startTime2 != null)
                {
                    queryTime.startTime2 = queryTime.startTime2.ToDate().AddDays(1);
                    qable.Where<daoben_distributor_image_res>((a, c) => c.end_date < queryTime.startTime2);
                }
                if (!string.IsNullOrEmpty(distributor_name))
                    qable.Where(a => a.distributor_name.Contains(distributor_name));

                string selStr = "a.company_name,a.distributor_name,a.name as activity_name, IF(a.pay_mode=2,'按月发放','一次性发放') as pay_mode,"
                            + "IF(a.target_mode=1,'按时间返利','按销量返利') as target_mode,CONCAT(DATE_FORMAT(a.start_date,'%Y-%m-%d'),' 至 ',DATE_FORMAT(a.end_date,'%Y-%m-%d')),"
                            + "CONCAT(DATE_FORMAT(c.start_date,'%Y-%m-%d'),' 至 ',DATE_FORMAT(c.end_date,'%Y-%m-%d')),c.rebate";
                var listDt = qable.OrderBy(pagination.sidx + " " + pagination.sord)
                        .OrderBy("a.distributor_id,a.end_date")
                        .Select(selStr).ToDataTable();
                int rowsCount = listDt.Rows.Count;

                string[] headerArr = new string[] { "所属机构", "经销商名称", "返利名称", "返利发放", "返利类型", "活动时间", "返利时间", "返利总金额" };
                int[] colWidthArr = new int[] { 18, 25, 38, 16, 16, 22, 22, 18 };

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

        public object GetTotalImage(QueryTime queryTime, int? company_id, string distributor_name)
        {
            OperatorModel LoginInfo = OperatorProvider.Provider.GetCurrent();
            if (LoginInfo == null)
                throw new Exception("用户登陆过期，请重新登录");
            CompanyInfo myCompanyInfo = LoginInfo.companyInfo;

            using (var db = SugarDao.GetInstance())
            {
                var qable = db.Queryable<daoben_distributor_image>()
                        .JoinTable<daoben_distributor_image_res>((a, c) => c.main_id == a.id);

                if (myCompanyInfo.category == "事业部")
                    qable.Where(a => a.company_id_parent == myCompanyInfo.id);
                else if (myCompanyInfo.category == "分公司")
                    qable.Where(a => a.company_id == myCompanyInfo.id);
                if (company_id > 0)
                    qable.Where(a => a.company_id == company_id);

                if (queryTime.startTime1 != null)
                    qable.Where<daoben_distributor_image_res>((a, c) => c.end_date >= queryTime.startTime1);
                if (queryTime.startTime2 != null)
                {
                    queryTime.startTime2 = queryTime.startTime2.ToDate().AddDays(1);
                    qable.Where<daoben_distributor_image_res>((a, c) => c.end_date < queryTime.startTime2);
                }

                if (!string.IsNullOrEmpty(distributor_name))
                    qable.Where(a => a.distributor_name.Contains(distributor_name));

                totalInfo totalInfo = qable.Select<totalInfo>("SUM(c.rebate) as total_amount,Count(*) as total_count")
                        .SingleOrDefault();
                if (totalInfo == null)
                    return "系统错误：无法获取总数";
                return totalInfo;
            }
        }

        #endregion

    }
}
