using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;

namespace CustomEditor.Controls
{
	/// <summary>
	/// @todo: эксперимент с захватом мыши № 2
	/// </summary>
	public class ScrollAndPan : ContentControl, IScrollInfo
	{
		private FrameworkElement _content;

		/// <summary>
		/// The transform that is applied to the content to offset it by 'ContentOffsetX' and 'ContentOffsetY'.
		/// </summary>
		private TranslateTransform _contentOffsetTransform;

		/// <summary>
		/// Used to disable syncronization between IScrollInfo interface and ContentOffsetX/ContentOffsetY.
		/// </summary>
		private bool _disableScrollOffsetSync;

		/// <summary>
		/// The width of the viewport in content coordinates, clamped to the width of the content.
		/// </summary>
		private double _constrainedContentViewportWidth;

		/// <summary>
		/// The height of the viewport in content coordinates, clamped to the height of the content.
		/// </summary>
		private double _constrainedContentViewportHeight;

		/// <summary>
		/// Records the unscaled extent of the content.
		/// This is calculated during the measure and arrange.
		/// </summary>
		private Size _unScaledExtent = new Size(0, 0);

		/// <summary>
		/// Records the size of the viewport (in viewport coordinates) onto the content.
		/// This is calculated during the measure and arrange.
		/// </summary>
		private Size _viewport = new Size(0, 0);

		public static readonly DependencyProperty ContentOffsetXProperty = DependencyProperty.Register(
			nameof(ContentOffsetX),
			typeof(double),
			typeof(ScrollAndPan),
			new FrameworkPropertyMetadata(0.0, ContentOffsetX_PropertyChanged, ContentOffsetX_Coerce));

		public static readonly DependencyProperty ContentOffsetYProperty = DependencyProperty.Register(
			nameof(ContentOffsetY),
			typeof(double),
			typeof(ScrollAndPan),
			new FrameworkPropertyMetadata(0.0, ContentOffsetY_PropertyChanged, ContentOffsetY_Coerce));

		public static readonly DependencyProperty ContentViewportWidthProperty = DependencyProperty.Register(
			nameof(ContentViewportWidth),
			typeof(double),
			typeof(ScrollAndPan),
			new FrameworkPropertyMetadata(0.0));

		public static readonly DependencyProperty ContentViewportHeightProperty = DependencyProperty.Register(
			nameof(ContentViewportHeight),
			typeof(double),
			typeof(ScrollAndPan),
			new FrameworkPropertyMetadata(0.0));

		public static readonly DependencyProperty IsMouseWheelScrollingEnabledProperty = DependencyProperty.Register(
			nameof(IsMouseWheelScrollingEnabled),
			typeof(bool),
			typeof(ScrollAndPan),
			new FrameworkPropertyMetadata(false));

		/// <summary>
		/// Get/set the X offset (in content coordinates) of the view on the content.
		/// </summary>
		public double ContentOffsetX
		{
			get => (double)GetValue(ContentOffsetXProperty);
			set => SetValue(ContentOffsetXProperty, value);
		}

		/// <summary>
		/// Event raised when the ContentOffsetX property has changed.
		/// </summary>
		public event EventHandler ContentOffsetXChanged;

		/// <summary>
		/// Get/set the Y offset (in content coordinates) of the view on the content.
		/// </summary>
		public double ContentOffsetY
		{
			get => (double)GetValue(ContentOffsetYProperty);
			set => SetValue(ContentOffsetYProperty, value);
		}

		/// <summary>
		/// Event raised when the ContentOffsetY property has changed.
		/// </summary>
		public event EventHandler ContentOffsetYChanged;

		/// <summary>
		/// Get the viewport width, in content coordinates.
		/// </summary>
		public double ContentViewportWidth
		{
			get => (double)GetValue(ContentViewportWidthProperty);
			set => SetValue(ContentViewportWidthProperty, value);
		}

		/// <summary>
		/// Get the viewport height, in content coordinates.
		/// </summary>
		public double ContentViewportHeight
		{
			get => (double)GetValue(ContentViewportHeightProperty);
			set => SetValue(ContentViewportHeightProperty, value);
		}

		/// <summary>
		/// Set to 'true' to enable the mouse wheel to scroll the zoom and pan control.
		/// This is set to 'false' by default.
		/// </summary>
		public bool IsMouseWheelScrollingEnabled
		{
			get => (bool)GetValue(IsMouseWheelScrollingEnabledProperty);
			set => SetValue(IsMouseWheelScrollingEnabledProperty, value);
		}

		public bool CanVerticallyScroll { get; set; }

		public bool CanHorizontallyScroll { get; set; }

		public double ExtentWidth => _unScaledExtent.Width;

		public double ExtentHeight => _unScaledExtent.Height;

		/// <summary>
		/// Get the width of the viewport onto the content.
		/// </summary>
		public double ViewportWidth => _viewport.Width;

		/// <summary>
		/// Get the height of the viewport onto the content.
		/// </summary>
		public double ViewportHeight => _viewport.Height;

		public ScrollViewer ScrollOwner { get; set; }

		/// <summary>
		/// The offset of the horizontal scrollbar.
		/// </summary>
		public double HorizontalOffset => ContentOffsetX;

		/// <summary>
		/// The offset of the vertical scrollbar.
		/// </summary>
		public double VerticalOffset => ContentOffsetY;

		public void SetHorizontalOffset(double offset)
		{
			if (_disableScrollOffsetSync)
				return;

			try
			{
				_disableScrollOffsetSync = true;
				ContentOffsetX = offset;
			}
			finally
			{
				_disableScrollOffsetSync = false;
			}
		}

		public void SetVerticalOffset(double offset)
		{
			if (_disableScrollOffsetSync)
				return;

			try
			{
				_disableScrollOffsetSync = true;
				ContentOffsetY = offset;
			}
			finally
			{
				_disableScrollOffsetSync = false;
			}
		}

		public void LineUp() => ContentOffsetY -= (ContentViewportHeight / 10);
		public void LineDown() => ContentOffsetY += (ContentViewportHeight / 10);
		public void LineLeft() => ContentOffsetX -= (ContentViewportWidth / 10);
		public void LineRight() => ContentOffsetX += (ContentViewportWidth / 10);

		public void PageUp() => ContentOffsetY -= ContentViewportHeight;
		public void PageDown() => ContentOffsetY += ContentViewportHeight;
		public void PageLeft() => ContentOffsetX -= ContentViewportWidth;
		public void PageRight() => ContentOffsetX += ContentViewportWidth;

		public void MouseWheelDown()
		{
			if (IsMouseWheelScrollingEnabled)
				LineDown();
		}

		public void MouseWheelLeft()
		{
			if (IsMouseWheelScrollingEnabled)
				LineLeft();
		}

		public void MouseWheelRight()
		{
			if (IsMouseWheelScrollingEnabled)
				LineRight();
		}

		public void MouseWheelUp()
		{
			if (IsMouseWheelScrollingEnabled)
				LineUp();
		}

		public Rect MakeVisible(Visual visual, Rect rectangle)
		{
			if (!_content.IsAncestorOf(visual))
				return rectangle;

			var transformedRect = visual.TransformToAncestor(_content).TransformBounds(rectangle);
			var viewportRect = new Rect(ContentOffsetX, ContentOffsetY, ContentViewportWidth, ContentViewportHeight);
			if (transformedRect.Contains(viewportRect))
				return rectangle;

			double horizontalOffset = 0;
			double verticalOffset = 0;

			if (transformedRect.Left < viewportRect.Left)
				horizontalOffset = transformedRect.Left - viewportRect.Left;
			else if (transformedRect.Right > viewportRect.Right)
				horizontalOffset = transformedRect.Right - viewportRect.Right;

			if (transformedRect.Top < viewportRect.Top)
				verticalOffset = transformedRect.Top - viewportRect.Top;
			else if (transformedRect.Bottom > viewportRect.Bottom)
				verticalOffset = transformedRect.Bottom - viewportRect.Bottom;

			SnapContentOffsetTo(new Point(ContentOffsetX + horizontalOffset, ContentOffsetY + verticalOffset));

			return rectangle;
		}

		public void SnapContentOffsetTo(Point contentOffset)
		{
			ContentOffsetX = contentOffset.X;
			ContentOffsetY = contentOffset.Y;
		}

		static ScrollAndPan()
		{
			DefaultStyleKeyProperty.OverrideMetadata(typeof(ScrollAndPan), new FrameworkPropertyMetadata(typeof(ScrollAndPan)));
		}

		public override void OnApplyTemplate()
		{
			base.OnApplyTemplate();

			_content = Template.FindName("PART_Content", this) as FrameworkElement;
			if (_content == null)
				return;

			_contentOffsetTransform = new TranslateTransform();
			UpdateTranslationX();
			UpdateTranslationY();

			var transformGroup = new TransformGroup();
			transformGroup.Children.Add(_contentOffsetTransform);
			_content.RenderTransform = transformGroup;
		}

		private static void ContentOffsetX_PropertyChanged(DependencyObject dObj, DependencyPropertyChangedEventArgs eventArgs)
		{
			var scrollAndPan = (ScrollAndPan)dObj;

			scrollAndPan.UpdateTranslationX();

			scrollAndPan.ContentOffsetXChanged?.Invoke(scrollAndPan, EventArgs.Empty);

			if (!scrollAndPan._disableScrollOffsetSync && scrollAndPan.ScrollOwner != null)
				scrollAndPan.ScrollOwner.InvalidateScrollInfo();
		}

		private static object ContentOffsetX_Coerce(DependencyObject dObj, object baseValue)
		{
			var c = (ScrollAndPan)dObj;
			var minOffsetX = 0.0;
			var maxOffsetX = Math.Max(0.0, c._unScaledExtent.Width - c._constrainedContentViewportWidth);

			var value = Math.Min(Math.Max((double)baseValue, minOffsetX), maxOffsetX);

			return value;
		}

		private static void ContentOffsetY_PropertyChanged(DependencyObject dObj, DependencyPropertyChangedEventArgs e)
		{
			var scrollAndPan = (ScrollAndPan)dObj;

			scrollAndPan.UpdateTranslationY();

			scrollAndPan.ContentOffsetYChanged?.Invoke(scrollAndPan, EventArgs.Empty);

			if (!scrollAndPan._disableScrollOffsetSync && scrollAndPan.ScrollOwner != null)
				scrollAndPan.ScrollOwner.InvalidateScrollInfo();
		}

		private static object ContentOffsetY_Coerce(DependencyObject dObj, object baseValue)
		{
			var scrollAndPan = (ScrollAndPan)dObj;
			var minOffsetY = 0.0;
			var maxOffsetY = Math.Max(0.0, scrollAndPan._unScaledExtent.Height - scrollAndPan._constrainedContentViewportHeight);

			var value = Math.Min(Math.Max((double)baseValue, minOffsetY), maxOffsetY);

			return value;
		}

		private void UpdateViewportSize(Size newSize)
		{
			if (_viewport == newSize)
				return;

			_viewport = newSize;

			UpdateContentViewportSize();

			ContentOffsetX = ContentOffsetX;
			ContentOffsetY = ContentOffsetY;

			ScrollOwner?.InvalidateScrollInfo();
		}

		private void UpdateContentViewportSize()
		{
			ContentViewportWidth = ViewportWidth;
			ContentViewportHeight = ViewportHeight;

			_constrainedContentViewportWidth = Math.Min(ContentViewportWidth, _unScaledExtent.Width);
			_constrainedContentViewportHeight = Math.Min(ContentViewportHeight, _unScaledExtent.Height);

			UpdateTranslationX();
			UpdateTranslationY();
		}

		private void UpdateTranslationX()
		{
			if (_contentOffsetTransform == null)
				return;

			var scaledContentWidth = _unScaledExtent.Width;
			if (scaledContentWidth < ViewportWidth)
				_contentOffsetTransform.X = (ContentViewportWidth - _unScaledExtent.Width) / 2;
			else
				_contentOffsetTransform.X = -ContentOffsetX;
		}

		private void UpdateTranslationY()
		{
			if (_contentOffsetTransform == null)
				return;

			var scaledContentHeight = _unScaledExtent.Height;
			if (scaledContentHeight < ViewportHeight)
				_contentOffsetTransform.Y = (ContentViewportHeight - _unScaledExtent.Height) / 2;
			else
				_contentOffsetTransform.Y = -ContentOffsetY;
		}

		protected override Size MeasureOverride(Size constraint)
		{
			var infiniteSize = new Size(double.PositiveInfinity, double.PositiveInfinity);
			var childSize = base.MeasureOverride(infiniteSize);

			if (childSize != _unScaledExtent)
			{
				_unScaledExtent = childSize;
				ScrollOwner?.InvalidateScrollInfo();
			}

			UpdateViewportSize(constraint);

			var width = constraint.Width;
			var height = constraint.Height;

			if (double.IsInfinity(width))
				width = childSize.Width;

			if (double.IsInfinity(height))
				height = childSize.Height;

			UpdateTranslationX();
			UpdateTranslationY();

			return new Size(width, height);
		}

		protected override Size ArrangeOverride(Size arrangeBounds)
		{
			var size = base.ArrangeOverride(this.DesiredSize);

			if (_content.DesiredSize != _unScaledExtent)
			{
				_unScaledExtent = _content.DesiredSize;

				ScrollOwner?.InvalidateScrollInfo();
			}

			UpdateViewportSize(arrangeBounds);

			return size;
		}
	}
}
