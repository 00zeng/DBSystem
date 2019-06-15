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
    /// 主推奖励活动
    /// </summary>
    public class RecommendationController : ControllerBase
    {
        private RecommendationApp app = new RecommendationApp();

        [HttpGet]
        [HandlerAuthorize]
        public ActionResult ShowActivity()
        {
            return View();
        }

        [HttpGet]
        [HandlerAjaxOnly]
        public ActionResult GetList(Pagination pagination, daoben_activity_recommendation queryInfo, QueryTime queryTime)
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
        public ActionResult GetListApprove(Pagination pagination, daoben_activity_recommendation queryInfo, QueryTime queryTime)
        {    
            var data = new
            {
                rows = app.GetListApprove(pagination, queryInfo),
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
        public ActionResult Add(daoben_activity_recommendation addInfo, List<daoben_activity_recommendation_product> productList,
                   List<daoben_activity_recommendation_emp> empList, List<daoben_activity_recommendation_reward> rewardList)
        {
            string result = app.Add(addInfo, productList, empList, rewardList);
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

        
        #region 审批
        [HttpPost]
        [HandlerAjaxOnly]
        [ValidateAntiForgeryToken]
        public ActionResult Approve(daoben_activity_recommendation_approve approve_info)
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