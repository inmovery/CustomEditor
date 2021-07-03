using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using CustomEditor.Controls.Thumbs;

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
				AddThumb();

			for (var i = 0; i < _adornedPolyline.Points.Count; i++)
			{
				var matrix = _adornedPolyline.GeometryTransform.Value;
				var point = matrix.Transform(_adornedPolyline.Points[i]);
				point = _adornedPolyline.TranslatePoint(point, GetParentCanvas());

				_adornedPolyline.Points[i] = point;
			}

			_adornedPolyline.Stretch = Stretch.None;
			_adornedPolyline.RenderTransform = null;

			UpdateDependencyProperty(_adornedPolyline, Canvas.LeftProperty, DependencyProperty.UnsetValue);
			UpdateDependencyProperty(_adornedPolyline, Canvas.TopProperty, DependencyProperty.UnsetValue);
			UpdateDependencyProperty(_adornedPolyline, FrameworkElement.WidthProperty, DependencyProperty.UnsetValue);
			UpdateDependencyProperty(_adornedPolyline, FrameworkElement.HeightProperty, DependencyProperty.UnsetValue);
		}

		private void OnPreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
		{
			// todo : добавить ещё проверку shift
		}

		public void PointDragStarted(object sender, DragStartedEventArgs e)
		{
			// todo: добавить сохранение состояния в буфер (пример: RaiseObjectChanged(new ModifyGraphicsObject(AdornedElement)))
		}

		public void PointDragDelta(object sender, DragDeltaEventArgs e)
		{
			var selectedPoint = _adornedPolyline.Points[VisualChildren.IndexOf(sender as PointDragThumb)];

			var dragDelta = new Point(e.HorizontalChange, e.VerticalChange);
			selectedPoint = _adornedPolyline.TranslatePoint(selectedPoint, GetParentCanvas());
			selectedPoint.X += dragDelta.X;
			selectedPoint.Y += dragDelta.Y;

			_adornedPolyline.Points[VisualChildren.IndexOf(sender as PointDragThumb)] = selectedPoint;

			InvalidateArrange();
		}

		protected override Size MeasureOverride(Size finalSize)
		{
			foreach (var visual in VisualChildren)
			{
				var pointDragThumb = (PointDragThumb)visual;
				pointDragThumb.Measure(finalSize);
			}

			return finalSize;
		}

		protected override Size ArrangeOverride(Size finalSize)
		{
			foreach (var visual in VisualChildren)
			{
				var pointDragThumb = (PointDragThumb)visual;
				var selectedPoint = _adornedPolyline.Points[VisualChildren.IndexOf(pointDragThumb)];
				selectedPoint = _adornedPolyline.TranslatePoint(selectedPoint, this);

				selectedPoint.X -= pointDragThumb.DesiredSize.Width / 2;
				selectedPoint.Y -= pointDragThumb.DesiredSize.Height / 2;

				pointDragThumb.Arrange(new Rect(selectedPoint, pointDragThumb.DesiredSize));
			}

			return finalSize;
		}

		private void AddThumb()
		{
			var pointDragThumb = new PointDragThumb(AdornedElement as FrameworkElement);
			pointDragThumb.DragStarted += PointDragStarted;
			pointDragThumb.DragDelta += PointDragDelta;
			pointDragThumb.PreviewMouseLeftButtonUp += OnPreviewMouseLeftButtonUp;

			VisualChildren.Add(pointDragThumb);
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
