using System.Windows;
using CustomEditor.ViewModels.Base;

namespace CustomEditor.Models
{
	public class Action : BaseViewModel
	{
		private UIElement _involvedElement;
		private ActionType _actionType;

		public Action()
		{
			_actionType = ActionType.None;
			InvolvedElement = new UIElement();
		}

		public Action(UIElement uiElement, ActionType actionType)
		{
			InvolvedElement = uiElement;
			ActionType = actionType;
		}

		public UIElement InvolvedElement
		{
			get => _involvedElement;
			set
			{
				_involvedElement = value;
				RaisePropertyChanged();
			}
		}

		public ActionType ActionType
		{
			get => _actionType;
			set
			{
				_actionType = value;
				RaisePropertyChanged();
			}
		}
	}
}