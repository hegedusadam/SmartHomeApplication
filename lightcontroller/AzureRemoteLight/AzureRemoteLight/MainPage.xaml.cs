﻿using AzureRemoteLight.ViewModel;
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

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace AzureRemoteLight
{
    public sealed partial class MainPage : Page
    {
		/// <summary>
		/// Data member of ViewModel.MainViewModel
		/// </summary>
		private MainViewModel _vm;
		public MainPage()
		{
			this.InitializeComponent();
			_vm = new MainViewModel();
			Loaded += async (sender, args) =>
			{
				//
				await _vm.SendDeviceToCloudMessagesAsync();

				// receive remote light control events
				await _vm.ReceiveCloudToDeviceMessageAsync();
			};
		}
	}
}
