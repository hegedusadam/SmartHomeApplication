using Microsoft.AspNet.SignalR;
using SmartHomeApplicationService.Hubs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Mvc;

namespace SmartHomeApplicationService.Controllers
{
    public class LampController : ApiController
    {
		public void Get(bool TurnOn)
		{
			var lampContext = GlobalHost.ConnectionManager.GetHubContext<LampHub>();

			LampHub.TurnOn = TurnOn;
			lampContext.Clients.All.OnSwitch(TurnOn);
		}
    }
}
