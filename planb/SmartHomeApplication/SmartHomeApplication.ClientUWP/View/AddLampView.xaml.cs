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
	public sealed partial class AddLampView : Page
	{
		private AddLampViewModel ViewModel;
		public AddLampView()
		{
			ViewModel = new AddLampViewModel();
			this.InitializeComponent();

			AlreadyHasLampTextBlock.Text = "You have already added a lamp. Visit the Switch Lamp Page to control it.";
			GuidTextBlock.Text = "GUID of Your Lamp:";
			AddLampButton.LabelText = "Add Lamp";

			AddLampButton.DefaultImageSource =
				new BitmapImage(new Uri("ms-appx:///Assets/SplitView/add.png", UriKind.Absolute));

			this.DataContext = ViewModel;
		}
	}
}
