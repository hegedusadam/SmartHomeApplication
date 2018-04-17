using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace SmartHomeApplication.ClientUWP.Controls
{
    public sealed partial class SplitViewButtonContent : UserControl
    {
		readonly SolidColorBrush defaultTextColor = new SolidColorBrush(Colors.White);

		readonly SolidColorBrush selectedTextColor = new SolidColorBrush(Color.FromArgb(0xFF, 0x1b, 0xa0, 0xe1));
		public BitmapImage DefaultImageSource;
		public string LabelText;
		public BitmapImage SelectedImageSource;

		public SplitViewButtonContent()
		{
			InitializeComponent();
		}

		public void SetSelected(bool selected)
		{
			if (selected)
			{
				Image.Source = SelectedImageSource;
				Label.Foreground = selectedTextColor;
			}
			else
			{
				Image.Source = DefaultImageSource;
				Label.Foreground = defaultTextColor;
			}
		}
	}
}
