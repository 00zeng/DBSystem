using System.Web.Mvc;
using Base.Code;
using ProjectWeb.Areas.DistributorManage.Application;
using ProjectShare.Database;
using System;
using System.Web;
using ProjectShare.Models;
using System.IO;
using System.Collections.Generic;
using NPOI.HSSF.UserModel;

namespace ProjectWeb.Areas.DistributorManage.Controllers
{
    public class ShippingBillController : ControllerBase
    {
        ShippingBillApp app = new ShippingBillApp();
        #region 页面显示
        [HttpGet]
        [HandlerAuthorize]
        public ActionResult History()
        {
            return View();
        }
        [HttpGet]
        [HandlerAuthorize]
        public ActionResult Import()
        {
            return View();
        }
        [HttpGet]
        [HandlerAuthorize]
        public ActionResult Now()
        {
            return View();
        }
        [HttpGet]
        [HandlerAuthorize]
        public ActionResult Setting()
        {
            return View();
        }
        [HttpGet]
        [HandlerAuthorize]
        public ActionResult NowIndex()
        {
            return View();
        }

        #endregion

        //导出excel
        public FileResult Export(Pagination pagination, QueryTime queryTime, string shipping_bill, string distributor_name,
            int? company_id = 0, int? total_count_min = null, int? total_count_max = null)
        {
            string fileName = "发货明细_" + DateTime.Now.ToString("yyyyMMddhhmmss") + ".xls";
            Stream fileStream = app.ExportExcel(pagination, queryTime, shipping_bill, distributor_name, company_id, total_count_min, total_count_max);
            return File(fileStream, "application/vnd.ms-excel", fileName);
        }
        /// <summary>
        /// 当前运单 - 发货明细
        /// </summary>
        [HttpGet]
        [HandlerAjaxOnly]
        public ActionResult GetEffectList(Pagination pagination, QueryTime queryTime, string shipping_bill, string distributor_name,
            int? company_id = 0, int? total_count_min = null, int? total_count_max = null)
        {
            var data = new
            {
                rows = app.GetEffectList(pagination, queryTime, shipping_bill, distributor_name, company_id, total_count_min, total_count_max),
                total = pagination.records
            };
            return Content(data.ToJson());
        }

        [HttpGet]
        [HandlerAjaxOnly]
        public ActionResult GetAllList(Pagination pagination, QueryTime queryTime, string queryName)
        {
            var data = new
            {
                rows = app.GetList(pagination, queryName, queryTime),
                total = pagination.records
            };
            return Content(data.ToJson());
        }
        [HttpGet]
        [HandlerAjaxOnly]
        public ActionResult GetApproveList(Pagination pagination, string queryName)
        {
            var data = new
            {
                rows = app.GetListApprove(pagination, queryName),
                total = pagination.records
            };
            return Content(data.ToJson());
        }

        /// <summary>
        /// 导入数据
        /// </summary>
        [HttpPost]
        [HandlerAjaxOnly]
        [ValidateAntiForgeryToken]
        public ActionResult Import(string importListStr, daoben_distributor_shipping importInfo)
        {
            List<daoben_distributor_shipping_detail> importList = importListStr.ToList<daoben_distributor_shipping_detail>();
            string result = app.Import(importList, importInfo);
            if (result == "success")
                return Success("导入成功。");
            else
                return Error(result);
        }
        /// <summary>
        /// 预留审批功能
        /// </summary>
        [HttpPost]
        [HandlerAjaxOnly]
        [ValidateAntiForgeryToken]
        public ActionResult Approve(daoben_distributor_shipping_approve approveInfo)
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
        /// 获取当前生效的运费模版，没有返回空值
        /// </summary>
        [HttpGet]
        [HandlerAjaxOnly]
        public ActionResult GetEffectInfo()
        {
            string data = app.GetEffectInfo();
            return Content(data);
        }
        /// <summary>
        /// 预留撤销功能
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

        /// <summary>
        /// 修改checkStatus
        /// </summary>
        /// <param name="idList">[ID1,ID2,ID3,..]</param>
        /// <param name="status">1线下处理，2删除</param>
        /// <returns></returns>
        //[HttpPost]
        //[HandlerAjaxOnly]
        //[ValidateAntiForgeryToken]
        //public ActionResult UpdateCheckStatus(List<int> idList, int status)
        //{
        //    string result = app.UpdateCheckStatus(idList, status);
        //    if (result == "success")
        //        return Success("success");
        //    else
        //        return Error(result);
        //}

    }
}