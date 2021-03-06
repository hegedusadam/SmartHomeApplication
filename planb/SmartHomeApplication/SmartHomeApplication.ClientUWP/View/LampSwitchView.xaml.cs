﻿using SmartHomeApplication.ClientUWP.ViewModel;
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
	public sealed partial class LampSwitchView : Page
	{
		private LampSwitchViewModel ViewModel;
		public LampSwitchView()
		{
			ViewModel = new LampSwitchViewModel();
			this.InitializeComponent();

			DeleteLamp.LabelText = "Delete Lamp";
			NoLampTextBlock.Text = "You do not have a Lamp added. Go to the Add Lamp page and create a lamp!";

			DeleteLamp.DefaultImageSource =
				new BitmapImage(new Uri("ms-appx:///Assets/Common/delete.png", UriKind.Absolute));

			this.DataContext = ViewModel;
			Loaded += async (sender, args) =>
			{
				await ViewModel.SetupHub();
			};
		}
	}
}
