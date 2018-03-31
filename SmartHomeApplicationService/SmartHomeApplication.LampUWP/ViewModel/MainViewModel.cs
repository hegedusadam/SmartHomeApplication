using GalaSoft.MvvmLight;
using Microsoft.AspNet.SignalR.Client;
using Microsoft.AspNet.SignalR.Client.Transports;
using Newtonsoft.Json;
using SmartHomeApplication.LampUWP.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Gpio;
using Windows.UI.Xaml.Navigation;

namespace SmartHomeApplication.LampUWP.ViewModel
{
	class MainViewModel : ViewModelBase
	{
		public int LampId { get; set; }
		
		/// <summary>
		///	GPIO settings
		/// </summary>
		public GpioController GpioController { get; }
		public GpioPin LedPin { get; }

		private readonly int OpenedPin = 4;

		/// <summary>
		/// Constructor
		/// </summary>
		public MainViewModel()
		{
			GpioController = GpioController.GetDefault();
			if (null != GpioController)
			{
				LedPin = GpioController.OpenPin(OpenedPin);
				LedPin.SetDriveMode(GpioPinDriveMode.Output);
			}
		}

		public async Task SetupHub()
		{
			var hubConnection = new HubConnection("http://smarthomeapplicationservice.azurewebsites.net/");
			var lampHub = hubConnection.CreateHubProxy("LampHub");

			lampHub.On<int>("SendLampId", setLampId); 
			lampHub.On<bool>("OnSwitch", handleCallback);

			await hubConnection.Start(new LongPollingTransport());
		}

		private void setLampId(int Id)
		{
			LampId = Id;
		}

		private async void handleCallback(bool TurnOn)
		{
			if (LedPin == null)
			{
				return;
			}

			if (TurnOn)
			{
				LedPin.Write(GpioPinValue.Low);
			}
			else if (!TurnOn)
			{
				LedPin.Write(GpioPinValue.High);
			}

			await SendLampState(LampId, TurnOn);
		}

		public async Task SendLampState(int LampId, bool LampIsOn)
		{
			LampStateDTO response = new LampStateDTO
			{
				Id = LampId,
				IsOn = LampIsOn,
				date = DateTime.Now
			};

			try
			{
				using (HttpClient client = new HttpClient())
				{
					string LampAddress = "http://smarthomeapplicationservice.azurewebsites.net/Lamp/UpdateLamp";
					string ChangeAddress = "http://smarthomeapplicationservice.azurewebsites.net/Change/AddChange";
					var content = new StringContent(JsonConvert.SerializeObject(response), Encoding.UTF8, "application/json");
					HttpResponseMessage LampMessage = await client.PutAsync(LampAddress, content);
					HttpResponseMessage ChangeMessage = await client.PostAsync(ChangeAddress, content);
				}
			} catch (WebException exception)
			{
				throw new WebException("Error occured during the request.", exception);
			}
		}
	}
}

