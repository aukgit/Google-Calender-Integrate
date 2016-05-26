using System;
using System.ComponentModel.DataAnnotations;

namespace CalendarMvc.Models.ViewModel {
    public class AppointmentViewModel {
        public string Title { get; set; }
        public string Subject { get; set; }
        [DataType(DataType.MultilineText)]
        public string Body { get; set; }
        public DateTime Start { get; set; }
        public DateTime End { get; set; }

        [Display(Name = "Remind Before Minutes")]
        public int RemindBeforeMins { get; set; }

        [Display(Name = "Attendees (use csv)")]
        public string Attendees { get; set; }
    }
}