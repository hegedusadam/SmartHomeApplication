using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.UI.Popups;

namespace SmartHomeApplication.ClientUWP.ViewModel
{
	public class AddLampViewModel : ViewModelBase
	{
		private string newLampGuid;
		private string lampGuid;

		public string NewLampGuid
		{
			get { return newLampGuid; }
			set
			{
				Set(ref newLampGuid, value);
			}
		}

		public string LampGuid
		{
			get { return lampGuid; }
			set
			{
				Set(ref lampGuid, value.Trim());
			}
		}

		private ICommand addLampCommand;
		public ICommand AddLampCommand =>
			addLampCommand ??
			(addLampCommand = new RelayCommand(async () => await AddNewLamp()));

		public AddLampViewModel()
		{
			LampGuid = App.UserInformation.lampGuid;
		}

		public async Task AddNewLamp()
		{
			try
			{
				var token = new JObject();
				token.Add("LampGuid", NewLampGuid);
				token.Add("UserID", App.UserInformation.userId);
				var result = await App.MobileService.InvokeApiAsync("/Lamp/AddUserToLamp", token);

				App.UserInformation.lampGuid = NewLampGuid;
				LampGuid = NewLampGuid;

				var deletedDialog = new MessageDialog("Lamp successfully added!");
				deletedDialog.Commands.Add(new UICommand("Ok"));
				await deletedDialog.ShowAsync();
			} catch (Exception e)
			{
				Debug.WriteLine(e.Message);
			}
		}
	}
}
