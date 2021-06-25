using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace CustomEditor.Converters
{
	public class BoolToBrushConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			var defaultValue = new SolidColorBrush(Colors.Transparent);
			if (value == null)
				return defaultValue;

			var colorBrush = parameter as SolidColorBrush;
			if (colorBrush == null)
				return defaultValue;

			var sourceValue = (bool)value;

			return sourceValue ? colorBrush : defaultValue;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			return DependencyProperty.UnsetValue;
		}
	}
}
