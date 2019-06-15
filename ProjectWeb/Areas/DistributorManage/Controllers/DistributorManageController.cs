using System.Web.Mvc;
using Base.Code;
using ProjectWeb.Areas.DistributorManage.Application;
using ProjectShare.Database;
using System.Collections.Generic;
using System.IO;
using System;
using ProjectShare.Models;

namespace ProjectWeb.Areas.DistributorManage.Controllers
{
    public class DistributorManageController : ControllerBase
    {
        private DistributorManageApp app = new DistributorManageApp();
        [HttpGet]
        [HandlerAuthorize]
        public ActionResult Import()
        {
            return View();
        }
        public ActionResult EditAccountInfo()
        {
            return View();
        }
        public ActionResult EditDistributorInfo()
        {
            return View();
        }
        public ActionResult EditPictureInfo()
        {
            return View();
        }
        //导出excel  
        public FileResult Export(Pagination pagination, daoben_distributor_info queryInfo)
        {
            string fileName = "经销商管理_" + DateTime.Now.ToString("yyyyMMddhhmmss") + ".xls";
            Stream fileStream = app.ExportExcel(pagination, queryInfo);
            return File(fileStream, "application/vnd.ms-excel", fileName);
        }

        [HttpGet]
        [HandlerAjaxOnly]
        public ActionResult GetGridJson(Pagination pagination, daoben_distributor_info queryInfo)
        {
            var data = new
            {
                rows = app.GetList(pagination, queryInfo),
                total = pagination.records,
            };
            return Content(data.ToJson());
        }

        [HttpGet]
        [HandlerAjaxOnly]
        public ActionResult GetGridJsonClosed(Pagination pagination, daoben_distributor_info queryInfo)
        {
            var data = new
            {
                rows = app.GetListClosed(pagination, queryInfo),
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
        public ActionResult Add(daoben_distributor_info distributorInfo, daoben_ms_account accountInfo,
                    List<daoben_distributor_info_file> image_list = null)
        {
            string result = app.Add(distributorInfo, accountInfo, image_list);
            if (result == "success")
                return Success("操作成功。", distributorInfo.id);
            else
                return Error(result);
        }

        [HttpPost]
        [HandlerAjaxOnly]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(daoben_distributor_info distributorInfo, List<daoben_distributor_info_file> add_image_list = null, List<int> del_image_list = null)
        {
            string result = app.Edit(distributorInfo, add_image_list, del_image_list);
            if (result == "success")
                return Success("操作成功。", distributorInfo.id);
            else
                return Error(result);
        }
        [HttpPost]
        [HandlerAjaxOnly]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(string[] idArray, DateTime? effect_date = null)
        {
            string result = app.Delete(idArray, effect_date);
            if (result == "success")
                return Success("操作成功。");
            else
                return Error(result);
        }
        /// <summary>
        /// 经销商调区
        /// </summary>
        [HttpPost]
        [HandlerAjaxOnly]
        [ValidateAntiForgeryToken]
        public ActionResult Ajust(string[] idArray, int company_id, int area_l1_id, int area_l2_id, DateTime? effect_date = null)
        {
            string result = app.Ajust(idArray, company_id, area_l1_id, area_l2_id, effect_date);
            if (result == "success")
                return Success("操作成功。");
            else
                return Error(result);
        }
        #region 启用/注销账户
        /// <summary>
        /// 注销/启用
        /// </summary>        
        /// <returns></returns>
        [HttpPost]
        [HandlerAjaxOnly]
        [ValidateAntiForgeryToken]
        public ActionResult Active(string id)
        {
            string result = app.AccountActive(id);
            if (result == "success")
                return Success("操作成功。", id);
            else
                return Error(result);
        }
        #endregion      

        /// <summary>
        /// 根据当前登录人的事业部ID获取全部的经销商
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [HandlerAjaxOnly]
        public ActionResult GetDistributorTree(int companyId = 0, int companyParentId = 0)
        {
            var data = app.GetDistributorTree(companyId, companyParentId);
            return Content(data);
        }

        /// <summary>
        /// 根据机构获取经销商选择列表
        /// </summary>
        /// <param name="companyId"></param>
        /// <returns></returns>
        [HttpGet]
        [HandlerAjaxOnly]
        public ActionResult GetIdNameList(int company_id = 0, int area_l1_id = 0, int area_l2_id = 0)
        {
            var data = app.GetIdNameList(company_id, area_l1_id, area_l2_id);
            return Content(data);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="distributorListStr">经销商基本信息</param>
        /// <returns></returns>
        [HttpPost]
        [HandlerAjaxOnly]
        [ValidateAntiForgeryToken]
        public ActionResult Import(string distributorListStr)
        {
            List<daoben_distributor_info> distributorList = distributorListStr.ToList<daoben_distributor_info>();
            string result = app.Import(distributorList);
            if (result == "success")
                return Success("导入成功。");
            else
                return Error(result);
        }

        /// <summary>
        /// 获取区域列表
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [HandlerAjaxOnly]
        public ActionResult GetAreaTree()
        {
            var data = app.GetAreaTree();
            return Content(data);
        }

        /// <summary>
        /// 获取全部经销商ID
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [HandlerAjaxOnly]
        public ActionResult GetAllDistriId()
        {
            var data = app.GetAllDistriId();
            return Content(data);
        }

        /// <summary>
        /// 经销商前3个月平均销量
        /// </summary>
        /// <param name="idStr">经销商ID串，用|分隔</param>
        /// <returns></returns>
        [HttpGet]
        [HandlerAjaxOnly]
        public ActionResult GetAvgBefore(string idStr)
        {
            var data = app.GetAvgBefore(idStr);
            return Content(data.ToString());
        }

    }
}