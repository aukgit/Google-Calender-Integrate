using System;

namespace CalendarMvc.Models.ViewModel {
    public class KendoSchedulerViewModel {
        public int OwnerId { get; set; }
        public string Title { get; set; }
        public DateTime Start { get; set; }
        public DateTime End { get; set; }
        public string Description { get; set; }
        public bool IsAllDay { get; set; }
        public string Email { get; set; }
        public string Recurrence { get; set; }
        public string RecurrenceRule { get; set; }
        public string RecurrenceException { get; set; }
    }
}