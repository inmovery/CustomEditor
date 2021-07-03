using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace CustomEditor.Converters
{
	public class BoolToVisibilityConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			var defaultValue = Visibility.Collapsed;
			if (value == null)
				return defaultValue;

			var result = (bool) value ? Visibility.Visible : defaultValue;
			return result;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			return DependencyProperty.UnsetValue;
		}
	}
}
