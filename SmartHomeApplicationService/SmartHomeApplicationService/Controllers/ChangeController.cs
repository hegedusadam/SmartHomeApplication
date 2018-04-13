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


		[HttpPost, ActionName("AddChange")]
		public void AddNewStateChange([Bind(Include = "Guid, IsOn, date")] LampState LampState)
		{
			db.Changes.Add(new Change
			{
				state = LampState.IsOn,
				date = LampState.date,
				lampid = db.Lamps.Find(LampState.Guid).Id
			});
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
