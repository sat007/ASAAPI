using ASA.API.Model;
using ASA.Core;
using ASA.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace ASA.API.Controllers
{
    public class SenderController : ApiController
    {
        private readonly ISenderService _senderService;
        public SenderController(ISenderService SenderService)
        {
            this._senderService = SenderService;
        }
        [HttpGet]
        public HttpResponseMessage Get()
        {
            var sendersList = _senderService.GetSenders().ToList();

            if (sendersList.Count > 0)
            {
                var senvm = sendersList.Select(sen => new SenderViewModel()
                {
                    SenderId = sen.SenderId,
                    ForName1 = sen.ForName1,
                    ForName2 = sen.ForName2,
                    AddressLine1 = sen.AddressLine1,
                    AddressLine2 = sen.AddressLine2,
                    AddressLine3 = sen.AddressLine3,
                    SurName = sen.SurName,
                    Title = sen.Title,
                    Country = sen.Country,
                    Email = sen.Email,
                    Mobile = sen.Mobile,
                    Postcode = sen.Postcode,
                    SenderPassword = sen.SenderPassword,
                    Telephone = sen.Telephone,
                    HMRCUserId = sen.HMRCUserId,
                    HMRCPassword = sen.HMRCPassword,
                    Type = sen.Type
                });
                return Request.CreateResponse<IEnumerable<SenderViewModel>>(HttpStatusCode.OK, senvm);
            }
            //return Request.CreateResponse<IEnumerable<SenderViewModel>>(HttpStatusCode.OK, senvm);
            if (sendersList == null || sendersList.Count == 0)
                throw new HttpResponseException(HttpStatusCode.NotFound);

            return Request.CreateResponse(HttpStatusCode.NoContent, "empty");
        }

        [HttpPost]
        public HttpResponseMessage Post([FromBody]SenderViewModel model)
        {
            if (ModelState.IsValid)
            {
                var sen = new Sender()
                {
                    //SenderId = model.SenderId,
                    ForName1 = model.ForName1,
                    ForName2 = model.ForName2,
                    AddressLine1 = model.AddressLine1,
                    AddressLine2 = model.AddressLine2,
                    AddressLine3 = model.AddressLine3,
                    Country = model.Country,
                    Email = model.Email,
                    Mobile = model.Mobile,
                    HMRCPassword = model.HMRCPassword,
                    HMRCUserId = model.HMRCUserId,
                    Postcode = model.Postcode,
                    SenderPassword = model.SenderPassword,
                    SurName = model.SurName,
                    Telephone = model.Telephone,
                    Title = model.Title,
                    Type = model.Type
                };
                var sucess = _senderService.Save(sen);
                if (sucess)
                {
                    var msg = new HttpResponseMessage(HttpStatusCode.Created);
                    return msg;
                }
                else
                {
                    throw new HttpResponseException(HttpStatusCode.Conflict);
                }

            }
            else
            {
                throw new HttpResponseException(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.BadRequest,
                    ReasonPhrase = "Model validation failed"
                });
            }
        }

        public HttpResponseMessage Put(int id, [FromBody]SenderViewModel model)
        {
            if (ModelState.IsValid)
            {
                var sender = new Sender()
                {
                    SenderId = model.SenderId,
                    ForName1 = model.ForName1,
                    ForName2 = model.ForName2,
                    AddressLine1 = model.AddressLine1,
                    AddressLine2 = model.AddressLine2,
                    AddressLine3 = model.AddressLine3,
                    Country = model.Country,
                    Email = model.Email,
                    Mobile = model.Mobile,
                    HMRCPassword = model.HMRCPassword,
                    HMRCUserId = model.HMRCUserId,
                    Postcode = model.Postcode,
                    SenderPassword = model.SenderPassword,
                    SurName = model.SurName,
                    Telephone = model.Telephone,
                    Title = model.Title,
                    Type = model.Type
                };
                bool status = _senderService.Update(sender);
                if (status) return new HttpResponseMessage(HttpStatusCode.OK);
                throw new HttpResponseException(HttpStatusCode.NotFound);
            }
            return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
        }

    }
}
