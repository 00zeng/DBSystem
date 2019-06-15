using System.Web.Mvc;
using Base.Code;
using ProjectWeb.Areas.SubordinateManage.Application;
using ProjectShare.Database;
using System;
using System.Collections.Generic;
using ProjectShare.Models;

namespace ProjectWeb.Areas.SubordinateManage.Controllers
{
    public class MySubordinateController : ControllerBase
    {
        private MySubordinateApp app = new MySubordinateApp();


        #region 页面显示
      
        [HttpGet]
        [HandlerAuthorize]
        public ActionResult GuideAdd() 
        {
            return View();
        }
        [HttpGet]
        [HandlerAuthorize]
        public ActionResult SalesAdd() 
        {
            return View();
        }
        [HttpGet]
        [HandlerAuthorize]
        public ActionResult SalesManageAdd()  
        {
            return View();
        }
        [HttpGet]
        [HandlerAuthorize]
        public ActionResult TrainerAdd()
        {
            return View();
        }
        [HttpGet]
        [HandlerAuthorize]
        public ActionResult TrainerManageAdd() 
        {
            return View();
        }
        #endregion




        [HttpGet]
        [HandlerAjaxOnly]
        public ActionResult GetGridJson(Pagination pagination,QueryTime queryTime, daoben_hr_emp_job jobInfo,
            string name = null)
        {
            var data = new
            {
                rows = app.GetList(pagination, jobInfo, name, queryTime),
                total = pagination.records,
            };
            return Content(data.ToJson());
        }

        [HttpGet]
        [HandlerAjaxOnly]
        public ActionResult GetAllocationList(Pagination pagination, string name = null)
        {
            var data = new
            {
                rows = app.GetAllocationList(pagination, name),
                total = pagination.records,
            };
            return Content(data.ToJson());
        }


        [HttpGet]
        [HandlerAjaxOnly]
        public ActionResult GetDistributorList(int area_id)
        {
            string data = app.GetDistributorList(area_id);
            return Content(data);
        }

        [HttpGet]
        [HandlerAjaxOnly]
        public ActionResult GetInfo(string id)
        {
            string data = app.GetInfo(id);
            return Content(data);
        }

        /// <summary>
        /// 根据区域获取员工选择列表
        /// </summary>
        /// <param name="area_id"></param>
        /// <param name="type">0: 业务经理；1：业务员；2：导购员</param>
        [HttpGet]
        [HandlerAjaxOnly]
        public ActionResult GetAreaEmpList(int area_id, uint type)
        {
            var data = app.GetAreaEmpList(area_id, type);
            return Content(data);
        }

        /// <summary>
        /// 经销商-业务员
        /// </summary>
        /// <param name="infoList"></param>
        /// <param name="jobInfoList"></param>
        /// <returns></returns>
        [HttpPost]
        [HandlerAjaxOnly]
        [ValidateAntiForgeryToken]
        public ActionResult SalesAdd(string emp_id, int area_id, daoben_distributor_info distributorInfo)
        {
            string result = app.SalesAdd(emp_id,area_id, distributorInfo);
            if (result == "success")
                return Success("设置成功。");
            else
                return Error(result);
        }
        [HttpPost]
        [HandlerAjaxOnly]
        [ValidateAntiForgeryToken]
        public ActionResult TrainerAdd(string emp_id, int area_id, daoben_distributor_info distributorInfo)
        {
            string result = app.TrainerAdd(emp_id, area_id, distributorInfo);
            if (result == "success")
                return Success("设置成功。");
            else
                return Error(result);
        }

        [HttpPost]
        [HandlerAjaxOnly]
        [ValidateAntiForgeryToken]
        public ActionResult SalesManageAdd(string emp_id, int area_id)
        {
            string result = app.SalesManageAdd(emp_id, area_id);
            if (result == "success")
                return Success("设置成功。");
            else
                return Error(result);
        }
        [HttpPost]
        [HandlerAjaxOnly]
        [ValidateAntiForgeryToken]
        public ActionResult TrainerManageAdd(string emp_id, int area_id)
        {
            string result = app.TrainerManageAdd(emp_id, area_id);
            if (result == "success")
                return Success("设置成功。");
            else
                return Error(result);
        }

        [HttpPost]
        [HandlerAjaxOnly]
        [ValidateAntiForgeryToken]
        public ActionResult GuideAdd(string emp_id, int area_id, daoben_distributor_info distributorInfo)
        {
            string result = app.GuideAdd(emp_id, area_id, distributorInfo);
            if (result == "success")
                return Success("设置成功。");
            else
                return Error(result);
        }
    }
}