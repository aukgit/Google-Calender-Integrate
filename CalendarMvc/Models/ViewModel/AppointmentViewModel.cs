using System;
using System.ComponentModel.DataAnnotations;

namespace CalendarMvc.Models.ViewModel {
    public class AppointmentViewModel {
        [Required]
        public string Title { get; set; }
        [DataType(DataType.MultilineText)]
        public string Body { get; set; }
        [Required]
        public DateTime Start { get; set; }
        [Required]
        public DateTime End { get; set; }
        public string Location { get; set; }

        public DateTime? ReminderDueDate { get; set; }

        [Display(Name = "Remind Before Minutes")]
        public int RemindBeforeMins { get; set; }

        [Display(Name = "Attendees (use csv)")]
        public string Attendees { get; set; }
    }
}