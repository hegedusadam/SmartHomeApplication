using GalaSoft.MvvmLight;
using Microsoft.AspNet.SignalR.Client;
using Microsoft.AspNet.SignalR.Client.Transports;
using Newtonsoft.Json;
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

		public int LampId { get; set; }

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

			CreateOrReadGuid();
		}

		public void CreateOrReadGuid()
		{

			if (ApplicationData.Current.LocalSettings.Values.ContainsKey("guid"))
			{
				LampGuid = (string)ApplicationData.Current.LocalSettings.Values["guid"];
			}
			else
			{
				Guid createGuid = Guid.NewGuid();
				string guidString = createGuid.ToString().Substring(0, 5);

				this.LampGuid = guidString;
				ApplicationData.Current.LocalSettings.Values["guid"] = guidString;
			}

			//try
			//{
			//	string destPath = Path.Combine(@"C:\Users\Adam\Downloads", "guid.txt");

			//	if (!File.Exists(destPath))
			//	{
			//		Guid guid = Guid.NewGuid();

			//		string guidString = guid.ToString().Substring(0, 5);

			//		this.guid = guidString;
			//		//File.WriteAllText(destPath, guidString);
			//		//DownloadsFolder.CreateFileAsync("file.txt");
			//	}
			//	else
			//	{
			//		this.guid = File.ReadAllText(destPath);
			//	}
			//}
			//catch (Exception e)
			//{
			//	Debug.WriteLine(e.Message);
			//}
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
					string LampAddress = "https://smarthomeapplicationservice.azurewebsites.net/Lamp/UpdateLamp";
					string ChangeAddress = "https://smarthomeapplicationservice.azurewebsites.net/Change/AddChange";
					var content = new StringContent(JsonConvert.SerializeObject(response), Encoding.UTF8, "application/json");
					HttpResponseMessage LampMessage = await client.PutAsync(LampAddress, content);
					HttpResponseMessage ChangeMessage = await client.PostAsync(ChangeAddress, content);

					string valami = content.ToString();
				}
			} catch (WebException exception)
			{
				throw new WebException("Error occured during the request.", exception);
			}
		}
	}
}

