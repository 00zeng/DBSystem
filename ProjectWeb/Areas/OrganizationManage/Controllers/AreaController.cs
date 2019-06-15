using System.Web.Mvc;
using Base.Code;
using ProjectShare.Database;
using ProjectWeb.Areas.OrganizationManage.Application;
using System;

namespace ProjectWeb.Areas.OrganizationManage.Controllers
{
    public class AreaController : ControllerBase
    {
        private AreaApp app = new AreaApp();

        #region 页面显示
        [HttpGet]
        [HandlerAuthorize]
        public ActionResult AreaL1Index()
        {
            return View();
        }

        [HttpGet]
        [HandlerAuthorize]
        public ActionResult AreaL2Index()
        {
            return View();
        }
        #endregion
        [HttpGet]
        [HandlerAjaxOnly]
        public ActionResult GetAreaList(Pagination pagination, daoben_org_area queryInfo)
        {
            var data = new
            {
                rows = app.GetAreaList(pagination, queryInfo),
                total = pagination.records,
            };
            return Content(data.ToJson());
        }

        [HttpGet]
        [HandlerAjaxOnly]
        public ActionResult GetInfo(int id)
        {
            var data = app.GetInfo(id);
            if (data == null)
                return null;
            return Content(data.ToJson());
        }

        [HttpPost]
        [HandlerAjaxOnly]
        [ValidateAntiForgeryToken]
        public ActionResult Add(daoben_org_area areaInfo)
        {
            string result = app.Add(areaInfo);
            if (result == "success")
                return Success("操作成功。", areaInfo.id);
            else
                return Error(result);
        }

        [HttpPost]
        [HandlerAjaxOnly]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(daoben_org_area areaInfo)
        {
            string result = app.Edit(areaInfo);
            if (result == "success")
                return Success("操作成功。", areaInfo.id);
            else
                return Error(result);
        }

        [HttpPost]
        [HandlerAjaxOnly]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int[] idArray, DateTime? effect_date = null)
        {
            string result = app.Delete(idArray, effect_date);
            if (result == "success")
                return Success("操作成功。");
            else
                return Error(result);
        }
        /// <summary>
        /// 调区-经理片区
        /// </summary>
        [HttpPost]
        [HandlerAjaxOnly]
        [ValidateAntiForgeryToken]
        public ActionResult AjustL1(int[] idArray, int company_id, DateTime? effect_date = null)
        {
            string result = app.AjustL1(idArray, company_id, effect_date);
            if (result == "success")
                return Success("操作成功。");
            else
                return Error(result);
        }
        /// <summary>
        /// 调区-业务片区
        /// </summary>
        [HttpPost]
        [HandlerAjaxOnly]
        [ValidateAntiForgeryToken]
        public ActionResult AjustL2(int[] idArray, int company_id, int area_l1_id, DateTime? effect_date = null)
        {
            string result = app.AjustL2(idArray, company_id, area_l1_id, effect_date);
            if (result == "success")
                return Success("操作成功。");
            else
                return Error(result);
        }
        /// <summary>
        /// 根据机构获取区域选择列表
        /// </summary>
        /// <param name="companyId">0-表示登录人所在机构</param>
        /// <returns></returns>
        [HttpGet]
        [HandlerAjaxOnly]
        public ActionResult GetIdNameList(int company_id = 0)
        {
            var data = app.GetIdNameList(company_id);
            if (data == null)
                return null;
            return Content(data.ToJson());
        }

        [HttpGet]
        [HandlerAjaxOnly]
        public ActionResult GetSalesAreaIdNameList(int id = 0)
        {
            var data = app.GetAreaIdNameList(id,true);
            if (data == null)
                return null;
            return Content(data.ToJson());
        }

        [HttpGet]
        [HandlerAjaxOnly]
        public ActionResult GetManagerAreaIdNameList(int company_id = 0)
        {
            var data = app.GetAreaIdNameList(company_id,false);
            if (data == null)
                return null;
            return Content(data.ToJson());
        }
    }
}