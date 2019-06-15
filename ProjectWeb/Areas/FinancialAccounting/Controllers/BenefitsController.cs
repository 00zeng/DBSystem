using System.Web.Mvc;
using Base.Code;
using ProjectWeb.Areas.FinancialAccounting.Application;
using ProjectShare.Database;
using System.Collections.Generic;
using ProjectWeb.Areas.HumanResource.Application;

namespace ProjectWeb.Areas.FinancialAccounting.Controllers
{
    public class BenefitsController : ControllerBase
    {
        private BenefitsApp app = new BenefitsApp();

        #region 页面显示
        [HttpGet]
        [HandlerAuthorize]
        public ActionResult Benefits()
        {
            return View();
        }
        [HttpGet]
        [HandlerAuthorize]
        public ActionResult editNew()
        {
            return View();
        }
        #endregion
        [HttpGet]
        [HandlerAjaxOnly]
        public ActionResult GetGridJson(Pagination pagination,daoben_hr_emp_job queryInfo, int approve_status=0)
        {
            var data = new
            {
                rows = app.GetList(pagination, queryInfo,approve_status),
                total = pagination.records,
            };
            return Content(data.ToJson());
        }
        [HttpGet]
        [HandlerAjaxOnly]
        public ActionResult GetListMyApprove(Pagination pagination, int company_id = 0)
        {
            var data = new
            {
                rows = app.GetListMyApprove(pagination, company_id),
                total = pagination.records,
            };
            return Content(data.ToJson());
        }


        /// <summary>
        /// 获取当前登录人的机构下全体部门或区域，和职位信息
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [HandlerAjaxOnly]
        public ActionResult GetAreaDeptPosEmpList()
        {
            EmployeeManageApp empApp = new EmployeeManageApp();
            string data = empApp.GetAreaDeptPosEmpList();
            return Content(data);
        }


        [HttpPost]
        [HandlerAjaxOnly]
        [ValidateAntiForgeryToken]
        public ActionResult Add(daoben_salary_benefit addInfo, List<daoben_salary_benefit_emp> empInfoList,
            List<daoben_salary_benefit_detail> addBenefitsList)
        {

            string result = app.Add(addInfo, empInfoList, addBenefitsList);
            if (result == "success")
                return Success("操作成功。");
            else
                return Error(result);
        }

        [HttpPost]
        [HandlerAjaxOnly]
        [ValidateAntiForgeryToken]
        public ActionResult Approve(daoben_salary_benefit_approve approveInfo)
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