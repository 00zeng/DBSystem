using Base.Code;
using System.Web.Mvc;

namespace ProjectWeb
{
    public class HandlerHomeLoginAttribute: AuthorizeAttribute
    {
        public bool Ignore = true;
        public HandlerHomeLoginAttribute(bool ignore = true)
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
                filterContext.HttpContext.Response.Write("<script>location.href = '/Account/Login';</script>");
                return;
            }
        }
    }
}