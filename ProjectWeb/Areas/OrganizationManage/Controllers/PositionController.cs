using System.Web.Mvc;
using Base.Code;
using ProjectWeb.Areas.OrganizationManage.Application;
using ProjectShare.Database;

namespace ProjectWeb.Areas.OrganizationManage.Controllers
{
    public class PositionController : ControllerBase
    {
        private PositionApp app = new PositionApp();

        
        [HttpGet]
        [HandlerAjaxOnly]
        public ActionResult GetGridJson(Pagination pagination, daoben_org_position positionInfo)
        {
            var data = new
            {
                rows = app.GetList(pagination, positionInfo),
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
        public ActionResult Add(daoben_org_position positionInfo/*,int salary_category=0*/)//接收变量的随便什么
        {
            string result = app.Add(positionInfo);
            if (result == "success")
                return Success("操作成功。", positionInfo.id);
            else
                return Error(result);
        }

        [HttpPost]
        [HandlerAjaxOnly]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(daoben_org_position positionInfo)
        {
            string result = app.Edit(positionInfo);
            if (result == "success")
                return Success("操作成功。", positionInfo.id);
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
        /// 根据部门获取职位选择列表
        /// </summary>
        /// <param name="deptId"></param>
        /// <returns></returns>
        [HttpGet]
        [HandlerAjaxOnly]
        public ActionResult GetIdNameList(int company_id = 0, bool sub_company=false, int dept_id = 0, bool office = false)
        {
            var data = app.GetIdNameList(company_id, sub_company, dept_id, office);
            if (data == null)
                return null;
            return Content(data.ToJson());
        }

        [HttpGet]
        [HandlerAjaxOnly]
        public ActionResult GetGradeList(int position_id, int grade_category)
        {
            var data = app.GetGradeList(position_id, grade_category);
            return Content(data);
        }

    }
}