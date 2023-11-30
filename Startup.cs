using Microsoft.Owin;
using Owin;
using System;
using System.Threading.Tasks;
using static WebAnime.App_Start.OwinCofig;

[assembly: OwinStartup(typeof(WebAnime.Startup))]

namespace WebAnime
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            AuthConfig(app);
        }
    }
}
