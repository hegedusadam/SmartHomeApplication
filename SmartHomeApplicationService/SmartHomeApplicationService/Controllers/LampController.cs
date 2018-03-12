﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.SignalR;
using SmartHomeApplicationService.Hubs;
using SmartHomeApplicationService.Models;

namespace SmartHomeApplicationService.Controllers
{
    public class LampController : Controller
    {
        private SmartHomeApplicationDatabaseLamps db = new SmartHomeApplicationDatabaseLamps();

        // GET: Lamps
        public ActionResult Index()
        {
            return this.Json(db.Lamps.ToList(), JsonRequestBehavior.AllowGet);
		}

        // GET: Lamps/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Lamp lamp = db.Lamps.Find(id);
            if (lamp == null)
            {
                return HttpNotFound();
            }
            return View(lamp);
        }

        // GET: Lamps/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Lamps/Create
        [HttpPost]
        public ActionResult Create([Bind(Include = "Id,name,ison")] Lamp lamp)
        {
            if (ModelState.IsValid)
            {
                db.Lamps.Add(lamp);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(lamp);
        }

        // GET: Lamps/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Lamp lamp = db.Lamps.Find(id);
            if (lamp == null)
            {
                return HttpNotFound();
            }
            return View(lamp);
        }

        // POST: Lamps/Edit/5
        [HttpPost]
        public ActionResult Edit([Bind(Include = "Id,name,ison")] Lamp lamp)
        {
            if (ModelState.IsValid)
            {
                db.Entry(lamp).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(lamp);
        }

        // GET: Lamps/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Lamp lamp = db.Lamps.Find(id);
            if (lamp == null)
            {
                return HttpNotFound();
            }
            return View(lamp);
        }

        // POST: Lamps/Delete/5
        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {
            Lamp lamp = db.Lamps.Find(id);
            db.Lamps.Remove(lamp);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

		[HttpGet, ActionName("GetLampId")]
		public void GetActualLampId()
		{
			var lampId = 1;
			var lampContext = GlobalHost.ConnectionManager.GetHubContext<LampHub>();

			LampHub.LampId = lampId;
			lampContext.Clients.All.SendLampId(lampId);
		}

		[HttpPost, ActionName("TurnLamp")]
		public void TurnLampOnOrOff(bool TurnOn)
		{
			var lampContext = GlobalHost.ConnectionManager.GetHubContext<LampHub>();

			LampHub.TurnOn = TurnOn;
			lampContext.Clients.All.OnSwitch(TurnOn);
		}

		[HttpPut, ActionName("UpdateLamp")]
		public void ChangeLampCondition([Bind(Include = "Id, IsOn")] LampState LampState)
		{
			db.Lamps.Find(LampState.Id).ison = LampState.IsOn;
			db.SaveChanges();
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
