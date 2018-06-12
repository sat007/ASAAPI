using ASA.API.Models;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace ASA.API.Controllers
{
    public class CaliculateVATController : ApiController
    {
        [HttpPost]
        public HttpResponseMessage Post([FromBody]JObject data)
        {
            if (data != null)
            {
                CalcVATViewModel calcvm = data["CalcVATViewModel"].ToObject<CalcVATViewModel>();

                if (ModelState.IsValid)
                {
                    int workingDays = HelperMethods.GetNumberOfWorkingDays(calcvm.StartPeriod, calcvm.EndPeriod);
                    int totalWorkingdays = (workingDays + Int16.Parse(calcvm.AdditionalDays)) - Int16.Parse(calcvm.DaysOff);
                    double grossExclVAT = (totalWorkingdays * Int16.Parse(calcvm.DayRate));
                    double vat = (grossExclVAT * 0.2);
                    double grossIncVAT = vat + grossExclVAT;
                    double vatrate = double.Parse(calcvm.VATRate) / 100;
                    double totalVAT = grossExclVAT * vatrate;
                    CalcVATResponse vatresp = new CalcVATResponse()
                    {
                        TotalWorkingDay = totalWorkingdays,
                        GrossExcludingVAT = grossExclVAT,
                        GrossIncludingVAT = grossIncVAT,
                        TotalVAT = totalVAT,
                        VATRate = vatrate
                    };
                    return Request.CreateResponse(HttpStatusCode.OK, vatresp);
                }
                return null;
            }
            return new HttpResponseMessage(HttpStatusCode.NoContent);
        }
    }
}
