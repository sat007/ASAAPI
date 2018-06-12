using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Security.Cryptography;
using System.Security.Cryptography.Xml;
using System.Text;
using System.Web.Script.Serialization;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace ASA.API
{
    public static class HelperMethods
    {
      
        public static XDocument GovTalkMessageToXDocument(GovTalkMessage govTalkMessage)
        {
            if (govTalkMessage != null)
            {
                XNamespace ns = "http://www.govtalk.gov.uk/CM/envelope";
                XDocument doc =
                    new XDocument(
                        new XDeclaration("1.0", "UTF-16", null),
                        new XElement(ns + "GovTalkMessage",
                            new XElement("EnvelopeVersion", govTalkMessage.EnvelopeVersion.ToString()),
                            new XElement("Header",
                                new XElement("MessageDetails",
                                    new XElement("Class",
                                        govTalkMessage.Header.MessageDetails.Class.ToString()),
                                    new XElement("Qualifier", govTalkMessage.Header.MessageDetails.Qualifier.ToString()),
                                    new XElement("Function", govTalkMessage.Header.MessageDetails.Function.ToString()),
                                    new XElement("CorrelationID",
                                        govTalkMessage.Header.MessageDetails.CorrelationID.ToString()),
                                    new XElement("Transformation",
                                        govTalkMessage.Header.MessageDetails.Transformation.ToString()),
                                    new XElement("GatewayTest",
                                        govTalkMessage.Header.MessageDetails.GatewayTest.ToString())
                                    ),
                                new XElement("SenderDetails",
                                        new XElement("IDAuthentication",
                                        new XElement("SenderID",
                                            govTalkMessage.Header.SenderDetails.IDAuthentication.SenderID.ToString()),
                                        new XElement("Authentication",
                                        new XElement("Method",
                                            govTalkMessage.Header.SenderDetails.IDAuthentication.Authentication.Select(
                                                x => x.Method).FirstOrDefault().ToString()),
                                        new XElement("Role",
                                            govTalkMessage.Header.SenderDetails.IDAuthentication.Authentication.Select(
                                                x => x.Role).FirstOrDefault().ToString()),
                                        new XElement("Value",
                                            govTalkMessage.Header.SenderDetails.IDAuthentication.Authentication.Select(
                                                x => x.Item).FirstOrDefault().ToString())
                                        )),
                                    new XElement("EmailAddress",
                                        govTalkMessage.Header.SenderDetails.EmailAddress.ToString()))
                                ),
                            new XElement("GovTalkDetails",
                                    new XElement("Keys",
                                    new XElement("Key", new XAttribute("Type", "VATRegNo"),
                                        govTalkMessage.GovTalkDetails.Keys.Select(x => x.Value)
                                            .FirstOrDefault()
                                            .ToString())),
                                    new XElement("ChannelRouting",
                                        new XElement("Channel",
                                            new XElement("URI",
                                                govTalkMessage.GovTalkDetails.ChannelRouting.Select(u => u.Channel.Item)
                                                    .FirstOrDefault()
                                                    .ToString()),
                                            new XElement("Product",
                                                govTalkMessage.GovTalkDetails.ChannelRouting.Select(
                                                    u => u.Channel.Product).FirstOrDefault().ToString()),
                                            new XElement("Version",
                                                govTalkMessage.GovTalkDetails.ChannelRouting.Select(
                                                    u => u.Channel.Version).FirstOrDefault().ToString())
                                            )
                                        )
                                    ),
                            new XElement("Body"
                                //new XElement()
                                ))
                        );
                foreach (var element in doc.Descendants()) //just add namespace using for each instead of hard coding 
                    element.Name = ns + element.Name.LocalName;

                XElement xe = XElement.Parse(govTalkMessage.Body.Any.SingleOrDefault().OuterXml.ToString());
                doc.Root.Element(ns + "Body").Add(xe);
                return doc;
            }
            return null;
        }
        public static XDocument GoVTalkMessageToXDocumentForPoll(GovTalkMessage govTalkMessage)
        {
            if (govTalkMessage != null)
            {
                XNamespace ns1 = XNamespace.Get("http://www.w3.org/2001/XMLSchema-instance");
                XNamespace ns2 = XNamespace.Get("http://www.w3.org/2001/XMLSchema");

                XNamespace ns = "http://www.govtalk.gov.uk/CM/envelope";
                XDocument doc = new XDocument(
                        new XDeclaration("1.0", "UTF-16", null),
                        new XElement(ns + "GovTalkMessage",
                            // new XAttribute(XNamespace.Xmlns + "xsi",ns1),
                            //new XAttribute(XNamespace.Xmlns + "xsd", ns2),
                            //new XAttribute(XNamespace.Xmlns, ns),
                            new XElement("EnvelopeVersion", govTalkMessage.EnvelopeVersion.ToString()),
                            new XElement("Header",
                                new XElement("MessageDetails",
                                    new XElement("Class",
                                        govTalkMessage.Header.MessageDetails.Class.ToString()),
                                    new XElement("Qualifier", govTalkMessage.Header.MessageDetails.Qualifier.ToString()),
                                    new XElement("Function", govTalkMessage.Header.MessageDetails.Function.ToString()),
                                    new XElement("CorrelationID",
                                        govTalkMessage.Header.MessageDetails.CorrelationID.ToString()),
                                    new XElement("Transformation",
                                        govTalkMessage.Header.MessageDetails.Transformation.ToString())
                                    ),
                                new XElement("SenderDetails")
                                ),
                            new XElement("GovTalkDetails",
                                new XElement("Keys")
                                ),
                            new XElement("Body")
                            ));
                foreach (var element in doc.Descendants()) //just add namespace using for each instead of hard coding 
                    element.Name = ns + element.Name.LocalName;
                return doc;
            }
            return null;
        }
        public static XDocument GovTalkMessageToXDocumentForDelete(GovTalkMessage govTalkMessage, bool testInLive)
        {
            if (govTalkMessage != null)
            {
                XNamespace ns = "http://www.govtalk.gov.uk/CM/envelope";
                XDocument doc = new XDocument(
                        new XDeclaration("1.0", "UTF-16", null),
                        new XElement(ns + "GovTalkMessage",
                            new XElement("EnvelopeVersion", govTalkMessage.EnvelopeVersion.ToString()),
                            new XElement("Header",
                                new XElement("MessageDetails",
                                    new XElement("Class",
                                        govTalkMessage.Header.MessageDetails.Class.ToString()),
                                    new XElement("Qualifier", govTalkMessage.Header.MessageDetails.Qualifier.ToString()),
                                    new XElement("Function", govTalkMessage.Header.MessageDetails.Function.ToString()),
                                    new XElement("CorrelationID",
                                        govTalkMessage.Header.MessageDetails.CorrelationID.ToString()),
                                    new XElement("Transformation",
                                        govTalkMessage.Header.MessageDetails.Transformation.ToString()),
                                    new XElement("GatewayTimestamp")
                                    ),
                                new XElement("SenderDetails")
                                ),
                            new XElement("GovTalkDetails",
                                new XElement("Keys")
                                ),
                            new XElement("Body")
                            ));
                foreach (var element in doc.Descendants()) //just add namespace using for each instead of hard coding 
                    element.Name = ns + element.Name.LocalName;
                return doc;

            }
            return null;
        }
        public static XDocument GovTalkMessageToXDocumentForData(GovTalkMessage govTalkMessage, bool testInLive)
        {
            if (govTalkMessage != null)
            {
                XNamespace ns = "http://www.govtalk.gov.uk/CM/envelope";
                XDocument doc = new XDocument(
                        new XDeclaration("1.0", "UTF-16", null),
                        new XElement(ns + "GovTalkMessage",
                            new XElement("EnvelopeVersion", govTalkMessage.EnvelopeVersion.ToString()),
                            new XElement("Header",
                                new XElement("MessageDetails",
                                    new XElement("Class",
                                        govTalkMessage.Header.MessageDetails.Class.ToString()),
                                    new XElement("Qualifier", govTalkMessage.Header.MessageDetails.Qualifier.ToString()),
                                    new XElement("Function", govTalkMessage.Header.MessageDetails.Function.ToString()),
                                    new XElement("CorrelationID"),
                                    new XElement("Transformation",
                                        govTalkMessage.Header.MessageDetails.Transformation.ToString())),
                                                                //new XElement("GatewayTimestamp")
                                                                //),
                                    new XElement("SenderDetails",
                                        new XElement("IDAuthentication",
                                        new XElement("SenderID",
                                            govTalkMessage.Header.SenderDetails.IDAuthentication.SenderID.ToString()),
                                        new XElement("Authentication",
                                        new XElement("Method",
                                            govTalkMessage.Header.SenderDetails.IDAuthentication.Authentication.Select(
                                                x => x.Method).FirstOrDefault().ToString()),
                                        new XElement("Role",
                                            govTalkMessage.Header.SenderDetails.IDAuthentication.Authentication.Select(
                                                x => x.Role).FirstOrDefault().ToString()),
                                        new XElement("Value",
                                            govTalkMessage.Header.SenderDetails.IDAuthentication.Authentication.Select(
                                                x => x.Item).FirstOrDefault().ToString())
                                    )))),
                            new XElement("GovTalkDetails",
                                new XElement("Keys")
                                ),
                            new XElement("Body")                  
                            ));
                foreach (var element in doc.Descendants()) //just add namespace using for each instead of hard coding 
                    element.Name = ns + element.Name.LocalName;
                return doc;

            }
            return null;
        }
        public static string SerializeIrEnvelope(IRenvelope iRenvelope)
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
        public static XmlElement ToXmlElement(XElement element)
        {
            var doc = new XmlDocument();
            doc.Load(element.CreateReader());
            return doc.DocumentElement;
        }
        public static string Serialize<T>(T value, XmlWriterSettings xmlWriterSettings = null)
        {
            if (value == null)
            {
                throw new ArgumentException("value");
            }

            var serializer = new XmlSerializer(typeof(T));

            var settings = xmlWriterSettings ?? new XmlWriterSettings
            {
                Encoding = new UnicodeEncoding(false, false),
                Indent = false,
                OmitXmlDeclaration = false
            };

            using (var textWriter = new StringWriter())
            {
                using (var xmlWriter = XmlWriter.Create(textWriter, settings))
                {
                    XmlSerializerNamespaces ns = new XmlSerializerNamespaces();
                    ns.Add("", "http://www.govtalk.gov.uk/CM/envelope");
                    serializer.Serialize(xmlWriter, value, ns);

                }

                return textWriter.ToString();
            }
        }

        public static T Deserialize<T>(string xml, XmlReaderSettings xmlReaderSettings = null)
            {
                if (string.IsNullOrEmpty(xml))
                {
                    throw new ArgumentException("xml");
                }
                var serializer = new XmlSerializer(typeof(T));
                var settings = xmlReaderSettings ?? new XmlReaderSettings();
                //No settings need modifying here 
                using (var textReader = new StringReader(xml))
                {
                    using (var xmlReader = XmlReader.Create(textReader, settings))
                    {
                        return (T)serializer.Deserialize(xmlReader);
                    }
                }
            }
            public static string GetIRMark(byte[] Xml)
            {
            string vbLf = "\n";
            string vbCrLf = "\r\n";
            int length = Xml.Length;
            // Convert Byte array to string
            string text = Encoding.UTF8.GetString(Xml, 0, length);
            XDocument xDoc = XDocument.Parse(text, LoadOptions.PreserveWhitespace);

            XNamespace xns = xDoc.Root.Name.Namespace;
            XElement body = xDoc.Root.Element(xns + "Body");

            var irmarkelement = from e in body.Descendants()
                                where e.Name.LocalName == "IRmark"
                                select e;
            if (irmarkelement != null)
            {
                irmarkelement.Remove();
            }
            XDocument xdoc = XDocument.Parse(body.ToString(), LoadOptions.PreserveWhitespace);

            var reader = xdoc.CreateReader();
            reader.MoveToContent();
            string strxml = reader.ReadOuterXml();

            // Normalise the document using C14N (Canonicalisation)  
            using (Stream stream = ToStream(strxml))
            {
                byte[] Buffer = new byte[stream.Length];
                stream.Read(Buffer, 0, (int)stream.Length);
                text = Encoding.UTF8.GetString(Buffer, 0, (int)stream.Length);
                text = text.Replace("&#xD;", "");
                text = text.Replace(vbCrLf, vbLf);
                text = text.Replace(vbCrLf, vbLf);
                byte[] b = Encoding.UTF8.GetBytes(text);
                SHA1 SHA = SHA1.Create();
                byte[] hashbytes = SHA.ComputeHash(b);
                return Convert.ToBase64String(hashbytes);

            }
            
    }
    private static Stream ToStream(string str)
    {
        byte[] bytes = Encoding.UTF8.GetBytes(str);
        return new MemoryStream(bytes);
    }
    public static string ExtractBodyContent(string xml)
        {
            string successResponse = "";
            if (!String.IsNullOrEmpty(xml))
            {
                XmlDocument doc = new XmlDocument();
                doc.PreserveWhitespace = true;
                doc.LoadXml(xml);
                XmlNamespaceManager ns = new XmlNamespaceManager(doc.NameTable);
                ns.AddNamespace("env", doc.DocumentElement.NamespaceURI);
                XmlNode Body = doc.SelectSingleNode("//env:Body", ns);
                successResponse = Body.InnerXml.ToString();
            }
            return successResponse;
        }
        private static List<DateTime> GetHolidays()
        {
            var client = new HttpClient();
            HttpResponseMessage response = client.GetAsync("https://www.gov.uk/bank-holidays.json").Result;
            response.EnsureSuccessStatusCode();
            if (response.IsSuccessStatusCode)
            {
                string responseBody = response.Content.ReadAsStringAsync().Result;
                var js = new JavaScriptSerializer();
                var holidays = js.Deserialize<Dictionary<string, GovJsonHolidays.Holidays>>(responseBody);
                return holidays["england-and-wales"].events.Select(d => d.Date).ToList();
            }
            return null;

        }
        public static int GetNumberOfWorkingDays(DateTime from, DateTime to)
        {
            int totaldays = 0;
            var holidays = GetHolidays();
            for (var date = from.AddDays(1); date < to; date = date.AddDays(1))
            {
                if (date.DayOfWeek != DayOfWeek.Saturday && date.DayOfWeek != DayOfWeek.Sunday && !holidays.Contains(date))
                {
                    totaldays++;
                }
            }
            return totaldays;
        }

        public static bool IsValidXml(string xmlFilePath, string xsdFilePath, string namespaceName)
            {
                var xdoc = XDocument.Parse(xmlFilePath);
                var schemas = new XmlSchemaSet();
                schemas.Add(namespaceName, xsdFilePath);

                Boolean result = true;
                xdoc.Validate(schemas, (sender, e) =>
                {
                    result = false;
                });

                return result;
            }
            //private static void Validate(String filename, XmlSchemaSet schemaSet)
            //{
            //    Console.WriteLine();
            //    Console.WriteLine("\r\nValidating XML file {0}...", filename.ToString());

            //    XmlSchema compiledSchema = null;

            //    foreach (XmlSchema schema in schemaSet.Schemas())
            //    {
            //        compiledSchema = schema;
            //    }

            //    XmlReaderSettings settings = new XmlReaderSettings();
            //    settings.Schemas.Add(compiledSchema);
            //    settings.ValidationEventHandler += new ValidationEventHandler(ValidationCallBack);
            //    settings.ValidationFlags |= XmlSchemaValidationFlags.ReportValidationWarnings;
            //    settings.ValidationType = ValidationType.Schema;

            //    //Create the schema validating reader.
            //    XmlReader vreader = XmlReader.Create(filename, settings);

            //    while (vreader.Read()) { }

            //    //Close the reader.
            //    vreader.Close();
            //}

            ////Display any warnings or errors.
            //private static void ValidationCallBack(object sender, ValidationEventArgs args)
            //{
            //    if (args.Severity == XmlSeverityType.Warning)
            //        Console.WriteLine("\tWarning: Matching schema not found.  No validation occurred." + args.Message);
            //    else
            //        Console.WriteLine("\tValidation error: " + args.Message);

            //}
    }
    
}
namespace ASA.GovJsonHolidays
{
    public class Holidays
    {
        public string division { get; set; }
        public List<Event> events { get; set; }
    }

    public class Event
    {
        public DateTime Date { get; set; }
        public string Notes { get; set; }
        public string Title { get; set; }
    }

}

