using System.Web.Mvc;
using Base.Code;
using ProjectShare.Database;
using System;
using ProjectShare.Models;
using System.IO;
using System.Collections.Generic;
using ProjectWeb.Areas.ProductManage.Application;

namespace ProjectWeb.Areas.ProductManage.Controllers
{
    public class PriceInfoController : ControllerBase
    {
        private PriceInfoApp app = new PriceInfoApp();


        //导出excel  
        public FileResult Export(Pagination pagination, int company_id = 0, string model = null,
            int? price_wholesale_min = null, int? price_wholesale_max = null)
        {
            string fileName = "当前价格_" + DateTime.Now.ToString("yyyyMMddhhmmss") + ".xls";
            Stream fileStream = app.ExportExcel(pagination, company_id, model, price_wholesale_min, price_wholesale_max);
            return File(fileStream, "application/vnd.ms-excel", fileName);
        }

        [HttpGet]
        [HandlerAjaxOnly]
        public ActionResult GetListEffect(Pagination pagination, daoben_product_price queryInfo, int? price_wholesale_min = null,
                    int? price_wholesale_max = null)
        {
            var data = new
            {
                rows = app.GetListEffect(pagination, price_wholesale_min,price_wholesale_max, queryInfo),
                total = pagination.records,
            };
            return Content(data.ToJson());
        }
        [HttpGet]
        [HandlerAjaxOnly]
        public ActionResult GetListPriceHistory(Pagination pagination, daoben_product_price queryInfo, QueryTime queryTime)
        {
            var data = new
            {
                rows = app.GetListPriceHistory(pagination, queryInfo, queryTime),
                total = pagination.records,
            };
            return Content(data.ToJson());
        }

        [HttpGet]
        [HandlerAjaxOnly]
        public ActionResult GetListHistory(Pagination pagination, QueryTime queryTime, string name = null, int? status = null)
        {
            string result = app.GetListImport(pagination, name, queryTime, status, false);              
            return Content(result);
        }
        
        [HttpGet]
        [HandlerAjaxOnly]
        public ActionResult GetListApprove(Pagination pagination, QueryTime queryTime, string name = null, int? status = null)
        {           
            string result = app.GetListImport(pagination, name, queryTime, status, true);
            return Content(result);
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


        [HttpPost]
        [HandlerAjaxOnly]
        [ValidateAntiForgeryToken]
        public ActionResult Import(string importListStr, daoben_product_price_approve mainInfo,
                List<IntIdNamePair> companyList)
        {
            List<daoben_product_price> importList = importListStr.ToList<daoben_product_price>();
            string result = app.Import(importList, mainInfo, companyList);
            if (result == "success")
                return Success("导入成功。", mainInfo.import_file);
            else
                return Error(result);
        }

        [HttpPost]
        [HandlerAjaxOnly]
        [ValidateAntiForgeryToken]
        public ActionResult Approve(daoben_product_price_approve approveInfo)
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


        //[HttpGet]
        //[HandlerAjaxOnly]
        //public ActionResult GetProductTree()
        //{
        //    string data = app.GetProductTree();
        //    return Content(data);
        //}

        [HttpGet]
        [HandlerAjaxOnly]
        public ActionResult GetEffectIdNameList(int companyId = 0)
        {
            string data = app.GetEffectIdNameList(companyId);
            return Content(data);
        }

    }
}