using Microsoft.Azure.Devices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace LightController
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		/// <summary>
		/// Data members
		/// </summary>
		private static ServiceClient ServiceClient;
		private static string connectionString = "HostName=SmartHomeApplication.azure-devices.net;SharedAccessKeyName=iothubowner;SharedAccessKey=3JE/ceW92b8Gkp79sRz0OAJxmm5KV7vQQ0ADqQOoQ70=";

		/// <summary>
		/// Constructor
		/// </summary>
		public MainWindow()
		{
			InitializeComponent();
			ServiceClient = ServiceClient.CreateFromConnectionString(connectionString);
		}

		/// <summary>
		/// Calls the SendCloudToDeviceMessageAsync method in order to switch the led on or off
		/// </summary>
		/// <param name="isOn"></param>
		/// <returns></returns>
		private async Task TurnLight(bool isOn)
		{
			await SendCloudToDeviceMessageAsync(isOn);
		}

		/// <summary>
		/// Send a message to cloud and Azure sends a message to the Raspberry Pi, to switch the led on or off
		/// </summary>
		/// <param name="isOn"></param>
		/// <returns></returns>
		private static async Task SendCloudToDeviceMessageAsync(bool isOn)
		{
			var commandMessage = new Message(Encoding.ASCII.GetBytes(isOn ? "on" : "off"));
			await ServiceClient.SendAsync("rpi3", commandMessage);
		}

		/// <summary>
		/// The UI calls these methods, to change the condition of the LED
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private async void BtnTurnOn_OnClick(object sender, RoutedEventArgs e)
		{
			await TurnLight(true);
		}
		private async void BtnTurnOff_OnClick(object sender, RoutedEventArgs e)
		{
			await TurnLight(false);
		}
	}
}
