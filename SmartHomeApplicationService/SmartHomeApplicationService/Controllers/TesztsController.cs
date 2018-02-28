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
    public class TesztsController : Controller
    {
        private SmartHomeApplicationDatabaseEntities db = new SmartHomeApplicationDatabaseEntities();

        // GET: Teszts
        public ActionResult Index()
        {
            return View(db.Teszts.ToList());
        }

        // GET: Teszts/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Teszt teszt = db.Teszts.Find(id);
            if (teszt == null)
            {
                return HttpNotFound();
            }
            return View(teszt);
        }

        // GET: Teszts/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Teszts/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,name,szeretiatejet")] Teszt teszt)
        {
            if (ModelState.IsValid)
            {
                db.Teszts.Add(teszt);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(teszt);
        }

        // GET: Teszts/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Teszt teszt = db.Teszts.Find(id);
            if (teszt == null)
            {
                return HttpNotFound();
            }
            return View(teszt);
        }

        // POST: Teszts/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,name,szeretiatejet")] Teszt teszt)
        {
            if (ModelState.IsValid)
            {
                db.Entry(teszt).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(teszt);
        }

        // GET: Teszts/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Teszt teszt = db.Teszts.Find(id);
            if (teszt == null)
            {
                return HttpNotFound();
            }
            return View(teszt);
        }

        // POST: Teszts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Teszt teszt = db.Teszts.Find(id);
            db.Teszts.Remove(teszt);
            db.SaveChanges();
            return RedirectToAction("Index");
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
