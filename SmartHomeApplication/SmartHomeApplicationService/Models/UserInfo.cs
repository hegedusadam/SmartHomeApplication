using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SmartHomeApplicationService.Models
{
	public class UserInfo
	{
		public int userId { get; set; }
		public string Name { get; set; }
		public string ImageUri { get; set; }
		public string UserProfileId { get; set; }
		public string lampGuid { get; set; }
	}
}