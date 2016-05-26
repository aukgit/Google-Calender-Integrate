using System;
using System.Configuration;
using System.Threading;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Auth.OAuth2.Flows;
using Google.Apis.Calendar.v3;
using Google.Apis.Calendar.v3.Data;
using Google.Apis.Services;
using Google.Apis.Util.Store;

namespace CalendarMvc.Modules.Calendar.Google {
    public class GoogleCalendarAccess {
        // If modifying these scopes, delete your previously saved credentials
        // at ~/.credentials/calendar-dotnet-quickstart.json
        private string[] _scopes = { CalendarService.Scope.CalendarReadonly, CalendarService.Scope.Calendar };
        private string ApplicationName = "Google Calendar API .NET Quickstart";
        private ClientSecrets _secrects;
        private UserCredential _credentials;
        
        private ClientSecrets GetClientSerects() {
            if (_secrects == null) {
                _secrects = new ClientSecrets() {
                    ClientId = ConfigurationManager.AppSettings["GoogleClientId"],
                    ClientSecret = ConfigurationManager.AppSettings["GoogleClientSecret"]
                };
            }
            return _secrects;
        }

        private UserCredential GetCredentials(string userEmailAddress) {
            if (_credentials == null) {
                var flow = new GoogleAuthorizationCodeFlow(new GoogleAuthorizationCodeFlow.Initializer {
                    ClientSecrets = GetClientSerects(),
                    Scopes = _scopes
                });
                var token = flow.LoadTokenAsync(userEmailAddress, CancellationToken.None).Result;
                _credentials = GoogleWebAuthorizationBroker.AuthorizeAsync(
                GetClientSerects(),
                _scopes,
                userEmailAddress,
                CancellationToken.None,
                new FileDataStore("Daimto.GoogleCalendar.Auth.Store")).Result;
            }
            return _credentials;
        }
        private CalendarService GetService(string userEmailAddress) {
            var credentials = GetCredentials(userEmailAddress);
            // Create Google Calendar API service.
            var service = new CalendarService(new BaseClientService.Initializer() {
                HttpClientInitializer = credentials,
                ApplicationName = ApplicationName,
            });
            return service;
        }

        public Events GetEvents(string userEmailAddress,string calenderType = "primary", int max = 10) {
            var service = GetService(userEmailAddress);

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