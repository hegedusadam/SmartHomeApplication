﻿using SmartHomeApplication.ClientUWP.ViewModel;
using System;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Imaging;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace SmartHomeApplication.ClientUWP.View
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class StatisticView : Page
	{
		private StatisticViewModel ViewModel;
		public StatisticView()
		{
            this.InitializeComponent();
			ViewModel = new StatisticViewModel();
			this.DataContext = ViewModel;

			TotaltTimeText.Text = "Total Time Switched On:";
			NoLampTextBlock.Text = "No lamp added! Please visit the Add Lamp view to add a new lamp!";
            WhenTextBlock.Text = "When";
            StateTextBlock.Text = "State";
            MinutesOnTextBlock.Text = "Time Switched On";

			DeleteChanges.LabelText = "Clear History";
			DeleteChanges.DefaultImageSource =
				new BitmapImage(new Uri("ms-appx:///Assets/Common/delete.png", UriKind.Absolute));

			Loaded += async (sender, args) =>
			{
				await ViewModel.SetupHub();
			};
		}
    }
}
