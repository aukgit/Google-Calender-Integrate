using System;
using System.Threading;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using DevMvcComponent;
using DevMvcComponent.Mail;

namespace CalendarMvc
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            var serviceStart = App.ExchangeServiceAccess;
            var mailServer = new GmailServer("calendar.test88@gmail.com", "calendar8");

            Mvc.Setup("Outlook Calendar Testing", "akarim@relisource.com,afrahman@relisource.com",System.Reflection.Assembly.GetExecutingAssembly(), mailServer);

        }
    }
}
