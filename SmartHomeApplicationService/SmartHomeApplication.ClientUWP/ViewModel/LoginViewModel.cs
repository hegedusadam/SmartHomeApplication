using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using Microsoft.WindowsAzure.MobileServices;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SmartHomeApplication.ClientUWP.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media.Imaging;

namespace SmartHomeApplication.ClientUWP.ViewModel
{
    public class LoginViewModel : ViewModelBase
    {
		private ICommand loginCommand;

		public UserInfo userInfo;

		private bool isLoggedIn;

		public bool IsLoggedIn
		{
			get { return isLoggedIn; }
			set { Set(ref isLoggedIn, value); }
		}

		public UserInfo UserInformation
		{
			get { return userInfo; }
			set { Set(ref userInfo, value); }
		}

		public ICommand LoginCommand =>
			loginCommand ??
			(loginCommand = new RelayCommand(async () => await AuthenticateAsync()));

		public async System.Threading.Tasks.Task<bool> AuthenticateAsync()
		{
			string message;
			IsLoggedIn = false;

			try
			{
				App.User = await App.MobileService
					.LoginAsync(MobileServiceAuthenticationProvider.Facebook, "smarthomeapplicationservice");
				message =
					string.Format("You are now signed in - {0}", App.User.UserId);

				await GetUserInfo();

				IsLoggedIn = true;
			}
			catch (InvalidOperationException)
			{
				message = "You must log in. Login Required";
			}

			var dialog = new MessageDialog(message);
			dialog.Commands.Add(new UICommand("OK"));
			await dialog.ShowAsync();

			return IsLoggedIn;
		}

		private async Task GetUserInfo()
		{
			var result = await App.MobileService.InvokeApiAsync("/User/GetUserInfo", HttpMethod.Get, null);

			UserInformation = result.ToObject<UserInfo>();
			//var httpclient = new HttpClient();
			//var bytes = await httpclient.GetByteArrayAsync(userInfo.ImageUri);
			//var pi = new BitmapImage();
			//await
			//	pi.SetSourceAsync(
			//		new MemoryStream(bytes).AsRandomAccessStream());
			await RegisterToDatabase();
		}

		private async Task RegisterToDatabase()
		{
				var token = new JObject();
				token.Add("Name", UserInformation.Name);
				token.Add("userId", App.User.UserId);
				var result = await App.MobileService.InvokeApiAsync("/User/Register", token);
		}
	}
}
