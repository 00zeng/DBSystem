using Base.Code;
using ProjectShare.Database;
using ProjectWeb.Areas.SalaryCalculate.Application;
using System;
using System.Web.Mvc;

namespace ProjectWeb.Areas.SalaryCalculate.Controllers
{
    public class PayrollSettingController : ControllerBase
    {
        private PayrollSettingApp app = new PayrollSettingApp();

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
        public ActionResult GetInfoByID(int id = 0)
        {
            var data = app.GetInfoByID(id);
            return Content(data.ToJson());
        }

        [HttpGet]
        [HandlerAjaxOnly]
        public ActionResult GetPayrollMonth()
        {
            var data = app.GetPayrollMonth();
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

        [HttpPost]
        [HandlerAjaxOnly]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(daoben_payroll_setting areaInfo)
        {
            string result = app.Edit(areaInfo);
            if (result == "success")
                return Success("设置成功，请等待财务经理审批", areaInfo.id);
            else
                return Error(result);
        }

        [HttpPost]
        [HandlerAjaxOnly]
        [ValidateAntiForgeryToken]
        public ActionResult Approve(daoben_payroll_setting approveInfo)
        {
            string result = app.Approve(approveInfo);
            if (result == "success")
                return Success("审批成功，本月结算日已设置为【" + ((DateTime)approveInfo.end_date).ToString("yyyy-MM-dd") + "】");
            else
                return Error(result);
        }
    }
}