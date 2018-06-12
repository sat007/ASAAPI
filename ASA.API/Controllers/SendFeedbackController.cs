using Newtonsoft.Json.Linq;
using SendGrid;
using SendGrid.Helpers.Mail;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace ASA.API.Controllers
{
    public class SendFeedbackController : ApiController
    {
        [HttpPost]
        public async Task<HttpResponseMessage> Post([FromBody]JObject data)
        {
            string email = data["Email"].ToString();
            string sento = data["To"].ToString();
            string body = data["Body"].ToString();
            if (!String.IsNullOrEmpty(email) && !String.IsNullOrEmpty(sento))
            {
                try
                {
                    //ASA
                    var apiKey = "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";
                    //var apiKey = Environment.GetEnvironmentVariable("ASA");
                    var client = new SendGridClient(apiKey);
                    var from = new EmailAddress(email, "User Feedback for service - VAT");
                    var subject = "Feedback";
                    var to = new EmailAddress(sento, "support");
                   // var cc = new EmailAddress("satwikchoudary@gmail.com", "support");
                    var plainTextContent = body;
                    var htmlContent = body;
                    List<EmailAddress> emails = new List<EmailAddress>();
                    emails.Add(new EmailAddress(sento));
                    emails.Add(new EmailAddress("satwikchoudary@gmail.com"));
                    //var msg = MailHelper.CreateSingleEmail(from, to, subject, plainTextContent, htmlContent);
                    var msg = MailHelper.CreateSingleEmailToMultipleRecipients(from, emails, subject, plainTextContent, htmlContent);
                    var response = await client.SendEmailAsync(msg);
                    return new HttpResponseMessage(HttpStatusCode.OK);

                }
                catch (Exception ex)
                {

                    throw ex;
                }

            }
            return null;

        }
    }
}
