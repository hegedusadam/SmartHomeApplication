using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartHomeApplication.ClientUWP.Model
{
	public class Change
	{
		public int Id { get; set; }
		public System.DateTime date { get; set; }
		public bool state { get; set; }
		public int lampid { get; set; }
		public TimeSpan timeOn { get; set; }
	}
}
