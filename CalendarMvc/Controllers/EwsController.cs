using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CalendarMvc.Models.ViewModel;

namespace CalendarMvc.Controllers
{
    public class EwsController : Controller
    {
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
            var attendees = appointment.Attendees.split(",");

            service.WriteEventInCalendar(appointment.Title, attendees: attendees);

            return View();
        }
    }
}