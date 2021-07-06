﻿using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using CustomEditor.Commands;
using CustomEditor.Controls;
using CustomEditor.Controls.Adorners;
using CustomEditor.Helpers;
using CustomEditor.Models;
using CustomEditor.Models.Events;
using CustomEditor.Services;
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

		private ToolPanelVm _toolData;
		private VisualProperties _shapeProperties;

		private readonly FileService _fileService;
		private readonly CanvasService _canvasService;

		/*
		private double _contentOffsetX;
		private double _contentOffsetY;
		private double _contentWidth;
		private double _contentHeight;
		private double _contentViewportWidth;
		private double _contentViewportHeight;
		*/

		public MainWindowViewModel()
		{
			ShapesCollection = new ObservableCollection<Shape>();
			ShapeProperties = new VisualProperties();
			ToolData = new ToolPanelVm();

			_fileService = new FileService();
			_canvasService = new CanvasService();

			ShapeProperties.BorderColorChanged += OnBorderColorChanged;
			ShapeProperties.FillColorChanged += OnFillColorChanged;
			ShapeProperties.ThicknessChanged += OnThicknessChanged;
			ShapeProperties.WidthChanged += OnWidthChanged;
			ShapeProperties.HeightChanged += OnHeightChanged;

			WindowPreviewKeyDownCommand = new RelayCommand<KeyEventArgs>(HandleKeyEvent);
			WindowPreviewKeyUpCommand = new RelayCommand<KeyEventArgs>(HandleKeyEvent);

			CanvasPreviewMouseMoveCommand = new RelayCommand<MouseEventArgs>(HandleCanvasMouseMovements);
			CanvasPreviewMouseDownCommand = new RelayCommand<MouseButtonEventArgs>(HandleCanvasMouseDown);
			CanvasPreviewMouseUpCommand = new RelayCommand<MouseButtonEventArgs>(HandleCanvasMouseUp);

			UndoCommand = new RelayCommand(() => WorkspaceCanvas.UndoLastChange());
			RedoCommand = new RelayCommand(() => WorkspaceCanvas.RedoLastChange());

			ImportImageCommand = new RelayCommand(ImportImage);

			ExportCommand = new RelayCommand(ExportCanvas);
			DeleteCommand = new RelayCommand(DeleteSelectedShapes);

			LoadProjectFileCommand = new RelayCommand(LoadProjectFile);
			SaveCanvasToProjectFileCommand = new RelayCommand(SaveCanvasToProjectFile);

			WindowLoadedCommand = new RelayCommand(WindowLoaded);
		}

		private void LoadProjectFile()
		{
			_fileService.OpenFileDialog(out var selectedFilePath, Constants.ProjectTypeFilter);
			if (string.IsNullOrEmpty(selectedFilePath))
				return; // todo: придумать какое-нибудь окошко об ошибке

			_canvasService.LoadWorkspaceCanvas(WorkspaceCanvas, selectedFilePath);
		}

		private void SaveCanvasToProjectFile()
		{
			_fileService.SaveFileDialog(out var selectedFilePath, Constants.ProjectTypeFilter);
			if (string.IsNullOrEmpty(selectedFilePath))
				return; // todo: придумать какое-нибудь окошко об ошибке

			_canvasService.ExportCanvasToProjectFile(WorkspaceCanvas, selectedFilePath);
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

		public ICommand ImportImageCommand { get; }

		public ICommand LoadProjectFileCommand { get; }

		public ICommand SaveCanvasToProjectFileCommand { get; }

		public ICommand WindowLoadedCommand { get; }

		public bool IsAvailableWidthAndHeight => WorkspaceCanvas?.SelectedItem is AdvancedRectangle or Image;
		public bool IsAvailableColorAndThicknessSettings => WorkspaceCanvas?.SelectedItem is AdvancedPolyline or AdvancedRectangle;

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

		public CustomCanvas WorkspaceCanvas => AncestorHelper.FindActiveWindow()?.FindName("Workspace") as CustomCanvas;

		public FilteredTextBox ThicknessInputField => AncestorHelper.FindActiveWindow()?.FindName("ThicknessInputField") as FilteredTextBox;

		public FilteredTextBox WidthInputField => AncestorHelper.FindActiveWindow()?.FindName("WidthInputField") as FilteredTextBox;

		public FilteredTextBox HeightInputField => AncestorHelper.FindActiveWindow()?.FindName("HeightInputField") as FilteredTextBox;

		public ToolType SelectedToolType => ToolData?.ActiveToolType ?? ToolType.Select;

		public bool IsShapeSelected => WorkspaceCanvas?.SelectedItem is AdvancedRectangle or AdvancedPolyline or Image;

		public ObservableCollection<Shape> ShapesCollection { get; set; }

		public VisualProperties ShapeProperties
		{
			get => _shapeProperties;
			set
			{
				_shapeProperties = value;
				// UpdateSelectedShape();
				RaisePropertyChanged();
			}
		}

		public ToolPanelVm ToolData
		{
			get => _toolData;
			set
			{
				_toolData = value;
				RaisePropertyChanged();
			}
		}

		// todo: для теста с ScrollMode
		/*
		public double ContentOffsetX
		{
			get => _contentOffsetX;
			set
			{
				_contentOffsetX = value;
				RaisePropertyChanged();
			}
		}

		public double ContentOffsetY
		{
			get => _contentOffsetY;
			set
			{
				_contentOffsetY = value;
				RaisePropertyChanged();
			}
		}

		public double ContentWidth
		{
			get => _contentWidth;
			set
			{
				_contentWidth = value;
				RaisePropertyChanged();
			}
		}

		public double ContentHeight
		{
			get => _contentHeight;
			set
			{
				_contentHeight = value;
				RaisePropertyChanged();
			}
		}

		public double ContentViewportWidth
		{
			get => _contentViewportWidth;
			set
			{
				_contentViewportWidth = value;
				RaisePropertyChanged();
			}
		}

		public double ContentViewportHeight
		{
			get => _contentViewportHeight;
			set
			{
				_contentViewportHeight = value;
				RaisePropertyChanged();
			}
		}
		*/

		private void WindowLoaded()
		{
			//WorkspaceCanvas.Initialize();
			WorkspaceCanvas.SelectedItemChanged += OnSelectedItemChanged;
		}

		public UIElement SelectedItem => WorkspaceCanvas?.SelectedItem;

		private void UpdateRectangleProperties(AdvancedRectangle selectedRectangle)
		{
			ShapeProperties.Width = selectedRectangle.Width;
			ShapeProperties.Height = selectedRectangle.Height;
			ShapeProperties.Thickness = selectedRectangle.StrokeThickness;
			ShapeProperties.FillColor = ((SolidColorBrush)selectedRectangle.Fill)?.Color ?? Colors.Transparent;
			ShapeProperties.BorderColor = ((SolidColorBrush)selectedRectangle.Stroke)?.Color ?? Colors.Transparent;
		}

		private void UpdatePolylineProperties(AdvancedPolyline selectedPolyline)
		{
			ShapeProperties.Width = selectedPolyline.Width;
			ShapeProperties.Height = selectedPolyline.Height;
			ShapeProperties.Thickness = selectedPolyline.StrokeThickness;
			ShapeProperties.FillColor = selectedPolyline.FillColor;
			ShapeProperties.BorderColor = selectedPolyline.BorderColor;
		}

		private void UpdateImageProperties(Image selectedImage)
		{
			ShapeProperties.Width = selectedImage.Width;
			ShapeProperties.Height = selectedImage.Height;
		}

		private void OnSelectedItemChanged(object sender, SelectedItemChangedEventArgs eventArgs)
		{
			if (SelectedItem is AdvancedRectangle selectedRectangle)
				UpdateRectangleProperties(selectedRectangle);

			if (SelectedItem is AdvancedPolyline selectedPolyline)
				UpdatePolylineProperties(selectedPolyline);

			if (SelectedItem is Image selectedImage)
				UpdateImageProperties(selectedImage);

			OnPropertyChanged(nameof(IsAvailableWidthAndHeight));
			OnPropertyChanged(nameof(IsAvailableColorAndThicknessSettings));
			OnPropertyChanged(nameof(IsShapeSelected));
		}

		private void UpdatePolyline(AdvancedPolyline selectedPolyline)
		{
			selectedPolyline.FillColor = ShapeProperties.FillColor;
			selectedPolyline.BorderColor = ShapeProperties.BorderColor;
			selectedPolyline.StrokeThickness = ShapeProperties.Thickness;
		}

		private void OnBorderColorChanged()
		{
			if (WorkspaceCanvas.SelectedItem is AdvancedRectangle selectedRectangle)
				selectedRectangle.Stroke = new SolidColorBrush(ShapeProperties.BorderColor);

			if (WorkspaceCanvas.SelectedItem is AdvancedPolyline selectedPolyline)
			{
				UpdatePolyline(selectedPolyline);
				var polylineAdorner = GetActivePolylineAdorner(selectedPolyline);
				polylineAdorner?.UpdatePreviewLineColor();
			}
		}

		private PolylineAdorner GetActivePolylineAdorner(UIElement adornedElement)
		{
			var adorners = WorkspaceCanvas.AdornerLayer.GetAdorners(adornedElement);

			if (adorners == null)
				return null;

			foreach (var adorner in adorners)
			{
				if (adorner is PolylineAdorner polylineAdorner)
					return polylineAdorner;
			}

			return null;
		}

		private void OnFillColorChanged()
		{
			var selectedFillColor = ShapeProperties.FillColor;
			switch (WorkspaceCanvas.SelectedItem)
			{
				case AdvancedRectangle selectedRectangle:
					selectedRectangle.Fill = new SolidColorBrush(selectedFillColor);
					break;
				case AdvancedPolyline selectedPolyline:
					selectedPolyline.Stroke = new SolidColorBrush(ShapeProperties.FillColor);
					break;
			}
		}

		private void OnThicknessChanged()
		{
			var selectedThickness = ShapeProperties.Thickness;
			if (WorkspaceCanvas.SelectedItem is AdvancedRectangle selectedRectangle)
				selectedRectangle.StrokeThickness = selectedThickness;

			if (WorkspaceCanvas.SelectedItem is AdvancedPolyline selectedPolyline)
				selectedPolyline.StrokeThickness = selectedThickness;
		}

		private void OnWidthChanged()
		{
			var selectedWidth = ShapeProperties.Width;
			if (selectedWidth < 1)
				return;

			if (WorkspaceCanvas.SelectedItem is AdvancedRectangle selectedRectangle)
				selectedRectangle.Width = selectedWidth;
		}

		private void OnHeightChanged()
		{
			var selectedHeight = ShapeProperties.Height;
			if (selectedHeight < 1)
				return;

			if (WorkspaceCanvas.SelectedItem is AdvancedRectangle selectedRectangle)
				selectedRectangle.Height = selectedHeight;
		}

		private void DeleteSelectedShapes()
		{
			WorkspaceCanvas.DeleteSelectedShapes();
		}

		private void ExportCanvas()
		{
			var exportFilter = Constants.ImagesFilter;
			_fileService.SaveFileDialog(out var selectedImagePath, exportFilter);
			if (string.IsNullOrEmpty(selectedImagePath))
				return;

			var pixelHeight = (int)WorkspaceCanvas.ActualHeight;
			var pixelWidth = (int)WorkspaceCanvas.ActualWidth;
			var dpi = 96d;

			var bitmapObject = new RenderTargetBitmap(pixelWidth, pixelHeight, dpi, dpi, PixelFormats.Pbgra32);
			bitmapObject.Render(WorkspaceCanvas);

			_canvasService.ExportCanvasToImage(WorkspaceCanvas, selectedImagePath);
		}

		private void HandleKeyEvent(KeyEventArgs eventArgs)
		{
			IsShiftKeyPressed = Keyboard.Modifiers == ModifierKeys.Shift;
			IsCtrlKeyPressed = Keyboard.Modifiers == ModifierKeys.Control;
		}

		private void HandleMouseMovePolyline(MouseEventArgs eventArgs)
		{
			if (_shape is AdvancedPolyline polyline)
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
					var rectangleShape = _shape as AdvancedRectangle;
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
			if (_shape != null)
			{
				var polyline = _shape as AdvancedPolyline;
				if (eventArgs.ChangedButton == MouseButton.Left)
					polyline?.Points.Add(eventArgs.GetPosition(WorkspaceCanvas));
				else
				{
					EditorHelper.UpdatePolylineLayoutProperties(ref polyline);
					ToolData.ActiveToolType = ToolType.Select;

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

			WorkspaceCanvas.SelectedItem = _shape;
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

			_shape = new AdvancedRectangle()
			{
				Fill = new SolidColorBrush(Colors.Black),
				Width = (endX - startX),
				Height = (endY - startY),
				MinWidth = 15,
				MinHeight = 15,
				RenderTransformOrigin = new Point(0.5d, 0.5d),
				SnapsToDevicePixels = true,
				StrokeThickness = 2.0d,
				Margin = new Thickness(startX, startY, 0.0d, 0.0d)
			};

			Canvas.SetLeft(_shape, 0.0d);
			Canvas.SetTop(_shape, 0.0d);
		}

		private void GeneratePolyline(Point initialPosition)
		{
			_shape = new AdvancedPolyline()
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

		private void ImportImage()
		{
			_fileService.OpenFileDialog(out var selectedImagePath, Constants.ImagesFilter);
			_canvasService.ImportImage(WorkspaceCanvas, selectedImagePath);
		}
	}
}
