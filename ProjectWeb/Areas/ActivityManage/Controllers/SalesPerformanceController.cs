using System.Web.Mvc;
using Base.Code;
using ProjectWeb.Areas.ActivityManage.Application;
using ProjectShare.Database;
using System;
using ProjectShare.Models;
using System.Collections.Generic;
using System.IO;

namespace ProjectWeb.Areas.ActivityManage.Controllers
{
    public class SalesPerformanceController : ControllerBase
    {
        private SalesPerformanceApp app = new SalesPerformanceApp();

        #region 页面显示
        [HttpGet]
        [HandlerAuthorize]
        public ActionResult SalesPerformanceApprove()
        {
            return View();
        }
        [HttpGet]
        [HandlerAuthorize]
        public ActionResult SalesPerformanceShow()
        {
            return View();
        }

        [HttpGet]
        [HandlerAuthorize]
        public ActionResult SalesPerformanceRequest()
        {
            return View();
        }
        [HttpGet]
        [HandlerAuthorize]
        public ActionResult SalesPerformanceCurrent()
        {
            return View();
        }
        [HttpGet]
        [HandlerAuthorize]
        public ActionResult MonthlyPerfAdd()
        {
            return View();
        }
        [HttpGet]
        [HandlerAuthorize]
        public ActionResult MonthlyPerfShow()
        {
            return View();
        }

        [HttpGet]
        [HandlerAuthorize]
        public ActionResult SalePerfAdd()
        {
            return View();
        }

        [HttpGet]
        [HandlerAuthorize]
        public ActionResult SalePerfShow()
        {
            return View();
        }
        [HttpGet]
        [HandlerAuthorize]
        public ActionResult SalePerfShowActivity()
        {
            return View();
        }
        [HttpGet]
        [HandlerAuthorize]
        public ActionResult TeamPerfAdd()
        {
            return View();
        }
        [HttpGet]
        [HandlerAuthorize]
        public ActionResult TeamPerfShow()
        {
            return View();
        }
        public ActionResult TeamPerfShowActivity()
        {
            return View();
        }
        #endregion

        [HttpGet]
        [HandlerAjaxOnly]
        public ActionResult GetList(Pagination pagination, daoben_activity_sales_perf queryInfo, QueryTime queryTime)
        {
            var data = new
            {
                rows = app.GetList(pagination, queryInfo, queryTime),
                total = pagination.records,
            };
            return Content(data.ToJson());
        }

        [HttpGet]
        [HandlerAjaxOnly]
        public ActionResult GetListApprove(Pagination pagination, daoben_hr_emp_job queryInfo, QueryTime queryTime)
        {
            var data = new
            {
                rows = app.GetListApprove(pagination, queryInfo),
                total = pagination.records,
            };
            return Content(data.ToJson());
        }

        [HttpGet]
        [HandlerAjaxOnly]
        public ActionResult GetListRequest(Pagination pagination, daoben_hr_emp_job queryInfo)
        {
            var data = new
            {
                rows = app.GetListRequest(pagination, queryInfo),
                total = pagination.records,
            };
            return Content(data.ToJson());
        }
        /// <summary>
        /// 销售详情列表
        /// </summary>
        /// <param name="pagination"></param>
        /// <returns></returns>
        [HttpGet]
        [HandlerAjaxOnly]
        public ActionResult GetSaleList(Pagination pagination, daoben_product_sn queryInfo, daoben_activity_sales_perf queryMainInfo)
        {
            var data = new
            {
                rows = app.GetSaleList(pagination, queryInfo, queryMainInfo),
                total = pagination.records,
            };
            return Content(data.ToJson());
        }

        [HttpGet]
        [HandlerAjaxOnly]
        public ActionResult GetInfo(string id)
        {
            var data = app.GetInfo(id);
            return Content(data.ToJson());
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="addInfo">daoben_activity_sales_perf 主表数据</param>
        /// <param name="productList">daoben_activity_sales_perf_product 参与机型 （全部机型可以为空）</param>
        /// <param name="empList">daoben_activity_recommendation_emp 参与员工 不能为空</param>
        /// <returns></returns>
        [HttpPost]
        [HandlerAjaxOnly]
        [ValidateAntiForgeryToken]
        public ActionResult Add(daoben_activity_sales_perf addInfo, List<daoben_activity_sales_perf_product> productList, List<daoben_activity_sales_perf_emp> empList)
        {
            string result = app.Add(addInfo, empList, productList);
            if (result == "success")
                return Success("操作成功。");
            else
                return Error(result);
        }
        /// <summary>
        /// 修改结束时间
        /// </summary>
        /// <param name="id"></param>
        /// <param name="alterDate"></param>
        /// <returns></returns>
        [HttpPost]
        [HandlerAjaxOnly]
        [ValidateAntiForgeryToken]
        public ActionResult Alter(string id, DateTime alterDate)
        {
            string result = app.Alter(id, alterDate);
            if (result == "success")
                return Success("操作成功。", id);
            else
                return Error(result);
        }

        [HttpPost]
        [HandlerAjaxOnly]
        [ValidateAntiForgeryToken]
        public ActionResult Recall(string id)
        {
            string result = app.Recall(id);
            if (result == "success")
                return Success("操作成功。", id);
            else
                return Error(result);
        }

        #region 审批
        [HttpPost]
        [HandlerAjaxOnly]
        [ValidateAntiForgeryToken]
        public ActionResult Approve(daoben_activity_sales_perf_approve approve_info)
        {
            string result = app.Approve(approve_info);
            if (result == "success")
                return Success("操作成功。", approve_info.id);
            else
                return Error(result);
        }



        #endregion


    }
}