using Newtonsoft.Json.Linq;
using SendGrid;
using SendGrid.Helpers.Mail;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.IO;

namespace ASA.API.Controllers
{
    
    public class ExportController : ApiController
    {

        [HttpPost]
        public async Task<HttpResponseMessage> Post([FromBody]JObject data)
        {           
            string sento = data["To"].ToString();
            string imgData = data["imgData"].ToString();

            byte[] base64EncodedBytes = System.Convert.FromBase64String(imgData);
            //BinaryWriter writer = new BinaryWriter(File.Open(@"C:\Temp\ASA.API\ASA.API\ClientData\pdf9.png", FileMode.CreateNew));
            //writer.Write(base64EncodedBytes);

            //string s = System.Text.Encoding.UTF8.GetString(base64EncodedBytes);
            //using (var imageFile = new FileStream(@"C:\Temp\ASA.API\ASA.API\ClientData\pdf9.pdf", FileMode.Create))
            //{
            //    imageFile.Write(base64EncodedBytes, 0, base64EncodedBytes.Length);
            //    imageFile.Flush();
            //}
            //var decodedFileBytes = Convert.FromBase64String(encodedFile);
           // File.WriteAllBytes(@"C:\Temp\ASA.API\ASA.API\ClientData\pdf9.png", base64EncodedBytes);



            if (!String.IsNullOrEmpty(sento))
            {
                try
                {
                    //ASA
                    var apiKey = "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";
                    //var apiKey = Environment.GetEnvironmentVariable("ASA");
                    var client = new SendGridClient(apiKey);
                    var from = new EmailAddress("asasoftwaresolutions@outlook.com", "VAT-Submission");
                    var subject = "ASA-VAT Returns";
                    var to = new EmailAddress(sento, "support");
                    // var cc = new EmailAddress("satwikchoudary@gmail.com", "support");
                    string fileContentsAsBase64 = Convert.ToBase64String(base64EncodedBytes);
                    //var attachment = new Attachment
                    //{
                    //    Filename = "SubmissionInformation.pdf",
                    //    Type = "application/pdf",
                    //    Content = fileContentsAsBase64
                    //};
                    var plainTextContent = "Autmated Email generated from service to export the VAT submission, please find the attachment";
                    var htmlContent = "Autmated Email generated from service to export the VAT submission, please find the attachment";
                  
                    var msg = MailHelper.CreateSingleEmail(from, to, subject, plainTextContent, htmlContent);
                    msg.AddAttachment("VATReturn.pdf", fileContentsAsBase64);//fileContentsAsBase64
                   
                    //var msg = MailHelper.CreateSingleEmailToMultipleRecipients(from, emails, subject, plainTextContent, htmlContent);
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
