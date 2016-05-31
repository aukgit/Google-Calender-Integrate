using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using CalendarMvc.Extensions;
using CalendarMvc.Models;
using CalendarMvc.Models.ViewModel;
using Microsoft.Exchange.WebServices.Data;

namespace CalendarMvc.Modules.Calendar.Outlook {
    [Serializable]
    public class ExchangeServiceAccess {
        private readonly string _email;
        private readonly string _password;

        private bool _isServiceCredentialsAreSet;
        private readonly ExchangeService _service;

        /// <summary>
        ///     Default version : ExchangeVersion.Exchange2013_SP1
        /// </summary>
        /// <param name="email"></param>
        /// <param name="password"></param>
        public ExchangeServiceAccess(string email, string password)
            : this(email, password, ExchangeVersion.Exchange2013_SP1) {}

        public ExchangeServiceAccess(string email, string password, ExchangeVersion version) {
            _email = email;
            _password = password;
            _service = new ExchangeService(version);
            GetConnectedExchangeService();
        }

        private void SetNetworkCredentialsInService() {
            _service.Credentials = new NetworkCredential(_email, _password);
        }

        public Uri RetriveAutoDiscoverableUri() {
            _service.AutodiscoverUrl(_email, RedirectionUrlValidationCallback);
            return _service.Url;
        }

        public ExchangeService GetConnectedExchangeService() {
            if (!_isServiceCredentialsAreSet) {
                SetNetworkCredentialsInService();
                RetriveAutoDiscoverableUri();
                _isServiceCredentialsAreSet = true;
            }
            return _service;
        }

        // The following is a basic redirection validation callback method. It 
        // inspects the redirection URL and only allows the Service object to 
        // follow the redirection link if the URL is using HTTPS. 
        //
        // This redirection URL validation callback provides sufficient security
        // for development and testing of your application. However, it may not
        // provide sufficient security for your deployed application. You should
        // always make sure that the URL validation callback method that you use
        // meets the security requirements of your organization.
        private static bool RedirectionUrlValidationCallback(string redirectionUrl) {
            // The default for the validation callback is to reject the URL.
            var result = false;

            var redirectionUri = new Uri(redirectionUrl);

            // Validate the contents of the redirection URL. In this simple validation
            // callback, the redirection URL is considered valid if it is using HTTPS
            // to encrypt the authentication credentials. 
            if (redirectionUri.Scheme == "https") {
                result = true;
            }

            return result;
        }

        /// <summary>
        ///     Finds recurring master calendar items, occurrences and exceptions for recurring calendar items,
        ///     and single occurrence calendar items by using the FindItem operation.
        /// </summary>
        /// <param name="queryFilter">Give search string</param>
        public FindItemsResults<Item> GetCalendarItems(int possibleItems = 100, string queryFilter = null) {
            // Specify a view that returns up to five recurring master items.
            var view = new ItemView(possibleItems);
            // Specify a calendar view for returning instances of a recurring series.

            try {
                // Find up to the first five recurring master appointments in the calendar with 'Weekly Tennis Lesson' set for the subject property.
                // This results in a FindItem operation call to EWS. This will return the recurring master
                // appointment.
                return _service.FindItems(WellKnownFolderName.Calendar, queryFilter, view);
            } catch (Exception ex) {
                Console.WriteLine("Error: " + ex.Message);
            }
            return null;
        }

        /// <summary>
        ///     Finds recurring master calendar items, occurrences and exceptions for recurring calendar items,
        ///     and single occurrence calendar items by using the FindItem operation.
        /// </summary>
        /// <param name="queryFilter">Give search string</param>
        /// <param name="folderId"></param>
        public FindItemsResults<Item> GetCalendarItems(FolderId folderId, int possibleItems = 100,
            string queryFilter = null) {
            // Specify a view that returns up to five recurring master items.
            var view = new ItemView(possibleItems);
            // Specify a calendar view for returning instances of a recurring series.

            try {
                // Find up to the first five recurring master appointments in the calendar with 'Weekly Tennis Lesson' set for the subject property.
                // This results in a FindItem operation call to EWS. This will return the recurring master
                // appointment.
                return _service.FindItems(folderId, queryFilter, view);
            } catch (Exception ex) {
                Console.WriteLine("Error: " + ex.Message);
            }
            return null;
        }

        /// <summary>
        ///     Finds recurring master calendar items, occurrences and exceptions for recurring calendar items,
        ///     and single occurrence calendar items by using the FindItem operation.
        /// </summary>
        /// <param name="eventOwners"></param>
        /// <param name="possibleItems"></param>
        /// <param name="queryFilter">Give search string</param>
        public List<KendoSchedulerViewModel> GetEventsAsKendoSchedulerViewModel(List<EventOwner> eventOwners,
            int possibleItems = 500, Dictionary<int, ItemId> ids = null, string queryFilter = null) {
            // Specify a view that returns up to five recurring master items.
            var view = new ItemView(possibleItems);
            // Specify a calendar view for returning instances of a recurring series.

            try {
                // Find up to the first five recurring master appointments in the calendar with 'Weekly Tennis Lesson' set for the subject property.
                // This results in a FindItem operation call to EWS. This will return the recurring master
                // appointment.
                var list = new List<KendoSchedulerViewModel>(possibleItems*(eventOwners.Count - 1));
                foreach (var eventOwner in eventOwners) {
                    var folderId = eventOwner.AsFolderId();
                    var meeetings = _service.FindItems(folderId, queryFilter, view);
                    AttachOrganizerProperty(ref meeetings);
                    foreach (var meeting in meeetings) {
                        var m = new KendoSchedulerViewModel();
                        m.Title = meeting.Subject;
                        m.Description = meeting.Body;
                        m.OwnerID = eventOwner.EventOwnerID;
                        var appointment = (Appointment) meeting;
                        if (ids != null) {
                            var id = appointment.Id;
                            var hashCode = id.UniqueId.GetHashCode();
                            ids[hashCode] = id;
                            m.TaskID = hashCode;
                        }
                        m.Email = appointment.Organizer.Address;
                        m.Start = appointment.Start;
                        m.End = appointment.End;
                        m.IsAllDay = appointment.IsAllDayEvent;
                        list.Add(m);
                    }
                }
                return list;
            } catch (Exception ex) {
                Console.WriteLine("Error: " + ex.Message);
            }
            return null;
        }

        public void AttachOrganizerProperty(ref FindItemsResults<Item> events) {
            var itmPropSet = new PropertySet(BasePropertySet.FirstClassProperties);
            itmPropSet.RequestedBodyType = BodyType.Text;
            _service.LoadPropertiesForItems(events, itmPropSet);
        }

        public FolderId GetFolderId(string email) {
            var mailBox = new Mailbox(email, null);
            return new FolderId(WellKnownFolderName.Calendar, mailBox);
        }

        /// <summary>
        ///     Finds recurring master calendar items, occurrences and exceptions for recurring calendar items,
        ///     and single occurrence calendar items by using the FindItem operation.
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        public List<OutlookAppointment> GetCalendarItems(DateTime start, DateTime end) {
            // Specify a view that returns up to five recurring master items.
            // Specify a calendar view for returning instances of a recurring series.
            var calView = new CalendarView(start, end);

            try {
                // Find all the appointments in the calendar based on the dates set in the CalendarView.
                // This results in a FindItem call to EWS. This will return the occurrences and exceptions
                // to a recurring series and will return appointments that are not part of a recurring series. This will not return 
                // recurring master items. Note that a search restriction or querystring cannot be used with a CalendarView.
                //ExtendedPropertyDefinition PidTagSenderSmtpAddress = new ExtendedPropertyDefinition(0x5D01, MapiPropertyType.Binary);
                ////PropertySet psPropset = new PropertySet(BasePropertySet.FirstClassProperties);
                //psPropset.Add(PidTagSenderSmtpAddress);
                //calView.PropertySet = psPropset;
                //calView.Traversal = ItemTraversal.Associated;

                var itmPropSet = new PropertySet(BasePropertySet.FirstClassProperties);
                itmPropSet.RequestedBodyType = BodyType.Text;

                var appointments = _service.FindAppointments(WellKnownFolderName.Calendar, calView);

                _service.LoadPropertiesForItems(from Item item in appointments select item, itmPropSet);

                var list = new List<OutlookAppointment>(appointments.TotalCount + 2);
                object email = null;
                foreach (var appointment in appointments) {
                    var outlookAppointment = new OutlookAppointment();
                    outlookAppointment.Email = appointment.Organizer.Address;
                    outlookAppointment.Recipients = appointment.RequiredAttendees;
                    outlookAppointment.Subject = appointment.Subject;
                    outlookAppointment.Body = appointment.Body.ToString();
                    outlookAppointment.Meeting = appointment;
                    list.Add(outlookAppointment);
                }
                return list;
            } catch (Exception ex) {
                Console.WriteLine("Error: " + ex.Message);
            }
            return null;
        }

        public Dictionary<string, Folder> GetCalendarFolders(string searchIn = "My Calendars") {
            var rtList = new Dictionary<string, Folder>();

            var rfRootFolderid = new FolderId(WellKnownFolderName.Root); //, mbMailboxname
            var fvFolderView = new FolderView(1000);
            SearchFilter sfSearchFilter = new SearchFilter.IsEqualTo(FolderSchema.DisplayName, "Common Views");
            var ffoldres = _service.FindFolders(rfRootFolderid, sfSearchFilter, fvFolderView);
            if (ffoldres.Folders.Count == 1) {
                var psPropset = new PropertySet(BasePropertySet.FirstClassProperties);
                var pidTagWlinkAddressBookEid = new ExtendedPropertyDefinition(0x6854, MapiPropertyType.Binary);
                var pidTagWlinkFolderType = new ExtendedPropertyDefinition(0x684F, MapiPropertyType.Binary);
                var pidTagWlinkGroupName = new ExtendedPropertyDefinition(0x6851, MapiPropertyType.String);

                psPropset.Add(pidTagWlinkAddressBookEid);
                psPropset.Add(pidTagWlinkFolderType);
                var iv = new ItemView(1000);
                iv.PropertySet = psPropset;
                iv.Traversal = ItemTraversal.Associated;

                SearchFilter cntSearch = new SearchFilter.IsEqualTo(pidTagWlinkGroupName, searchIn);
                var fiResults = ffoldres.Folders[0].FindItems(cntSearch, iv);
                foreach (var itItem in fiResults.Items) {
                    try {
                        object groupName = null;
                        object wlinkAddressBookEid = null;
                        if (itItem.TryGetProperty(pidTagWlinkAddressBookEid, out wlinkAddressBookEid)) {
                            var ssStoreId = (byte[]) wlinkAddressBookEid;
                            var leLegDnStart = 0;
                            var lnLegDn = "";
                            for (var ssArraynum = ssStoreId.Length - 2; ssArraynum != 0; ssArraynum--) {
                                if (ssStoreId[ssArraynum] == 0) {
                                    leLegDnStart = ssArraynum;
                                    lnLegDn = Encoding.ASCII.GetString(ssStoreId, leLegDnStart + 1,
                                        ssStoreId.Length - (leLegDnStart + 2));
                                    ssArraynum = 1;
                                }
                            }
                            var ncCol = _service.ResolveName(lnLegDn, ResolveNameSearchLocation.DirectoryOnly, true);
                            if (ncCol.Count > 0) {
                                var sharedCalendarId = new FolderId(WellKnownFolderName.Calendar,
                                    ncCol[0].Mailbox.Address);
                                var sharedCalendaFolder = Folder.Bind(_service, sharedCalendarId);
                                rtList.Add(ncCol[0].Mailbox.Address, sharedCalendaFolder);
                            }
                        }
                    } catch (Exception exception) {
                        Console.WriteLine(exception.Message);
                    }
                }
            }
            return rtList;
        }

        public List<OutlookFolder> GetSharedCalendarFolders(string searchIn = "My Calendars") {
            var list = new List<OutlookFolder>(25);

            var rfRootFolderid = new FolderId(WellKnownFolderName.Root); //, mbMailboxname
            var fvFolderView = new FolderView(1000);
            SearchFilter sfSearchFilter = new SearchFilter.IsEqualTo(FolderSchema.DisplayName, "Common Views");
            var ffoldres = _service.FindFolders(rfRootFolderid, sfSearchFilter, fvFolderView);
            if (ffoldres.Folders.Count == 1) {
                var propertySet = new PropertySet(BasePropertySet.FirstClassProperties);
                var pidTagWlinkAddressBookEid = new ExtendedPropertyDefinition(0x6854, MapiPropertyType.Binary);
                var pidTagWlinkFolderType = new ExtendedPropertyDefinition(0x684F, MapiPropertyType.Binary);
                var pidTagWlinkGroupName = new ExtendedPropertyDefinition(0x6851, MapiPropertyType.String);

                propertySet.Add(pidTagWlinkAddressBookEid);
                propertySet.Add(pidTagWlinkFolderType);
                var itemView = new ItemView(1000);
                itemView.PropertySet = propertySet;
                itemView.Traversal = ItemTraversal.Associated;

                SearchFilter cntSearch = new SearchFilter.IsEqualTo(pidTagWlinkGroupName, searchIn);
                var fiResults = ffoldres.Folders[0].FindItems(cntSearch, itemView);
                foreach (var itItem in fiResults.Items) {
                    try {
                        object wlinkAddressBookEid = null;
                        if (itItem.TryGetProperty(pidTagWlinkAddressBookEid, out wlinkAddressBookEid)) {
                            var ssStoreId = (byte[]) wlinkAddressBookEid;
                            var lnLegDn = "";
                            for (var ssArraynum = ssStoreId.Length - 2; ssArraynum != 0; ssArraynum--) {
                                if (ssStoreId[ssArraynum] == 0) {
                                    var leLegDnStart = ssArraynum;
                                    lnLegDn = Encoding.ASCII.GetString(ssStoreId, leLegDnStart + 1,
                                        ssStoreId.Length - (leLegDnStart + 2));
                                    ssArraynum = 1;
                                }
                            }
                            var ncCol = _service.ResolveName(lnLegDn, ResolveNameSearchLocation.DirectoryOnly, true);
                            if (ncCol.Count > 0) {
                                var outlookFolder = new OutlookFolder();
                                var sharedCalendarId = new FolderId(WellKnownFolderName.Calendar,
                                    ncCol[0].Mailbox.Address);
                                var sharedCalendaFolder = Folder.Bind(_service, sharedCalendarId);
                                outlookFolder.FolderId = sharedCalendarId;
                                outlookFolder.Folder = sharedCalendaFolder;
                                outlookFolder.MailBox = ncCol[0].Mailbox;
                                outlookFolder.Email = outlookFolder.MailBox.Address;
                                outlookFolder.DisplayName = outlookFolder.MailBox.Name;

                                list.Add(outlookFolder);
                            }
                        }
                    } catch (Exception exception) {
                        Console.WriteLine(exception.Message);
                    }
                }
            }
            return list;
        }

        public Appointment WriteEventInCalendar(
            string title,
            string body,
            DateTime start,
            DateTime end,
            string location = "",
            DateTime? reminderDueDate = null,
            int remindBeforeMins = 60,
            bool isAllDay = false,
            string[] attendees = null) {
            var meeting = new Appointment(_service);

            // Set the properties on the meeting object to create the meeting.
            meeting.Subject = title;
            meeting.Body = body;
            meeting.Start = start;
            meeting.End = end;
            meeting.Location = location;
            meeting.IsAllDayEvent = isAllDay;
            AddAttendees(meeting, attendees, true);
            if (reminderDueDate.HasValue) {
                meeting.ReminderDueBy = reminderDueDate.Value;
            }
            meeting.ReminderMinutesBeforeStart = remindBeforeMins;

            // Save the meeting to the Calendar folder and send the meeting request.
            meeting.Save(SendInvitationsMode.SendToAllAndSaveCopy);

            return meeting;
        }

        private void AddAttendees(Appointment appointment, string[] attendees = null, bool isNew = false) {
            if (attendees != null) {
                if (!isNew) {
                    appointment.RequiredAttendees.Clear();
                }
                foreach (var attendee in attendees) {
                    appointment.RequiredAttendees.Add(attendee);
                }
            }
        }

        public KendoSchedulerViewModel UpdateAppointment(KendoSchedulerViewModel m, ItemId id, string[] attendees = null) {
            //Appointment meeting = new Appointment(_service);
            var item = Item.Bind(_service, id, new PropertySet(ItemSchema.Subject));
            var appointment = (Appointment) item;
            appointment.Subject = m.Title;
            appointment.Body = m.Description;
            appointment.Start = m.Start;
            appointment.End = m.End;
            appointment.IsAllDayEvent = m.IsAllDay;
            AddAttendees(appointment, attendees, false);
            //appointment.title
            item.Update(ConflictResolutionMode.AlwaysOverwrite);
            return m;
        }

        public void DestroyAppointment(KendoSchedulerViewModel m, ItemId id) {
            var item = Item.Bind(_service, id, new PropertySet(ItemSchema.Subject));
            var appointment = (Appointment) item;
            appointment.Delete(DeleteMode.MoveToDeletedItems);
        }
    }
}