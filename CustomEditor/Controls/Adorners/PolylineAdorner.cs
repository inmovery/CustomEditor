using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using CustomEditor.Controls.Thumbs;
using CustomEditor.Models;

namespace CustomEditor.Controls.Adorners
{
	public class PolylineAdorner : Adorner
	{
		protected VisualCollection VisualChildren;
		private readonly AdvancedPolyline _adornedPolyline;

		public PolylineAdorner(UIElement adornedElement) : base(adornedElement)
		{
			VisualChildren = new VisualCollection(this);

			_adornedPolyline = AdornedElement as AdvancedPolyline;

			if (_adornedPolyline == null)
				return;

			for (var i = 0; i < _adornedPolyline.Points.Count; i++)
			{
				var matrix = _adornedPolyline.GeometryTransform.Value;
				var point = matrix.Transform(_adornedPolyline.Points[i]);
				point = _adornedPolyline.TranslatePoint(point, GetParentCanvas());

				_adornedPolyline.Points[i] = point;
			}

			for (var i = 1; i < _adornedPolyline.Points.Count; i++)
			{
				var startPoint = _adornedPolyline.Points[i - 1];
				var endPoint = _adornedPolyline.Points[i];
				AddLinePreview(startPoint, endPoint);
			}

			for (var i = 0; i < _adornedPolyline.Points.Count; i++)
				AddThumb();

			_adornedPolyline.Stretch = Stretch.None;
			_adornedPolyline.RenderTransform = null;

			UpdateDependencyProperty(_adornedPolyline, Canvas.LeftProperty, DependencyProperty.UnsetValue);
			UpdateDependencyProperty(_adornedPolyline, Canvas.TopProperty, DependencyProperty.UnsetValue);
			UpdateDependencyProperty(_adornedPolyline, WidthProperty, DependencyProperty.UnsetValue);
			UpdateDependencyProperty(_adornedPolyline, HeightProperty, DependencyProperty.UnsetValue);
		}

		private void OnPreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
		{
			// todo : добавить проверку shift
		}

		public void PointDragStarted(object sender, DragStartedEventArgs e)
		{
			// todo: добавить сохранение состояния в буфер (пример: RaiseObjectChanged(new ModifyGraphicsObject(AdornedElement)))
		}

		public void PointDragDelta(object sender, DragDeltaEventArgs e)
		{
			var pointIndex = GetPointIndexByVisualObject(sender as PointDragThumb);
			var selectedPoint = _adornedPolyline.Points[pointIndex];

			var dragDelta = new Point(e.HorizontalChange, e.VerticalChange);
			selectedPoint = _adornedPolyline.TranslatePoint(selectedPoint, GetParentCanvas());
			selectedPoint.X += dragDelta.X;
			selectedPoint.Y += dragDelta.Y;

			var previousPoint = _adornedPolyline.Points[pointIndex];
			_adornedPolyline.Points[pointIndex] = selectedPoint;

			var startList = new List<Point>();
			var endList = new List<Point>();

			var selectionIndex = _adornedPolyline.Points.IndexOf(selectedPoint);
			foreach (var visualChild in VisualChildren)
			{
				if (visualChild is Line line)
				{
					var isStartPoint = Math.Abs(line.X1 - previousPoint.X) < Constants.MinComparableThreshold &&
					                   Math.Abs(line.Y1 - previousPoint.Y) < Constants.MinComparableThreshold;
					if (isStartPoint)
					{
						line.X1 = selectedPoint.X;
						line.Y1 = selectedPoint.Y;
					}

					var isEndPoint = Math.Abs(line.X2 - previousPoint.X) < Constants.MinComparableThreshold &&
					                 Math.Abs(line.Y2 - previousPoint.Y) < Constants.MinComparableThreshold;
					if (isEndPoint)
					{
						line.X2 = selectedPoint.X;
						line.Y2 = selectedPoint.Y;
					}
				}
			}

			InvalidateArrange();
		}

		protected override Size MeasureOverride(Size finalSize)
		{
			foreach (var visual in VisualChildren)
			{
				switch (visual)
				{
					case PointDragThumb pointDragChild:
						pointDragChild.Measure(finalSize);
						break;

					case Line linePreview:
						linePreview.Measure(finalSize);
						break;
				}
			}

			return finalSize;
		}

		private int GetPointIndexByVisualObject(Visual visualObject)
		{
			var countLines = (VisualChildren.Count / 2);
			var globalIndex = VisualChildren.IndexOf(visualObject);
			var pointIndex = globalIndex - countLines;

			return pointIndex;
		}

		protected override Size ArrangeOverride(Size arrangeBounds)
		{
			foreach (var visual in VisualChildren)
			{
				switch (visual)
				{
					case PointDragThumb pointDragChild:
						var pointIndex = GetPointIndexByVisualObject(pointDragChild);
						var selectedPoint = _adornedPolyline.Points[pointIndex];
						selectedPoint = _adornedPolyline.TranslatePoint(selectedPoint, this);

						selectedPoint.X -= pointDragChild.DesiredSize.Width / 2;
						selectedPoint.Y -= pointDragChild.DesiredSize.Height / 2;

						pointDragChild.Arrange(new Rect(selectedPoint, pointDragChild.DesiredSize));

						break;

					case Line selectedLinearLine:
						var finalRect = new Rect(arrangeBounds);
						selectedLinearLine?.Arrange(finalRect);

						break;
				}
			}

			return arrangeBounds;
		}

		private void AddThumb()
		{
			var pointDragThumb = new PointDragThumb(AdornedElement as FrameworkElement);
			pointDragThumb.DragStarted += PointDragStarted;
			pointDragThumb.DragDelta += PointDragDelta;
			pointDragThumb.PreviewMouseLeftButtonUp += OnPreviewMouseLeftButtonUp;

			VisualChildren.Add(pointDragThumb);
			Panel.SetZIndex((UIElement)VisualChildren[VisualChildren.Count - 1], 1);

			_adornedPolyline.UpdateLayout();
		}

		public void UpdatePreviewLineColor()
		{
			var polyline = (AdornedElement as AdvancedPolyline) ?? new AdvancedPolyline();
			foreach (var visualChild in VisualChildren)
			{
				var selectedColor = new SolidColorBrush()
				{
					Color = polyline.BorderColor,
					Opacity = 0.5
				};

				if (visualChild is Line lineChild)
					lineChild.Stroke = selectedColor;
			}
		}

		private void AddLinePreview(Point startPoint, Point endPoint)
		{
			var polyline = (AdornedElement as AdvancedPolyline) ?? new AdvancedPolyline();

			var selectedColor = new SolidColorBrush()
			{
				Color = polyline.BorderColor,
				Opacity = 0.5
			};

			var linePath = new Line()
			{
				X1 = startPoint.X,
				Y1 = startPoint.Y,
				X2 = endPoint.X,
				Y2 = endPoint.Y,
				Stroke = selectedColor,
				StrokeThickness = polyline.StrokeThickness + 10.0d,
				StrokeStartLineCap = PenLineCap.Round,
				StrokeEndLineCap = PenLineCap.Round,
				StrokeLineJoin = PenLineJoin.Round
			};

			VisualChildren.Add(linePath);
			Panel.SetZIndex((UIElement)VisualChildren[VisualChildren.Count - 1], 0);

			_adornedPolyline.UpdateLayout();
		}

		private void UpdateDependencyProperty(DependencyObject dependencyObject, DependencyProperty dependencyProperty, object value)
		{
			var binding = BindingOperations.GetBinding(dependencyObject, dependencyProperty);
			if (binding != null)
			{
				binding.FallbackValue = value;
				BindingOperations.GetBindingExpression(dependencyObject, dependencyProperty)?.UpdateTarget();
			}
			else
				dependencyObject.SetValue(dependencyProperty, value);
		}

		private UIElement GetParentCanvas() => VisualTreeHelper.GetParent(AdornedElement) as UIElement;

		protected override int VisualChildrenCount => VisualChildren.Count;

		protected override Visual GetVisualChild(int index) => VisualChildren[index];

		public override GeneralTransform GetDesiredTransform(GeneralTransform transform)
		{
			return new MatrixTransform();
		}
	}
}
