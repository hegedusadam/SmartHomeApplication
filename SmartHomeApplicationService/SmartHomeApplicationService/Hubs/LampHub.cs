using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.SignalR;

namespace SmartHomeApplicationService.Hubs
{
	public class LampHub : Hub
	{
		public static int LampId { get; set; }
		public static bool TurnOn { get; set; }
		public static bool ClientSwitch { get; set; }
	}
}