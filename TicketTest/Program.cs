using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Net;
using System.Security.Cryptography.X509Certificates;

namespace TicketTest
{
    class Program
    {

        private X509Certificate2 certificate_ { get; set; }


        static void Main(string[] args)
        {

            if (args.Length < 2)
            {
                Console.WriteLine("Please supply option and server e.g. TicketTest.exe R localhost [user] [userdirectory]");
                Console.WriteLine("Options:");
                Console.WriteLine("R Request Ticket");
            } else if (args[0].Equals("R")) {
                string option = args[0];
                string server = args[1];
                string userid, udc;
                if (args.Length == 4)
                {
                    userid = args[2];
                    udc = args[3];
                } else {
                    userid = "TestUserId";
                    udc = "TestUDC";
                }

                Program TicketObj=new Program();
                //Load Certificates
                TicketObj.TicketExampleCertificate();
                //Request ticket
                string Ticket=TicketObj.TicketRequest("POST", server, userid, udc);


                Console.WriteLine("Response");
                Console.WriteLine("***********************************************************************");
                Console.WriteLine();
                Console.WriteLine("Ticket");
                Console.WriteLine("------------------------");
                Console.WriteLine();
                Console.WriteLine(Ticket);
            }

            
            
        }

        public void TicketExampleCertificate()
        {
            Console.WriteLine("Loading certificate to use for ticket request");
            Console.WriteLine("***********************************************************************");
            // First locate the Qlik Sense certificate
            X509Store store = new X509Store(StoreName.My, StoreLocation.CurrentUser);
            try
            {
                store.Open(OpenFlags.ReadOnly);
                certificate_ = store.Certificates.Cast<X509Certificate2>().FirstOrDefault(c => c.FriendlyName == "QlikClient");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }

            Console.WriteLine("Certificate found");
            Console.WriteLine(certificate_.ToString());
            store.Close();
            ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };
            Console.WriteLine("Certificate Loaded");
            Console.WriteLine();
        }


        public string TicketRequest(string method, string server, string user, string userdirectory)
        {

            Console.WriteLine("Requesting Ticket");
            Console.WriteLine("***********************************************************************");
            Console.WriteLine();

            //Create URL to REST endpoint for tickets
            string url = "https://" + server + ":4243/qps/ticket";



            //Create the HTTP Request and add required headers and content in Xrfkey
            string Xrfkey = "0123456789abcdef";
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url + "?Xrfkey=" + Xrfkey);
            // Add the method to authentication the user
            request.ClientCertificates.Add(certificate_);
            request.Method = method;
            request.Accept = "application/json";
            request.Headers.Add("X-Qlik-Xrfkey", Xrfkey);



            string body = "{ 'UserId':'" + user + "','UserDirectory':'" + userdirectory + "','Attributes': []}";
            byte[] bodyBytes = Encoding.UTF8.GetBytes(body);

            if (!string.IsNullOrEmpty(body))
            {
                request.ContentType = "application/json";
                request.ContentLength = bodyBytes.Length;
                
                // Write Request object
            
                Console.WriteLine("URL");
                Console.WriteLine("------------------------");
                Console.WriteLine(request.RequestUri.ToString());
                Console.WriteLine();
                Console.WriteLine("Request Headers");
                Console.WriteLine("------------------------");
                Console.WriteLine(request.Headers.ToString());
                Console.WriteLine();
                
                //Write Body
                Console.WriteLine("Body");
                Console.WriteLine(body.ToString());
                Console.WriteLine();

                Stream requestStream = request.GetRequestStream();
                requestStream.Write(bodyBytes, 0, bodyBytes.Length);
                requestStream.Close();
            }

            // make the web request and return the content
            try
            {
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                Stream stream = response.GetResponseStream();
                return stream != null ? new StreamReader(stream).ReadToEnd() : string.Empty;
            }
            catch (WebException ex)
            {
                using (var stream = ex.Response.GetResponseStream())
                using (var reader = new StreamReader(stream))
                {
                    Console.WriteLine(ex.ToString());
                    Console.WriteLine(reader.ReadToEnd());
                    
                }
                return "Error";
            }

            
        }
    }
}
