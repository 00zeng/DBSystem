using Base.Code;
using MySqlSugar;
using ProjectShare.Database;
using ProjectShare.Process;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ProjectWeb.Areas.Statistics.Application
{
    public class StatisticsApp
    {

        public string SaleStatistics(int companyL1Id, int companyL2Id, int areaL1Id, int areaL2Id,
                string distributorId, DateTime? startDate, DateTime? endDate, int dateType)
        {
            DateTime now = DateTime.Now.Date;
            if (startDate == null)
                startDate = now.AddDays(1 - now.Day);
            var LoginInfo = OperatorProvider.Provider.GetCurrent();
            if (LoginInfo == null)
                return "登录超时，请重新登录";
            CompanyInfo myCompanyInfo = LoginInfo.companyInfo;
            if (myCompanyInfo.category == "事业部")
                companyL1Id = myCompanyInfo.id;
            else if (myCompanyInfo.category == "分公司")
                companyL2Id = myCompanyInfo.id;

            string selStr = null;
            string selTimeStr = null;
            string whereStr = null;

            switch (dateType)
            {
                case 3:     // 按年
                    selTimeStr = "DATE_FORMAT({0},'%Y') t_interval ";
                    endDate = (endDate == null ? now : ((DateTime)endDate).Date.AddYears(1));// 实际统计为结束日的24:00
                    break;
                case 2:     // 按季度，时间格式如：2018-Q1
                    selTimeStr = "CONCAT(YEAR({0}),'-Q',QUARTER(a.sale_time)) t_interval ";
                    endDate = (endDate == null ? now : ((DateTime)endDate).Date.AddMonths(3));// TODO
                    break;
                case 1:     // 按月
                    selTimeStr = "DATE_FORMAT({0}, '%Y-%m') t_interval ";
                    endDate = (endDate == null ? now : ((DateTime)endDate).Date.AddMonths(1));
                    break;
                default:    // 按天
                    selTimeStr = "DATE_FORMAT({0},'%m-%d') t_interval ";
                    endDate = (endDate == null ? now : ((DateTime)endDate).Date.AddDays(1));
                    break;
            }

            if (areaL2Id > 0)                      // 统计单一业务片区
                whereStr = " {0}" + string.Format(".area_l2_id ='{0}' ", areaL2Id); 
            else if (areaL1Id > 0)                      // 统计单一经理片区
                whereStr = " {0}" + string.Format(".area_l1_id ='{0}' ", areaL1Id);
            else if (companyL2Id > 0)                   // 统计单一分公司
                whereStr = " {0}" + string.Format(".company_id ='{0}' ", companyL2Id);
            else if (companyL1Id > 0)                   // 统计单一事业部
                whereStr = " {0}" + string.Format(".company_id_parent ='{0}' ", companyL1Id);

            using (var db = SugarDao.GetInstance())
            {
                #region 出库
                selStr = "SUM(a.price_wholesale) as amount, COUNT(*) as quantity, "
                        + string.Format(selTimeStr, "a.outstorage_time");
                var outQable = db.Queryable<daoben_product_sn>()
                        .JoinTable<daoben_distributor_info>((a, b) => a.out_distributor_id == b.id)
                        .Where<daoben_distributor_info>((a, b) => a.outstorage_time >= startDate && a.outstorage_time < endDate);
                if (!string.IsNullOrEmpty(distributorId))   // 统计单一经销商
                    outQable.Where(a => a.out_distributor_id == distributorId);
                else
                    outQable.Where(string.Format(whereStr, "b"));
                List<SaleStatModel> outList = outQable.Select<SaleStatModel>(selStr)
                        .GroupBy("t_interval").OrderBy("t_interval asc").ToList();
                #endregion

                #region 实销
                selStr = "SUM(a.price_sale) as amount, COUNT(*) as quantity, "
                        + string.Format(selTimeStr, "a.sale_time");
                var saleQable = db.Queryable<daoben_product_sn>()
                        .JoinTable<daoben_distributor_info>((a, b) => a.sale_distributor_id == b.id)
                        .Where<daoben_distributor_info>((a, b) => a.sale_time >= startDate && a.sale_time < endDate);
                if (!string.IsNullOrEmpty(distributorId))   // 统计单一经销商
                    saleQable.Where(a => a.sale_distributor_id == distributorId);
                else
                    saleQable.Where(string.Format(whereStr, "b"));
                List<SaleStatModel> saleList = saleQable.Select<SaleStatModel>(selStr)
                        .GroupBy("t_interval").OrderBy("t_interval asc").ToList();
                #endregion

                #region 买断
                selStr = "SUM(a.price_sale) as amount, COUNT(*) as quantity, "
                        + string.Format(selTimeStr, "a.buyout_time");
                var buyoutQable = db.Queryable<daoben_product_sn>()
                        .JoinTable<daoben_distributor_info>((a, b) => a.sale_distributor_id == b.id)
                        .Where<daoben_distributor_info>((a, b) => a.buyout_time >= startDate && a.buyout_time < endDate);
                if (!string.IsNullOrEmpty(distributorId))   // 统计单一经销商
                    buyoutQable.Where(a => a.sale_distributor_id == distributorId);
                else
                    buyoutQable.Where(string.Format(whereStr, "b"));
                List<SaleStatModel> buyoutList = buyoutQable.Select<SaleStatModel>(selStr)
                        .GroupBy("t_interval").OrderBy("t_interval asc").ToList();
                #endregion
                #region 补差
                selStr = "SUM(a.diff_price) as amount, COUNT(*) as quantity, "
                        + string.Format(selTimeStr, "a.accur_time");
                var refundQable = db.Queryable<daoben_sale_refund>()
                        .JoinTable<daoben_sale_refund_approve>((a, c) => a.import_file_id == c.id)
                        .Where<daoben_sale_refund_approve>((a, c) => a.accur_time >= startDate && a.accur_time < endDate && c.status == 100);
                if (!string.IsNullOrEmpty(distributorId))   // 统计单一经销商
                    refundQable.Where(a => a.distributor_id == distributorId);
                else
                    refundQable.Where(string.Format(whereStr, "a"));
                List<SaleStatModel> refundList = refundQable.Select<SaleStatModel>(selStr)
                        .GroupBy("t_interval").OrderBy("t_interval asc").ToList();
                #endregion
                #region 退库
                selStr = "SUM(a.price_wholesale) as amount, COUNT(*) as quantity, "
                        + string.Format(selTimeStr, "a.accur_time");
                var returnQable = db.Queryable<daoben_sale_return>()
                        .JoinTable<daoben_distributor_info>((a, b) => a.distributor_id == b.id)
                        .JoinTable<daoben_sale_return_approve>((a, c) => a.import_file_id == c.id)
                        .Where<daoben_sale_return_approve>((a, c) => a.accur_time >= startDate && a.accur_time < endDate && c.status == 100);
                if (!string.IsNullOrEmpty(distributorId))   // 统计单一经销商
                    returnQable.Where(a => a.distributor_id == distributorId);
                else
                    returnQable.Where(string.Format(whereStr, "b"));
                List<SaleStatModel> returnList = returnQable.Select<SaleStatModel>(selStr)
                        .GroupBy("t_interval").OrderBy("t_interval asc").ToList();
                #endregion
                object resObj = new
                {
                    outList = outList,
                    saleList = saleList,
                    buyoutList = buyoutList,
                    refundList = refundList,
                    returnList = returnList
                };
                return resObj.ToJson();
            }
        }
        public class SaleStatModel
        {
            public string t_interval { get; set; }
            public int quantity { get; set; }
            public decimal amount { get; set; }
        }
#if false
                public string SnStatistics(int companyL1Id, int companyL2Id, int areaL1Id, int areaL2Id,
                string distributorId, DateTime? startDate, DateTime? endDate, int dateType, int statType)
        {
            DateTime now = DateTime.Now.Date;
            if (startDate == null)
                startDate = now.AddDays(1 - now.Day);
            var LoginInfo = OperatorProvider.Provider.GetCurrent();
            if (LoginInfo == null)
                return "登录超时，请重新登录";
            CompanyInfo myCompanyInfo = LoginInfo.companyInfo;
            if (myCompanyInfo.category == "事业部")
                companyL1Id = myCompanyInfo.id;
            else if (myCompanyInfo.category == "分公司")
                companyL2Id = myCompanyInfo.id;

            string selStr = "SUM({0}) as amount, COUNT(*) as quantity, ";
            string timePar = null;
            string amountPar = null;
            switch (dateType)
            {
                case 3:     // 按年
                    selStr += "DATE_FORMAT({1},'%Y') t_interval ";
                    endDate = (endDate == null ? now : ((DateTime)endDate).Date.AddYears(1));// 实际统计为结束日的24:00
                    break;
                case 2:     // 按季度，时间格式如：2018-Q1
                    selStr += "CONCAT(YEAR({1}),'-Q',QUARTER(a.sale_time)) t_interval ";
                    endDate = (endDate == null ? now : ((DateTime)endDate).Date.AddMonths(3));// TODO
                    break;
                case 1:     // 按月
                    selStr += "DATE_FORMAT({1}, '%Y-%m') t_interval ";
                    endDate = (endDate == null ? now : ((DateTime)endDate).Date.AddMonths(1));
                    break;
                default:    // 按天
                    selStr += "DATE_FORMAT({1},'%m-%d') t_interval ";
                    endDate = (endDate == null ? now : ((DateTime)endDate).Date.AddDays(1));

                    break;
            }

            using (var db = SugarDao.GetInstance())
            {
                var qable = db.Queryable<daoben_product_sn>();
                string whereStr = null;
                switch (statType)
                {
                    case 1:     // 出库
                        timePar = "a.outstorage_time";
                        amountPar = "a.price_wholesale";
                        qable.JoinTable<daoben_distributor_info>((a, b) => a.out_distributor_id == b.id)
                                .Where<daoben_distributor_info>((a, b) => a.outstorage_time >= startDate && a.outstorage_time < endDate);
                        whereStr = string.Format(" a.out_distributor_id='{0}' ", distributorId);
                        break;
                    case 2:     // 实销
                        timePar = "a.sale_time";
                        amountPar = "a.price_retail";
                        qable.JoinTable<daoben_distributor_info>((a, b) => a.sale_distributor_id == b.id)
                                .Where<daoben_distributor_info>((a, b) => a.sale_time >= startDate && a.sale_time < endDate);
                        whereStr = string.Format(" a.sale_distributor_id='{0}' ", distributorId);
                        break;
                    case 3:     // 买断
                        timePar = "a.buyout_time";
                        amountPar = "a.price_buyout";
                        qable.JoinTable<daoben_distributor_info>((a, b) => a.sale_distributor_id == b.id)
                                .Where<daoben_distributor_info>((a, b) => a.buyout_time >= startDate && a.buyout_time < endDate);
                        whereStr = string.Format(" a.sale_distributor_id='{0}' ", distributorId);
                        break;
                }
                selStr = string.Format(selStr, amountPar, timePar);

                if (!string.IsNullOrEmpty(distributorId))   // 统计单一经销商
                    qable.Where(whereStr);
                else if (areaL2Id > 0)                      // 统计单一业务片区
                    qable.Where<daoben_distributor_info>((a, b) => b.area_l2_id == areaL2Id);
                else if (areaL1Id > 0)                      // 统计单一经理片区
                    qable.Where<daoben_distributor_info>((a, b) => b.area_l1_id == areaL1Id);
                else if (companyL2Id > 0)                   // 统计单一分公司
                    qable.Where<daoben_distributor_info>((a, b) => b.company_id == companyL2Id);
                else if (companyL1Id > 0)                   // 统计单一事业部
                    qable.Where<daoben_distributor_info>((a, b) => b.company_id_parent == companyL1Id);
                string statisRes = qable.Select(selStr).GroupBy("t_interval").OrderBy("t_interval asc").ToJson();
                return statisRes;
            }
        }

        public string OutStorageStatistics(int companyL1Id, int companyL2Id, int areaL1Id, int areaL2Id,
                string distributorId, DateTime? startDate, DateTime? endDate, int dateType)
        {
            DateTime now = DateTime.Now.Date;
            if (endDate == null)
                endDate = now;
            else
                endDate = ((DateTime)endDate).Date.AddDays(1); // 实际统计为结束日的24:00
            if (startDate == null)
                startDate = now.AddDays(1 - now.Day);
            var LoginInfo = OperatorProvider.Provider.GetCurrent();
            if (LoginInfo == null)
                return "登录超时，请重新登录";
            CompanyInfo myCompanyInfo = LoginInfo.companyInfo;
            if (myCompanyInfo.category == "事业部")
                companyL1Id = myCompanyInfo.id;
            else if (myCompanyInfo.category == "分公司")
                companyL2Id = myCompanyInfo.id;
            string selStr = null;
            switch (dateType)
            {
                case 3:     // 按年
                    selStr = "DATE_FORMAT(a.outstorage_time,'%Y') t_interval, count(*) as quantity, SUM(a.price_buyout) as amount ";
                    break;
                case 2:     // 按季度
                    selStr = "CONCAT(YEAR(a.outstorage_time),QUARTER(a.outstorage_time)) t_interval, count(*) as quantity, SUM(a.price_buyout) as amount ";
                    break;
                case 1:     // 按月
                    selStr = "DATE_FORMAT(a.outstorage_time, '%Y-%m') t_interval, count(*) as quantity, SUM(a.price_buyout) as amount ";
                    break;
                default:    // 按天
                    selStr = "DATE_FORMAT(a.outstorage_time,'%m-%d') t_interval, count(*) as quantity, SUM(a.price_buyout) as amount ";
                    break;
            }

            using (var db = SugarDao.GetInstance())
            {
                var qable = db.Queryable<daoben_product_sn>()
                        .JoinTable<daoben_distributor_info>((a, b) => a.distributor_id == b.id)
                        .Where<daoben_distributor_info>((a, b) => a.outstorage_time >= startDate && a.outstorage_time < endDate);
                if (!string.IsNullOrEmpty(distributorId))   // 统计单一经销商
                    qable.Where(a => a.distributor_id == distributorId);
                else if (areaL2Id > 0)                      // 统计单一业务片区
                    qable.Where<daoben_distributor_info>((a, b) => b.area_l2_id == areaL2Id);
                else if (areaL1Id > 0)                      // 统计单一经理片区
                    qable.Where<daoben_distributor_info>((a, b) => b.area_l1_id == areaL1Id);
                else if (companyL2Id > 0)                   // 统计单一分公司
                    qable.Where<daoben_distributor_info>((a, b) => b.company_id == companyL2Id);
                else if (companyL1Id > 0)                   // 统计单一事业部
                    qable.Where<daoben_distributor_info>((a, b) => b.company_id_parent == companyL1Id);
                string statisRes = qable.Select(selStr).GroupBy("t_interval").OrderBy("t_interval asc").ToJson();
                return statisRes;
            }
        }



        public string SaleStatistics(int companyL1Id, int companyL2Id, int areaL1Id, int areaL2Id,
                string distributorId, DateTime? startDate, DateTime? endDate, int dateType)
        {
            DateTime now = DateTime.Now.Date;
            if (endDate == null)
                endDate = now;
            else
                endDate = ((DateTime)endDate).Date.AddDays(1); // 实际统计为结束日的24:00
            if (startDate == null)
                startDate = now.AddDays(1 - now.Day);
            var LoginInfo = OperatorProvider.Provider.GetCurrent();
            if (LoginInfo == null)
                return "登录超时，请重新登录";
            CompanyInfo myCompanyInfo = LoginInfo.companyInfo;
            if (myCompanyInfo.category == "事业部")
                companyL1Id = myCompanyInfo.id;
            else if (myCompanyInfo.category == "分公司")
                companyL2Id = myCompanyInfo.id;
            string selStr = null;
            switch (dateType)
            {
                case 3:     // 按年
                    selStr = "DATE_FORMAT(a.sale_time,'%Y') t_interval, count(*) as quantity, SUM(a.price_wholesale) as wholesale_amount, SUM(a.price_retail) as retail_amount ";
                    break;
                case 2:     // 按季度
                    selStr = "CONCAT(YEAR(a.sale_time),QUARTER(a.sale_time)) t_interval, count(*) as quantity, SUM(a.price_wholesale) as wholesale_amount, SUM(a.price_retail) as retail_amount ";
                    break;
                case 1:     // 按月
                    selStr = "DATE_FORMAT(a.sale_time, '%Y-%m') t_interval, count(*) as quantity, SUM(a.price_wholesale) as wholesale_amount, SUM(a.price_retail) as retail_amount ";
                    break;
                default:    // 按天
                    selStr = "DATE_FORMAT(a.sale_time,'%m-%d') t_interval, count(*) as quantity, SUM(a.price_wholesale) as wholesale_amount, SUM(a.price_retail) as retail_amount ";
                    break;
            }

            using (var db = SugarDao.GetInstance())
            {
                var qable = db.Queryable<daoben_product_sn>()
                        .JoinTable<daoben_distributor_info>((a, b) => a.distributor_id == b.id)
                        .Where<daoben_distributor_info>((a, b) => a.outstorage_time >= startDate && a.outstorage_time < endDate);
                if (!string.IsNullOrEmpty(distributorId))   // 统计单一经销商
                    qable.Where(a => a.distributor_id == distributorId);
                else if (areaL2Id > 0)                      // 统计单一业务片区
                    qable.Where<daoben_distributor_info>((a, b) => b.area_l2_id == areaL2Id);
                else if (areaL1Id > 0)                      // 统计单一经理片区
                    qable.Where<daoben_distributor_info>((a, b) => b.area_l1_id == areaL1Id);
                else if (companyL2Id > 0)                   // 统计单一分公司
                    qable.Where<daoben_distributor_info>((a, b) => b.company_id == companyL2Id);
                else if (companyL1Id > 0)                   // 统计单一事业部
                    qable.Where<daoben_distributor_info>((a, b) => b.company_id_parent == companyL1Id);
                string statisRes = qable.Select(selStr).GroupBy("t_interval").OrderBy("t_interval asc").ToJson();
                return statisRes;
            }
        }

        public string BuyoutStatistics(int companyL1Id, int companyL2Id, int areaL1Id, int areaL2Id,
                string distributorId, DateTime? startDate, DateTime? endDate, int dateType)
        {
            DateTime now = DateTime.Now.Date;
            if (endDate == null)
                endDate = now;
            else
                endDate = ((DateTime)endDate).Date.AddDays(1); // 实际统计为结束日的24:00
            if (startDate == null)
                startDate = now.AddDays(1 - now.Day);
            var LoginInfo = OperatorProvider.Provider.GetCurrent();
            if (LoginInfo == null)
                return "登录超时，请重新登录";
            CompanyInfo myCompanyInfo = LoginInfo.companyInfo;
            if (myCompanyInfo.category == "事业部")
                companyL1Id = myCompanyInfo.id;
            else if (myCompanyInfo.category == "分公司")
                companyL2Id = myCompanyInfo.id;
            string selStr = null;
            switch (dateType)
            {
                case 3:     // 按年
                    selStr = "DATE_FORMAT(a.buyout_time,'%Y') t_interval, count(*) as quantity, SUM(a.price_buyout) as amount ";
                    break;
                case 2:     // 按季度
                    selStr = "CONCAT(YEAR(a.buyout_time),QUARTER(a.buyout_time)) t_interval, count(*) as quantity, SUM(a.price_buyout) as amount ";
                    break;
                case 1:     // 按月
                    selStr = "DATE_FORMAT(a.buyout_time, '%Y-%m') t_interval, count(*) as quantity, SUM(a.price_buyout) as amount ";
                    break;
                default:    // 按天
                    selStr = "DATE_FORMAT(a.buyout_time,'%m-%d') t_interval, count(*) as quantity, SUM(a.price_buyout) as amount ";
                    break;
            }

            using (var db = SugarDao.GetInstance())
            {
                var qable = db.Queryable<daoben_product_sn>()
                        .JoinTable<daoben_distributor_info>((a, b) => a.distributor_id == b.id)
                        .Where<daoben_distributor_info>((a, b) => a.outstorage_time >= startDate && a.outstorage_time < endDate);
                if (!string.IsNullOrEmpty(distributorId))   // 统计单一经销商
                    qable.Where(a => a.distributor_id == distributorId);
                else if (areaL2Id > 0)                      // 统计单一业务片区
                    qable.Where<daoben_distributor_info>((a, b) => b.area_l2_id == areaL2Id);
                else if (areaL1Id > 0)                      // 统计单一经理片区
                    qable.Where<daoben_distributor_info>((a, b) => b.area_l1_id == areaL1Id);
                else if (companyL2Id > 0)                   // 统计单一分公司
                    qable.Where<daoben_distributor_info>((a, b) => b.company_id == companyL2Id);
                else if (companyL1Id > 0)                   // 统计单一事业部
                    qable.Where<daoben_distributor_info>((a, b) => b.company_id_parent == companyL1Id);
                string statisRes = qable.Select(selStr).GroupBy("t_interval").OrderBy("t_interval asc").ToJson();
                return statisRes;
            }
        }

        // 补差
        public string RefundStatistics(int companyL1Id, int companyL2Id, int areaL1Id, int areaL2Id,
               string distributorId, DateTime? startDate, DateTime? endDate, int dateType)
        {
            DateTime now = DateTime.Now.Date;
            if (endDate == null)
                endDate = now;
            else
                endDate = ((DateTime)endDate).Date.AddDays(1); // 实际统计为结束日的24:00
            if (startDate == null)
                startDate = now.AddDays(1 - now.Day);
            var LoginInfo = OperatorProvider.Provider.GetCurrent();
            if (LoginInfo == null)
                return "登录超时，请重新登录";
            CompanyInfo myCompanyInfo = LoginInfo.companyInfo;
            if (myCompanyInfo.category == "事业部")
                companyL1Id = myCompanyInfo.id;
            else if (myCompanyInfo.category == "分公司")
                companyL2Id = myCompanyInfo.id;
            string selStr = null;
            switch (dateType)
            {
                case 3:     // 按年
                    selStr = "DATE_FORMAT(a.accur_time,'%Y') t_interval, count(*) as quantity, SUM(a.diff_price) as amount ";
                    break;
                case 2:     // 按季度
                    selStr = "CONCAT(YEAR({1}),'-Q',QUARTER(a.accur_time)) t_interval, count(*) as quantity, SUM(a.diff_price) as amount ";
                    break;
                case 1:     // 按月
                    selStr = "DATE_FORMAT(a.accur_time, '%Y-%m') t_interval, count(*) as quantity, SUM(a.diff_price) as amount ";
                    break;
                default:    // 按天
                    selStr = "DATE_FORMAT(a.accur_time,'%m-%d') t_interval, count(*) as quantity, SUM(a.diff_price) as amount ";
                    break;
            }

            using (var db = SugarDao.GetInstance())
            {
                var qable = db.Queryable<daoben_sale_refund>()
                        .JoinTable<daoben_sale_refund_approve>((a, c) => a.import_file_id == c.id)
                        .Where<daoben_sale_refund_approve>((a, c) => a.accur_time >= startDate && a.accur_time < endDate && c.status == 100);
                if (!string.IsNullOrEmpty(distributorId))   // 统计单一经销商
                    qable.Where(a => a.distributor_id == distributorId);
                else if (areaL2Id > 0)                      // 统计单一业务片区
                    qable.Where(a => a.area_l2_id == areaL2Id);
                else if (areaL1Id > 0)                      // 统计单一经理片区
                    qable.Where(a => a.area_l1_id == areaL1Id);
                else if (companyL2Id > 0)                   // 统计单一分公司
                    qable.Where(a => a.company_id == companyL2Id);
                else if (companyL1Id > 0)                   // 统计单一事业部
                    qable.Where(a => a.company_id_parent == companyL1Id);
                string statisRes = qable.Select(selStr).GroupBy("t_interval").OrderBy("t_interval asc").ToJson();
                return statisRes;
            }
        }

        public string ReturnStatistics(int companyL1Id, int companyL2Id, int areaL1Id, int areaL2Id,
               string distributorId, DateTime? startDate, DateTime? endDate, int dateType)
        {
            DateTime now = DateTime.Now.Date;
            if (endDate == null)
                endDate = now;
            else
                endDate = ((DateTime)endDate).Date.AddDays(1); // 实际统计为结束日的24:00
            if (startDate == null)
                startDate = now.AddDays(1 - now.Day);
            var LoginInfo = OperatorProvider.Provider.GetCurrent();
            if (LoginInfo == null)
                return "登录超时，请重新登录";
            CompanyInfo myCompanyInfo = LoginInfo.companyInfo;
            if (myCompanyInfo.category == "事业部")
                companyL1Id = myCompanyInfo.id;
            else if (myCompanyInfo.category == "分公司")
                companyL2Id = myCompanyInfo.id;
            string selStr = null;
            switch (dateType)
            {
                case 3:     // 按年
                    selStr = "DATE_FORMAT(a.accur_time,'%Y') t_interval, count(*) as quantity, SUM(a.price_wholesale) as amount ";
                    break;
                case 2:     // 按季度
                    selStr = "CONCAT(YEAR({1}),'-Q',QUARTER(a.accur_time)) t_interval, count(*) as quantity, SUM(a.price_wholesale) as amount ";
                    break;
                case 1:     // 按月
                    selStr = "DATE_FORMAT(a.accur_time, '%Y-%m') t_interval, count(*) as quantity, SUM(a.price_wholesale) as amount ";
                    break;
                default:    // 按天
                    selStr = "DATE_FORMAT(a.accur_time,'%m-%d') t_interval, count(*) as quantity, SUM(a.price_wholesale) as amount ";
                    break;
            }

            using (var db = SugarDao.GetInstance())
            {
                var qable = db.Queryable<daoben_sale_return>()
                        .JoinTable<daoben_distributor_info>((a, b) => a.distributor_id == b.id)
                        .JoinTable<daoben_sale_return_approve>((a, c) => a.import_file_id == c.id)
                        .Where<daoben_sale_return_approve>((a, c) => a.accur_time >= startDate && a.accur_time < endDate && c.status == 100);
                if (!string.IsNullOrEmpty(distributorId))   // 统计单一经销商
                    qable.Where(a => a.distributor_id == distributorId);
                else if (areaL2Id > 0)                      // 统计单一业务片区
                    qable.Where<daoben_distributor_info>((a, b) => b.area_l2_id == areaL2Id);
                else if (areaL1Id > 0)                      // 统计单一经理片区
                    qable.Where<daoben_distributor_info>((a, b) => b.area_l1_id == areaL1Id);
                else if (companyL2Id > 0)                   // 统计单一分公司
                    qable.Where<daoben_distributor_info>((a, b) => b.company_id == companyL2Id);
                else if (companyL1Id > 0)                   // 统计单一事业部
                    qable.Where<daoben_distributor_info>((a, b) => b.company_id_parent == companyL1Id);
                string statisRes = qable.Select(selStr).GroupBy("t_interval").OrderBy("t_interval asc").ToJson();
                return statisRes;
            }
        }
#endif
    }
}
