using ASA.Core;
using ASA.Core.Interfaces;
using System;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace ASA.API
{
    internal class SubmissionManager
    {
        private readonly IGatewayService _gatewayService;
        private string _filename; 
        private string _path;
        private string _filepath;
        private string _submissionUrl;
        private string _pollUrl;
            
        public SubmissionManager(IGatewayService gatewayService)
        {
            this._gatewayService = gatewayService;
        }
        public SubmissionManager() { }

        public HMRCResponse SendSubmissionRequest(Sender sender, Business business, PeriodData periodData,
            VAT100 vatData, bool testInLive)
        {
            HMRCResponse hmrcResponse = (HMRCResponse)null;
            if(testInLive)
            {
                _submissionUrl = ConfigurationManager.AppSettings["testsubmissionurl"];
                
            }
            else
            {
                _submissionUrl = ConfigurationManager.AppSettings["submissionurl"];
            }
            //create govtalk message 
            var govTalkMessage = _gatewayService.CreateGovTalkMessage(sender, business, periodData, vatData, testInLive);

            //serailize object to string
            //var strGovTalkMessage = HelperMethods.Serialize(govTalkMessage);
            var strGovTalkMessage = HelperMethods.GovTalkMessageToXDocument(govTalkMessage).ToString();
            //generate irmark from govtalk message 
            byte[] bytes = Encoding.UTF8.GetBytes(strGovTalkMessage);

            //var iREnvelopeBody = _gatewayService.CreateIRenvelopeBody(sender, business, perioddata, vatData);

            var irmark = HelperMethods.GetIRMark(bytes);
            XDocument xdoc;
            //set irmark value to the govtalk message body 
            if (irmark != null)
            {
                xdoc = XDocument.Parse(strGovTalkMessage, LoadOptions.PreserveWhitespace);
                //xdoc.Save("Without.xml");
                if (xdoc.Root != null)
                {
                    XNamespace ns = xdoc.Root.Name.Namespace;
                    XElement body = xdoc.Root.Element(ns + "Body");
                    if (body != null)
                    {
                        var bodyElelemts = from el in body.Descendants()
                                           where el.Name.LocalName == "IRenvelope"
                                           select el;

                        foreach (var ele in bodyElelemts)
                        {
                            XNamespace ns1 = ele.Name.Namespace;
                            var element = ele.Descendants(ns1 + "IRmark").First();
                            if (element != null)
                            {
                                element.Value = irmark;
                            }

                        }
                    }
                }
                string strSendRequest = xdoc.ToString();
                this._filename = business.BusinessName.ToString();
                this._path = Path.Combine(System.Web.HttpContext.Current.Server.MapPath("~/ClientData"), _filename);
                bool folderexists = CreateBusDir(_path);
                bool periodexists = CreatePerdiodDir(_path +"\\" +periodData.PeriodrefId);
                this._filepath = _path + "\\" + periodData.PeriodrefId;
                if (folderexists == true && periodexists)
                {
                    Log(strSendRequest, _filepath, "submission_Request");
                    //System.IO.StreamWriter file = new System.IO.StreamWriter(_filepath +"\\" + "submissionrequest.xml"+DateTime.Now);
                    //file.WriteLine(strSendRequest);
                    //file.Close();
                }
                hmrcResponse = _gatewayService.SendHMRCMessage(strSendRequest,
                    _submissionUrl);
                
                if(_filepath!=null && hmrcResponse.ResponseData !=null)
                {
                    if (hmrcResponse.Type.ToString() == "error")
                    {
                        Log(hmrcResponse.ResponseData, _filepath, "submission_Error");
                        
                    }
                    else
                        if(hmrcResponse.Type.ToString() == "acknowledgement") { 
                        Log(hmrcResponse.ResponseData, _filepath, "submission_Acknowledgement");
                        }
                        else
                        {
                        Log(hmrcResponse.ResponseData, _filepath, "submission_Response");
                        }

                }
                return hmrcResponse;
            }
            return null;
        }

        public HMRCResponse SendPollRequest(string correlationId, string uri, bool testInLive)
        {
            if (!(String.IsNullOrEmpty(correlationId) && String.IsNullOrEmpty(uri))) //send poll request 
            {
                var govTalkMessageForPoll =
                    _gatewayService.CreateGovTalkMessageForPollRequest(correlationId, uri, testInLive);
                //StringWriter sw = new StringWriter();
                //var pollreq = new XmlSerializer(typeof(GovTalkMessage));
                //pollreq.Serialize(sw, govTalkMessageForPoll);
                //HelperMethods.GoVTalkMessageToXDocumentForPoll(govtalkmessagepollreq);
                var pollreq = HelperMethods.GoVTalkMessageToXDocumentForPoll(govTalkMessageForPoll);
                var pollStr = pollreq.ToString();
                if (!(String.IsNullOrEmpty(pollStr)))
                {
                    Log(pollStr, _filepath, "submission_pollRequest");
                    var pollresponse = _gatewayService.SendHMRCMessage(pollStr, uri);
                    if (pollresponse.Type.ToString() == "Response")
                    {
                        Log(pollresponse.ResponseData, _filepath, "submission_pollResponse");
                        return pollresponse;
                    }
                    else
                    {
                        int x = 0;
                        do
                        {
                            var result = _gatewayService.SendHMRCMessage(pollStr, uri);
                            pollresponse = result;
                            if (pollresponse.Type.ToString() == "Response" &&
                                pollresponse.Errors.Count == 0)
                            {
                                //delete request in gateway tpengine and return sucess response back to client but move this as sepearte method  
                                //delete request moved to controller
                                Log(pollresponse.ResponseData, _filepath, "submission_pollResponse");
                                return pollresponse;

                            }
                        } while (pollresponse.Type.ToString() == "Acknowledgement" && x < 5);
                        Log(pollresponse.ResponseData, _filepath, "submission_pollErrors");
                        return pollresponse;
                    }
                }


            }
            return null;
        }
        public HMRCResponse SendDeleteRequest(string correlationId, bool testInLive, string uri)
        {
            HMRCResponse hmrcResponse = (HMRCResponse)null;
            if (!String.IsNullOrEmpty(correlationId))
            {
                var govTalkMessageForDeletion = _gatewayService.CreateGovTalkMessageForDeleteRequest(correlationId, testInLive);
                //StringWriter sw = new StringWriter();
                //var deleteReq = new XmlSerializer(typeof(GovTalkMessage));
                //deleteReq.Serialize(sw, govTalkMessageForDeletion);
                var deletereq = HelperMethods.GovTalkMessageToXDocumentForDelete(govTalkMessageForDeletion, testInLive).ToString();
                var strdeleteReq = deletereq.ToString();
                Log(strdeleteReq, _filepath, "submission_deleteRequest");
                if (!(String.IsNullOrEmpty(strdeleteReq)))
                {
                    hmrcResponse = _gatewayService.SendHMRCMessage(strdeleteReq, uri);
                    Log(hmrcResponse.ResponseData, _filepath, "submission_deleteResponse");
                    return hmrcResponse;
                }
            }
            Log(hmrcResponse.ResponseData, _filepath, "submission_deleteResponse");
            return hmrcResponse;
        }
        public HMRCResponse SendDataRequest(string senderId, string senderValue, bool testInLive)
        {
            HMRCResponse hmrcResponse = (HMRCResponse)null;
            if (testInLive)
            {
                _submissionUrl = ConfigurationManager.AppSettings["testsubmissionurl"];

            }
            else
            {
                _submissionUrl = ConfigurationManager.AppSettings["submissionurl"];
            }
            if (!String.IsNullOrEmpty(senderId) && !String.IsNullOrEmpty(senderValue))
            {
                var govTalkMessageForDataRequest = _gatewayService.CreateGovTalkMessageForDataRequest(senderId, senderValue, testInLive);
                var dataRequest = HelperMethods.GovTalkMessageToXDocumentForData(govTalkMessageForDataRequest, testInLive);
                var strDataRequest = dataRequest.ToString();
                Log(strDataRequest, _filepath, "submission_dataRequest");
                if (!(String.IsNullOrEmpty(strDataRequest)))
                {
                    hmrcResponse = _gatewayService.SendHMRCMessage(strDataRequest, _submissionUrl);
                    Log(hmrcResponse.ResponseData, _filepath, "submission_dataResponse");
                    return hmrcResponse;
                }               
             }
            Log(hmrcResponse.ResponseData, _filepath, "submission_deleteResponse");
            return hmrcResponse;
        }


        public bool CreateBusDir(string path)
        {
           bool exists = System.IO.Directory.Exists(path);

            if (!exists)
            {
                System.IO.Directory.CreateDirectory((path));
                
            }
            return true;

        }
        public bool CreatePerdiodDir(string path)
        {
            bool exists = System.IO.Directory.Exists(path);

            if (!exists)
            {
                System.IO.Directory.CreateDirectory((path));

            }
            return true;
        }
        private void Log(string inputData, string path, string filename)
        {
            if (inputData != null && path!=null && filename!=null)
            {
                System.IO.StreamWriter file = new System.IO.StreamWriter(Path.Combine(path + "\\", filename +"_"+ DateTime.Now.ToString("dd.MM.yyyy HH.mm.ss")+".xml"));
                file.WriteLine(inputData);
                file.Close();
            }
        }
        public DateTime AddMonthsCustom(DateTime source, int months)
        {
            var firstDayOfTargetMonth = new DateTime(source.Year, source.Month, 1).AddMonths(months);
            var lastDayofSourceMonth = DateTime.DaysInMonth(source.Year, source.Month);
            var lastDayofTargetMonth = DateTime.DaysInMonth(firstDayOfTargetMonth.Year, firstDayOfTargetMonth.Month);

            var targetDay = source.Day > lastDayofTargetMonth ? lastDayofTargetMonth : source.Day;
            if (source.Day == lastDayofSourceMonth)
                targetDay = lastDayofTargetMonth;

            return new DateTime(
                firstDayOfTargetMonth.Year,
                firstDayOfTargetMonth.Month,
                targetDay,
                source.Hour,
                source.Minute,
                source.Second,
                source.Millisecond,
                source.Kind);
        }
    }
}
