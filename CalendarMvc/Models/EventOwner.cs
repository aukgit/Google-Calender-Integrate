using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CalendarMvc.Models {
    public class EventOwner {
        public int EventOwnerID { get; set; }
        public string Email { get; set; }
        public string OwnerName { get; set; }
        public string Color { get; set; }
    }
}