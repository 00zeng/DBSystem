using System.Web.Mvc;

namespace ProjectWeb.Areas.SubordinateManage
{
    public class SubordinateManageAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "SubordinateManage";
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