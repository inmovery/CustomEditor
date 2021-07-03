using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace CustomEditor.ViewModels.Base
{
	/// <summary>
	/// A base view model that fires Property Changed events as needed
	/// </summary>
	public class BaseViewModel : INotifyPropertyChanged
	{

		/// <summary>
		/// The event that is fired when any child property changes its value
		/// </summary>
		public event PropertyChangedEventHandler PropertyChanged = (sender, e) => { };

		public void SetProperty<T>(ref T property, T value, [CallerMemberName] string propertyName = null)
		{
			if (EqualityComparer<T>.Default.Equals(property, value))
				return;

			property = value;
			OnPropertyChanged(propertyName);
		}

		/// <summary>
		/// Call this to fire a <see cref="PropertyChanged"/> event
		/// </summary>
		/// <param name="propertyName"></param>
		public void OnPropertyChanged(string propertyName) => PropertyChanged(this, new PropertyChangedEventArgs(propertyName));

		/// <summary>
		/// Call this to a fire a PropertyChanged event by typical method
		/// </summary>
		/// <param name="propertyName"></param>
		protected virtual void RaisePropertyChanged([CallerMemberName] string propertyName = "")
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}
	}
}
