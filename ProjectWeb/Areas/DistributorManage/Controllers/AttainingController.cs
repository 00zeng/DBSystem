using System.Web.Mvc;
using Base.Code;
using ProjectWeb.Areas.DistributorManage.Application;
using ProjectShare.Database;
using System;
using System.Collections.Generic;
using ProjectShare.Models;
using ProjectWeb.Areas.ProductManage.Application;

namespace ProjectWeb.Areas.DistributorManage.Controllers
{

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
        [HandlerAuthorize]
        public ActionResult EditFull()
        {
            return View();
        }
        [HttpGet]
        [HandlerAuthorize]
        public ActionResult UpdateShowAct()
        {
            return View();
        }
        private PriceInfoApp price = new PriceInfoApp();

        /// <summary>
        /// 全部列表
        /// </summary>
        [HttpGet]
        [HandlerAjaxOnly]
        public ActionResult GetGridJson(Pagination pagination, daoben_distributor_attaining queryInfo, QueryTime queryTime, int? status = null)
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
        public ActionResult GetGridJson2(Pagination pagination, daoben_distributor_attaining queryInfo)
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
        public ActionResult GetInfo(string id, bool forEdit = false)
        {
            var data = app.GetInfo(id, forEdit);
            return Content(data);
        }

        /// <summary>
        /// 销售详情列表
        /// </summary>
        /// <param name="pagination"></param>
        /// <returns></returns>
        [HttpGet]
        [HandlerAjaxOnly]
        public ActionResult GetSaleList(Pagination pagination, daoben_product_sn queryInfo, daoben_distributor_attaining queryMainInfo)
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
        public ActionResult Add(daoben_distributor_attaining addInfo, List<daoben_distributor_attaining_distributor> distributorList,
                List<daoben_distributor_attaining_product_sec> productSecList, List<daoben_distributor_attaining_product> productList,
                List<daoben_distributor_attaining_time_sec> timeSecList, List<daoben_distributor_attaining_rebate> rebateList,
                List<daoben_distributor_attaining_rebate_product> rebateProList)
        {
            string result = app.Add(addInfo, distributorList, productSecList, productList, timeSecList, rebateList, rebateProList);
            if (result == "success")
                return Success("操作成功。", addInfo.id);
            else
                return Error(result);
        }

        [HttpPost]
        [HandlerAjaxOnly]
        [ValidateAntiForgeryToken]
        public ActionResult Approve(daoben_distributor_attaining_approve approveInfo)
        {
            string result = app.Approve(approveInfo);
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
        public ActionResult EditFull(List<daoben_distributor_attaining_product_sec> productSecList, List<daoben_distributor_attaining_product> productList,
                List<daoben_distributor_attaining_time_sec> timeSecList, List<daoben_distributor_attaining_rebate> rebateList,
                List<daoben_distributor_attaining_rebate_product> rebateProList, string orig_id)
        {
            string result = app.EditFull(productSecList, productList,timeSecList, rebateList, rebateProList, orig_id);
            if (result == "success")
                return Success("操作成功。", orig_id);
            else
                return Error(result);
        }

    }
}