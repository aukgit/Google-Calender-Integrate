

using System;
using System.Security.Policy;
using System.Threading.Tasks;
using System.Web.Mvc;
using Microsoft.Experimental.IdentityModel.Clients.ActiveDirectory;
using Org.BouncyCastle.Asn1.Ocsp;

namespace CalenderMvc.Modules.Calender.Outlook {
    public class OutlookCalenderAccess {

        string authority = "https://login.microsoftonline.com/common";
        string clientId = System.Configuration.ConfigurationManager.AppSettings["ida:ClientID"];
        string clientSecret = System.Configuration.ConfigurationManager.AppSettings["ida:ClientSecret"];
           private static string[] scopes = { "https://outlook.office.com/mail.read",
                                   "https://outlook.office.com/calendars.read" };
        /// <summary>
        /// Get a url string to sign in microsoft account.
        /// </summary>
        /// <param name="onSuccessRedirectUri"></param>
        /// <param name="additionalScopes"></param>
        /// <returns></returns>
        public async Task<string> GetMicrosoftSignInUrl(Uri onSuccessRedirectUri, string [] additionalScopes = null) {
            AuthenticationContext authContext = new AuthenticationContext(authority);

            // Generate the parameterized URL for Azure signin
            Uri authUri = await authContext.GetAuthorizationRequestUrlAsync(scopes, additionalScopes, clientId,
                onSuccessRedirectUri, UserIdentifier.AnyUser, null);

            // Redirect the browser to the Azure signin page
            return authUri.ToString();
        }
    }

}
