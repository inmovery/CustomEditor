using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;

namespace CustomEditor.Controls.Adorners
{
	public class AdornerWithControl : Adorner
	{
		protected VisualCollection VisualChildren;

		public AdornerWithControl(UIElement adornedElement) : base(adornedElement)
		{
			VisualChildren = new VisualCollection(this);
			DataContext = adornedElement;
		}

		protected override int VisualChildrenCount => VisualChildren.Count;
		protected override Visual GetVisualChild(int index) => VisualChildren[index];
	}
}
