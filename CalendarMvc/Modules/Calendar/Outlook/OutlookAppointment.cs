using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.Exchange.WebServices.Data;

namespace CalendarMvc.Modules.Calendar.Outlook {
    public class OutlookAppointment {
        /// <summary>
        /// From email
        /// </summary>
        public string Email { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
        public AttendeeCollection Recipients { get; set; }
        public Item Meeting { get; set; }
    }
}