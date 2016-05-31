using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CalendarMvc.Models;
using Microsoft.Exchange.WebServices.Data;

namespace CalendarMvc.Extensions {
    public static class EventOwnerExtension {
        public static FolderId AsFolderId(this EventOwner eventOwner) {
            var mailBox = new Mailbox(eventOwner.Email, null);
            return new FolderId(WellKnownFolderName.Calendar, mailBox);
        }
    }
}