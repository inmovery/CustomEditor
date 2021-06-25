using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace CustomEditor.Controls.Thumbs
{
	public class MoveThumb : Thumb
	{
		private UIElement _adornedElement;

		public MoveThumb()
		{
			DragDelta += new DragDeltaEventHandler(OnDragDelta);
		}

		private void OnDragDelta(object sender, DragDeltaEventArgs e)
		{
			_adornedElement = DataContext as UIElement;

			Canvas.SetLeft(_adornedElement ?? new UIElement(), Canvas.GetLeft(_adornedElement) + e.HorizontalChange);
			Canvas.SetTop(_adornedElement, Canvas.GetTop(_adornedElement) + e.VerticalChange);
		}
	}
}
