using System.Windows;
using System.Windows.Input;
using CustomEditor.Controls.ZoomAndPan;
using CustomEditor.Models;
using CustomEditor.ViewModels;

namespace CustomEditor.Views
{
	public partial class MainWindow : Window
	{
		/// <summary>
		/// Specifies the current state of the mouse handling logic.
		/// </summary>
		private MouseHandlingMode mouseHandlingMode = MouseHandlingMode.None;

		/// <summary>
		/// The point that was clicked relative to the ZoomAndPanControl.
		/// </summary>
		private Point origZoomAndPanControlMouseDownPoint;

		/// <summary>
		/// The point that was clicked relative to the WorkspaceCanvas that is contained within the ZoomAndPanControl.
		/// </summary>
		private Point origContentMouseDownPoint;

		/// <summary>
		/// Records which mouse button clicked during mouse dragging.
		/// </summary>
		private MouseButton mouseButtonDown;

		public MainWindow()
		{
			InitializeComponent();
		}

        /// <summary>
        /// Event raised on mouse down in the ZoomAndPanControl.
        /// </summary>
        private void zoomAndPanControl_MouseDown(object sender, MouseButtonEventArgs e)
        {
	        var vm = DataContext as MainWindowViewModel;
	        if (vm?.SelectedToolType == ToolType.Rectangle)
		        return;

	        Workspace.Focus();
            Keyboard.Focus(Workspace);

            mouseButtonDown = e.ChangedButton;
            origZoomAndPanControlMouseDownPoint = e.GetPosition(zoomAndPanControl);
            origContentMouseDownPoint = e.GetPosition(Workspace);

            if ((Keyboard.Modifiers & ModifierKeys.Shift) != 0 &&
                (e.ChangedButton == MouseButton.Left ||
                 e.ChangedButton == MouseButton.Right))
            {
                // Shift + left- or right-down initiates zooming mode.
                mouseHandlingMode = MouseHandlingMode.Zooming;
            }
            else if (mouseButtonDown == MouseButton.Left)
            {
                // Just a plain old left-down initiates panning mode.
                mouseHandlingMode = MouseHandlingMode.Panning;
            }

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
	        var vm = DataContext as MainWindowViewModel;
	        if (vm?.SelectedToolType == ToolType.Rectangle)
		        return;

            if (mouseHandlingMode != MouseHandlingMode.None)
            {
                if (mouseHandlingMode == MouseHandlingMode.Zooming)
                {
                    if (mouseButtonDown == MouseButton.Left)
                    {
                        // Shift + left-click zooms in on the WorkspaceCanvas.
                        ZoomIn();
                    }
                    else if (mouseButtonDown == MouseButton.Right)
                    {
                        // Shift + left-click zooms out from the WorkspaceCanvas.
                        ZoomOut();
                    }
                }

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
	        var vm = DataContext as MainWindowViewModel;
	        if (vm?.SelectedToolType == ToolType.Rectangle)
		        return;

            if (mouseHandlingMode == MouseHandlingMode.Panning)
            {
                //
                // The user is left-dragging the mouse.
                // Pan the viewport by the appropriate amount.
                //
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
                ZoomIn();
            }
            else if (e.Delta < 0)
            {
                ZoomOut();
            }
        }

        /// <summary>
        /// The 'ZoomIn' command (bound to the plus key) was executed.
        /// </summary>
        private void ZoomIn_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            ZoomIn();
        }

        /// <summary>
        /// The 'ZoomOut' command (bound to the minus key) was executed.
        /// </summary>
        private void ZoomOut_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            ZoomOut();
        }

        /// <summary>
        /// Zoom the viewport out by a small increment.
        /// </summary>
        private void ZoomOut()
        {
            zoomAndPanControl.ContentScale -= 0.1;
        }

        /// <summary>
        /// Zoom the viewport in by a small increment.
        /// </summary>
        private void ZoomIn()
        {
            zoomAndPanControl.ContentScale += 0.1;
        }
    }
}