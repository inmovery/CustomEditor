using System;
using CustomEditor.Models;
using Microsoft.Win32;

namespace CustomEditor.Services
{
	public class FileService
	{
		public void OpenFileDialog(out string selectedPath, string filter)
		{
			selectedPath = string.Empty;
			var openFileDialog = new OpenFileDialog
			{
				InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop),
				Filter = filter
			};

			if (openFileDialog.ShowDialog() == true)
				selectedPath = openFileDialog.FileName;
		}

		public void SaveFileDialog(out string selectedPath, string filter)
		{
			selectedPath = string.Empty;
			var saveFileDialog = new SaveFileDialog
			{
				InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop),
				Filter = filter
			};

			if (saveFileDialog.ShowDialog() == true)
				selectedPath = saveFileDialog.FileName;
		}
	}
}
