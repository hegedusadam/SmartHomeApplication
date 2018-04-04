﻿using GalaSoft.MvvmLight;
using Microsoft.AspNet.SignalR.Client;
using Microsoft.AspNet.SignalR.Client.Transports;
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
using Windows.UI.Core;
using Windows.UI.Xaml;

namespace SmartHomeApplication.ClientUWP.ViewModel
{
	class LampSwitchViewModel : ViewModelBase
	{
		private bool isOn;
		private bool isInit = true;
		private bool isLocalChange = true;

		public LampSwitchViewModel()
		{
			Task.Run(async () =>
			{
				await Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () =>
				{
					await Initialize();
				});
			});
		}

		public bool IsOn
		{
			get { return isOn; }
			set
			{
				Set(ref isOn, value);

				if (!isInit && isLocalChange)
				{
					Task.Run(async () =>
					{
						await SwitchLamp();
					});
				}

				isInit = false;
				isLocalChange = true;
			}
		}

		public async Task Initialize()
		{
			try
			{
				IsOn = await App.MobileService.InvokeApiAsync<bool>("/Lamp/GetLampState", HttpMethod.Get, null);
			}
			catch (Exception exception)
			{
				Debug.WriteLine(exception.Message);
			}
		}

		public async Task SetupHub()
		{
			var hubConnection = new HubConnection("http://smarthomeapplicationservice.azurewebsites.net/");
			var lampHub = hubConnection.CreateHubProxy("LampHub");

			lampHub.On<bool>("SwitchClient", SwitchUponChange);

			await hubConnection.Start(new LongPollingTransport());
		}

		private async void SwitchUponChange(bool SwitchClient)
		{
			isLocalChange = false;
			await Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal,  () =>
			{
				IsOn = SwitchClient;
			});
		}

		public async Task SwitchLamp()
		{
			try
			{
				var token = new JObject();
				token.Add("TurnOn", IsOn.ToString().ToLower());
				var result = await App.MobileService.InvokeApiAsync("/Lamp/TurnLamp", token);
			}
			catch (Exception exception)
			{
				Debug.WriteLine(exception.Message);
			}
		}
	}
}