using System;
using System.Windows.Media;
using CustomEditor.ViewModels.Base;
using Action = System.Action;

namespace CustomEditor.ViewModels
{
	public class VisualProperties : BaseViewModel
	{
		private Color? _fillColor;
		private Color _borderColor;
		private double _thickness;
		private double _width;
		private double _height;

		public VisualProperties()
		{
			_fillColor = Colors.Black;
			_borderColor = Colors.Black;
			_thickness = 0.0d;
			_width = 0.0d;
			_height = 0.0d;
		}

		public event Action FillColorChanged;
		public event Action BorderColorChanged;
		public event Action ThicknessChanged;
		public event Action WidthChanged;
		public event Action HeightChanged;

		public Color? FillColor
		{
			get => _fillColor;
			set
			{
				_fillColor = value;
				RaisePropertyChanged();
				FillColorChanged?.Invoke();
			}
		}

		public Color BorderColor
		{
			get => _borderColor;
			set
			{
				_borderColor = value;
				RaisePropertyChanged();
				BorderColorChanged?.Invoke();
			}
		}

		public double Thickness
		{
			get => _thickness;
			set
			{
				_thickness = value;
				RaisePropertyChanged();
				ThicknessChanged?.Invoke();
			}
		}

		public double Width
		{
			get => _width;
			set
			{
				_width = value;
				RaisePropertyChanged();
				WidthChanged?.Invoke();
			}
		}

		public double Height
		{
			get => _height;
			set
			{
				_height = value;
				RaisePropertyChanged();
				HeightChanged?.Invoke();
			}
		}
	}
}
