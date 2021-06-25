using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace CustomEditor.Controls
{
	public class ButtonWithImage : Button
	{
		public static readonly DependencyProperty PathImageProperty = DependencyProperty.Register(
			nameof(PathImage),
			typeof(PathGeometry),
			typeof(ButtonWithImage),
			new FrameworkPropertyMetadata(null));

		public PathGeometry PathImage
		{
			get => (PathGeometry)GetValue(PathImageProperty);
			set => SetValue(PathImageProperty, value);
		}

		public static readonly DependencyProperty FillProperty = DependencyProperty.Register(
			nameof(Fill),
			typeof(SolidColorBrush),
			typeof(ButtonWithImage),
			new PropertyMetadata(new SolidColorBrush(Colors.Transparent)));

		public SolidColorBrush Fill
		{
			get => (SolidColorBrush)GetValue(FillProperty);
			set => SetValue(FillProperty, value);
		}

		public static readonly DependencyProperty IsActiveProperty = DependencyProperty.Register(
			nameof(IsActive),
			typeof(bool),
			typeof(ButtonWithImage),
			new PropertyMetadata(false));

		public bool IsActive
		{
			get => (bool)GetValue(IsActiveProperty);
			set => SetValue(IsActiveProperty, value);
		}

		protected override void OnClick()
		{
			IsActive = !IsActive;
			base.OnClick();
		}
	}
}
