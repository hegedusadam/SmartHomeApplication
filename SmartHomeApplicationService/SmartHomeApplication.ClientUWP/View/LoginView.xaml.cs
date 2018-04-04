using GalaSoft.MvvmLight.Views;
using Microsoft.WindowsAzure.MobileServices;
using SmartHomeApplication.ClientUWP.ViewModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace SmartHomeApplication.ClientUWP.View
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class LoginView : Page
    {
		private LoginViewModel ViewModel;
        public LoginView()
        {
			ViewModel = new LoginViewModel();
            this.InitializeComponent();
			this.DataContext = ViewModel;
        }

		protected override async void OnNavigatedTo(NavigationEventArgs e)
		{
			base.OnNavigatedTo(e);
			ViewModel.PropertyChanged += ViewModel_PropertyChanged;

			if (e.Parameter is Uri)
			{
				App.MobileService.ResumeWithURL(e.Parameter as Uri);
			}
		}

		private void ViewModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
		{
			switch (e.PropertyName)
			{
				case nameof(ViewModel.IsLoggedIn):
					//When the first screen of the app is launched after user has logged in, initialize the processor that manages connection to OBD Device and to the IOT Hub

					Frame.Navigate(typeof(LampSwitchView));
					break;
			}
		}

	}
}
