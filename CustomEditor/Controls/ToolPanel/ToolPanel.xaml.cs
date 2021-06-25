using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace CustomEditor.Controls.ToolPanel
{
	public partial class ToolPanel : UserControl
	{
		public ToolPanel()
		{
			InitializeComponent();
		}

		public static readonly DependencyProperty IsShapeSelectedProperty = DependencyProperty.Register(
			nameof(IsShapeSelected),
			typeof(bool),
			typeof(ToolPanel),
			new PropertyMetadata(false));

		public bool IsShapeSelected
		{
			get => (bool)GetValue(IsShapeSelectedProperty);
			set => SetValue(IsShapeSelectedProperty, value);
		}

		public static readonly DependencyProperty ExportCommandProperty = DependencyProperty.Register(
			nameof(ExportCommand),
			typeof(ICommand),
			typeof(ToolPanel),
			new PropertyMetadata(null));

		public ICommand ExportCommand
		{
			get => (ICommand)GetValue(ExportCommandProperty);
			set => SetValue(ExportCommandProperty, value);
		}

		public static readonly DependencyProperty DeleteCommandProperty = DependencyProperty.Register(
			nameof(DeleteCommand),
			typeof(ICommand),
			typeof(ToolPanel),
			new PropertyMetadata(null));

		public ICommand DeleteCommand
		{
			get => (ICommand)GetValue(DeleteCommandProperty);
			set => SetValue(DeleteCommandProperty, value);
		}

		public static readonly DependencyProperty IsDeleteVisibleProperty = DependencyProperty.Register(
			nameof(IsDeleteVisible),
			typeof(bool),
			typeof(ToolPanel),
			new PropertyMetadata(false));

		public bool IsDeleteVisible
		{
			get => (bool)GetValue(IsDeleteVisibleProperty);
			set => SetValue(IsDeleteVisibleProperty, value);
		}
	}
}