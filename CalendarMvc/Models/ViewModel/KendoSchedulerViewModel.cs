using System;

namespace CalendarMvc.Models.ViewModel {
    public class KendoSchedulerViewModel {
        public int TaskID { get; set; }
        public int OwnerID { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public TimeZone StartTimezone { get; set; }
        public DateTime Start { get; set; }
        public DateTime End { get; set; }
        public TimeZone EndTimezone { get; set; }        
        public bool IsAllDay { get; set; }
        public string Email { get; set; }
        public string RecurrenceID { get; set; }
        public string RecurrenceRule { get; set; }
        public string RecurrenceException { get; set; }
    }
}