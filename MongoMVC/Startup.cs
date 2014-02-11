using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(MongoMVC.Startup))]
namespace MongoMVC
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
