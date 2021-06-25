using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;

namespace CustomEditor.Controls.Thumbs
{
	public class RotateThumb : Thumb
	{
		private Point _centerPoint;
		private Vector _startVector;
		private double _initialAngle;
		private Canvas _designerCanvas;
		private UIElement _designerItem;
		private RotateTransform _rotateTransform;

		public RotateThumb()
		{
			DragDelta += new DragDeltaEventHandler(RotateThumb_DragDelta);
			DragStarted += new DragStartedEventHandler(RotateThumb_DragStarted);
		}

		private void RotateThumb_DragStarted(object sender, DragStartedEventArgs e)
		{
			_designerItem = DataContext as UIElement;

			if (_designerItem != null)
			{
				_designerCanvas = VisualTreeHelper.GetParent(_designerItem) as Canvas;

				if (_designerCanvas != null)
				{
					_centerPoint = _designerItem.TranslatePoint(
						new Point((_designerItem as FrameworkElement).Width * _designerItem.RenderTransformOrigin.X,
							(_designerItem as FrameworkElement).Height * _designerItem.RenderTransformOrigin.Y),
						_designerCanvas);

					Point startPoint = Mouse.GetPosition(_designerCanvas);
					_startVector = Point.Subtract(startPoint, _centerPoint);

					_rotateTransform = _designerItem.RenderTransform as RotateTransform;
					if (_rotateTransform == null)
					{
						_designerItem.RenderTransform = new RotateTransform(0);
						_initialAngle = 0;
					}
					else
					{
						_initialAngle = _rotateTransform.Angle;
					}
				}
			}
		}

		private void RotateThumb_DragDelta(object sender, DragDeltaEventArgs e)
		{
			if (_designerItem != null && _designerCanvas != null)
			{
				Point currentPoint = Mouse.GetPosition(_designerCanvas);
				Vector deltaVector = Point.Subtract(currentPoint, _centerPoint);

				double angle = Vector.AngleBetween(_startVector, deltaVector);

				RotateTransform rotateTransform = _designerItem.RenderTransform as RotateTransform;
				rotateTransform.Angle = _initialAngle + Math.Round(angle, 0);
				_designerItem.InvalidateMeasure();
			}
		}
	}
}
