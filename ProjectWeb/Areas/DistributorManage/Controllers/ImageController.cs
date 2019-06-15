using System.Web.Mvc;
using Base.Code;
using ProjectWeb.Areas.DistributorManage.Application;
using ProjectShare.Database;
using System;
using System.Collections.Generic;
using ProjectShare.Models;

namespace ProjectWeb.Areas.DistributorManage.Controllers
{
    public class ImageController : ControllerBase
    {
        private ImageApp app = new ImageApp();

        #region 页面显示
        [HttpGet]
        [HandlerAuthorize]
        public ActionResult ShowActivity()
        {
            return View();
        }
        #endregion

        [HttpGet]
        [HandlerAjaxOnly]
        public ActionResult GetGridJson(Pagination pagination, daoben_distributor_image queryInfo, QueryTime queryTime, int? status = null)
        {     
            var data = new
            {
                rows = app.AllList(pagination, queryInfo, status, queryTime),
                total = pagination.records,
            };
            return Content(data.ToJson());
        }
        [HttpGet]
        [HandlerAjaxOnly]
        public ActionResult GetGridJson2(Pagination pagination, daoben_distributor_image queryInfo)
        {
            var data = new
            {
                rows = app.ApproveList(pagination, queryInfo),
                total = pagination.records,
            };
            return Content(data.ToJson());
        }
        /// <summary>
        /// 销售详情列表
        /// </summary>
        /// <param name="pagination"></param>
        /// <returns></returns>
        [HttpGet]
        [HandlerAjaxOnly]
        public ActionResult GetSaleList(Pagination pagination, daoben_product_sn queryInfo, daoben_distributor_image queryMainInfo ,QueryTime queryTime)
        {
            var data = new
            {
                rows = app.GetSaleList(pagination, queryInfo, queryMainInfo, queryTime),
                total = pagination.records,
            };
            return Content(data.ToJson());
        }

        /// <summary>
        /// 根据申请表获取申请信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [HandlerAjaxOnly]
        public ActionResult GetInfo(string id)
        {
            var data = app.GetInfo(id);
            return Content(data);
        }

        /// <summary>
        /// 形象返利
        /// </summary>
        /// <param name="addInfo">形象返利主表信息</param>
        /// <param name="image_list">图片列表</param>
        /// <returns></returns>
        [HttpPost]
        [HandlerAjaxOnly]
        [ValidateAntiForgeryToken]
        public ActionResult Add(daoben_distributor_image addInfo, List<daoben_distributor_image_file> image_list = null)
        {
            string result = app.Add(addInfo, image_list);
            if (result == "success")
                return Success("操作成功。", addInfo.id);
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
        public ActionResult Approve(daoben_distributor_image_approve approveInfo)
        {
            string result = app.Approve(approveInfo);
            if (result == "success")
                return Success("操作成功。");
            else
                return Error(result);
        }


    }
}