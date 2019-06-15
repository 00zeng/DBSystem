using System.Web.Mvc;
using Base.Code;
using ProjectWeb.Areas.SaleManage.Application;
using ProjectShare.Database;
using System;
using System.Collections.Generic;
using ProjectShare.Models;

namespace ProjectWeb.Areas.SaleManage.Controllers
{
    public class ExclusiveController : ControllerBase
    {
        private ExclusiveApp app = new ExclusiveApp();
        #region 页面显示
        [HttpGet]
        [HandlerAuthorize]
        public ActionResult History()
        {
            return View();
        }
        #endregion

        /// <summary>
        /// 查看全部
        /// </summary>
        [HttpGet]
        [HandlerAjaxOnly]
        public ActionResult GetAllList(Pagination pagination, daoben_sale_exclusive_detail queryInfo, QueryTime queryTime)
        {  
            var data = new
            {
                rows = app.GetAllList(pagination, queryInfo, queryTime),
                total = pagination.records,
            };
            return Content(data.ToJson());
        }
        /// <summary>
        /// 历史记录
        /// </summary>
        [HttpGet]
        [HandlerAjaxOnly]
        public ActionResult GetListHistory(Pagination pagination, QueryTime queryTime, string queryName = null)
        {   
            var data = new
            {
                rows = app.GetListHistory(pagination, queryName, queryTime),
                total = pagination.records,
            };
            return Content(data.ToJson());
        }
        /// <summary>
        /// 审批列表
        /// </summary>
        [HttpGet]
        [HandlerAjaxOnly]
        public ActionResult GetListApprove(Pagination pagination, string queryName = null)
        {
            var data = new
            {
                rows = app.GetListApprove(pagination, queryName),
                total = pagination.records,
            };
            return Content(data.ToJson());
        }

        /// <summary>
        /// 包销导入
        /// </summary>
        [HttpPost]
        [HandlerAjaxOnly]
        [ValidateAntiForgeryToken]
        public ActionResult Import(string importListStr, daoben_sale_exclusive importInfo)
        {
            try
            {
                List<daoben_sale_exclusive_detail> importList = importListStr.ToList<daoben_sale_exclusive_detail>();
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
            catch (Exception ex)
            {
                return Error(ex.Message);
            }
        }
        /// <summary>
        /// 审批
        /// </summary>
        [HttpPost]
        [HandlerAjaxOnly]
        [ValidateAntiForgeryToken]
        public ActionResult Approve(daoben_sale_exclusive_approve approveInfo)
        {
            string result = app.Approve(approveInfo);
            if (result == "success")
                return Success("操作成功。");
            else
                return Error(result);
        }

        /// <summary>
        /// 查看详情 主列表
        /// </summary>
        [HttpGet]
        [HandlerAjaxOnly]
        public ActionResult GetInfoMain(string id)
        {
            string data = app.GetInfoMain(id);
            return Content(data);
        }
        /// <summary>
        /// 查看详情 子列表
        /// </summary>
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
        /// 撤回
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
        

    }
}