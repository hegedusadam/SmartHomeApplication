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
		private SmartHomeApplicationDatabase db = new SmartHomeApplicationDatabase();

		// GET: Change
		public ActionResult Index()
        {
            return this.Json(db.Changes.ToList(), JsonRequestBehavior.AllowGet);
		}


		[HttpPost, ActionName("AddChange")]
		public void AddNewStateChange([Bind(Include = "Guid, IsOn, date")] LampState LampState)
		{
			try
			{
				db.Changes.Add(new Change
				{
					state = LampState.IsOn,
					date = LampState.date,
					lampid = db.Lamps.Where(l => l.lampguid.Trim() == LampState.Guid).FirstOrDefault().Id
				});
				db.SaveChanges();
			}
			catch (Exception e)
			{
				
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
