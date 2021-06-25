using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace CustomEditor.Converters
{
	public class BoolToThicknessConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			var defaultValue = new Thickness(0);
			if (value == null)
				return defaultValue;

			var parameterValue = parameter as string ?? string.Empty;
			var thicknessParameter = double.Parse(parameterValue, CultureInfo.InvariantCulture);
			var sourceValue = (bool)value;

			return sourceValue ? thicknessParameter : defaultValue;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			return DependencyProperty.UnsetValue;
		}
	}
}
