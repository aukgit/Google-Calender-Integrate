﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CalenderMvc.Models {
    public class KendoSchedulerViewModel {
        public string Title { get; set; }
        public DateTime Start { get; set; }
        public DateTime End { get; set; }
        public string Description { get; set; }
        public bool IsAllDay { get; set; }
        public string Recurrence { get; set; }
        public string RecurrenceRule { get; set; }
        public string RecurrenceException { get; set; }
    }
}