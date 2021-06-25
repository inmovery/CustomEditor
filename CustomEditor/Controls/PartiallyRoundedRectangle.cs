using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;

namespace CustomEditor.Controls
{
	public class PartiallyRoundedRectangle : Shape
	{
		public static readonly DependencyProperty RadiusXProperty = DependencyProperty.Register(
			nameof(RadiusX),
			typeof(int),
			typeof(PartiallyRoundedRectangle),
			new PropertyMetadata(0));

		public int RadiusX
		{
			get => (int)GetValue(RadiusXProperty);
			set => SetValue(RadiusXProperty, value);
		}

		public static readonly DependencyProperty RadiusYProperty = DependencyProperty.Register(
			nameof(RadiusY),
			typeof(int),
			typeof(PartiallyRoundedRectangle),
			new PropertyMetadata(0));

		public int RadiusY
		{
			get => (int)GetValue(RadiusYProperty);
			set => SetValue(RadiusYProperty, value);
		}

		public static readonly DependencyProperty RoundTopLeftProperty = DependencyProperty.Register(
			nameof(RoundTopLeft),
			typeof(bool),
			typeof(PartiallyRoundedRectangle),
			new PropertyMetadata(false));

		public bool RoundTopLeft
		{
			get => (bool)GetValue(RoundTopLeftProperty);
			set => SetValue(RoundTopLeftProperty, value);
		}

		public static readonly DependencyProperty RoundTopRightProperty = DependencyProperty.Register(
			nameof(RoundTopRight),
			typeof(bool),
			typeof(PartiallyRoundedRectangle),
			new PropertyMetadata(false));

		public bool RoundTopRight
		{
			get => (bool)GetValue(RoundTopRightProperty);
			set => SetValue(RoundTopRightProperty, value);
		}

		public static readonly DependencyProperty RoundBottomLeftProperty = DependencyProperty.Register(
			nameof(RoundBottomLeft),
			typeof(bool),
			typeof(PartiallyRoundedRectangle),
			new PropertyMetadata(false));

		public bool RoundBottomLeft
		{
			get => (bool)GetValue(RoundBottomLeftProperty);
			set => SetValue(RoundBottomLeftProperty, value);
		}

		public static readonly DependencyProperty RoundBottomRightProperty = DependencyProperty.Register(
			nameof(RoundBottomRight),
			typeof(bool),
			typeof(PartiallyRoundedRectangle),
			new PropertyMetadata(false));

		public bool RoundBottomRight
		{
			get => (bool)GetValue(RoundBottomRightProperty);
			set => SetValue(RoundBottomRightProperty, value);
		}

		protected override Geometry DefiningGeometry
		{
			get
			{
				Geometry result = new RectangleGeometry(new Rect(0.0d, 0.0d, Width, Height), RadiusX, RadiusY);
				var halfWidth = Width / 2;
				var halfHeight = Height / 2;

				if (!RoundTopLeft)
				{
					var rectangleGeometry = new RectangleGeometry(new Rect(0.0d, 0.0d, halfWidth, halfHeight));
					result = new CombinedGeometry(GeometryCombineMode.Union, result, rectangleGeometry);
				}

				if (!RoundTopRight)
				{
					var rectangleGeometry = new RectangleGeometry(new Rect(halfWidth, 0.0d, halfWidth, halfHeight));
					result = new CombinedGeometry(GeometryCombineMode.Union, result, rectangleGeometry);
				}

				if (!RoundBottomLeft)
				{
					var rectangleGeometry = new RectangleGeometry(new Rect(0.0d, halfHeight, halfWidth, halfHeight));
					result = new CombinedGeometry(GeometryCombineMode.Union, result, rectangleGeometry);
				}

				if (!RoundBottomRight)
				{
					var rectangleGeometry = new RectangleGeometry(new Rect(halfWidth, halfHeight, halfWidth, halfHeight));
					result = new CombinedGeometry(GeometryCombineMode.Union, result, rectangleGeometry);
				}

				return result;
			}
		}
	}
}
