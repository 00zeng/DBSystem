using System.Web.Mvc;
using Base.Code;
using ProjectWeb.Areas.HumanResource.Application;
using ProjectShare.Database;
using System;
using System.Web;
using ProjectShare.Models;
using System.IO;
using System.Collections.Generic;

namespace ProjectWeb.Areas.HumanResource.Controllers
{
    public class AttendanceController : ControllerBase
    {
        private AttendanceApp app = new AttendanceApp();

        #region 页面显示
        [HttpGet]
        [HandlerAuthorize]
        public ActionResult Import()
        {
            return View();
        }

        #endregion

        [HttpGet]
        [HandlerAjaxOnly]
        public ActionResult GetGridJson(Pagination pagination, string queryName,
                   QueryTime queryTime)
        {                 
            var data = new
            {
                rows = app.GetList(pagination, queryName, queryTime),
                total = pagination.records,
            };
            return Content(data.ToJson());
        }

        [HttpGet]
        [HandlerAjaxOnly]
        public ActionResult GetGridJson2(Pagination pagination, QueryTime queryTime)
        {
               
            var data = new
            {
                rows = app.GetListHistory(pagination, queryTime),
                total = pagination.records,
            };
            return Content(data.ToJson());
        }
        ///// <summary>
        ///// 保留我的审批
        ///// </summary>
        //[HttpGet]
        //[HandlerAjaxOnly]
        //public ActionResult GetGridJson3(Pagination pagination, string queryName = null)
        //{
        //    var data = new
        //    {
        //        rows = app.GetListApprove(pagination, queryName),
        //        total = pagination.records,
        //    };
        //    return Content(data.ToJson());
        //}
        /// <summary>
        /// 导入考勤表数据
        /// </summary>
        [HttpPost]
        [HandlerAjaxOnly]
        [ValidateAntiForgeryToken]
        public ActionResult Import(string importListStr, daoben_hr_attendance_approve importInfo)
        {
            List<daoben_hr_attendance> importList = importListStr.ToList<daoben_hr_attendance>();
            string result = app.Import(importList, importInfo);
            if (result == "success")
                return Success("导入成功。", importInfo.import_file);
            else
                return Error(result);
        }
        /// <summary>
        /// 预留审批功能
        /// </summary>
        //[HttpPost]
        //[HandlerAjaxOnly]
        //[ValidateAntiForgeryToken]
        //public ActionResult Approve(daoben_hr_attendance_approve approveInfo)
        //{
        //    string result = app.Approve(approveInfo);
        //    if (result == "success")
        //        return Success("操作成功。");
        //    else
        //        return Error(result);
        //}

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

        ///// <summary>
        ///// 预留撤销功能
        ///// </summary>
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