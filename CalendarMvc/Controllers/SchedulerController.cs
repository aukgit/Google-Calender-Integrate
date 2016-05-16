﻿using System.Web.Mvc;

namespace CalendarMvc.Controllers {
    public class SchedulerController : Controller {

        private static string[] scopes = { "https://outlook.office.com/mail.read" };


        [HttpPost]
        public ActionResult Index(string userEmailAdress) {

            return View("ListEvents");
        }
    }
}