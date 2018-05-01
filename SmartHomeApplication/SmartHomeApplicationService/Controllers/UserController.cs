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
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SmartHomeApplicationService.Models;

namespace SmartHomeApplicationService.Controllers
{
	public class UserController : Controller
	{
		private SmartHomeApplicationDatabase db = new SmartHomeApplicationDatabase();

		[HttpPost, ActionName("Register")]
		public int RegisterToDatabase(UserInfo userInfo)
		{
			User user = db.Users.Where(u => u.UserProfileId == userInfo.UserProfileId).FirstOrDefault();

			if (user == null)
			{
				string[] names = userInfo.Name.Split(' ');
				User newUser = db.Users.Add(new User
				{
					FirstName = names[0],
					LastName = names[1],
					UserProfileId = userInfo.UserProfileId
				});

				db.SaveChanges();

				return newUser.Id;
			}

			return user.Id;
		}

		[HttpPost, ActionName("GetGuid")]
		public string GetLampGuid(UserInfo userInfo)
		{

			User user = db.Users.Where(u => u.UserProfileId == userInfo.UserProfileId).FirstOrDefault();

			string guid;

			if (user.Lamp == null)
			{
				guid = "NOGUID";
			}
			else
			{
				guid = user.Lamp.lampguid;
			}

            return JsonConvert.SerializeObject(guid);
        }


		[HttpGet, ActionName("GetUserInfo")]
		public async Task<string> GetUserInfo()
		{
			string accessToken = GetAccessToken();
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

            return JsonConvert.SerializeObject(info);
        }

		private string GetAccessToken()
		{
			string token = "";

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
