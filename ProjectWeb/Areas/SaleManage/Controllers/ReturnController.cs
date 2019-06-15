using System.Web.Mvc;
using Base.Code;
using ProjectWeb.Areas.SaleManage.Application;
using ProjectShare.Database;
using System;
using System.Web;
using ProjectShare.Models;
using System.IO;
using System.Collections.Generic;

namespace ProjectWeb.Areas.SaleManage.Controllers
{
    public class ReturnController : ControllerBase
    {
        private ReturnApp app = new ReturnApp();
        [HttpGet]
        [HandlerAuthorize]
        public ActionResult History()
        {
            return View();
        }


        [HttpGet]
        [HandlerAjaxOnly]
        public ActionResult GetGridJson(Pagination pagination, daoben_sale_return queryInfo,
                    QueryTime queryTime)
        {






            var data = new
            {
                rows = app.GetList(pagination, queryInfo, queryTime),
                total = pagination.records,
            };
            return Content(data.ToJson());
        }

        [HttpGet]
        [HandlerAjaxOnly]
        public ActionResult GetGridJson2(Pagination pagination, QueryTime queryTime, string queryName = null)
        {
            var data = new
            {
                rows = app.GetListHistory(pagination, queryName, queryTime),
                total = pagination.records,
            };
            return Content(data.ToJson());
        }
        [HttpGet]
        [HandlerAjaxOnly]
        public ActionResult GetGridJson3(Pagination pagination, string queryName = null)
        {
            var data = new
            {
                rows = app.GetListApprove(pagination, queryName),
                total = pagination.records,
            };
            return Content(data.ToJson());
        }

        [HttpPost]
        [HandlerAjaxOnly]
        [ValidateAntiForgeryToken]
        public ActionResult Import(string importListStr, daoben_sale_return_approve importInfo)
        {
            List<daoben_sale_return> importList = importListStr.ToList<daoben_sale_return>();
            int delCount = 0;
            string result = app.Import(importList, importInfo, ref delCount);
            if (result == "success")
            {
                if (delCount > 0)
                    return Success("导入成功，其中有【" + delCount.ToString() + "】重复数据已删除", importInfo.import_file);
                return Success("导入成功。", importInfo.import_file);
            }
            else
                return Error(result);
        }
        [HttpPost]
        [HandlerAjaxOnly]
        [ValidateAntiForgeryToken]
        public ActionResult Approve(daoben_sale_return_approve approveInfo)
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

        [HttpGet]
        [HandlerAjaxOnly]
        public ActionResult GetDistributorList()
        {
            string data = app.GetDistributorList();
            return Content(data);
        }

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


        /// <summary>
        /// 修改checkStatus
        /// </summary>
        /// <param name="idList">[ID1,ID2,ID3,..]</param>
        /// <param name="status">1线下处理，2删除</param>
        /// <returns></returns>
        [HttpPost]
        [HandlerAjaxOnly]
        [ValidateAntiForgeryToken]
        public ActionResult UpdateCheckStatus(List<int> idList, int status)
        {
            string result = app.UpdateCheckStatus(idList, status);
            if (result == "success")
                return Success("success");
            else
                return Error(result);
        }
        /// <summary>
        /// 保留数据，将子表id这条数据置为1，其他相同串码置为23
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        [HandlerAjaxOnly]
        [ValidateAntiForgeryToken]
        public ActionResult Keeping(int id)
        {
            string result = app.Keeping(id);
            if (result == "success")
                return Success("success");
            else
                return Error(result);
        }
    }
}