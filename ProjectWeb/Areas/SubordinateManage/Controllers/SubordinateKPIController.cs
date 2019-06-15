using System.Web.Mvc;
using Base.Code;
using ProjectWeb.Areas.SubordinateManage.Application;
using ProjectShare.Database;

namespace ProjectWeb.Areas.SubordinateManage.Controllers
{
    public class SubordinateKPIController : ControllerBase
    {
        private SubordinateKPIApp app = new SubordinateKPIApp();


        #region 基础操作
        #region 页面显示
        [HttpGet]
        [HandlerAuthorize]
        public ActionResult SubordinateShow()
        {
            return View();
        }
        [HttpGet]
        [HandlerAuthorize]
        public ActionResult SubordinateAdd()
        {
            return View();
        }
        [HttpGet]
        [HandlerAuthorize]
        public ActionResult SubordinateEdit()
        {
            return View();
        }
        [HttpGet]
        [HandlerAuthorize]
        public ActionResult NewArea()
        {
            return View();
        }

        #endregion

        [HttpGet]
        [HandlerAjaxOnly]
        public ActionResult ShowInfo()
        {

            string Info = app.GetInfo();
            return Content(Info);
        }






        [HttpGet]
        [HandlerAjaxOnly]
        public ActionResult GetGridJson(Pagination pagination, string name = null, string work_number = null)
        {
            var data = new
            {
                rows = app.GetList(pagination, name, work_number),
                total = pagination.records,
            };
            return Content(data.ToJson());
        }
        #endregion

    }
}