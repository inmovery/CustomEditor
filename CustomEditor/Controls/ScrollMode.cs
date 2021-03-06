using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;

namespace CustomEditor.Controls
{
	/// <summary>
	/// @todo: эксперимент с захватом мыши № 1
	/// </summary>
	public class ScrollMode : Grid, IScrollInfo
	{
		private TranslateTransform _translateTransform;
		private Vector _offset;
		private Size _extent;
		private Size _initialExtent;
		private Size _viewport;
		private Point _panInitialPosition;
		private CustomCanvas _parent;
		private Point _viewportBottomRightInitial;
		private Point _viewportTopLeftInitial;

		private double HighestElement => 0.0d; // _parent?.TopLimit ?? 0.0d;

		private double LowestElement => 0.0d; // _parent?.BottomLimit ?? 0.0d;

		private double MostLeftElement => 0.0d; // _parent?.LeftLimit ?? 0.0d;

		private double MostRightElement => 0.0d; // _parent?.RightLimit ?? 0.0d;

		/// <summary>
		/// Positive Vertical offset of <see cref="ScrollOwner"/>
		/// </summary>
		protected double TopOffset => Math.Abs(TopLimit - HighestElement);

		/// <summary>
		/// Negative Vertical offset of <see cref="ScrollOwner"/>
		/// </summary>
		protected double BottomOffset => (BottomLimit - LowestElement);

		/// <summary>
		/// Positive Horizontal offset of <see cref="ScrollOwner"/>
		/// </summary>
		protected double LeftOffset => Math.Abs(LeftLimit - MostLeftElement);

		/// <summary>
		/// Negative Horizontal offset of <see cref="ScrollOwner"/>
		/// </summary>
		protected double RightOffset => (RightLimit - MostRightElement);

		/// <summary>
		/// Top limit of <see cref="ScrollOwner"/> viewport
		/// </summary>
		protected double TopLimit => TranslatePoint(_viewportTopLeftInitial, _parent).Y;

		/// <summary>
		/// Bottom limit of <see cref="ScrollOwner"/> viewport
		/// </summary>
		protected double BottomLimit => TranslatePoint(_viewportBottomRightInitial, _parent).Y;

		/// <summary> 
		/// Left limit of <see cref="ScrollOwner"/> viewport
		/// </summary>
		protected double LeftLimit => TranslatePoint(_viewportTopLeftInitial, _parent).X;

		/// <summary>
		/// Right limit of <see cref="ScrollOwner"/> viewport
		/// </summary>
		protected double RightLimit => TranslatePoint(_viewportBottomRightInitial, _parent).X;

		public bool TranslatedVertically { get; private set; }

		public bool TranslatedHorizontally { get; private set; }

		/// <summary>
		/// <see cref="IScrollInfo"/> member
		/// </summary>
		public bool CanHorizontallyScroll { get; set; }

		/// <summary>
		/// <see cref="IScrollInfo"/> member
		/// </summary>
		public bool CanVerticallyScroll { get; set; }

		/// <summary>
		/// <see cref="IScrollInfo"/> member
		/// </summary>
		public double ExtentHeight => _extent.Height;

		/// <summary>
		/// <see cref="IScrollInfo"/> member
		/// </summary>
		public double ExtentWidth => _extent.Width;

		/// <summary>
		/// <see cref="IScrollInfo"/> member
		/// </summary>
		public double HorizontalOffset => _offset.X;

		/// <summary>
		/// <see cref="IScrollInfo"/> member
		/// </summary>
		public ScrollViewer ScrollOwner { get; set; }

		/// <summary>
		/// <see cref="IScrollInfo"/> member
		/// </summary>
		public double VerticalOffset => _offset.Y;

		/// <summary>
		/// <see cref="IScrollInfo"/> member
		/// </summary>
		public double ViewportHeight => _viewport.Height;

		/// <summary>
		/// <see cref="IScrollInfo"/> member
		/// </summary>
		public double ViewportWidth => _viewport.Width;


		/// <summary>
		/// <see cref="IScrollInfo"/> method
		/// </summary>
		public void LineDown()
		{
			PanVertically(1);
			ScrollOwner.InvalidateScrollInfo();
		}

		/// <summary>
		/// <see cref="IScrollInfo"/> method
		/// </summary>
		public void LineLeft()
		{
			PanHorizontally(1);
			ScrollOwner.InvalidateScrollInfo();
		}

		/// <summary>
		/// <see cref="IScrollInfo"/> method
		/// </summary>
		public void LineRight()
		{
			PanHorizontally(-1);
			ScrollOwner.InvalidateScrollInfo();
		}

		/// <summary>
		/// <see cref="IScrollInfo"/> method
		/// </summary>
		public void LineUp()
		{
			PanVertically(-1);
			ScrollOwner.InvalidateScrollInfo();
		}

		public Rect MakeVisible(Visual visual, Rect rectangle)
		{
			return new Rect(ScrollOwner.RenderSize);
		}

		/// <summary>
		/// <see cref="IScrollInfo"/> method
		/// </summary>
		public void MouseWheelDown()
		{
			PanVertically(1);
			ScrollOwner.InvalidateScrollInfo();
		}

		/// <summary>
		/// <see cref="IScrollInfo"/> method
		/// </summary>
		public void MouseWheelLeft()
		{
			PanHorizontally(1);
			ScrollOwner.InvalidateScrollInfo();
		}

		/// <summary>
		/// <see cref="IScrollInfo"/> method
		/// </summary>
		public void MouseWheelRight()
		{
			PanHorizontally(-1);
			ScrollOwner.InvalidateScrollInfo();
		}

		/// <summary>
		/// <see cref="IScrollInfo"/> method
		/// </summary>
		public void MouseWheelUp()
		{
			PanVertically(-1);
			ScrollOwner.InvalidateScrollInfo();
		}

		/// <summary>
		/// <see cref="IScrollInfo"/> method
		/// </summary>
		public void PageDown()
		{
			PanVertically(1);
			ScrollOwner.InvalidateScrollInfo();
		}

		/// <summary>
		/// <see cref="IScrollInfo"/> method
		/// </summary>
		public void PageLeft()
		{
			PanHorizontally(1);
			ScrollOwner.InvalidateScrollInfo();
		}

		/// <summary>
		/// <see cref="IScrollInfo"/> method
		/// </summary>
		public void PageRight()
		{
			PanHorizontally(-1);
			ScrollOwner.InvalidateScrollInfo();
		}

		/// <summary>
		/// <see cref="IScrollInfo"/> method
		/// </summary>
		public void PageUp()
		{
			PanVertically(-1);
			ScrollOwner.InvalidateScrollInfo();
		}

		/// <summary>
		/// <see cref="IScrollInfo"/> method
		/// </summary>
		public void SetHorizontalOffset(double offset)
		{
			if (!TranslatedHorizontally)
			{
				if (offset != _offset.X)
				{
					if (offset > _offset.X)
					{
						if (LeftLimit < MostLeftElement)
							ScrollHorizontally(1);
						else
							ScrollHorizontally(offset - _offset.X);
					}
					else
						ScrollHorizontally(offset - _offset.X);

					_offset.X = offset;
				}
			}
			else
			{
				offset = CoerceHorizontalOffset(offset);
				_offset.X = offset;
			}

			if (_offset.X == 0 && LeftLimit < MostLeftElement && RightLimit > MostRightElement)
				_extent.Width = _viewport.Width;

			TranslatedHorizontally = false;
		}

		/// <summary>
		/// <see cref="IScrollInfo"/> method
		/// </summary>
		public void SetVerticalOffset(double offset)
		{
			if (!TranslatedVertically)
			{
				if (offset != _offset.Y)
				{
					// todo: придумать другой способ дебагинга (ex. через логирование)
					// Console.WriteLine(offset);
					if (offset > _offset.Y)
					{
						if (TopLimit < HighestElement)
							ScrollVertically(1);
						else
							ScrollVertically(offset - _offset.Y);
					}
					else
						ScrollVertically(offset - _offset.Y);

					_offset.Y = offset;
				}
			}
			else
			{
				offset = CoerceVerticalOffset(offset);
				_offset.Y = offset;
			}

			if (_offset.Y == 0 && TopLimit < HighestElement && BottomLimit > LowestElement)
				_extent.Height = _viewport.Height;

			TranslatedVertically = false;
		}

		protected override void OnPreviewMouseLeftButtonDown(MouseButtonEventArgs e)
		{
			if (Keyboard.IsKeyDown(Key.Space))
			{
				_panInitialPosition = e.GetPosition(this);
				CaptureMouse();
			}
		}

		protected override void OnPreviewMouseMove(MouseEventArgs e)
		{
			if (Mouse.LeftButton != MouseButtonState.Pressed)
				return;

			var currentPosition = e.GetPosition(this);
			var deltaHeight = currentPosition.Y - _panInitialPosition.Y;
			var deltaWidth = currentPosition.X - _panInitialPosition.X;

			if (deltaWidth != 0)
				PanHorizontally(-deltaWidth);

			if (deltaHeight != 0)
				PanVertically(-deltaHeight);

			ScrollOwner.InvalidateScrollInfo();
			_panInitialPosition = currentPosition;
		}
		protected override void OnMouseUp(MouseButtonEventArgs e)
		{
			if (IsMouseCaptured)
				ReleaseMouseCapture();
		}
		protected override void OnPreviewMouseWheel(MouseWheelEventArgs e)
		{
			// @ todo: придумать как аккуратно сделать зум с Shift (по горизонтали)
		}

		protected override Size MeasureOverride(Size constraint)
		{
			if (ScrollOwner == null)
				return base.MeasureOverride(constraint);

			if (_viewport != constraint)
			{
				_viewportTopLeftInitial = new Point(0, 0);
				_viewport = constraint;
				_viewportBottomRightInitial = new Point(_viewport.Width, _viewport.Height);
				_initialExtent = _viewport;

				if (TopLimit < HighestElement && BottomLimit > LowestElement)
					_extent.Height = _viewport.Height;

				if (LeftLimit < MostLeftElement && RightLimit > MostRightElement)
					_extent.Width = _viewport.Width;

				AdjustScrollVertically();
				AdjustScrollHorizontally();
			}
			ScrollOwner.InvalidateScrollInfo();

			return base.MeasureOverride(constraint);
		}
		protected override Size ArrangeOverride(Size arrangeSize)
		{
			if (ScrollOwner == null)
				return base.ArrangeOverride(arrangeSize);

			if (_viewport != arrangeSize)
			{
				_viewportTopLeftInitial = new Point(0, 0);
				_viewport = arrangeSize;
				_viewportBottomRightInitial = new Point(_viewport.Width, _viewport.Height);
				_initialExtent = _viewport;

				if (TopLimit > HighestElement && BottomLimit > LowestElement)
					_extent.Height = _viewport.Height;

				if (LeftLimit < MostLeftElement && RightLimit > MostRightElement)
					_extent.Width = _viewport.Width;

				AdjustScrollVertically();
				AdjustScrollHorizontally();
			}
			ScrollOwner.InvalidateScrollInfo();

			return base.ArrangeOverride(arrangeSize);
		}

		internal void AdjustScrollVertically()
		{
			TranslatedVertically = true;
			SetVerticalOffset(TopOffset);
			UpdateExtentHeight();

			ScrollOwner.InvalidateScrollInfo();
		}

		internal void AdjustScrollHorizontally()
		{
			TranslatedHorizontally = true;
			SetHorizontalOffset(LeftOffset);
			UpdateExtentWidth();

			ScrollOwner.InvalidateScrollInfo();
		}

		/// <summary>
		/// Scrolls and translates vertically the <see cref="ScrollOwner"/> viewport
		/// </summary>
		/// <param name="offset">Scroll factor which determines the speed</param>
		/// <param name="reverseScroll">Reverse scrolling direction which is specifed by the sign of the offset</param>
		public void PanVertically(double offset, bool reverseScroll = false)
		{
			TranslatedVertically = false;
			if (reverseScroll)
				ScrollVertically(-offset);
			else
				ScrollVertically(offset);

			if (TopLimit > HighestElement || BottomLimit < LowestElement)
			{
				SetVerticalOffset(VerticalOffset + offset);
				UpdateExtentHeight();
			}
			else
				SetVerticalOffset(0);

			ScrollOwner.InvalidateScrollInfo();
		}

		/// <summary>
		/// Scrolls and translates horizontally the <see cref="ScrollOwner"/> viewport
		/// </summary>
		/// <param name="offset">Scroll factor which determines the speed</param>
		/// <param name="reverseScroll">Reverse scrolling direction which is specifed by the sign of the offset</param>
		public void PanHorizontally(double offset, bool reverseScroll = false)
		{
			TranslatedHorizontally = false;
			if (reverseScroll)
				ScrollHorizontally(-offset);
			else
				ScrollHorizontally(offset);

			if (LeftLimit > MostLeftElement || RightLimit < MostRightElement)
			{
				SetHorizontalOffset(offset);
				UpdateExtentWidth();
			}
			else
				SetHorizontalOffset(0);

			ScrollOwner.InvalidateScrollInfo();
		}

		public void Initialize(CustomCanvas canvas)
		{
			_parent = canvas;
			// _translateTransform = _parent.TranslateTransform;
		}

		private double CoerceVerticalOffset(double offset)
		{
			if (double.IsNaN(offset) || double.IsInfinity(offset))
				offset = 0;

			if (TopLimit > HighestElement)
				offset = TopOffset;
			else if (offset > 0)
				offset = Math.Min(_offset.Y + offset, BottomOffset);

			if (TopLimit < HighestElement && BottomLimit > LowestElement)
				offset = 0;

			return offset;
		}

		private double CoerceHorizontalOffset(double offset)
		{
			if (double.IsNaN(offset) || double.IsInfinity(offset))
				offset = 0;

			if (LeftLimit > MostLeftElement)
				offset = LeftOffset;
			else if (offset > 0)
				offset = Math.Min(_offset.X + offset, RightOffset);

			if (LeftLimit < MostLeftElement && RightLimit > MostRightElement)
				offset = 0;

			return offset;
		}

		private void UpdateExtentWidth()
		{
			if (LeftLimit > MostLeftElement && RightLimit > MostRightElement)
				_extent.Width = _initialExtent.Width + Math.Abs(LeftOffset);
			else if (RightLimit < MostRightElement && LeftLimit < MostLeftElement)
				_extent.Width = _initialExtent.Width + Math.Abs(RightOffset);
			else if (LeftLimit > MostLeftElement && RightLimit < MostRightElement)
				_extent.Width = _initialExtent.Width + LeftOffset + Math.Abs(RightOffset);
		}
		private void ScrollVertically(double offset)
		{
			_translateTransform.Y += -offset;
			TranslatedVertically = true;
		}

		private void ScrollHorizontally(double offset)
		{
			_translateTransform.X += -offset;
			TranslatedHorizontally = true;
		}

		private void UpdateExtentHeight()
		{
			if (TopLimit > HighestElement && BottomLimit > LowestElement)
				_extent.Height = _initialExtent.Height + Math.Abs(TopOffset);
			else if (BottomLimit < LowestElement && TopLimit < HighestElement)
				_extent.Height = _initialExtent.Height + Math.Abs(BottomOffset);
			else if (TopLimit > HighestElement && BottomLimit < LowestElement)
				_extent.Height = _initialExtent.Height + TopOffset + Math.Abs(BottomOffset);
		}
	}
}
