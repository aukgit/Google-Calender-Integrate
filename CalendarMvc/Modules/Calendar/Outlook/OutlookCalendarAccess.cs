using System;
using System.Linq;
using System.Threading.Tasks;
using CalendarMvc.Models;
using Microsoft.Experimental.IdentityModel.Clients.ActiveDirectory;
using Microsoft.Office365.OutlookServices;

namespace CalendarMvc.Modules.Calendar.Outlook {
    public class OutlookCalendarAccess {
        readonly string _authority = "https://login.microsoftonline.com/common";
        readonly string _clientId = System.Configuration.ConfigurationManager.AppSettings["ida:ClientID"];
        readonly string _clientSecret = System.Configuration.ConfigurationManager.AppSettings["ida:ClientSecret"];
        readonly string _outlookApiEndpoint = "https://outlook.office.com/api/v2.0";
        private static readonly string[] Scopes = { "email", "profile", "https://outlook.office.com/contacts.read", "https://outlook.office.com/mail.read",
                                        "https://outlook.office.com/calendars.read" };
        /// <summary>
        /// Get a url string to sign in Microsoft account.
        /// </summary>
        /// <param name="onSuccessRedirectUri"></param>
        /// <param name="additionalScopes"></param>
        /// <returns></returns>
        public async Task<string> GetMicrosoftSignInUrl(Uri onSuccessRedirectUri, string[] additionalScopes = null) {
            AuthenticationContext authContext = new AuthenticationContext(_authority);

            // Generate the parameterized URL for Azure signin
            Uri authUri = await authContext.GetAuthorizationRequestUrlAsync(Scopes, additionalScopes, _clientId,
                onSuccessRedirectUri, UserIdentifier.AnyUser, null);

            // Redirect the browser to the Azure signin page
            return authUri.ToString();
        }

        public async Task<OutlookServicesClient> GetOutlookClient(OutlookToken outlookToken) {
            var client = new OutlookServicesClient(new Uri(_outlookApiEndpoint),
               async () => {
                   // Since we have it locally from the Session, just return it here.
                   return outlookToken.Token;
               });

            client.Context.SendingRequest2 += (sender, e) => InsertXAnchorMailboxHeader(sender, e, outlookToken.Email);
            return client;
        }

        private void InsertXAnchorMailboxHeader(object sender, Microsoft.OData.Client.SendingRequest2EventArgs e, string email) {
            e.RequestMessage.SetHeader("X-AnchorMailbox", email);
        }
        public ClientCredential GetCredentials() {
            return new ClientCredential(_clientId, _clientSecret);
        }

        public AuthenticationContext GetAuthenticationContext() {
            return new AuthenticationContext(_authority);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="authCode">Get the 'code' parameter from the Azure redirect</param>
        /// <returns></returns>
        public async Task<OutlookToken> GetAccessToken(string authCode, Uri onSuccessRedirectUri) {
            // Get the 'code' parameter from the Azure redirect

            var authContext = GetAuthenticationContext();


            // Use client ID and secret to establish app identity
            ClientCredential credential = GetCredentials();

            try {
                // Get the token
                var authResult = await authContext.AcquireTokenByAuthorizationCodeAsync(
                    authCode, onSuccessRedirectUri, credential, Scopes);
                var outlookToken = new OutlookToken();
                // Save the token in the session
                outlookToken.Token = authCode;

                // Try to get user info
                outlookToken.Email = GetUserEmail(authContext, _clientId);
                return outlookToken;
            } catch (Exception ex) {
                throw ex;
            }
            return null;
        }

        private string GetUserEmail(AuthenticationContext context, string clientId) {
            // ADAL caches the ID token in its token cache by the client ID
            foreach (TokenCacheItem item in context.TokenCache.ReadItems()) {
                if (item.Scope.Contains(clientId)) {
                    return GetEmailFromIdToken(item.Token);
                }
            }
            return string.Empty;
        }


 
        private string GetEmailFromIdToken(string token) {
            // JWT is made of three parts, separated by a '.' 
            // First part is the header 
            // Second part is the token 
            // Third part is the signature 
            string[] tokenParts = token.Split('.');
            if (tokenParts.Length < 3) {
                // Invalid token, return empty
            }
            // Token content is in the second part, in urlsafe base64
            string encodedToken = tokenParts[1];
            // Convert from urlsafe and add padding if needed
            int leftovers = encodedToken.Length % 4;
            if (leftovers == 2) {
                encodedToken += "==";
            } else if (leftovers == 3) {
                encodedToken += "=";
            }
            encodedToken = encodedToken.Replace('-', '+').Replace('_', '/');
            // Decode the string
            var base64EncodedBytes = System.Convert.FromBase64String(encodedToken);
            string decodedToken = System.Text.Encoding.UTF8.GetString(base64EncodedBytes);
            // Load the decoded JSON into a dynamic object
            dynamic jwt = Newtonsoft.Json.JsonConvert.DeserializeObject(decodedToken);
            // User's email is in the preferred_username field
            return jwt.preferred_username;
        }
    }

}
