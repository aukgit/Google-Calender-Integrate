using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using CalendarMvc.Models;

namespace CalendarMvc.Controllers {
    public class OutlookTokensController : Controller {
        private readonly ApplicationDbContext db = new ApplicationDbContext();

        // GET: OutlookTokens
        public ActionResult Index() {
            return View(db.OutlookTokens.ToList());
        }

        // GET: OutlookTokens/Details/5
        public ActionResult Details(int? id) {
            if (id == null) {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var outlookToken = db.OutlookTokens.Find(id);
            if (outlookToken == null) {
                return HttpNotFound();
            }
            return View(outlookToken);
        }

        // GET: OutlookTokens/Create
        public ActionResult Create() {
            return View();
        }

        // POST: OutlookTokens/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "OutlookTokenID,Token,Email")] OutlookToken outlookToken) {
            if (ModelState.IsValid) {
                db.OutlookTokens.Add(outlookToken);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(outlookToken);
        }

        // GET: OutlookTokens/Edit/5
        public ActionResult Edit(int? id) {
            if (id == null) {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var outlookToken = db.OutlookTokens.Find(id);
            if (outlookToken == null) {
                return HttpNotFound();
            }
            return View(outlookToken);
        }

        // POST: OutlookTokens/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "OutlookTokenID,Token,Email")] OutlookToken outlookToken) {
            if (ModelState.IsValid) {
                db.Entry(outlookToken).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(outlookToken);
        }

        // GET: OutlookTokens/Delete/5
        public ActionResult Delete(int? id) {
            if (id == null) {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var outlookToken = db.OutlookTokens.Find(id);
            if (outlookToken == null) {
                return HttpNotFound();
            }
            return View(outlookToken);
        }

        // POST: OutlookTokens/Delete/5
        [HttpPost]
        [ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id) {
            var outlookToken = db.OutlookTokens.Find(id);
            db.OutlookTokens.Remove(outlookToken);
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