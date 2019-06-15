using System.Web.Mvc;
using Base.Code;
using ProjectWeb.Areas.FinancialAccounting.Application;
using ProjectShare.Database;
using System.Collections.Generic;

namespace ProjectWeb.Areas.FinancialAccounting.Controllers
{
    public class StaySubsidyController : ControllerBase
    {
        private StaySubsidyApp app = new StaySubsidyApp();

        #region 页面显示
        [HttpGet]
        [HandlerAuthorize]
        public ActionResult EmployeeAdd()
        {
            return View();
        }
        [HttpGet]
        [HandlerAuthorize]
        public ActionResult EmployeeShow()
        {
            return View();
        }

        [HttpGet]
        [HandlerAuthorize]
        public ActionResult EmployeeEdit()
        {
            return View();
        }
        #endregion


        [HttpGet]
        [HandlerAjaxOnly]
        public ActionResult GetGridJson(Pagination pagination, daoben_salary_staysubsidy queryInfo)
        {
            var data = new
            {
                rows = app.GetListPage(pagination, queryInfo),
                total = pagination.records,
            };
            return Content(data.ToJson());
        }

        [HttpGet]
        [HandlerAjaxOnly]
        public ActionResult GetGridJson2(Pagination pagination)
        {
            var data = new
            {
                rows = app.GetListMyApprove(pagination),
                total = pagination.records,
            };
            return Content(data.ToJson());
        }

        [HttpPost]
        [HandlerAjaxOnly]
        [ValidateAntiForgeryToken]
        public ActionResult Add(daoben_salary_staysubsidy addInfo, List<daoben_salary_staysubsidy_emp> listEmp)
        {
            string result = app.Add(addInfo, listEmp);
            if (result == "success")
                return Success("操作成功。");
            else
                return Error(result);
        }


        [HttpPost]
        [HandlerAjaxOnly]
        [ValidateAntiForgeryToken]
        public ActionResult Approve(daoben_salary_staysubsidy_approve approveInfo)
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
            string data = app.GetInfo(id).ToJson();
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

    }
}