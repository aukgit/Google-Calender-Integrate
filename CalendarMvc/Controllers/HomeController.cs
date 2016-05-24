using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using CalendarMvc.Models;
using CalendarMvc.Models.ViewModel;
using CalendarMvc.Modules.Calendar.Outlook;
using DevMvcComponent.Extensions;
using Microsoft.Experimental.IdentityModel.Clients.ActiveDirectory;

namespace CalendarMvc.Controllers {
    public class HomeController : Controller {
        private readonly ApplicationDbContext db = new ApplicationDbContext();
        private readonly OutlookCalendarAccess outlookAccess = new OutlookCalendarAccess();
        // followed from https://dev.outlook.com/restapi/tutorial/dotnet
        // https://dev.outlook.com/restapi
        //http://www.matvelloso.com/2015/01/30/troubleshooting-common-azure-active-directory-errors/
        public ActionResult Index() {
            return View();
        }

        public async Task<ActionResult> SignIn() {
            // The url in our app that Azure should redirect to after successful sign in
            var redirectUri = new Uri(Url.Action("Authorize", "Home", null, Request.Url.Scheme));
            var microsoftSignInUrl = await outlookAccess.GetMicrosoftSignInUrl(redirectUri);
            // Redirect the browser to the Azure signin page
            return Redirect(microsoftSignInUrl);
        }

        public async Task<ActionResult> Authorize() {
            // Get the 'code' parameter from the Azure redirect
            var authCode = Request.Params["code"];
            var redirectUri = new Uri(Url.Action("Authorize", "Home", null, Request.Url.Scheme) );
            var outlookToken = await outlookAccess.GetAccessToken(authCode, redirectUri);
            if (outlookToken != null) {
                AddTokenToDatabase(outlookToken);
                return RedirectToAction("Index");
            }
            return HttpNotFound();
        }

        private async void AddTokenToDatabase(OutlookToken outlookToken) {
            if (outlookToken != null) {
                var dbToken = db.OutlookTokens.FirstOrDefault(n => n.Email == outlookToken.Email);
                if (dbToken != null) {
                    dbToken.Token = outlookToken.Token;
                    dbToken.RefreshToken = outlookToken.RefreshToken;
                    dbToken.IsRefreshTokenExpired = outlookToken.IsRefreshTokenExpired;
                } else {
                    db.OutlookTokens.Add(outlookToken);
                }
                db.SaveChanges();
            }
        }

        public async Task<ActionResult> Inbox() {
            var outlookTokens = db.OutlookTokens
                //.Select(n => new OutlookTokenViewModel {Token = n.Token, Email = n.Email, OutlookTokenID = n.OutlookTokenID})
                                  .ToList();
            if (outlookTokens.Count == 0) {
                // If there's no token in the session, redirect to Home
                return Redirect("/");
            }
            var list = new List<OutlookTokenViewModel>(outlookTokens.Count + 2);

            foreach (var outlookToken in outlookTokens) {
                var outlookTokenViewModel = outlookToken.Cast<OutlookToken, OutlookTokenViewModel>();

                try {
                    var client = await outlookAccess.GetOutlookClient(outlookToken);
                    var mailResults = await client.Me.Messages
                                                  .OrderByDescending(m => m.ReceivedDateTime)
                                                  .Take(4)
                                                  .Select(m => new DisplayMessage(m.Subject, m.ReceivedDateTime, m.From))
                                                  .ExecuteAsync();
                    var eventResults = await client.Me.Events
                                                   .OrderByDescending(e => e.Start.DateTime)
                                                   .Take(4)
                                                   .Select(e => new DisplayEvent(e.Subject, e.Start.DateTime, e.End.DateTime))
                                                   .ExecuteAsync();
                    outlookTokenViewModel.Messages = mailResults.CurrentPage;
                    outlookTokenViewModel.Events = eventResults.CurrentPage;
                    list.Add(outlookTokenViewModel);
                } catch (AdalException ex) {
                   // return Content(string.Format("ERROR retrieving messages: {0}", ex.Message));
                }
            }
            return View(list);
        }

        public ActionResult About() {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact() {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        ~HomeController() {
            db.Dispose();
        }
    }
}