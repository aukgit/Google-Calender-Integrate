using System.Web.Mvc;
using CalenderMvc.Modules.GoogleCalender;
namespace CalenderMvc.Controllers {
    public class SchedulerController : Controller {
        // GET: Scheduler
        public ActionResult Index() {
            return View();
        }

        [HttpPost]
        public ActionResult Index(string userEmailAdress) {
            var googleCalender = new GoogleCalenderAccess();
            var events = googleCalender.GetEvents(userEmailAdress, max: 100);
            ViewBag.events = events.Items;
            return View("ListEvents");
        }
    }
}