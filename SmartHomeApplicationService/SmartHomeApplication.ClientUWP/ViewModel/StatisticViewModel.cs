using GalaSoft.MvvmLight;
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
		ObservableCollection<Change> observableCollection;

		private string lampGuid;
		public string LampGuid
		{
			get { return lampGuid; }
			set
			{
				Set(ref lampGuid, value);
			}
		}

		public ObservableCollection<Change> ObservableCollection
		{
			get { return observableCollection; }
			set
			{
				Set(ref observableCollection, value);
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

				var token = new JObject();
				token.Add("guid", LampGuid);
				var stringContent = new StringContent(token.ToString());
				var result = await App.MobileService.InvokeApiAsync("/Lamp/GetChanges", token);
				ICollection<Change> changes = result.ToObject<ICollection<Change>>();
				ObservableCollection = new ObservableCollection<Change>(changes);
			}
			catch (Exception exception)
			{
				Debug.WriteLine(exception.Message);
			}
		}
	}
}
