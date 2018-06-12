using ASA.API.Controllers;
using ASA.Core.Interfaces;
using ASA.Core.Services;
using Elmah.Contrib.WebApi;
using FluentValidation.WebApi;
using Microsoft.Practices.Unity;
using System.Web.Http;
using System.Web.Http.ExceptionHandling;
using Unity.WebApi;
using System.Web.Http.Cors;
using ASA.Core.Infrastructure;
using ASA.Core.Repositories;


namespace ASA.API
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Web API configuration and services
            var container = new UnityContainer();
            //gateway service 
            container.RegisterType(typeof(IGatewayService), typeof(GatewayService));
            var gatewayService = typeof(IGatewayService);
            container.RegisterType<SubmissionController>(new InjectionConstructor(gatewayService));
            container.RegisterType<SubmissionManager>(new InjectionConstructor(gatewayService));

            //client service 
            container.RegisterType(typeof(IClientService), typeof(ClientService));
            var clientService = typeof(IClientService);
            container.RegisterType<ClientController>(new InjectionConstructor(clientService));

            //business sevice 
            container.RegisterType(typeof(IBusinessService), typeof(BusinessService));
            var businessService = typeof(IBusinessService);
            container.RegisterType<BusinessController>(new InjectionConstructor(businessService));

            //period service 
            container.RegisterType(typeof(IPeriodService), typeof(PeriodService));
            var periodService = typeof(IPeriodService);
            container.RegisterType<PeriodController>(new InjectionConstructor(periodService));

            //invoice service 
            container.RegisterType(typeof(IInvoiceService), typeof(InvoiceService));
            var invoiceService = typeof(IInvoiceService);
            container.RegisterType<InvoiceController>(new InjectionConstructor(invoiceService, periodService));
          

            //container.RegisterType(typeof(ISenderService), typeof(SenderService));            
            
            container.RegisterType(typeof(IPeriodDataRepository), typeof(PeriodDataRepository));
            container.RegisterType(typeof(IHMRCResponseRepository), typeof(HMRCResponseRepository));
            container.RegisterType(typeof(IInvoiceRepository), typeof(InvoiceRepository));
            container.RegisterType(typeof(IClientRepository), typeof(ClientRepository));
            container.RegisterType(typeof(IBusinessRepository), typeof(BusinessRepository));
            container.RegisterType(typeof(IUnitOfWork), typeof(UnitOfWork));
            container.RegisterType(typeof(IDataBaseFactory), typeof(DataBaseFactory), new PerResolveLifetimeManager());

            GlobalConfiguration.Configuration.DependencyResolver = new UnityDependencyResolver(container);
            config.Services.Add(typeof(IExceptionLogger), new ElmahExceptionLogger());
            config.Filters.Add(new ValidateModelStateAttribute());
            FluentValidationModelValidatorProvider.Configure(config);
            var cors = new EnableCorsAttribute("*", "*", "*");
            config.EnableCors(cors);
            // Web API routes
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );
        }
    }
}
