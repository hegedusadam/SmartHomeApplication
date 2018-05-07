using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.SignalR;
using Newtonsoft.Json;
using SmartHomeApplicationService.Hubs;
using SmartHomeApplicationService.Models;

namespace SmartHomeApplicationService.Controllers
{
	public class LampController : Controller
	{
		private SmartHomeApplicationDb db = new SmartHomeApplicationDb();
		private IHubContext lampContext = GlobalHost.ConnectionManager.GetHubContext<LampHub>();

		[System.Web.Mvc.Authorize]
		[HttpPost, ActionName("AddUserToLamp")]
		public string AddUserLamp(NewLamp newLamp)
		{
            try
            {
                Lamp lamp = db.Lamps.Where(l => l.lampguid.Trim() == newLamp.LampGuid).FirstOrDefault();

                User user = db.Users.Find(newLamp.UserId);
                lamp.Users.Add(user);

                db.SaveChanges();

                return JsonConvert.SerializeObject(new HttpStatusCodeResult(HttpStatusCode.OK));
            }
            catch (Exception e)
            {
                return JsonConvert.SerializeObject(new HttpStatusCodeResult(HttpStatusCode.InternalServerError));
            }
		}

		[System.Web.Mvc.Authorize]
		[HttpPost, ActionName("DeleteUserFromLamp")]
		public string DeleteUserFromLamp(UserInfo userInfo)
		{
            try
            {
                Lamp lamp = db.Lamps.Where(l => l.lampguid.Trim() == userInfo.lampGuid).FirstOrDefault();


                User user = db.Users.Find(userInfo.userId);
                lamp.Users.Remove(user);

                db.SaveChanges();

                return JsonConvert.SerializeObject(new HttpStatusCodeResult(HttpStatusCode.OK));
            }
            catch (Exception e)
            {
                return JsonConvert.SerializeObject(new HttpStatusCodeResult(HttpStatusCode.InternalServerError));
            }
		}

		[HttpPost, ActionName("RegisterDevice")]
		public ActionResult RegisterDevice(string Guid)
		{
			Lamp newLamp = db.Lamps.Add(new Lamp
			{
				lampguid = Guid,
				ison = false
			});

			db.SaveChanges();

			return new HttpStatusCodeResult(HttpStatusCode.OK);
		}

		[System.Web.Mvc.Authorize]
		[HttpPost, ActionName("GetLampState")]
		public string GetActualLampState(string guid)
		{
			Lamp lamp = db.Lamps.Where(l => l.lampguid == guid).FirstOrDefault();
			lampContext.Clients.All.SendLampId(lamp.ison);

			return JsonConvert.SerializeObject(lamp.ison);
		}

		[System.Web.Mvc.Authorize]
		[HttpPost, ActionName("TurnLamp")]
		public ActionResult TurnLampOnOrOff(bool TurnOn, string LampGuid)
		{
			LampHub.TurnOn = TurnOn;
			lampContext.Clients.All.OnSwitch(TurnOn, LampGuid);

			return new HttpStatusCodeResult(HttpStatusCode.OK);
		}

		[System.Web.Mvc.Authorize]
		[HttpPost, ActionName("GetChanges")]
		public string GetChangesByGuid(string guid)
		{
			Lamp lamp = db.Lamps.Where(l => l.lampguid.Trim() == guid).FirstOrDefault();

			return JsonConvert.SerializeObject(lamp.Changes);
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
