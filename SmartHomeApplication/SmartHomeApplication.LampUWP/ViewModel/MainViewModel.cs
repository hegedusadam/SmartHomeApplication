using GalaSoft.MvvmLight;
using Microsoft.AspNet.SignalR.Client;
using Microsoft.AspNet.SignalR.Client.Transports;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SmartHomeApplication.LampUWP.Model;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Gpio;
using Windows.Storage;
using Windows.UI.Xaml.Navigation;

namespace SmartHomeApplication.LampUWP.ViewModel
{
	class MainViewModel : ViewModelBase
	{
		private string lampGuid;
        private readonly string key = "lampguid8";

		public string LampGuid
		{
			get { return lampGuid; }
			set { Set(ref lampGuid, value); }
		}

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

		public async Task CreateOrReadGuid()
		{

			if (ApplicationData.Current.LocalSettings.Values.ContainsKey(key))
			{
				LampGuid = (string)ApplicationData.Current.LocalSettings.Values[key];
			}
			else
			{
				Guid createGuid = Guid.NewGuid();
				string guidString = createGuid.ToString().Substring(0, 5);

				this.LampGuid = guidString;
				ApplicationData.Current.LocalSettings.Values[key] = guidString;

				await RegisterDevice(guidString);
			}
		}

		private async Task RegisterDevice(string guidString)
		{
			try
			{
				GuidDTO response = new GuidDTO
				{
					Guid = guidString
				};

				using (HttpClient client = new HttpClient())
				{
					string RegisterLampAddress = "https://smarthomeapplicationservice.azurewebsites.net/Lamp/RegisterDevice";
					var content = new StringContent(JsonConvert.SerializeObject(response), Encoding.UTF8, "application/json");
					HttpResponseMessage LampMessage = await client.PostAsync(RegisterLampAddress, content);
				}
			}
			catch (Exception e)
			{
				Debug.WriteLine(e.Message);
			}
		}

		public async Task SetupHub()
		{
			var hubConnection = new HubConnection("http://smarthomeapplicationservice.azurewebsites.net/");
			var lampHub = hubConnection.CreateHubProxy("LampHub");

			lampHub.On<bool, string>("OnSwitch", SwitchLamp);

			await hubConnection.Start(new LongPollingTransport());
		}

		private async void SwitchLamp(bool TurnOn, string guid)
		{
			try
			{
				if (!guid.Equals(this.LampGuid))
				{
					return;
				}

				//if (LedPin == null)
				//{
				//	return;
				//}

				//if (TurnOn)
				//{
				//	LedPin.Write(GpioPinValue.Low);
				//}
				//else if (!TurnOn)
				//{
				//	LedPin.Write(GpioPinValue.High);
				//}

				await SendLampState(TurnOn);
			}
			catch (Exception e)
			{
				Debug.Write(e.Message);
			}
		}

		private async Task SendLampState(bool LampIsOn)
		{
			LampStateDTO response = new LampStateDTO
			{
				Guid = LampGuid,
				IsOn = LampIsOn,
				date = DateTime.Now
			};

			try
			{
				using (HttpClient client = new HttpClient())
				{
					string LampAddress = "https://smarthomeapplicationservice.azurewebsites.net/Lamp/UpdateLamp";
					string ChangeAddress = "https://smarthomeapplicationservice.azurewebsites.net/Change/AddChange";
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

