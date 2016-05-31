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
                                           attendees: attendees
                                           );

            return View("Success");
        }
    }
}