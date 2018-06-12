using System;
using System.Configuration;
using System.IO;
using System.Net;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;
using ASA.Core.Interfaces;
using System.Linq;
using System.Collections.Generic;
using ASA.Core.Repositories;
using ASA.Core.Infrastructure;

namespace ASA.Core.Services
{
    public class GatewayService : IGatewayService
    {
        private readonly IHMRCResponseRepository _hmrcResponseRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPeriodDataRepository _periodRepository;
        private readonly IBusinessRepository _businessrepository;
        //private readonly ISuccessResponseRepository _successRepository;
        public GatewayService(IHMRCResponseRepository hmrcResponseRepository, IUnitOfWork unitOfWork, IPeriodDataRepository periodRepository, IBusinessRepository businessRepository)
        {
            this._hmrcResponseRepository = hmrcResponseRepository;
            this._unitOfWork = unitOfWork;
            this._periodRepository = periodRepository;
            this._businessrepository = businessRepository;
            //this._successRepository = successRepository;
        }
        public IList<PeriodData> GetSubmissions(int businessId)
        {
            return _periodRepository.GetAllIncluding(h => h.HmrcResponses, v => v.VAT100).Where(p => p.BusinessId == businessId).ToList();            
        }
        public HMRCResponse Save(HMRCResponse response)
        {
            var res = _hmrcResponseRepository.Add(response);
            SaveCommit();
            return res;
        }
        public void UpdatePeriodStatus(PeriodData period)
        {
            _periodRepository.Update(period);            
        }
        public void AddPeriodData(PeriodData data)
        {
            _periodRepository.Add(data);
        }

        public void AddNextQuaterPeriod(PeriodData period)
        {
            _periodRepository.Add(period);
           // SaveCommit();
        }
        public void UpdateEntities(Business entity)
        {
            _businessrepository.Update(entity);            
        }

        //public void UpdateNextQuaterInBVM(Business business)
        //{
        //    _businessrepository.Update(business);
        //    //SaveCommit();
        //}
        private void SaveCommit()
        {
            _unitOfWork.Commit();
        }
        public GovTalkMessage CreateGovTalkMessage(Sender sender, Business business, PeriodData periodData, VAT100 vatData, bool testInLive)
        {
            string classType = !testInLive ? "HMRC-VAT-DEC" : "HMRC-VAT-DEC-TIL";
            string gatewayTest = !testInLive ? "0" : "1";

            //Gov Talk message 
            GovTalkMessage govTalkMessage = new GovTalkMessage();
            //Gov Talk header 
            GovTalkMessageHeader header = new GovTalkMessageHeader();
            //sender details 
            GovTalkMessageHeaderSenderDetails senderDetails = new GovTalkMessageHeaderSenderDetails();
            GovTalkMessageHeaderSenderDetailsIDAuthentication idauth = new GovTalkMessageHeaderSenderDetailsIDAuthentication();
            idauth.SenderID = sender.HMRCUserId;
            GovTalkMessageHeaderSenderDetailsIDAuthenticationAuthentication aauth = new GovTalkMessageHeaderSenderDetailsIDAuthenticationAuthentication();
            aauth.Method = GovTalkMessageHeaderSenderDetailsIDAuthenticationAuthenticationMethod.clear;
            aauth.Role = "principal";
            aauth.Item = sender.HMRCPassword;
            GovTalkMessageHeaderSenderDetailsIDAuthenticationAuthentication[] authentications = new GovTalkMessageHeaderSenderDetailsIDAuthenticationAuthentication[1];
            authentications[0] = aauth;
            idauth.Authentication = authentications;
            senderDetails.IDAuthentication = idauth;
            senderDetails.EmailAddress = sender.Email;
            //message details 
            GovTalkMessageHeaderMessageDetails messagedetails = new GovTalkMessageHeaderMessageDetails();
            messagedetails.Class = classType;
            messagedetails.Qualifier = GovTalkMessageHeaderMessageDetailsQualifier.request;
            messagedetails.Function = GovTalkMessageHeaderMessageDetailsFunction.submit;
            messagedetails.FunctionSpecified = true;
            messagedetails.CorrelationID = "";
            messagedetails.Transformation = GovTalkMessageHeaderMessageDetailsTransformation.XML;
            messagedetails.GatewayTest = gatewayTest;
            messagedetails.TransformationSpecified = true;
            header.MessageDetails = messagedetails;
            header.SenderDetails = senderDetails;
            //Gov Talk details 
            GovTalkMessageGovTalkDetails govTalkDetails = new GovTalkMessageGovTalkDetails();
            //keys 
            GovTalkMessageGovTalkDetailsKey key = new GovTalkMessageGovTalkDetailsKey();
            key.Type = ConfigurationManager.AppSettings["GovTalkDetailsKeyType"];
            key.Value = business.VATRegNo.ToString();
            GovTalkMessageGovTalkDetailsKey[] keys = new GovTalkMessageGovTalkDetailsKey[1];
            keys[0] = key;
            //Channel routing 
            GovTalkMessageGovTalkDetailsChannelRoutingChannel channel = new GovTalkMessageGovTalkDetailsChannelRoutingChannel();
            channel.Version = ConfigurationManager.AppSettings["ChannelVersion"];
            channel.Product = ConfigurationManager.AppSettings["ChannelProduct"];
            channel.ItemElementName = ItemChoiceType.URI;
            channel.Item = ConfigurationManager.AppSettings["ChannelUri"];
            GovTalkMessageGovTalkDetailsChannelRouting channelRouting = new GovTalkMessageGovTalkDetailsChannelRouting();
            channelRouting.Channel = channel;
            GovTalkMessageGovTalkDetailsChannelRouting[] channelRoutings = new GovTalkMessageGovTalkDetailsChannelRouting[1];
            channelRoutings[0] = channelRouting;
            govTalkDetails.Keys = keys;
            govTalkDetails.ChannelRouting = channelRoutings;

            //Gov Talk Body 
            GovTalkMessageBody body = new GovTalkMessageBody();
            //populate body content with ir68 data 
            //IR68 ir68 = new IR68(periodData, vatData);         //commented out to use method instead of object instantiation with period/vat data 
            //IRenvelope iRenvelope = ir68.CreateIREnvelopeBody(sender, business);

            IRenvelope iRenvelope = CreateIRenvelopeBody(sender, business, periodData, vatData);
            //serialse ir object data into xml elements and add to Gov Talk body 
            //XElement xe = XElement.Parse(SerializeIrEnvelope(iRenvelope));
            XElement xe = XElement.Parse(IREnvelopeToXDocument(iRenvelope).ToString());
            XmlElement xelement = ToXmlElement(xe);
            XmlElement[] xmlElements = new XmlElement[1];
            xmlElements[0] = xelement;
            body.Any = xmlElements;
            govTalkMessage.EnvelopeVersion = ConfigurationManager.AppSettings["GovTalkMessageEnvelopeVersion"];
            govTalkMessage.Header = header;
            govTalkMessage.GovTalkDetails = govTalkDetails;
            govTalkMessage.Body = body;
            return govTalkMessage;

        }
        //CreateGovTalkMessageForAuthCheck
        public GovTalkMessage CreateGovTalkMessageForAuthCheck(Sender sender, Business business, bool testInLive)
        {
            string classType = !testInLive ? "HMRC-VAT-DEC" : "HMRC-VAT-DEC-TIL";
            string gatewayTest = !testInLive ? "0" : "1";

            //Gov Talk message 
            GovTalkMessage govTalkMessage = new GovTalkMessage();
            //Gov Talk header 
            GovTalkMessageHeader header = new GovTalkMessageHeader();
            //sender details 
            GovTalkMessageHeaderSenderDetails senderDetails = new GovTalkMessageHeaderSenderDetails();
            GovTalkMessageHeaderSenderDetailsIDAuthentication idauth = new GovTalkMessageHeaderSenderDetailsIDAuthentication();
            idauth.SenderID = sender.HMRCUserId;
            GovTalkMessageHeaderSenderDetailsIDAuthenticationAuthentication aauth = new GovTalkMessageHeaderSenderDetailsIDAuthenticationAuthentication();
            aauth.Method = GovTalkMessageHeaderSenderDetailsIDAuthenticationAuthenticationMethod.clear;
            aauth.Role = "principal";
            aauth.Item = sender.HMRCPassword;
            GovTalkMessageHeaderSenderDetailsIDAuthenticationAuthentication[] authentications = new GovTalkMessageHeaderSenderDetailsIDAuthenticationAuthentication[1];
            authentications[0] = aauth;
            idauth.Authentication = authentications;
            senderDetails.IDAuthentication = idauth;
            senderDetails.EmailAddress = sender.Email;
            //message details 
            GovTalkMessageHeaderMessageDetails messagedetails = new GovTalkMessageHeaderMessageDetails();
            messagedetails.Class = classType;
            messagedetails.Qualifier = GovTalkMessageHeaderMessageDetailsQualifier.request;
            messagedetails.Function = GovTalkMessageHeaderMessageDetailsFunction.submit;
            messagedetails.FunctionSpecified = true;
            messagedetails.CorrelationID = "";
            messagedetails.Transformation = GovTalkMessageHeaderMessageDetailsTransformation.XML;
            messagedetails.GatewayTest = gatewayTest;
            messagedetails.TransformationSpecified = true;
            header.MessageDetails = messagedetails;
            header.SenderDetails = senderDetails;
            //Gov Talk details 
            GovTalkMessageGovTalkDetails govTalkDetails = new GovTalkMessageGovTalkDetails();
            //keys 
            GovTalkMessageGovTalkDetailsKey key = new GovTalkMessageGovTalkDetailsKey();
            key.Type = ConfigurationManager.AppSettings["GovTalkDetailsKeyType"];
            key.Value = ConfigurationManager.AppSettings["GovTalkDetailsKey"];
            GovTalkMessageGovTalkDetailsKey[] keys = new GovTalkMessageGovTalkDetailsKey[1];
            keys[0] = key;
            //Channel routing 
            GovTalkMessageGovTalkDetailsChannelRoutingChannel channel = new GovTalkMessageGovTalkDetailsChannelRoutingChannel();
            channel.Version = ConfigurationManager.AppSettings["ChannelVersion"];
            channel.Product = ConfigurationManager.AppSettings["ChannelProduct"];
            channel.ItemElementName = ItemChoiceType.URI;
            channel.Item = ConfigurationManager.AppSettings["ChannelUri"];
            GovTalkMessageGovTalkDetailsChannelRouting channelRouting = new GovTalkMessageGovTalkDetailsChannelRouting();
            channelRouting.Channel = channel;
            GovTalkMessageGovTalkDetailsChannelRouting[] channelRoutings = new GovTalkMessageGovTalkDetailsChannelRouting[1];
            channelRoutings[0] = channelRouting;
            govTalkDetails.Keys = keys;
            govTalkDetails.ChannelRouting = channelRoutings;

            //Gov Talk Body 
            GovTalkMessageBody body = new GovTalkMessageBody();
            //populate body content with ir68 data 
            //IR68 ir68 = new IR68(periodData, vatData);         //commented out to use method instead of object instantiation with period/vat data 
            //IRenvelope iRenvelope = ir68.CreateIREnvelopeBody(sender, business);

           // IRenvelope iRenvelope = CreateIRenvelopeBody(sender, business, periodData, vatData);
            //serialse ir object data into xml elements and add to Gov Talk body 
            //XElement xe = XElement.Parse(SerializeIrEnvelope(iRenvelope));
            //XElement xe = XElement.Parse(IREnvelopeToXDocument(iRenvelope).ToString());
            //XmlElement xelement = ToXmlElement(xe);
            //XmlElement[] xmlElements = new XmlElement[1];
            //xmlElements[0] = xelement;
            //body.Any = xmlElements;
            govTalkMessage.EnvelopeVersion = ConfigurationManager.AppSettings["GovTalkMessageEnvelopeVersion"];
            govTalkMessage.Header = header;
            govTalkMessage.GovTalkDetails = govTalkDetails;
            govTalkMessage.Body = body;
            return govTalkMessage;

        }
        public GovTalkMessage CreateGovTalkMessageForPollRequest(string correlationId, string polluri, bool testInLive)
        {
            //define classtype 
            string classType = !testInLive ? "HMRC-VAT-DEC" : "HMRC-VAT-DEC-TIL";

            if (correlationId != "" && polluri.Trim() != "")
            {
                GovTalkMessageHeader header = new GovTalkMessageHeader();
                GovTalkMessageHeaderSenderDetails senderDeatils = new GovTalkMessageHeaderSenderDetails();
                GovTalkMessageHeaderMessageDetailsResponseEndPoint endpoint = new GovTalkMessageHeaderMessageDetailsResponseEndPoint();
                endpoint.PollInterval = "10";
                endpoint.Value = polluri;
                GovTalkMessageHeaderMessageDetails messageDetails = new GovTalkMessageHeaderMessageDetails();
                messageDetails.Class = classType;
                messageDetails.Qualifier = GovTalkMessageHeaderMessageDetailsQualifier.poll;
                messageDetails.Function = GovTalkMessageHeaderMessageDetailsFunction.submit;
                messageDetails.FunctionSpecified = true;
                messageDetails.CorrelationID = correlationId;
                messageDetails.ResponseEndPoint = endpoint;
                messageDetails.Transformation = GovTalkMessageHeaderMessageDetailsTransformation.XML;
                header.MessageDetails = messageDetails;
                header.SenderDetails = senderDeatils;

                GovTalkMessageGovTalkDetails details = new GovTalkMessageGovTalkDetails();
                //GovTalkMessageGovTalkDetailsKey key = new GovTalkMessageGovTalkDetailsKey();
                GovTalkMessageGovTalkDetailsKey[] Keys = new GovTalkMessageGovTalkDetailsKey[1];
                //Keys[0] = key;
                details.Keys = Keys;
                GovTalkMessageBody body = new GovTalkMessageBody();
                GovTalkMessage govTalkMessage = new GovTalkMessage();
                govTalkMessage.EnvelopeVersion = ConfigurationManager.AppSettings["GovTalkMessageEnvelopeVersion"];
                govTalkMessage.Header = header;
                govTalkMessage.GovTalkDetails = details;
                govTalkMessage.Body = body;
                return govTalkMessage;
            }
            return null;
        }

        public GovTalkMessage CreateGovTalkMessageForDeleteRequest(string correlationId, bool testInLive)
        {
            if (correlationId != "")
            {
                //define classtype 
                string classType = !testInLive ? "HMRC-VAT-DEC" : "HMRC-VAT-DEC-TIL";

                GovTalkMessage govTalkMessage = new GovTalkMessage();
                govTalkMessage.EnvelopeVersion = ConfigurationManager.AppSettings["GovTalkMessageEnvelopeVersion"];
                //ConfigurationManager.AppSettings["GovTalkMessageEnvelopeVersion"].ToString();
                //header 
                GovTalkMessageHeader header = new GovTalkMessageHeader();
                //header message details 
                GovTalkMessageHeaderMessageDetails messageDetails = new GovTalkMessageHeaderMessageDetails();
                messageDetails.Class = classType;
                messageDetails.Qualifier = GovTalkMessageHeaderMessageDetailsQualifier.request;
                messageDetails.Function = GovTalkMessageHeaderMessageDetailsFunction.delete;
                messageDetails.CorrelationID = correlationId;
                messageDetails.Transformation = GovTalkMessageHeaderMessageDetailsTransformation.XML;
                messageDetails.FunctionSpecified = true;
                messageDetails.TransformationSpecified = true;
                //sender details 
                GovTalkMessageHeaderSenderDetails senderDeatils = new GovTalkMessageHeaderSenderDetails();
                header.SenderDetails = senderDeatils;
                header.MessageDetails = messageDetails;
                //govtalk details 
                GovTalkMessageGovTalkDetails govTalkDetails = new GovTalkMessageGovTalkDetails();
                GovTalkMessageGovTalkDetailsKey[] keys = new GovTalkMessageGovTalkDetailsKey[1];
                govTalkDetails.Keys = keys;
                //govtalk body 
                GovTalkMessageBody body = new GovTalkMessageBody();
                govTalkMessage.Body = body;
                govTalkMessage.Header = header;
                govTalkMessage.GovTalkDetails = govTalkDetails;
                return govTalkMessage;
            }
            return null;
        }

        public GovTalkMessage CreateGovTalkMessageForDataRequest(string senderId, string senderValue, bool testInLive)
        {
            if (!String.IsNullOrEmpty(senderId) && !String.IsNullOrEmpty(senderValue))
            {
                //define classtype 
                string classType = !testInLive ? "HMRC-VAT-DEC" : "HMRC-VAT-DEC-TIL";
                string gatewayTest = !testInLive ? "0" : "1";

                GovTalkMessage govTalkMessage = new GovTalkMessage();
                govTalkMessage.EnvelopeVersion =
                    ConfigurationManager.AppSettings["GovTalkMessageEnvelopeVersion"];
                //header 
                GovTalkMessageHeader header = new GovTalkMessageHeader();
                //header message details 
                GovTalkMessageHeaderMessageDetails messageDetails = new GovTalkMessageHeaderMessageDetails();
                messageDetails.Class = classType;
                messageDetails.Qualifier = GovTalkMessageHeaderMessageDetailsQualifier.request;
                messageDetails.Function = GovTalkMessageHeaderMessageDetailsFunction.list;
                messageDetails.Transformation = GovTalkMessageHeaderMessageDetailsTransformation.XML;
                messageDetails.FunctionSpecified = true;
                messageDetails.TransformationSpecified = true;
                messageDetails.CorrelationID = "";
                messageDetails.GatewayTest = gatewayTest;
                //sender details 
                GovTalkMessageHeaderSenderDetails senderDetails = new GovTalkMessageHeaderSenderDetails();
                GovTalkMessageHeaderSenderDetailsIDAuthentication idauth = new GovTalkMessageHeaderSenderDetailsIDAuthentication();
                idauth.SenderID = senderId;
                GovTalkMessageHeaderSenderDetailsIDAuthenticationAuthentication aauth = new GovTalkMessageHeaderSenderDetailsIDAuthenticationAuthentication();
                aauth.Method = GovTalkMessageHeaderSenderDetailsIDAuthenticationAuthenticationMethod.clear;
                aauth.Role = "principal";
                aauth.Item = senderValue;
                GovTalkMessageHeaderSenderDetailsIDAuthenticationAuthentication[] authentications = new GovTalkMessageHeaderSenderDetailsIDAuthenticationAuthentication[1];
                authentications[0] = aauth;
                idauth.Authentication = authentications;
                senderDetails.IDAuthentication = idauth;
                header.MessageDetails = messageDetails;
                header.SenderDetails = senderDetails;
                //govtalk details 
                GovTalkMessageGovTalkDetails govTalkDetails = new GovTalkMessageGovTalkDetails();
                GovTalkMessageGovTalkDetailsKey[] keys = new GovTalkMessageGovTalkDetailsKey[1];
                govTalkDetails.Keys = keys;
                //govtalk body 
                GovTalkMessageBody body = new GovTalkMessageBody();
                govTalkMessage.Body = body;
                govTalkMessage.Header = header;
                govTalkMessage.GovTalkDetails = govTalkDetails;
                return govTalkMessage;

            }
            return null;
        }

        public HMRCResponse SendHMRCMessage(string message, string uri)
        {
            HMRCResponse hmrcResponse = (HMRCResponse) null;
            if (uri.Trim() != "" && message != null)
            {
                try
                {
                    HttpWebRequest httpWebRequest = WebRequest.Create(uri) as HttpWebRequest;
                    if (httpWebRequest != null)
                    {
                        httpWebRequest.Method = "POST";
                        byte[] bytes = Encoding.UTF8.GetBytes(message);
                        httpWebRequest.ContentType = "application/x-www-form-urlencoded";
                        httpWebRequest.ContentLength = (long) bytes.Length;
                        Stream requestStream = httpWebRequest.GetRequestStream();
                        requestStream.Write(bytes, 0, bytes.Length);
                        requestStream.Close();
                        HttpWebResponse httpWebResponse = httpWebRequest.GetResponse() as HttpWebResponse;
                        if (httpWebResponse != null && httpWebResponse.StatusCode == HttpStatusCode.OK)
                        {
                            Stream responseStream = httpWebResponse.GetResponseStream();
                            if (responseStream != null)
                            {
                                StreamReader streamReader = new StreamReader(responseStream);
                                string str = streamReader.ReadToEnd();
                                hmrcResponse = new HMRCResponse(str);
                                streamReader.Close();
                            }
                            if (responseStream != null) responseStream.Close();
                        }

                        if (httpWebResponse != null) httpWebResponse.Close();
                    }
                }
                catch (WebException ex)
                {

                    using (var stream = ex.Response.GetResponseStream())
                    {
                        if (stream != null)
                            using (var reader = new StreamReader(stream))
                            {
                                string str = reader.ReadToEnd();
                                hmrcResponse = new HMRCResponse(str);
                                
                            }
                        hmrcResponse = null;
                    }
                }
            }
            return hmrcResponse;
        }

        private IRenvelope CreateIRenvelopeBody(Sender sender, Business business, PeriodData periodData,
            VAT100 vatData)
        {
            IRenvelope irenvelope = new IRenvelope();
            IRheader irheader = new IRheader();

            IRheaderKey irheaderkey = new IRheaderKey
            {
                Type = "VATRegNo",
                Value = business.VATRegNo
            };
            irheader.PeriodID = periodData.PeriodrefId;
            irheader.Sender = IRheaderSender.Individual;
           // irheader.PeriodStart = periodData.StartPeriod;
            //irheader.PeriodEnd = periodData.EndPeriod;

            IRheaderKey[] irHeaderKeys = new IRheaderKey[1];
            irHeaderKeys[0] = irheaderkey;
            irheader.Keys = irHeaderKeys;

            VATDeclarationRequest_ContactDetailsStructure contact = new VATDeclarationRequest_ContactDetailsStructure();
            VATDeclarationRequest_ContactDetailsStructureName name =
                new VATDeclarationRequest_ContactDetailsStructureName
                {
                    Fore = new[] {sender.ForName1, sender.ForName2},
                    Sur = sender.SurName,
                    Ttl = sender.Title
                };

            VATDeclarationRequest_ContactDetailsStructureEmail email =
                new VATDeclarationRequest_ContactDetailsStructureEmail
                {
                    Preferred = VATDeclarationRequest_YesNoType.yes,
                    PreferredSpecified = true,
                    Type = VATDeclarationRequest_WorkHomeType.work,
                    TypeSpecified = true,
                    Value = sender.Email
                };
            contact.Name = name;

            VATDeclarationRequest_ContactDetailsStructureEmail[] Emails =
                new VATDeclarationRequest_ContactDetailsStructureEmail[1];
            Emails[0] = email;
            contact.Email = Emails;

            IRheaderPrincipal principal = new IRheaderPrincipal {Contact = contact};
            irheader.Principal = principal;

            IRheaderIRmark irmark = new IRheaderIRmark();
            irmark.Type = IRheaderIRmarkType.generic;
            irmark.Value = "";
            irheader.IRmark = irmark;

            VATDeclarationRequest vatreq = new VATDeclarationRequest
            {
                VATDueOnOutputs = vatData.Box1,
                VATDueOnECAcquisitions = vatData.Box2,
                TotalVAT = vatData.Box3,
                VATReclaimedOnInputs = vatData.Box4,
                NetVAT = vatData.Box5,
                NetSalesAndOutputs = vatData.Box6,
                NetPurchasesAndInputs = vatData.Box7,
                NetECSupplies = vatData.Box8,
                NetECAcquisitions = vatData.Box9
            };

            //set values here
            irenvelope.IRheader = irheader;
            irenvelope.VATDeclarationRequest = vatreq;
            return irenvelope;
            
        }
       
        private XmlElement ToXmlElement(XElement element)
        {
            var doc = new XmlDocument();
            doc.Load(element.CreateReader());
            return doc.DocumentElement;
        }
        private static XNode IREnvelopeToXDocument(IRenvelope envelope)
        {
            if (envelope != null)
            {
                

                XNamespace ns = "http://www.govtalk.gov.uk/taxation/vat/vatdeclaration/2";
                XDocument doc =
                    new XDocument(
                        new XDeclaration("1.0", "UTF-16", null),
                        new XElement(ns + "IRenvelope",
                            new XElement(ns + "IRheader",
                                new XElement(ns + "Keys",
                                    new XElement(ns + "Key", new XAttribute("Type", "VATRegNo"),
                                        envelope.IRheader.Keys.Select(x => x.Value).FirstOrDefault().ToString())),
                                new XElement(ns + "PeriodID", envelope.IRheader.PeriodID.ToString()),
                                    // new XElement(ns + "PeriodStart", envelope.IRheader.PeriodStart.ToString("yyyy-MM-dd")),
                                     //new XElement(ns + "PeriodEnd", envelope.IRheader.PeriodEnd.ToString("yyyy-MM-dd")),
                                new XElement(ns + "Principal",
                                    new XElement(ns + "Contact",
                                        new XElement(ns + "Name",
                                            new XElement(ns + "Ttl", envelope.IRheader.Principal.Contact.Name.Ttl.ToString()),
                                            new XElement(ns + "Fore", envelope.IRheader.Principal.Contact.Name.Fore.GetValue(0).ToString()),
                                            new XElement(ns + "Fore", !String.IsNullOrEmpty(envelope.IRheader.Principal.Contact.Name.Fore.Last() ?? "")),
                                            new XElement(ns + "Sur", envelope.IRheader.Principal.Contact.Name.Sur.ToString())
                                            ),
                                        new XElement(ns + "Email",
                                            //new XAttribute("Type",
                                            //    envelope.IRheader.Principal.Contact.Email.Select(x => x.Type).FirstOrDefault().ToString()),
                                            //new XAttribute("Preferred",
                                            //    envelope.IRheader.Principal.Contact.Email.Select(x => x.Preferred).FirstOrDefault()
                                            //        .ToString()),
                                            envelope.IRheader.Principal.Contact.Email.Select(x => x.Value).FirstOrDefault().ToString()))
                                    ),
                                new XElement(ns + "IRmark", new XAttribute("Type", "generic"), ""),
                                new XElement(ns + "Sender", envelope.IRheader.Sender.ToString())
                                ),
                            new XElement(ns + "VATDeclarationRequest",
                                new XElement(ns + "VATDueOnOutputs",
                                    envelope.VATDeclarationRequest.VATDueOnOutputs.ToString()),
                                new XElement(ns + "VATDueOnECAcquisitions",
                                    envelope.VATDeclarationRequest.VATDueOnECAcquisitions.ToString()),
                                new XElement(ns + "TotalVAT", envelope.VATDeclarationRequest.TotalVAT.ToString()),
                                new XElement(ns + "VATReclaimedOnInputs",
                                    envelope.VATDeclarationRequest.VATReclaimedOnInputs.ToString()),
                                new XElement(ns + "NetVAT", envelope.VATDeclarationRequest.NetVAT.ToString()),
                                new XElement(ns + "NetSalesAndOutputs",
                                    envelope.VATDeclarationRequest.NetSalesAndOutputs.ToString()),
                                new XElement(ns + "NetPurchasesAndInputs",
                                    envelope.VATDeclarationRequest.NetPurchasesAndInputs.ToString()),
                                new XElement(ns + "NetECSupplies", envelope.VATDeclarationRequest.NetECSupplies.ToString()),
                                new XElement(ns + "NetECAcquisitions",
                                    envelope.VATDeclarationRequest.NetECAcquisitions.ToString())
                                )
                            ));
                //foreach (var element in doc.Descendants()) //just add namespace using for each instead of hard coding 
                //element.Name = aw + element.Name.LocalName;
                return doc;
            }
            return null;

        }
       
    }
}
