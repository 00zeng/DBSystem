using System.Web.Mvc;
using Base.Code;
using ProjectShare.Database;
using ProjectWeb.Areas.OrganizationManage.Application;

namespace ProjectWeb.Areas.OrganizationManage.Controllers
{
    public class DepartmentController : ControllerBase
    {
        private DeptApp app = new DeptApp();

        [HttpGet]
        [HandlerAjaxOnly]
        public ActionResult GetGridJson(Pagination pagination, daoben_org_dept queryInfo)
        {
            var data = new
            {
                rows = app.GetList(pagination, queryInfo),
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
        public ActionResult Add(daoben_org_dept deptInfo, string position_name, daoben_org_position positionInfo)
        {
            string result = app.Add(deptInfo, position_name, positionInfo);
            if (result == "success")
                return Success("操作成功。", deptInfo.id);
            else
                return Error(result);
        }

        [HttpPost]
        [HandlerAjaxOnly]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(daoben_org_dept deptInfo)
        {
            string result = app.Edit(deptInfo);
            if (result == "success")
                return Success("操作成功。", deptInfo.id);
            else
                return Error(result);
        }

        [HttpPost]
        [HandlerAjaxOnly]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id)
        {
            string result = app.Delete(id);
            if (result == "success")
                return Success("操作成功。", id);
            else
                return Error(result);
        }

        /// <summary>
        /// 根据机构获取部门选择列表
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
        public ActionResult GetDeptAddrList(int id)
        {
            var data = app.GetDeptAddrList(id);
            return Content(data);
        }

    }
}