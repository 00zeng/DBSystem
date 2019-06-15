using Base.Code;
using ProjectShare.Database;
using ProjectShare.Models;
using ProjectWeb.Areas.SalaryCalculate.Application;
using System;
using System.Collections.Generic;
using System.IO;
using System.Web.Mvc;

namespace ProjectWeb.Areas.SalaryCalculate.Controllers
{
    public class PayrollGuideController : ControllerBase
    {
        private PayrollGuideApp app = new PayrollGuideApp();

        //导出excel  
        public FileResult Export(Pagination pagination)
        {
            string fileName = "导购工资_" + DateTime.Now.ToString("yyyyMMddhhmmss") + ".xls";
            Stream fileStream = app.ExportExcel(pagination);
            return File(fileStream, "application/vnd.ms-excel", fileName);
        }
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

        /// <summary>
        /// 薪资结算列表
        /// </summary>
        /// <param name="pagination"></param>
        /// <param name="queryInfo"></param>
        /// <param name="show_all">默认获取当月未结算的，true-全部员工</param>
        /// <returns></returns>
        [HttpGet]
        [HandlerAjaxOnly]
        public ActionResult GetListCalculate(Pagination pagination, daoben_hr_emp_job queryInfo, QueryTime queryTime, bool show_all = false)
        {
            var data = new
            {
                rows = app.GetListCalculate(pagination, queryInfo, queryTime, show_all),
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
        public ActionResult Add(daoben_payroll_guide addInfo, List<daoben_payroll_guide_sub> subList)
        {
            string result = app.Add(addInfo, subList);
            if (result == "success")
                return Success("操作成功。", addInfo.id);
            else
                return Error(result);
        }
    }
}