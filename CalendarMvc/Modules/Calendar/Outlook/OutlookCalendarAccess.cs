using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using CalendarMvc.Models;
using Microsoft.Experimental.IdentityModel.Clients.ActiveDirectory;
using Microsoft.OData.Client;
using Microsoft.Office365.OutlookServices;
using Newtonsoft.Json;

namespace CalendarMvc.Modules.Calendar.Outlook {
    public class OutlookCalendarAccess {
        private static readonly string[] Scopes = {
            "https://outlook.office.com/contacts.read",
            "https://outlook.office.com/mail.read",
            "https://outlook.office.com/calendars.read"
        };

        private readonly string _authorityUrl = "https://login.microsoftonline.com/common";
        private readonly string _clientId = ConfigurationManager.AppSettings["ida:ClientID"];
        private readonly string _clientSecret = ConfigurationManager.AppSettings["ida:ClientSecret"];
        private readonly string _outlookApiEndpoint = "https://outlook.office.com/api/v2.0";

        /// <summary>
        ///     Get a url string to sign in Microsoft account.
        /// </summary>
        /// <param name="onSuccessRedirectUri"></param>
        /// <param name="additionalScopes"></param>
        /// <returns></returns>
        public async Task<string> GetMicrosoftSignInUrl(Uri onSuccessRedirectUri, string[] additionalScopes = null) {
            var authContext = new AuthenticationContext(_authorityUrl);

            // Generate the parameterized URL for Azure signin
            var authUri = await authContext.GetAuthorizationRequestUrlAsync(Scopes, additionalScopes, _clientId,
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

        private void InsertXAnchorMailboxHeader(object sender, SendingRequest2EventArgs e, string email) {
            e.RequestMessage.SetHeader("X-AnchorMailbox", email);
        }

        public ClientCredential GetCredentials() {
            return new ClientCredential(_clientId, _clientSecret);
        }

        public AuthenticationContext GetAuthenticationContext() {
            return new AuthenticationContext(_authorityUrl);
        }

        /// <summary>
        /// </summary>
        /// <param name="authCode">Get the 'code' parameter from the Azure redirect</param>
        /// <returns></returns>
        public async Task<OutlookToken> GetAccessToken(string authCode, Uri onSuccessRedirectUri) {
            // Get the 'code' parameter from the Azure redirect

            var authContext = GetAuthenticationContext();

            // Use client ID and secret to establish app identity
            var credential = GetCredentials();

            try {
                // Get the token
                var authResult = await authContext.AcquireTokenByAuthorizationCodeAsync(
                    authCode, onSuccessRedirectUri, credential, Scopes);
                var outlookToken = new OutlookToken();
                // Save the token in the session
                var token = "";

                outlookToken.IsRefreshTokenExpired = false;
                // Try to get user info
                outlookToken.Email = GetUserEmail(authContext, _clientId, out token);
                outlookToken.Token = authResult.Token;
                outlookToken.RefreshToken = await GetRefreshRoken(authCode, onSuccessRedirectUri);
                return outlookToken;
            } catch (Exception ex) {
                throw ex;
            }
        }

        private async Task<string> GetRefreshRoken(string authCode, Uri onSuccessRedirectUri) {
            return await GetRefreshRoken(authCode, onSuccessRedirectUri.AbsoluteUri);
        }

        private async Task<string> GetRefreshRoken(string authCode, string onSuccessRedirectUri) {
            var client = new HttpClient();
            var parameters = new Dictionary<string, string> {
                {"client_id", _clientId},
                {"client_secret", _clientSecret},
                {"code", authCode},
                {"redirect_uri", onSuccessRedirectUri},
                {"grant_type", "refresh_token"}
            };
            var content = new FormUrlEncodedContent(parameters);
            var response = await client.PostAsync(_authorityUrl + "/oauth2/v2.0/token", content);
            var tokensJsonString = await response.Content.ReadAsStringAsync();
            dynamic token = JsonConvert.DeserializeObject(tokensJsonString);
            return token.refresh_token;
        }

        private string GetUserEmail(AuthenticationContext context, string clientId, out string token) {
            // ADAL caches the ID token in its token cache by the client ID
            foreach (var item in context.TokenCache.ReadItems()) {
                if (item.Scope.Contains(clientId)) {
                    token = item.Token;
                    return GetEmailFromIdToken(item.Token);
                }
            }
            token = "";
            return string.Empty;
        }

        private string GetEmailFromIdToken(string token) {
            // JWT is made of three parts, separated by a '.' 
            // First part is the header 
            // Second part is the token 
            // Third part is the signature 
            var tokenParts = token.Split('.');
            if (tokenParts.Length < 3) {
                // Invalid token, return empty
            }
            // Token content is in the second part, in urlsafe base64
            var encodedToken = tokenParts[1];
            // Convert from urlsafe and add padding if needed
            var leftovers = encodedToken.Length % 4;
            if (leftovers == 2) {
                encodedToken += "==";
            } else if (leftovers == 3) {
                encodedToken += "=";
            }
            encodedToken = encodedToken.Replace('-', '+').Replace('_', '/');
            // Decode the string
            var base64EncodedBytes = Convert.FromBase64String(encodedToken);
            var decodedToken = Encoding.UTF8.GetString(base64EncodedBytes);
            // Load the decoded JSON into a dynamic object
            dynamic jwt = JsonConvert.DeserializeObject(decodedToken);
            // User's email is in the preferred_username field
            return jwt.preferred_username;
        }
    }
}