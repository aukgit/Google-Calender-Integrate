using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CalendarMvc.Models;

namespace CalendarMvc.BusinessLogic {
    public class Logic {
        protected ApplicationDbContext db;
        public Logic(ApplicationDbContext db) {
            this.db = db;
        }


    }
}