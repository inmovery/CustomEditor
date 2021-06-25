using System;
using System.Linq;
using System.Windows;
using System.Windows.Media;

namespace CustomEditor.Helpers
{
	public static class AncestorHelper
	{
		public static T FindAncestor<T>(this DependencyObject current)
			where T : DependencyObject
		{
			if (current == null)
				return null;
			do
			{
				if (current is T ancestor)
					return ancestor;

				current = VisualTreeHelper.GetParent(current);
			}
			while (current != null);

			return null;
		}

		public static T FindVisualParent<T>(DependencyObject child) where T : DependencyObject
		{
			var parentObject = VisualTreeHelper.GetParent(child);

			if (parentObject == null)
				return null;

			var parent = parentObject as T;
			return parent ?? FindVisualParent<T>(parentObject);
		}

		public static T GetChildOfType<T>(this DependencyObject depObj) where T : DependencyObject
		{
			if (depObj == null)
				return null;

			for (var i = 0; i < VisualTreeHelper.GetChildrenCount(depObj); i++)
			{
				var child = VisualTreeHelper.GetChild(depObj, i);

				var result = (child as T) ?? GetChildOfType<T>(child);
				if (result != null)
					return result;
			}

			return null;
		}

		public static Window FindActiveWindow()
		{
			if (!Invoking.IsMainThread)
				throw new InvalidOperationException("Attempt to access window not from main thread");

			return Application.Current?.Windows.OfType<Window>().SingleOrDefault(x => x.IsActive);
		}
	}
}
