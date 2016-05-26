using System;
using System.Collections.Generic;
using System.Net;
using Microsoft.Exchange.WebServices.Data;

namespace CalendarMvc.Modules.Calendar.Outlook {
    public class ExchangeServiceAccess {
        private readonly string _email;
        private readonly string _password;
        private ExchangeService _service;

        private bool _isServiceCredentialsAreSet;

        /// <summary>
        /// Default version : ExchangeVersion.Exchange2013_SP1
        /// </summary>
        /// <param name="email"></param>
        /// <param name="password"></param>
        public ExchangeServiceAccess(string email, string password) : this(email, password, ExchangeVersion.Exchange2013_SP1) { }

        public ExchangeServiceAccess(string email, string password, ExchangeVersion version) {
            _email = email;
            _password = password;
            _service = new ExchangeService(version);
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
        /// Finds recurring master calendar items, occurrences and exceptions for recurring calendar items,
        /// and single occurrence calendar items by using the FindItem operation.
        /// </summary>
        /// <param name="queryFilter">Give search string</param>
        private FindItemsResults<Item> GetCalendarItems(int possibleItems = 100, string queryFilter = null) {
            // Specify a view that returns up to five recurring master items.
            ItemView view = new ItemView(possibleItems);

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
        /// Finds recurring master calendar items, occurrences and exceptions for recurring calendar items,
        /// and single occurrence calendar items by using the FindItem operation.
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <param name="queryFilter"></param>
        private FindItemsResults<Item> GetCalendarItems(DateTime start, DateTime end) {
            // Specify a view that returns up to five recurring master items.
            // Specify a calendar view for returning instances of a recurring series.
            CalendarView calView = new CalendarView(start, end);

            try {
                // Find all the appointments in the calendar based on the dates set in the CalendarView.
                // This results in a FindItem call to EWS. This will return the occurrences and exceptions
                // to a recurring series and will return appointments that are not part of a recurring series. This will not return 
                // recurring master items. Note that a search restriction or querystring cannot be used with a CalendarView.
                return _service.FindItems(WellKnownFolderName.Calendar, calView);
            } catch (Exception ex) {
                Console.WriteLine("Error: " + ex.Message);
            }
            return null;
        }
        static Dictionary<string, Folder> GetCalendarFolders(ExchangeService service, String searchIn = "My Calendars") {
            Dictionary<String, Folder> rtList = new Dictionary<string, Folder>();

            FolderId rfRootFolderid = new FolderId(WellKnownFolderName.Root);//, mbMailboxname
            FolderView fvFolderView = new FolderView(1000);
            SearchFilter sfSearchFilter = new SearchFilter.IsEqualTo(FolderSchema.DisplayName, "Common Views");
            FindFoldersResults ffoldres = service.FindFolders(rfRootFolderid, sfSearchFilter, fvFolderView);
            if (ffoldres.Folders.Count == 1) {

                PropertySet psPropset = new PropertySet(BasePropertySet.FirstClassProperties);
                ExtendedPropertyDefinition PidTagWlinkAddressBookEID = new ExtendedPropertyDefinition(0x6854, MapiPropertyType.Binary);
                ExtendedPropertyDefinition PidTagWlinkFolderType = new ExtendedPropertyDefinition(0x684F, MapiPropertyType.Binary);
                ExtendedPropertyDefinition PidTagWlinkGroupName = new ExtendedPropertyDefinition(0x6851, MapiPropertyType.String);

                psPropset.Add(PidTagWlinkAddressBookEID);
                psPropset.Add(PidTagWlinkFolderType);
                ItemView iv = new ItemView(1000);
                iv.PropertySet = psPropset;
                iv.Traversal = ItemTraversal.Associated;

                SearchFilter cntSearch = new SearchFilter.IsEqualTo(PidTagWlinkGroupName, searchIn);
                FindItemsResults<Item> fiResults = ffoldres.Folders[0].FindItems(cntSearch, iv);
                foreach (Item itItem in fiResults.Items) {
                    try {
                        object GroupName = null;
                        object WlinkAddressBookEID = null;
                        if (itItem.TryGetProperty(PidTagWlinkAddressBookEID, out WlinkAddressBookEID)) {

                            byte[] ssStoreID = (byte[]) WlinkAddressBookEID;
                            int leLegDnStart = 0;
                            String lnLegDN = "";
                            for (int ssArraynum = (ssStoreID.Length - 2); ssArraynum != 0; ssArraynum--) {
                                if (ssStoreID[ssArraynum] == 0) {
                                    leLegDnStart = ssArraynum;
                                    lnLegDN = System.Text.Encoding.ASCII.GetString(ssStoreID, leLegDnStart + 1, (ssStoreID.Length - (leLegDnStart + 2)));
                                    ssArraynum = 1;
                                }
                            }
                            NameResolutionCollection ncCol = service.ResolveName(lnLegDN, ResolveNameSearchLocation.DirectoryOnly, true);
                            if (ncCol.Count > 0) {

                                FolderId SharedCalendarId = new FolderId(WellKnownFolderName.Calendar, ncCol[0].Mailbox.Address);
                                Folder SharedCalendaFolder = Folder.Bind(service, SharedCalendarId);
                                rtList.Add(ncCol[0].Mailbox.Address, SharedCalendaFolder);


                            }

                        }
                    } catch (Exception exception) {
                        Console.WriteLine(exception.Message);
                    }

                }
            }
            return rtList;
        }
        static Dictionary<string, Folder> GetSharedCalendarFolders(ExchangeService service, String searchIn = "My Calendars") {
            Dictionary<String, Folder> rtList = new Dictionary<string, Folder>();

            FolderId rfRootFolderid = new FolderId(WellKnownFolderName.Root);//, mbMailboxname
            FolderView fvFolderView = new FolderView(1000);
            SearchFilter sfSearchFilter = new SearchFilter.IsEqualTo(FolderSchema.DisplayName, "Common Views");
            FindFoldersResults ffoldres = service.FindFolders(rfRootFolderid, sfSearchFilter, fvFolderView);
            if (ffoldres.Folders.Count == 1) {

                PropertySet psPropset = new PropertySet(BasePropertySet.FirstClassProperties);
                ExtendedPropertyDefinition PidTagWlinkAddressBookEID = new ExtendedPropertyDefinition(0x6854, MapiPropertyType.Binary);
                ExtendedPropertyDefinition PidTagWlinkFolderType = new ExtendedPropertyDefinition(0x684F, MapiPropertyType.Binary);
                ExtendedPropertyDefinition PidTagWlinkGroupName = new ExtendedPropertyDefinition(0x6851, MapiPropertyType.String);

                psPropset.Add(PidTagWlinkAddressBookEID);
                psPropset.Add(PidTagWlinkFolderType);
                ItemView iv = new ItemView(1000);
                iv.PropertySet = psPropset;
                iv.Traversal = ItemTraversal.Associated;

                SearchFilter cntSearch = new SearchFilter.IsEqualTo(PidTagWlinkGroupName, searchIn);
                FindItemsResults<Item> fiResults = ffoldres.Folders[0].FindItems(cntSearch, iv);
                foreach (Item itItem in fiResults.Items) {
                    try {
                        object GroupName = null;
                        object WlinkAddressBookEID = null;
                        if (itItem.TryGetProperty(PidTagWlinkAddressBookEID, out WlinkAddressBookEID)) {

                            byte[] ssStoreID = (byte[]) WlinkAddressBookEID;
                            int leLegDnStart = 0;
                            String lnLegDN = "";
                            for (int ssArraynum = (ssStoreID.Length - 2); ssArraynum != 0; ssArraynum--) {
                                if (ssStoreID[ssArraynum] == 0) {
                                    leLegDnStart = ssArraynum;
                                    lnLegDN = System.Text.Encoding.ASCII.GetString(ssStoreID, leLegDnStart + 1, (ssStoreID.Length - (leLegDnStart + 2)));
                                    ssArraynum = 1;
                                }
                            }
                            NameResolutionCollection ncCol = service.ResolveName(lnLegDN, ResolveNameSearchLocation.DirectoryOnly, true);
                            if (ncCol.Count > 0) {

                                FolderId SharedCalendarId = new FolderId(WellKnownFolderName.Calendar, ncCol[0].Mailbox.Address);
                                Folder SharedCalendaFolder = Folder.Bind(service, SharedCalendarId);
                                rtList.Add(ncCol[0].Mailbox.Address, SharedCalendaFolder);


                            }

                        }
                    } catch (Exception exception) {
                        Console.WriteLine(exception.Message);
                    }

                }
            }
            return rtList;
        }

        public Appointment WriteEventInCalendar(
            string title,
            string body,
            DateTime start,
            DateTime end,
            string location = "",
            DateTime? reminderDueDate = null,
            int remindBeforeMins = 60,
            string[] attendees = null) {
            Appointment meeting = new Appointment(_service);

            // Set the properties on the meeting object to create the meeting.
            meeting.Subject = title;
            meeting.Body = body;
            meeting.Start = start;
            meeting.End = end;
            meeting.Location = location;
            if (attendees != null) {
                foreach (var attendee in attendees) {
                    meeting.RequiredAttendees.Add(attendee);
                }
            }
            if (reminderDueDate.HasValue) {
                meeting.ReminderDueBy = reminderDueDate.Value;
            }
            meeting.ReminderMinutesBeforeStart = remindBeforeMins;

            // Save the meeting to the Calendar folder and send the meeting request.
            meeting.Save(SendInvitationsMode.SendToAllAndSaveCopy);

            return meeting;
        }
    }

}