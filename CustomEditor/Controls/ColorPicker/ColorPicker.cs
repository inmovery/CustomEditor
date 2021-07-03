using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using CustomEditor.Controls.ColorPicker.ColorManipulation;

namespace CustomEditor.Controls.ColorPicker
{
	public class ColorPicker : Control
	{
		private bool _inCallback;
		private Slider? _hueSlider;

		public const string HueSliderPartName = "PART_HueSlider";

		static ColorPicker()
		{
			DefaultStyleKeyProperty.OverrideMetadata(typeof(ColorPicker), new FrameworkPropertyMetadata(typeof(ColorPicker)));
		}

		public static readonly DependencyProperty ColorProperty = DependencyProperty.Register(
			nameof(Color),
			typeof(Color),
			typeof(ColorPicker),
			new FrameworkPropertyMetadata(default(Color), FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, ColorPropertyChangedCallback));

		public Color Color
		{
			get => (Color)GetValue(ColorProperty);
			set => SetValue(ColorProperty, value);
		}

		public static readonly RoutedEvent ColorChangedEvent = EventManager.RegisterRoutedEvent(
			nameof(Color),
			RoutingStrategy.Bubble,
			typeof(RoutedPropertyChangedEventHandler<Color>),
			typeof(ColorPicker));

		public event RoutedPropertyChangedEventHandler<Color> ColorChanged
		{
			add => AddHandler(ColorChangedEvent, value);
			remove => RemoveHandler(ColorChangedEvent, value);
		}

		public static readonly DependencyProperty HsbProperty = DependencyProperty.Register(
			nameof(Hsb),
			typeof(Hsb),
			typeof(ColorPicker),
			new FrameworkPropertyMetadata(default(Hsb), FrameworkPropertyMetadataOptions.AffectsArrange | FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, HsbPropertyChangedCallback));

		public Hsb Hsb
		{
			get => (Hsb)GetValue(HsbProperty);
			set => SetValue(HsbProperty, value);
		}

		private static void ColorPropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs eventArgs)
		{
			var colorPicker = (ColorPicker)d;
			//if (colorPicker._inCallback)
			//	return;

			colorPicker._inCallback = true;
			colorPicker.SetCurrentValue(HsbProperty, ((Color)eventArgs.NewValue).ToHsb());

			var oldColor = (Color)eventArgs.OldValue;
			var newColor = (Color)eventArgs.NewValue;
			var args = new RoutedPropertyChangedEventArgs<Color>(oldColor, newColor)
			{
				RoutedEvent = ColorChangedEvent
			};

			colorPicker.RaiseEvent(args);
			colorPicker._inCallback = false;
		}

		private static void HsbPropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs eventArgs)
		{
			var colorPicker = (ColorPicker)d;
			if (colorPicker._inCallback)
				return;

			colorPicker._inCallback = true;

			var color = default(Color);
			if (eventArgs.NewValue is Hsb hsb)
				color = hsb.ToColor();

			colorPicker.SetCurrentValue(ColorProperty, color);

			colorPicker._inCallback = false;
		}

		public override void OnApplyTemplate()
		{
			if (_hueSlider != null)
				_hueSlider.ValueChanged -= HueSliderOnValueChanged;

			_hueSlider = GetTemplateChild(HueSliderPartName) as Slider;
			if (_hueSlider != null)
				_hueSlider.ValueChanged += HueSliderOnValueChanged;

			base.OnApplyTemplate();
		}

		private void HueSliderOnValueChanged(object sender, RoutedPropertyChangedEventArgs<double> eventArgs)
		{
			if (Hsb is var hsb)
				Hsb = new Hsb(eventArgs.NewValue, 1, 1);
		}

		protected override Size ArrangeOverride(Size arrangeBounds)
		{
			var result = base.ArrangeOverride(arrangeBounds);
			return result;
		}
	}
}
