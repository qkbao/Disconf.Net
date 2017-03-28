using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(Disconf.Net.Web.Startup))]
namespace Disconf.Net.Web
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
        }
    }
}
