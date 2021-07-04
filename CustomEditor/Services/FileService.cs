using System.IO;
using Microsoft.Win32;

namespace CustomEditor.Services
{
	public class FileService
	{
		public void OpenFileDialog(out string selectedPath)
		{
			selectedPath = string.Empty;
			var openFileDialog = new OpenFileDialog
			{
				Filter = "Image files (*.png;*.jpeg;*jpg)|*.png;*.jpeg;*jpg|All files (*.*)|*.*"
			};

			if (openFileDialog.ShowDialog() == true)
				selectedPath = openFileDialog.FileName;
		}

		public void SaveFileDialog(out string selectedPath)
		{
			selectedPath = string.Empty;
			var saveFileDialog = new SaveFileDialog
			{
				RestoreDirectory = true,
				Filter = "Image files (*.png;*.jpeg;*jpg)|*.png;*.jpeg;*jpg|All files (*.*)|*.*"
			};

			if (saveFileDialog.ShowDialog() == true)
				selectedPath = saveFileDialog.FileName;
		}
	}
}
