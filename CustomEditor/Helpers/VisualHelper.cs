using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;

namespace CustomEditor.Helpers
{
	internal static class VisualHelper
	{
		internal static bool HasAdornerThumbParent(DependencyObject reference)
		{
			if (reference is Thumb)
				return true;

			if (reference == null)
				return false;

			var parent = VisualTreeHelper.GetParent(reference);
			return parent is not ScrollViewer && HasAdornerThumbParent(parent);
		}

		internal static bool HasScrollBarParent(DependencyObject reference)
		{
			if (reference is ScrollBar)
				return true;

			if (reference == null)
				return false;

			var parent = VisualTreeHelper.GetParent(reference);
			return parent is not ScrollViewer && HasScrollBarParent(parent);
		}
	}
}
