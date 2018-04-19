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
		private SmartHomeApplicationDatabase db = new SmartHomeApplicationDatabase();
		private IHubContext lampContext = GlobalHost.ConnectionManager.GetHubContext<LampHub>();

		// GET: Lamps
		public ActionResult Index()
        {
            return this.Json(db.Lamps.ToList(), JsonRequestBehavior.AllowGet);
		}

		[System.Web.Mvc.Authorize]
		[HttpPost, ActionName("AddUserToLamp")]
		public ActionResult AddUserLamp(NewLamp newLamp)
		{
			try
			{
				Lamp lamp = db.Lamps.Where(l => l.lampguid.Trim() == newLamp.LampGuid).FirstOrDefault();

				lamp.name = newLamp.LampName;

				User user = db.Users.Find(newLamp.UserId);
				lamp.Users.Add(user);

				db.SaveChanges();

			}
			catch (Exception e)
			{
				return new HttpStatusCodeResult(HttpStatusCode.InternalServerError);
			}

			return new HttpStatusCodeResult(HttpStatusCode.OK);
		}

		[System.Web.Mvc.Authorize]
		[HttpPost, ActionName("DeleteUserFromLamp")]
		public ActionResult DeleteUserFromLamp(UserInfo userInfo)
		{
			try
			{
				Lamp lamp = db.Lamps.Where(l => l.lampguid.Trim() == userInfo.lampGuid).FirstOrDefault();


				User user = db.Users.Find(userInfo.userId);
				lamp.Users.Remove(user);

				db.SaveChanges();
			}
			catch (Exception e)
			{
				return new HttpStatusCodeResult(HttpStatusCode.InternalServerError);
			}

			return new HttpStatusCodeResult(HttpStatusCode.OK);
		}

		[HttpPost, ActionName("RegisterDevice")]
		public ActionResult RegisterDevice(GuidDTO guidDTO)
		{
			try
			{
				Lamp newLamp = db.Lamps.Add(new Lamp
				{
					name = "DefaultLampName",
					lampguid = guidDTO.Guid,
					ison = false
				});

				db.SaveChanges();
			}
			catch (Exception e)
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

		[System.Web.Mvc.Authorize]
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
			try
			{
				LampHub.TurnOn = TurnOn;
				lampContext.Clients.All.OnSwitch(TurnOn, LampGuid);

				return new HttpStatusCodeResult(HttpStatusCode.OK);
			}
			catch (Exception e)
			{
				return null;
			}
		}

		[HttpPut, ActionName("UpdateLamp")]
		public ActionResult ChangeLampCondition([Bind(Include = "Guid, IsOn")] LampState LampState)
		{
			Lamp lamp = db.Lamps.Where(l => l.lampguid.Trim() == LampState.Guid).FirstOrDefault();
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
