using System.Web.Mvc;
using Base.Code;
using ProjectShare.Database;
using System.Collections.Generic;
using ProjectShare.Models;
using System;
using System.IO;
using ProjectWeb.Areas.ProductManage.Application;

namespace ProjectWeb.Areas.ProductManage.Controllers
{
    public class CommissionInfoController : ControllerBase
    {
        private CommissionInfoApp app = new CommissionInfoApp();


        //导出excel  
        public FileResult Export(Pagination pagination, QueryTime queryTime, int company_id = 0, string model = null,
            int guide_commission_min = 0, int guide_commission_max = 0, int exclusive_commission_min = 0, int exclusive_commission_max = 0)
        {
            string fileName = "提成信息_" + DateTime.Now.ToString("yyyyMMddhhmmss") + ".xls";
            Stream fileStream = app.ExportExcel(pagination, queryTime, company_id, model, guide_commission_min, guide_commission_max, exclusive_commission_min, exclusive_commission_max);
            return File(fileStream, "application/vnd.ms-excel", fileName);
        }
        [HttpGet]
        [HandlerAjaxOnly]
        public ActionResult GetListEffect(Pagination pagination, QueryTime queryTime, int company_id = 0, string model = null,
            int guide_commission_min = 0, int guide_commission_max = 0, int exclusive_commission_min = 0, int exclusive_commission_max = 0)
        {
            var data = new
            {
                rows = app.GetListEffect(pagination,queryTime, company_id, model,guide_commission_min,guide_commission_max,exclusive_commission_min,exclusive_commission_max),
                total = pagination.records,
            };
            return Content(data.ToJson());
        }
        [HttpGet]
        [HandlerAjaxOnly]
        public ActionResult GetListCommissionHistory(Pagination pagination, QueryTime queryTime, int company_id = 0, string model = null,
            int guide_commission_min = 0, int guide_commission_max = 0, int exclusive_commission_min = 0, int exclusive_commission_max = 0)
        {
            var data = new
            {
                rows = app.GetListCommissionHistory(pagination, queryTime, company_id, model, guide_commission_min, guide_commission_max, exclusive_commission_min, exclusive_commission_max),
                total = pagination.records,
            };
            return Content(data.ToJson());
        }

        [HttpGet]
        [HandlerAjaxOnly]
        public ActionResult GetListHistory(Pagination pagination, QueryTime queryTime, int? effect_status = null, string name = null)
        {
            //var data = new
            //{
            //    rows = app.GetListImport(pagination, name, queryTime, false, effect_status),
            //    total = pagination.records,
            //};
            string result = app.GetListImport(pagination, name, queryTime, false, effect_status);
            return Content(result);
        }

        [HttpGet]
        [HandlerAjaxOnly]
        public ActionResult GetListApprove(Pagination pagination, QueryTime queryTime, string name = null)
        {
            //var data = new
            //{
            //    rows = app.GetListImport(pagination, name, queryTime, true, null),
            //    total = pagination.records,
            //};
            //return Content(data.ToJson());
            string result = app.GetListImport(pagination, name, queryTime, true, null);
            return Content(result);
        }

        [HttpPost]
        [HandlerAjaxOnly]
        [ValidateAntiForgeryToken]
        public ActionResult Import(string importListStr, daoben_product_commission mainInfo,
                List<IntIdNamePair> companyList)
        {
            List<daoben_product_commission_detail> importList = importListStr.ToList<daoben_product_commission_detail>();
            string result = app.Import(importList, mainInfo, companyList);
            if (result == "success")
                return Success("导入成功。", mainInfo.import_file);
            else
                return Error(result);
        }



        [HttpPost]
        [HandlerAjaxOnly]
        [ValidateAntiForgeryToken]
        public ActionResult Approve(daoben_product_commission_approve approveInfo)
        {
            string result = app.Approve(approveInfo);
            if (result == "success")
                return Success("操作成功。");
            else
                return Error(result);
        }

        [HttpGet]
        [HandlerAjaxOnly]
        public ActionResult GetInfoMain(string id)
        {
            string data = app.GetInfoMain(id);
            return Content(data);
        }
        [HttpGet]
        [HandlerAjaxOnly]
        public ActionResult GetInfoPage(Pagination pagination, string id, string queryStr = null)
        {
            var data = new
            {
                rows = app.GetInfoPage(pagination, id, queryStr),
                total = pagination.records
            };
            return Content(data.ToJson());
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