using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace CustomEditor.Converters
{
	public class DoubleFormatConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (value == null)
				return DependencyProperty.UnsetValue;

			var sourceValue = (double)value;
			return Math.Round(sourceValue);
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			return DependencyProperty.UnsetValue;
		}
	}
}
