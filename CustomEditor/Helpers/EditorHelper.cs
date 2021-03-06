using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using CustomEditor.Controls;

namespace CustomEditor.Helpers
{
	internal static class EditorHelper
	{
		public static void UpdatePolylineLayoutProperties(ref AdvancedPolyline polyline)
		{
			var pointsCollection = polyline.Points.Clone();
			var bounds = new Rect
			{
				X = double.MaxValue,
				Y = double.MaxValue,
				Width = 0.0d,
				Height = 0.0d
			};

			foreach (var point in pointsCollection)
			{
				if (point.X < bounds.X)
					bounds.X = point.X;

				if (point.Y < bounds.Y)
					bounds.Y = point.Y;

				if (point.X > bounds.Width)
					bounds.Width = point.X;

				if (point.Y > bounds.Height)
					bounds.Height = point.Y;
			}

			bounds.Width -= bounds.X;
			bounds.Height -= bounds.Y;
			for (var i = 0; i < pointsCollection.Count; i++)
			{
				var newPoint = new Point
				{
					X = pointsCollection[i].X - bounds.X,
					Y = pointsCollection[i].Y - bounds.Y
				};

				pointsCollection[i] = newPoint;
			}

			polyline.Points = pointsCollection.Clone();
			pointsCollection.Clear();

			var polylineThickness = polyline.StrokeThickness;

			Canvas.SetLeft(polyline, bounds.X - polylineThickness / 2.0);
			Canvas.SetTop(polyline, bounds.Y - polylineThickness / 2.0);

			polyline.Width = bounds.Width + polylineThickness;
			polyline.Height = bounds.Height + polylineThickness;
			polyline.Fill = Brushes.Transparent;
			polyline.Stretch = Stretch.Fill;
		}

		public static void UpdateRectangleLayoutProperties(ref AdvancedRectangle rectangle)
		{
			var x = Mouse.GetPosition(rectangle);

			/*
			var pointsCollection = rectangle.Points.Clone();
			var bounds = new Rect
			{
				X = double.MaxValue,
				Y = double.MaxValue,
				Width = 0.0d,
				Height = 0.0d
			};

			foreach (var point in pointsCollection)
			{
				if (point.X < bounds.X)
					bounds.X = point.X;

				if (point.Y < bounds.Y)
					bounds.Y = point.Y;

				if (point.X > bounds.Width)
					bounds.Width = point.X;

				if (point.Y > bounds.Height)
					bounds.Height = point.Y;
			}

			bounds.Width -= bounds.X;
			bounds.Height -= bounds.Y;
			for (var i = 0; i < pointsCollection.Count; i++)
			{
				var newPoint = new Point
				{
					X = pointsCollection[i].X - bounds.X,
					Y = pointsCollection[i].Y - bounds.Y
				};

				pointsCollection[i] = newPoint;
			}

			rectangle.Points = pointsCollection.Clone();
			pointsCollection.Clear();

			var polylineThickness = rectangle.StrokeThickness;

			Canvas.SetLeft(rectangle, bounds.X - polylineThickness / 2.0);
			Canvas.SetTop(rectangle, bounds.Y - polylineThickness / 2.0);

			rectangle.Width = bounds.Width + polylineThickness;
			rectangle.Height = bounds.Height + polylineThickness;
			rectangle.Fill = Brushes.Transparent;
			rectangle.Stretch = Stretch.Fill;

			*/
		}

		public static Rect BoundsRelativeTo(this FrameworkElement element, Visual relativeTo)
		{
			return element.TransformToVisual(relativeTo).TransformBounds(LayoutInformation.GetLayoutSlot(element));
		}
	}
}
