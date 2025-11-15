using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(Council.UI.Startup))]
namespace Council.UI
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
