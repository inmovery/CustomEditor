using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using CustomEditor.Controls;
using CustomEditor.Helpers;
using CustomEditor.ViewModels;

namespace CustomEditor.Views
{
	public partial class MainWindow : Window
	{
		public MainWindow()
		{
			InitializeComponent();
		}

		/*
		/// <summary>
		/// Specifies the current state of the mouse handling logic.
		/// </summary>
		private MouseHandlingMode mouseHandlingMode = MouseHandlingMode.None;

		/// <summary>
		/// The point that was clicked relative to the ZoomAndPanControl.
		/// </summary>
		private Point origZoomAndPanControlMouseDownPoint;

		/// <summary>
		/// The point that was clicked relative to the content that is contained within the ZoomAndPanControl.
		/// </summary>
		private Point origContentMouseDownPoint;

		/// <summary>
		/// Records which mouse button clicked during mouse dragging.
		/// </summary>
		private MouseButton mouseButtonDown;

		/// <summary>
		/// Saves the previous zoom rectangle, pressing the backspace key jumps back to this zoom rectangle.
		/// </summary>
		private Rect prevZoomRect;

		/// <summary>
		/// Save the previous content scale, pressing the backspace key jumps back to this scale.
		/// </summary>
		private double prevZoomScale;

		/// <summary>
		/// Set to 'true' when the previous zoom rect is saved.
		/// </summary>
		private bool prevZoomRectSet = false;

		/// <summary>
		/// Event raised when the Window has loaded.
		/// </summary>
		private void MainWindow_Loaded(object sender, RoutedEventArgs e)
		{
			ExpandContent();
		}

		/// <summary>
		/// Expand the content area to fit the rectangles.
		/// </summary>
		private void ExpandContent()
		{
			double xOffset = 0;
			double yOffset = 0;
			Rect contentRect = new Rect(0, 0, 0, 0);
			foreach (var element in Workspace.Children)
			{
				var uiElement = element as FrameworkElement;
				var data = uiElement.BoundsRelativeTo(Workspace);

				if (data.X < xOffset)
				{
					xOffset = data.X;
				}

				if (data.Y < yOffset)
				{
					yOffset = data.Y;
				}

				contentRect.Union(new Rect(data.X, data.Y, data.Width, data.Height));
			}

			//
			// Translate all rectangles so they are in positive space.
			//
			xOffset = Math.Abs(xOffset);
			yOffset = Math.Abs(yOffset);

			foreach (var element in Workspace.Children)
			{
				var uiElement = element as FrameworkElement;
				var data = uiElement.BoundsRelativeTo(Workspace);

				data.X += xOffset;
				data.Y += yOffset;
			}

			((MainWindowViewModel) DataContext).ContentWidth = contentRect.Width;
			((MainWindowViewModel) DataContext).ContentHeight = contentRect.Height;
		}

		/// <summary>
		/// Event raised on mouse down in the ZoomAndPanControl.
		/// </summary>
		private void zoomAndPanControl_MouseDown(object sender, MouseButtonEventArgs e)
		{
			Workspace.Focus();
			Keyboard.Focus(Workspace);

			mouseButtonDown = e.ChangedButton;
			origZoomAndPanControlMouseDownPoint = e.GetPosition(zoomAndPanControl);
			origContentMouseDownPoint = e.GetPosition(Workspace);

			mouseHandlingMode = MouseHandlingMode.Panning;

			if (mouseHandlingMode != MouseHandlingMode.None)
			{
				// Capture the mouse so that we eventually receive the mouse up event.
				zoomAndPanControl.CaptureMouse();
				e.Handled = true;
			}
		}


		/// <summary>
		/// Event raised on mouse up in the ZoomAndPanControl.
		/// </summary>
		private void zoomAndPanControl_MouseUp(object sender, MouseButtonEventArgs e)
		{
			if (mouseHandlingMode != MouseHandlingMode.None)
			{
				zoomAndPanControl.ReleaseMouseCapture();
				mouseHandlingMode = MouseHandlingMode.None;
				e.Handled = true;
			}
		}

		/// <summary>
		/// Event raised on mouse move in the ZoomAndPanControl.
		/// </summary>
		private void zoomAndPanControl_MouseMove(object sender, MouseEventArgs e)
		{
			if (mouseHandlingMode == MouseHandlingMode.Panning)
			{
				Point curContentMousePoint = e.GetPosition(Workspace);
				Vector dragOffset = curContentMousePoint - origContentMouseDownPoint;

				zoomAndPanControl.ContentOffsetX -= dragOffset.X;
				zoomAndPanControl.ContentOffsetY -= dragOffset.Y;

				e.Handled = true;
			}
		}

		/// <summary>
		/// Event raised by rotating the mouse wheel
		/// </summary>
		private void zoomAndPanControl_MouseWheel(object sender, MouseWheelEventArgs e)
		{
			e.Handled = true;

			if (e.Delta > 0)
			{
				Point curContentMousePoint = e.GetPosition(Workspace);
				zoomAndPanControl.LineLeft();
			}
			else if (e.Delta < 0)
			{
				Point curContentMousePoint = e.GetPosition(Workspace);
				zoomAndPanControl.LineRight();
			}
		}

		/// <summary>
		/// Event raised when a mouse button is clicked down over a Rectangle.
		/// </summary>
		private void Rectangle_MouseDown(object sender, MouseButtonEventArgs e)
		{
			Workspace.Focus();
			Keyboard.Focus(Workspace);

			if ((Keyboard.Modifiers & ModifierKeys.Shift) != 0)
			{
				//
				// When the shift key is held down special zooming logic is executed in content_MouseDown,
				// so don't handle mouse input here.
				//
				return;
			}

			if (mouseHandlingMode != MouseHandlingMode.None)
			{
				//
				// We are in some other mouse handling mode, don't do anything.
				return;
			}

			mouseHandlingMode = MouseHandlingMode.DraggingRectangles;
			origContentMouseDownPoint = e.GetPosition(Workspace);

			Rectangle rectangle = (Rectangle)sender;
			rectangle.CaptureMouse();

			e.Handled = true;
		}

		/// <summary>
		/// Event raised when a mouse button is released over a Rectangle.
		/// </summary>
		private void Rectangle_MouseUp(object sender, MouseButtonEventArgs e)
		{
			if (mouseHandlingMode != MouseHandlingMode.DraggingRectangles)
			{
				//
				// We are not in rectangle dragging mode.
				//
				return;
			}

			mouseHandlingMode = MouseHandlingMode.None;

			Rectangle rectangle = (Rectangle)sender;
			rectangle.ReleaseMouseCapture();

			e.Handled = true;
		}

		/// <summary>
		/// Event raised when the mouse cursor is moved when over a Rectangle.
		/// </summary>
		private void Rectangle_MouseMove(object sender, MouseEventArgs e)
		{
			if (mouseHandlingMode != MouseHandlingMode.DraggingRectangles)
			{
				//
				// We are not in rectangle dragging mode, so don't do anything.
				//
				return;
			}

			Point curContentPoint = e.GetPosition(Workspace);
			Vector rectangleDragVector = curContentPoint - origContentMouseDownPoint;

			//
			// When in 'dragging rectangles' mode update the position of the rectangle as the user drags it.
			//

			origContentMouseDownPoint = curContentPoint;

			Rectangle rectangle = (Rectangle)sender;
			Canvas.SetLeft(rectangle, Canvas.GetLeft(rectangle) + rectangleDragVector.X);
			Canvas.SetTop(rectangle, Canvas.GetTop(rectangle) + rectangleDragVector.Y);

			ExpandContent();

			e.Handled = true;
		}
		*/

	}
}