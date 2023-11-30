using Ninject;
using Ninject.Web.Common.WebHost;
using System.Web.Mvc;
using System.Web.Routing;
using WebAnime.App_Start;

namespace WebAnime
{
    public class MvcApplication : NinjectHttpApplication
    {
        protected override void OnApplicationStarted()//Application_Start
        {
            AreaRegistration.RegisterAllAreas();
            RouteConfig.RegisterRoutes(RouteTable.Routes);
        }
        protected override IKernel CreateKernel() => NInjectConfig.Kernel;//DI Container
    }
}
