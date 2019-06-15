using System.Web.Mvc;
using Base.Code;
using ProjectWeb.Areas.SystemManage.Application;
using System;

namespace ProjectWeb.Controllers
{
    public class ClientsDataController : Controller
    {
        [HttpGet]
        public ActionResult GetClientsDataJson()
        {
            object menubuttonlist = null;
            OperatorModel LoginInfo = OperatorProvider.Provider.GetCurrent();
            if (LoginInfo == null)
                throw new Exception("用户登陆过期，请重新登录");
            var data = new
            {
                authorizeMenu = new SysMenuApp().GetMenuList(LoginInfo.roleId, ref menubuttonlist),
                authorizeButton = menubuttonlist,
                loginInfo = LoginInfo,
            };
            return Content(data.ToJson());
        }

        [HttpGet]
        public ActionResult GetGUID()
        {
            object retObj = new { guid = Common.GuId() };

            return Content(retObj.ToJson());
        }
    }
}