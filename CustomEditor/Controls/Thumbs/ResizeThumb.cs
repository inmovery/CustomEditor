using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Documents;
using System.Windows.Media;
using CustomEditor.Controls.Adorners;

namespace CustomEditor.Controls.Thumbs
{
	public class ResizeThumb : Thumb
	{
		private RotateTransform _rotateTransform;
		private double _angle;
		private Adorner _adorner;
		private Point _transformOrigin;

		private CustomCanvas _canvas;
		private UIElement _adornedElement;

		private const double MinimalSize = 40;

		public ResizeThumb()
		{
			DragStarted += new DragStartedEventHandler(ResizeThumb_DragStarted);
			DragDelta += new DragDeltaEventHandler(ResizeThumb_DragDelta);
			DragCompleted += new DragCompletedEventHandler(ResizeThumb_DragCompleted);
		}

		private void ResizeThumb_DragStarted(object sender, DragStartedEventArgs e)
		{
			_adornedElement = DataContext as UIElement;

			if (_adornedElement == null)
				return;

			_canvas = VisualTreeHelper.GetParent(_adornedElement) as CustomCanvas;
			if (_canvas == null)
				return;

			_transformOrigin = _adornedElement.RenderTransformOrigin;
			_rotateTransform = _adornedElement.RenderTransform as RotateTransform;
			if (_rotateTransform != null)
				_angle = _rotateTransform.Angle * Math.PI / 180.0;
			else
				_angle = 0.0d;

			var adornerLayer = AdornerLayer.GetAdornerLayer(_canvas);
			if (adornerLayer == null)
				return;

			_adorner = new SizeAdorner(_adornedElement);
			adornerLayer.Add(_adorner);
		}

		private void ResizeThumb_DragDelta(object sender, DragDeltaEventArgs eventArgs)
		{
			if (_adornedElement != null)
			{
				double deltaVertical;
				double deltaHorizontal;

				var element = _adornedElement as FrameworkElement ?? new FrameworkElement();
				switch (VerticalAlignment)
				{
					case VerticalAlignment.Bottom when element.Height + eventArgs.VerticalChange > MinimalSize:
						deltaVertical = Math.Min(-eventArgs.VerticalChange, element.ActualHeight - element.MinHeight);
						Canvas.SetTop(_adornedElement, Canvas.GetTop(_adornedElement) + (_transformOrigin.Y * deltaVertical * (1 - Math.Cos(-_angle))));
						Canvas.SetLeft(_adornedElement, Canvas.GetLeft(_adornedElement) - deltaVertical * _transformOrigin.Y * Math.Sin(-_angle));
						element.Height -= deltaVertical;
						break;

					case VerticalAlignment.Top when element.Height - eventArgs.VerticalChange > MinimalSize:
						deltaVertical = Math.Min(eventArgs.VerticalChange, element.ActualHeight - element.MinHeight);
						Canvas.SetTop(_adornedElement, Canvas.GetTop(_adornedElement) + deltaVertical * Math.Cos(-_angle) + (_transformOrigin.Y * deltaVertical * (1 - Math.Cos(-_angle))));
						Canvas.SetLeft(_adornedElement, Canvas.GetLeft(_adornedElement) + deltaVertical * Math.Sin(-_angle) - (_transformOrigin.Y * deltaVertical * Math.Sin(-_angle)));
						element.Height -= deltaVertical;
						break;
				}

				switch (HorizontalAlignment)
				{
					case HorizontalAlignment.Left when element.Width - eventArgs.HorizontalChange > MinimalSize:
						deltaHorizontal = Math.Min(eventArgs.HorizontalChange, element.ActualWidth - element.MinWidth);
						Canvas.SetTop(_adornedElement, Canvas.GetTop(_adornedElement) + deltaHorizontal * Math.Sin(_angle) - _transformOrigin.X * deltaHorizontal * Math.Sin(_angle));
						Canvas.SetLeft(_adornedElement, Canvas.GetLeft(_adornedElement) + deltaHorizontal * Math.Cos(_angle) + (_transformOrigin.X * deltaHorizontal * (1 - Math.Cos(_angle))));
						element.Width -= deltaHorizontal;
						break;

					case HorizontalAlignment.Right when element.Width + eventArgs.HorizontalChange > MinimalSize:
						deltaHorizontal = Math.Min(-eventArgs.HorizontalChange, element.ActualWidth - element.MinWidth);
						Canvas.SetTop(_adornedElement, Canvas.GetTop(_adornedElement) - _transformOrigin.X * deltaHorizontal * Math.Sin(_angle));
						Canvas.SetLeft(_adornedElement, Canvas.GetLeft(_adornedElement) + (deltaHorizontal * _transformOrigin.X * (1 - Math.Cos(_angle))));
						element.Width -= deltaHorizontal;
						break;
				}
			}

			UpdatePropertiesState();

			eventArgs.Handled = true;
		}

		private void ResizeThumb_DragCompleted(object sender, DragCompletedEventArgs e)
		{
			var canvas = VisualTreeHelper.GetParent(_adornedElement) as CustomCanvas;
			canvas?.RaiseSelectedItemChanged();

			if (_adorner == null) 
				return;

			var adornerLayer = AdornerLayer.GetAdornerLayer(canvas);
			adornerLayer?.Remove(_adorner);

			_adorner = null;
		}

		private void UpdatePropertiesState()
		{
			var canvas = VisualTreeHelper.GetParent(_adornedElement) as CustomCanvas;
			canvas?.RaiseSelectedItemChanged();
		}
	}
}
