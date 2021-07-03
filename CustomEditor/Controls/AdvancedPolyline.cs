using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;
using CustomEditor.ViewModels;

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

		public static readonly DependencyProperty FillRuleProperty = DependencyProperty.Register(
			nameof(FillRule),
			typeof(FillRule),
			typeof(AdvancedPolyline),
			new FrameworkPropertyMetadata(FillRule.EvenOdd, FrameworkPropertyMetadataOptions.AffectsRender));

		public FillRule FillRule
		{
			get => (FillRule)GetValue(FillRuleProperty);
			set => SetValue(FillRuleProperty, value);
		}

		protected override Geometry DefiningGeometry => DefineGeometry();

		private Geometry DefineGeometry()
		{
			var _polylineGeometry = Geometry.Empty;
			var points = Points;
			var pathFigure = new PathFigure();
			if (points == null)
				_polylineGeometry = Geometry.Empty;
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
				pathGeometry.FillRule = FillRule;

				_polylineGeometry = pathGeometry.Bounds == Rect.Empty ? Geometry.Empty : pathGeometry;
			}

			return _polylineGeometry;
		}
	}
}
