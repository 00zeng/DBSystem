using System.Web.Mvc;
using Base.Code;
using ProjectShare.Database;
using System;
using System.Web;
using ProjectShare.Models;
using System.IO;
using System.Collections.Generic;
using ProjectWeb.Areas.ActivityManage.Application;

namespace ProjectWeb.Areas.ActivityManage.Controllers
{
    public class CalculateController : ControllerBase
    {
        CalculateApp app = new CalculateApp();
        #region 页面显示
        [HttpGet]
        [HandlerAuthorize]
        public ActionResult SalesPerfIndex()
        {
            return View();
        }
        [HttpGet]
        [HandlerAuthorize]
        public ActionResult RankingIndex()
        {
            return View();
        }
        [HttpGet]
        [HandlerAuthorize]
        public ActionResult PKIndex()
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
        public ActionResult RecomIndex()
        {
            return View();
        }
        [HttpGet]
        [HandlerAuthorize]
        public ActionResult AttainingIndex()
        {
            return View();
        }

        #endregion

        /// <summary>
        /// 子表，需要时间参数，员工id
        /// </summary>
        [HttpGet]
        [HandlerAjaxOnly]
        public ActionResult AttainingList(Pagination pagination, QueryTime queryTime, string emp_id)
        {
            var data = new
            {
                rows = app.AttainingList(pagination, queryTime, emp_id),
                total = pagination.records
            };
            return Content(data.ToJson());
        }
        public FileResult AttainingExport(Pagination pagination, QueryTime queryTime, string emp_name, int? company_id = 0, int emp_Category = 0)
        {
            string fileName = "达量活动返利明细_" + DateTime.Now.ToString("yyyyMMddhhmmss") + ".xls";
            Stream fileStream = app.ExportExcelAttaining(pagination, queryTime, company_id, emp_name, emp_Category);
            return File(fileStream, "application/vnd.ms-excel", fileName);
        }
        /// <summary>
        /// 父表，需要时间参数
        /// </summary>
        [HttpGet]
        [HandlerAjaxOnly]
        public ActionResult AttainingMainList(Pagination pagination, QueryTime queryTime, string emp_name, int? emp_category = 0, int company_id = 0)
        {
            var data = new
            {
                rows = app.AttainingMainList(pagination, queryTime, company_id, emp_name, emp_category),
                total = pagination.records
            };
            return Content(data.ToJson());
        }

        /// <summary>
        /// 子表，需要时间参数，员工id
        /// </summary>
        [HttpGet]
        [HandlerAjaxOnly]
        public ActionResult RecomList(Pagination pagination, QueryTime queryTime, string emp_id)
        {
            var data = new
            {
                rows = app.RecomList(pagination, queryTime, emp_id),
                total = pagination.records
            };
            return Content(data.ToJson());
        }
        /// <summary>
        /// 父表，需要时间参数
        /// </summary>
        [HttpGet]
        [HandlerAjaxOnly]
        public ActionResult RecomMainList(Pagination pagination, QueryTime queryTime, string emp_name, int? emp_category = 0, int company_id = 0)
        {
            var data = new
            {
                rows = app.RecomMainList(pagination, queryTime, company_id, emp_name, emp_category),
                total = pagination.records
            };
            return Content(data.ToJson());
        }
        public FileResult RecomExport(Pagination pagination, QueryTime queryTime, string emp_name, int? company_id = 0, int? emp_category = 0)
        {
            string fileName = "主推活动返利明细_" + DateTime.Now.ToString("yyyyMMddhhmmss") + ".xls";
            Stream fileStream = app.ExportExcelRecom(pagination, queryTime, company_id, emp_name, emp_category);
            return File(fileStream, "application/vnd.ms-excel", fileName);
        }
        /// <summary>
        /// 子表，需要时间参数，员工id
        /// </summary>
        [HttpGet]
        [HandlerAjaxOnly]
        public ActionResult RankList(Pagination pagination, QueryTime queryTime, string emp_id)
        {
            var data = new
            {
                rows = app.RankList(pagination, queryTime, emp_id),
                total = pagination.records
            };
            return Content(data.ToJson());
        }
        /// <summary>
        /// 父表，需要时间参数
        /// </summary>
        [HttpGet]
        [HandlerAjaxOnly]
        public ActionResult RankMainList(Pagination pagination, QueryTime queryTime, string emp_name, int company_id = 0, int? emp_category = 0)
        {
            var data = new
            {
                rows = app.RankMainList(pagination, queryTime, company_id, emp_name, emp_category),
                total = pagination.records
            };
            return Content(data.ToJson());
        }

        public FileResult RankExport(Pagination pagination, QueryTime queryTime, string emp_name, int? company_id = 0, int? emp_category = 0)
        {
            string fileName = "排名比赛活动奖励明细_" + DateTime.Now.ToString("yyyyMMddhhmmss") + ".xls";
            Stream fileStream = app.ExportExcelRank(pagination, queryTime, company_id, emp_name, emp_category);
            return File(fileStream, "application/vnd.ms-excel", fileName);
        }
        /// <summary>
        /// 子表，需要时间参数，员工id
        /// </summary>
        [HttpGet]
        [HandlerAjaxOnly]
        public ActionResult PKList(Pagination pagination, QueryTime queryTime, string emp_id)
        {
            var data = new
            {
                rows = app.PKList(pagination, queryTime, emp_id),
                total = pagination.records
            };
            return Content(data.ToJson());
        }
        /// <summary>
        /// 父表，需要时间参数
        /// </summary>
        [HttpGet]
        [HandlerAjaxOnly]
        public ActionResult PKMainList(Pagination pagination, QueryTime queryTime, string emp_name, int company_id = 0, int? emp_category = 0)
        {
            var data = new
            {
                rows = app.PKMainList(pagination, queryTime, company_id, emp_name, emp_category),
                total = pagination.records
            };
            return Content(data.ToJson());
        }

        public FileResult PKExport(Pagination pagination, QueryTime queryTime, string emp_name, int? company_id = 0, int? emp_category = 0)
        {
            string fileName = "PK比赛活动奖励明细_" + DateTime.Now.ToString("yyyyMMddhhmmss") + ".xls";
            Stream fileStream = app.ExportExcelPK(pagination, queryTime, company_id, emp_name, emp_category);
            return File(fileStream, "application/vnd.ms-excel", fileName);
        }
        /// <summary>
        /// 子表，需要时间参数，员工id
        /// </summary>
        [HttpGet]
        [HandlerAjaxOnly]
        public ActionResult PerfList(Pagination pagination, QueryTime queryTime, string emp_id)
        {
            var data = new
            {
                rows = app.PerfList(pagination, queryTime, emp_id),
                total = pagination.records
            };
            return Content(data.ToJson());
        }
        /// <summary>
        /// 父表，需要时间参数
        /// </summary>
        [HttpGet]
        [HandlerAjaxOnly]
        public ActionResult PerfMainList(Pagination pagination, QueryTime queryTime, string emp_name, int company_id = 0, int? emp_category = 0)
        {
            var data = new
            {
                rows = app.PerfMainList(pagination, queryTime, company_id, emp_name, emp_category),
                total = pagination.records
            };
            return Content(data.ToJson());
        }

        public FileResult PerfExport(Pagination pagination, QueryTime queryTime, string emp_name, int? company_id = 0, int? emp_category = 0)
        {
            string fileName = "业务考核奖励明细_" + DateTime.Now.ToString("yyyyMMddhhmmss") + ".xls";
            Stream fileStream = app.ExportExcelSalesPer(pagination, queryTime, company_id, emp_name, emp_category);
            return File(fileStream, "application/vnd.ms-excel", fileName);
        }

        [HttpGet]
        [HandlerAjaxOnly]
        public ActionResult GetTotalAttaining(QueryTime queryTime, string emp_name, int? company_id = 0, int? emp_category = 0)
        {
            var data = app.GetTotalAttaining(queryTime, company_id, emp_name, emp_category);
            return Content(data.ToJson());
        }
        [HttpGet]
        [HandlerAjaxOnly]
        public ActionResult GetTotalRecom(QueryTime queryTime, string emp_name, int? company_id = 0, int? emp_category = 0)
        {
            var data = app.GetTotalRecom(queryTime, company_id, emp_name, emp_category);
            return Content(data.ToJson());
        }
        [HttpGet]
        [HandlerAjaxOnly]
        public ActionResult GetTotalPK(QueryTime queryTime, string emp_name, int? company_id = 0, int? emp_category = 0)
        {
            var data = app.GetTotalPK(queryTime, company_id, emp_name, emp_category);
            return Content(data.ToJson());
        }
        [HttpGet]
        [HandlerAjaxOnly]
        public ActionResult GetTotalRank(QueryTime queryTime, string emp_name, int company_id = 0, int emp_category = 0)
        {
            var data = app.GetTotalRank(queryTime, company_id, emp_name, emp_category);
            return Content(data.ToJson());
        }

        [HttpGet]
        [HandlerAjaxOnly]
        public ActionResult GetTotalSalesPer(QueryTime queryTime, string emp_name, int? company_id = 0, int? emp_category = 0)
        {
            var data = app.GetTotalSalesPer(queryTime, company_id, emp_name, emp_category);
            return Content(data.ToJson());
        }
    }
}