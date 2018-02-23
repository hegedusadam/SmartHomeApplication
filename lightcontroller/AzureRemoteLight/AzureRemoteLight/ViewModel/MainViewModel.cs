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
		/// <summary>
		/// GPIO settings
		/// </summary>
		public GpioController GpioController { get; }
		public GpioPin LedPin { get; }

		/// <summary>
		/// Azure IoT settings of raspberry
		/// </summary>
		public DeviceClient DeviceClient { get; }

		public string IotHubUri { get; } = "SmartHomeApplication.azure-devices.net";
		public string DeviceKey { get; } = "qB1w140QB445Z4zdG5NP04QtKeJohUwzp1R9SEBmRIk=";
		public string DeviceId => "rpi3";

		private readonly int OpenedPin = 4;

		/// <summary>
		/// Constructor
		/// </summary>
		public MainViewModel()
		{
			DeviceClient = DeviceClient.Create(IotHubUri, new DeviceAuthenticationWithRegistrySymmetricKey(DeviceId, DeviceKey));
			GpioController = GpioController.GetDefault();
			if (null != GpioController)
			{
				LedPin = GpioController.OpenPin(OpenedPin);
				LedPin.SetDriveMode(GpioPinDriveMode.Output);
			}
		}

		/// <summary>
		/// Sending a test message to Cloud
		/// </summary>
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
				await DeviceClient.SendEventAsync(message);
				Debug.WriteLine("{0} > Sending message: {1}", DateTime.Now, messageString);
				//IsAzureConnected = true;
			}
			catch (Exception ex)
			{
				Debug.WriteLine(ex.Message);
			}
		}

		/// <summary>
		/// Receiving message from cloud (on/off)
		/// </summary>
		public async Task ReceiveCloudToDeviceMessageAsync()
		{
			Debug.WriteLine("\nReceiving cloud to device messages from service");
			while (true)
			{
				Message receivedMessage = await DeviceClient.ReceiveAsync();

				if (receivedMessage == null)
				{
					continue;
				}

				var msg = Encoding.ASCII.GetString(receivedMessage.GetBytes());

				if (msg == "on")
				{
					LedPin.Write(GpioPinValue.Low);
				}
				else if (msg == "off")
				{
					LedPin.Write(GpioPinValue.High);
				}

				await DeviceClient.CompleteAsync(receivedMessage);
			}
		}
	}
}
