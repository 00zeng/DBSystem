using Base.Code;
using System.Web.Mvc;

namespace ProjectWeb
{
    public class HandlerLoginAttribute : AuthorizeAttribute
    {
        public bool Ignore = true;
        public HandlerLoginAttribute(bool ignore = true)
        {
            Ignore = ignore;
        }
        public override void OnAuthorization(AuthorizationContext filterContext)
        {
            if (Ignore == false)
            {
                return;
            }
            if (OperatorProvider.Provider.GetCurrent() == null)
            {
                filterContext.HttpContext.Response.Write("<script>top.location.href = '/Account/Login';</script>");
                return;
            }
        }
    }
}