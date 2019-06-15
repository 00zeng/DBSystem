using System.Web.Mvc;
using Base.Code;
using ProjectWeb.Areas.OrganizationManage.Application;
using ProjectShare.Database;
using System.Collections.Generic;
using System;

namespace ProjectWeb.Areas.OrganizationManage.Controllers
{
    public class CompanyController : ControllerBase
    {
        private CompanyApp app = new CompanyApp();

        [HttpGet]
        [HandlerAjaxOnly]
        public ActionResult GetGridJson(Pagination pagination, daoben_org_company companyInfo)
        {
            var data = new
            {
                rows = app.GetList(pagination, companyInfo),
                total = pagination.records,
            };
            return Content(data.ToJson());
        }

        [HttpGet]
        [HandlerAjaxOnly]
        public ActionResult GetInfo(int id)
        {
            daoben_org_company data = app.GetInfo(id);
            if (data == null)
                return null;
            return Content(data.ToJson());
        }

        [HttpPost]
        [HandlerAjaxOnly]
        [ValidateAntiForgeryToken]
        public ActionResult Add(daoben_org_company companyInfo, List<daoben_org_grade> grade_list)
        {
            string result = app.Add(companyInfo, grade_list);
            if (result == "success")
                return Success("操作成功。", companyInfo.id);
            else
                return Error(result);
        }

        [HttpPost]
        [HandlerAjaxOnly]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(daoben_org_company companyInfo)
        {
            string result = app.Edit(companyInfo);
            if (result == "success")
                return Success("操作成功。", companyInfo.id);
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
        /// 获取机构类型列表
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [HandlerAjaxOnly]
        public ActionResult GetCategoryJson()
        {
            var data = app.GetCategoryList();
            if (data == null)
                return null;
            return Content(data.ToJson());
        }

        /// <summary>
        /// 获取机构选择列表
        /// </summary>
        /// <param name="category"></param>
        /// <param name="param_type">
        ///     0: 获取类型为category的机构列表
        ///     1：获取类型为category的机构的上级机构列表
        /// </param>
        /// <returns></returns>
        [HttpGet]
        [HandlerAjaxOnly]
        public ActionResult GetIdNameList(string category = "分公司", int param_type = 0)
        {
            var data = app.GetIdNameList(category, param_type);
            if (data == null)
                return null;
            return Content(data.ToJson());
        }

        /// <summary>
        /// 获取机构选择列表
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [HandlerAjaxOnly]
        public ActionResult GetIdNameCategoryList(string category = null)
        {
            string data = app.GetIdNameCategoryList(category);
            return Content(data);
        }

        [HttpGet]
        [HandlerAjaxOnly]
        public ActionResult GetAddr(int id)
        {
            var data = app.GetAddr(id);
            //if (data == null)
            //    return null;
            return Content(data.ToJson());
        }

        [HttpGet]
        [HandlerAjaxOnly]
        public ActionResult GetOrgTree(int type = 1)
        {
            string data;
            if (type == 1)
                data = app.GetOrgDistriTree();
            else
                data = app.GetOrgDeptTree();
            return Content(data);
        }

#if false
        /// <summary>
        /// 获取机构选择列表
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [HandlerAjaxOnly]
        public ActionResult GetSelectJson()
        {
            var data = app.GetSelectJson();
            return Content(data.ToJson());
        }

        /// <summary>
        /// 获取机构选择列表
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [HandlerAjaxOnly]
        public ActionResult GetSelectJson2(int parent_id)
        {
            var data = app.GetSelectJson2(parent_id);
            return Content(data.ToJson());
        }

        /// <summary>
        /// 经销商管理，下拉选择所属分公司
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [HandlerAjaxOnly]
        public ActionResult GetSelectJson3()
        {
            var data = app.GetSelectJson3();
            return Content(data.ToJson());
        }

        /// <summary>
        ///事业部在前，分公司在后的机构下拉栏排序
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [HandlerAjaxOnly]
        public ActionResult GetSelectJson4()
        {
            var data = app.GetSelectJson4();
            return Content(data.ToJson());
        }

#endif
    }
}