using System;
using System.Collections.Generic;
using CalendarMvc.Models.ViewModel;
using Microsoft.OData.ProxyExtensions;
using Microsoft.Office365.OutlookServices;

namespace CalendarMvc.Modules.Extensions {
    public static class ExtensionsToTransformKendoViewModel {
        public static IList<KendoSchedulerViewModel> TransformToKendoEvents(this IPagedCollection<IEvent> events) {

            var currentPage = events.CurrentPage;

            var list = new List<KendoSchedulerViewModel>(currentPage.Count + 2);


            foreach (var _event in currentPage) {
                var kendoViewModel = new KendoSchedulerViewModel();
                kendoViewModel.Description = _event.Body.ToString();
                kendoViewModel.Start = DateTime.Parse(_event.Start.DateTime);
                kendoViewModel.End = DateTime.Parse(_event.End.DateTime);
                kendoViewModel.Title = _event.Subject;
                kendoViewModel.IsAllDay = _event.IsAllDay.HasValue ? _event.IsAllDay.Value : false;
                kendoViewModel.RecurrenceId = null;
                kendoViewModel.RecurrenceRule = null;
                kendoViewModel.RecurrenceException =null;
                kendoViewModel.StartTimezone = _event.OriginalStartTimeZone;
                kendoViewModel.EndTimezone = _event.OriginalEndTimeZone;

                list.Add(kendoViewModel);
            }
            return list;
        }
    }

}