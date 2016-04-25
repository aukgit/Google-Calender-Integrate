using Google.Apis.Auth.OAuth2;
using Google.Apis.Calendar.v3;
using Google.Apis.Calendar.v3.Data;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Google.Apis.Auth.OAuth2.Flows;
using Google.Apis.Auth.OAuth2.Responses;

namespace CalenderMvc.Modules.GoogleCalender {
    public class GoogleCalenderAccess {
        // If modifying these scopes, delete your previously saved credentials
        // at ~/.credentials/calendar-dotnet-quickstart.json
        private string[] _scopes = { CalendarService.Scope.CalendarReadonly, CalendarService.Scope.Calendar };
        private string ApplicationName = "Google Calendar API .NET Quickstart";
        private ClientSecrets _secrects;
        private UserCredential _credentials;


        private void SendReq() {
            string gurl = "client_id=" + ConfigurationManager.AppSettings["GoogleClientId"] +
                          "&client_secret=" + ConfigurationManager.AppSettings["GoogleClientSecret"];

            string url = "https://www.googleapis.com/oauth2/v3/token";

            // creates the post data for the POST request
            string postData = (gurl);

            // create the POST request
            HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(url);
            webRequest.Host = "www.googleapis.com";

            webRequest.Method = "POST";
            webRequest.ContentType = "application/x-www-form-urlencoded";
            webRequest.ContentLength = postData.Length;

            // POST the data
            using (StreamWriter requestWriter2 = new StreamWriter(webRequest.GetRequestStream())) {
                requestWriter2.Write(postData);
            }

            //This actually does the request and gets the response back
            HttpWebResponse resp = (HttpWebResponse)webRequest.GetResponse();

            string googleAuth;

            using (StreamReader responseReader = new StreamReader(webRequest.GetResponse().GetResponseStream())) {
                //dumps the HTML from the response into a string variable
                googleAuth = responseReader.ReadToEnd();
            }
        }
        /// <summary>
        /// Authenticating to Google using a Service account
        /// Documentation: https://developers.google.com/accounts/docs/OAuth2#serviceaccount
        /// </summary>
        /// <param name="serviceAccountEmail">From Google Developer console https://console.developers.google.com</param>
        /// <param name="keyFilePath">Location of the Service account key file downloaded from Google Developer console https://console.developers.google.com</param>
        /// <returns></returns>
        public static CalendarService AuthenticateServiceAccount(string serviceAccountEmail, string keyFilePath) {

            // check the file exists
            if (!File.Exists(keyFilePath)) {
                Console.WriteLine("An Error occurred - Key file does not exist");
                return null;
            }

            string[] scopes = new string[] {
        CalendarService.Scope.Calendar  ,  // Manage your calendars
        CalendarService.Scope.CalendarReadonly    // View your Calendars
            };

            var certificate = new X509Certificate2(keyFilePath, "notasecret", X509KeyStorageFlags.Exportable);
            try {
                ServiceAccountCredential credential = new ServiceAccountCredential(
                    new ServiceAccountCredential.Initializer(serviceAccountEmail) {
                        Scopes = scopes
                    }.FromCertificate(certificate));

                // Create the service.
                CalendarService service = new CalendarService(new BaseClientService.Initializer() {
                    HttpClientInitializer = credential,
                    ApplicationName = "Calendar API Sample",
                });
                return service;
            } catch (Exception ex) {

                Console.WriteLine(ex.InnerException);
                return null;

            }
        }
        private ClientSecrets GetClientSerects() {
            if (_secrects == null) {
                _secrects = new ClientSecrets() {
                    ClientId = ConfigurationManager.AppSettings["GoogleClientId"],
                    ClientSecret = ConfigurationManager.AppSettings["GoogleClientSecret"]
                };
            }
            return _secrects;
        }

        private UserCredential GetCredentials() {
            if (_credentials == null) {
                var flow = new GoogleAuthorizationCodeFlow(new GoogleAuthorizationCodeFlow.Initializer {
                        ClientSecrets = GetClientSerects(),
                        Scopes = _scopes
                    });
                var token = flow.LoadTokenAsync("user", CancellationToken.None).Result;
                _credentials = new UserCredential(flow,
                "user",
                token);
            }
            return _credentials;
        }
        private CalendarService GetService() {
            var credentials = GetCredentials();
            // Create Google Calendar API service.
            var service = new CalendarService(new BaseClientService.Initializer() {
                HttpClientInitializer = credentials,
                ApplicationName = ApplicationName,
            });
            return service;
        }

        public Events GetEvents(string calenderType = "primary", int max = 10) {
            SendReq();
            var service = GetService();

            // Define parameters of request.
            EventsResource.ListRequest request = service.Events.List(calenderType);
            request.TimeMin = DateTime.Now;
            request.ShowDeleted = false;
            request.SingleEvents = true;
            request.MaxResults = max;
            request.OrderBy = EventsResource.ListRequest.OrderByEnum.StartTime;

            // List events.
            return request.Execute();
        }

    }
}