using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Security.Cryptography.Xml;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace ASA.Core
{
    //not been used anywhere in the solution 
    public static class PublicMethods
    {
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
        public static XmlElement ToXmlElement(this XElement element)
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
        private static string GetIRMark(byte[] Xml)
        {
            string vbLf = "\n";
            string vbCrLf = "\r\n";

            // Convert Byte array to string
            string text = Encoding.UTF8.GetString(Xml);
            XmlDocument doc = new XmlDocument();
            doc.PreserveWhitespace = true;
            doc.LoadXml(text);

            XmlNamespaceManager ns = new XmlNamespaceManager(doc.NameTable);
            ns.AddNamespace("env", doc.DocumentElement.NamespaceURI);
            XmlNode Body = doc.SelectSingleNode("//env:Body", ns);
            ns.AddNamespace("tax", Body.FirstChild.NamespaceURI);

            // Create an XML document of just the body section
            XmlDocument xmlBody = new XmlDocument();
            xmlBody.PreserveWhitespace = true;
            xmlBody.LoadXml(Body.OuterXml);

            // Remove any existing IRMark   
            XmlNode nodeIr = xmlBody.SelectSingleNode("//tax:IRmark", ns);
            if (nodeIr != null)
            {
                nodeIr.ParentNode.RemoveChild(nodeIr);
            }

            // Normalise the document using C14N (Canonicalisation)  
            XmlDsigC14NTransform c14n = new XmlDsigC14NTransform();
            c14n.LoadInput(xmlBody);
            using (Stream S = (Stream)c14n.GetOutput())
            {
                // var ss = new StreamReader(S).ReadToEnd();
                // Console.WriteLine(ss);
                // Console.ReadLine();
                byte[] Buffer = new byte[S.Length];

                // Convert to string and normalise line endings  
                S.Read(Buffer, 0, (int)S.Length);
                text = Encoding.UTF8.GetString(Buffer);
                text = text.Replace("&#xD;", "");
                text = text.Replace(vbCrLf, vbLf);
                text = text.Replace(vbCrLf, vbLf);

                // Convert the final document back into a byte array 
                byte[] b = Encoding.UTF8.GetBytes(text);

                // Create the SHA-1 hash from the final document 
                SHA1 SHA = SHA1.Create();
                byte[] hash = SHA.ComputeHash(b);

                //var hasher = WinRTCrypto.HashAlgorithmProvider.OpenAlgorithm(HashAlgorithm.Sha1);
                //byte[] hash = hasher.HashData(b);

                return Convert.ToBase64String(hash);
            }

        }
    }
}
