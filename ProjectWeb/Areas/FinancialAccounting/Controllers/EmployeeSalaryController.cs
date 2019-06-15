using System.Web.Mvc;
using Base.Code;
using ProjectWeb.Areas.FinancialAccounting.Application;
using ProjectShare.Database;
using System.Collections;
using ProjectWeb.Areas.HumanResource.Application;
using System;
using ProjectShare.Models;
using System.Collections.Generic;

namespace ProjectWeb.Areas.FinancialAccounting.Controllers
{
    public class EmployeeSalaryController : ControllerBase
    {
        private EmployeeSalaryApp app = new EmployeeSalaryApp();

        #region 页面显示
        [HttpGet]
        [HandlerAuthorize]
        public ActionResult OfficeSetting()
        {
            return View();
        }
        [HttpGet]
        [HandlerAuthorize]
        public ActionResult OfficeIndex()
        {
            return View();
        }
        [HttpGet]
        [HandlerAuthorize]
        public ActionResult SubsidyIndex()
        {
            return View();
        }
        [HttpGet]
        [HandlerAuthorize]
        public ActionResult SalesSetting()
        {
            return View();
        }
        [HttpGet]
        [HandlerAuthorize]
        public ActionResult SalesShow()
        {
            return View();
        }

        [HttpGet]
        [HandlerAuthorize]
        public ActionResult TrainerSetting()
        {
            return View();
        }

        [HttpGet]
        [HandlerAuthorize]
        public ActionResult TrainerIndex()
        {
            return View();
        }
        [HttpGet]
        [HandlerAuthorize]
        public ActionResult SubsidySetting()
        {
            return View();
        }
        [HttpGet]
        [HandlerAuthorize]
        public ActionResult SubsidyShow()
        {
            return View();
        }
        [HttpGet]
        [HandlerAuthorize]
        public ActionResult SubsidyApprove()
        {
            return View();
        }
        [HttpGet]
        [HandlerAuthorize]
        public ActionResult KPISubsidySetting()
        {
            return View();
        }
        [HttpGet]
        [HandlerAuthorize]
        public ActionResult KPISubsidyShow()
        {
            return View();
        }
        [HttpGet]
        [HandlerAuthorize]
        public ActionResult KPISubsidyApprove()
        {
            return View();
        }
        [HttpGet]
        [HandlerAuthorize]
        public ActionResult GuideSetting()
        {
            return View();
        }
        [HttpGet]
        [HandlerAuthorize]
        public ActionResult GuideShow()
        {
            return View();
        }
        [HttpGet]
        [HandlerAuthorize]
        public ActionResult GuideApprove()
        {
            return View();
        }
        [HttpGet]
        [HandlerAuthorize]
        public ActionResult TrainerApprove()
        {
            return View();
        }
        [HttpGet]
        [HandlerAuthorize]
        public ActionResult OfficeShow()
        {
            return View();
        }
        [HttpGet]
        [HandlerAuthorize]
        public ActionResult TrainerShow()
        {
            return View();
        }
        [HttpGet]
        [HandlerAuthorize]
        public ActionResult OfficeApprove()
        {
            return View();
        }
        [HttpGet]
        [HandlerAuthorize]
        public ActionResult TrainerCheck()
        {
            return View();
        }
        [HttpGet]
        [HandlerAuthorize]
        public ActionResult TrainerCheckSetting()
        {
            return View();
        }
        [HttpGet]
        [HandlerAuthorize]
        public ActionResult TrainerCheckShow()
        {
            return View();
        }
        [HttpGet]
        [HandlerAuthorize]
        public ActionResult TrainerCheckApprove()
        {
            return View();
        }
        [HttpGet]
        [HandlerAuthorize]
        public ActionResult GuideHistoryShow()
        {
            return View();
        }
        [HttpGet]
        [HandlerAuthorize]
        public ActionResult SalesHistoryShow()
        {
            return View();
        }
        [HttpGet]
        [HandlerAuthorize]
        public ActionResult OfficeHistoryShow()
        {
            return View();
        }
        public ActionResult KPISubsidyHistoryShow()
        {
            return View();
        }
        [HttpGet]
        [HandlerAuthorize]
        public ActionResult SubsidyHistoryShow()
        {
            return View();
        }
        [HttpGet]
        [HandlerAuthorize]
        public ActionResult TrainerHistoryShow()
        {
            return View();
        }
        public ActionResult TrainerSalaryHistoryShow()
        {
            return View();
        }
        #endregion

        #region 查看 

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
        /// 历史记录
        /// </summary>
        [HttpGet]
        [HandlerAjaxOnly]
        public ActionResult GetHistoryList(Pagination pagination, daoben_hr_emp_job queryInfo, QueryTime queryTime, int category = 0)
        {
            var data = new
            {
                rows = app.GetHistoryList(pagination, queryInfo, category, queryTime),
                total = pagination.records,
            };
            return Content(data.ToJson());
        }


        [HttpGet]
        [HandlerAjaxOnly]
        public ActionResult GetOfficeList(Pagination pagination, daoben_hr_emp_job queryInfo, QueryTime queryTime)
        {
            var data = new
            {
                rows = app.GetOfficeList(pagination, queryInfo, queryTime),
                total = pagination.records,
            };
            return Content(data.ToJson());
        }
        [HttpGet]
        [HandlerAjaxOnly]
        public ActionResult GetTrainerList(Pagination pagination, daoben_hr_emp_job queryInfo, QueryTime queryTime)
        {
            var data = new
            {
                rows = app.GetTrainerList(pagination, queryInfo, queryTime),
                total = pagination.records,
            };
            return Content(data.ToJson());
        }
        [HttpGet]
        [HandlerAjaxOnly]
        public ActionResult GetTrainerKPIList(Pagination pagination, daoben_hr_emp_job queryInfo, QueryTime queryTime)
        {
            var data = new
            {
                rows = app.GetTrainerList(pagination, queryInfo, queryTime),
                total = pagination.records,
            };
            return Content(data.ToJson());
        }

        [HttpGet]
        [HandlerAjaxOnly]
        public ActionResult GetGridJson4(Pagination pagination, daoben_hr_emp_job queryInfo)
        {
            var data = new
            {
                rows = app.GetSubList(pagination, queryInfo),
                total = pagination.records,
            };
            return Content(data.ToJson());
        }

        #endregion


        [HttpPost]
        [HandlerAjaxOnly]
        [ValidateAntiForgeryToken]
        public ActionResult AddGeneral(daoben_salary_emp addInfo, daoben_salary_emp_general addGeneralInfo)
        {
            string result = app.AddGeneral(addInfo, addGeneralInfo);
            if (result == "success")
                return Success("操作成功。");
            else
                return Error(result);
        }

        [HttpPost]
        [HandlerAjaxOnly]
        [ValidateAntiForgeryToken]
        public ActionResult AddSales(daoben_salary_emp mainInfo, List<IdNamePair> empList, daoben_salary_emp_sales addSalesInfo, List<daoben_salary_emp_sales_sub> subList)
        {
            string result = app.AddSales(mainInfo, empList, addSalesInfo, subList);
            if (result == "success")
                return Success("操作成功。");
            else
                return Error(result);
        }
        [HttpPost]
        [HandlerAjaxOnly]
        [ValidateAntiForgeryToken]
        public ActionResult AddTrainer(daoben_salary_emp addInfo, List<daoben_salary_emp_trainer> subList)
        {
            string result = app.AddTrainer(addInfo, subList);
            if (result == "success")
                return Success("操作成功。");
            else
                return Error(result);
        }
        [HttpPost]
        [HandlerAjaxOnly]
        [ValidateAntiForgeryToken]
        public ActionResult AddGuide(List<IdNamePair> empList, daoben_salary_emp mainInfo, daoben_salary_emp_general addGeneral)
        {
            string result = app.AddGuide(empList, mainInfo, addGeneral);
            if (result == "success")
                return Success("操作成功。");
            else
                return Error(result);
        }
        [HttpPost]
        [HandlerAjaxOnly]
        [ValidateAntiForgeryToken]
        public ActionResult AddKpi(daoben_salary_emp addInfo, daoben_salary_emp_kpi_subsidy subsidyInfo)
        {
            string result = app.AddKpi(addInfo, subsidyInfo);
            if (result == "success")
                return Success("操作成功。");
            else
                return Error(result);
        }

        [HttpPost]
        [HandlerAjaxOnly]
        [ValidateAntiForgeryToken]
        public ActionResult AddSubsidy(daoben_salary_emp addInfo, daoben_salary_emp_subsidy subsidyInfo)
        {
            string result = app.AddSubsidy(addInfo, subsidyInfo);
            if (result == "success")
                return Success("操作成功。");
            else
                return Error(result);
        }

        /// <summary>
        /// 申请表ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [HandlerAjaxOnly]
        public ActionResult GetSalaryInfo(string id)
        {
            string data = app.GetSalaryInfo(id);
            return Content(data);
        }
        /// <summary>
        /// 员工ID
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
        public ActionResult Approve(daoben_salary_emp_approve ApproveInfo)
        {
            string result = app.Approve(ApproveInfo);
            if (result == "success")
                return Success(result);
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
        [HttpGet]
        [HandlerAjaxOnly]
        public ActionResult GetPosSalary(string id)
        {
            string data = app.GetPosSalary(id).ToString();
            return Content(data);
        }


        [HttpGet]
        [HandlerAjaxOnly]
        public ActionResult GetTrainerSetInfo(string id)
        {
            string data = app.GetTrainerSetInfo(id).ToString();
            return Content(data);
        }
    }
}