using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace CustomEditor.Converters
{
	public class StringToFloatConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (value == null)
				return 0.0f;

			if (parameter != null && (string)parameter == "double")
				return double.Parse((string)value);

			return float.Parse((string)value, CultureInfo.InvariantCulture);
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			return value?.ToString() ?? DependencyProperty.UnsetValue;
		}
	}
}
