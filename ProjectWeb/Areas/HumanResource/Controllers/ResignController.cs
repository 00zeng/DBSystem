using System.Web.Mvc;
using Base.Code;
using ProjectWeb.Areas.HumanResource.Application;
using ProjectShare.Database;
using System.Collections.Generic;
using System;
using ProjectShare.Models;

namespace ProjectWeb.Areas.HumanResource.Controllers
{
    public class ResignController : ControllerBase
    {
        private ResignApp app = new ResignApp();


        /// <summary>
        /// 查看所有离职申请
        /// </summary>
        /// <param name="pagination"></param>
        /// <param name="queryInfo"></param>
        /// <param name="status">审批状态
        /// 1-未审批 2-审批中 3-已审批 4-不通过
        /// </param>
        /// <returns></returns>
        [HttpGet]
        [HandlerAjaxOnly]
        public ActionResult GetListAll(Pagination pagination, daoben_hr_resign queryInfo, QueryTime queryTime, int? status = null, bool? isMyApprove = null)
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
        /// <param name="pagination"></param>
        /// <param name="resignInfo"></param>
        /// <returns></returns>
        [HttpGet]
        [HandlerAjaxOnly]
        public ActionResult GetListApprove(Pagination pagination, daoben_hr_resign queryInfo, QueryTime queryTime, int? status = null)
        {
            var data = new
            {
                rows = app.GetListApprove(pagination, queryInfo, status, queryTime),
                total = pagination.records,
            };
            return Content(data.ToJson());
        }

        /// <summary>
        /// 我的申请 TO BE DELETE
        /// </summary>
        [HttpGet]
        [HandlerAjaxOnly]
        public ActionResult GetMyList(Pagination pagination, daoben_hr_resign queryInfo, QueryTime queryTime, int? status = null, bool isMyApprove = true)
        {  
            var data = new
            {
                rows = app.GetMyList(pagination, queryInfo, status, queryTime),
                total = pagination.records,
            };
            return Content(data.ToJson());
        }
        /// <summary>
        /// 获取离职信息
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
        public ActionResult Approve(daoben_hr_resign_approve approveInfo, daoben_hr_resign mainInfo, List<daoben_hr_resign_effect> effectInfoList)
        {
            if (approveInfo == null)
                return Error("信息没传过来");
            string result = app.Approve(approveInfo, mainInfo, effectInfoList);
            if (result == "success")
                return Success("操作成功。");
            else
                return Error(result);
        }

        /// <summary>
        /// 申请者确认核实时间
        /// 放在->我的申请 ->查看详情 ->确认核实时间/撤销申请
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
        public ActionResult Add(daoben_hr_resign addInfo, List<daoben_hr_resign_file> fileInfo)
        {
            string result = app.Add(addInfo, fileInfo);
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
