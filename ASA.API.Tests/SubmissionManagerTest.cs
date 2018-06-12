using ASA.Core;
using ASA.Core.Interfaces;
using ASA.Core.Services;
using FakeItEasy;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace ASA.API.Tests
{
    [TestClass]
        public class SubmissionManagerTest
        {
            private GatewayService _gatewayService;
       

        [TestInitialize]
            public void Initilize()
            {
                _gatewayService = A.Fake<GatewayService>();
                //TODO: should validate produced xml against scheme before submitting to process 
                //  XmlSchemaSet schemaSet = new XmlSchemaSet();
                //schemaSet.Add("http://www.govtalk.gov.uk/CM/envelope", "envelope-v2-0-HMRC.xsd");

            }
        [TestMethod]
        public void VerifyAppDomainHasConfigurationSettings()
        {
            string value = ConfigurationManager.AppSettings["ChannelProduct"];
            Assert.IsFalse(String.IsNullOrEmpty(value), "No App.Config found.");
        }
        [TestMethod]
            public void Submission_SendRequest()
            {
                //Arrange 
                //Fake input params 
                var sen = new Sender
                {
                    Title = "Mr",
                    ForName1 = "Sat",
                    ForName2="aa",
                    SurName = "Gat",
                    AddressLine1 = "133Gleny",
                    AddressLine2 = "Barking",
                    Postcode = "Ig11",
                    Country = "UK",
                    Email = "sat@test.com",
                    Mobile = "0123456789",
                    HMRCUserId = "VATDEC180a01",
                    SenderPassword = "testing1"

                };
                var bus = new Business
                {
                    BusinessName = "ABC",
                    RegisteredDate = DateTime.Parse("07-04-2017"),
                    TradingName = "ABC",
                    VATRegNo = "999900001",

                };
                var add = new BusinessAddress
                {
                    Line1 = "1 First Add",
                    Line2 = "2 Second add",
                    Line3 = "3 Third",
                    Postcode = "Ig11",
                    Country = "UK"
                };
              //  bus.AddressList.Add(add);
                var per = new PeriodData
                {
                    PeriodrefId = "201701",
                    StartPeriod = DateTime.Parse("07-04-2017"),
                    EndPeriod = DateTime.Parse("31-07-2017")

                };
                var vat = new VAT100
                {
                    Box1 = "1.50",
                    Box2 = "0.50",
                    Box3 = "2.00",
                    Box4 = "2.00",
                    Box5 = "0.00",
                    Box6 = "20",
                    Box7 = "10",
                    Box8 = "10",
                    Box9= "5"
                };


                //Arrange
                //var sm = new SubmissionManager(_gatewayService);
          //  var test = sm.SendSubmissionRequest(sen, bus, per, vat, true);
            //Act
            //var govtalkmessage = _gatewayService.CreateGovTalkMessage(sen, bus, per, vat, true);

            //    var strGovTalkMessage = HelperMethods.GovTalkMessageToXDocument(govtalkmessage).ToString();


            ////generate irmark from govtalk message 
            //byte[] bytes = Encoding.UTF8.GetBytes(strGovTalkMessage);

            //    var irmark = HelperMethods.GetIRMark(bytes);

            //    var xdoc = XDocument.Parse(strGovTalkMessage, LoadOptions.PreserveWhitespace); //set irmark value in the doc

            //    if (irmark!=null)
            //    {
            //        XNamespace ns = xdoc.Root.Name.Namespace;
            //        XElement body = xdoc.Root.Element(ns + "Body");
            //        if (body != null)
            //        {
            //            var bodyElelemts = from el in body.Descendants()
            //                               where el.Name.LocalName == "IRenvelope"
            //                               select el;

            //            foreach (var ele in bodyElelemts)
            //            {
            //                XNamespace ns1 = ele.Name.Namespace;
            //                var element = ele.Descendants(ns1 + "IRmark").First();
            //                if (element != null)
            //                {
            //                    element.Value = irmark;
            //                }

            //            }
            //        }
            //    }
            //    string strSendRequest = xdoc.ToString();

            //TODO: Need to validate the xml before sending to gateway but this seems to be complicate yet as
            //this needs to be validate individual classes irenvelope and vat payload and govtlak envelope so for future 
            //http://stackoverflow.com/questions/751511/validating-an-xml-against-referenced-xsd-in-c-sharp

            // bool result = HelperMethods.IsValidXml(strSendRequest, @"C:\Temp\ASA.API\ASA.Core\XSDs\envelope-v2-0-HMRC.xsd", "http://www.govtalk.gov.uk/CM/envelope");
            //var submissionResponse = sm.SendSubmissionRequest(sen, bus, per, vat, true);

            //var erorrs = submissionResponse.Errors.Count;
            string correlationId = "585F0728964645B2A944B4410D0EF5A8";
            string polluri = "https://secure.dev.gateway.gov.uk/poll";
            var govTalkMessageForPoll =
                   _gatewayService.CreateGovTalkMessageForPollRequest(correlationId, polluri, true);
            var pollreq = HelperMethods.GoVTalkMessageToXDocumentForPoll(govTalkMessageForPoll);
            var pollStr = pollreq.ToString();
            var pollresponse = _gatewayService.SendHMRCMessage(pollStr, polluri);

            var strContent = HelperMethods.ExtractBodyContent(pollresponse.ResponseData.ToString());
            var successResponse = HelperMethods.Deserialize<SuccessResponse>(strContent);

            var str = HelperMethods.Serialize(successResponse);
            var doc = XDocument.Parse(str, LoadOptions.PreserveWhitespace);
            ////select only body element from xdoc 
            XNamespace nsr = "http://www.govtalk.gov.uk/taxation/vat/vatdeclaration/2";
            var hmrcresponsemsgnode = (from d in doc.Descendants()
                                       where d.Name.LocalName == "Message"
                                       select d).ToList();
            var messagedetailsnode = (from m in doc.Descendants()
                                      where m.Name.LocalName == "AcceptedTime"
                                      select m).ToList();
            var paymentBodyNode = (from p in doc.Descendants()
                                   where p.Name.LocalName ==  "Body"
                                   select p).ToList();


            List<XElement> col = new List<XElement>();
            col.AddRange(hmrcresponsemsgnode);
            col.AddRange(messagedetailsnode);
            col.AddRange(paymentBodyNode);
            Dictionary<string, List<XElement>> dic = new Dictionary<string, List<XElement>>();
            dic.Add("hmrcResponseNode", hmrcresponsemsgnode);
            dic.Add("password", messagedetailsnode);
            dic.Add("plan_id", paymentBodyNode);

            //Asset 
            // Assert.IsInstanceOfType(govtalkmessage, typeof(GovTalkMessage)); //check if the object is govTalkMessage 
            //Assert.IsTrue(result); //validate generated xml 
            // Assert.AreEqual(erorrs, 0);

        }
        }
    }
