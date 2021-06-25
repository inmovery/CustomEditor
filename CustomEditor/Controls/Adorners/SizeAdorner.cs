using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;
using CustomEditor.Controls.AdornerChrome;

namespace CustomEditor.Controls.Adorners
{
	public class SizeAdorner : Adorner
	{
		private readonly SizeChrome _chrome;
		private readonly VisualCollection _visuals;

		public SizeAdorner(UIElement adornedElement) : base(adornedElement)
		{
			_chrome = new SizeChrome()
			{
				DataContext = adornedElement
			};

			_visuals = new VisualCollection(this)
			{
				_chrome
			};

			SnapsToDevicePixels = true;
		}

		protected override Size ArrangeOverride(Size arrangeBounds)
		{
			_chrome.Arrange(new Rect(new Point(0.0, 0.0), arrangeBounds));
			return arrangeBounds;
		}

		protected override int VisualChildrenCount => _visuals.Count;
		protected override Visual GetVisualChild(int index) => _visuals[index];
	}
}
