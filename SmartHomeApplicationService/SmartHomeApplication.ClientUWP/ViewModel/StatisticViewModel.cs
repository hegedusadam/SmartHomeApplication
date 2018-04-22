using GalaSoft.MvvmLight;
using Microsoft.AspNet.SignalR.Client;
using Microsoft.AspNet.SignalR.Client.Transports;
using Newtonsoft.Json.Linq;
using SmartHomeApplication.ClientUWP.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Core;

namespace SmartHomeApplication.ClientUWP.ViewModel
{
	public class StatisticViewModel : ViewModelBase
	{
		private ObservableCollection<Change> changesCollection;
		private TimeSpan totalTimeOn;
		private string lampGuid;

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
			ChangesCollection = observableCollection;
		}

		private async Task<ICollection<Change>> GetChanges()
		{
				var token = new JObject();
				token.Add("guid", LampGuid);
				var stringContent = new StringContent(token.ToString());
				var result = await App.MobileService.InvokeApiAsync("/Lamp/GetChanges", token);
				ICollection<Change> changes = result.ToObject<ICollection<Change>>();

				return changes;
		}

		private void GetMinutesOn(ObservableCollection<Change> observableCollection)
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

		private TimeSpan GetTotalTimeOn(ObservableCollection<Change> observableCollection)
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
	}
}
