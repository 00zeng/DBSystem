using System.Web.Mvc;

namespace ProjectWeb.Areas.DistributorManage
{
    public class DistributorManageAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "DistributorManage";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
                this.AreaName + "_Default",
                this.AreaName + "/{controller}/{action}/{id}",
                new { area = this.AreaName, controller = "Home", action = "Index", id = UrlParameter.Optional },
                new string[] { "ProjectWeb.Areas." + this.AreaName + ".Controllers" }
            );
        }
    }
}