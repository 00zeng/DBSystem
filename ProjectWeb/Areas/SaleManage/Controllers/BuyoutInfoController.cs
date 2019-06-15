using System.Web.Mvc;
using Base.Code;
using ProjectWeb.Areas.SaleManage.Application;
using ProjectShare.Database;
using System.Collections.Generic;
using ProjectWeb.Areas.ProductManage.Application;

namespace ProjectWeb.Areas.SaleManage.Controllers
{
    public class BuyoutInfoController : ControllerBase
    {
        private BuyoutInfoApp app = new BuyoutInfoApp();
        private PriceInfoApp priceApp = new PriceInfoApp();

        #region 页面显示
        [HttpGet]
        [HandlerAuthorize]
        public ActionResult Import()
        {
            return View();
        }
        [HttpGet]
        [HandlerAuthorize]
        public ActionResult RequestIndex()
        {
            return View();
        }

        [HttpGet]
        [HandlerAuthorize]
        public ActionResult BuyoutInfoAdd()
        {
            return View();
        }
        [HttpGet]
        [HandlerAuthorize]
        public ActionResult MyRequest()
        {
            return View();
        }

        [HttpGet]
        [HandlerAuthorize]
        public ActionResult Confirm()
        {
            return View();
        }

        [HttpGet]
        [HandlerAuthorize]
        public ActionResult BuyoutInfoCheck()
        {
            return View();
        }
        [HttpGet]
        [HandlerAuthorize]
        public ActionResult ConfirmIndex()
        {
            return View();
        }

        [HttpGet]
        [HandlerAuthorize]
        public ActionResult History()
        {
            return View();
        }

        #endregion

        /// <summary>
        /// 查看所有
        /// </summary>
        [HttpGet]
        [HandlerAjaxOnly]
        public ActionResult GetListAll(Pagination pagination, daoben_sale_buyout queryInfo, daoben_sale_buyout_request requestInfo)
        {
            var data = new
            {
                rows = app.GetListAll(pagination, queryInfo, requestInfo),
                total = pagination.records,
            };
            return Content(data.ToJson());
        }

        /// <summary>
        /// 查看申请（买断申请）
        /// </summary>
        [HttpGet]
        [HandlerAjaxOnly]
        public ActionResult GetListRequest(Pagination pagination, string distributorName = null,
                    string salesName = null, bool isMyRequest = false)
        {
            var data = new
            {
                rows = app.GetListRequest(pagination, distributorName, salesName, isMyRequest),
                total = pagination.records,
            };
            return Content(data.ToJson());
        }

        /// <summary>
        /// 买断审批
        /// </summary>
        [HttpGet]
        [HandlerAjaxOnly]
        public ActionResult GetListApprove(Pagination pagination, string distributorName, string salesName)
        {
            var data = new
            {
                rows = app.GetListApprove(pagination, distributorName, salesName),
                total = pagination.records,
            };
            return Content(data.ToJson());
        }

        /// <summary>
        /// 买断确认
        /// </summary>
        [HttpGet]
        [HandlerAjaxOnly]
        public ActionResult GetListConfirm(Pagination pagination, string queryName)
        {
            var data = new
            {
                rows = app.GetListConfirm(pagination, queryName),
                total = pagination.records,
            };
            return Content(data.ToJson());
        }

        /// <summary>
        /// 新增门店买断申请
        /// </summary>
        [HttpPost]
        [HandlerAjaxOnly]
        [ValidateAntiForgeryToken]
        public ActionResult StoreAdd(daoben_sale_buyout_request addInfo, List<daoben_sale_buyout> buyoutList)
        {
            string result = app.StoreAdd(addInfo, buyoutList);
            if (result == "success")
                return Success("操作成功。");
            else
                return Error(result);
        }
        /// <summary>
        /// 门店买断审批
        /// </summary>
        [HttpPost]
        [HandlerAjaxOnly]
        [ValidateAntiForgeryToken]
        public ActionResult StoreApprove(daoben_sale_buyout_request_approve approveInfo, daoben_sale_buyout_request requestIndo, List<daoben_sale_buyout> buyoutList)
        {
            string result = app.StoreApprove(approveInfo, requestIndo, buyoutList);
            if (result == "success")
                return Success("操作成功。");
            else
                return Error(result);
        }
        /// <summary>
        /// 仓库买断申请
        /// </summary>
        [HttpPost]
        [HandlerAjaxOnly]
        [ValidateAntiForgeryToken]
        public ActionResult StorageAdd(daoben_sale_buyout_request addInfo, List<daoben_sale_buyout_request_sub> buyoutList)
        {
            string result = app.StorageAdd(addInfo, buyoutList);
            if (result == "success")
                return Success("操作成功。");
            else
                return Error(result);
        }

        /// <summary>
        /// 仓库买断审批
        /// </summary>
        [HttpPost]
        [HandlerAjaxOnly]
        [ValidateAntiForgeryToken]
        public ActionResult StorageApprove(daoben_sale_buyout_request_approve approveInfo)
        {
            string result = app.StorageApprove(approveInfo);
            if (result == "success")
                return Success("操作成功。");
            else
                return Error(result);
        }


        /// <summary>
        /// 获取买断信息，id为申请表ID
        /// </summary>
        [HttpGet]
        [HandlerAjaxOnly]
        public ActionResult GetBuyoutInfo(string id)
        {
            string data = app.GetBuyoutInfo(id);
            return Content(data);
        }

        /// <summary>
        /// 获取业务员和导购员列表
        /// </summary>
        [HttpGet]
        [HandlerAjaxOnly]
        public ActionResult GetSalesInfo(string distributor_id = null)
        {
            string data = app.GetSalesInfo(distributor_id);
            return Content(data);
        }

        /// <summary>
        /// 撤销申请
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
        /// 根据串码获取型号颜色买断价
        /// </summary>
        [HttpGet]
        [HandlerAjaxOnly]
        public ActionResult GetModelByPhoneSn(string phone_sn)
        {
            int type = 1;
            string data = priceApp.GetColorPriceInfo(phone_sn, type);
            return Content(data);
        }
        /// <summary>
        /// 根据型号获取串码颜色买断价
        /// </summary>
        [HttpGet]
        [HandlerAjaxOnly]
        public ActionResult GetModelByModel()
        {
            int type = 2;
            string data = priceApp.GetColorPriceInfo(null, type);
            return Content(data);
        }


    }
}