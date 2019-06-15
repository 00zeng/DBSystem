using System.Web.Mvc;
using Base.Code;
using ProjectWeb.Areas.SubordinateManage.Application;
using ProjectShare.Database;
using System;
using System.Collections.Generic;
using ProjectShare.Models;

namespace ProjectWeb.Areas.SubordinateManage.Controllers
{
    /// <summary>
    /// 待指派业务员列表
    /// 待指派导购员列表
    /// 待指派业务经理列表
    /// 指派 导购员 给 经销商
    /// 指派 业务员 给 区域
    /// 指派 业务经理 给 区域
    /// </summary>
    public class AssignmentController : ControllerBase
    {
        private AssignmentApp app = new AssignmentApp();


        #region 页面显示


        [HttpGet]
        [HandlerAuthorize]
        public ActionResult GuideIndex()
        {
            return View();
        }
        [HttpGet]
        [HandlerAuthorize]
        public ActionResult TrainerIndex()
        {
            return View();
        }
        [HttpGet]
        [HandlerAuthorize]
        public ActionResult TrainerManagerIndex()
        {
            return View();
        }
        [HttpGet]
        [HandlerAuthorize]
        public ActionResult SalesIndex()
        {
            return View();
        }
        [HttpGet]
        [HandlerAuthorize]
        public ActionResult SalesManagerIndex()
        {
            return View();
        }

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


        /// <summary>
        /// 待分配导购员列表
        /// </summary>
        [HttpGet]
        [HandlerAjaxOnly]
        public ActionResult GuideList(Pagination pagination, string queryName = null, bool isAllGuide = false)
        {
            var data = new
            {
                rows = app.GetGuideList(pagination, queryName, isAllGuide),
                total = pagination.records,
            };
            return Content(data.ToJson());
        }
        /// <summary>
        /// 待分配业务员列表
        /// </summary>
        [HttpGet]
        [HandlerAjaxOnly]
        public ActionResult SalesList(Pagination pagination, string queryName = null, bool isAllSales = false)
        {
            var data = new
            {
                rows = app.GetSalesList(pagination, queryName, isAllSales),
                total = pagination.records,
            };
            return Content(data.ToJson());
        }
        /// <summary>
        /// 待分配业务经理列表
        /// </summary>
        [HttpGet]
        [HandlerAjaxOnly]
        public ActionResult SalesManagerList(Pagination pagination, string queryName = null, bool isAllSalesManger = false)
        {
            var data = new
            {
                rows = app.GetSalesManager(pagination, queryName, isAllSalesManger),
                total = pagination.records,
            };
            return Content(data.ToJson());
        }
        /// <summary>
        /// 待分配培训师列表
        /// </summary>
        [HttpGet]
        [HandlerAjaxOnly]
        public ActionResult GetTrainer(Pagination pagination, string queryName = null, bool isAllTrainer = false)
        {
            var data = new
            {
                rows = app.GetTrainer(pagination, queryName, isAllTrainer),
                total = pagination.records,
            };
            return Content(data.ToJson());
        }
        /// <summary>
        /// 待分配培训经理列表
        /// </summary>
        [HttpGet]
        [HandlerAjaxOnly]
        public ActionResult GetTrainerManager(Pagination pagination, string queryName = null, bool isAllTrainerManger = false)
        {
            var data = new
            {
                rows = app.GetTrainerManager(pagination, queryName, isAllTrainerManger),
                total = pagination.records,
            };
            return Content(data.ToJson());
        }


        /// <summary>
        /// 经销商-业务员
        /// </summary>       
        [HttpPost]
        [HandlerAjaxOnly]
        [ValidateAntiForgeryToken]
        public ActionResult SalesAdd(string emp_id, int area_l2_id, DateTime? effect_date)
        {
            string result = app.SalesAdd(emp_id, area_l2_id, effect_date);
            if (result == "success")
                return Success("设置成功。");
            else
                return Error(result);
        }
       
        /// <summary>
        /// 业务经理 - 业务片区
        /// </summary>
        /// <param name="emp_id">业务经理员工ID</param>
        /// <param name="area_l1_id">业务片区ID</param>
        /// <returns></returns>
        [HttpPost]
        [HandlerAjaxOnly]
        [ValidateAntiForgeryToken]
        public ActionResult SalesManageAdd(string emp_id, int area_l1_id, DateTime? effect_date)
        {
            string result = app.SalesManageAdd(emp_id, area_l1_id, effect_date);
            if (result == "success")
                return Success("设置成功。");
            else
                return Error(result);
        }

        /// <summary>
        /// 培训师 - 业务片区
        /// </summary>
        /// <param name="emp_id">业务经理员工ID</param>
        /// <param name="area_l1_id">业务片区ID</param>
        /// <returns></returns>
        [HttpPost]
        [HandlerAjaxOnly]
        [ValidateAntiForgeryToken]
        public ActionResult TrainerAdd(string emp_id, int area_l1_id)
        {
            string result = app.TrainerAdd(emp_id, area_l1_id);
            if (result == "success")
                return Success("设置成功。");
            else
                return Error(result);
        }
        /// <summary>
        /// 培训师 - 业务片区
        /// </summary>
        /// <param name="emp_id">业务经理员工ID</param>
        /// <param name="area_l1_id">业务片区ID</param>
        /// <returns></returns>
        [HttpPost]
        [HandlerAjaxOnly]
        [ValidateAntiForgeryToken]
        public ActionResult TrainerManagerAdd(string emp_id, int company_id)
        {
            string result = app.TrainerManagerAdd(emp_id, company_id);
            if (result == "success")
                return Success("设置成功。");
            else
                return Error(result);
        }
        /// <summary>
        /// 导购员 - 经销商
        /// </summary>        
        [HttpPost]
        [HandlerAjaxOnly]
        [ValidateAntiForgeryToken]
        public ActionResult GuideAdd(string emp_id, int area_l2_id, List<daoben_distributor_info> distributorList, DateTime? effect_date)
        {
            string result = app.GuideAdd(emp_id, area_l2_id, distributorList, effect_date);
            if (result == "success")
                return Success("设置成功。");
            else
                return Error(result);
        }


        /// <summary>
        /// 解除导购员的分配
        /// </summary>
        /// <param name="emp_id">导购员员工id</param>
        /// <returns></returns>
        [HttpPost]
        [HandlerAjaxOnly]
        [ValidateAntiForgeryToken]
        public ActionResult RemoveGuide(string emp_id)
        {
            string result = app.Remove(emp_id, 2);
            if (result == "success")
                return Success("设置成功。");
            else
                return Error(result);
        }
        /// <summary>
        /// 解除培训师的分配
        /// </summary>
        /// <param name="emp_id">导购员员工id</param>
        /// <returns></returns>
        [HttpPost]
        [HandlerAjaxOnly]
        [ValidateAntiForgeryToken]
        public ActionResult RemoveTrainer(string emp_id)
        {
            string result = app.Remove(emp_id, 3);
            if (result == "success")
                return Success("设置成功。");
            else
                return Error(result);
        }
        /// <summary>
        /// 解除培训经理的分配
        /// </summary>
        /// <param name="emp_id">导购员员工id</param>
        /// <returns></returns>
        [HttpPost]
        [HandlerAjaxOnly]
        [ValidateAntiForgeryToken]
        public ActionResult RemoveTrainerManager(string emp_id)
        {
            string result = app.Remove(emp_id, 4);
            if (result == "success")
                return Success("设置成功。");
            else
                return Error(result);
        }
        [HttpPost]
        [HandlerAjaxOnly]
        [ValidateAntiForgeryToken]
        public ActionResult RemoveSale(string emp_id)
        {
            string result = app.Remove(emp_id, 1);
            if (result == "success")
                return Success("设置成功。");
            else
                return Error(result);
        }
        [HttpPost]
        [HandlerAjaxOnly]
        [ValidateAntiForgeryToken]
        public ActionResult RemoveSalesManager(string emp_id)
        {
            string result = app.Remove(emp_id, 0);
            if (result == "success")
                return Success("设置成功。");
            else
                return Error(result);
        }
        
        [HttpGet]
        [HandlerAjaxOnly]
        public ActionResult GetGuideInfo(string id)
        {
            string data = app.GetGuideInfo(id);
            return Content(data);
        }
        [HttpGet]
        [HandlerAjaxOnly]
        public ActionResult GetSalesInfo(string id)
        {
            string data = app.GetSalesInfo(id);
            return Content(data);
        }
        [HttpGet]
        [HandlerAjaxOnly]
        public ActionResult GetSalesManagerInfo(string id)
        {
            string data = app.GetSalesInfo(id);
            return Content(data);
        }
    }
}