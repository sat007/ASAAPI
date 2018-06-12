//using FluentValidation.WebApi;
using System.Web.Http;
namespace ASA.API
{
    public class WebApiApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {

            GlobalConfiguration.Configure(WebApiConfig.Register);
            //UnityConfig.Initialise();
            //FluentValidationModelValidatorProvider.Configure(GlobalConfiguration.Configuration);
        }
    }
}
