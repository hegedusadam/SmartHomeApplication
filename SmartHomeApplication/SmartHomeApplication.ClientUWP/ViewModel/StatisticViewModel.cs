using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using Microsoft.AspNet.SignalR.Client;
using Microsoft.AspNet.SignalR.Client.Transports;
using Newtonsoft.Json.Linq;
using SmartHomeApplication.ClientUWP.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.UI.Core;
using Windows.UI.Popups;

namespace SmartHomeApplication.ClientUWP.ViewModel
{
    public class StatisticViewModel : ViewModelBase
	{
		private ObservableCollection<Change> changesCollection;
		private TimeSpan totalTimeOn;
		private string lampGuid;
		private ICommand deleteChangesCommand;

		public ICommand DeleteChangesCommand =>
			deleteChangesCommand ??
			(deleteChangesCommand = new RelayCommand(async () => await DeleteChanges()));

		public string LampGuid
		{
			get { return lampGuid; }
			set
			{
				Set(ref lampGuid, value);
			}
		}

		public TimeSpan TotalTimeOn
		{
			get { return totalTimeOn; }
			set
			{
				Set(ref totalTimeOn, value);
			}
		}

		public ObservableCollection<Change> ChangesCollection
		{
			get { return changesCollection; }
			set
			{
				Set(ref changesCollection, value);
			}
		}

		public StatisticViewModel()
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

				await GetData();
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

			lampHub.On<string>("ChangeAdded", RefreshChanges);
			lampHub.On<string>("ChangesDeleted", RefreshChanges);

			await hubConnection.Start(new LongPollingTransport());
		}

		private async void RefreshChanges(string guid)
		{
			if (!LampGuid.Equals(guid))
			{
				return;
			}

			await Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () =>
			{
				await GetData();
			});
		}

		private async Task GetData()
		{
			ICollection<Change> changes = await GetChanges();
			ObservableCollection<Change> observableCollection = new ObservableCollection<Change>(changes);
			GetMinutesOn(observableCollection);
			TotalTimeOn = GetTotalTimeOn(observableCollection);
			ChangesCollection = new ObservableCollection<Change>(observableCollection.Reverse());
		}

		private async Task<ICollection<Change>> GetChanges()
		{
            try
            {
                var token = new JObject();
                token.Add("guid", LampGuid);
                var stringContent = new StringContent(token.ToString());
                var result = await App.MobileService.InvokeApiAsync("/Lamp/GetChanges", token);
                ICollection<Change> changes = result.ToObject<ICollection<Change>>();

                return changes;
            }
            catch (Exception e)
            {
                return null;
            }
		}

		public static void GetMinutesOn(ObservableCollection<Change> observableCollection)
		{
			Change onChange = new Change();

			foreach (var change in observableCollection)
			{
				if (change.state)
				{
					onChange = change;
				}
				else
				{
					change.timeOn = change.date.Subtract(onChange.date);
				}
			}
		}

		public static TimeSpan GetTotalTimeOn(ObservableCollection<Change> observableCollection)
		{
			int TotalSeconds = 0;

			foreach (var change in observableCollection)
			{
				if (!change.state)
				{
					TotalSeconds = TotalSeconds + (int)change.timeOn.TotalSeconds;
				}
			}

			return TimeSpan.FromSeconds(TotalSeconds);
		}

		public async Task DeleteChanges()
		{
			try
			{
				var confirmationDialog = new MessageDialog("Are you sure you want to delete the history of your lamp?");
				confirmationDialog.Commands.Add(new UICommand("Yes") { Id = 0 });
				confirmationDialog.Commands.Add(new UICommand("Cancel") { Id = 1 });
				var choice = await confirmationDialog.ShowAsync();

				if ((int)choice.Id == 0)
				{
					var token = new JObject();
					token.Add("lampGuid", LampGuid);
					var result = await App.MobileService.InvokeApiAsync("/Change/DeleteChanges",  token);
                    HttpResponseMessage httpResponse = result.ToObject<HttpResponseMessage>();

                    if (httpResponse.StatusCode.Equals(HttpStatusCode.OK))
                    {
                        var deletedDialog = new MessageDialog("Successfully cleared history!");
                        deletedDialog.Commands.Add(new UICommand("Ok"));
                        await deletedDialog.ShowAsync();
                    }
                    else
                    {
                        var errorDialog = new MessageDialog("Error! Failed to delete history! Please try again!");
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
