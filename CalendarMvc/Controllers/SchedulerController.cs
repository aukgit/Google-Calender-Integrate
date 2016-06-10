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
        private Dictionary<int, ItemId> _ids;
        private ApplicationDbContext db = new ApplicationDbContext();
        private EventOwnerLogic _eventOwnerLogic;

        public SchedulerController() {
            _eventOwnerLogic = new EventOwnerLogic(db);
        }

        private Dictionary<int, ItemId> Ids {
            get {
                _ids = HttpContext.Cache["_ids"] as Dictionary<int, ItemId>;
                if (_ids == null) {
                    _ids = new Dictionary<int, ItemId>(3000);
                    HttpContext.Cache["_ids"] = _ids;
                }
                return _ids;
            }
            set {
                HttpContext.Cache["_ids"] = value;
            }
        }

        private void SaveTaskId(Appointment appointment) {
            var id = appointment.Id;
            var hashCode = id.UniqueId.GetHashCode();
            Ids[hashCode] = id;
        }


        public ActionResult GetOwners() {
            var owners = db.EventOwners.Select(n =>
                new {
                    text = n.OwnerName,
                    value = n.EventOwnerID,
                    color = n.Color,
                    id = n.EventOwnerID,
                    timezone = n.Timezone,
                    time = n.Time,
                    email = n.Email
                }).ToList();
            return Json(owners, JsonRequestBehavior.AllowGet);
        }


        public JsonResult Read() {
            var service = App.ExchangeServiceAccess;
            var owners = db.EventOwners.ToList();
            var ids = Ids;
            var kendoViewModels = service.GetEventsAsKendoSchedulerViewModel(owners, ids: ids);
            Ids = ids;
            //var data = GetGeneratedSampleData(500);
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
            var id = Ids[model.TaskID];
            //var result = service.DestroyAppointment(model, id);
            service.DestroyAppointment(id);
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
                SaveTaskId(createdAppointment);
            }
            return Json(model, JsonRequestBehavior.AllowGet);
        }
        public ActionResult Index() {

            return View("ListEvents");
        }

        public List<KendoSchedulerViewModel> GetGeneratedSampleData(int number) {
            var list = new List<KendoSchedulerViewModel>(number);
            Random rnd = new Random();
            for (int i = 0; i < number; i++) {
                var item = new KendoSchedulerViewModel();
                int testNum = rnd.Next(1, 101);

                var start = DateTime.Now;
                var division = Math.Ceiling(testNum / 20.0) + 2;
                var rand2 = ((int)division ^ testNum);
                var rand3 = division + testNum;
                var rand4 = rand2 + rand3 + division + i;
                start = start.AddHours(testNum * division + i + rand4 + rand2);
                item.Start = start;
                item.End = start.AddMinutes(testNum * 2 + division + rand3 + i);
                if (testNum <= 33) {
                    item.Title = "Busy : " + i;
                    item.Email = "akarim@relisource.com";
                    item.Description = "Rater is Busy now";
                    item.OwnerID = rnd.Next(1, 5);
                    item.TaskID = i;
                    item.IsAllDay = false;
                } else if (testNum <= 66) {
                    item.Title = "Holiday : " + i;
                    item.Email = "afrahman@relisource.com";
                    item.Description = "Today is holiyday";
                    item.OwnerID = rnd.Next(1, 5);
                    item.TaskID = i;
                    item.IsAllDay = false;
                } else {
                    item.Title = "Working : " + i;
                    item.Email = "morahman@relisource.com";
                    item.Description = "Rater is working";
                    item.OwnerID = rnd.Next(1, 5);
                    item.TaskID = i;
                    item.IsAllDay = false;
                }
                item.Color = GetColorForTestKendo(testNum);
                item.BorderClass = GetIsBorderForTestKendo(testNum);
                list.Add(item);
            }
            return list;
        }

        public string GetColorForTestKendo(int testNum) {
            if (testNum <= 33) {
                return "blue";
            } else if (testNum <= 66) {
                return "orange";
            } else {
                return "violet";
            }
        }

        public string GetIsBorderForTestKendo(int num) {
            if (num > 50 && num < 60) {
                return "red-border";
            } else {
                return "";
            }
        }
    }
}