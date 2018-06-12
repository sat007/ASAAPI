using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using SendGrid;
using SendGrid.Helpers.Mail;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace ASA.API.Controllers
{
    public class PasswordRecoverController : ApiController
    {
        [HttpPost]
        public async Task<HttpResponseMessage> Post([FromBody]JObject data)
        {
            string useremail = data["useremail"].ToString();
            string userpwd = data["userpwd"].ToString();
            if (!String.IsNullOrEmpty(useremail) && !String.IsNullOrEmpty(userpwd))
            {
                try
                {
                    //ASA
                    var apiKey = "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";
                    //var apiKey = Environment.GetEnvironmentVariable("ASA");
                    var client = new SendGridClient(apiKey);
                    var from = new EmailAddress("noreply@outlook.com", "ASA Support");
                    var subject = "Password reovery service";
                    var to = new EmailAddress(useremail, "User");
                    var plainTextContent = "password: " + userpwd;
                    var htmlContent = "<strong>Your password is: </strong>" + userpwd;
                    var msg = MailHelper.CreateSingleEmail(from, to, subject, plainTextContent, htmlContent);
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
