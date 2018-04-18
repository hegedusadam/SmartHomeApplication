using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Media.Imaging;

namespace SmartHomeApplication.ClientUWP.Model
{
	public class UserInfo
	{
		public int userId;
		public string Name { get; set; }
		public string ImageUri { get; set; }
		public string lampGuid { get; set; }
	}
}
