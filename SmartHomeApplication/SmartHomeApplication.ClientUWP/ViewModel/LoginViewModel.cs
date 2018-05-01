﻿using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using Microsoft.WindowsAzure.MobileServices;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SmartHomeApplication.ClientUWP.Model;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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

		private bool isLoggedIn;

		public bool IsLoggedIn
		{
			get { return isLoggedIn; }
			set { Set(ref isLoggedIn, value); }
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
			catch (Exception e)
			{
				message = "You must log in. Login Required";
			}

			var dialog = new MessageDialog(message);
			dialog.Commands.Add(new UICommand("OK"));
			await dialog.ShowAsync();

			App.IsLoggedIn = IsLoggedIn;
			return IsLoggedIn;
		}

		private async Task GetUserInfo()
		{
			var result = await App.MobileService.InvokeApiAsync("/User/GetUserInfo", HttpMethod.Get, null);

			App.UserInformation = result.ToObject<UserInfo>();

			await RegisterToDatabase();
			await GetLampGuid();
		}

		private async Task RegisterToDatabase()
		{
			try
			{
				var token = new JObject();
				token.Add("Name", App.UserInformation.Name);
				token.Add("UserProfileId", App.User.UserId);
				var result = await App.MobileService.InvokeApiAsync("/User/Register", token);
				App.UserInformation.userId = result.ToObject<int>();
			}
			catch (Exception e)
			{
				Debug.WriteLine(e.Message);
			}
		}

		private async Task GetLampGuid()
		{
			try
			{
				var token = new JObject();
				token.Add("UserProfileId", App.User.UserId);
				var result = await App.MobileService.InvokeApiAsync("/User/GetGuid", token);
				//trim to be deleted
				App.UserInformation.lampGuid = result.ToObject<string>().Trim();
			}
			catch (Exception e)
			{
				Debug.WriteLine(e.Message);
			}
		}
	}
}