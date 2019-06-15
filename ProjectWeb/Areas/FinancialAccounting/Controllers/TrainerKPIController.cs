using System.Web.Mvc;
using Base.Code;
using ProjectWeb.Areas.FinancialAccounting.Application;
using ProjectShare.Database;
using System;
using ProjectShare.Models;
using System.Collections.Generic;

namespace ProjectWeb.Areas.FinancialAccounting.Controllers
{
    public class TrainerKPIController : ControllerBase
    {
        private TrainerKPIApp app = new TrainerKPIApp();

        /// <summary>
        /// KPI历史
        /// </summary>
        /// <param name="pagination"></param>
        /// <returns></returns>
        [HttpGet]
        [HandlerAjaxOnly]
        public ActionResult GetListHistory(Pagination pagination)
        {
            var data = new
            {
                rows = app.GetListHistory(pagination),
                total = pagination.records,
            };
            return Content(data.ToJson());
        }

        [HttpGet]
        [HandlerAjaxOnly]
        public ActionResult GetListApprove(Pagination pagination)
        {
            var data = new
            {
                rows = app.GetListApprove(pagination),
                total = pagination.records,
            };
            return Content(data.ToJson());
        }
        /// <summary>
        /// 薪资结算列表
        /// </summary>
        /// <param name="pagination"></param>
        /// <param name="queryInfo"></param>
        /// <param name="show_all">默认获取当月未结算的，true-全部员工</param>
        /// <returns></returns>
        [HttpGet]
        [HandlerAjaxOnly]
        public ActionResult GetListCalculate(Pagination pagination, daoben_hr_emp_job queryInfo, bool show_all = false)
        {
            var data = new
            {
                rows = app.GetListKPI(pagination, queryInfo, show_all),
                total = pagination.records,
            };
            return Content(data.ToJson());
        }
        [HttpGet]
        [HandlerAjaxOnly]
        public ActionResult GetGuideSaleList(Pagination pagination, daoben_product_sn queryInfo, DateTime month, string emp_id, string queryName)
        {
            var data = new
            {
                rows = app.GetGuideSaleList(pagination, queryInfo, month, emp_id, queryName),
                total = pagination.records,
            };
            return Content(data.ToJson());
        }

        [HttpGet]
        [HandlerAjaxOnly]
        public ActionResult GetHighLevelSaleList(Pagination pagination, daoben_product_sn queryInfo, DateTime month, string emp_id, int high_level = 0)
        {
            var data = new
            {
                rows = app.GetHighLevelSaleList(pagination, queryInfo, month, emp_id, high_level),
                total = pagination.records,
            };
            return Content(data.ToJson());
        }
        [HttpGet]
        [HandlerAjaxOnly]
        public ActionResult GetSettingInfo(string id)
        {
            var data = app.GetSettingInfo(id);
            return Content(data);
        }
        [HttpPost]
        [HandlerAjaxOnly]
        [ValidateAntiForgeryToken]
        public ActionResult Add(daoben_kpi_trainer addInfo, List<daoben_kpi_trainer_detail> detailList)
        {
            string result = app.Add(addInfo, detailList);
            if (result == "success")
                return Success("操作成功。");
            else
                return Error(result);
        }

        [HttpGet]
        [HandlerAjaxOnly]
        public ActionResult GetInfo(string id)
        {
            string data = app.GetInfo(id);
            return Content(data);
        }

        [HttpGet]
        [HandlerAjaxOnly]
        public ActionResult CalculateRatio(string id, DateTime month)
        {
            string data = app.CalculateRatio(id, month).ToJson();
            return Content(data);
        }
#if false
        [HttpPost]
        [HandlerAjaxOnly]
        [ValidateAntiForgeryToken]
        public ActionResult Approve(daoben_salary_kpi_approve approveInfo)
        {
            string result = app.Approve(approveInfo);
            if (result == "success")
                return Success("操作成功。");
            else
                return Error(result);
        }
#endif
        /// <summary>
        /// 撤销申请
        /// </summary>
        [HttpPost]
        [HandlerAjaxOnly]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(string id)
        {
            if (string.IsNullOrEmpty(id))
                return Error("ID不能为空");
            string result = app.Delete(id);
            if (result == "success")
                return Success("操作成功。", id);
            else
                return Error(result);
        }

    }
}