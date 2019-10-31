﻿using Base.Code;
using ProjectShare.Database;
using ProjectWeb.Areas.SalaryCalculate.Application;
using System;
using System.Collections.Generic;
using System.IO;
using System.Web.Mvc;

namespace ProjectWeb.Areas.SalaryCalculate.Controllers
{
    public class PayrollSalesController : ControllerBase
    {
        private PayrollSalesApp app = new PayrollSalesApp();
        //导出excel  
        public FileResult Export(Pagination pagination)
        {
            string fileName = "业务工资_" + DateTime.Now.ToString("yyyyMMddhhmmss") + ".xls";
            Stream fileStream = app.ExportExcel(pagination);
            return File(fileStream, "application/vnd.ms-excel", fileName);
        }
        [HttpGet]
        [HandlerAjaxOnly]
        public ActionResult GetListHistory(Pagination pagination, daoben_hr_emp_job queryInfo)
        {
            var data = new
            {
                rows = app.GetListHistory(pagination, queryInfo),
                total = pagination.records,
            };
            return Content(data.ToJson());
        }

        [HttpGet]
        [HandlerAjaxOnly]
        public ActionResult GetInfo(string id)
        {
            var data = app.GetInfo(id);
            return Content(data);
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

        [HttpGet]
        [HandlerAjaxOnly]
        public ActionResult GetListCalculate(Pagination pagination, daoben_hr_emp_job queryInfo, bool show_all = false)
        {
            var data = new
            {
                rows = app.GetListCalculate(pagination, queryInfo, show_all),
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
        public ActionResult Add(daoben_payroll_sales addInfo, List<daoben_payroll_sales_sub> subList, daoben_kpi_sales monthInfo)
        {
            string result = app.Add(addInfo, subList, monthInfo);
            if (result == "success")
                return Success("操作成功。", addInfo.id);
            else
                return Error(result);
        }
    }
}