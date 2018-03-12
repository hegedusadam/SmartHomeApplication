using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using SmartHomeApplicationService.Models;

namespace SmartHomeApplicationService.Controllers
{
    public class ChangeController : Controller
    {
        private SmartHomeApplicationDatabaseChanges db = new SmartHomeApplicationDatabaseChanges();

        // GET: Change
        public ActionResult Index()
        {
            return this.Json(db.Changes.ToList(), JsonRequestBehavior.AllowGet);
		}


        // POST: Change/Create
        [HttpPost]
        public ActionResult Create([Bind(Include = "Id,date,state,lampid")] Change change)
        {
            if (ModelState.IsValid)
            {
                db.Changes.Add(change);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(change);
        }

		[HttpPost, ActionName("AddChange")]
		public void AddNewStateChange([Bind(Include = "Id, IsOn, date")] LampState LampState)
		{
			db.Changes.Add(new Change { state = LampState.IsOn, date = LampState.date, lampid = LampState.Id });
			db.SaveChanges();
		}

        // GET: Change/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Change change = db.Changes.Find(id);
            if (change == null)
            {
                return HttpNotFound();
            }
            return View(change);
        }

        // POST: Change/Edit/5
        [HttpPost]
        public ActionResult Edit([Bind(Include = "Id,date,state,lampid")] Change change)
        {
            if (ModelState.IsValid)
            {
                db.Entry(change).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(change);
        }

        // POST: Change/Delete/5
        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {
            Change change = db.Changes.Find(id);
            db.Changes.Remove(change);
            db.SaveChanges();

			return this.Json(change , JsonRequestBehavior.AllowGet);
		}

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
