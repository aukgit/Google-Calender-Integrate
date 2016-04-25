
using System.Web.Mvc;
using CalenderMvc.Modules.GoogleCalender;

namespace CalenderMvc.Controllers
{
    public class SchedulerController : Controller
    {
        // GET: Scheduler
        public ActionResult Index() {
            var googleCalender = new GoogleCalenderAccess();

            var events = googleCalender.GetEvents(max: 100);
            return View(events);
        }
    }
}