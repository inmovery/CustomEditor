using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace CustomEditor.Helpers
{
	public static class EditorHelper
	{
		public static void UpdatePolylineLayoutProperties(ref Polyline polyline)
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
	}
}
