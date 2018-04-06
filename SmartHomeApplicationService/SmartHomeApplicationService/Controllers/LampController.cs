using System;
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

		// POST: Lamps/Delete/5
		[System.Web.Mvc.Authorize]
		[HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {
            Lamp lamp = db.Lamps.Find(id);
            db.Lamps.Remove(lamp);
            db.SaveChanges();
			return new HttpStatusCodeResult(HttpStatusCode.OK);
		}

		[HttpGet, ActionName("GetLampId")]
		public ActionResult GetActualLampId()
		{
			var lampId = 1;

			LampHub.LampId = lampId;
			lampContext.Clients.All.SendLampId(lampId);
			return new HttpStatusCodeResult(HttpStatusCode.OK);
		}

		[HttpGet, ActionName("GetLampState")]
		public ActionResult GetActualLampState()
		{
			var lampId = 1;

			LampHub.LampId = lampId;
			lampContext.Clients.All.SendLampId(lampId);
			return this.Json(db.Lamps.Find(lampId).ison, JsonRequestBehavior.AllowGet);
		}

		[System.Web.Mvc.Authorize]
		[HttpPost, ActionName("TurnLamp")]
		public ActionResult TurnLampOnOrOff(bool TurnOn)
		{
			LampHub.TurnOn = TurnOn;
			lampContext.Clients.All.OnSwitch(TurnOn);

			return new HttpStatusCodeResult(HttpStatusCode.OK);
		}

		[HttpPut, ActionName("UpdateLamp")]
		public ActionResult ChangeLampCondition([Bind(Include = "Id, IsOn")] LampState LampState)
		{
			db.Lamps.Find(LampState.Id).ison = LampState.IsOn;
			db.SaveChanges();

			lampContext.Clients.All.SwitchClient(LampState.IsOn);
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
