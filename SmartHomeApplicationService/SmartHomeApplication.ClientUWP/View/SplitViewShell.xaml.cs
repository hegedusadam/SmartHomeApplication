using Microsoft.WindowsAzure.MobileServices;
using SmartHomeApplication.ClientUWP.Controls;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace SmartHomeApplication.ClientUWP.View
{
	/// <summary>
	/// An empty page that can be used on its own or navigated to within a Frame.
	/// </summary>
	public sealed partial class SplitViewShell : Page
	{
		SplitViewButtonContent selectedControl;

		public SplitViewShell(Frame frame)
		{
			this.InitializeComponent();
			SmartHomeSplitView.Content = frame;
			frame.Navigated += Frame_Navigated;

			AddLamp.LabelText = "Add Lamp";
			AddLamp.DefaultImageSource =
				new BitmapImage(new Uri("ms-appx:///Assets/SplitView/add.png", UriKind.Absolute));
			AddLamp.SelectedImageSource =
				new BitmapImage(new Uri("ms-appx:///Assets/SplitView/selected_add.png", UriKind.Absolute));

			SwitchLamp.LabelText = "Switch Lamp";
			SwitchLamp.DefaultImageSource =
				new BitmapImage(new Uri("ms-appx:///Assets/SplitView/switch.png", UriKind.Absolute));
			SwitchLamp.SelectedImageSource =
				new BitmapImage(new Uri("ms-appx:///Assets/SplitView/selected_switch.png", UriKind.Absolute));

			Statistic.LabelText = "Statistic";
			Statistic.DefaultImageSource =
				new BitmapImage(new Uri("ms-appx:///Assets/SplitView/statistic.png", UriKind.Absolute));
			Statistic.SelectedImageSource =
				new BitmapImage(new Uri("ms-appx:///Assets/SplitView/selected_statistic.png", UriKind.Absolute));

			ProfileImage.ImageSource = new BitmapImage(new Uri(App.UserInformation.ImageUri, UriKind.Absolute));
			ProfileName.Text = App.UserInformation.Name;

			Logout.LabelText = "Log Out";
			Logout.DefaultImageSource = 
				new BitmapImage(new Uri("ms-appx:///Assets/SplitView/logout.png", UriKind.Absolute));
			Logout.SelectedImageSource =
				new BitmapImage(new Uri("ms-appx:///Assets/SplitView/selected_logout.png", UriKind.Absolute));
		}

		private void Frame_Navigated(object sender, NavigationEventArgs e)
		{
			SmartHomeSplitView.IsPaneOpen = false;
		}

		private void AddLampButton_Click(object sender, RoutedEventArgs e)
		{
			SelectControl(AddLamp);
			SmartHomeSplitView.IsPaneOpen = false;
			PageTitle.Text = "ADD LAMP";
			((Frame)SmartHomeSplitView.Content).Navigate(typeof(AddLampView));
		}

		private void SwitchLampButton_Click(object sender, RoutedEventArgs e)
		{
			SelectControl(SwitchLamp);
			SmartHomeSplitView.IsPaneOpen = false;
			PageTitle.Text = "SWITCH LAMP";
			((Frame)SmartHomeSplitView.Content).Navigate(typeof(LampSwitchView));
		}

		private void StatisticButton_Click(object sender, RoutedEventArgs e)
		{
			SelectControl(Statistic);
			SmartHomeSplitView.IsPaneOpen = false;
			PageTitle.Text = "STATISTIC";
			((Frame)SmartHomeSplitView.Content).Navigate(typeof(StatisticView));
		}

		private async void LogoutButton_Click(object sender, RoutedEventArgs e)
		{

			var confirmationDialog = new MessageDialog("Are you sure you want to log out?");
			confirmationDialog.Commands.Add(new UICommand("Yes") { Id = 0 });
			confirmationDialog.Commands.Add(new UICommand("Cancel") { Id = 1 });
			var choice = await confirmationDialog.ShowAsync();

			if ((int)choice.Id == 0)
			{
				App.UserInformation = null;
				App.User = null;
				App.IsLoggedIn = false;
				await App.MobileService.LogoutAsync();

				Frame frame = new Frame();
				frame.Navigate(typeof(LoginView));
				Window.Current.Content = frame;
				Window.Current.Activate();
			}
			else
			{
				return;
			}
		}

		private void HamburgerButton_Click(object sender, RoutedEventArgs e)
		{
			SmartHomeSplitView.IsPaneOpen = !SmartHomeSplitView.IsPaneOpen;
			if (SmartHomeSplitView.IsPaneOpen)
			{
				double openPaneLength = MainGrid.ActualWidth * .6;
				if (openPaneLength > SmartHomeSplitView.OpenPaneLength && openPaneLength < 300)
				{
					SmartHomeSplitView.OpenPaneLength = openPaneLength;
					AddLampButton.Width = openPaneLength;
					SwitchLampButton.Width = openPaneLength;
				}
			}
		}

		public void SelectControl(SplitViewButtonContent control)
		{
			selectedControl?.SetSelected(false);
			control.SetSelected(true);
			selectedControl = control;
		}

		public void SetTitle(string title)
		{
			PageTitle.Text = title;
		}

		public void SetSelectedPage(string page)
		{
			switch (page)
			{
				case "SWITCH LAMP":
					SelectControl(SwitchLamp);
					break;
				case "STATISTIC":
					SelectControl(Statistic);
					break;
				default:
					SelectControl(AddLamp);
					break;
			}
		}
	}
}
