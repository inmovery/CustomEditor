using System.Threading;
using System.Windows;

namespace CustomEditor.Helpers
{
	internal static class Invoking
	{
		public static bool IsMainThread => Application.Current == null || Thread.CurrentThread == Application.Current.Dispatcher.Thread;
	}
}
