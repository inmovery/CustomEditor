using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using CustomEditor.Models;
using CustomEditor.Models.Events;

namespace CustomEditor.Controls.Thumbs
{
	public class RotateThumb : Thumb
	{
		private Point _centerPoint;
		private Vector _startVector;
		private double _initialAngle;
		private RotateTransform _rotateTransform;

		private UIElement _adornedElement;

		public RotateThumb()
		{
			DragDelta += new DragDeltaEventHandler(RotateThumb_DragDelta);
			DragStarted += new DragStartedEventHandler(RotateThumb_DragStarted);
			DragCompleted += new DragCompletedEventHandler(RotateThumb_DragCompleted);
		}

		private void RotateThumb_DragStarted(object sender, DragStartedEventArgs e)
		{
			_adornedElement = DataContext as UIElement;
			if (_adornedElement == null)
				return;

			var canvas = VisualTreeHelper.GetParent(_adornedElement) as CustomCanvas;
			if (canvas == null)
				return;

			var element = _adornedElement as FrameworkElement ?? new FrameworkElement();
			_centerPoint = _adornedElement.TranslatePoint(
				new Point(
					element.Width * _adornedElement.RenderTransformOrigin.X,
					element.Height * _adornedElement.RenderTransformOrigin.Y),
				canvas);

			var startPoint = Mouse.GetPosition(canvas);
			_startVector = Point.Subtract(startPoint, _centerPoint);

			_rotateTransform = _adornedElement.RenderTransform as RotateTransform;
			if (_rotateTransform == null)
			{
				_adornedElement.RenderTransform = new RotateTransform(0);
				_initialAngle = 0;
			}
			else
				_initialAngle = _rotateTransform.Angle;
		}

		private void RotateThumb_DragDelta(object sender, DragDeltaEventArgs e)
		{
			var canvas = VisualTreeHelper.GetParent(_adornedElement) as CustomCanvas;
			if (_adornedElement == null || canvas == null)
				return;

			var currentPoint = Mouse.GetPosition(canvas);
			var deltaVector = Point.Subtract(currentPoint, _centerPoint);

			var angle = Vector.AngleBetween(_startVector, deltaVector);

			var rotateTransform = _adornedElement.RenderTransform as RotateTransform;
			if (rotateTransform != null)
				rotateTransform.Angle = _initialAngle + Math.Round(angle, 0);

			_adornedElement.InvalidateMeasure();
		}

		private void RotateThumb_DragCompleted(object sender, DragCompletedEventArgs e)
		{
			// UpdatePropertiesState();
		}
	}
}
