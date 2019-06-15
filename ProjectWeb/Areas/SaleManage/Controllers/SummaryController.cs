using System.Web.Mvc;
using Base.Code;
using ProjectWeb.Areas.SaleManage.Application;
using ProjectShare.Database;
using System;
using System.Web;
using ProjectShare.Models;
using System.IO;
using System.Collections.Generic;

namespace ProjectWeb.Areas.SaleManage.Controllers
{
    public class SummaryController : ControllerBase
    {
        #region 页面显示
        [HttpGet]
        [HandlerAuthorize]
        public ActionResult SnIndex()
        {
            return View();
        }

        [HttpGet]
        [HandlerAuthorize]
        public ActionResult DistributorIndex()
        {
            return View();
        }

        [HttpGet]
        [HandlerAuthorize]
        public ActionResult SalesIndex()
        {
            return View();
        }

        [HttpGet]
        [HandlerAuthorize]
        public ActionResult SalesManagerIndex()
        {
            return View();
        }

        [HttpGet]
        [HandlerAuthorize]
        public ActionResult GuideIndex()
        {
            return View();
        }
        #endregion

        SummaryApp app = new SummaryApp();

        #region 串码汇总
        /// <summary>
        /// 串码删除（退库串码不可删）
        /// </summary>
        /// <param name="snList"></param>
        /// <returns></returns>
        [HttpPost]
        [HandlerAjaxOnly]
        [ValidateAntiForgeryToken]
        public ActionResult SnDelete(string[] snList)
        {
            string result = app.SnDelete(snList);
            if (result == "success")
                return Success("操作成功。");
            else
                return Error(result);
        }

        /// <param name="queryTime">统计时间</param>
        /// <param name="queryInfo">搜索条件：串码/型号</param>
        /// <returns></returns>
        [HttpGet]
        [HandlerAjaxOnly]
        public ActionResult SnMainList(Pagination pagination, QueryTime queryTime, daoben_product_sn queryInfo)
        {
            var data = new
            {
                rows = app.SnMainList(pagination, queryTime, queryInfo),
                total = pagination.records
            };
            return Content(data.ToJson());
        }

        public FileResult ExportExcelSn(Pagination pagination, QueryTime queryTime, daoben_product_sn queryInfo)
        {
            string fileName = "串码汇总_" + DateTime.Now.ToString("yyyyMMddhhmmss") + ".xls";
            Stream fileStream = app.ExportExcelSn(pagination, queryTime, queryInfo);
            return File(fileStream, "application/vnd.ms-excel", fileName);
        }
        #endregion

        #region 经销商销量汇总
        [HttpGet]
        [HandlerAjaxOnly]
        public ActionResult DistriMainList(Pagination pagination, QueryTime queryTime, string distributor_name, int? company_id = 0)
        {
            var data = new
            {
                rows = app.DistriMainList(pagination, queryTime, company_id, distributor_name),
                total = pagination.records
            };
            return Content(data.ToJson());
        }
        /// <summary>
        /// 子表，需要时间参数，经销商id
        /// </summary>
        [HttpGet]
        [HandlerAjaxOnly]
        public ActionResult DistriDetailList(Pagination pagination, QueryTime queryTime, string distributor_id)
        {
            var data = new
            {
                rows = app.DistriDetailList(pagination, queryTime, distributor_id),
                total = pagination.records
            };
            return Content(data.ToJson());
        }

        [HttpGet]
        [HandlerAjaxOnly]
        public ActionResult DistriGetTotalInfo(QueryTime queryTime, string emp_name, int? company_id = 0)
        {
            var data = app.DistriGetTotalInfo(queryTime, company_id, emp_name);
            return Content(data.ToJson());
        }

        public FileResult ExportExcelDistriDetail(Pagination pagination, QueryTime queryTime, string disrtibutor_name, int? company_id = 0)
        {
            string fileName = "经销商销量明细_" + DateTime.Now.ToString("yyyyMMddhhmmss") + ".xls";
            Stream fileStream = app.ExportExcelDistriDetail(pagination, queryTime, company_id, disrtibutor_name);
            return File(fileStream, "application/vnd.ms-excel", fileName);
        }
        #endregion

        #region 业务员销量汇总
        [HttpGet]
        [HandlerAjaxOnly]
        public ActionResult SalesMainList(Pagination pagination, QueryTime queryTime, string emp_name, int? company_id = 0)
        {
            var data = new
            {
                rows = app.SalesMainList(pagination, queryTime, company_id, emp_name),
                total = pagination.records
            };
            return Content(data.ToJson());
        }

        [HttpGet]
        [HandlerAjaxOnly]
        public ActionResult SalesDetailList(Pagination pagination, QueryTime queryTime, string emp_id)
        {
            if (string.IsNullOrEmpty(emp_id))
                return null;
            var data = new
            {
                rows = app.SalesDetailList(pagination, queryTime, emp_id),
                total = pagination.records
            };
            return Content(data.ToJson());
        }

        [HttpGet]
        [HandlerAjaxOnly]
        public ActionResult SalesGetTotalInfo(QueryTime queryTime, string emp_name, int? company_id = 0)
        {
            var data = app.SalesGetTotalInfo(queryTime, company_id, emp_name);
            return Content(data.ToJson());
        }

        public FileResult ExportExcelSalesDetail(Pagination pagination, QueryTime queryTime, string emp_name, int? company_id = 0)
        {
            string fileName = "业务员销量明细_" + DateTime.Now.ToString("yyyyMMddhhmmss") + ".xls";
            Stream fileStream = app.ExportExcelSalesDetail(pagination, queryTime, company_id, emp_name);
            return File(fileStream, "application/vnd.ms-excel", fileName);
        }
        #endregion

        #region 业务经理销量汇总
        [HttpGet]
        [HandlerAjaxOnly]
        public ActionResult SalesManagerMainList(Pagination pagination, QueryTime queryTime, int? company_id, string emp_name)
        {
            var data = new
            {
                rows = app.SalesManagerMainList(pagination, queryTime, company_id, emp_name),
                total = pagination.records
            };
            return Content(data.ToJson());
        }

        [HttpGet]
        [HandlerAjaxOnly]
        public ActionResult SalesManagerDetailList(Pagination pagination, QueryTime queryTime, string emp_id)
        {
            if (string.IsNullOrEmpty(emp_id))
                return null;
            var data = new
            {
                rows = app.SalesManagerDetailList(pagination, queryTime, emp_id),
                total = pagination.records
            };
            return Content(data.ToJson());
        }

        [HttpGet]
        [HandlerAjaxOnly]
        public ActionResult SalesManagerGetTotalInfo(QueryTime queryTime, string emp_name, int? company_id = 0)
        {
            var data = app.SalesManagerGetTotalInfo(queryTime, company_id, emp_name);
            return Content(data.ToJson());
        }

        public FileResult ExportExcelSalesManagerDetail(Pagination pagination, QueryTime queryTime, string emp_name, int? company_id = 0)
        {
            string fileName = "业务经理销量明细_" + DateTime.Now.ToString("yyyyMMddhhmmss") + ".xls";
            Stream fileStream = app.ExportExcelSalesManagerDetail(pagination, queryTime, company_id, emp_name);
            return File(fileStream, "application/vnd.ms-excel", fileName);
        }


        #endregion

        #region 导购销量汇总
        [HttpGet]
        [HandlerAjaxOnly]
        public ActionResult GuideMainList(Pagination pagination, QueryTime queryTime, string emp_name, int? company_id = 0, string emp_id = null)
        {
            var data = new
            {
                rows = app.GuideMainList(pagination, queryTime, company_id, emp_name, emp_id),
                total = pagination.records
            };
            return Content(data.ToJson());
        }

        [HttpGet]
        [HandlerAjaxOnly]
        public ActionResult GuideDetailList(Pagination pagination, QueryTime queryTime, string emp_id)
        {
            if (string.IsNullOrEmpty(emp_id))
                return null;
            var data = new
            {
                rows = app.GuideDetailList(pagination, queryTime, emp_id),
                total = pagination.records
            };
            return Content(data.ToJson());
        }

        [HttpGet]
        [HandlerAjaxOnly]
        public ActionResult GuideGetTotalInfo(QueryTime queryTime, string emp_name, int? company_id = 0, string emp_id = null)
        {
            var data = app.GuideGetTotalInfo(queryTime, company_id, emp_name, emp_id);
            return Content(data.ToJson());
        }

        public FileResult ExportExcelGuideDetail(Pagination pagination, QueryTime queryTime, string emp_name, int? company_id = 0, string emp_id = null)
        {
            string fileName = "导购员销量明细_" + DateTime.Now.ToString("yyyyMMddhhmmss") + ".xls";
            Stream fileStream = app.ExportExcelGuideDetail(pagination, queryTime, company_id, emp_name, emp_id);
            return File(fileStream, "application/vnd.ms-excel", fileName);
        }

        #endregion
    }
}