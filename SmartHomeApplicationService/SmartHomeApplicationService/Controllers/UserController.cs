using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Newtonsoft.Json.Linq;
using SmartHomeApplicationService.Models;

namespace SmartHomeApplicationService.Controllers
{
    public class UserController : Controller
    {
        private SmartHomeApplicationDatabaseUserTable db = new SmartHomeApplicationDatabaseUserTable();

        // GET: Users
        public ActionResult Index()
        {
            return this.Json(db.Users.ToList(), JsonRequestBehavior.AllowGet);
        }

		[HttpPost, ActionName("RegisterOrLogin")]
		public ActionResult RegisterOrLogin(string userId)
		{
			User user = db.Users.Where(u => u.UserProfileId == userId).FirstOrDefault();

			if (user == null)
			{
				db.Users.Add(new User
				{
					FirstName = "Elek",
					LastName = "Mekk",
					UserProfileId = userId
				});

				db.SaveChanges();
			}

			return new HttpStatusCodeResult(HttpStatusCode.OK);
		}

        // GET: Users/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            User user = db.Users.Find(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            return View(user);
        }

        // POST: Users/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        public ActionResult Create([Bind(Include = "Id,FirstName,LastName")] User user)
        {
            if (ModelState.IsValid)
            {
                db.Users.Add(user);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(user);
        }

        // GET: Users/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            User user = db.Users.Find(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            return View(user);
        }

        // POST: Users/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        public ActionResult Edit([Bind(Include = "Id,FirstName,LastName")] User user)
        {
            if (ModelState.IsValid)
            {
                db.Entry(user).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(user);
        }

		[HttpDelete, ActionName("DeleteById")]
		public ActionResult DeleteById(int? id)
		{
			if (id == null)
			{
				return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
			}
			User user = db.Users.Find(id);
			if (user == null)
			{
				return HttpNotFound();
			}

			db.Users.Remove(user);
			db.SaveChanges();
			return new HttpStatusCodeResult(HttpStatusCode.Accepted);
		}

		// GET: Users/Delete/5
		public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            User user = db.Users.Find(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            return View(user);
        }

        // POST: Users/Delete/5
        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {
            User user = db.Users.Find(id);
            db.Users.Remove(user);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

		[HttpGet, ActionName("GetUserInfo")]
		public async Task<JsonResult> GetUserInfo()
		{

			string provider = "";
			string secret;
			string accessToken = GetAccessToken(out provider, out secret);

			UserInfo info = new UserInfo();

			using (HttpClient client = new HttpClient())
			{
				using (
					HttpResponseMessage response =
						await
							client.GetAsync("https://graph.facebook.com/me" + "?access_token=" +
											accessToken))
				{
					var o = JObject.Parse(await response.Content.ReadAsStringAsync());
					info.Name = o["name"].ToString();
				}
				using (
					HttpResponseMessage response =
						await
							client.GetAsync("https://graph.facebook.com/me" +
											"/picture?redirect=false&access_token=" + accessToken))
				{
					var x = JObject.Parse(await response.Content.ReadAsStringAsync());
					info.ImageUri = (x["data"]["url"].ToString());
				}
			}

			return this.Json(info, JsonRequestBehavior.AllowGet);
		}

		private string GetAccessToken(out string provider, out string secret)
		{
			var serviceUser = this.User as ClaimsPrincipal;
			var ident = serviceUser.FindFirst("http://schemas.microsoft.com/identity/claims/identityprovider").Value;
			string token = "";
			secret = "";
			provider = ident;

			token = Request.Headers.GetValues("X-MS-TOKEN-FACEBOOK-ACCESS-TOKEN").FirstOrDefault();
			
			return token;
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
