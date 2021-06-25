using System;
using System.Windows;
using System.Windows.Controls.Primitives;

namespace CustomEditor.Controls.Adorners
{
	public class BaseThumb : Thumb
	{
		protected FrameworkElement ControlledItem;
		public BaseThumb(FrameworkElement el) : base()
		{
			ControlledItem = el ?? throw new ArgumentNullException();

			DragDelta += new DragDeltaEventHandler(OnDragDelta);
			DragStarted += new DragStartedEventHandler(OnDragStarted);
			DragCompleted += new DragCompletedEventHandler(OnDragCompleted);
		}

		protected virtual void OnDragDelta(object sender, DragDeltaEventArgs e)
		{
		}

		protected virtual void OnDragStarted(object sender, DragStartedEventArgs e)
		{
		}

		protected virtual void OnDragCompleted(object sender, DragCompletedEventArgs e)
		{
		}
	}
}
