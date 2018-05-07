using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
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
using System.Windows.Input;
using Windows.UI.Core;
using Windows.UI.Popups;
using Windows.UI.Xaml;

namespace SmartHomeApplication.ClientUWP.ViewModel
{
	class LampSwitchViewModel : ViewModelBase
	{
		private bool isOn;
		private bool isInit = true;
		private bool isLocalChange = true;
		private string lampGuid;
		private ICommand deleteLampCommand;

		public ICommand DeleteLampCommand =>
			deleteLampCommand ??
			(deleteLampCommand = new RelayCommand(async () => await DeleteLamp()));

		public string LampGuid
		{
			get { return lampGuid; }
			set
			{
				Set(ref lampGuid, value);
				App.UserInformation.lampGuid = value;
			}
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

        public LampSwitchViewModel()
        {
            LampGuid = App.UserInformation.lampGuid;

            Task.Run(async () =>
            {
                await Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () =>
                {
                    await Initialize();
                });
            });
        }

        private async Task Initialize()
		{
			try
			{
				if (LampGuid.Equals("NOGUID"))
				{
					return;
				}

				var token = new JObject();
				token.Add("guid", LampGuid);
				var result = await App.MobileService.InvokeApiAsync("/Lamp/GetLampState", token);
				IsOn = result.ToObject<bool>();
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

			lampHub.On<bool, string>("SwitchClient", SwitchUponChange);

			await hubConnection.Start(new LongPollingTransport());
		}

		private async void SwitchUponChange(bool SwitchClient, string guid)
		{
			if (!guid.Equals(App.UserInformation.lampGuid))
			{
				return;
			}

			isLocalChange = false;
			await Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal,  () =>
			{
				IsOn = SwitchClient;
			});
		}

		private async Task SwitchLamp()
		{
			try
			{
				var token = new JObject();
				token.Add("TurnOn", IsOn.ToString().ToLower());
				token.Add("LampGuid", LampGuid);
				var result = await App.MobileService.InvokeApiAsync("/Lamp/TurnLamp", token);
			}
			catch (Exception exception)
			{
				Debug.WriteLine(exception.Message);
			}
		}

		private async Task DeleteLamp()
		{
			try
			{
				var confirmationDialog = new MessageDialog("Are you sure you want to delete this lamp?");
				confirmationDialog.Commands.Add(new UICommand("Yes") { Id = 0 });
				confirmationDialog.Commands.Add(new UICommand("Cancel") { Id = 1 });
				var choice = await confirmationDialog.ShowAsync();

				if ((int)choice.Id == 0)
				{
					var token = new JObject();
					token.Add("userId", App.UserInformation.userId);
					token.Add("lampGuid", LampGuid);
					var result = await App.MobileService.InvokeApiAsync("/Lamp/DeleteUserFromLamp", token);
                    HttpResponseMessage httpResponse = result.ToObject<HttpResponseMessage>();

                    if (httpResponse.StatusCode.Equals(HttpStatusCode.OK))
                    {
                        var deletedDialog = new MessageDialog("Lamp successfully deleted!");
                        deletedDialog.Commands.Add(new UICommand("Ok"));
                        await deletedDialog.ShowAsync();

                        LampGuid = "NOGUID";
                    }
                    else
                    {
                        var errorDialog = new MessageDialog("Error! Failed to delete lamp! Please try again!");
                        errorDialog.Commands.Add(new UICommand("Ok"));
                        await errorDialog.ShowAsync();
                    }
                }
				else
				{
					return;
				}
			}
			catch (Exception exception)
			{
				Debug.WriteLine(exception.Message);
			}
		}
	}
}