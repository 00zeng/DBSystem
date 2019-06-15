using System;
using System.Web.Mvc;
using Base.Code;
using ProjectWeb.Application;
using ProjectShare.Database;
using ProjectShare.Models;

namespace ProjectWeb.Controllers
{
    public class AccountController : Controller
    {
        private AccountApp app = new AccountApp();
        // GET: Account
        public ActionResult Login()
        {
            return View();
        }

        public ActionResult UpdatePassword()
        {
            return View();
        }

        [HttpPost]
        [HandlerAjaxOnly]
        public ActionResult CheckLogin(daoben_ms_account accountInfo)
        {
            try
            {
                //if (!ModelState.IsValid)
                //{
                //    return Error("登录失败！");
                //}
                LoginStat result = app.LoginCheck(accountInfo, false);
                if (result == LoginStat.Success)
                {
                    return Content(new AjaxResult { state = ResultType.success.ToString(), message = "登录成功。" }.ToJson());
                }
                else if (result == LoginStat.LockedOut)
                    return Content(new AjaxResult { state = ResultType.error.ToString(), message = "该账户已经锁定！" }.ToJson());
                else if (result == LoginStat.Forbidden)
                    return Content(new AjaxResult { state = ResultType.error.ToString(), message = "该账户已经停用！" }.ToJson());
                else if (result == LoginStat.Failure)
                    return Content(new AjaxResult { state = ResultType.error.ToString(), message = "用户名或密码错误！" }.ToJson());
                else
                    return Content(new AjaxResult { state = ResultType.error.ToString(), message = "登录失败，请联系管理员！" }.ToJson());
            }
            catch
            {
                return Content(new AjaxResult { state = ResultType.error.ToString(), message = "登录失败，请联系管理员！" }.ToJson());
            }
        }


        [HttpPost]
        public ActionResult OutLogin()
        {
            try
            {
                Session.Abandon();
                Session.Clear();
                OperatorProvider.Provider.RemoveCurrent();
                return Content(new AjaxResult { state = ResultType.success.ToString(), message = "退出成功！" }.ToJson());
            }
            catch (Exception ex)
            {
            }
            return Content(new AjaxResult { state = ResultType.error.ToString(), message = "退出失败！" }.ToJson());
        }

        [HttpPost]
        [HandlerAjaxOnly]
        public ActionResult UpdatePassword(string pass, string new_pass)
        {
            string result = app.UpdatePassword(pass, new_pass);
            if (result == "success")
                return Content(new AjaxResult { state = ResultType.success.ToString(), message = "账号重置成功！" }.ToJson());
            else
                return Content(new AjaxResult { state = ResultType.error.ToString(), message = result }.ToJson());
        }
    }
}