using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using Microsoft.WindowsAzure.MobileServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.UI.Popups;
using Windows.UI.Xaml;

namespace SmartHomeApplication.ClientUWP.ViewModel
{
    public class LoginViewModel : ViewModelBase
    {
		private MobileServiceUser user;

		private ICommand loginCommand;

		public ICommand LoginCommand =>
			loginCommand ??
			(loginCommand = new RelayCommand(async () => await AuthenticateAsync()));


		// Define a method that performs the authentication process
		// using a Facebook sign-in. 
		public async System.Threading.Tasks.Task<bool> AuthenticateAsync()
		{
			string message;
			bool success = false;
			try
			{
				// Change 'MobileService' to the name of your MobileServiceClient instance.
				// Sign-in using Facebook authentication.
				user = await App.MobileService
					.LoginAsync(MobileServiceAuthenticationProvider.Google, "smarthomeapplicationservice");
				message =
					string.Format("You are now signed in - {0}", user.UserId);

				success = true;
			}
			catch (InvalidOperationException)
			{
				message = "You must log in. Login Required";
			}

			var dialog = new MessageDialog(message);
			dialog.Commands.Add(new UICommand("OK"));
			await dialog.ShowAsync();
			return success;
		}
	}
}
