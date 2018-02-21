using GalaSoft.MvvmLight;
using Microsoft.Azure.Devices.Client;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Gpio;

namespace AzureRemoteLight.ViewModel
{
	class MainViewModel : ViewModelBase
	{
		public GpioController gpioController { get; }
		public GpioPin ledPin { get; }

		public DeviceClient deviceClient { get; }

		public string IotHubUri { get; } = "SmartHomeApplication.azure-devices.net";
		public string DeviceKey { get; } = "qB1w140QB445Z4zdG5NP04QtKeJohUwzp1R9SEBmRIk=";
		public string DeviceId => "rpi3";

		public MainViewModel()
		{
			deviceClient = DeviceClient.Create(IotHubUri, new DeviceAuthenticationWithRegistrySymmetricKey(DeviceId, DeviceKey));
			gpioController = GpioController.GetDefault();
			if (null != gpioController)
			{
				ledPin = gpioController.OpenPin(4);
				ledPin.SetDriveMode(GpioPinDriveMode.Output);
			}
		}

		public async Task SendDeviceToCloudMessagesAsync()
		{
			try
			{
				var telemetryDataPoint = new
				{
					deviceId = DeviceId,
					message = "Hello"
				};
				var messageString = JsonConvert.SerializeObject(telemetryDataPoint);
				var message = new Message(Encoding.ASCII.GetBytes(messageString));
				await deviceClient.SendEventAsync(message);
				Debug.WriteLine("{0} > Sending message: {1}", DateTime.Now, messageString);
				//IsAzureConnected = true;
			}
			catch (Exception ex)
			{
				Debug.WriteLine(ex.Message);
			}
		}

		public async Task ReceiveCloudToDeviceMessageAsync()
		{
			Debug.WriteLine("\nReceiving cloud to device messages from service");
			while (true)
			{
				Message receivedMessage = await deviceClient.ReceiveAsync();
				if (receivedMessage == null) continue;
				var msg = Encoding.ASCII.GetString(receivedMessage.GetBytes());
				if (msg == "on")
				{
					ledPin.Write(GpioPinValue.Low);
				}
				if (msg == "off")
				{
					ledPin.Write(GpioPinValue.High);
				}
				await deviceClient.CompleteAsync(receivedMessage);
			}
		}
	}
}
