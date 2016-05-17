using System.Collections.Generic;

namespace CalendarMvc.Models.ViewModel {
    public class OutlookTokenViewModel : OutlookToken {
        public IReadOnlyList<DisplayMessage> Messages { get; set; }
        public IReadOnlyList<DisplayEvent> Events { get; set; }
    }
}