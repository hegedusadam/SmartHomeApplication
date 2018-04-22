using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Data;

namespace SmartHomeApplication.ClientUWP.Converters
{
	public class TimeSpanToStringConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, string language)
		{
			TimeSpan timeSpan = (TimeSpan)value;

			if (timeSpan.TotalMinutes == 0)
			{
				return "";
			}

			string hours = timeSpan.Hours.ToString();
			string minutes = timeSpan.Minutes.ToString();
			string seconds = timeSpan.Seconds.ToString();

			return string.Format("{0} hours {1} minutes {2} seconds", hours, minutes, seconds);
		}

		public object ConvertBack(object value, Type targetType, object parameter, string language)
		{
			throw new NotImplementedException();
		}
	}
}
