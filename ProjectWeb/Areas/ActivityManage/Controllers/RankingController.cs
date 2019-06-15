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
    public class RankingController : ControllerBase
    {
        private RankingApp app = new RankingApp();
        [HttpGet]
        [HandlerAuthorize]
        public ActionResult ShowActivity()
        {
            return View();
        }
        [HttpGet]
        [HandlerAjaxOnly]
        public ActionResult GetList(Pagination pagination, daoben_activity_ranking queryInfo, QueryTime queryTime)
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
        public ActionResult GetListApprove(Pagination pagination, daoben_activity_ranking queryInfo, QueryTime queryTime)
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
            return Content(data.ToJson());
        }
        /// <summary>
        /// 销售详情列表
        /// </summary>
        /// <param name="pagination"></param>
        /// <returns></returns>
        [HttpGet]
        [HandlerAjaxOnly]
        public ActionResult GetSaleList(Pagination pagination, daoben_product_sn queryInfo, daoben_activity_ranking queryMainInfo)
        {
            var data = new
            {
                rows = app.GetSaleList(pagination, queryInfo, queryMainInfo),
                total = pagination.records,
            };
            return Content(data.ToJson());
        }
        [HttpPost]
        [HandlerAjaxOnly]
        [ValidateAntiForgeryToken]
        public ActionResult Add(daoben_activity_ranking addInfo, List<daoben_activity_ranking_reward> rewardList,
                    List<daoben_activity_ranking_emp> empList)
        {
            string result = app.Add(addInfo, rewardList, empList);
            if (result == "success")
                return Success("操作成功。");
            else
                return Error(result);
        }
        /// <summary>
        /// 修改活动结束时间
        /// </summary>
        /// <param name="id"></param>
        /// <param name="alterDate"></param>
        /// <returns></returns>
        [HttpPost]
        [HandlerAjaxOnly]
        [ValidateAntiForgeryToken]
        public ActionResult Alter(string id, DateTime alterDate)
        {
            string result = app.Alter(id, alterDate);
            if (result == "success")
                return Success("操作成功。", id);
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
        public ActionResult Approve(daoben_activity_ranking_approve approve_info)
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