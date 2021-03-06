using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;

namespace CustomEditor.Controls
{
	public class AdvancedPolyline : Shape
	{
		public static readonly DependencyProperty PointsProperty = DependencyProperty.Register(
			nameof(Points),
			typeof(PointCollection),
			typeof(AdvancedPolyline),
			new FrameworkPropertyMetadata(new PointCollection(), FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsRender));

		public PointCollection Points
		{
			get => (PointCollection)GetValue(PointsProperty);
			set => SetValue(PointsProperty, value);
		}

		public static readonly DependencyProperty BorderColorProperty = DependencyProperty.Register(
			nameof(BorderColor),
			typeof(Color),
			typeof(AdvancedPolyline),
			new FrameworkPropertyMetadata(Colors.LightSkyBlue, FrameworkPropertyMetadataOptions.AffectsRender));

		public Color BorderColor
		{
			get => (Color)GetValue(BorderColorProperty);
			set => SetValue(BorderColorProperty, value);
		}

		public static readonly DependencyProperty FillColorProperty = DependencyProperty.Register(
			nameof(FillColor),
			typeof(Color),
			typeof(AdvancedPolyline),
			new FrameworkPropertyMetadata(Colors.Black, FrameworkPropertyMetadataOptions.AffectsRender));

		public Color FillColor
		{
			get => (Color)GetValue(FillColorProperty);
			set => SetValue(FillColorProperty, value);
		}

		protected override Geometry DefiningGeometry => DefineGeometry();

		private Geometry DefineGeometry()
		{
			Geometry polylineGeometry;
			var points = Points;
			var pathFigure = new PathFigure();
			if (points == null)
				polylineGeometry = Geometry.Empty;
			else
			{
				if (points.Count > 0)
				{
					pathFigure.StartPoint = points[0];
					if (points.Count > 1)
					{
						var pointArray = new Point[points.Count - 1];
						for (var index = 1; index < points.Count; ++index)
							pointArray[index - 1] = points[index];

						pathFigure.Segments.Add(new PolyLineSegment(pointArray, true));
					}
				}

				var pathGeometry = new PathGeometry();
				pathGeometry.Figures.Add(pathFigure);
				pathGeometry.FillRule = FillRule.EvenOdd;

				polylineGeometry = pathGeometry.Bounds == Rect.Empty ? Geometry.Empty : pathGeometry;
			}

			return polylineGeometry;
		}
	}
}
