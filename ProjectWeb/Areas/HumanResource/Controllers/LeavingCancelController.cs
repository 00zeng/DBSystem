using System.Web.Mvc;
using Base.Code;
using ProjectWeb.Areas.HumanResource.Application;
using ProjectShare.Database;
using System;
using ProjectShare.Models;

namespace ProjectWeb.Areas.HumanResource.Controllers
{
    public class LeavingCancelController : ControllerBase
    {
        private LeavingCancelApp app = new LeavingCancelApp();


        /// <summary>
        /// 查看全部
        /// </summary>
        [HttpGet]
        [HandlerAjaxOnly]
        public ActionResult GetListAll(Pagination pagination, daoben_hr_leaving queryInfo,QueryTime queryTime, int? status = null, bool? isMyApprove = null)
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
        /// 部门经理 ： 本部门除了自己，第一段审批
        /// 分公司助理 ：分公司部门经理，第一阶段
        /// 分公司总经理 ：分公司除了自己，第二阶段
        /// 事业部助理 ： 事业部部门经理，第一阶段
        /// 事业部总经理 ：事业部，第二阶段
        /// 人事 ：全部第三阶段
        /// </summary>
        [HttpGet]
        [HandlerAjaxOnly]
        public ActionResult GetListApprove(Pagination pagination, daoben_hr_leaving queryInfo,QueryTime queryTime, int? status = null)
        {
            var data = new
            {
                rows = app.GetListApprove(pagination, queryInfo, status, queryTime),
                total = pagination.records,
            };
            return Content(data.ToJson());
        }

        /// <summary>
        /// 查看我的申请 TO BE DELETE
        /// </summary>
        [HttpGet]
        [HandlerAjaxOnly]
        public ActionResult GetMyList(Pagination pagination, daoben_hr_leaving queryInfo, QueryTime queryTime, int? status = null, bool isMyApprove = true)
        {
            var data = new
            {
                rows = app.GetMyList(pagination, queryInfo, status, queryTime),
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
        /// 新增申请：leaving_id
        /// leaving_id 没有获取方式
        /// </summary>
        [HttpPost]
        [HandlerAjaxOnly]
        [ValidateAntiForgeryToken]
        public ActionResult Add(daoben_hr_leaving_cancel addInfo)
        {
            string result = app.Add(addInfo);
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
        public ActionResult Approve(daoben_hr_leaving_cancel_approve approveInfo)
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
