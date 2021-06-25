using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using CustomEditor.Controls;

namespace CustomEditor.Views
{
	public partial class MainWindow : Window
	{
		public MainWindow()
		{
			InitializeComponent();
		}

		private void ChangeColor_OnClick(object sender, RoutedEventArgs e)
		{
			(Workspace.SelectedItem as Shape).Fill = Brushes.Red;
		}
	}
}