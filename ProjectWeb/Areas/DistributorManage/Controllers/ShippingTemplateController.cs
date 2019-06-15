using System.Web.Mvc;
using Base.Code;
using ProjectWeb.Areas.DistributorManage.Application;
using ProjectShare.Database;
using System;
using System.Web;
using ProjectShare.Models;
using System.IO;
using System.Collections.Generic;

namespace ProjectWeb.Areas.DistributorManage.Controllers
{
    public class ShippingTemplateController : ControllerBase
    {
        private ShippingTemplateApp app = new ShippingTemplateApp();
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

        [HttpGet]
        [HandlerAjaxOnly]
        public ActionResult GetGridJson(Pagination pagination,QueryTime queryTime, string queryName)
        {
            var data = new
            {
                rows = app.GetList(pagination, queryName, queryTime),
                total = pagination.records
            };
            return Content(data.ToJson());
        }

        /// <summary>
        /// 导入数据,主表ID前端生成主表GUID,附表main_id与之对应
        /// </summary>
        [HttpPost]
        [HandlerAjaxOnly]
        [ValidateAntiForgeryToken]
        public ActionResult Import(List<daoben_distributor_shipping_template> importList, daoben_distributor_shipping_template_approve importInfo)
        {
            string result = app.Import(importList, importInfo);
            if (result == "success")
                return Success("导入成功。");
            else
                return Error(result);
        }
        /// <summary>
        /// 预留审批功能
        /// </summary>
        //[HttpPost]
        //[HandlerAjaxOnly]
        //[ValidateAntiForgeryToken]
        //public ActionResult Approve(daoben_distributor_shipping_template_approve approveInfo)
        //{
        //    string result = app.Approve(approveInfo);
        //    if (result == "success")
        //        return Success("操作成功。");
        //    else
        //        return Error(result);
        //}
        /// <summary>
        /// 根据ID查看信息
        /// </summary>
        [HttpGet]
        [HandlerAjaxOnly]
        public ActionResult GetInfoById(string id)
        {
            string data = app.GetInfo(id, false);
            return Content(data);
        }
        /// <summary>
        /// 获取当前生效的运费数据，没有返回空值
        /// </summary>
        [HttpGet]
        [HandlerAjaxOnly]
        public ActionResult GetEffectInfo()
        {
            string data = app.GetInfo(null, true);
            return Content(data);
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
        public ActionResult GetInfoPage(Pagination pagination, string id)
        {
            var data = new
            {
                rows = app.GetInfoPage(pagination, id),
                total = pagination.records
            };
            return Content(data.ToJson());
        }
        /// <summary>
        /// 预留撤销功能
        /// </summary>
        //[HttpPost]
        //[HandlerAjaxOnly]
        //[ValidateAntiForgeryToken]
        //public ActionResult Delete(string id)
        //{
        //    if (string.IsNullOrEmpty(id))
        //        return Error("ID不能为空");

        //    string result = app.Delete(id);
        //    if (result == "success")
        //        return Success("操作成功。", id);
        //    else
        //        return Error(result);
        //}
    }
}