using GalaSoft.MvvmLight;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SmartHomeApplication.ClientUWP.Model;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace SmartHomeApplication.ClientUWP.ViewModel
{
	class LampSwitchViewModel : ViewModelBase
	{
		private bool isOn = false;

		public bool IsOn
		{
			get { return isOn; }
			set
			{
				isOn = value;
				Task.Run(async () =>
				{
					await SwitchLamp();
				});

				RaisePropertyChanged();
			}
		}

		public async Task SwitchLamp()
		{
			try
			{
				var token = new JObject();
				token.Add("TurnOn", IsOn.ToString().ToLower());
				var result = await App.MobileService.InvokeApiAsync("/Lamp/TurnLamp", token); // This is the line that fails
			}
			catch (Exception exception)
			{
				Debug.WriteLine(exception.Message);
			}
		}
	}
}