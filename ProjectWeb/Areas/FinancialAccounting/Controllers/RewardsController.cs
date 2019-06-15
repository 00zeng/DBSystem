using System.Web.Mvc;
using Base.Code;
using ProjectWeb.Areas.FinancialAccounting.Application;
using ProjectShare.Database;
using System;
using System.Collections.Generic;
using ProjectShare.Models;

namespace ProjectWeb.Areas.FinancialAccounting.Controllers
{
    public class RewardsController : ControllerBase
    {
        private RewardsApp app = new RewardsApp();

        #region 页面显示
        [HttpGet]
        [HandlerAuthorize]
        public ActionResult RewardsApprove()
        {
            return View();
        }
        [HttpGet]
        [HandlerAuthorize]
        public ActionResult RewardsRequest()
        {
            return View();
        }
        [HttpGet]
        [HandlerAuthorize]
        public ActionResult RewardsShow()
        {
            return View();
        }
        #endregion

        #region 查看
        
        [HttpGet]
        [HandlerAjaxOnly]
        public ActionResult GetGridJson(Pagination pagination, daoben_hr_emp_job queryInfo, QueryTime queryTime)
        {
            var data = new
            {
                rows = app.GetGridJson(pagination, queryInfo, queryTime),
                total = pagination.records,
            };
            return Content(data.ToJson());
        }


       
        [HttpGet]
        [HandlerAjaxOnly]
        public ActionResult GetGridJson2(Pagination pagination, daoben_hr_emp_job queryInfo)
        {
            var data = new
            {
                rows = app.GetGridJson2(pagination, queryInfo),
                total = pagination.records,
            };
            return Content(data.ToJson());
        }

       
        [HttpGet]
        [HandlerAjaxOnly]
        public ActionResult GetGridJson3(Pagination pagination, daoben_hr_emp_job queryInfo)
        {
            var data = new
            {
                rows = app.GetGridJson3(pagination, queryInfo),
                total = pagination.records,
            };
            return Content(data.ToJson());
        }

        #endregion

        [HttpPost]
        [HandlerAjaxOnly]
        [ValidateAntiForgeryToken]
        public ActionResult Add(daoben_salary_reward rewardsInfo, List<daoben_salary_reward_detail> rewardsAddList)
        {
            string result = app.Add(rewardsInfo,rewardsAddList);
            if (result == "success")
                return Success("操作成功。");
            else
                return Error(result);
        }
        /// <summary>
        /// 根据奖罚表ID获取奖罚数据
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [HandlerAjaxOnly]
        public ActionResult GetInfo(string id)
        {
            string data = app.GetInfo(id);
            return Content(data);
        }       

        [HttpPost]
        [HandlerAjaxOnly]
        [ValidateAntiForgeryToken]
        public ActionResult Approve(daoben_salary_reward_approve approveInfo)
        {
            if (approveInfo == null)
                return Error("信息没传过来");
            string result = app.Approve(approveInfo);
            if (result == "success")
                return Success("操作成功。");
            else
                return Error(result);
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


    }
}