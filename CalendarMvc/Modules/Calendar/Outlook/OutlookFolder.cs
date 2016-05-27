using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.Exchange.WebServices.Data;

namespace CalendarMvc.Modules.Calendar.Outlook {
    public class OutlookFolder {
        public FolderId FolderId { get; set; }
        public string Email { get; set; }
        public string DisplayName { get; set; }
        public EmailAddress MailBox { get; set; }
        public Folder Folder { get; set; }
    }
}