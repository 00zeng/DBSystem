using ProjectWeb.Areas.SalaryCalculate.Application;
using System.Web.Mvc;
using System.Web.Routing;

namespace ProjectWeb
{
    public class MvcApplication : System.Web.HttpApplication
    {
        private static System.Threading.Timer _timer;
       // private const int interval = 1000 * 60 * 10;//间隔时间 10分钟
        private const int intervalOneM = 1000 * 60;//间隔时间 1分钟
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);

            //if (_timer == null)
            //{
            //    _timer = new System.Threading.Timer(new System.Threading.TimerCallback(SystemUpdate), this, 0, intervalOneM);
            //}
        }

        private void SystemUpdate(object sender)
        {
            PayrollSettingApp payrollSettingApp = new PayrollSettingApp();
            payrollSettingApp.Add();
        }
    }
}
