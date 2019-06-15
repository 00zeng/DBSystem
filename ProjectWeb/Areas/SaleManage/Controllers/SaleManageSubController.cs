using System.Web.Mvc;
using Base.Code;
//using ProjectWeb.Areas.SaleManage.Application;
using ProjectShare.Database;

namespace ProjectWeb.Areas.SaleManage.Controllers
{
    public class SaleManageSubController : ControllerBase
    {
       

        #region 页面显示
        [HttpGet]
        [HandlerAuthorize]
        public ActionResult SubAdd()
        {
            return View();
        }
        [HttpGet]
        [HandlerAuthorize]
        public ActionResult SubShow()
        {
            return View();
        }

        [HttpGet]
        [HandlerAuthorize]
        public ActionResult SubApproval()
        {
            return View();
        }
        #endregion

        
    }
}