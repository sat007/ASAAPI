using ASA.Core;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ASA.API.Model
{
    public class PeriodViewModel
    {
        [Required]
        public string PeriodRefId { get; set; }
        //[Required]
        public int PeriodId { get; set; }
                                           // public byte TaxQuater { get; set; }
        [Required]
        public DateTime StartPeriod { get; set; }
        [Required]
        public DateTime EndPeriod { get; set; }
        public SubmissionStatus Status { get; set; }
    }
    public class DenormPeriodView
    {
        public int Quater { get; set; }
        public IList<string> Number { get; set; }
    }
    public class ResponseViewModel
    {
        private SuccessResponse _response;
        private MessageType[] _message; 
        public ResponseViewModel(HMRCResponse XML)
        {
            if(XML != null)
            {
                var strContent = HelperMethods.ExtractBodyContent(XML.ResponseData.ToString());
                this._response = HelperMethods.Deserialize<SuccessResponse>(strContent);
                this._message = _response.Message;

            }

        }
        public ResponseViewModel() { }
        public int PeriodId { get; set; }
        public string PeriodRefId { get; set; }
        public string ResponseXML { get; set; }
        public SuccessResponse Response
        {
            get
            {
                return this._response;
            }
            //set { this._response = value; }
        }
        [JsonConverter(typeof(StringEnumConverter))]
        public SubmissionStatus Status { get; set; }

         public MessageType[] Message { get { return this._message; }
            //set { this._message = value;  }
        }
        public System.DateTime AcceptedTime { get; set; }
        //public bool TestInLive { get; set; }
        public string PaymentDueDate { get; set; }
        public PaymentNotification PaymentNotification { get; set; }
        public InformationNotification InformationNotification { get; set; }

    }   
}