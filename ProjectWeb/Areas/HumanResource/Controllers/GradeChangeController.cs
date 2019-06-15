using System.Web.Mvc;
using Base.Code;
using ProjectWeb.Areas.HumanResource.Application;
using ProjectShare.Database;
using System.Collections.Generic;
using System;
using ProjectShare.Models;

namespace ProjectWeb.Areas.HumanResource.Controllers
{
    public class GradeChangeController : ControllerBase
    {
        private GradeChangeApp app = new GradeChangeApp();

        #region 页面显示       

        [HttpGet]
        [HandlerAuthorize]
        public ActionResult MyShow()
        {
            return View();
        }
        [HttpGet]
        [HandlerAuthorize]
        public ActionResult PosHolidayEdit()
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
        public ActionResult GetListAll(Pagination pagination, daoben_hr_emp_job queryInfo, QueryTime queryTime, int approve_status,
            bool isMyApprove=false)
        {
            object rows = null;
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
        /// 查看审批
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
        public ActionResult GetMyList(Pagination pagination, daoben_hr_emp_job queryInfo, int approve_status, QueryTime queryTime)
        {
            
             
             
                 
             
                 
            var data = new
            {
                rows = app.GetMyList(pagination, queryInfo, approve_status, queryTime),
                total = pagination.records,
            };
            return Content(data.ToJson());
        }

        /// <summary>
        /// 职等调整申请
        /// </summary>
        /// <param name="addInfo">emp_id,type,area_id_new,dept_id_new,grade_new,计划调整时间change_date 还有期望计划概况什么的</param>
        /// <param name="fileInfo"></param>
        /// <returns></returns>
        [HttpPost]
        [HandlerAjaxOnly]
        [ValidateAntiForgeryToken]
        public ActionResult Add(daoben_hr_grade_change addInfo, List<daoben_hr_grade_change_file> fileInfo)
        {
            string result = app.Add(addInfo, fileInfo);
            if (result == "success")
                return Success("操作成功。");
            else
                return Error(result);
        }

        [HttpGet]
        [HandlerAjaxOnly]
        public ActionResult GetInfo(string id)
        {
            string data = app.GetInfo(id);
            return Content(data);
        }

        /// <summary>
        /// 提交审核
        /// </summary>
        /// <param name="approveInfo"> main_id,status</param>
        /// <param name="mainInfo">id,date_approve</param>
        /// <returns></returns> 
        [HttpPost]
        [HandlerAjaxOnly]
        [ValidateAntiForgeryToken]
        public ActionResult Approve(daoben_hr_grade_change_approve approveInfo, daoben_hr_grade_change mainInfo)
        {
            if (approveInfo == null)
                return Error("信息没传过来");
            string result = app.Approve(approveInfo, mainInfo);
            if (result == "success")
                return Success("操作成功。");
            else
                return Error(result);
        }

        /// <summary>
        /// 申请者确认核实时间
        /// 放在->我的申请 ->查看详情 ->确认核实时间
        /// </summary>
        [HttpPost]
        [HandlerAjaxOnly]
        [ValidateAntiForgeryToken]
        public ActionResult Confirm(string id)
        {
            string result = app.Confirm(id);
            if (result == "success")
                return Success("操作成功。");
            else
                return Error(result);
        }

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
