﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SmartHomeApplicationService.Models
{
	public class LampState
	{
		public string Guid { get; set; }
		public bool IsOn { get; set; }
		public DateTime date { get; set; }
	}
}