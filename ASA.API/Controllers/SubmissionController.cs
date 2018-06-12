using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net;
using System.Web.Http;
using ASA.API.Model;
using Newtonsoft.Json.Linq;
using ASA.Core.Interfaces;
using ASA.Core;
using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;
using SendGrid;
using SendGrid.Helpers.Mail;
using System.Threading.Tasks;
using ASA.API.Models;

namespace ASA.API.Controllers
{
    public class SubmissionController : ApiController
    {
        private readonly IGatewayService _gatewayService;

        public SubmissionController(IGatewayService gatewayService)
        {
            this._gatewayService = gatewayService;
        }

        [HttpGet]
        public IEnumerable<ResponseViewModel> Get(int businessId)
        {
            string businessIdStr = businessId.ToString();
            if (!String.IsNullOrWhiteSpace(businessIdStr))
            {
                var periodData = _gatewayService.GetSubmissions(Int16.Parse(businessIdStr));
                if (periodData.Count > 0)
                {
                    var responses = periodData.Select(p => new ResponseViewModel(p.HmrcResponses.FirstOrDefault())
                    {
                        PeriodId = p.PeriodId,
                        PeriodRefId = p.PeriodrefId,
                        Status = p.Status                                                
                    }).Where(pd=>pd.Status == SubmissionStatus.Accepted).ToList();
                    return responses;  
                }
                else
                    throw new HttpResponseException(HttpStatusCode.NoContent);
               
            }
            throw new HttpResponseException(HttpStatusCode.ExpectationFailed);
        }        

        // [CheckModelForNull] //doest seems to be working as exp
        [HttpPost]
        public HttpResponseMessage Post(SubmissionViewModel model)
        {
            if (ModelState.IsValid) //always returning true 
                {
                string testInLive = model.RunMode;
                bool mode = testInLive.Equals("1") ? true : false;

                var sender = new Sender
                    {
                        Title = model.BussinessViewModel.Sender.Title,
                        ForName1 = model.BussinessViewModel.Sender.ForName1,
                        ForName2 = model.BussinessViewModel.Sender.ForName2,
                        SurName = model.BussinessViewModel.Sender.SurName,
                        Email = model.BussinessViewModel.Sender.Email,
                        AddressLine1 = model.BussinessViewModel.Sender.AddressLine1,
                        AddressLine2 = model.BussinessViewModel.Sender.AddressLine2,
                        AddressLine3 = model.BussinessViewModel.Sender.AddressLine3,
                        Postcode = model.BussinessViewModel.Sender.Postcode,
                        SenderPassword = model.BussinessViewModel.Sender.SenderPassword,
                        Type = model.BussinessViewModel.Sender.Type,
                        Mobile = model.BussinessViewModel.Sender.Mobile,
                        Telephone = model.BussinessViewModel.Sender.Telephone,
                        HMRCPassword = model.BussinessViewModel.Sender.HMRCPassword,
                        HMRCUserId = model.BussinessViewModel.Sender.HMRCUserId,
                        Country = model.BussinessViewModel.Sender.Country,
                        SenderId = model.BussinessViewModel.Sender.SenderId
                };

                    var address = new BusinessAddress
                    {
                        Line1 = model.BussinessViewModel.BusinessAddress.Line1,
                        Line2 = model.BussinessViewModel.BusinessAddress.Line2,
                        Line3 = model.BussinessViewModel.BusinessAddress.Line3,
                        Line4 = model.BussinessViewModel.BusinessAddress.Line4,
                        Country = model.BussinessViewModel.BusinessAddress.Country,
                        Postcode = model.BussinessViewModel.BusinessAddress.Postcode,
                        BusinessAddressId = model.BussinessViewModel.BusinessAddress.BusinessAddressId
                    };
                    var business = new Business
                    {
                        BusinessName = model.BussinessViewModel.BusinessName,                        
                        RegisteredDate = model.BussinessViewModel.RegisteredDate,
                        VATRegNo = model.BussinessViewModel.VATRegNo,
                        TradingName = model.BussinessViewModel.TradingName, 
                        BusinessId = model.BussinessViewModel.BusinessId, 
                        RegNo = model.BussinessViewModel.RegNo,
                        VATRate = model.BussinessViewModel.VATRate,
                        SenderId = model.BussinessViewModel.SenderId                                                
                    };

                    var perioddata = new PeriodData
                    {
                        PeriodrefId = model.PeriodViewModel.PeriodRefId,                        
                        StartPeriod = model.PeriodViewModel.StartPeriod,
                        EndPeriod = model.PeriodViewModel.EndPeriod,
                        PeriodId = model.PeriodViewModel.PeriodId,
                        BusinessId = model.BussinessViewModel.BusinessId
                    };
                    var vatData = new VAT100()
                    {
                        Box1 = model.VAT100ViewModel.VATDueOnOutputs.ToString("N"),
                        Box2 = model.VAT100ViewModel.VATDueOnECAcquisitions.ToString("N"),
                        Box3 = model.VAT100ViewModel.TotalVAT.ToString("N"),
                        Box4 = model.VAT100ViewModel.VATReclaimedOnInputs.ToString("N"),
                        Box5 = model.VAT100ViewModel.NetVAT.ToString("N"),
                        Box6 = model.VAT100ViewModel.NetSalesAndOutputs.ToString(),
                        Box7 = model.VAT100ViewModel.NetPurchasesAndInputs.ToString(),
                        Box8 = model.VAT100ViewModel.NetECSupplies.ToString(),
                        Box9 = model.VAT100ViewModel.NetECAcquisitions.ToString(),
                        LastUpdated = model.VAT100ViewModel.LastUpdated
                    };
                     
                    SubmissionManager submissionmanager = new SubmissionManager(_gatewayService);

                    //create govtalk message 
                    var submissionResponse = submissionmanager.SendSubmissionRequest(sender, business, perioddata, vatData, mode);
                    //read correlationId and poll uri from response
                    //TODO: every time it fails delete the request and resubmit on sucess capt response and send back to client 

                    //  List<Object> responses = new List<Object>();
                    Dictionary<string, List<XElement>> dicresponses = new Dictionary<string, List<XElement>>();
                    if (submissionResponse.Errors.Count == 0)
                    {
                        //submission passes send poll request 
                        string correlationId = submissionResponse.CorrelationId.ToString();
                        string polluri = submissionResponse.FollowOnUri.ToString();
                        if (!(String.IsNullOrEmpty(correlationId) && String.IsNullOrEmpty(polluri)))
                        {
                            var pollResponse = submissionmanager.SendPollRequest(correlationId, polluri, mode);
                            if (pollResponse.Errors.Count == 0)
                            {
                            //success get success response 
                            perioddata.Status = SubmissionStatus.Accepted; //update status as accepted by HMRC and VAT info in the vat table 
                            perioddata.VAT100 = vatData; // attaching new vat data to update 
                           
                            //perioddata.HmrcResponses.Add(pollResponse);
                            
                            var nextQuaterStartDate = submissionmanager.AddMonthsCustom(perioddata.StartPeriod, 3);
                            var nextQuaterEndDate = submissionmanager.AddMonthsCustom(perioddata.EndPeriod, 3);

                            var nextPeriod = new PeriodData()
                            {
                                StartPeriod = nextQuaterStartDate,
                                EndPeriod = nextQuaterEndDate,
                                BusinessId = business.BusinessId,
                                PeriodrefId = nextQuaterEndDate.ToString("yyyy-MM"),
                                Status = SubmissionStatus.Draft
                            };
                            business.BusinessAddress = address;
                            business.Sender = sender;
                            business.NextQuaterStartDate = nextQuaterStartDate; //update bussiness model props with next quater start and end dates 
                            business.NextQuaterEndDate = nextQuaterEndDate;
                            
                            perioddata.HmrcResponses.Add(pollResponse);
                            business.Periods.Add(perioddata);
                            business.Periods.Add(nextPeriod);
                            _gatewayService.UpdateEntities(business);
                            //  _gatewayService.Save(pollResponse);                             
                            var strContent = HelperMethods.ExtractBodyContent(pollResponse.ResponseData.ToString());
                                var successResponse = HelperMethods.Deserialize<SuccessResponse>(strContent);
                            //save the response in the DB 
                            //_gatewayService.AddNextQuaterPeriod(nextPeriod); //Add next VAT quater in the database 

                            //business.Periods.Add(nextPeriod);
                            //_gatewayService.UpdateNextQuaterInBVM(business);
                           // _gatewayService.SaveResponse(successResponse); // commit to DB after adding all above objects to repositories 
                                //if (successResponse != null)
                                //{
                                //TODO: serialise sucessresponse, sucessresponseirmark, message type
                                var str = HelperMethods.Serialize(successResponse);
                                var xdoc = XDocument.Parse(str, LoadOptions.PreserveWhitespace);
                                ////select only body element from xdoc 

                                var hmrcresponsemsgnode = (from d in xdoc.Descendants()
                                                           where d.Name.LocalName == "Message"
                                                           select d).ToList();
                                var vatperiodnode = (from m in xdoc.Descendants()
                                                     where m.Name.LocalName == "VATPeriod"
                                                     select m).ToList();
                                var paymentBodyNode = (from p in xdoc.Descendants()
                                                       where p.Name.LocalName == "Body"
                                                       select p).ToList();
                                var errorsNode = (from p in xdoc.Descendants()
                                                  where p.Name.LocalName == "Error"
                                                  select p).ToList();

                                dicresponses.Add("hmrcResponse", hmrcresponsemsgnode);
                                dicresponses.Add("vatPeriod", vatperiodnode);
                                dicresponses.Add("paymentDetails", paymentBodyNode);
                                dicresponses.Add("Errors", errorsNode);
                                //responses.Add(successResponse);
                                return Request.CreateResponse<Dictionary<string, List<XElement>>>(HttpStatusCode.OK, dicresponses);
                            }
                            else
                            {
                                //failure bus valid failed handle here 
                                //delete the request and resubmit as new request as mentioned in the gov site
                                submissionmanager.SendDeleteRequest(correlationId, mode, polluri);
                                var temp = XDocument.Parse(pollResponse.ResponseData);
                                var pollerrors = (from p in temp.Descendants()
                                                  where p.Name.LocalName == "Error"
                                                  select p).ToList();

                                dicresponses.Add("Errors", pollerrors);
                                // responses.Add(pollResponse);
                            }

                        }
                        else
                        {
                            //id or uri is null
                            // responses.Add(submissionResponse);
                            var temp = XDocument.Parse(submissionResponse.ResponseData);
                            var submissionerrors = (from p in temp.Descendants()
                                                    where p.Name.LocalName == "Error"
                                                    select p).ToList();

                            dicresponses.Add("Errors", submissionerrors);

                            return Request.CreateResponse<Dictionary<string, List<XElement>>>(HttpStatusCode.BadRequest, dicresponses);

                        }

                    }
                    else
                    {
                        //return submission failure response 
                        // responses.Add(submissionResponse);
                        var temp = XDocument.Parse(submissionResponse.ResponseData);
                        var submissionerrors = (from p in temp.Descendants()
                                                where p.Name.LocalName == "Error"
                                                select p).ToList();

                        dicresponses.Add("Errors", submissionerrors);




                    }
                    return Request.CreateResponse<Dictionary<string, List<XElement>>>(HttpStatusCode.OK, dicresponses);

                }
            return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);

            //}
            //throw new HttpResponseException(new HttpResponseMessage { StatusCode = HttpStatusCode.BadRequest, ReasonPhrase = "data is null" });

        }

        //public HttpResponseMessage Validate([FromBody]JObject data)
        //{
        //    if (data != null)
        //    {
        //        SenderViewModel senvm = data["SenderViewModel"].ToObject<SenderViewModel>();
        //        BusinessViewModel busvm = data["BusinessViewModel"].ToObject<BusinessViewModel>();
        //        string isLiveRun = data["Runmode"].ToString();
        //        bool mode = isLiveRun.Equals("1") ? false : true;
        //        if (ModelState.IsValid) //always returning true 
        //        {
        //            var sender = new Sender
        //            {
        //                Title = senvm.Title,
        //                ForName1 = senvm.ForName1,
        //                ForName2 = senvm.ForName2,
        //                SurName = senvm.SurName,
        //                Email = senvm.Email,
        //                AddressLine1 = senvm.AddressLine1,
        //                AddressLine2 = senvm.AddressLine2,
        //                AddressLine3 = senvm.AddressLine3,
        //                Postcode = senvm.Postcode,
        //                SenderId = senvm.SenderId,
        //                SenderPassword = senvm.SenderPassword,
        //                Type = senvm.Type,
        //                Mobile = senvm.Mobile,
        //                Telephone = senvm.Telephone
        //            };
        //            var business = new Business
        //            {
        //                BusinessName = busvm.BusinessName,
        //                //AddressDetails = busvm.AddressDetails,
        //                RegisteredDate = busvm.RegisteredDate,
        //                VATRegNo = busvm.VATRegNo,
        //                TradingName = busvm.TradingName
        //            };

        //            SubmissionManager service = new SubmissionManager(_gatewayService);

        //            //create govtalk message 
        //            var submissionResponse = service.CreateGovTalkMessageForAuthCheck(sender, business, mode);
        //            if (submissionResponse.Errors.Count == 0)
        //            {
        //                //sucess
        //            }
        //            else
        //            {
        //                //fail
        //            }

        //    }

    //    public async Task<bool> SendEmailConfirmation(Dictionary<string, List<XElement>> data, string email)
    //    {
    //        if ((data!=null) && (data.Count>0))
    //        {
    //            //var resp = XElement.Load(data.hmrcResponse.ToString());
    //            var apiKey = "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";
    //            //var apiKey = Environment.GetEnvironmentVariable("ASA");
    //            var client = new SendGridClient(apiKey);
    //            var from = new EmailAddress("no reply", "HMRC Response - VAT Submission");
    //            var subject = "HMRC Response ";
    //            var to = new EmailAddress(email, "Response");
    //            // var cc = new EmailAddress("satwikchoudary@gmail.com", "support");
    //            var plainTextContent="";
    //            var htmlContent="";
    //            foreach (var item in data)
    //            {
    //                plainTextContent = item.Value.ToString();
    //                htmlContent = item.Value.ToString();
    //            }

    //            //var plainTextContent = body;
    //            //var htmlContent = body;
    //            List<EmailAddress> emails = new List<EmailAddress>();
    //            emails.Add(new EmailAddress(email));
    //            //emails.Add(new EmailAddress("satwikchoudary@gmail.com"));
    //            var msg = MailHelper.CreateSingleEmail(from, to, subject, plainTextContent, htmlContent);
    //            //var msg = MailHelper.CreateSingleEmailToMultipleRecipients(from, emails, subject, plainTextContent, htmlContent);
    //            var response = await client.SendEmailAsnc(msg);
    //            return true;
         

    //        }
    //        return false;
    //    }
    }
}
