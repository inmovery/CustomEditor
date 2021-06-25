using System.Windows.Input;
using CustomEditor.Controls;
using Microsoft.Xaml.Behaviors;

namespace CustomEditor.Behaviors
{
	public class MultipleSelectionBehavior : Behavior<CustomCanvas>
	{
		protected override void OnAttached()
		{
			AssociatedObject.PreviewMouseDown += OnPreviewMouseDown;
			AssociatedObject.PreviewMouseUp += OnPreviewMouseUp;
		}

		protected override void OnDetaching()
		{
			AssociatedObject.PreviewMouseDown -= OnPreviewMouseDown;
			AssociatedObject.PreviewMouseUp -= OnPreviewMouseUp;
		}

		private void OnPreviewMouseDown(object sender, MouseButtonEventArgs eventArgs)
		{
			// todo: добавить множественный выбор
			eventArgs.Handled = true;
		}

		public void OnPreviewMouseUp(object sender, MouseButtonEventArgs eventArgs)
		{
		}
	}
}