using System;
using System.IO;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using CustomEditor.Helpers;

namespace CustomEditor.Controls.Thumbs
{
	public class RotateThumb : Thumb
	{
		private Point _centerPoint;
		private Vector _startVector;
		private double _initialAngle;

		private RotateTransform _rotateTransform;

		private UIElement _adornedElement;

		public static readonly DependencyProperty VisualAngleProperty = DependencyProperty.Register(
			nameof(VisualAngle),
			typeof(double),
			typeof(RotateThumb),
			new PropertyMetadata(0.0d, OnInitialAngleChanged));

		private static void OnInitialAngleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			var thumb = d as RotateThumb;
			var angle = (double)e.NewValue;

			thumb?.UpdateCursor(angle);
		}

		public double VisualAngle
		{
			get => (double)GetValue(VisualAngleProperty);
			set => SetValue(VisualAngleProperty, value);
		}

		public RotateThumb()
		{
			ForceCursor = true;
			DragDelta += RotateThumb_DragDelta;
			DragStarted += RotateThumb_DragStarted;
			DragCompleted += RotateThumb_DragCompleted;
		}

		protected override void OnMouseEnter(MouseEventArgs e)
		{
			_adornedElement = DataContext as UIElement;

			var rotateTransform = _adornedElement?.RenderTransform as RotateTransform;
			if (rotateTransform == null)
			{
				UpdateCursor(VisualAngle);
				return;
			}

			var changedAngle = rotateTransform.Angle;
			var newAngle = VisualAngle + changedAngle;

			UpdateCursor(newAngle);
			base.OnMouseEnter(e);
		}

		private void UpdateCursor(double angle)
		{
			Cursor = CreateRotateCursor(angle);
		}

		private void RotateThumb_DragStarted(object sender, DragStartedEventArgs e)
		{
			_adornedElement = DataContext as UIElement;
			if (_adornedElement == null)
				return;

			var canvas = VisualTreeHelper.GetParent(_adornedElement) as CustomCanvas;
			if (canvas == null)
				return;

			var element = _adornedElement as FrameworkElement ?? new FrameworkElement();
			_centerPoint = _adornedElement.TranslatePoint(
				new Point(
					element.Width * _adornedElement.RenderTransformOrigin.X,
					element.Height * _adornedElement.RenderTransformOrigin.Y),
				canvas);

			var startPoint = Mouse.GetPosition(canvas);
			_startVector = Point.Subtract(startPoint, _centerPoint);

			_rotateTransform = _adornedElement.RenderTransform as RotateTransform;
			if (_rotateTransform == null)
			{
				_adornedElement.RenderTransform = new RotateTransform(0);
				_initialAngle = 0;
			}
			else
				_initialAngle = _rotateTransform.Angle;
		}

		private void RotateThumb_DragDelta(object sender, DragDeltaEventArgs e)
		{
			var canvas = VisualTreeHelper.GetParent(_adornedElement) as CustomCanvas;
			if (_adornedElement == null || canvas == null)
				return;

			var currentPoint = Mouse.GetPosition(canvas);
			var deltaVector = Point.Subtract(currentPoint, _centerPoint);

			var angle = Vector.AngleBetween(_startVector, deltaVector);

			Cursor = CreateRotateCursor(VisualAngle + _initialAngle + angle);

			if (_adornedElement.RenderTransform is RotateTransform rotateTransform)
				rotateTransform.Angle = _initialAngle + Math.Round(angle, 0);

			_adornedElement.InvalidateMeasure();
		}

		private Cursor CreateRotateCursor(double angle)
		{
			var rotateTransform = new RotateTransform
			{
				CenterX = 22,
				CenterY = 11,
				Angle = angle
			};

			var drawingBrush = new DrawingBrush
			{
				Stretch = Stretch.Uniform,
				Drawing = IconsHelper.GetDrawingTemplateByName("RotateDrawing"),
				Transform = rotateTransform
			};

			var drawingVisual = new DrawingVisual();
			using (var drawingContext = drawingVisual.RenderOpen())
			{
				drawingContext.DrawRectangle(drawingBrush, new Pen(Brushes.Blue, 0), new Rect(0, 0, 44, 22));
				drawingContext.Close();
			}

			var rtb = new RenderTargetBitmap(44, 22, 96, 96, PixelFormats.Pbgra32);
			rtb.Render(drawingVisual);

			using (var pngStream = new MemoryStream())
			{
				var encoder = new PngBitmapEncoder();
				encoder.Frames.Add(BitmapFrame.Create(rtb));
				encoder.Save(pngStream);
				pngStream.Position = 0;

				var pngBytes = pngStream.ToArray();
				var size = pngBytes.GetLength(0);

				using (var cursorStream = new MemoryStream())
				{
					// Reserved must be zero; 2 bytes
					cursorStream.Write(BitConverter.GetBytes((short)0), 0, 2);

					// Image type 1 = ico 2 = cur; 2 bytes
					cursorStream.Write(BitConverter.GetBytes((short)2), 0, 2);

					// Number of images; 2 bytes
					cursorStream.Write(BitConverter.GetBytes((short)1), 0, 2);

					cursorStream.WriteByte(44); // Width
					cursorStream.WriteByte(22); // Height

					// Number of Colors in the color palette. Should be 0 if the image doesn't use a color palette
					cursorStream.WriteByte(0);

					// Reserved must be 0
					cursorStream.WriteByte(0);

					// 2 bytes. In CUR format: Specifies the horizontal coordinates of the hotspot in number of pixels from the left
					cursorStream.Write(BitConverter.GetBytes((short)(44 / 2.0)), 0, 2);

					// 2 bytes. In CUR format: Specifies the vertical coordinates of the hotspot in number of pixels from the top
					cursorStream.Write(BitConverter.GetBytes((short)(22 / 2.0)), 0, 2);

					// Specifies the size of the image's data in bytes
					cursorStream.Write(BitConverter.GetBytes(size), 0, 4);

					// Specifies the offset of BMP or PNG data from the beginning of the ICO/CUR file
					cursorStream.Write(BitConverter.GetBytes(22), 0, 4);

					// Write the png data
					cursorStream.Write(pngBytes, 0, size);
					cursorStream.Seek(0, SeekOrigin.Begin);

					return new Cursor(cursorStream);
				}
			}
		}

		private void RotateThumb_DragCompleted(object sender, DragCompletedEventArgs e)
		{
			// UpdatePropertiesState();
		}
	}
}
