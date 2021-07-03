﻿using System;
using System.Linq;
using System.Windows.Media;

namespace CustomEditor.Controls.ColorPicker.ColorManipulation
{
	public static class HsbConverter
	{
		public static Color ToColor(this Hsb hsv)
		{
			var h = hsv.Hue;
			var s = hsv.Saturation;
			var b = hsv.Brightness;

			b *= 255;

			if (s.IsCloseTo(0))
				return Color.FromRgb((byte)b, (byte)b, (byte)b);

			if (h.IsCloseTo(360))
				h = 0;

			while (h > 360)
				h -= 360;

			while (h < 0)
				h += 360;

			h /= 60;

			var i = (int)Math.Floor(h);
			var f = h - i;
			var p = b * (1 - s);
			var q = b * (1 - s * f);
			var t = b * (1 - s * (1 - f));

			return i switch
			{
				0 => Color.FromRgb((byte) b, (byte) t, (byte) p),
				1 => Color.FromRgb((byte) q, (byte) b, (byte) p),
				2 => Color.FromRgb((byte) p, (byte) b, (byte) t),
				3 => Color.FromRgb((byte) p, (byte) q, (byte) b),
				4 => Color.FromRgb((byte) t, (byte) p, (byte) b),
				5 => Color.FromRgb((byte) b, (byte) p, (byte) q),
				_ => throw new Exception("Invalid HSB values")
			};
		}

		public static Hsb ToHsb(this Color color)
		{
			double r = color.R;
			double g = color.G;
			double b = color.B;

			r /= 255;
			g /= 255;
			b /= 255;

			var rgb = new[] { r, g, b };
			var max = rgb.Max();
			var min = rgb.Min();
			var v = max;
			var h = max;

			var d = max - min;
			var s = max.IsCloseTo(0) ? 0 : d / max;

			if (max.IsCloseTo(min))
				h = 0;
			else
			{
				if (max.IsCloseTo(r))
					h = (g - b) / d + (g < b ? 6 : 0);
				else if (max.IsCloseTo(g))
					h = (b - r) / d + 2;
				else if (max.IsCloseTo(b))
					h = (r - g) / d + 4;

				h *= 60;
			}

			return new Hsb(h, s, v);
		}

		private static bool IsCloseTo(this double value, double target, double tolerance = double.Epsilon)
		{
			return Math.Abs(value - target) < tolerance;
		}
	}
}
