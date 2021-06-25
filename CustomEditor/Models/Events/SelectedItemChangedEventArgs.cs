using System;
using System.Windows;

namespace CustomEditor.Models.Events
{
	public class SelectedItemChangedEventArgs : EventArgs
	{
		public UIElement SelectedItem { get; set; }

		public SelectedItemChangedEventArgs(UIElement selectedItem)
		{
			SelectedItem = selectedItem;
		}
	}
}
