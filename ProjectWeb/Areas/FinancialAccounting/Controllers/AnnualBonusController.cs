using System.Web.Mvc;
using Base.Code;
using ProjectWeb.Areas.FinancialAccounting.Application;
using ProjectShare.Database;
using System;
using ProjectShare.Models;

namespace ProjectWeb.Areas.FinancialAccounting.Controllers
{
    public class AnnualBonusController : ControllerBase
    {
        private AnnualBonusApp app = new AnnualBonusApp();
        [HttpGet]
        [HandlerAuthorize]
        public ActionResult GuideBonus()
        {
            return View();
        }
        [HttpGet]
        [HandlerAuthorize]
        public ActionResult GuideBonusDetail()
        {
            return View();
        }

        /// <summary>
        /// 年终奖列表
        /// </summary>
        [HttpGet]
        [HandlerAjaxOnly]
        public ActionResult GetGridJson(Pagination pagination, daoben_hr_emp_job queryInfo, QueryTime queryTime, int approve_status = 0)
        {
            var data = new
            {
                rows = app.GetGridJson(pagination, queryInfo, approve_status, queryTime),
                total = pagination.records,
            };
            return Content(data.ToJson());
        }
        /// <summary>
        /// 我的审批，年终奖列表
        /// </summary>
        [HttpGet]
        [HandlerAjaxOnly]
        public ActionResult GetGridJson2(Pagination pagination, daoben_hr_emp_job queryInfo)
        {
            var data = new
            {
                rows = app.GetGridJson2(pagination, queryInfo),
                total = pagination.records,
            };
            return Content(data.ToJson());
        }
        /// <summary>
        /// 员工列表
        /// </summary>
        [HttpGet]
        [HandlerAjaxOnly]
        public ActionResult GetGridJson3(Pagination pagination, daoben_hr_emp_job queryInfo)
        {
            var data = new
            {
                rows = app.GetGridJson3(pagination, queryInfo),
                total = pagination.records,
            };
            return Content(data.ToJson());
        }


        [HttpPost]
        [HandlerAjaxOnly]
        [ValidateAntiForgeryToken]
        public ActionResult Add(daoben_salary_annualbonus addInfo)
        {
            string result = app.Add(addInfo);
            if (result == "success")
                return Success("操作成功。");
            else
                return Error(result);
        }

        /// <summary>
        /// 根据年终奖表ID获取奖罚信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [HandlerAjaxOnly]
        public ActionResult GetInfo(string id)
        {
            string data = app.GetInfo(id);
            return Content(data);
        }

        [HttpPost]
        [HandlerAjaxOnly]
        [ValidateAntiForgeryToken]
        public ActionResult Approve(daoben_salary_annualbonus_approve approveInfo)
        {
            string result = app.Approve(approveInfo);
            if (result == "success")
                return Success("操作成功。");
            else
                return Error(result);
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
    }
}