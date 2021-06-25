using System.Windows.Input;
using System.Windows.Shapes;
using Microsoft.Xaml.Behaviors;

namespace CustomEditor.Behaviors
{
	public class PolylineEditBehavior : Behavior<Polyline>
	{
		protected override void OnAttached()
		{
			AssociatedObject.PreviewMouseDoubleClick += PreviewMouseDoubleClick;
			AssociatedObject.PreviewMouseUp += OnPreviewMouseUp;
		}

		protected override void OnDetaching()
		{
			AssociatedObject.PreviewMouseDown -= OnPreviewMouseDown;
			AssociatedObject.PreviewMouseUp -= OnPreviewMouseUp;
		}

		private void OnPreviewMouseDown(object sender, MouseButtonEventArgs eventArgs)
		{
			/*
			var scrollViewer = AssociatedObject.GetChildOfType<ScrollViewer>();
			if (eventArgs.Delta < 0)
				scrollViewer.LineRight();
			else
				scrollViewer.LineLeft();
			*/

			eventArgs.Handled = true;
		}

		public void OnPreviewMouseUp(object sender, MouseButtonEventArgs eventArgs)
		{

		}
	}
}
