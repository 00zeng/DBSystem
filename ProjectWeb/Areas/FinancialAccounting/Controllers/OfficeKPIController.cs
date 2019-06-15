using System.Web.Mvc;
using Base.Code;
using ProjectWeb.Areas.FinancialAccounting.Application;
using ProjectShare.Database;
using System;
using ProjectShare.Models;

namespace ProjectWeb.Areas.FinancialAccounting.Controllers
{
    public class OfficeKPIController : ControllerBase
    {
        private OfficeKPIApp app = new OfficeKPIApp();       

        [HttpGet]
        [HandlerAjaxOnly]
        public ActionResult GetListHistory(Pagination pagination, daoben_hr_emp_job queryInfo, QueryTime queryTime, int approve_status = 0)
        {
            var data = new
            {
                rows = app.GetListHistory(pagination, queryInfo, approve_status, queryTime),
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
                rows = app.GetListApprove(pagination, queryInfo, queryTime),
                total = pagination.records,
            };
            return Content(data.ToJson());
        }
        [HttpGet]
        [HandlerAjaxOnly]
        public ActionResult GetList(Pagination pagination, daoben_hr_emp_job queryInfo, QueryTime queryTime, bool show_all = false)
        {
            var data = new
            {
                rows = app.GetList(pagination, queryInfo, queryTime, show_all),
                total = pagination.records,
            };
            return Content(data.ToJson());
        }
        [HttpPost]
        [HandlerAjaxOnly]
        [ValidateAntiForgeryToken]
        public ActionResult Add(daoben_salary_kpi addInfo)
        {
            string result = app.Add(addInfo);
            if (result == "success")
                return Success("操作成功。");
            else
                return Error(result);
        }
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

        [HttpGet]
        [HandlerAjaxOnly]
        public ActionResult GetInfo(string id)
        {
            string data = app.GetInfo(id);
            return Content(data);
        }


        [HttpGet]
        [HandlerAjaxOnly]
        public ActionResult GetOrigInfo(string emp_id, DateTime month)
        {
            string data = app.GetOrigInfo(emp_id, month);
            return Content(data);
        }
        /// <summary>
        /// 根据员工ID获取员工职位信息和KPI补贴
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [HandlerAjaxOnly]
        public ActionResult GetKpiSubInfo(string id)
        {
            string data = app.GetKpiSubInfo(id);
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

        [HttpGet]
        [HandlerAjaxOnly]
        public ActionResult GetSettingInfo(string id)
        {
            var data = app.GetSettingInfo(id);
            return Content(data);
        }

    }
}