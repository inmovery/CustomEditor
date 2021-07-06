using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using CustomEditor.Helpers;

namespace CustomEditor.Controls.Thumbs
{
	public class LinearMoveThumb : Thumb
	{
		private RotateTransform _rotateTransform;
		private UIElement _adornedElement;

		public LinearMoveThumb()
		{
			Style = AncestorHelper.FindActiveWindow().FindResource("PointDragThumb") as Style;
			DragStarted += new DragStartedEventHandler(OnDragStarted);
			DragDelta += new DragDeltaEventHandler(OnDragDelta);
		}

		private void OnDragStarted(object sender, DragStartedEventArgs eventArgs)
		{
			_adornedElement = DataContext as UIElement;

			var workspaceCanvas = VisualTreeHelper.GetParent(_adornedElement) as CustomCanvas;

			if (_adornedElement != null)
				_rotateTransform = _adornedElement.RenderTransform as RotateTransform;
		}

		private void OnDragDelta(object sender, DragDeltaEventArgs eventArgs)
		{
			_adornedElement = (DataContext as UIElement) ?? new UIElement();

			var dragDelta = new Point(eventArgs.HorizontalChange, eventArgs.VerticalChange);
			if (_rotateTransform != null)
				dragDelta = _rotateTransform.Transform(dragDelta);

			Canvas.SetLeft(_adornedElement, Canvas.GetLeft(_adornedElement) + dragDelta.X);
			Canvas.SetTop(_adornedElement, Canvas.GetTop(_adornedElement) + dragDelta.Y);
		}
	}
}
