using System;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Linq;
using CalenderMvc.Models;


namespace CalenderMvc.Controllers {
    public class SchedulerController : Controller {

        private static string[] scopes = { "https://outlook.office.com/mail.read" };


        [HttpPost]
        public ActionResult Index(string userEmailAdress) {

            return View("ListEvents");
        }
    }
}