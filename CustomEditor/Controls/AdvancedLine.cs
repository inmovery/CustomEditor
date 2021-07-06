using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;

namespace CustomEditor.Controls
{
	public class AdvancedLine : Shape
	{
		public static readonly DependencyProperty X1Property = DependencyProperty.Register(
			nameof(X1),
			typeof(double),
			typeof(AdvancedLine),
			new FrameworkPropertyMetadata(0.0d, FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsRender));

		public double X1
		{
			get => (double)GetValue(X1Property);
			set => SetValue(X1Property, value);
		}

		public static readonly DependencyProperty Y1Property = DependencyProperty.Register(
			nameof(Y1),
			typeof(double),
			typeof(AdvancedLine),
			new FrameworkPropertyMetadata(0.0d, FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsRender));

		public double Y1
		{
			get => (double)GetValue(Y1Property);
			set => SetValue(Y1Property, value);
		}

		public static readonly DependencyProperty X2Property = DependencyProperty.Register(
			nameof(X2),
			typeof(double),
			typeof(AdvancedLine),
			new FrameworkPropertyMetadata(0.0d, FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsRender));

		public double X2
		{
			get => (double)GetValue(X2Property);
			set => SetValue(X2Property, value);
		}

		public static readonly DependencyProperty Y2Property = DependencyProperty.Register(
			nameof(Y2),
			typeof(double),
			typeof(AdvancedLine),
			new FrameworkPropertyMetadata(0.0d, FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsRender));

		public double Y2
		{
			get => (double)GetValue(Y2Property);
			set => SetValue(Y2Property, value);
		}

		public static readonly DependencyProperty PreviewStartProperty = DependencyProperty.Register(
			nameof(PreviewStart),
			typeof(Point),
			typeof(AdvancedLine),
			new FrameworkPropertyMetadata(default(Point), FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsRender));

		public Point PreviewStart
		{
			get => (Point)GetValue(PreviewStartProperty);
			set => SetValue(PreviewStartProperty, value);
		}

		public static readonly DependencyProperty PreviewEndProperty = DependencyProperty.Register(
			nameof(PreviewEnd),
			typeof(Point),
			typeof(AdvancedLine),
			new FrameworkPropertyMetadata(default(Point), FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsRender));

		public Point PreviewEnd
		{
			get => (Point)GetValue(PreviewEndProperty);
			set => SetValue(PreviewEndProperty, value);
		}

		protected override Geometry DefiningGeometry => new LineGeometry(new Point(X1, Y1), new Point(X2, Y2));
	}
}
