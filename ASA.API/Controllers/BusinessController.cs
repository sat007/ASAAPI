using ASA.API.Model;
using ASA.Core;
using ASA.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Threading.Tasks;


namespace ASA.API.Controllers

//    [HttpPost("channels")]
//public async System.Threading.Tasks.Task Create(Channel channel)
//{
//    await _channelRepository.CreateChannel(channel);
//}
{
    public class BusinessController : ApiController
    {
        private readonly IBusinessService _businessService;        

        public BusinessController(IBusinessService BusinessService)
        {
            this._businessService = BusinessService;
        }
        // GET: api/Business
        [HttpGet]
        public async Task<HttpResponseMessage> Get(string senderId)
        {
            if(!String.IsNullOrWhiteSpace(senderId))
            {
                var Id = Int32.Parse(senderId);
                var businessList = await _businessService.GetBusinesses(Id);
                var bvm = businessList.Select(b => new BusinessViewModel()
                {
                    BusinessName = b.BusinessName,
                    RegisteredDate = b.RegisteredDate,
                    TradingName = b.TradingName,
                    VATRegNo = b.VATRegNo,
                    VATRate = b.VATRate,
                    BusinessId = b.BusinessId,
                    SenderId = b.SenderId,
                    NextQuaterStartDate = b.NextQuaterStartDate,
                    NextQuaterEndDate = b.NextQuaterEndDate,
                    RegNo = b.RegNo,
                    BusinessAddress = new BusinessAdressViewModel()
                    {
                        BusinessAddressId = b.BusinessAddress.BusinessAddressId,
                        Line1 = b.BusinessAddress.Line1,
                        Line2 = b.BusinessAddress.Line2,
                        Line3 = b.BusinessAddress.Line3,
                        Line4 = b.BusinessAddress.Line4,
                        Postcode = b.BusinessAddress.Postcode,
                        Country = b.BusinessAddress.Country,
                    },
                    Periods = b.Periods.Select(p=> new PeriodViewModel()
                    {
                        PeriodId = p.PeriodId, EndPeriod= p.EndPeriod,PeriodRefId = p.PeriodrefId, StartPeriod = p.StartPeriod, Status = p.Status
                    }).ToList()
                });
                if (businessList == null)
                    throw new HttpResponseException(HttpStatusCode.NotFound);
                return Request.CreateResponse<IEnumerable<BusinessViewModel>>(HttpStatusCode.OK, bvm);
            }
            throw new HttpResponseException(HttpStatusCode.ExpectationFailed);
        }

        // GET: api/ Business/5
        [HttpGet]
        public HttpResponseMessage Get(int id)
        {
            var business = _businessService.GetBusinessById(id);
            var bvm = business.Select(b => new BusinessViewModel()
            {
                BusinessName = b.BusinessName,
                RegisteredDate = b.RegisteredDate,
                TradingName = b.TradingName,
                VATRegNo = b.VATRegNo,
                VATRate = b.VATRate,
                BusinessId = b.BusinessId,
                SenderId = b.SenderId,
                NextQuaterEndDate = b.NextQuaterEndDate,
                NextQuaterStartDate =b.NextQuaterStartDate,
                RegNo = b.RegNo,
                BusinessAddress = new BusinessAdressViewModel()
                {
                    BusinessAddressId = b.BusinessAddress.BusinessAddressId,
                    Line1 = b.BusinessAddress.Line1,
                    Line2 = b.BusinessAddress.Line2,
                    Line3 = b.BusinessAddress.Line3,
                    Line4 = b.BusinessAddress.Line4,
                    Postcode = b.BusinessAddress.Postcode,
                    Country = b.BusinessAddress.Country,
                },
                Periods = b.Periods.Select(p => new PeriodViewModel()
                {
                    PeriodId = p.PeriodId,
                    EndPeriod = p.EndPeriod,
                    PeriodRefId = p.PeriodrefId,
                    StartPeriod = p.StartPeriod,
                    Status = p.Status
                }).ToList(),
                Sender = new SenderViewModel()
                {
                    Title = b.Sender.Title,
                    ForName1 = b.Sender.ForName1,
                    ForName2 = b.Sender.ForName2,
                    SurName = b.Sender.SurName,
                    Email = b.Sender.Email,
                    Mobile = b.Sender.Mobile,
                    AddressLine1 = b.Sender.AddressLine1,
                    AddressLine2 = b.Sender.AddressLine2,
                    AddressLine3 = b.Sender.AddressLine3,
                    Postcode = b.Sender.Postcode,
                    Country = b.Sender.Country,
                    SenderPassword = b.Sender.SenderPassword,
                    Telephone = b.Sender.Telephone,
                    Type = b.Sender.Type,
                    HMRCPassword = b.Sender.HMRCPassword,
                    HMRCUserId = b.Sender.HMRCUserId,
                    SenderId = b.SenderId
                }
            }).SingleOrDefault();
            if (business == null)
                throw new HttpResponseException(HttpStatusCode.NotFound);
            return Request.CreateResponse<BusinessViewModel>(HttpStatusCode.OK, bvm);
        }

        // POST: api/Business
        [HttpPost]
        public HttpResponseMessage Post(BusinessViewModel model)
        {
            if(ModelState.IsValid)
            {
                var bvm = new Business()
                {
                    BusinessName = model.BusinessName,
                    TradingName = model.TradingName,
                    VATRegNo = model.VATRegNo,
                    RegisteredDate = model.RegisteredDate,
                    VATRate = model.VATRate,
                    NextQuaterStartDate = model.NextQuaterStartDate,
                    NextQuaterEndDate = model.NextQuaterEndDate,     
                    RegNo = model.RegNo,               
                    BusinessAddress = new BusinessAddress()
                    {
                        Line1 = model.BusinessAddress.Line1,
                        Line2 = model.BusinessAddress.Line2,
                        Line3 = model.BusinessAddress.Line3,
                        Line4 = model.BusinessAddress.Line4,
                        Postcode = model.BusinessAddress.Postcode,
                        Country = model.BusinessAddress.Country
                    },
                    Sender = new Sender()
                    {
                        Title = model.Sender.Title,
                        ForName1 = model.Sender.ForName1,
                        ForName2 = model.Sender.ForName2,
                        SurName = model.Sender.SurName,
                        Email = model.Sender.Email,
                        Mobile = model.Sender.Mobile,
                        AddressLine1 = model.Sender.AddressLine1,
                        AddressLine2 = model.Sender.AddressLine2,
                        AddressLine3 = model.Sender.AddressLine3,
                        Postcode = model.Sender.Postcode,
                        Country = model.Sender.Country,
                        SenderPassword = model.Sender.SenderPassword,
                        Telephone = model.Sender.Telephone,
                        Type = model.Sender.Type,
                        HMRCPassword = model.Sender.HMRCPassword,
                        HMRCUserId =model.Sender.HMRCUserId                        
                    }
                };
                var nextQPeriod = new PeriodData()
                {
                    PeriodrefId = model.Period.PeriodRefId,
                    StartPeriod = model.Period.StartPeriod,
                    EndPeriod = model.Period.EndPeriod,
                    Status = SubmissionStatus.Draft
                };
                bvm.Periods.Add(nextQPeriod);
                var bus =_businessService.Save(bvm);
                if (bus != null)
                {
                    //var msg = new HttpResponseMessage(HttpStatusCode.Created);
                    //msg.Headers.Location = new Uri(Request.RequestUri +"/" + bus.BusinessId.ToString());
                    return Request.CreateResponse(HttpStatusCode.Created, bus.BusinessId.ToString()+","+bus.Sender.SenderId);
                    //return msg;
                }
                throw new HttpResponseException(HttpStatusCode.Conflict);
            }
            return new HttpResponseMessage(HttpStatusCode.ExpectationFailed);
      }
        

        // PUT: api/Business/5
        [HttpPut]
        public HttpResponseMessage Put(int id, [FromBody]BusinessViewModel model)
        {
            if (ModelState.IsValid)
            {
                var bvm = new Business()
                {
                    BusinessId = model.BusinessId,
                    BusinessName = model.BusinessName,
                    TradingName = model.TradingName,
                    VATRegNo = model.VATRegNo,
                    RegisteredDate = model.RegisteredDate,
                    VATRate = model.VATRate,
                    NextQuaterStartDate = model.NextQuaterStartDate,
                    NextQuaterEndDate = model.NextQuaterEndDate,
                    RegNo = model.RegNo,
                    SenderId = model.Sender.SenderId,
                    BusinessAddress = new BusinessAddress()
                    {
                        Line1 = model.BusinessAddress.Line1,
                        Line2 = model.BusinessAddress.Line2,
                        Line3 = model.BusinessAddress.Line3,
                        Line4 = model.BusinessAddress.Line4,
                        Postcode = model.BusinessAddress.Postcode,
                        Country = model.BusinessAddress.Country,
                        BusinessAddressId = model.BusinessAddress.BusinessAddressId                        
                    },
                    Sender = new Sender()
                    {
                        Title = model.Sender.Title,
                        ForName1 = model.Sender.ForName1,
                        ForName2 = model.Sender.ForName2,
                        SurName = model.Sender.SurName,
                        Email = model.Sender.Email,
                        Mobile = model.Sender.Mobile,
                        AddressLine1 = model.Sender.AddressLine1,
                        AddressLine2 = model.Sender.AddressLine2,
                        AddressLine3 = model.Sender.AddressLine3,
                        Postcode = model.Sender.Postcode,
                        Country = model.Sender.Country,
                        SenderPassword = model.Sender.SenderPassword,
                        Telephone = model.Sender.Telephone,
                        Type = model.Sender.Type,
                        HMRCPassword = model.Sender.HMRCPassword,
                        HMRCUserId = model.Sender.HMRCUserId,
                        SenderId = model.Sender.SenderId
                    }
                };
                //var nextQPeriod = new PeriodData()
                //{
                //    PeriodrefId = model.Period.PeriodRefId,
                //    StartPeriod = model.Period.StartPeriod,
                //    EndPeriod = model.Period.EndPeriod,
                //    Status = model.Period.Status
                //};
                //bvm.Periods.Add(nextQPeriod);

                bool isSaved = _businessService.Update(bvm);
                if (isSaved) return new HttpResponseMessage(HttpStatusCode.OK);
                throw new HttpResponseException(HttpStatusCode.Conflict);
            }
            return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
        }

        // DELETE: api/Business/5
        public void Delete(int id)
        {

        }
    }
      
}