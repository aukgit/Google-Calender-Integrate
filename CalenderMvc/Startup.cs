using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(CalenderMvc.Startup))]
namespace CalenderMvc
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
