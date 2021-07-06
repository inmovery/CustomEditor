using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using CustomEditor.Controls;

namespace CustomEditor.Services
{
	public class CanvasService
	{

		public void ExportCanvasToProjectFile(CustomCanvas canvas, string filename)
		{
			var xamlString = XamlWriter.Save(canvas);
			using (var streamWriter = new StreamWriter(File.Create(filename)))
			{
				streamWriter.Write(xamlString);
			}
		}

		public void LoadWorkspaceCanvas(CustomCanvas canvas, string filename)
		{
			var canvasSource = XamlReader.Load(new FileStream(filename, FileMode.Open)) as CustomCanvas;
			if (canvasSource == null)
				return; // todo: добавить тут вывод сообщение о том, что что-то пошло не так

			canvas.Children.Clear();
			UpdateCanvasChildren(canvas, canvasSource);
		}

		private void UpdateCanvasChildren(CustomCanvas targetCanvas, CustomCanvas canvasSource)
		{
			if (canvasSource.Children.Count < 1)
				return;

			var canvasChild = canvasSource.Children[0];
			if (canvasChild == null)
				return;

			canvasSource.Children.Remove(canvasChild);
			targetCanvas.Children.Add(canvasChild);

			UpdateCanvasChildren(targetCanvas, canvasSource);
		}

		public void ExportCanvasToImage(CustomCanvas canvas, string filename)
		{
			var pixelHeight = (int)canvas.ActualHeight;
			var pixelWidth = (int)canvas.ActualWidth;
			var dpi = 96d;

			var pixelFormat = PixelFormats.Pbgra32; // PixelFormats.Default
			var bitmapObject = new RenderTargetBitmap(pixelWidth, pixelHeight, dpi, dpi, PixelFormats.Default);
			bitmapObject.Render(canvas);

			Export(bitmapObject, filename);
		}

		private void Export(object bitmapObject, string filename)
		{
			if (bitmapObject.Equals(null))
				return;

			var type = bitmapObject.GetType();
			if (type != typeof(string) && type != typeof(BitmapImage) && type != typeof(RenderTargetBitmap))
				return;

			try
			{
				var extension = Path.GetExtension(filename).ToLower();

				BitmapEncoder encoder;
				switch (extension)
				{
					case ".gif":
						encoder = new GifBitmapEncoder();
						break;
					case ".png":
						encoder = new PngBitmapEncoder();
						break;
					case ".jpg":
					case ".jpeg":
						encoder = new JpegBitmapEncoder();
						break;
					default:
						return;
				}

				encoder.Frames.Add(type == typeof(BitmapImage)
					? BitmapFrame.Create((BitmapImage)bitmapObject)
					: BitmapFrame.Create((RenderTargetBitmap)bitmapObject));

				using (var fileStream = new FileStream(filename, FileMode.Create, FileAccess.Write))
				{
					encoder.Save(fileStream);
				}
			}
			catch (Exception ex)
			{
				// todo: изменить на какое-нибудь другое окошко
				MessageBox.Show("Error: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
			}
		}

		public void ImportImage(CustomCanvas canvas, string filename)
		{
			var image = new Image();
			var bitmapImage = new BitmapImage(new Uri(filename));

			var pixelWidth = canvas.ActualWidth / 4;
			var pixelHeight = canvas.ActualHeight / 4;
			var bitmapWidth = bitmapImage.PixelWidth;
			var bitmapHeight = bitmapImage.PixelHeight;

			if (bitmapWidth < pixelWidth && bitmapHeight < pixelHeight)
			{
				image.Source = bitmapImage;
				image.Width = bitmapWidth;
				image.Height = bitmapHeight;
			}
			else
			{
				var scaleX = pixelWidth / bitmapWidth;
				var scaleY = pixelHeight / bitmapHeight;
				image.Source = new TransformedBitmap(bitmapImage, new ScaleTransform(scaleX, scaleY));
				image.Width = bitmapWidth * scaleX;
				image.Height = bitmapHeight * scaleY;
			}

			image.RenderTransformOrigin = new Point(0.5d, 0.5d);
			image.SnapsToDevicePixels = true;
			image.Stretch = Stretch.Fill;

			Canvas.SetLeft(image, 0.0d);
			Canvas.SetTop(image, 0.0d);

			canvas.Children.Add(image);
		}
	}
}
