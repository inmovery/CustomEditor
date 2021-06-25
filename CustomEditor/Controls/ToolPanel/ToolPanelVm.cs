using CustomEditor.Models;
using CustomEditor.ViewModels.Base;

namespace CustomEditor.Controls.ToolPanel
{
	public class ToolPanelVm : BaseViewModel
	{
		private ToolType _activeToolType;
		private bool _isRectangleToolActive;
		private bool _isPolylineToolActive;
		private bool _isSelectToolActive;

		public ToolPanelVm()
		{
			IsSelectToolActive = true;
		}

		public ToolType ActiveToolType
		{
			get => _activeToolType;
			set
			{
				_activeToolType = value;
				RaisePropertyChanged();
			}
		}

		public bool IsRectangleToolActive
		{
			get => _isRectangleToolActive;
			set
			{
				_isRectangleToolActive = value;
				if (IsRectangleToolActive)
				{
					ActiveToolType = ToolType.Rectangle;
					IsPolylineToolActive = false;
					IsSelectToolActive = false;
				}

				RaisePropertyChanged();
			}
		}

		public bool IsPolylineToolActive
		{
			get => _isPolylineToolActive;
			set
			{
				_isPolylineToolActive = value;
				if (IsPolylineToolActive)
				{
					ActiveToolType = ToolType.Polyline;
					IsRectangleToolActive = false;
					IsSelectToolActive = false;
				}

				RaisePropertyChanged();
			}
		}

		public bool IsSelectToolActive
		{
			get => _isSelectToolActive;
			set
			{
				_isSelectToolActive = value;
				if (IsSelectToolActive)
				{
					ActiveToolType = ToolType.Select;
					IsRectangleToolActive = false;
					IsPolylineToolActive = false;
				}

				RaisePropertyChanged();
			}
		}
	}
}
