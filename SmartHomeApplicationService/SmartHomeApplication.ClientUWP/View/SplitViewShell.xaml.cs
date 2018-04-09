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
	public sealed partial class SplitViewShell : Page
	{
		public SplitViewShell(Frame frame)
		{
			this.InitializeComponent();
			SmartHomeSplitView.Content = frame;
			frame.Navigated += Frame_Navigated;
		}

		private void Frame_Navigated(object sender, NavigationEventArgs e)
		{
			SmartHomeSplitView.IsPaneOpen = false;
		}

		private void AddLampRadioButton_Click(object sender, RoutedEventArgs e)
		{ 
			SmartHomeSplitView.IsPaneOpen = false;
			((Frame)SmartHomeSplitView.Content).Navigate(typeof(AddLampView));
		}

		private void SwitchLampRadioButton_Click(object sender, RoutedEventArgs e)
		{
			SmartHomeSplitView.IsPaneOpen = false;
			((Frame)SmartHomeSplitView.Content).Navigate(typeof(LampSwitchView));
		}
	}
}
