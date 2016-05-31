using System;
using System.Collections.Generic;
using System.Web.Mvc;
using CalendarMvc.Models;
using System.Linq;
using System.Web.Caching;
using CalendarMvc.BusinessLogic;
using CalendarMvc.Models.ViewModel;
using Microsoft.Exchange.WebServices.Data;
using Newtonsoft.Json;

namespace CalendarMvc.Controllers {
    public class SchedulerController : Controller {

        //private static string[] scopes = { "https://outlook.office.com/mail.read" };
        private Dictionary<string, ItemId> _ids;
        private ApplicationDbContext db = new ApplicationDbContext();
        private EventOwnerLogic _eventOwnerLogic;

        public SchedulerController() {
            _eventOwnerLogic = new EventOwnerLogic(db);
        }

        private Dictionary<string, ItemId> Ids {
            get {
                _ids = HttpContext.Cache["_ids"] as Dictionary<string, ItemId>;
                if (_ids == null) {
                    _ids = new Dictionary<string, ItemId>(3000);
                    HttpContext.Cache["_ids"] = _ids;
                }
                return _ids;
            }
            set {
                HttpContext.Cache["_ids"] = value;
            }
        }
        public ActionResult GetOwners() {
            var owners = db.EventOwners.Select(n =>
                new {
                    text = n.OwnerName,
                    value = n.EventOwnerID,
                    color = n.Color
                }).ToList();
            return Json(owners, JsonRequestBehavior.AllowGet);
        }

    
        public JsonResult Read() {
            var service = App.ExchangeServiceAccess;
            var meetings = service.GetCalendarItems(DateTime.Now.AddMonths(-2), DateTime.Now.AddMonths(2));
            var owners = db.EventOwners.ToList();
            int i = 0;
            var ids = Ids;
            var adminOwner = owners.FirstOrDefault(owner => owner.Email == App.Email);
            var kendoViewModels = meetings.ToList();
            //var isoJson = JsonConvert.SerializeObject(kendoViewModels);
            return Json(kendoViewModels, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Update(KendoSchedulerViewModel model) {
            var service = App.ExchangeServiceAccess;
            var ids = Ids;
            var id = ids[model.TaskID];
            string[] attendees = _eventOwnerLogic.GetAttendees(model);
            service.UpdateAppointment(model, id, attendees);
            return Json(model, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Destroy(KendoSchedulerViewModel model) {
            var service = App.ExchangeServiceAccess;
            var ids = Ids;
            var id = ids[model.TaskID];
            //var result = service.DestroyAppointment(model, id);
            service.DestroyAppointment(model, id);
            return Json(model, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Create(KendoSchedulerViewModel model) {
            var owner = db.EventOwners.Find(model.OwnerID);
            if (owner != null) {
                var service = App.ExchangeServiceAccess;
                string[] attendees = _eventOwnerLogic.GetAttendees(model);
                var createdAppointment = service.WriteEventInCalendar(
                     model.Title,
                     model.Description,
                     model.Start,
                     model.End,
                     isAllDay: model.IsAllDay,
                     attendees: attendees);
                model.TaskID = createdAppointment.Id.UniqueId;
            }
            return Json(model, JsonRequestBehavior.AllowGet);
        }
        public ActionResult Index() {

            return View("ListEvents");
        }
    }
}