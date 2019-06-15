using Base.Code;
using ProjectShare.Database;
using ProjectShare.Models;
using ProjectWeb.Areas.HumanResource.Application;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ProjectWeb.Areas.HumanResource.Controllers
{
    public class LeavingController : ControllerBase
    {
        private LeavingApp app = new LeavingApp();

        #region 页面显示
        [HttpGet]
        [HandlerAuthorize]
        public ActionResult UpdateEmp()
        {
            return View();
        }

        [HttpGet]
        [HandlerAuthorize]
        public ActionResult NewEmp()
        {
            return View();
        }

        #endregion


        /// <summary>
        /// 查看全部
        /// </summary>
        [HttpGet]
        [HandlerAjaxOnly]
        public ActionResult GetListAll(Pagination pagination, daoben_hr_emp_job queryInfo, QueryTime queryTime, 
            int approve_status,bool isMyApprove = false)
        {    
            object rows;
            if (isMyApprove == true)
                rows = app.GetMyList(pagination, queryInfo, approve_status, queryTime);
            else
                rows = app.GetListAll(pagination, queryInfo, approve_status, queryTime);
            var data = new
            {
                rows = rows,
                total = pagination.records,
            };
            return Content(data.ToJson());
        }


        /// <summary>
        /// 我的审批
        /// 部门经理 ： 本部门除了自己，第一段审批
        /// 分公司助理 ：分公司部门经理，第一阶段
        /// 分公司总经理 ：分公司除了自己，第二阶段
        /// 事业部助理 ： 事业部部门经理，第一阶段
        /// 事业部总经理 ：事业部，第二阶段
        /// 人事 ：全部第三阶段
        /// </summary>
        [HttpGet]
        [HandlerAjaxOnly]
        public ActionResult GetListApprove(Pagination pagination, daoben_hr_emp_job queryInfo)
        {
            
            var data = new
            {
                rows = app.GetListApprove(pagination, queryInfo),
                total = pagination.records,
            };
            return Content(data.ToJson());
        }

        /// <summary>
        /// 查看我的申请 to be delete
        /// </summary>
        [HttpGet]
        [HandlerAjaxOnly]
        public ActionResult GetMyList(Pagination pagination, daoben_hr_emp_job queryInfo, QueryTime queryTime, int status)
        {
            var data = new
            {
                rows = app.GetMyList(pagination, queryInfo, status,queryTime),
                total = pagination.records,
            };
            return Content(data.ToJson());
        }



        /// <summary>
        /// 通过申请ID获取申请表信息
        /// </summary>
        [HttpGet]
        [HandlerAjaxOnly]
        public ActionResult GetInfo(string id)
        {
            string data = app.GetInfo(id);
            return Content(data);
        }

        /// <summary>
        /// 新增申请
        /// </summary>
        [HttpPost]
        [HandlerAjaxOnly]
        [ValidateAntiForgeryToken]
        public ActionResult Add(daoben_hr_leaving addInfo, List<daoben_hr_leaving_file> image_list)
        {
            string result = app.Add(addInfo, image_list);
            if (result == "success")
                return Success("操作成功。");
            else
                return Error(result);
        }


        /// <summary>
        /// 提交审批
        /// </summary>
        [HttpPost]
        [HandlerAjaxOnly]
        [ValidateAntiForgeryToken]
        public ActionResult Approve(daoben_hr_leaving_approve approveInfo)
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
            string result = app.Delete(id);
            if (result == "success")
                return Success("操作成功。", id);
            else
                return Error(result);
        }

    }
}