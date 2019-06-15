using Base.Code;
using Base.Code.Security;
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
using System.Text;
using System.Web;

namespace ProjectWeb.Areas.SaleManage.Application
{
    public class SummaryApp
    {
        #region 串码汇总
        /// <summary>
        /// 串码删除（退库串码不可删，串码删除功能不对事业部及分公司开放）
        /// </summary>
        /// <param name="snList"></param>
        /// <returns></returns>
        public string SnDelete(string[] snArray)
        {
            OperatorModel LoginInfo = OperatorProvider.Provider.GetCurrent();
            if (LoginInfo == null)
                throw new Exception("用户登陆过期，请重新登录");
            if (snArray == null || snArray.Length < 1)
                return "success";
            List<string> snList = snArray.ToList<string>();
            using (var db = SugarDao.GetInstance())
            {
                try
                {
                    db.CommandTimeOut = 30;
                    db.BeginTran();
                    db.Delete<daoben_sale_outstorage>(a => snList.Contains(a.phone_sn) && a.check_status == 1);
                    db.Delete<daoben_sale_salesinfo>(a => snList.Contains(a.phone_sn) && a.check_status == 1);
                    db.Delete<daoben_sale_buyout_import_temp>(a => snList.Contains(a.phone_sn) && a.check_status == 1);
                    db.Delete<daoben_sale_exclusive_detail>(a => snList.Contains(a.phone_sn) && a.check_status == 1);
                    db.Delete<daoben_sale_refund>(a => snList.Contains(a.phone_sn));
                    db.Delete<daoben_product_sn_outlay>(a => snList.Contains(a.phone_sn));
                    db.Delete<daoben_product_sn>(a => snList.Contains(a.phone_sn) && a.sale_type > -1);
                    db.CommitTran();
                    return "success";
                }
                catch (Exception ex)
                {
                    db.RollbackTran();
                    return "系统出错：" + ex.Message;
                }
            }
        }
        public object SnMainList(Pagination pagination, QueryTime queryTime, daoben_product_sn queryInfo)
        {
            var LoginInfo = OperatorProvider.Provider.GetCurrent();
            if (LoginInfo == null)
                return null;
            if (pagination == null)
                pagination = new Pagination();
            pagination.page = pagination.page > 0 ? pagination.page : 1;
            pagination.rows = pagination.rows > 0 ? pagination.rows : 20;
            pagination.sidx = string.IsNullOrEmpty(pagination.sidx) ? "id" : pagination.sidx;
            pagination.sord = string.IsNullOrEmpty(pagination.sord) ? "desc" : pagination.sord;
            CompanyInfo myCompanyInfo = LoginInfo.companyInfo;
            PositionInfo myPositionInfo = LoginInfo.positionInfo;

            int records = 0;
            using (var db = SugarDao.GetInstance())
            {
                var qable = db.Queryable<daoben_product_sn>();
                if (myCompanyInfo.category == "事业部")
                    qable.Where(a => a.company_id_parent == myCompanyInfo.id);
                else if (myCompanyInfo.category == "分公司")
                    qable.Where(a => a.company_id == myCompanyInfo.id);
                if (queryInfo.company_id > 0)
                    qable.Where(a => a.company_id == queryInfo.company_id);

                if (queryTime != null)
                {
                    //出库时间
                    if (queryTime.startTime1 != null)
                        qable.Where(a => a.outstorage_time >= queryTime.startTime1);
                    if (queryTime.endTime1 != null)
                    {
                        queryTime.endTime1 = queryTime.endTime1.ToDate().AddDays(1);
                        qable.Where(a => a.outstorage_time < queryTime.endTime1);
                    }
                    //实销时间
                    if (queryTime.startTime2 != null)
                        qable.Where(a => a.sale_time >= queryTime.startTime2);
                    if (queryTime.endTime2 != null)
                    {
                        queryTime.endTime2 = queryTime.endTime2.ToDate().AddDays(1);
                        qable.Where(a => a.sale_time < queryTime.endTime2);
                    }
                }
                if (queryInfo != null)
                {
                    if (queryInfo.phone_sn != null)
                        qable.Where(a => a.phone_sn.Contains(queryInfo.phone_sn));
                    if (queryInfo.model != null)
                        qable.Where(a => a.model.Contains(queryInfo.model));
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

        public MemoryStream ExportExcelSn(Pagination pagination, QueryTime queryTime, daoben_product_sn queryInfo)
        {
            var LoginInfo = OperatorProvider.Provider.GetCurrent();
            if (LoginInfo == null)
                return null;
            if (pagination == null)
                pagination = new Pagination();
            pagination.sidx = string.IsNullOrEmpty(pagination.sidx) ? "id" : pagination.sidx;
            pagination.sord = string.IsNullOrEmpty(pagination.sord) ? "desc" : pagination.sord;
            CompanyInfo myCompanyInfo = LoginInfo.companyInfo;
            PositionInfo myPositionInfo = LoginInfo.positionInfo;

            using (var db = SugarDao.GetInstance())
            {
                var qable = db.Queryable<daoben_product_sn>();
                if (myCompanyInfo.category == "事业部")
                    qable.Where(a => a.company_id_parent == myCompanyInfo.id);
                else if (myCompanyInfo.category == "分公司")
                    qable.Where(a => a.company_id == myCompanyInfo.id);
                if (queryInfo.company_id > 0)
                    qable.Where(a => a.company_id == queryInfo.company_id);

                //出库时间
                if (queryTime.startTime1 != null)
                    qable.Where(a => a.outstorage_time >= queryTime.startTime1);
                if (queryTime.endTime1 != null)
                {
                    queryTime.endTime1 = queryTime.endTime1.ToDate().AddDays(1);
                    qable.Where(a => a.outstorage_time < queryTime.endTime1);
                }
                //实销时间
                if (queryTime.startTime2 != null)
                    qable.Where(a => a.sale_time >= queryTime.startTime2);
                if (queryTime.endTime2 != null)
                {
                    queryTime.endTime2 = queryTime.endTime2.ToDate().AddDays(1);
                    qable.Where(a => a.sale_time < queryTime.endTime2);
                }


                if (queryInfo.phone_sn != null)
                    qable.Where(a => a.phone_sn.Contains(queryInfo.phone_sn));
                if (queryInfo.model != null)
                    qable.Where(a => a.model.Contains(queryInfo.model));

                string selStr = "phone_sn, model, color, sale_type, price_wholesale, price_retail, refund_amount, guide_commission, outstorage_time, out_distributor, out_sales_m, out_sales, sale_time, sale_distributor, sales_m, sales, reporter";

                var listDt = qable.OrderBy(pagination.sidx + " " + pagination.sord)
                                .Select(selStr.ToString()).ToDataTable();
                int rowsCount = listDt.Rows.Count;

                string[] headerArr = new string[] { "串码", "型号", "颜色", "状态", "批发价", "实销价", "调价补差", "导购提成", "出库时间", "出库经销商", "出库业务经理", "出库业务员", "实销时间", "实销经销商", "实销业务经理", "实销业务员", "上报人" };
                int[] colWidthArr = new int[] { 20, 25, 10, 10, 10, 10, 10, 10, 15, 15, 15, 15, 15, 15, 15, 15, 15 };

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
        #endregion

        #region 经销商销量汇总
        public object DistriMainList(Pagination pagination, QueryTime queryTime, int? company_id, string distributor_name)
        {
            var LoginInfo = OperatorProvider.Provider.GetCurrent();
            if (LoginInfo == null)
                return null;
            if (pagination == null)
                pagination = new Pagination();
            pagination.page = pagination.page > 0 ? pagination.page : 1;
            pagination.rows = pagination.rows > 0 ? pagination.rows : 20;
            pagination.sidx = string.IsNullOrEmpty(pagination.sidx) ? "distributor_name" : pagination.sidx;
            pagination.sord = string.IsNullOrEmpty(pagination.sord) ? "desc" : pagination.sord;
            CompanyInfo myCompanyInfo = LoginInfo.companyInfo;
            PositionInfo myPositionInfo = LoginInfo.positionInfo;

            StringBuilder sqlStr = new StringBuilder("select SQL_CALC_FOUND_ROWS max(outstorage_count) as outstorage_count, ");
            sqlStr.Append("max(outstorage_amount) as outstorage_amount, ");
            sqlStr.Append("max(sale_count) as sale_count, max(sale_amount) as sale_amount, max(normal_count) as normal_count,");
            sqlStr.Append("max(normal_amount) as normal_amount, max(buyout_count) as buyout_count, max(buyout_amount) as buyout_amount,");
            sqlStr.Append("max(ex_count) as ex_count, max(ex_amount) as ex_amount, company_linkname, distributor_id, distributor_name from ");
            sqlStr.Append("(select count(*) as outstorage_count, sum(price_wholesale) as outstorage_amount, 0 as sale_count, 0 as sale_amount,");
            sqlStr.Append("0 as normal_count, 0 as normal_amount,0 as buyout_count, 0 as buyout_amount,0 as ex_count, 0 as ex_amount,");
            sqlStr.Append("company_linkname, out_distributor_id as distributor_id, out_distributor as distributor_name ");
            sqlStr.Append("from daoben_product_sn where  sale_type>-1 {0} group by distributor_id union ");
            sqlStr.Append("select 0 as outstorage_count, 0 as outstorage_amount, ");
            sqlStr.Append("count(if(sale_type>0,1,null) )as sale_count, sum(if(sale_type>0,price_sale,0))as sale_amount,");
            sqlStr.Append("count(if(sale_type=1,1,null)) as normal_count, sum(if(sale_type=1,price_sale,0)) as normal_amount,");
            sqlStr.Append("count(if(sale_type=2,1,null) )as buyout_count, sum(if(sale_type=2,price_sale,0)) as buyout_amount,");
            sqlStr.Append("count(if(sale_type=3,1,null)) as ex_count, sum(if(sale_type=3,price_sale,0)) as ex_amount,");
            sqlStr.Append("company_linkname, sale_distributor_id as distributor_id, sale_distributor as distributor_name ");
            sqlStr.Append("from daoben_product_sn where sale_type>0 {1} group by distributor_id) c ");
            sqlStr.Append(" group by distributor_id order by " + pagination.sidx + " " + pagination.sord);
            sqlStr.Append(" limit " + pagination.rows + " offset " + (pagination.page - 1) * pagination.rows);

            string whereStr = "";
            if (myCompanyInfo.category == "事业部")
                whereStr += " AND company_id_parent=" + myCompanyInfo.id + " ";
            else if (myCompanyInfo.category == "分公司")
                whereStr += " AND company_id=" + myCompanyInfo.id + " ";
            if (company_id > 0)
                whereStr += " AND company_id=" + company_id + " ";
            if (!string.IsNullOrEmpty(distributor_name))
                whereStr += " AND (out_distributor LIKE '%" + distributor_name + "%' OR sale_distributor LIKE '%" + distributor_name + "%') ";

            string whereStr2 = whereStr;
            if (queryTime != null)
            {
                if (queryTime.startTime1 != null)
                {
                    whereStr += " AND outstorage_time>='" + queryTime.startTime1 + "' ";
                    whereStr2 += " AND sale_time>='" + queryTime.startTime1 + "' ";

                }
                if (queryTime.startTime2 != null)
                {
                    queryTime.startTime2 = queryTime.startTime2.ToDate().AddDays(1);
                    whereStr += " AND outstorage_time<'" + queryTime.startTime2 + "' ";
                    whereStr2 += " AND sale_time<'" + queryTime.startTime2 + "' ";
                }
            }
            using (var db = SugarDao.GetInstance())
            {
                List<totalInfo> list = db.SqlQuery<totalInfo>(string.Format(sqlStr.ToString(), whereStr, whereStr2));
                pagination.records = db.SqlQuery<int>("SELECT FOUND_ROWS() as count").FirstOrDefault();
                return list;
            }
        }

        public object DistriDetailList(Pagination pagination, QueryTime queryTime, string distributor_id)
        {

            if (string.IsNullOrEmpty(distributor_id))
                return null;
            var LoginInfo = OperatorProvider.Provider.GetCurrent();
            if (LoginInfo == null)
                return null;
            int records = 0;
            if (pagination == null)
                pagination = new Pagination();
            pagination.page = pagination.page > 0 ? pagination.page : 1;
            pagination.rows = pagination.rows > 0 ? pagination.rows : 20;
            pagination.sidx = string.IsNullOrEmpty(pagination.sidx) ? "id" : pagination.sidx;
            pagination.sord = string.IsNullOrEmpty(pagination.sord) ? "desc" : pagination.sord;
            CompanyInfo myCompanyInfo = LoginInfo.companyInfo;
            PositionInfo myPositionInfo = LoginInfo.positionInfo;

            using (var db = SugarDao.GetInstance())
            {
                var qable = db.Queryable<daoben_product_sn>()
                    .Where(a => (a.out_distributor_id == distributor_id || a.sale_distributor_id == distributor_id));
                if (queryTime.startTime1 != null)
                    qable.Where(a => (a.outstorage_time >= queryTime.startTime1 || a.sale_time >= queryTime.startTime1));
                if (queryTime.startTime2 != null)
                {
                    queryTime.startTime2 = queryTime.startTime2.ToDate().AddDays(1);
                    qable.Where(a => (a.outstorage_time < queryTime.startTime2 || a.sale_time < queryTime.startTime2));
                }
                string listStr = qable
                        .Select("phone_sn,model,color,special_offer,sale_type,price_sale,sale_distributor,sale_time,price_wholesale,out_distributor,outstorage_time")
                        .OrderBy(pagination.sidx + " " + pagination.sord)
                        .ToJsonPage(pagination.page, pagination.rows, ref records);
                pagination.records = records;
                if (string.IsNullOrEmpty(listStr) || listStr == "[]")
                    return null;
                return listStr.ToJson();
            }
        }

        public object DistriGetTotalInfo(QueryTime queryTime, int? company_id, string distributor_name)
        {
            OperatorModel LoginInfo = OperatorProvider.Provider.GetCurrent();
            if (LoginInfo == null)
                throw new Exception("用户登陆过期，请重新登录");
            CompanyInfo myCompanyInfo = LoginInfo.companyInfo;
            string whereStr = "";
            if (myCompanyInfo.category == "事业部")
                whereStr += " AND company_id_parent=" + myCompanyInfo.id + " ";
            else if (myCompanyInfo.category == "分公司")
                whereStr += " AND company_id=" + myCompanyInfo.id + " ";
            if (company_id > 0)
                whereStr += " AND company_id=" + company_id + " ";

            if (queryTime.startTime1 != null)
                whereStr += " AND {0}>='" + queryTime.startTime1 + "' ";
            if (queryTime.startTime2 != null)
            {
                queryTime.startTime2 = queryTime.startTime2.ToDate().AddDays(1);
                whereStr += " AND {0}<'" + queryTime.startTime2 + "' ";
            }

            if (!string.IsNullOrEmpty(distributor_name))
                whereStr += " AND (out_distributor LIKE '%" + distributor_name + "%' OR sale_distributor LIKE '%" + distributor_name + "%') ";

            using (var db = SugarDao.GetInstance())
            {
                totalInfo totalInfo = db.Queryable<daoben_product_sn>()
                        //.JoinTable<daoben_distributor_info>((a, b) => a.out_distributor_id == b.id)
                        .Where(" sale_type>-1 " + string.Format(whereStr, "outstorage_time"))
                        .Select<totalInfo>("COUNT(*) as outstorage_count, SUM(price_wholesale) as outstorage_amount")
                        .SingleOrDefault();
                if (totalInfo == null)
                    totalInfo = new totalInfo();

                List<totalInfo> totalSaleList = db.Queryable<daoben_product_sn>()
                       //.JoinTable<daoben_distributor_info>((a, b) => a.sale_distributor_id == b.id)
                       .Where(" sale_type>0 " + string.Format(whereStr, "sale_time")).GroupBy("sale_type")
                       .Select<totalInfo>("sale_type, COUNT(*) as total_count, SUM(price_sale) as total_amount")
                       .ToList();
                if (totalSaleList == null || totalSaleList.Count == 0)
                    return totalInfo;
                foreach (totalInfo saleInfo in totalSaleList)
                {
                    if (saleInfo.sale_type == 1)
                    {
                        totalInfo.normal_count = saleInfo.total_count;
                        totalInfo.normal_amount = saleInfo.total_amount;
                    }
                    else if (saleInfo.sale_type == 2)
                    {
                        totalInfo.buyout_count = saleInfo.total_count;
                        totalInfo.buyout_amount = saleInfo.total_amount;
                    }
                    else
                    {
                        totalInfo.ex_count = saleInfo.total_count;
                        totalInfo.ex_amount = saleInfo.total_amount;
                    }
                    totalInfo.sale_count += saleInfo.total_count;
                    totalInfo.sale_amount += saleInfo.total_amount;
                }
                return totalInfo;

            }
        }

        public MemoryStream ExportExcelDistriDetail(Pagination pagination, QueryTime queryTime, int? company_id, string distributor_name)
        {
            var LoginInfo = OperatorProvider.Provider.GetCurrent();
            if (LoginInfo == null)
                return null;
            if (pagination == null)
                pagination = new Pagination();
            //pagination.sidx = string.IsNullOrEmpty(pagination.sidx) ? "out_distributor" : pagination.sidx;
            //pagination.sord = string.IsNullOrEmpty(pagination.sord) ? "desc" : pagination.sord;
            CompanyInfo myCompanyInfo = LoginInfo.companyInfo;
            PositionInfo myPositionInfo = LoginInfo.positionInfo;

            using (var db = SugarDao.GetInstance())
            {
                var qable = db.Queryable<daoben_product_sn>();

                if (myCompanyInfo.category == "事业部")
                    qable.Where(a => a.company_id_parent == myCompanyInfo.id);
                else if (myCompanyInfo.category == "分公司")
                    qable.Where(a => a.company_id == myCompanyInfo.id);
                if (company_id > 0)
                    qable.Where(a => a.company_id == company_id);

                if (queryTime.startTime1 != null)
                    qable.Where(a => (a.outstorage_time >= queryTime.startTime1 || a.sale_time >= queryTime.startTime1));
                if (queryTime.startTime2 != null)
                {
                    queryTime.startTime2 = queryTime.startTime2.ToDate().AddDays(1);
                    qable.Where(a => (a.outstorage_time < queryTime.startTime2 || a.sale_time < queryTime.startTime2));
                }

                if (!string.IsNullOrEmpty(distributor_name))
                    qable.Where(a => (a.out_distributor.Contains(distributor_name) || a.sale_distributor.Contains(distributor_name)));


                var listDt = qable.OrderBy("out_distributor,sale_distributor desc")
                                .Select("phone_sn,model,color,(CASE WHEN sale_type=0 THEN '已出库' WHEN sale_type=1 THEN '正常销售' WHEN sale_type=2 THEN '买断' WHEN sale_type=3 THEN '包销' WHEN sale_type=-1 THEN '退库' ELSE '-' END),price_wholesale,out_distributor,outstorage_time,price_sale,sale_distributor,sale_time")
                                .ToDataTable();
                int rowsCount = listDt.Rows.Count;

                string[] headerArr = new string[] { "串码", "型号", "颜色", "销售状态", "出库价格", "出库经销商", "出库时间", "实销价格", "实销经销商", "实销时间" };
                int[] colWidthArr = new int[] { 35, 25, 18, 15, 18, 40, 18, 18, 40, 18 };

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
        #endregion

        #region 业务员销量汇总
        public object SalesMainList(Pagination pagination, QueryTime queryTime, int? company_id, string emp_name)
        {
            var LoginInfo = OperatorProvider.Provider.GetCurrent();
            if (LoginInfo == null)
                return null;
            if (pagination == null)
                pagination = new Pagination();
            pagination.page = pagination.page > 0 ? pagination.page : 1;
            pagination.rows = pagination.rows > 0 ? pagination.rows : 20;
            pagination.sidx = string.IsNullOrEmpty(pagination.sidx) ? "emp_name" : pagination.sidx;
            pagination.sord = string.IsNullOrEmpty(pagination.sord) ? "desc" : pagination.sord;
            CompanyInfo myCompanyInfo = LoginInfo.companyInfo;
            PositionInfo myPositionInfo = LoginInfo.positionInfo;

            StringBuilder sqlStr = new StringBuilder("select SQL_CALC_FOUND_ROWS max(outstorage_count) as outstorage_count,");
            sqlStr.Append("max(outstorage_amount) as outstorage_amount, ");
            sqlStr.Append("max(sale_count) as sale_count, max(sale_amount) as sale_amount, max(normal_count) as normal_count,");
            sqlStr.Append("max(normal_amount) as normal_amount, max(buyout_count) as buyout_count, max(buyout_amount) as buyout_amount,");
            sqlStr.Append("max(ex_count) as ex_count, max(ex_amount) as ex_amount, company_linkname, emp_id, emp_name from ");
            sqlStr.Append("(select count(*) as outstorage_count, sum(price_wholesale) as outstorage_amount, 0 as sale_count, 0 as sale_amount,");
            sqlStr.Append("0 as normal_count, 0 as normal_amount,0 as buyout_count, 0 as buyout_amount,0 as ex_count, 0 as ex_amount,");
            sqlStr.Append("company_linkname, out_sales_id as emp_id, out_sales as emp_name ");
            sqlStr.Append("from daoben_product_sn where  sale_type>-1 {0} group by emp_id union ");
            sqlStr.Append("select 0 as outstorage_count, 0 as outstorage_amount, ");
            sqlStr.Append("count(if(sale_type>0,1,null) )as sale_count, sum(if(sale_type>0,price_sale,0))as sale_amount,");
            sqlStr.Append("count(if(sale_type=1,1,null)) as normal_count, sum(if(sale_type=1,price_sale,0)) as normal_amount,");
            sqlStr.Append("count(if(sale_type=2,1,null) )as buyout_count, sum(if(sale_type=2,price_sale,0)) as buyout_amount,");
            sqlStr.Append("count(if(sale_type=3,1,null)) as ex_count, sum(if(sale_type=3,price_sale,0)) as ex_amount,");
            sqlStr.Append("company_linkname, sales_id as emp_id, sales as emp_name ");
            sqlStr.Append("from daoben_product_sn where sale_type>0 {1} group by emp_id) c ");
            sqlStr.Append(" group by emp_id order by " + pagination.sidx + " " + pagination.sord);
            sqlStr.Append(" limit " + pagination.rows + " offset " + (pagination.page - 1) * pagination.rows);


            string whereStr = "";
            if (myCompanyInfo.category == "事业部")
                whereStr += " AND company_id_parent=" + myCompanyInfo.id + " ";
            else if (myCompanyInfo.category == "分公司")
                whereStr += " AND company_id=" + myCompanyInfo.id + " ";
            if (company_id > 0)
                whereStr += " AND company_id=" + company_id + " ";
            if (!string.IsNullOrEmpty(emp_name))
                whereStr += " AND (out_sales LIKE '%" + emp_name + "%' OR sales LIKE '%" + emp_name + "%') ";

            string whereStr2 = whereStr;
            if (queryTime != null)
            {
                if (queryTime.startTime1 != null)
                {
                    whereStr += " AND outstorage_time>='" + queryTime.startTime1 + "' ";
                    whereStr2 += " AND sale_time>='" + queryTime.startTime1 + "' ";

                }
                if (queryTime.startTime2 != null)
                {
                    queryTime.startTime2 = queryTime.startTime2.ToDate().AddDays(1);
                    whereStr += " AND outstorage_time<'" + queryTime.startTime2 + "' ";
                    whereStr2 += " AND sale_time<'" + queryTime.startTime2 + "' ";

                }
            }
            using (var db = SugarDao.GetInstance())
            {
                List<totalInfo> list = db.SqlQuery<totalInfo>(string.Format(sqlStr.ToString(), whereStr, whereStr2));
                pagination.records = db.SqlQuery<int>("SELECT FOUND_ROWS() as count").FirstOrDefault();
                return list;
            }
        }

        public object SalesDetailList(Pagination pagination, QueryTime queryTime, string emp_id)
        {
            if (string.IsNullOrEmpty(emp_id))
                return null;
            var LoginInfo = OperatorProvider.Provider.GetCurrent();
            if (LoginInfo == null)
                return null;
            int records = 0;
            if (pagination == null)
                pagination = new Pagination();
            pagination.page = pagination.page > 0 ? pagination.page : 1;
            pagination.rows = pagination.rows > 0 ? pagination.rows : 20;
            pagination.sidx = string.IsNullOrEmpty(pagination.sidx) ? "id" : pagination.sidx;
            pagination.sord = string.IsNullOrEmpty(pagination.sord) ? "desc" : pagination.sord;
            CompanyInfo myCompanyInfo = LoginInfo.companyInfo;
            PositionInfo myPositionInfo = LoginInfo.positionInfo;

            using (var db = SugarDao.GetInstance())
            {
                var qable = db.Queryable<daoben_product_sn>()
                    .Where(a => (a.out_sales_id == emp_id || a.sales_id == emp_id));
                if (queryTime.startTime1 != null)
                    qable.Where(a => (a.outstorage_time >= queryTime.startTime1 || a.sale_time >= queryTime.startTime1));
                if (queryTime.startTime2 != null)
                {
                    queryTime.startTime2 = queryTime.startTime2.ToDate().AddDays(1);
                    qable.Where(a => (a.outstorage_time < queryTime.startTime2 || a.sale_time < queryTime.startTime2));
                }
                string listStr = qable
                       .Select("phone_sn,model,color,special_offer,high_level,sale_type,price_sale,sales,sale_time,price_wholesale,out_sales,outstorage_time")
                       .OrderBy(pagination.sidx + " " + pagination.sord)
                       .ToJsonPage(pagination.page, pagination.rows, ref records);
                pagination.records = records;
                if (string.IsNullOrEmpty(listStr) || listStr == "[]")
                    return null;
                return listStr.ToJson();
            }
        }
        public object SalesGetTotalInfo(QueryTime queryTime, int? company_id, string emp_name)
        {
            OperatorModel LoginInfo = OperatorProvider.Provider.GetCurrent();
            if (LoginInfo == null)
                throw new Exception("用户登陆过期，请重新登录");
            CompanyInfo myCompanyInfo = LoginInfo.companyInfo;
            string whereStr = "";
            if (myCompanyInfo.category == "事业部")
                whereStr += " AND company_id_parent=" + myCompanyInfo.id + " ";
            else if (myCompanyInfo.category == "分公司")
                whereStr += " AND company_id=" + myCompanyInfo.id + " ";
            if (company_id > 0)
                whereStr += " AND company_id=" + company_id + " ";

            if (queryTime.startTime1 != null)
                whereStr += " AND {0}>='" + queryTime.startTime1 + "' ";
            if (queryTime.startTime2 != null)
            {
                queryTime.startTime2 = queryTime.startTime2.ToDate().AddDays(1);
                whereStr += " AND {0}<'" + queryTime.startTime2 + "' ";
            }

            if (!string.IsNullOrEmpty(emp_name))
                whereStr += " AND (out_sales LIKE '%" + emp_name + "%' OR sales LIKE '%" + emp_name + "%') ";

            using (var db = SugarDao.GetInstance())
            {
                totalInfo totalInfo = db.Queryable<daoben_product_sn>()
                        .Where(" sale_type>-1 " + string.Format(whereStr, "outstorage_time"))
                        .Select<totalInfo>("COUNT(*) as outstorage_count, SUM(price_wholesale) as outstorage_amount")
                        .SingleOrDefault();
                if (totalInfo == null)
                    totalInfo = new totalInfo();

                List<totalInfo> totalSaleList = db.Queryable<daoben_product_sn>()
                       .Where(" sale_type>0 " + string.Format(whereStr, "sale_time")).GroupBy("sale_type")
                       .Select<totalInfo>("sale_type, COUNT(*) as total_count, SUM(price_sale) as total_amount")
                       .ToList();
                if (totalSaleList == null || totalSaleList.Count == 0)
                    return totalInfo;
                foreach (totalInfo saleInfo in totalSaleList)
                {
                    if (saleInfo.sale_type == 1)
                    {
                        totalInfo.normal_count = saleInfo.total_count;
                        totalInfo.normal_amount = saleInfo.total_amount;
                    }
                    else if (saleInfo.sale_type == 2)
                    {
                        totalInfo.buyout_count = saleInfo.total_count;
                        totalInfo.buyout_amount = saleInfo.total_amount;
                    }
                    else
                    {
                        totalInfo.ex_count = saleInfo.total_count;
                        totalInfo.ex_amount = saleInfo.total_amount;
                    }
                    totalInfo.sale_count += saleInfo.total_count;
                    totalInfo.sale_amount += saleInfo.total_amount;
                }
                return totalInfo;

            }
        }
        public MemoryStream ExportExcelSalesDetail(Pagination pagination, QueryTime queryTime, int? company_id, string emp_name)
        {
            var LoginInfo = OperatorProvider.Provider.GetCurrent();
            if (LoginInfo == null)
                return null;
            if (pagination == null)
                pagination = new Pagination();
            //pagination.sidx = string.IsNullOrEmpty(pagination.sidx) ? "out_sales" : pagination.sidx;
            //pagination.sord = string.IsNullOrEmpty(pagination.sord) ? "desc" : pagination.sord;
            CompanyInfo myCompanyInfo = LoginInfo.companyInfo;
            PositionInfo myPositionInfo = LoginInfo.positionInfo;

            using (var db = SugarDao.GetInstance())
            {
                var qable = db.Queryable<daoben_product_sn>();

                if (myCompanyInfo.category == "事业部")
                    qable.Where(a => a.company_id_parent == myCompanyInfo.id);
                else if (myCompanyInfo.category == "分公司")
                    qable.Where(a => a.company_id == myCompanyInfo.id);
                if (company_id > 0)
                    qable.Where(a => a.company_id == company_id);

                if (queryTime.startTime1 != null)
                    qable.Where(a => (a.outstorage_time >= queryTime.startTime1 || a.sale_time >= queryTime.startTime1));
                if (queryTime.startTime2 != null)
                {
                    queryTime.startTime2 = queryTime.startTime2.ToDate().AddDays(1);
                    qable.Where(a => (a.outstorage_time < queryTime.startTime2 || a.sale_time < queryTime.startTime2));
                }

                if (!string.IsNullOrEmpty(emp_name))
                    qable.Where(a => (a.out_sales.Contains(emp_name) || a.sales.Contains(emp_name)));

                StringBuilder selStr = new StringBuilder("phone_sn,model,color,");
                selStr.Append("(CASE WHEN sale_type=0 THEN '已出库' WHEN sale_type=1 THEN '正常销售' WHEN sale_type=2 THEN '买断' WHEN sale_type=3 THEN '包销' WHEN sale_type=-1 THEN '退库' ELSE '-' END),");
                selStr.Append("IF(special_offer=1,'是','否'),IF(high_level=1,'是','否'),price_wholesale,out_sales,outstorage_time,price_sale,sales,sale_time");

                var listDt = qable.OrderBy("out_sales,sales desc")
                                .Select(selStr.ToString())
                                .ToDataTable();

                int rowsCount = listDt.Rows.Count;

                string[] headerArr = new string[] { "串码", "型号", "颜色", "销售状态", "特价机", "高端机", "出库价格", "出库业务员", "出库时间", "实销价格", "实销业务员", "实销时间" };
                int[] colWidthArr = new int[] { 35, 25, 18, 15, 15, 15, 18, 30, 18, 18, 30, 18 };

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
        #endregion

        #region 业务经理销量汇总
        public object SalesManagerMainList(Pagination pagination, QueryTime queryTime, int? company_id, string emp_name)
        {
            var LoginInfo = OperatorProvider.Provider.GetCurrent();
            if (LoginInfo == null)
                return null;
            if (pagination == null)
                pagination = new Pagination();
            pagination.page = pagination.page > 0 ? pagination.page : 1;
            pagination.rows = pagination.rows > 0 ? pagination.rows : 20;
            pagination.sidx = string.IsNullOrEmpty(pagination.sidx) ? "emp_name" : pagination.sidx;
            pagination.sord = string.IsNullOrEmpty(pagination.sord) ? "desc" : pagination.sord;
            CompanyInfo myCompanyInfo = LoginInfo.companyInfo;
            PositionInfo myPositionInfo = LoginInfo.positionInfo;

            StringBuilder sqlStr = new StringBuilder("select SQL_CALC_FOUND_ROWS max(outstorage_count) as outstorage_count,");
            sqlStr.Append("max(outstorage_amount) as outstorage_amount, ");
            sqlStr.Append("max(sale_count) as sale_count, max(sale_amount) as sale_amount, max(normal_count) as normal_count,");
            sqlStr.Append("max(normal_amount) as normal_amount, max(buyout_count) as buyout_count, max(buyout_amount) as buyout_amount,");
            sqlStr.Append("max(ex_count) as ex_count, max(ex_amount) as ex_amount, company_linkname, emp_id, emp_name from ");
            sqlStr.Append("(select count(*) as outstorage_count, sum(price_wholesale) as outstorage_amount, 0 as sale_count, 0 as sale_amount,");
            sqlStr.Append("0 as normal_count, 0 as normal_amount,0 as buyout_count, 0 as buyout_amount,0 as ex_count, 0 as ex_amount,");
            sqlStr.Append("company_linkname, out_sales_m_id as emp_id, out_sales_m as emp_name from daoben_product_sn ");
            sqlStr.Append(" where  sale_type>-1 and out_sales_m_id is not null and out_sales_m_id !='' {0} group by emp_id union ");
            sqlStr.Append("select 0 as outstorage_count, 0 as outstorage_amount, ");
            sqlStr.Append("count(if(sale_type>0,1,null) )as sale_count, sum(if(sale_type>0,price_sale,0))as sale_amount,");
            sqlStr.Append("count(if(sale_type=1,1,null)) as normal_count, sum(if(sale_type=1,price_sale,0)) as normal_amount,");
            sqlStr.Append("count(if(sale_type=2,1,null) )as buyout_count, sum(if(sale_type=2,price_sale,0)) as buyout_amount,");
            sqlStr.Append("count(if(sale_type=3,1,null)) as ex_count, sum(if(sale_type=3,price_sale,0)) as ex_amount,");
            sqlStr.Append("company_linkname, sales_m_id as emp_id, sales_m as emp_name from daoben_product_sn ");
            sqlStr.Append(" where sale_type>0 and sales_m_id is not null and sales_m_id !=''{1} group by emp_id) c ");
            sqlStr.Append(" group by emp_id order by " + pagination.sidx + " " + pagination.sord);
            sqlStr.Append(" limit " + pagination.rows + " offset " + (pagination.page - 1) * pagination.rows);


            string whereStr = "";
            if (myCompanyInfo.category == "事业部")
                whereStr += " AND company_id_parent=" + myCompanyInfo.id + " ";
            else if (myCompanyInfo.category == "分公司")
                whereStr += " AND company_id=" + myCompanyInfo.id + " ";
            if (company_id > 0)
                whereStr += " AND company_id=" + company_id + " ";
            if (!string.IsNullOrEmpty(emp_name))
                whereStr += " AND (out_sales_m LIKE '%" + emp_name + "%' OR sales_m LIKE '%" + emp_name + "%') ";
            string whereStr2 = whereStr;

            if (queryTime != null)
            {
                if (queryTime.startTime1 != null)
                {
                    whereStr += " AND outstorage_time>='" + queryTime.startTime1 + "' ";
                    whereStr2 += " AND sale_time>='" + queryTime.startTime1 + "' ";

                }
                if (queryTime.startTime2 != null)
                {
                    queryTime.startTime2 = queryTime.startTime2.ToDate().AddDays(1);
                    whereStr += " AND outstorage_time<'" + queryTime.startTime2 + "' ";
                    whereStr2 += " AND sale_time<'" + queryTime.startTime2 + "' ";

                }
            }
            using (var db = SugarDao.GetInstance())
            {
                List<totalInfo> list = db.SqlQuery<totalInfo>(string.Format(sqlStr.ToString(), whereStr, whereStr2));
                pagination.records = db.SqlQuery<int>("SELECT FOUND_ROWS() as count").FirstOrDefault();
                return list;
            }
        }
        public object SalesManagerDetailList(Pagination pagination, QueryTime queryTime, string emp_id)
        {
            if (string.IsNullOrEmpty(emp_id))
                return null;
            var LoginInfo = OperatorProvider.Provider.GetCurrent();
            if (LoginInfo == null)
                return null;
            int records = 0;
            if (pagination == null)
                pagination = new Pagination();
            pagination.page = pagination.page > 0 ? pagination.page : 1;
            pagination.rows = pagination.rows > 0 ? pagination.rows : 20;
            pagination.sidx = string.IsNullOrEmpty(pagination.sidx) ? "id" : pagination.sidx;
            pagination.sord = string.IsNullOrEmpty(pagination.sord) ? "desc" : pagination.sord;
            CompanyInfo myCompanyInfo = LoginInfo.companyInfo;
            PositionInfo myPositionInfo = LoginInfo.positionInfo;
            using (var db = SugarDao.GetInstance())
            {
                var qable = db.Queryable<daoben_product_sn>()
                   .Where(a => (a.out_sales_m_id == emp_id || a.sales_m_id == emp_id));
                if (queryTime.startTime1 != null)
                    qable.Where(a => (a.outstorage_time >= queryTime.startTime1 || a.sale_time >= queryTime.startTime1));
                if (queryTime.startTime2 != null)
                {
                    queryTime.startTime2 = queryTime.startTime2.ToDate().AddDays(1);
                    qable.Where(a => (a.outstorage_time < queryTime.startTime2 || a.sale_time < queryTime.startTime2));
                }
                string listStr = qable
                       .Select("phone_sn,model,color,special_offer,high_level,sale_type,price_sale,sales_m,sale_time,price_wholesale,out_sales_m,outstorage_time")
                       .OrderBy(pagination.sidx + " " + pagination.sord)
                       .ToJsonPage(pagination.page, pagination.rows, ref records);
                pagination.records = records;
                if (string.IsNullOrEmpty(listStr) || listStr == "[]")
                    return null;
                return listStr.ToJson();
            }
        }
        public object SalesManagerGetTotalInfo(QueryTime queryTime, int? company_id, string emp_name)
        {
            OperatorModel LoginInfo = OperatorProvider.Provider.GetCurrent();
            if (LoginInfo == null)
                throw new Exception("用户登陆过期，请重新登录");
            CompanyInfo myCompanyInfo = LoginInfo.companyInfo;
            string whereStr = "";
            if (myCompanyInfo.category == "事业部")
                whereStr += " AND company_id_parent=" + myCompanyInfo.id + " ";
            else if (myCompanyInfo.category == "分公司")
                whereStr += " AND company_id=" + myCompanyInfo.id + " ";
            if (company_id > 0)
                whereStr += " AND company_id=" + company_id + " ";

            if (queryTime.startTime1 != null)
                whereStr += " AND {0}>='" + queryTime.startTime1 + "' ";
            if (queryTime.startTime2 != null)
            {
                queryTime.startTime2 = queryTime.startTime2.ToDate().AddDays(1);
                whereStr += " AND {0}<'" + queryTime.startTime2 + "' ";
            }

            if (!string.IsNullOrEmpty(emp_name))
                whereStr += " AND (out_sales_m LIKE '%" + emp_name + "%' OR sales_m LIKE '%" + emp_name + "%') ";

            using (var db = SugarDao.GetInstance())
            {
                totalInfo totalInfo = db.Queryable<daoben_product_sn>()
                        .Where(" sale_type>-1 " + string.Format(whereStr, "outstorage_time"))
                        .Select<totalInfo>("COUNT(*) as outstorage_count, SUM(price_wholesale) as outstorage_amount")
                        .SingleOrDefault();
                if (totalInfo == null)
                    totalInfo = new totalInfo();

                List<totalInfo> totalSaleList = db.Queryable<daoben_product_sn>()
                       .Where(" sale_type>0 " + string.Format(whereStr, "sale_time")).GroupBy("sale_type")
                       .Select<totalInfo>("sale_type, COUNT(*) as total_count, SUM(price_sale) as total_amount")
                       .ToList();
                if (totalSaleList == null || totalSaleList.Count == 0)
                    return totalInfo;
                foreach (totalInfo saleInfo in totalSaleList)
                {
                    if (saleInfo.sale_type == 1)
                    {
                        totalInfo.normal_count = saleInfo.total_count;
                        totalInfo.normal_amount = saleInfo.total_amount;
                    }
                    else if (saleInfo.sale_type == 2)
                    {
                        totalInfo.buyout_count = saleInfo.total_count;
                        totalInfo.buyout_amount = saleInfo.total_amount;
                    }
                    else
                    {
                        totalInfo.ex_count = saleInfo.total_count;
                        totalInfo.ex_amount = saleInfo.total_amount;
                    }
                    totalInfo.sale_count += saleInfo.total_count;
                    totalInfo.sale_amount += saleInfo.total_amount;
                }
                return totalInfo;

            }
        }
        public MemoryStream ExportExcelSalesManagerDetail(Pagination pagination, QueryTime queryTime, int? company_id, string emp_name)
        {
            var LoginInfo = OperatorProvider.Provider.GetCurrent();
            if (LoginInfo == null)
                return null;
            if (pagination == null)
                pagination = new Pagination();
            //pagination.sidx = string.IsNullOrEmpty(pagination.sidx) ? "out_sales_m" : pagination.sidx;
            //pagination.sord = string.IsNullOrEmpty(pagination.sord) ? "desc" : pagination.sord;
            CompanyInfo myCompanyInfo = LoginInfo.companyInfo;
            PositionInfo myPositionInfo = LoginInfo.positionInfo;

            using (var db = SugarDao.GetInstance())
            {
                var qable = db.Queryable<daoben_product_sn>();

                if (myCompanyInfo.category == "事业部")
                    qable.Where(a => a.company_id_parent == myCompanyInfo.id);
                else if (myCompanyInfo.category == "分公司")
                    qable.Where(a => a.company_id == myCompanyInfo.id);
                if (company_id > 0)
                    qable.Where(a => a.company_id == company_id);

                if (queryTime.startTime1 != null)
                    qable.Where(a => (a.outstorage_time >= queryTime.startTime1 || a.sale_time >= queryTime.startTime1));
                if (queryTime.startTime2 != null)
                {
                    queryTime.startTime2 = queryTime.startTime2.ToDate().AddDays(1);
                    qable.Where(a => (a.outstorage_time < queryTime.startTime2 || a.sale_time < queryTime.startTime2));
                }

                if (!string.IsNullOrEmpty(emp_name))
                    qable.Where(a => (a.out_sales_m.Contains(emp_name) || a.sales_m.Contains(emp_name)));

                StringBuilder selStr = new StringBuilder("phone_sn,model,color,");
                selStr.Append("(CASE WHEN sale_type=0 THEN '已出库' WHEN sale_type=1 THEN '正常销售' WHEN sale_type=2 THEN '买断' WHEN sale_type=3 THEN '包销' WHEN sale_type=-1 THEN '退库' ELSE '-' END),");
                selStr.Append("IF(special_offer=1,'是','否'),IF(high_level=1,'是','否'),price_wholesale,out_sales_m,outstorage_time,price_sale,sales_m,sale_time");

                var listDt = qable.OrderBy("out_sales_m,sales_m desc")
                                .Select(selStr.ToString())
                                .ToDataTable();

                int rowsCount = listDt.Rows.Count;

                string[] headerArr = new string[] { "串码", "型号", "颜色", "销售状态", "特价机", "高端机", "出库价格", "出库业务经理", "出库时间", "实销价格", "实销业务经理", "实销时间" };
                int[] colWidthArr = new int[] { 35, 25, 18, 15, 15, 15, 18, 30, 18, 18, 30, 18 };

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

        #endregion

        #region 导购销量汇总
        public object GuideMainList(Pagination pagination, QueryTime queryTime, int? company_id, string emp_name, string emp_id)
        {
            var LoginInfo = OperatorProvider.Provider.GetCurrent();
            if (LoginInfo == null)
                return null;
            if (pagination == null)
                pagination = new Pagination();
            pagination.page = pagination.page > 0 ? pagination.page : 1;
            pagination.rows = pagination.rows > 0 ? pagination.rows : 20;
            pagination.sidx = string.IsNullOrEmpty(pagination.sidx) ? "emp_id" : pagination.sidx;
            pagination.sord = string.IsNullOrEmpty(pagination.sord) ? "desc" : pagination.sord;
            CompanyInfo myCompanyInfo = LoginInfo.companyInfo;
            PositionInfo myPositionInfo = LoginInfo.positionInfo;

            StringBuilder selStr = new StringBuilder("SQL_CALC_FOUND_ROWS reporter_id AS emp_id,reporter AS emp_name,company_linkname,");

            selStr.Append(" SUM(if(sale_type>0 , guide_commission, 0)) AS total_commission, ");
            selStr.Append(" COUNT(if(sale_type>0,1,null)) AS total_count,");
            selStr.Append(" SUM(if(sale_type>0 ,price_sale, 0)) AS total_amount, ");

            selStr.Append(" SUM(if(sale_type=1 , guide_commission, 0)) AS normal_commission, ");
            selStr.Append(" COUNT(if(sale_type=1,1,null)) AS normal_count,");
            selStr.Append(" SUM(if(sale_type=1 ,price_sale, 0)) AS normal_amount, ");

            selStr.Append(" SUM(if(sale_type=2 , guide_commission, 0)) AS buyout_commission, ");
            selStr.Append(" COUNT(if(sale_type=2,1,null))AS buyout_count,");
            selStr.Append(" SUM(if(sale_type=2 ,price_sale, 0)) AS buyout_amount, ");

            selStr.Append(" SUM(if(sale_type=3 , guide_commission, 0)) AS ex_commission, ");
            selStr.Append(" COUNT(if(sale_type=3,1,null)) AS ex_count,");
            selStr.Append(" SUM(if(sale_type=3 ,price_sale, 0)) AS ex_amount ");

            using (var db = SugarDao.GetInstance())
            {
                var qable = db.Queryable<daoben_product_sn>().Where(a => a.reporter_type == 1);
                if (myCompanyInfo.category == "事业部")
                    qable.Where(a => a.company_id_parent == myCompanyInfo.id);
                else if (myCompanyInfo.category == "分公司")
                    qable.Where(a => a.company_id == myCompanyInfo.id);
                if (company_id > 0)
                    qable.Where(a => a.company_id == company_id);

                if (queryTime.startTime1 != null)
                    qable.Where(a => (a.sale_time >= queryTime.startTime1));

                if (queryTime.startTime2 != null)
                {
                    queryTime.startTime2 = queryTime.startTime2.ToDate().AddDays(1);
                    qable.Where(a => (a.sale_time < queryTime.startTime2));
                }
                if (!string.IsNullOrEmpty(emp_id))
                    qable.Where(a => a.reporter_id.Equals(emp_id));
                else if (!string.IsNullOrEmpty(emp_name))
                    qable.Where(a => a.reporter.Contains(emp_name));

                string listStr = qable
                        .OrderBy(pagination.sidx + " " + pagination.sord)
                        .GroupBy(a => a.reporter_id)
                        .Select(selStr.ToString())
                        .ToJsonPage(pagination.page, pagination.rows);
                pagination.records = db.SqlQuery<int>("SELECT FOUND_ROWS() as count").FirstOrDefault();
                if (string.IsNullOrEmpty(listStr) || listStr == "[]")
                    return null;
                return listStr.ToJson();
            }
        }

        public object GuideDetailList(Pagination pagination, QueryTime queryTime, string emp_id)
        {
            if (string.IsNullOrEmpty(emp_id))
                return null;
            var LoginInfo = OperatorProvider.Provider.GetCurrent();
            if (LoginInfo == null)
                return null;
            int records = 0;
            if (pagination == null)
                pagination = new Pagination();
            pagination.page = pagination.page > 0 ? pagination.page : 1;
            pagination.rows = pagination.rows > 0 ? pagination.rows : 20;
            pagination.sidx = string.IsNullOrEmpty(pagination.sidx) ? "sale_time" : pagination.sidx;
            pagination.sord = string.IsNullOrEmpty(pagination.sord) ? "desc" : pagination.sord;
            CompanyInfo myCompanyInfo = LoginInfo.companyInfo;
            PositionInfo myPositionInfo = LoginInfo.positionInfo;

            using (var db = SugarDao.GetInstance())
            {
                var qable = db.Queryable<daoben_product_sn>().Where(a => (a.reporter_id == emp_id));

                if (queryTime.startTime1 != null)
                    qable.Where(a => (a.sale_time >= queryTime.startTime1));
                if (queryTime.startTime2 != null)
                {
                    queryTime.startTime2 = queryTime.startTime2.ToDate().AddDays(1);
                    qable.Where(a => (a.sale_time < queryTime.startTime2));
                }

                string listStr = qable
                       .Select("phone_sn,model,color,special_offer,high_level,sale_type,guide_commission,price_wholesale,price_sale,sales_m,sale_time")
                       .OrderBy(pagination.sidx + " " + pagination.sord)
                       .ToJsonPage(pagination.page, pagination.rows, ref records);
                pagination.records = records;
                if (string.IsNullOrEmpty(listStr) || listStr == "[]")
                    return null;
                return listStr.ToJson();
            }
        }

        public object GuideGetTotalInfo(QueryTime queryTime, int? company_id, string emp_name, string emp_id)
        {
            OperatorModel LoginInfo = OperatorProvider.Provider.GetCurrent();
            if (LoginInfo == null)
                throw new Exception("用户登陆过期，请重新登录");
            CompanyInfo myCompanyInfo = LoginInfo.companyInfo;
            using (var db = SugarDao.GetInstance())
            {
                var qable = db.Queryable<daoben_product_sn>().Where(a => a.reporter_type == 1);
                if (myCompanyInfo.category == "事业部")
                    qable.Where(a => a.company_id_parent == myCompanyInfo.id);
                else if (myCompanyInfo.category == "分公司")
                    qable.Where(a => a.company_id == myCompanyInfo.id);
                if (company_id > 0)
                    qable.Where(a => a.company_id == company_id);

                if (queryTime.startTime1 != null)
                    qable.Where(a => (a.sale_time >= queryTime.startTime1));

                if (queryTime.startTime2 != null)
                {
                    queryTime.startTime2 = queryTime.startTime2.ToDate().AddDays(1);
                    qable.Where(a => (a.sale_time < queryTime.startTime2));
                }
                if (!string.IsNullOrEmpty(emp_id))
                    qable.Where(a => a.reporter_id.Equals(emp_id));
                else if (!string.IsNullOrEmpty(emp_name))
                    qable.Where(a => a.reporter.Contains(emp_name));

                totalInfo totalInfo = new totalInfo();
                List<totalInfo> totalList = qable
                      .Select<totalInfo>("sale_type, COUNT(*) as total_count, SUM(price_sale) as total_amount , SUM(guide_commission) as total_commission")
                      .GroupBy("sale_type")
                      .ToList();
                if (totalList == null || totalList.Count == 0)
                    return totalInfo;
                foreach (totalInfo saleInfo in totalList)
                {
                    if (saleInfo.sale_type == 1)
                    {
                        totalInfo.normal_count = saleInfo.total_count;
                        totalInfo.normal_amount = saleInfo.total_amount;
                        totalInfo.normal_commission = saleInfo.total_commission;
                    }
                    else if (saleInfo.sale_type == 2)
                    {
                        totalInfo.buyout_count = saleInfo.total_count;
                        totalInfo.buyout_amount = saleInfo.total_amount;
                        totalInfo.buyout_commission = saleInfo.total_commission;
                    }
                    else
                    {
                        totalInfo.ex_count = saleInfo.total_count;
                        totalInfo.ex_amount = saleInfo.total_amount;
                        totalInfo.ex_commission = saleInfo.total_commission;
                    }
                    totalInfo.total_count += saleInfo.total_count;
                    totalInfo.total_amount += saleInfo.total_amount;
                    totalInfo.total_commission += saleInfo.total_commission;

                }
                return totalInfo;
            }
        }

        public MemoryStream ExportExcelGuideDetail(Pagination pagination, QueryTime queryTime, int? company_id, string emp_name, string emp_id)
        {
            var LoginInfo = OperatorProvider.Provider.GetCurrent();
            if (LoginInfo == null)
                return null;
            if (pagination == null)
                pagination = new Pagination();
            pagination.sidx = string.IsNullOrEmpty(pagination.sidx) ? "emp_name" : pagination.sidx;
            pagination.sord = string.IsNullOrEmpty(pagination.sord) ? "desc" : pagination.sord;
            CompanyInfo myCompanyInfo = LoginInfo.companyInfo;
            PositionInfo myPositionInfo = LoginInfo.positionInfo;
            using (var db = SugarDao.GetInstance())
            {
                var qable = db.Queryable<daoben_product_sn>().Where(a => a.reporter_type == 1);
                if (myCompanyInfo.category == "事业部")
                    qable.Where(a => a.company_id_parent == myCompanyInfo.id);
                else if (myCompanyInfo.category == "分公司")
                    qable.Where(a => a.company_id == myCompanyInfo.id);
                if (company_id > 0)
                    qable.Where(a => a.company_id == company_id);

                if (queryTime.startTime1 != null)
                    qable.Where(a => (a.sale_time >= queryTime.startTime1));

                if (queryTime.startTime2 != null)
                {
                    queryTime.startTime2 = queryTime.startTime2.ToDate().AddDays(1);
                    qable.Where(a => (a.sale_time < queryTime.startTime2));
                }
                if (!string.IsNullOrEmpty(emp_id))
                    qable.Where(a => a.reporter_id.Equals(emp_id));
                else if (!string.IsNullOrEmpty(emp_name))
                    qable.Where(a => a.reporter.Contains(emp_name));

                StringBuilder selStr = new StringBuilder("company_linkname, reporter as emp_name, phone_sn,model,color,");
                selStr.Append("(CASE WHEN sale_type=0 THEN '已出库' WHEN sale_type=1 THEN '正常销售' WHEN sale_type=2 THEN '买断' WHEN sale_type=3 THEN '包销' WHEN sale_type=-1 THEN '退库' ELSE '-' END),");
                selStr.Append("guide_commission,IF(special_offer=1,'是','否'),IF(high_level=1,'是','否'),price_wholesale,price_sale,sale_time");

                var listDt = qable.OrderBy(pagination.sidx + " " + pagination.sord)
                                .Select(selStr.ToString())
                                .ToDataTable();

                int rowsCount = listDt.Rows.Count;
                string[] headerArr = new string[] { "所属机构", "导购员", "串码", "型号", "颜色", "销售状态", "提成", "特价机", "高端机", "批发价格", "实销价格", "实销时间" };
                int[] colWidthArr = new int[] { 25, 25, 35, 25, 18, 18, 18, 15, 15, 18, 18, 18 };

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
        #endregion
    }
}
