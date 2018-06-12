using ASA.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Newtonsoft.Json;
using System.Web.Http.Results;
using ASA.API.Model;

namespace ASA.API.Controllers
{
    public class PeriodController : ApiController
    {
        private readonly IPeriodService _periodService; 

        public PeriodController(IPeriodService periodService)
        {
            this._periodService = periodService;
        }

        // GET: api/Period
        [HttpGet]
        public void Get(int businessId)
        {
            // IList<KeyValuePair<string, string>> responseList = null;
            //var responses = _periodService.GetPeriodsByBusinessId(businessId).
            //                        Where(p => p.BusinessId == businessId && p.Status == Core.SubmissionStatus.Accepted).
            //                        Select(h => new ResponseViewModel()
            //                        {
            //                            PeriodId = h.PeriodId,
            //                            PeriodRefId = h.PeriodrefId,
            //                            Status = h.Status,
            //                            ResponseXML = h.HmrcResponses.Select(d => d.ResponseData).FirstOrDefault().ToString()                                
            //                        }).ToList();            
            //if(responses.Count >0)
            //{
            //    var xml = responses.Select(x => x.ResponseXML).FirstOrDefault();
            //    var content = HelperMethods.ExtractBodyContent(xml.ToString());

            //    var successResponse = HelperMethods.Deserialize<SuccessResponse>(content);
            //    foreach (var response in responses)
            //    {
            //        response.Response = successResponse; // as we only have one response so set all responses to same resp
            //    }             
            //    return Request.CreateResponse<IEnumerable<ResponseViewModel>>(responses);
            //}
            //return Request.CreateResponse<IEnumerable<ResponseViewModel>>(responses);
        }

        // GET: api/Period/5
        //public string Get(int id)
        //{
        //    return "value";
        //}

        // POST: api/Period
        public void Post([FromBody]string value)
        {
        }

        // PUT: api/Period/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE: api/Period/5
        public void Delete(int id)
        {
        }
    }
}
