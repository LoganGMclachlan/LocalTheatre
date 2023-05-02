using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(Local_Theatre.Startup))]
namespace Local_Theatre
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
