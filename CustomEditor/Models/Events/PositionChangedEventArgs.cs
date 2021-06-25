using System;
using System.Windows;

namespace CustomEditor.Models.Events
{
	public sealed class PositionChangedEventArgs : EventArgs
	{
		public Point Position { get; set; }
		public Point PreviousPosition { get; set; }

		public PositionChangedEventArgs(Point position, Point previousPosition)
		{
			Position = position;
			PreviousPosition = previousPosition;
		}
	}
}
