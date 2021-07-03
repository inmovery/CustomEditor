using System;
using System.Runtime.InteropServices;

namespace CustomEditor.Models.Events
{
	[ComVisible(true)]
	[Serializable]
	public delegate void SelectedItemChangedEventHandler(object sender, SelectedItemChangedEventArgs eventArgs);
}