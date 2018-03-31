using GalaSoft.MvvmLight;
using Newtonsoft.Json;
using SmartHomeApplication.ClientUWP.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
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
				isOn = !isOn;
				Task.Run(async () =>
				{
					await SwitchLamp();
				});

				RaisePropertyChanged();
			}
		}

		public async Task SwitchLamp()
		{
			SwitchLampDTO respone = new SwitchLampDTO
			{
				TurnOn = IsOn
			};

			try
			{
				using (HttpClient client = new HttpClient())
				{
					string LampAddress = "http://smarthomeapplicationservice.azurewebsites.net/Lamp/TurnLamp";
					var content = new StringContent(JsonConvert.SerializeObject(respone), Encoding.UTF8, "application/json");
					HttpResponseMessage LampMessage = await client.PostAsync(LampAddress, content);
				}
			}
			catch (WebException exception)
			{
				throw new WebException("Error occured during the request.", exception);
			}
		}
	}
}