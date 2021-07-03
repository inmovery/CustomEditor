using System.Windows;
using CustomEditor.Helpers;

namespace CustomEditor.Controls.Thumbs
{
	public class PointDragThumb : BaseThumb
	{
		public PointDragThumb(FrameworkElement adornedElement) : base(adornedElement)
		{
			Style = AncestorHelper.FindActiveWindow().FindResource("PointDragThumb") as Style;
		}
	}
}
