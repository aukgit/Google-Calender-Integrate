using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CalendarMvc.Models;
using CalendarMvc.Models.ViewModel;

namespace CalendarMvc.Controllers {
    public class EwsController : Controller {
        private ApplicationDbContext db = new ApplicationDbContext();
        // GET: Ews
        public ActionResult Index() {
            return View();
        }

        public ActionResult Create() {

            return View();
        }

        public ActionResult GetSharedCalendars() {
            var service = App.ExchangeServiceAccess;
            var sharedFolders = service.GetSharedCalendarFolders();
            foreach (var folder in sharedFolders) {
                var email = folder.Email;
                var eventOwner = db.EventOwners.FirstOrDefault(n => n.Email == email);
                if (eventOwner == null) {
                    eventOwner = new EventOwner();
                    db.EventOwners.Add(eventOwner); //insert
                }
                eventOwner.Email = email;
                eventOwner.OwnerName = folder.DisplayName;
            }
            db.SaveChanges();

            var list = db.EventOwners.ToList();
            return View(list);
        }

        [HttpPost]
        public ActionResult Create(AppointmentViewModel appointment) {
            var service = App.ExchangeServiceAccess;
            string[] attendees = null;
            if (appointment.Attendees != null) {
                attendees = appointment.Attendees.Split(',');
            }
            var meeting = service.WriteEventInCalendar(appointment.Title,
                                           appointment.Body,
                                           appointment.Start,
                                           appointment.End,
                                           appointment.Location,
                                           appointment.ReminderDueDate,
                                           appointment.RemindBeforeMins,
                                           attendees
                                           );

            return View("Success");
        }
    }
}