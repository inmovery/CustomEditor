using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using CustomEditor.Commands;
using CustomEditor.Controls;
using CustomEditor.Controls.ToolPanel;
using CustomEditor.Helpers;
using CustomEditor.Models;
using CustomEditor.ViewModels.Base;

namespace CustomEditor.ViewModels
{
	public class MainWindowViewModel : BaseViewModel
	{
		private bool _isShiftKeyPressed;
		private bool _isCtrlKeyPressed;

		private Point _startPoint = new Point(-1, -1);
		private Point _endPoint = new Point(-1, -1);

		private UIElement _shape;

		private bool _isDrawing;

		public MainWindowViewModel()
		{
			ShapesCollection = new ObservableCollection<Shape>();

			WindowPreviewKeyDownCommand = new RelayCommand<KeyEventArgs>(HandleKeyEvent);
			WindowPreviewKeyUpCommand = new RelayCommand<KeyEventArgs>(HandleKeyEvent);

			CanvasPreviewMouseMoveCommand = new RelayCommand<MouseEventArgs>(HandleCanvasMouseMovements);
			CanvasPreviewMouseDownCommand = new RelayCommand<MouseButtonEventArgs>(HandleCanvasMouseDown);
			CanvasPreviewMouseUpCommand = new RelayCommand<MouseButtonEventArgs>(HandleCanvasMouseUp);

			UndoCommand = new RelayCommand(() => WorkspaceCanvas.UndoLastChange());
			RedoCommand = new RelayCommand(() => WorkspaceCanvas.RedoLastChange());

			ExportCommand = new RelayCommand(ExportCanvas);
			DeleteCommand = new RelayCommand(DeleteSelectedShapes);
		}

		public ICommand ExportCommand { get; }
		public ICommand DeleteCommand { get; }

		public ICommand WindowPreviewKeyDownCommand { get; }
		public ICommand WindowPreviewKeyUpCommand { get; }

		public ICommand CanvasPreviewMouseDownCommand { get; }
		public ICommand CanvasPreviewMouseUpCommand { get; }
		public ICommand CanvasPreviewMouseMoveCommand { get; }

		public ICommand UndoCommand { get; }
		public ICommand RedoCommand { get; }

		public ICommand ShowEditToolCommand { get; }

		public bool IsShiftKeyPressed
		{
			get => _isShiftKeyPressed;
			set
			{
				_isShiftKeyPressed = value;
				RaisePropertyChanged();
			}
		}

		public bool IsCtrlKeyPressed
		{
			get => _isCtrlKeyPressed;
			set
			{
				_isCtrlKeyPressed = value;
				RaisePropertyChanged();
			}
		}

		public bool IsDrawing
		{
			get => _isDrawing;
			set
			{
				_isDrawing = value;
				RaisePropertyChanged();
			}
		}

		public ToolPanel ToolPanel => AncestorHelper.FindActiveWindow()?.FindName("ToolPanel") as ToolPanel;
		public CustomCanvas WorkspaceCanvas => AncestorHelper.FindActiveWindow()?.FindName("Workspace") as CustomCanvas;

		public ToolType SelectedToolType => (ToolPanel?.DataContext as ToolPanelVm)?.ActiveToolType ?? ToolType.Select;

		public bool IsShapeSelected => WorkspaceCanvas.SelectedItem is Shape;

		public ObservableCollection<Shape> ShapesCollection { get; set; }

		private void DeleteSelectedShapes()
		{
			WorkspaceCanvas.DeleteSelectedShapes();
		}

		private void ExportCanvas()
		{
		}

		private void HandleKeyEvent(KeyEventArgs eventArgs)
		{
			IsShiftKeyPressed = Keyboard.Modifiers == ModifierKeys.Shift;
			IsCtrlKeyPressed = Keyboard.Modifiers == ModifierKeys.Control;
		}

		private void HandleMouseMovePolyline(MouseEventArgs eventArgs)
		{
			if (_shape == null)
				return;

			var polyline = _shape as Polyline;
			polyline.Points[polyline.Points.Count - 1] = eventArgs.GetPosition(WorkspaceCanvas);
		}

		private void HandleMouseMoveRectangle(MouseEventArgs eventArgs)
		{
			if (eventArgs.LeftButton == MouseButtonState.Pressed && !IsShiftKeyPressed)
			{
				if (_startPoint.X < 0)
					return;

				_endPoint = eventArgs.GetPosition(WorkspaceCanvas);

				var startX = Math.Min(_startPoint.X, _endPoint.X);
				var startY = Math.Min(_startPoint.Y, _endPoint.Y);
				var endX = Math.Max(_startPoint.X, _endPoint.X);
				var endY = Math.Max(_startPoint.Y, _endPoint.Y);

				if (_shape == null)
				{
					GenerateRectangle();
					WorkspaceCanvas.Children.Add(_shape);
				}
				else
				{
					var rectangleShape = _shape as PartiallyRoundedRectangle;
					if (rectangleShape == null)
						return;

					rectangleShape.Width = (endX - startX);
					rectangleShape.Height = (endY - startY);
					rectangleShape.Margin = new Thickness(startX, startY, 0, 0);

					_shape = rectangleShape;
				}
			}
		}

		private void HandleMouseDownPolyline(MouseButtonEventArgs eventArgs)
		{
			var test = eventArgs.GetPosition(WorkspaceCanvas);
			if (_shape != null)
			{
				var polyline = _shape as Polyline;
				if (eventArgs.ChangedButton == MouseButton.Left)
					polyline.Points.Add(eventArgs.GetPosition(WorkspaceCanvas));
				else
				{
					EditorHelper.UpdatePolylineLayoutProperties(ref polyline);

					IsDrawing = false;
					_shape = null;
				}
			}
			else
			{
				var initialPosition = eventArgs.GetPosition(WorkspaceCanvas);
				GeneratePolyline(initialPosition);
				WorkspaceCanvas.Children.Add(_shape);
				IsDrawing = true;
			}
		}

		private void HandleMouseDownRectangle(MouseButtonEventArgs eventArgs)
		{
			_startPoint = eventArgs.GetPosition(WorkspaceCanvas);
			_shape = null;
		}

		private void HandleCanvasMouseMovements(MouseEventArgs eventArgs)
		{
			if (IsShiftKeyPressed)
				return;

			if (SelectedToolType == ToolType.Polyline)
				HandleMouseMovePolyline(eventArgs);

			if (SelectedToolType == ToolType.Rectangle)
				HandleMouseMoveRectangle(eventArgs);
		}

		private void HandleCanvasMouseDown(MouseButtonEventArgs eventArgs)
		{
			if (IsShiftKeyPressed || IsCtrlKeyPressed)
				return;

			if (SelectedToolType == ToolType.Polyline)
				HandleMouseDownPolyline(eventArgs);

			if (SelectedToolType == ToolType.Rectangle)
				HandleMouseDownRectangle(eventArgs);
		}

		private void HandleCanvasMouseUp(MouseButtonEventArgs eventArgs)
		{
			if (SelectedToolType != ToolType.Rectangle)
				return;

			WorkspaceCanvas.AddSelectionAdorner(_shape);

			_shape = null;
			_startPoint.X = -1;
			_startPoint.Y = -1;
			_endPoint.X = -1;
			_endPoint.Y = -1;
		}

		private void GenerateRectangle()
		{
			var startX = Math.Min(_startPoint.X, _endPoint.X);
			var startY = Math.Min(_startPoint.Y, _endPoint.Y);
			var endX = Math.Max(_startPoint.X, _endPoint.X);
			var endY = Math.Max(_startPoint.Y, _endPoint.Y);

			_shape = new PartiallyRoundedRectangle()
			{
				Fill = new SolidColorBrush(Colors.Black),
				Width = (endX - startX),
				Height = (endY - startY),
				MinWidth = 15,
				MinHeight = 15,
				RenderTransformOrigin = new Point(0.5, 0.5),
				SnapsToDevicePixels = true,
				StrokeThickness = 2.0d,
				Margin = new Thickness(startX, startY, 0.0d, 0.0d)
			};

			Canvas.SetLeft(_shape, 0.0d);
			Canvas.SetTop(_shape, 0.0d);
		}

		private void GeneratePolyline(Point initialPosition)
		{
			_shape = new Polyline()
			{
				Stroke = Brushes.Black,
				StrokeThickness = 8,
				MinWidth = 15,
				MinHeight = 15,
				RenderTransformOrigin = new Point(0.5, 0.5),
				StrokeStartLineCap = PenLineCap.Round,
				StrokeEndLineCap = PenLineCap.Round,
				StrokeLineJoin = PenLineJoin.Round,
				Points = new PointCollection
				{
					initialPosition,
					initialPosition
				}
			};

			Canvas.SetLeft(_shape, 0.0d);
			Canvas.SetTop(_shape, 0.0d);
		}
	}
}
