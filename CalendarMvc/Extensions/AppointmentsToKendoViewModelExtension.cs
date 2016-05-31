using System.Collections.Generic;
using System.Linq;
using CalendarMvc.Models;
using CalendarMvc.Models.ViewModel;
using CalendarMvc.Modules.Calendar.Outlook;
using Microsoft.Exchange.WebServices.Data;

namespace CalendarMvc.Extensions {
    public static class AppointmentsToKendoViewModelExtension {
        public static List<KendoSchedulerViewModel> GetKendoSchedulerViewModel(this List<OutlookAppointment> list, List<EventOwner> eventOwners, Dictionary<int, ItemId> ids) {
            return list.Select(n => {
                var eventOwner = eventOwners.FirstOrDefault(owner => owner.Email == n.Email);
                var result = new KendoSchedulerViewModel();
                if (eventOwner != null) {
                    result.OwnerID = eventOwner.EventOwnerID;
                } else {
                    result.OwnerID = 2; //admin
                }
                var appointment = (Appointment) n.Meeting;
                var id = n.Meeting.Id;
                var hashCode = id.UniqueId.GetHashCode();
                ids[hashCode] = id;
                result.TaskID = hashCode;
                //result.Email = n.Email;
                result.Title = n.Subject;
                result.Description = n.Body;
                result.Start = appointment.Start;
                result.End = appointment.End;
                result.IsAllDay = appointment.IsAllDayEvent;
                return result;
            });
        }
    }
}