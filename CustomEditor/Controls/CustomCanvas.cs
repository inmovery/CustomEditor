using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using CustomEditor.Controls.Adorners;
using CustomEditor.Gestures;
using CustomEditor.Helpers;
using CustomEditor.Models;
using CustomEditor.Models.Events;

namespace CustomEditor.Controls
{
	public class CustomCanvas : Canvas
	{
		// todo: для эксперимента с ScrollMode
		/*
		public double TopLimit { get; set; }
		public double RightLimit { get; set; }
		public double BottomLimit { get; set; }
		public double LeftLimit { get; set; }

		private TranslateTransform _translateTransform;
		private ScaleTransform _scaleTransform;
		private Zoom _zoomGesture;
		private Vector _offset;
		private Size _extent;
		private Size _initialExtent;
		private Size _viewport;
		private Point _panInitialPosition;
		private CustomCanvas _parent => this;
		private Point _viewportBottomRightInitial;
		private Point _viewportTopLeftInitial;

		private double HighestElement => _parent.TopLimit;

		private double LowestElement => _parent.BottomLimit;

		private double MostLeftElement => _parent.LeftLimit;

		private double MostRightElement => _parent.RightLimit;

		public ScrollMode ScrollContainer { get; }


		public readonly ScaleTransform ScaleTransform = new ScaleTransform();
		public readonly TranslateTransform TranslateTransform = new TranslateTransform();

		public void Initialize()
		{
			_canvasContainer = AncestorHelper.FindVisualParent<ScrollMode>(this);
			_canvasContainer?.Initialize(this);
		}
		*/

		//private LinkedList<Action> _undo;
		//private LinkedList<Action> _redo;
		private UIElement _selectedItem;

		private Point _startPoint = new Point(-1.0d, -1.0d);
		private Point _endPoint = new Point(-1.0d, -1.0d);

		private UIElement _previewShape;

		public AdornerLayer AdornerLayer => AdornerLayer.GetAdornerLayer(this);

		public event SelectedItemChangedEventHandler SelectedItemChanged;
		public event EventHandler TestChanged;

		public static readonly DependencyProperty IsShiftKeyPressedProperty = DependencyProperty.Register(
			nameof(IsShiftKeyPressed),
			typeof(bool),
			typeof(CustomCanvas),
			new PropertyMetadata(false));

		public bool IsShiftKeyPressed
		{
			get => (bool)GetValue(IsShiftKeyPressedProperty);
			set => SetValue(IsShiftKeyPressedProperty, value);
		}

		public static readonly DependencyProperty IsDrawingProperty = DependencyProperty.Register(
			nameof(IsDrawing),
			typeof(bool),
			typeof(CustomCanvas),
			new PropertyMetadata(false));

		public bool IsDrawing
		{
			get => (bool)GetValue(IsDrawingProperty);
			set => SetValue(IsDrawingProperty, value);
		}

		public static readonly DependencyProperty IsEditToolActiveProperty = DependencyProperty.Register(
			nameof(IsEditToolActive),
			typeof(bool),
			typeof(CustomCanvas),
			new PropertyMetadata(false));

		public bool IsEditToolActive
		{
			get => (bool)GetValue(IsEditToolActiveProperty);
			set => SetValue(IsEditToolActiveProperty, value);
		}

		public static DependencyProperty MousePositionProperty = DependencyProperty.Register(
			nameof(MousePosition),
			typeof(Point),
			typeof(CustomCanvas),
			new FrameworkPropertyMetadata(default(Point)));

		public Point MousePosition
		{
			get => (Point)GetValue(MousePositionProperty);
			set => SetValue(MousePositionProperty, value);
		}

		public UIElement SelectedItem
		{
			get => _selectedItem;
			set
			{
				_selectedItem = value;
				SelectedItemChanged?.Invoke(this, new SelectedItemChangedEventArgs(SelectedItem));
			}
		}

		public void UndoLastChange()
		{
			/*
			var undoValue = _undo.First.Value;

			_undo.RemoveFirst();
			Children.Remove(undoValue.InvolvedElement);

			_redo.AddFirst(undoValue);
			*/
		}

		public void RedoLastChange()
		{
			/*
			var redoValue = _redo.First.Value;
			_redo.RemoveFirst();

			_undo.AddFirst(redoValue);
			*/
		}

		public void DeleteSelectedShapes()
		{
			Children.Remove(SelectedItem);
		}

		public void RaiseSelectedItemChanged()
		{
			SelectedItemChanged?.Invoke(this, new SelectedItemChangedEventArgs(SelectedItem));
		}

		/*
		public void AdjustScroll()
		{
			ScrollContainer?.AdjustScrollVertically();
			ScrollContainer?.AdjustScrollHorizontally();
		}
		*/

		protected override void OnPreviewMouseMove(MouseEventArgs eventArgs)
		{
			// todo: ниже эксперимент с ScrollMode
			/*
			var elementRect = BoundsRelativeTo(SelectedItem as FrameworkElement, this);

			var top = elementRect.Top;
			var bottom = elementRect.Bottom;
			var right = elementRect.Right;
			var left = elementRect.Left;

			var test = top > 0 || bottom > 0 || right > 0 || left > 0;
			var stop = 0;
			if (test)
				stop = 1;

			TopLimit = Math.Min(TopLimit, top);
			BottomLimit = Math.Max(BottomLimit, bottom);
			RightLimit = Math.Max(RightLimit, right);
			LeftLimit = Math.Min(LeftLimit, left);
			AdjustScroll();
			*/

			if (IsShiftKeyPressed && eventArgs.LeftButton == MouseButtonState.Pressed)
			{
				if (_startPoint.X < 0)
					return;

				_endPoint = eventArgs.GetPosition(this);

				var startX = Math.Min(_startPoint.X, _endPoint.X);
				var startY = Math.Min(_startPoint.Y, _endPoint.Y);
				var endX = Math.Max(_startPoint.X, _endPoint.X);
				var endY = Math.Max(_startPoint.Y, _endPoint.Y);

				if (_previewShape != null)
				{
					if (_previewShape is Rectangle rectangleShape)
					{
						rectangleShape.Width = (endX - startX);
						rectangleShape.Height = (endY - startY);
						rectangleShape.Margin = new Thickness(startX, startY, 0, 0);

						_previewShape = rectangleShape;
					}
				}
				else
				{
					GenerateDashRectangle();
					Children.Add(_previewShape);
				}

				CheckBoundsOfShapes();
			}

			base.OnPreviewMouseMove(eventArgs);
		}

		protected override void OnPreviewMouseDown(MouseButtonEventArgs eventArgs)
		{
			_startPoint = eventArgs.GetPosition(this);
			_previewShape = null;

			if (Equals(eventArgs.Source, this))
			{
				UnselectAllElements();
				return;
			}

			if (_selectedItem != null)
				UnselectSingleAdorner(SelectedItem);

			SelectedItem = eventArgs.Source as UIElement;

			AddSelectionAdorner(SelectedItem);

			base.OnPreviewMouseDown(eventArgs);
		}

		protected override void OnPreviewMouseUp(MouseButtonEventArgs eventArgs)
		{
			if (IsShiftKeyPressed)
			{
				SelectedItem = _previewShape;

				AddSelectionAdorner(SelectedItem);
				Children.Remove(SelectedItem);

				_previewShape = null;
			}

			_startPoint.X = _startPoint.Y = -1;
			_endPoint.X = _endPoint.Y = -1;

			base.OnPreviewMouseUp(eventArgs);
		}

		private void GenerateDashRectangle()
		{
			var foreground = new SolidColorBrush(Colors.Blue);

			var startX = Math.Min(_startPoint.X, _endPoint.X);
			var startY = Math.Min(_startPoint.Y, _endPoint.Y);
			var endX = Math.Max(_startPoint.X, _endPoint.X);
			var endY = Math.Max(_startPoint.Y, _endPoint.Y);

			_previewShape = new Rectangle()
			{
				Fill = Brushes.Transparent,
				Stroke = foreground,
				Width = (endX - startX),
				Height = (endY - startY),
				StrokeThickness = 2.0d,
				Margin = new Thickness(startX, startY, 0.0d, 0.0d),
				StrokeDashArray = new DoubleCollection { 1.0d, 2.0d },
				StrokeDashCap = PenLineCap.Round
			};

			SetLeft(_previewShape, 0.0d);
			SetTop(_previewShape, 0.0d);
		}

		private void CheckBoundsOfShapes()
		{
			var previewRect = CalculateElementRect(_previewShape);
			foreach (UIElement item in Children)
			{
				if (Equals(_previewShape, item))
					continue;

				var rectItem = CalculateElementRect(item);
				if (AreIntersectedRectangles(previewRect, rectItem))
					ChangeSelectionColor(item, Brushes.Blue);
				else
					ChangeSelectionColor(item, Brushes.Black);
			}
		}

		private void ChangeSelectionColor(UIElement item, Brush colorBrush)
		{
			if (item is AdvancedRectangle rectangleShape)
				rectangleShape.Fill = colorBrush;

			if (item is AdvancedPolyline polylineShape)
				polylineShape.Stroke = colorBrush;
		}

		private Rect CalculateElementRect(UIElement elementSelected)
		{
			var element = elementSelected as FrameworkElement;
			if (element == null)
				return new Rect();

			var transform = element.TransformToAncestor(this);

			var leftTopPoint = transform.Transform(new Point(0, 0));
			var rightBottomPoint = transform.Transform(new Point(element.ActualWidth, element.ActualHeight));
			var rightTopPoint = transform.Transform(new Point(element.ActualWidth, 0));
			var leftBottomPoint = transform.Transform(new Point(0, element.ActualHeight));

			var minX = Math.Min(Math.Min(leftTopPoint.X, rightTopPoint.X), Math.Min(leftBottomPoint.X, rightBottomPoint.X));
			var minY = Math.Min(Math.Min(leftTopPoint.Y, rightTopPoint.Y), Math.Min(leftBottomPoint.Y, rightBottomPoint.Y));
			var maxX = Math.Max(Math.Max(leftTopPoint.X, rightTopPoint.X), Math.Max(leftBottomPoint.X, rightBottomPoint.X));
			var maxY = Math.Max(Math.Max(leftTopPoint.Y, rightTopPoint.Y), Math.Max(leftBottomPoint.Y, rightBottomPoint.Y));

			return new Rect(minX, minY, maxX - minX, maxY - minY);
		}

		private bool AreIntersectedRectangles(Rect first, Rect second)
		{
			if (first.TopLeft.X > second.BottomRight.X)
				return false;

			if (second.TopLeft.X > first.BottomRight.X)
				return false;

			if (first.TopLeft.Y > second.BottomRight.Y)
				return false;

			if (second.TopLeft.Y > first.BottomRight.Y)
				return false;

			return true;
		}

		protected internal void AddSelectionAdorner(UIElement uiElement)
		{
			if (uiElement != null && !IsDrawing)
			{
				var rectangleAdorner = new RectangleAdorner(uiElement);
				rectangleAdorner.DoubleClickExecuted += OnDoubleClickByAdorner;

				if (IsEditToolActive)
					return;

				var selectedElement = uiElement as FrameworkElement;
				if (selectedElement == null)
					return;

				var isSelectedElementNan = double.IsNaN(selectedElement.Width);
				if (isSelectedElementNan && uiElement is AdvancedPolyline polyline)
					EditorHelper.UpdatePolylineLayoutProperties(ref polyline);

				AdornerLayer.Add(rectangleAdorner);
			}
		}

		private void OnDoubleClickByAdorner(UIElement adornedElement)
		{
			if (adornedElement is AdvancedRectangle)
				return;

			ActivatePolylineEditor(adornedElement);
		}

		private void ActivatePolylineEditor(UIElement adornedElement)
		{
			UnselectSingleAdorner(SelectedItem);

			var polylineAdorner = new PolylineAdorner(adornedElement);
			AdornerLayer.Add(polylineAdorner);

			IsEditToolActive = true;
			SelectedItem = adornedElement;
		}

		protected void UnselectSingleAdorner(UIElement adornedElement)
		{
			SelectedItem = null;
			var adorners = AdornerLayer.GetAdorners(adornedElement);
			if (adorners == null)
				return;

			foreach (var adorner in adorners)
				AdornerLayer.Remove(adorner);
		}

		protected void UnselectAllElements()
		{
			IsEditToolActive = false;
			foreach (UIElement uiElement in Children)
				UnselectSingleAdorner(uiElement);
		}
	}
}