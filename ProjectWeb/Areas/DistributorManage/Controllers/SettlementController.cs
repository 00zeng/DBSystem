using System.Web.Mvc;
using Base.Code;
using ProjectWeb.Areas.DistributorManage.Application;
using ProjectShare.Database;
using System;
using System.Web;
using ProjectShare.Models;
using System.IO;
using System.Collections.Generic;

namespace ProjectWeb.Areas.DistributorManage.Controllers
{
    public class SettlementController : ControllerBase
    {
        SettlementApp app = new SettlementApp();
        #region 页面显示
        [HttpGet]
        [HandlerAuthorize]
        public ActionResult ShippingIndex()
        {
            return View();
        }
        [HttpGet]
        [HandlerAuthorize]
        public ActionResult RefundIndex()
        {
            return View();
        }
        [HttpGet]
        [HandlerAuthorize]
        public ActionResult RebateIndex()
        {
            return View();
        }
        [HttpGet]
        [HandlerAuthorize]
        public ActionResult AttainingRebateIndex()
        {
            return View();
        }
        [HttpGet]
        [HandlerAuthorize]
        public ActionResult ImageRebateIndex()
        {
            return View();
        }
        [HttpGet]
        [HandlerAuthorize]
        public ActionResult RecomRebateIndex()
        {
            return View();
        }
        [HttpGet]
        [HandlerAuthorize]
        public ActionResult BuyoutRefundIndex()
        {
            return View();
        }
        public ActionResult ExclusiveRefundIndex()
        {
            return View();
        }

        #endregion

        #region Shipping 运费信息
        /// <summary>
        /// 父表，需要时间参数
        /// </summary>
        [HttpGet]
        [HandlerAjaxOnly]
        public ActionResult ShippingMainList(Pagination pagination, QueryTime queryTime, string distributor_name, int company_id = 0)
        {
            var data = new
            {
                rows = app.ShippingMainList(pagination, queryTime, company_id, distributor_name),
                total = pagination.records
            };
            return Content(data.ToJson());
        }
        /// <summary>
        /// 子表，需要时间参数，经销商id
        /// </summary>
        [HttpGet]
        [HandlerAjaxOnly]
        public ActionResult ShippingList(Pagination pagination, QueryTime queryTime, string distributor_id)
        {
            var data = new
            {
                rows = app.ShippingList(pagination, queryTime, distributor_id),
                total = pagination.records
            };
            return Content(data.ToJson());
        }

      
        public FileResult ExportExcelShipping(Pagination pagination, QueryTime queryTime, int? company_id, string distributor_name)
        {
            string fileName = "经销商运费明细_" + DateTime.Now.ToString("yyyyMMddhhmmss") + ".xls";
            Stream fileStream = app.ExportExcelShipping(pagination, queryTime, company_id, distributor_name);
            return File(fileStream, "application/vnd.ms-excel", fileName);
        }
        [HttpGet]
        [HandlerAjaxOnly]
        public ActionResult GetTotalShipping(QueryTime queryTime, int? company_id, string distributor_name)
        {
            var data = app.GetTotalShipping(queryTime, company_id, distributor_name);
            return Content(data.ToJson());
        }
        #endregion

        #region Refund 调价补差
        [HttpGet]
        [HandlerAjaxOnly]
        public ActionResult RefundMainList(Pagination pagination, QueryTime queryTime, string distributor_name, int? company_id = 0)
        {
            var data = new
            {
                rows = app.RefundMainList(pagination, queryTime, company_id, distributor_name),
                total = pagination.records
            };
            return Content(data.ToJson());
        }

        /// <summary>
        /// 子表，需要时间参数，经销商id
        /// </summary>
        [HttpGet]
        [HandlerAjaxOnly]
        public ActionResult RefundList(Pagination pagination, QueryTime queryTime, string distributor_id)
        {
            var data = new
            {
                rows = app.RefundList(pagination, queryTime, distributor_id),
                total = pagination.records
            };
            return Content(data.ToJson());
        }       

        public FileResult ExportExcelRefund(Pagination pagination, QueryTime queryTime, int? company_id, string distributor_name)
        {
            string fileName = "经销商调价补差明细_" + DateTime.Now.ToString("yyyyMMddhhmmss") + ".xls";
            Stream fileStream = app.ExportExcelRefund(pagination, queryTime, company_id, distributor_name);
            return File(fileStream, "application/vnd.ms-excel", fileName);
        }

        [HttpGet]
        [HandlerAjaxOnly]
        public ActionResult GetTotalRefund(QueryTime queryTime, int? company_id, string distributor_name)
        {
            var data = app.GetTotalRefund(queryTime, company_id, distributor_name);
            return Content(data.ToJson());
        }
        #endregion

        #region BuyoutRefund 买断补差
        /// <param name="queryTime">统计时间</param>
        /// <param name="company_id">搜索条件：分公司ID</param>
        /// <param name="distributor_name">搜索条件：经销商名称</param>
        /// <returns></returns>
        [HttpGet]
        [HandlerAjaxOnly]
        public ActionResult BuyoutRefundMainList(Pagination pagination, QueryTime queryTime, string distributor_name, int? company_id = 0)
        {
            var data = new
            {
                rows = app.BuyoutRefundMainList(pagination, queryTime, company_id, distributor_name),
                total = pagination.records
            };
            return Content(data.ToJson());
        }

        /// <summary>
        /// 子表，需要时间参数，经销商id
        /// </summary>
        [HttpGet]
        [HandlerAjaxOnly]
        public ActionResult BuyoutRefundList(Pagination pagination, QueryTime queryTime, string distributor_id)
        {
            var data = new
            {
                rows = app.BuyoutRefundList(pagination, queryTime, distributor_id),
                total = pagination.records
            };
            return Content(data.ToJson());
        }

       
        public FileResult ExportExcelBuyoutRefund(Pagination pagination, QueryTime queryTime, int? company_id, string distributor_name)
        {
            string fileName = "经销商买断补差明细_" + DateTime.Now.ToString("yyyyMMddhhmmss") + ".xls";
            Stream fileStream = app.ExportExcelBuyoutRefund(pagination, queryTime, company_id, distributor_name);
            return File(fileStream, "application/vnd.ms-excel", fileName);
        }
        [HttpGet]
        [HandlerAjaxOnly]
        public ActionResult GetTotalBuyoutRefund(QueryTime queryTime, int? company_id, string distributor_name)
        {
            var data = app.GetTotalBuyoutRefund(queryTime, company_id, distributor_name);
            return Content(data.ToJson());
        }
        #endregion

        #region ExclusiveRefund 包销补差
        /// <param name="queryTime">统计时间</param>
        /// <param name="company_id">搜索条件：分公司ID</param>
        /// <param name="distributor_name">搜索条件：经销商名称</param>
        /// <returns></returns>
        [HttpGet]
        [HandlerAjaxOnly]
        public ActionResult ExclusiveRefundMainList(Pagination pagination, QueryTime queryTime, string distributor_name, int? company_id = 0)
        {
            var data = new
            {
                rows = app.ExclusiveRefundMainList(pagination, queryTime, company_id, distributor_name),
                total = pagination.records
            };
            return Content(data.ToJson());
        }
        /// <summary>
        /// 子表，需要时间参数，经销商id
        /// </summary>
        [HttpGet]
        [HandlerAjaxOnly]
        public ActionResult ExclusiveRefundList(Pagination pagination, QueryTime queryTime, string distributor_id)
        {
            var data = new
            {
                rows = app.ExclusiveRefundList(pagination, queryTime, distributor_id),
                total = pagination.records
            };
            return Content(data.ToJson());
        }       
        public FileResult ExportExcelExclusiveRefund(Pagination pagination, QueryTime queryTime, int? company_id, string distributor_name)
        {
            string fileName = "经销商包销补差明细_" + DateTime.Now.ToString("yyyyMMddhhmmss") + ".xls";
            Stream fileStream = app.ExportExcelExclusiveRefund(pagination, queryTime, company_id, distributor_name);
            return File(fileStream, "application/vnd.ms-excel", fileName);
        }
        [HttpGet]
        [HandlerAjaxOnly]
        public ActionResult GetTotalExclusiveRefund(QueryTime queryTime, int? company_id, string distributor_name)
        {
            var data = app.GetTotalExclusiveRefund(queryTime, company_id, distributor_name);
            return Content(data.ToJson());
        }

        #endregion

        #region 达量返利
        /// <summary>
        /// 子表，需要时间参数，经销商id
        /// </summary>
        [HttpGet]
        [HandlerAjaxOnly]
        public ActionResult AttainingList(Pagination pagination, QueryTime queryTime, string distributor_id)
        {
            var data = new
            {
                rows = app.AttainingList(pagination, queryTime, distributor_id),
                total = pagination.records
            };
            return Content(data.ToJson());
        }

        /// <summary>
        /// 父表，需要时间参数
        /// </summary>
        [HttpGet]
        [HandlerAjaxOnly]
        public ActionResult AttainingMainList(Pagination pagination, QueryTime queryTime, string distributor_name, int company_id = 0)
        {
            var data = new
            {
                rows = app.AttainingMainList(pagination, queryTime, company_id, distributor_name),
                total = pagination.records
            };
            return Content(data.ToJson());
        }

        public FileResult AttainingExport(Pagination pagination, QueryTime queryTime, string distributor_name, int? company_id = 0)
        {
            string fileName = "经销商达量返利明细_" + DateTime.Now.ToString("yyyyMMddhhmmss") + ".xls";
            Stream fileStream = app.ExportExcelAttaining(pagination, queryTime, company_id, distributor_name);
            return File(fileStream, "application/vnd.ms-excel", fileName);
        }


        [HttpGet]
        [HandlerAjaxOnly]
        public ActionResult GetTotalAttaining(QueryTime queryTime, string distributor_name, int? company_id = 0)
        {
            var data = app.GetTotalAttaining(queryTime, company_id, distributor_name);
            return Content(data.ToJson());
        }
        #endregion

        #region 主推返利
        /// <summary>
        /// 子表，需要时间参数，经销商id
        /// </summary>
        [HttpGet]
        [HandlerAjaxOnly]
        public ActionResult RecomList(Pagination pagination, QueryTime queryTime, string distributor_id)
        {
            var data = new
            {
                rows = app.RecomList(pagination, queryTime, distributor_id),
                total = pagination.records
            };
            return Content(data.ToJson());
        }

        /// <summary>
        /// 父表，需要时间参数
        /// </summary>
        [HttpGet]
        [HandlerAjaxOnly]
        public ActionResult RecomMainList(Pagination pagination, QueryTime queryTime, string distributor_name, int company_id = 0)
        {
            var data = new
            {
                rows = app.RecomMainList(pagination, queryTime, company_id, distributor_name),
                total = pagination.records
            };
            return Content(data.ToJson());
        }
        public FileResult RecomExport(Pagination pagination, QueryTime queryTime, string distributor_name, int? company_id = 0)
        {
            string fileName = "经销商主推返利明细_" + DateTime.Now.ToString("yyyyMMddhhmmss") + ".xls";
            Stream fileStream = app.ExportExcelRecom(pagination, queryTime, company_id, distributor_name);
            return File(fileStream, "application/vnd.ms-excel", fileName);
        }

        [HttpGet]
        [HandlerAjaxOnly]
        public ActionResult GetTotalRecom(QueryTime queryTime, string emp_name, int? company_id = 0)
        {
            var data = app.GetTotalRecom(queryTime, company_id, emp_name);
            return Content(data.ToJson());
        }
        #endregion

        #region 形象返利
        /// <summary>
        /// 子表，需要时间参数，经销商id
        /// </summary>
        [HttpGet]
        [HandlerAjaxOnly]
        public ActionResult ImageList(Pagination pagination, QueryTime queryTime, string distributor_id)
        {
            var data = new
            {
                rows = app.ImageList(pagination, queryTime, distributor_id),
                total = pagination.records
            };
            return Content(data.ToJson());
        }

        /// <summary>
        /// 父表，需要时间参数
        /// </summary>
        [HttpGet]
        [HandlerAjaxOnly]
        public ActionResult ImageMainList(Pagination pagination, QueryTime queryTime, string distributor_name, int company_id = 0)
        {
            var data = new
            {
                rows = app.ImageMainList(pagination, queryTime, company_id, distributor_name),
                total = pagination.records
            };
            return Content(data.ToJson());
        }
        public FileResult ImageExport(Pagination pagination, QueryTime queryTime, string distributor_name, int? company_id = 0)
        {
            string fileName = "经销商形象返利明细_" + DateTime.Now.ToString("yyyyMMddhhmmss") + ".xls";
            Stream fileStream = app.ExportExcelImage(pagination, queryTime, company_id, distributor_name);
            return File(fileStream, "application/vnd.ms-excel", fileName);
        }

        [HttpGet]
        [HandlerAjaxOnly]
        public ActionResult GetTotalImage(QueryTime queryTime, string emp_name, int? company_id = 0)
        {
            var data = app.GetTotalImage(queryTime, company_id, emp_name);
            return Content(data.ToJson());
        }
        #endregion
    }
}