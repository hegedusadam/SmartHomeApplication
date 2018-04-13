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

namespace SmartHomeApplication.ClientUWP.ViewModel
{
	public class AddLampViewModel : ViewModelBase
	{
		private string lampName;
		private string lampGuid;

		public string LampName
		{
			get { return lampName; }
			set
			{
				Set(ref lampName, value);
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

		public async Task AddNewLamp()
		{
			try
			{
				var token = new JObject();
				token.Add("LampName", LampName);
				token.Add("LampGuid", LampGuid);
				token.Add("UserID", App.UserInformation.userId);
				var result = await App.MobileService.InvokeApiAsync("/Lamp/AddLamp", token);
			} catch (Exception e)
			{
				Debug.WriteLine(e.Message);
			}
		}
	}
}
