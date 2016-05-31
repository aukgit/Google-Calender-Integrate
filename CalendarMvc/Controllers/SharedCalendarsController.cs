using System;
using System.Data.Entity;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using CalendarMvc.Models;

namespace CalendarMvc.Controllers {
    public class SharedCalendarsController : Controller {
        private readonly ApplicationDbContext db = new ApplicationDbContext();
        private static String HexConverter(System.Drawing.Color c) {
            return "#" + c.R.ToString("X2") + c.G.ToString("X2") + c.B.ToString("X2");
        }
        // GET: SharedCalendars
        public ActionResult Index() {
            var service = App.ExchangeServiceAccess;
            var myCalendarSharedFolders = service.GetSharedCalendarFolders();
            var othersSharedFolders = service.GetSharedCalendarFolders("Other calendars");
            var sharedFolders = myCalendarSharedFolders.Concat(othersSharedFolders);
            var eventOwner2 = db.EventOwners.FirstOrDefault(n => n.Email == App.Email);
            if (eventOwner2 == null) {
                eventOwner2 = new EventOwner();
                db.EventOwners.Add(eventOwner2); //insert
            }
            eventOwner2.Email = App.Email;
            eventOwner2.OwnerName = App.EmailDisplay;
            eventOwner2.Color = HexConverter(Color.Red);
            foreach (var folder in sharedFolders) {
                var email = folder.Email;
                var eventOwner = db.EventOwners.FirstOrDefault(n => n.Email == email);
                if (eventOwner == null) {
                    eventOwner = new EventOwner();
                    eventOwner.Color = HexConverter(Color.Blue);
                    db.EventOwners.Add(eventOwner); //insert
                }
                var events = service.GetCalendarItems(folder.FolderId);

                eventOwner.Email = email;
                eventOwner.OwnerName = folder.DisplayName;
            }

            db.SaveChanges();

            var list = db.EventOwners.ToList();
            return View(list);
        }

        // GET: SharedCalendars/Details/5
        public ActionResult Details(int? id) {
            if (id == null) {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var eventOwner = db.EventOwners.Find(id);
            if (eventOwner == null) {
                return HttpNotFound();
            }
            return View(eventOwner);
        }

        // GET: SharedCalendars/Create
        public ActionResult Create() {
            return View();
        }

        // POST: SharedCalendars/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "EventOwnerID,Email,OwnerName,Color")] EventOwner eventOwner) {
            if (ModelState.IsValid) {
                db.EventOwners.Add(eventOwner);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(eventOwner);
        }

        // GET: SharedCalendars/Edit/5
        public ActionResult Edit(int? id) {
            if (id == null) {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var eventOwner = db.EventOwners.Find(id);
            if (eventOwner == null) {
                return HttpNotFound();
            }
            return View(eventOwner);
        }

        // POST: SharedCalendars/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "EventOwnerID,Email,OwnerName,Color")] EventOwner eventOwner) {
            if (ModelState.IsValid) {
                db.Entry(eventOwner).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(eventOwner);
        }

        // GET: SharedCalendars/Delete/5
        public ActionResult Delete(int? id) {
            if (id == null) {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var eventOwner = db.EventOwners.Find(id);
            if (eventOwner == null) {
                return HttpNotFound();
            }
            return View(eventOwner);
        }

        // POST: SharedCalendars/Delete/5
        [HttpPost]
        [ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id) {
            var eventOwner = db.EventOwners.Find(id);
            db.EventOwners.Remove(eventOwner);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing) {
            if (disposing) {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}