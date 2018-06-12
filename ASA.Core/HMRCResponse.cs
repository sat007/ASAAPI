using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Xml.Linq;

namespace ASA.Core
{
    public class HMRCResponse
    {

        private int _pollInterval;
        private DateTime _timeStamp;
        private IList<DataRequestStatus> _alDataRequest;
        private IList<SubmissionError> _alErrors;
        private string _correlationID;
        private string _responseData;
        private string _followOnUri;
        private ResponseType _type;
        public int PeriodId { get; set; }
        [ForeignKey("PeriodId")]
        public virtual PeriodData PeriodInfo { get; set; }

        [Key]
        public int HMRCResponseId { get; set; }
        public string CorrelationId
        {
            get { return this._correlationID; }
            set { this._correlationID = value; }

        }
        public string ResponseData
        {
            get { return this._responseData; }
            set { this._responseData = value; }

        }
        public string FollowOnUri
        {
            get { return this._followOnUri; }

        }
        public int PollInterval
        {
            get { return this._pollInterval; }
        }
        public DateTime TimeStamp
        {
            get { return this._timeStamp; }
            set { this._timeStamp = value; }
        }
        public IList<DataRequestStatus> DataRequest
        {
            get { return this._alDataRequest; }
        }
        public IList<SubmissionError> Errors
        {
            get { return _alErrors; }
            set { this._alErrors = value; }
        }
        public ResponseType Type
        {
            get { return this._type; }
            set { this._type = value; }
        }
        public enum ResponseType
        {
            Acknowledgement,
            Delete,
            Data,
            Error,
            Response,
            Unknown
        }
       // public virtual ICollection<DataRequestStatus> DataRequests { get; set; }
        public HMRCResponse()
        {
        }

        public HMRCResponse(string response)
        {
            this._responseData = response;
            var xDocument = XDocument.Parse(response);

            var elementsByTagName1 = (from d in xDocument.Descendants()
                                      where d.Name.LocalName == "Qualifier"
                                      select d);
            var elementsByTagName2 = (from d in xDocument.Descendants()
                                      where d.Name.LocalName == "Function"
                                      select d
                                     );

            if (elementsByTagName1 != null && elementsByTagName2 != null && elementsByTagName1.Count() == 1)
            {
                switch (elementsByTagName1.First().Value)
                {
                    case "acknowledgement":
                        this._type = ResponseType.Acknowledgement;
                        break;
                    case "error":
                        this._type = ResponseType.Error;
                        break;
                    case "response":
                        switch (elementsByTagName2.First().Value)
                        {
                            case "delete":
                                this._type = ResponseType.Delete;
                                break;
                            case "list":
                                this._type = ResponseType.Data;
                                break;
                            default:
                                this._type = ResponseType.Response;
                                break;
                        }
                        break;
                    default:
                        this._type = ResponseType.Unknown;
                        break;
                }
            }
            else
            {
                this._type = ResponseType.Unknown;
            }
            var elementsByTagName3 = (from d in xDocument.Descendants()
                                      where d.Name.LocalName == "CorrelationID"
                                      select d).First();

            if (elementsByTagName3 != null)
                this._correlationID = elementsByTagName3.Value;

            var elementsByTagName4 = (from d in xDocument.Descendants()
                                      where d.Name.LocalName == "ResponseEndPoint"
                                      select d);

            if (elementsByTagName4 != null)
            {
                this._followOnUri = elementsByTagName4.Select(i => i.Value).FirstOrDefault();
                var namedItem = elementsByTagName4.Attributes("PollInterval").FirstOrDefault();
                if (namedItem != null)
                    int.TryParse(namedItem.Value, out this._pollInterval);
            }
            var elementsByTagName5 = (from d in xDocument.Descendants()
                                      where d.Name.LocalName == "GatewayTimestamp"
                                      select d);
            if (elementsByTagName5 != null)
                DateTime.TryParse(elementsByTagName5.Select(g => g.Value).FirstOrDefault(), out this._timeStamp);
            this._alErrors = new List<SubmissionError>();

            if (this._type == HMRCResponse.ResponseType.Error)
            {
                var errorslist = (from d in xDocument.Descendants()
                                  where d.Name.LocalName == "Error"
                                  select d);

                var err = (from d in xDocument.Descendants()
                           where d.Name.LocalName == "err:Error"
                           select d);
                this.AddErrorsToList(errorslist, "");
                this.AddErrorsToList(err, "err:");
            }
            this._alDataRequest = new List<DataRequestStatus>();
            if (this._type != HMRCResponse.ResponseType.Data)
                return;
            var statusrecord = (from d in xDocument.Descendants()
                                where d.Name.LocalName == "StatusRecord"
                                select d);
            this.AddDataItemsToList(statusrecord);
        }
        private void AddDataItemsToList(IEnumerable data)
        {
            if (data == null)
            {
                return;
            }

            foreach (XElement node in data)
            {
                string correlationID = "";
                string status = "";
                XElement xmlNode2 = node.Element("CorrelationID");
                XElement xmlNode3 = node.Element("Status");
                if (xmlNode2 != null)
                    correlationID = xmlNode2.Value.ToString();
                if (xmlNode3 != null)
                    status = xmlNode3.Value.ToString();

                this._alDataRequest.Add(new DataRequestStatus(correlationID, status));
            }
        }

        private void AddErrorsToList(IEnumerable errors, string prefix)
        {
            if (errors == null)
                return;
            foreach (XElement xmlNode1 in errors)
            {
                string raisedBy = "";
                string number = "";
                string type = "";
                string text = "";
                string location = "";
                XElement xmlNode2 = (XElement)xmlNode1.Element(prefix + "RaisedBy");
                XElement xmlNode3 = (XElement)xmlNode1.Element(prefix + "Number");
                XElement xmlNode4 = (XElement)xmlNode1.Element(prefix + "Type");
                XElement xmlNode5 = (XElement)xmlNode1.Element(prefix + "Text");
                XElement xmlNode6 = (XElement)xmlNode1.Element(prefix + "Location");
                if (xmlNode2 != null)
                    raisedBy = xmlNode2.Value;
                if (xmlNode3 != null)
                    number = xmlNode3.Value;
                if (xmlNode4 != null)
                    type = xmlNode4.Value;
                if (xmlNode5 != null)
                    text = xmlNode5.Value;
                if (xmlNode6 != null)
                    location = xmlNode6.Value;
                this._alErrors.Add((SubmissionError)new SubmissionError(raisedBy, number, type, text, location));
            }
        }
    }

    public class ResponseDenormView
    {
        public Guid CorrelationId { get; set; }
        public MessageType[] Message { get; set; }
        public System.DateTime AcceptedTime { get; set; }     
        public bool TestInLive { get; set; }
        public string PaymentDueDate { get; set; }
        public PaymentNotification  PaymentNotification { get; set; }
        public InformationNotification InformationNotification { get; set; }

    }
    public class MessageType
    {
        private string codeField;
        private string langField;
        private string valueField;   
        public string code
        {
            get
            {
                return this.codeField;
            }
            set
            {
                this.codeField = value;
            }
        }       
        public string lang
        {
            get
            {
                return this.langField;
            }
            set
            {
                this.langField = value;
            }
        }
        public string Value
        {
            get
            {
                return this.valueField;
            }
            set
            {
                this.valueField = value;
            }
        }
    }    
    public class PaymentNotification
    {
        public string Narrative { get; set; }
        public string NetVAT { get; set; }
        public string NilPaymentIndicator { get; set; }
    }
    public class InformationNotification
    {
        public string Narrative { get; set; }
    }
   
}
