using System;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using CustomEditor.Controls.AdornerChrome;

namespace CustomEditor.Controls.Adorners
{
	public class RectangleAdorner : Adorner
	{
		protected VisualCollection Visuals;
		private UIElement rectangleBaseChrome;

		public RectangleAdorner(UIElement adornedElement) : base(adornedElement)
		{
			Visuals = new VisualCollection(this);
			DataContext = adornedElement;

			rectangleBaseChrome = new RectangleChrome();
			Visuals.Add(rectangleBaseChrome);
		}

		public event Action<UIElement> DoubleClickExecuted;

		protected override void OnPreviewMouseLeftButtonDown(MouseButtonEventArgs eventArgs)
		{
			if (eventArgs.ClickCount < 2)
				return;

			DoubleClickExecuted?.Invoke(AdornedElement);
		}

		protected override Size ArrangeOverride(Size arrangeBounds)
		{
			var finalRect = new Rect(arrangeBounds);
			rectangleBaseChrome.Arrange(finalRect);

			return arrangeBounds;
		}

		protected override int VisualChildrenCount => Visuals.Count;
		protected override Visual GetVisualChild(int index) => Visuals[index];
	}
}
