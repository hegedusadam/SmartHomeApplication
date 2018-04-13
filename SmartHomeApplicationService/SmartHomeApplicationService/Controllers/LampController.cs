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
		private IHubContext lampContext = GlobalHost.ConnectionManager.GetHubContext<LampHub>();

		// GET: Lamps
		public ActionResult Index()
        {
            return this.Json(db.Lamps.ToList(), JsonRequestBehavior.AllowGet);
		}

		[System.Web.Mvc.Authorize]
		[HttpPost, ActionName("AddLamp")]
		public ActionResult AddLamp(NewLamp lamp)
		{
			try
			{
				Lamp newLamp = db.Lamps.Add(new Lamp
				{
					name = lamp.LampName,
					userid = lamp.UserId,
					lampguid = lamp.LampGuid,
					ison = false
				});

				//db.SaveChanges();

				User user = db.Users.Find(lamp.UserId);
				user.Lamp = newLamp;

				db.SaveChanges();
			} catch(Exception e)
			{
				return new HttpStatusCodeResult(HttpStatusCode.InternalServerError);
			}

			return new HttpStatusCodeResult(HttpStatusCode.OK);
		}

		[System.Web.Mvc.Authorize]
		[HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {
            Lamp lamp = db.Lamps.Find(id);
            db.Lamps.Remove(lamp);
            db.SaveChanges();
			return new HttpStatusCodeResult(HttpStatusCode.OK);
		}

		[HttpPost, ActionName("GetLampState")]
		public ActionResult GetActualLampState(string guid)
		{
			try
			{
				Lamp lamp = db.Lamps.Where(l => l.lampguid == guid).FirstOrDefault();
				lampContext.Clients.All.SendLampId(lamp.ison);
				return this.Json(lamp.ison, JsonRequestBehavior.AllowGet);
			}
			catch (Exception e)
			{
				return null;
			}
		}

		[System.Web.Mvc.Authorize]
		[HttpPost, ActionName("TurnLamp")]
		public ActionResult TurnLampOnOrOff(bool TurnOn, string LampGuid)
		{
			LampHub.TurnOn = TurnOn;
			lampContext.Clients.All.OnSwitch(TurnOn, LampGuid);

			return new HttpStatusCodeResult(HttpStatusCode.OK);
		}

		[HttpPut, ActionName("UpdateLamp")]
		public ActionResult ChangeLampCondition([Bind(Include = "Guid, IsOn")] LampState LampState)
		{
			Lamp lamp = db.Lamps.Where(l => l.name == LampState.Guid).FirstOrDefault();
			lamp.ison = LampState.IsOn;

			db.SaveChanges();

			lampContext.Clients.All.SwitchClient(LampState.IsOn, LampState.Guid);
			return new HttpStatusCodeResult(HttpStatusCode.OK);
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
