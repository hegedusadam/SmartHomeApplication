using SmartHomeApplication.ClientUWP.Controls;
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
				new BitmapImage(new Uri("ms-appx:///Assets/SplitView/profile.png", UriKind.Absolute));
			AddLamp.SelectedImageSource =
				new BitmapImage(new Uri("ms-appx:///Assets/SplitView/selected_profile.png", UriKind.Absolute));

			SwitchLamp.LabelText = "Switch Lamp";
			SwitchLamp.DefaultImageSource =
				new BitmapImage(new Uri("ms-appx:///Assets/SplitView/logout.png", UriKind.Absolute));
			SwitchLamp.SelectedImageSource =
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
				default:
					SelectControl(AddLamp);
					break;
			}
		}
	}
}
