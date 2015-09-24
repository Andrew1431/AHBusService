using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(AHBusService.Startup))]
namespace AHBusService
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
