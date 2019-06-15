using System.Web.Mvc;
using Base.Code;
using ProjectWeb.Areas.FinancialAccounting.Application;
using ProjectShare.Database;
using System;
using ProjectShare.Models;
using System.Collections.Generic;
using System.IO;

namespace ProjectWeb.Areas.FinancialAccounting.Controllers
{
    public class SalesKPIController : ControllerBase
    {
        private SalesKPIApp app = new SalesKPIApp();


        /// <summary>
        /// KPI历史
        /// </summary>
        /// <param name="pagination"></param>
        /// <returns></returns>
        [HttpGet]
        [HandlerAjaxOnly]
        public ActionResult GetListHistory(Pagination pagination, daoben_hr_emp_job queryInfo, QueryTime queryTime)
        {
            var data = new
            {
                rows = app.GetListHistory(pagination, queryInfo, queryTime),
                total = pagination.records,
            };
            return Content(data.ToJson());
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pagination"></param>
        /// <param name="salesKPIId"></param>
        /// <param name="queryInfo"></param>
        /// <param name="origin">1-查看页面（~sn）；2-提交页面（~sn_temp）</param>
        /// <returns></returns>
        [HttpGet]
        [HandlerAjaxOnly]
        public ActionResult GetSaleList(Pagination pagination, string salesKPIId, daoben_product_sn queryInfo, int origin)
        {
            var data = new
            {
                rows = app.GetSaleList(pagination, salesKPIId, queryInfo, origin),
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
        public ActionResult GetSettingInfo(string id, DateTime? month = null)
        {
            var data = app.GetSettingInfo(id, month);
            return Content(data);
        }
        [HttpPost]
        [HandlerAjaxOnly]
        [ValidateAntiForgeryToken]
        public ActionResult Add(daoben_kpi_sales addInfo)
        {
            string result = app.Add(addInfo);
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pagination"></param>
        /// <param name="queryInfo"></param>
        /// <param name="origin">来源：1-daoben_product_sn_outlay；2-daoben_kpi_sales_sn_temp</param>
        /// <returns></returns>
        public FileResult ExportExcel(Pagination pagination, daoben_kpi_sales queryInfo, int origin)
        {
            string fileName = "业务员提成明细_" + DateTime.Now.ToString("yyyyMMddhhmmss") + ".xls";
            Stream fileStream = app.ExportExcel(pagination, queryInfo, origin);
            return File(fileStream, "application/vnd.ms-excel", fileName);
        }

    }
}