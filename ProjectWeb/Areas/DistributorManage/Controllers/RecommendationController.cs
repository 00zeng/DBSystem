using System.Web.Mvc;
using Base.Code;
using ProjectWeb.Areas.DistributorManage.Application;
using ProjectShare.Database;
using System.Collections.Generic;
using ProjectShare.Models;
using ProjectWeb.Areas.ProductManage.Application;

namespace ProjectWeb.Areas.DistributorManage.Controllers
{
    public class RecommendationController : ControllerBase
    {
        [HttpGet]
        [HandlerAuthorize]
        public ActionResult ShowActivity()
        {
            return View();
        }
        private RecommendationApp app = new RecommendationApp();
        private PriceInfoApp price = new PriceInfoApp();

        /// <summary>
        /// 全部列表
        /// </summary>
        [HttpGet]
        [HandlerAjaxOnly]
        public ActionResult GetGridJson(Pagination pagination, daoben_distributor_recommendation queryInfo, QueryTime queryTime, int? status = null )
        {  
            var data = new
            {
                rows = app.AllList(pagination, queryInfo, status, queryTime),
                total = pagination.records,
            };
            return Content(data.ToJson());
        }
        /// <summary>
        /// 审批列表
        /// </summary>
        [HttpGet]
        [HandlerAjaxOnly]
        public ActionResult GetGridJson2(Pagination pagination, daoben_distributor_recommendation queryInfo)
        {
            var data = new
            {
                rows = app.ApproveList(pagination, queryInfo),
                total = pagination.records,
            };
            return Content(data.ToJson());
        }
        /// <summary>
        /// 经销商列表
        /// </summary>
        [HttpGet]
        [HandlerAjaxOnly]
        public ActionResult GetGridJson3(Pagination pagination, string queryName)
        {
            var data = new
            {
                rows = app.DistributorList(pagination, queryName),
                total = pagination.records,
            };
            return Content(data.ToJson());
        }

        /// <summary>
        /// 根据申请表获取申请信息
        /// </summary>
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
        public ActionResult Add(daoben_distributor_recommendation addInfo, List<daoben_distributor_recommendation_distributor> distributorList,
           List<daoben_distributor_recommendation_product> productList, List<daoben_distributor_recommendation_rebate> rebateList)
        {
            string result = app.Add(addInfo, distributorList, productList, rebateList);
            if (result == "success")
                return Success("操作成功。", addInfo.id);
            else
                return Error(result);
        }

        [HttpPost]
        [HandlerAjaxOnly]
        [ValidateAntiForgeryToken]
        public ActionResult Approve(daoben_distributor_recommendation_approve approveInfo)
        {
            string result = app.Approve(approveInfo);
            if (result == "success")
                return Success("操作成功。");
            else
                return Error(result);
        }

    }
}