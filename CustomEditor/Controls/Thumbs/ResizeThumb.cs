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
		private UIElement _designerItem;
		private Canvas _canvas;

		private const double MinimalSize = 40;

		public ResizeThumb()
		{
			DragStarted += new DragStartedEventHandler(ResizeThumb_DragStarted);
			DragDelta += new DragDeltaEventHandler(ResizeThumb_DragDelta);
			DragCompleted += new DragCompletedEventHandler(ResizeThumb_DragCompleted);
		}

		private void ResizeThumb_DragStarted(object sender, DragStartedEventArgs e)
		{
			_designerItem = DataContext as UIElement;

			if (_designerItem == null)
				return;

			_canvas = VisualTreeHelper.GetParent(_designerItem) as Canvas;
			if (_canvas == null)
				return;

			_transformOrigin = _designerItem.RenderTransformOrigin;
			_rotateTransform = _designerItem.RenderTransform as RotateTransform;
			if (_rotateTransform != null)
				_angle = _rotateTransform.Angle * Math.PI / 180.0;
			else
				_angle = 0.0d;

			var adornerLayer = AdornerLayer.GetAdornerLayer(_canvas);
			if (adornerLayer == null)
				return;

			_adorner = new SizeAdorner(_designerItem);
			adornerLayer.Add(_adorner);
		}

		private void ResizeThumb_DragDelta(object sender, DragDeltaEventArgs e)
		{
			if (_designerItem != null)
			{
				double deltaVertical;
				double deltaHorizontal;

				switch (VerticalAlignment)
				{
					case VerticalAlignment.Bottom when (_designerItem as FrameworkElement).Height + e.VerticalChange > MinimalSize:
						deltaVertical = Math.Min(-e.VerticalChange, (_designerItem as FrameworkElement).ActualHeight - (_designerItem as FrameworkElement).MinHeight);
						Canvas.SetTop(_designerItem, Canvas.GetTop(_designerItem) + (_transformOrigin.Y * deltaVertical * (1 - Math.Cos(-_angle))));
						Canvas.SetLeft(_designerItem, Canvas.GetLeft(_designerItem) - deltaVertical * _transformOrigin.Y * Math.Sin(-_angle));
						(_designerItem as FrameworkElement).Height -= deltaVertical;
						break;

					case VerticalAlignment.Top when (_designerItem as FrameworkElement).Height - e.VerticalChange > MinimalSize:
						deltaVertical = Math.Min(e.VerticalChange, (_designerItem as FrameworkElement).ActualHeight - (_designerItem as FrameworkElement).MinHeight);
						Canvas.SetTop(_designerItem, Canvas.GetTop(_designerItem) + deltaVertical * Math.Cos(-_angle) + (_transformOrigin.Y * deltaVertical * (1 - Math.Cos(-_angle))));
						Canvas.SetLeft(_designerItem, Canvas.GetLeft(_designerItem) + deltaVertical * Math.Sin(-_angle) - (_transformOrigin.Y * deltaVertical * Math.Sin(-_angle)));
						(_designerItem as FrameworkElement).Height -= deltaVertical;
						break;

					default:
						break;
				}

				switch (HorizontalAlignment)
				{
					case HorizontalAlignment.Left when (_designerItem as FrameworkElement).Width - e.HorizontalChange > MinimalSize:
						deltaHorizontal = Math.Min(e.HorizontalChange, (_designerItem as FrameworkElement).ActualWidth - (_designerItem as FrameworkElement).MinWidth);
						Canvas.SetTop(_designerItem, Canvas.GetTop(_designerItem) + deltaHorizontal * Math.Sin(_angle) - _transformOrigin.X * deltaHorizontal * Math.Sin(_angle));
						Canvas.SetLeft(_designerItem, Canvas.GetLeft(_designerItem) + deltaHorizontal * Math.Cos(_angle) + (_transformOrigin.X * deltaHorizontal * (1 - Math.Cos(_angle))));
						(_designerItem as FrameworkElement).Width -= deltaHorizontal;
						break;

					case HorizontalAlignment.Right when (_designerItem as FrameworkElement).Width + e.HorizontalChange > MinimalSize:
						deltaHorizontal = Math.Min(-e.HorizontalChange, (_designerItem as FrameworkElement).ActualWidth - (_designerItem as FrameworkElement).MinWidth);
						Canvas.SetTop(_designerItem, Canvas.GetTop(_designerItem) - _transformOrigin.X * deltaHorizontal * Math.Sin(_angle));
						Canvas.SetLeft(_designerItem, Canvas.GetLeft(_designerItem) + (deltaHorizontal * _transformOrigin.X * (1 - Math.Cos(_angle))));
						(_designerItem as FrameworkElement).Width -= deltaHorizontal;
						break;

					default:
						break;
				}
			}

			e.Handled = true;
		}

		private void ResizeThumb_DragCompleted(object sender, DragCompletedEventArgs e)
		{
			if (_adorner == null)
				return;

			var adornerLayer = AdornerLayer.GetAdornerLayer(_canvas);
			adornerLayer?.Remove(this._adorner);

			_adorner = null;
		}
	}
}
