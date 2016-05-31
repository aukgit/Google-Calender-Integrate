using CalendarMvc.Models;
using CalendarMvc.Models.ViewModel;
using Microsoft.Exchange.WebServices.Data;

namespace CalendarMvc.BusinessLogic {
    public class EventOwnerLogic : Logic {
        public EventOwnerLogic(ApplicationDbContext db)
            : base(db) {}

        public void AddOrModifyEventOwner(EventOwner owner) {
            
        }


        public string[] GetAttendees(int eventOwnerId) {
            string[] attendees = null;
            var owner = db.EventOwners.Find(eventOwnerId);
            if (owner.Email != App.Email) {
                attendees = new[] { owner.Email };
            }
            return attendees;
        }
        public string[] GetAttendees(KendoSchedulerViewModel model) {
            return GetAttendees(model.OwnerID);
        }

        public FolderId GetFolderId(EventOwner eventOwner) {
            var mailBox = new Mailbox(eventOwner.Email, null);
            return new FolderId(WellKnownFolderName.Calendar, mailBox);
        }

    }
}