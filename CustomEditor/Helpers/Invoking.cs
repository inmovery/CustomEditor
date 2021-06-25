using System.Threading;
using System.Windows;

namespace CustomEditor.Helpers
{
	public static class Invoking
	{
		public static bool IsMainThread => Application.Current == null || Thread.CurrentThread == Application.Current.Dispatcher.Thread;
	}
}
