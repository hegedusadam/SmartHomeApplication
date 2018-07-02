using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartHomeApplication.LampUWP.Model
{
	class LampStateDTO
	{
		public string Guid { get; set; }
		public bool IsOn { get; set; }
		public DateTime date { get; set; }
	}
}
