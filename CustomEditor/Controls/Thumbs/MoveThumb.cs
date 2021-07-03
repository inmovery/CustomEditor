using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;

namespace CustomEditor.Controls.Thumbs
{
	public class MoveThumb : Thumb
	{
		private RotateTransform _rotateTransform;
		private UIElement _adornedElement;

		public MoveThumb()
		{
			DragStarted += new DragStartedEventHandler(OnDragStarted);
			DragDelta += new DragDeltaEventHandler(OnDragDelta);
		}

		private void OnDragStarted(object sender, DragStartedEventArgs eventArgs)
		{
			_adornedElement = DataContext as UIElement;

			var workspaceCanvas = VisualTreeHelper.GetParent(_adornedElement) as Canvas;
			// workspaceCanvas.CreateRestorePoint();

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
