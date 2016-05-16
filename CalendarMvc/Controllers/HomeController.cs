using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using CalendarMvc.Models;
using CalendarMvc.Models.ViewModel;
using CalendarMvc.Modules.Calendar.Outlook;
using Microsoft.Experimental.IdentityModel.Clients.ActiveDirectory;
using Microsoft.Office365.OutlookServices;

namespace CalendarMvc.Controllers {
    public class HomeController : Controller {
        OutlookCalendarAccess outlookAccess = new OutlookCalendarAccess();
        private readonly ApplicationDbContext db = new ApplicationDbContext();
        // followed from https://dev.outlook.com/restapi/tutorial/dotnet

        public ActionResult Index() {
            return View();
        }

        public async Task<ActionResult> SignIn() {
            // The url in our app that Azure should redirect to after successful sign in
            Uri redirectUri = new Uri(Url.Action("Authorize", "Home", null, Request.Url.Scheme));
            var microsoftSignInUrl = await outlookAccess.GetMicrosoftSignInUrl(redirectUri);
            // Redirect the browser to the Azure signin page
            return Redirect(microsoftSignInUrl);
        }

        public async Task<ActionResult> Authorize() {
            // Get the 'code' parameter from the Azure redirect
            string authCode = Request.Params["code"];

            Uri redirectUri = new Uri(Url.Action("Authorize", "Home", null, Request.Url.Scheme));
            var outlookToken = await outlookAccess.GetAccessToken(authCode, redirectUri);
            if (outlookToken != null) {
                Session["token"] = outlookToken;
                db.OutlookTokens.Add(outlookToken);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return HttpNotFound();
        }

        public async Task<ActionResult> Inbox() {
            var outlookToken = (OutlookToken) Session["token"];
            if (outlookToken == null) {
                // If there's no token in the session, redirect to Home
                return Redirect("/");
            }

            try {
                var client = await  outlookAccess.GetOutlookClient(outlookToken);

                var mailResults = await client.Me.Messages
                                  .OrderByDescending(m => m.ReceivedDateTime)
                                  .Take(10)
                                  .Select(m => new DisplayMessage(m.Subject, m.ReceivedDateTime, m.From))
                                  .ExecuteAsync();

                return View(mailResults.CurrentPage);
            } catch (AdalException ex) {
                return Content(string.Format("ERROR retrieving messages: {0}", ex.Message));
            }
        }

        public async Task<ActionResult> Calendar() {
            var outlookToken = (OutlookToken) Session["token"];
            if (outlookToken == null) {
                // If there's no token in the session, redirect to Home
                return Redirect("/");
            }

            try {
                var client = await outlookAccess.GetOutlookClient(outlookToken);
                var eventResults = await client.Me.Events
                                    .OrderByDescending(e => e.Start.DateTime)
                                    .Take(10)
                                    .Select(e => new DisplayEvent(e.Subject, e.Start.DateTime, e.End.DateTime))
                                    .ExecuteAsync();

                return View(eventResults.CurrentPage);
            } catch (AdalException ex) {
                return Content(string.Format("ERROR retrieving events: {0}", ex.Message));
            }
        }


        public ActionResult About() {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact() {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}