using System.Windows;
using System.Windows.Input;
using Microsoft.Xaml.Behaviors;

namespace CustomEditor.Behaviors
{
	public class DoubleClickToCommandBehavior : Behavior<FrameworkElement>
	{
		public static readonly DependencyProperty CommandProperty = DependencyProperty.Register(
			nameof(Command),
			typeof(ICommand),
			typeof(DoubleClickToCommandBehavior),
			new UIPropertyMetadata(null));

		public ICommand Command
		{
			get => (ICommand)GetValue(CommandProperty);
			set => SetValue(CommandProperty, value);
		}

		public static DependencyProperty CommandParameterProperty = DependencyProperty.RegisterAttached(
			nameof(CommandParameter),
			typeof(object),
			typeof(DoubleClickToCommandBehavior),
			new UIPropertyMetadata(null));

		public object CommandParameter
		{
			get => GetValue(CommandParameterProperty);
			set => SetValue(CommandParameterProperty, value);
		}

		protected override void OnAttached()
		{
			base.OnAttached();
			AssociatedObject.MouseLeftButtonDown += OnMouseLeftButtonDown;
		}

		private void OnMouseLeftButtonDown(object sender, MouseButtonEventArgs eventArgs)
		{
			if (eventArgs.ClickCount < 2)
				return;

			if (Command == null)
				return;

			if (Command.CanExecute(null))
				Command.Execute(CommandParameter);
		}

		protected override Freezable CreateInstanceCore()
		{
			throw new System.NotImplementedException();
		}
	}
}
