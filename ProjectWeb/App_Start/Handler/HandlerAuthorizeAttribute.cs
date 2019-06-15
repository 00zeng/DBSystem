using System.Text;
using System.Web;
using System.Web.Mvc;
using Base.Code;
using ProjectWeb.Areas.SystemManage.Application;
using ProjectShare.Models;

namespace ProjectWeb
{
    public class HandlerAuthorizeAttribute : ActionFilterAttribute
    {
        public bool Ignore { get; set; }
        public HandlerAuthorizeAttribute(bool ignore = true)
        {
            Ignore = ignore;
        }
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var current = OperatorProvider.Provider.GetCurrent();
            if (current == null)
            {
                WebHelper.WriteCookie("deepinf_login_error", "overdue");
                filterContext.HttpContext.Response.Write("<script>top.location.href = '/Account/Login';</script>");
                return;
            }
            if (Ignore == false)
            {
                return;
            }
            // 暂不检查页面操作权限
            return;
            if (!this.ActionAuthorize())
            {
                StringBuilder sbScript = new StringBuilder();
                sbScript.Append("<script type='text/javascript'>alert('很抱歉！您的权限不足，访问被拒绝！');</script>");
                filterContext.Result = new ContentResult() { Content = sbScript.ToString() };
                return;
            }
        }
        private bool ActionAuthorize()
        {
            OperatorModel LoginInfo = OperatorProvider.Provider.GetCurrent();
            int roleId = LoginInfo == null ? 0 : LoginInfo.roleId;
            if (roleId == ConstData.ROLE_ID_ADMIN)
                return true;
            try
            {
                string url = HttpContext.Current.Request.ServerVariables["SCRIPT_NAME"].ToString().Split('?')[0];
                if (string.IsNullOrEmpty(url))
                    return true;
                return new SysAuthorityApp().ActionValidate(roleId, url);
            }
            catch { return true; }
        }

    }
}