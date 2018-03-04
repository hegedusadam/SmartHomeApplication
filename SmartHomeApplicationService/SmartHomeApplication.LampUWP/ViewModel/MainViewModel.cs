using GalaSoft.MvvmLight;
using Microsoft.AspNet.SignalR.Client;
using Microsoft.AspNet.SignalR.Client.Transports;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Gpio;
using Windows.UI.Xaml.Navigation;

namespace SmartHomeApplication.LampUWP.ViewModel
{
	class MainViewModel : ViewModelBase
	{
		/// <summary>
		/// GPIO settings
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

			lampHub.On<bool>("OnSwitch", handleCallback);

			await hubConnection.Start(new LongPollingTransport());
		}

		private void handleCallback(bool TurnOn)
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
		}
	}
}
