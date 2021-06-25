using System.Windows;
using CustomEditor.Helpers;

namespace CustomEditor.Controls.Adorners
{
	public class PointDragThumb : BaseThumb
	{
		public PointDragThumb(FrameworkElement el) : base(el)
		{
			Style = AncestorHelper.FindActiveWindow().FindResource("PointDragThumb") as Style;
		}
	}
}
