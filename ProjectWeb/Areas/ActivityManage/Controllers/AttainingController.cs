using System.Web.Mvc;
using Base.Code;
using ProjectWeb.Areas.ActivityManage.Application;
using ProjectShare.Database;
using System.Collections.Generic;
using System;
using ProjectShare.Models;
using System.IO;

namespace ProjectWeb.Areas.ActivityManage.Controllers
{


    /// <summary>
    /// 达量奖励活动
    /// </summary>
    public class AttainingController : ControllerBase
    {
        private AttainingApp app = new AttainingApp();

        [HttpGet]
        [HandlerAuthorize]
        public ActionResult ShowActivity()
        {
            return View();
        }

        [HttpGet]
        [HandlerAjaxOnly]
        public ActionResult GetList(Pagination pagination, daoben_activity_attaining queryInfo, QueryTime queryTime)
        {
            var data = new
            {
                rows = app.GetList(pagination, queryInfo, queryTime),
                total = pagination.records
            };
            return Content(data.ToJson());
        }

        [HttpGet]
        [HandlerAjaxOnly]
        public ActionResult GetListApprove(Pagination pagination, daoben_activity_attaining queryInfo, QueryTime queryTime)
        {
            var data = new
            {
                rows = app.GetListApprove(pagination, queryInfo),
                total = pagination.records
            };
            return Content(data.ToJson());
        }
        [HttpGet]
        [HandlerAjaxOnly]
        public ActionResult GetSaleList(Pagination pagination, daoben_product_sn queryInfo, daoben_activity_attaining queryMainInfo)
        {
            var data = new
            {
                rows = app.GetSaleList(pagination, queryInfo, queryMainInfo),
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

        [HttpPost]
        [HandlerAjaxOnly]
        [ValidateAntiForgeryToken]
        public ActionResult Add(daoben_activity_attaining addInfo, List<daoben_activity_attaining_product> productList,
                    List<daoben_activity_attaining_emp> empList, List<daoben_activity_attaining_reward> rewardList, List<daoben_activity_attaining_product_reward> proRewardList)
        {
            string result = app.Add(addInfo, productList, empList, rewardList, proRewardList);
            if (result == "success")
                return Success("操作成功。");
            else
                return Error(result);
        }

        [HttpPost]
        [HandlerAjaxOnly]
        [ValidateAntiForgeryToken]
        public ActionResult Recall(string id)
        {
            string result = app.Recall(id);
            if (result == "success")
                return Success("操作成功。", id);
            else
                return Error(result);
        }

        [HttpPost]
        [HandlerAjaxOnly]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(daoben_activity_attaining mainInfo, List<daoben_activity_attaining_product_reward> proRewardList)
        {
            string result = app.Edit(mainInfo, proRewardList);
            if (result == "success")
                return Success("操作成功。", mainInfo.id);
            else
                return Error(result);
        }
       

        #region 审批
        [HttpPost]
        [HandlerAjaxOnly]
        [ValidateAntiForgeryToken]
        public ActionResult Approve(daoben_activity_attaining_approve approve_info)
        {
            string result = app.Approve(approve_info);
            if (result == "success")
                return Success("操作成功。", approve_info.id);
            else
                return Error(result);
        }
        #endregion
    }
}