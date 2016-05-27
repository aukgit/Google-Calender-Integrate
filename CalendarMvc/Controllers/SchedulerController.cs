using System;
using System.Web.Mvc;
using CalendarMvc.Models;
using System.Linq;
using CalendarMvc.Models.ViewModel;
using Microsoft.Exchange.WebServices.Data;

namespace CalendarMvc.Controllers {
    public class SchedulerController : Controller {

        private static string[] scopes = { "https://outlook.office.com/mail.read" };
        private ApplicationDbContext db = new ApplicationDbContext();

        public ActionResult Read() {
            var service = App.ExchangeServiceAccess;
            var meetings = service.GetCalendarItems(DateTime.Now.AddMonths(-2), DateTime.Now.AddMonths(2));
            var owners = db.EventOwners.ToList();
            var kendoViewModels = meetings.Select(n => {
                var firstOrDefault = owners.FirstOrDefault(owner => owner.Email == n.Email);
                var result = new KendoSchedulerViewModel();
                if (firstOrDefault != null) {
                    result.OwnerId = firstOrDefault.EventOwnerID;
                } else {
                    result.OwnerId = -1;
                }
                var appointment = (Appointment) n.Meeting;
                result.Email = n.Email;
                result.Title = n.Subject;
                result.Description = n.Body;
                result.Start = appointment.Start;
                result.End = appointment.End;
                result.IsAllDay = appointment.IsAllDayEvent;
                result.Recurrence = appointment.ICalRecurrenceId.ToString();
                return result;
            }).ToList();
            return Json(kendoViewModels, JsonRequestBehavior.AllowGet);
        }


        public ActionResult Index() {

            return View("ListEvents");
        }
    }
}