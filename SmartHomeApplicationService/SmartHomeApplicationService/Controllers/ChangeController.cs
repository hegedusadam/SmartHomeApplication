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
	public class ChangeController : Controller
	{
		private SmartHomeApplicationDatabase db = new SmartHomeApplicationDatabase();
		private IHubContext lampContext = GlobalHost.ConnectionManager.GetHubContext<LampHub>();

		// GET: Change
		public string Index()
		{
			return JsonConvert.SerializeObject(db.Changes.ToList());
		}


		[HttpPost, ActionName("AddChange")]
		public void AddNewStateChange([Bind(Include = "Guid, IsOn, date")] LampState LampState)
		{
			db.Changes.Add(new Change
			{
				state = LampState.IsOn,
				date = LampState.date,
				lampid = db.Lamps.Where(l => l.lampguid.Trim() == LampState.Guid).FirstOrDefault().Id
			});
			db.SaveChanges();

			lampContext.Clients.All.ChangeAdded(LampState.Guid);
		}

		[HttpPost, ActionName("DeleteChanges")]
		public string DeleteChangesByGuid(string lampGuid)
		{
            try
            {
                int lampId = db.Lamps.Where(l => l.lampguid.Trim() == lampGuid).FirstOrDefault().Id;
                db.Changes.RemoveRange(db.Changes.Where(c => c.lampid == lampId));

                db.SaveChanges();

                lampContext.Clients.All.ChangesDeleted(lampGuid);

                return JsonConvert.SerializeObject(new HttpStatusCodeResult(HttpStatusCode.OK));
            }
            catch (Exception e)
            {
                return JsonConvert.SerializeObject(new HttpStatusCodeResult(HttpStatusCode.InternalServerError));
            }
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
