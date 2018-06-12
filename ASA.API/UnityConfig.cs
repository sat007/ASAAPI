using ASA.API.Controllers;
using ASA.Core.Interfaces;
using ASA.Core.Services;
using Microsoft.Practices.Unity;

namespace ASA.API
{
    ///Not used as unity is loaded in webconfig file  
    ///
    //public static class UnityConfig
    //{
    //    private static IUnityContainer _unityContainer;
    //    public static IUnityContainer Container
    //    {
    //        get { return _unityContainer; }
    //    }
    //    public static void Initialise()
    //    {
    //        var container = new UnityContainer();
    //        container.RegisterType(typeof(IGatewayService), typeof(GatewayService));
    //        var gatewayService = typeof(IGatewayService);
    //        container.RegisterType<SubmissionController>(new InjectionConstructor(gatewayService));
    //        container.RegisterType<SubmissionManager>(new InjectionConstructor(gatewayService));
    //        _unityContainer = container;
            
    //    }

    //}
}