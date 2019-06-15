using System.Web.Mvc;
using Base.Code;
using ProjectWeb.Areas.HumanResource.Application;
using ProjectShare.Database;
using System.Collections.Generic;
using System;
using ProjectShare.Models;

namespace ProjectWeb.Areas.HumanResource.Controllers
{
    public class PositionChangeController : ControllerBase
    {
        private PositionChangeApp app = new PositionChangeApp();

        #region 页面显示
        [HttpGet]
        [HandlerAuthorize]
        public ActionResult MyAdd()
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
        public ActionResult MyShow()
        {
            return View();
        }

        #endregion

        /// <summary>
        /// 查看全部
        /// </summary>
        [HttpGet]
        [HandlerAjaxOnly]
        public ActionResult GetListAll(Pagination pagination, daoben_hr_position_change queryInfo, QueryTime queryTime, int? status = null,bool? isMyApprove = null)
        {
            object rows = null;               
            if (isMyApprove == true)
                rows = app.GetMyList(pagination, queryInfo, status, queryTime);
            else
                rows = app.GetListAll(pagination, queryInfo, status, queryTime);
            var data = new
            {
                rows = rows,
                total = pagination.records,
            };
            return Content(data.ToJson());
        }

        /// <summary>
        /// 我的审批
        /// </summary>
        [HttpGet]
        [HandlerAjaxOnly]
        public ActionResult GetListApprove(Pagination pagination, daoben_hr_position_change queryInfo)
        {
            var data = new
            {
                rows = app.GetListApprove(pagination, queryInfo),
                total = pagination.records,
            };
            return Content(data.ToJson());
        }
        /// <summary>
        /// 我的申请 TO BE DELETE
        /// </summary>
        [HttpGet]
        [HandlerAjaxOnly]
        public ActionResult GetMyList(Pagination pagination, daoben_hr_position_change queryInfo, QueryTime queryTime, int? status = null)
        {
            var data = new
            {
                rows = app.GetMyList(pagination, queryInfo, status, queryTime),
                total = pagination.records,
            };
            return Content(data.ToJson());
        }

        /// <summary>
        /// GetInfo by ID
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
        /// <summary>
        /// 岗位调整申请
        /// </summary>
        /// <param name="addInfo">emp_id,type,area_id_new,dept_id_new,company_id_new,change_date </param>
        /// <param name="fileInfo"></param>
        /// <returns></returns>
        [HttpPost]
        [HandlerAjaxOnly]
        [ValidateAntiForgeryToken]
        public ActionResult Add(daoben_hr_position_change addInfo, daoben_hr_position_change_file fileInfo)
        {
            if (addInfo == null)
                return Error("信息错误：获取的数据为空");
            string result = app.Add(addInfo, fileInfo);
            if (result == "success")
                return Success("操作成功。");
            else
                return Error(result);
        }

        /// <summary>
        /// 提交审核
        /// </summary>
        /// <param name="approveInfo"> main_id,status</param>
        /// /// <param name="effectInfoList">id,date_approve</param>
        /// <param name="effectInfoList">all</param>
        /// <returns></returns> 
        [HttpPost]
        [HandlerAjaxOnly]
        [ValidateAntiForgeryToken]
        public ActionResult Approve(daoben_hr_position_change_approve approveInfo, daoben_hr_position_change mainInfo, List<daoben_hr_position_change_effect> effectInfoList)
        {
            if (approveInfo == null)
                return Error("信息错误：审批信息为空");
            string result = app.Approve(approveInfo, mainInfo, effectInfoList);
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
        public ActionResult Confirm(daoben_hr_position_change confirmInfo)
        {
            string result = app.Confirm(confirmInfo);
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
