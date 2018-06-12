using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;

namespace ASA.API
{
    public class ValidateModelStateAttribute : ActionFilterAttribute
    {
        public void onActionExecuting(HttpActionContext actionContext)
        {
            if (!actionContext.ModelState.IsValid)
            {
                actionContext.Response = actionContext.Request.CreateErrorResponse(HttpStatusCode.BadRequest,
                    actionContext.ModelState);
            }
        }
    }
    [AttributeUsage(AttributeTargets.Method, Inherited = true)]
    public class CheckModelForNullAttribute : ActionFilterAttribute
    {
        private readonly Func<Dictionary<string, object>, bool> _validate;

        public CheckModelForNullAttribute()
            : this(arguments =>
                arguments.ContainsValue(null))
        { }

        public CheckModelForNullAttribute(Func<Dictionary<string, object>, bool> checkCondition)
        {
            _validate = checkCondition;
        }

        public override void OnActionExecuting(HttpActionContext actionContext)
        {
            if (_validate(actionContext.ActionArguments))
            {
                actionContext.Response = actionContext.Request.CreateErrorResponse(
       HttpStatusCode.BadRequest, "The argument cannot be null");
            }
        }
    }
}