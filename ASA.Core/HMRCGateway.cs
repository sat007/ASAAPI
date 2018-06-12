using System;
using System.Collections.Generic;
using System.Configuration;
//using System.Data.SqlTypes;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace ASA.Core
{
    public class HMRCGateway
    {
        public GovTalkMessage CreateGovTalkMessage(Sender sender, Business business,PeriodData periodData,VAT100 vat100Data,bool testInLive )
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
                idauth.SenderID = sender.SenderId;
                GovTalkMessageHeaderSenderDetailsIDAuthenticationAuthentication aauth = new GovTalkMessageHeaderSenderDetailsIDAuthenticationAuthentication();
                aauth.Method = GovTalkMessageHeaderSenderDetailsIDAuthenticationAuthenticationMethod.clear;
                aauth.Role = "principal";
                aauth.Item = sender.SenderPassword;
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
                IR68 ir68 = new IR68(periodData, vat100Data);
                IRenvelope iRenvelope = ir68.CreateIREnvelopeBody(sender, business);
          //serialse ir object data into xml elements and add to Gov Talk body 
                XElement xe = XElement.Parse(SerializeIrEnvelope(iRenvelope));
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

        public GovTalkMessage CreatePollRequest(string correlationId, string polluri)
        {
            if (correlationId != "" && polluri.Trim()!="")
            {
                GovTalkMessageHeader header = new GovTalkMessageHeader();
                GovTalkMessageHeaderSenderDetails senderDeatils = new GovTalkMessageHeaderSenderDetails();
                GovTalkMessageHeaderMessageDetailsResponseEndPoint endpoint = new GovTalkMessageHeaderMessageDetailsResponseEndPoint();
                endpoint.PollInterval = "10";
                endpoint.Value = polluri;
                GovTalkMessageHeaderMessageDetails messageDetails = new GovTalkMessageHeaderMessageDetails();
                messageDetails.Class = "HMRC-VAT-DEC-TIL";
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

        public GovTalkMessage CreateDeleteRequest(string correlationId, bool testInLive)
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

        public GovTalkMessage CreateDataRequest(string senderId, string senderValue, bool testInLive)
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
        private string SerializeIrEnvelope(IRenvelope iRenvelope)
        {
            using (StringWriter sw = new StringWriter())
            {
                var serializer = new XmlSerializer(typeof(IRenvelope));
                XmlSerializerNamespaces ns = new XmlSerializerNamespaces();
                ns.Add("", ConfigurationManager.AppSettings["IRurl"].ToString());
                serializer.Serialize(sw, iRenvelope, ns);
                var xmlString = sw.ToString();
                return xmlString;
            }
        }
        private XmlElement ToXmlElement(XElement element)
        {
            var doc = new XmlDocument();
            doc.Load(element.CreateReader());
            return doc.DocumentElement;
        }
    
    }

    //internal static class ConfigurationManager
    //{
    //    //private Dictionary<string, string> _dictionary; 
    //    //public string AppSettings {
    //    //    get { return this._dictionary; }
    //    //}


    //    //XDocument _doc = XDocument.Load("..//packages.config");
    //    //IEnumerable<XElement> xelements = _doc.Elements("appSettings").Elements();
    //    //this._dictionary = xelements.ToDictionary(x => x.Name, x => x.Value);
    //    private static string _senderData;
    //    public static string SenderData
    //    {
    //        get { return _senderData = "SenderData.xml"; }
            
    //    }
    //    public static string ChannelVersion {
    //        get { return "2.02"; } 
    //         }

    //    public static string ChannelProduct {
    //        get { return "ASA Software Pro"; }
    //    }

    //    public static string ChannelUri
    //    {
    //        get { return "7468"; }
    //    }

    //    public static string GovTalkDetailsKeyType
    //    {
    //        get { return "VATRegNo"; }
    //    }

    //    public static string GovTalkDetailsKey
    //    {
    //        get { return "999900001"; }
    //    }

    //    public static string GovTalkMessageEnvelopeVersion
    //    {
    //        get { return "2.0"; }
    //    }

    //    public static string SubmissionUrl
    //    {
    //        get { return "https://secure.dev.gateway.gov.uk/submission"; }
    //    }

    //    public static string PollUrl
    //    {
    //        get { return "https://secure.dev.gateway.gov.uk/poll"; }
    //    }

    //    public static string PathToLocalStorage
    //    {
    //        get { return FileSystem.Current.LocalStorage.Path; }
    //    }

    //}

}
