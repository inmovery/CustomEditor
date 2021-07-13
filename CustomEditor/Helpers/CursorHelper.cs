using System.IO;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace CustomEditor.Helpers
{
	public static class CursorHelper
	{
		public static Cursor ConvertToCursor(UIElement control, Point hotSpot)
		{
			// convert UIElement to PNG stream
			var pngStream = new MemoryStream();
			control.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
			var rect = new Rect(0, 0, control.DesiredSize.Width, control.DesiredSize.Height);
			var rtb = new RenderTargetBitmap((int)control.DesiredSize.Width, (int)control.DesiredSize.Height, 96, 96,
				PixelFormats.Pbgra32);

			control.Arrange(rect);
			rtb.Render(control);

			var png = new PngBitmapEncoder();
			png.Frames.Add(BitmapFrame.Create(rtb));
			png.Save(pngStream);

			// write cursor header info
			var cursorStream = new MemoryStream();
			cursorStream.Write(new[] { (byte)0x00, (byte)0x00 }, 0, 2);// ICONDIR: Reserved. Must always be 0.
			cursorStream.Write(new[] { (byte)0x02, (byte)0x00 }, 0, 2);// ICONDIR: Specifies image type: 1 for icon (.ICO) image, 2 for cursor (.CUR) image. Other values are invalid
			cursorStream.Write(new[] { (byte)0x01, (byte)0x00 }, 0, 2);// ICONDIR: Specifies number of images in the file.
			cursorStream.Write(new[] { (byte)control.DesiredSize.Width }, 0, 1);// ICONDIRENTRY: Specifies image width in pixels. Can be any number between 0 and 255. Value 0 means image width is 256 pixels.
			cursorStream.Write(new[] { (byte)control.DesiredSize.Height }, 0, 1);// ICONDIRENTRY: Specifies image height in pixels. Can be any number between 0 and 255. Value 0 means image height is 256 pixels.
			cursorStream.Write(new[] { (byte)0x00 }, 0, 1);// ICONDIRENTRY: Specifies number of colors in the color palette. Should be 0 if the image does not use a color palette.
			cursorStream.Write(new[] { (byte)0x00 }, 0, 1);// ICONDIRENTRY: Reserved. Should be 0.
			cursorStream.Write(new[] { (byte)hotSpot.X, (byte)0x00 }, 0, 2);// ICONDIRENTRY: Specifies the horizontal coordinates of the hotspot in number of pixels from the left.
			cursorStream.Write(new[] { (byte)hotSpot.Y, (byte)0x00 }, 0, 2);// ICONDIRENTRY: Specifies the vertical coordinates of the hotspot in number of pixels from the top.
			cursorStream.Write(new[]
				{
                    // ICONDIRENTRY: Specifies the size of the image's data in bytes
                    (byte) ((pngStream.Length & 0x000000FF)),
					(byte) ((pngStream.Length & 0x0000FF00) >> 0x08),
					(byte) ((pngStream.Length & 0x00FF0000) >> 0x10),
					(byte) ((pngStream.Length & 0xFF000000) >> 0x18)
				}, 0, 4);
			cursorStream.Write(new[]
				{
                    // ICONDIRENTRY: Specifies the offset of BMP or PNG data from the beginning of the ICO/CUR file
                    (byte) 0x16,
					(byte) 0x00,
					(byte) 0x00,
					(byte) 0x00,
				}, 0, 4);

			// copy PNG stream to cursor stream
			pngStream.Seek(0, SeekOrigin.Begin);
			pngStream.CopyTo(cursorStream);

			// return cursor stream
			cursorStream.Seek(0, SeekOrigin.Begin);
			return new Cursor(cursorStream);
		}
	}
}
