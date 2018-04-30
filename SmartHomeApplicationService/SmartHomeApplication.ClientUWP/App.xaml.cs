using GalaSoft.MvvmLight;
using Microsoft.WindowsAzure.MobileServices;
using SmartHomeApplication.ClientUWP.Model;
using SmartHomeApplication.ClientUWP.View;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Networking.Connectivity;
using Windows.UI.Core;
using Windows.UI.Popups;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

namespace SmartHomeApplication.ClientUWP
{
    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    sealed partial class App : Application
    {
		public static MobileServiceUser User;
		public static MobileServiceClient MobileService = new MobileServiceClient("https://smarthomeapplicationservice.azurewebsites.net");
		public static UserInfo UserInformation;
		public static bool IsLoggedIn = false;
		/// <summary>
		/// Initializes the singleton application object.  This is the first line of authored code
		/// executed, and as such is the logical equivalent of main() or WinMain().
		/// </summary>
		public App()
        {
            this.InitializeComponent();
            this.Suspending += OnSuspending;

            ApplicationView.PreferredLaunchViewSize = new Size(700, 500);
            ApplicationView.PreferredLaunchWindowingMode = ApplicationViewWindowingMode.PreferredLaunchViewSize;

            Task.Run(async () =>
            {
                await Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () =>
                {
                    await hasInternetConnection();
                });
            });
        }

        /// <summary>
        /// Invoked when the application is launched normally by the end user.  Other entry points
        /// will be used such as when the application is launched to open a specific file.
        /// </summary>
        /// <param name="e">Details about the launch request and process.</param>
        protected override void OnLaunched(LaunchActivatedEventArgs e)
        {
			SplitViewShell shell = Window.Current.Content as SplitViewShell;

			if (shell == null)
			{
				Frame rootFrame = new Frame();
				rootFrame.NavigationFailed += OnNavigationFailed;

				rootFrame.Navigate(typeof(LoginView), e.Arguments);
				Window.Current.Content = rootFrame;
			}

			Window.Current.Activate();
        }

        /// <summary>
        /// Invoked when Navigation to a certain page fails
        /// </summary>
        /// <param name="sender">The Frame which failed navigation</param>
        /// <param name="e">Details about the navigation failure</param>
        void OnNavigationFailed(object sender, NavigationFailedEventArgs e)
        {
            throw new Exception("Failed to load Page " + e.SourcePageType.FullName);
        }

        /// <summary>
        /// Invoked when application execution is being suspended.  Application state is saved
        /// without knowing whether the application will be terminated or resumed with the contents
        /// of memory still intact.
        /// </summary>
        /// <param name="sender">The source of the suspend request.</param>
        /// <param name="e">Details about the suspend request.</param>
        private void OnSuspending(object sender, SuspendingEventArgs e)
        {
            var deferral = e.SuspendingOperation.GetDeferral();
            //TODO: Save application state and stop any background activity
            deferral.Complete();
        }

		protected override void OnActivated(IActivatedEventArgs args)
		{
			if (args.Kind == ActivationKind.Protocol)
			{
				ProtocolActivatedEventArgs protocolArgs = args as ProtocolActivatedEventArgs;
				Frame content = Window.Current.Content as Frame;
				if (content.Content.GetType() == typeof(LoginView))
				{
					content.Navigate(typeof(LoginView), protocolArgs.Uri);
				}
			}
			Window.Current.Activate();
			base.OnActivated(args);
		}

        public static async Task hasInternetConnection()
        {
            ConnectionProfile connections = NetworkInformation.GetInternetConnectionProfile();
            bool internet = connections != null && connections.GetNetworkConnectivityLevel() == NetworkConnectivityLevel.InternetAccess;

            if (!internet)
            {
                var noInternetDialog = new MessageDialog("Could not connect to the Internet. Please fix your connection and try again!");
                noInternetDialog.Title = "Connection Error";
                noInternetDialog.Commands.Add(new UICommand("Ok"));
                await noInternetDialog.ShowAsync();

                Current.Exit();
            }
        }
    }
}
