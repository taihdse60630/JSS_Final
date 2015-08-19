using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(JobSearchingSystem.Startup))]
namespace JobSearchingSystem
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
