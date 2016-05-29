using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(VotacionesWebSite.Startup))]
namespace VotacionesWebSite
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
