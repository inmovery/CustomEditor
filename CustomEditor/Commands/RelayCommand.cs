using System;
using System.Diagnostics;
using System.Windows.Input;
using CustomEditor.Commands.Base;

namespace CustomEditor.Commands
{
	/// <summary>
	/// Provides an implementation of the <see cref="ICommand"/> interface.
	/// </summary>
	public class RelayCommand : CommandBase
	{
		private readonly Action _execute;
		private readonly Func<bool> _canExecute;

		/// <summary>
		/// Initializes a new instance of the <see cref="RelayCommand"/> class.
		/// </summary>
		/// <param name="execute">The action to execute.</param>
		public RelayCommand(Action execute) : this(execute, null)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="RelayCommand"/> class.
		/// </summary>
		/// <param name="execute">The action to execute.</param>
		/// <param name="canExecute">The predicate to check whether the function can be executed.</param>
		public RelayCommand(Action execute, Func<bool> canExecute)
		{
			_execute = execute ?? throw new ArgumentNullException(nameof(execute));
			_canExecute = canExecute;
		}

		/// <summary>
		/// Defines the method that determines whether the command can execute in its current state.
		/// </summary>
		/// <returns>True if command can be executed; False otherwise.</returns>
		public override bool CanExecute()
		{
			return _canExecute == null || _canExecute();
		}

		/// <summary>
		/// Defines the method to be called when the command is invoked.
		/// </summary>
		public override void Execute()
		{
			_execute();
		}
	}

	/// <summary>
	/// Provides an implementation of the <see cref="ICommand"/> interface.
	/// </summary>
	/// <typeparam name="T">The type of the command parameter.</typeparam>
	public class RelayCommand<T> : CommandBase<T>
	{
		private readonly Action<T> _execute;
		private readonly Predicate<T> _canExecute;

		/// <summary>
		/// Initializes a new instance of the <see cref="RelayCommand{T}"/> class.
		/// </summary>
		/// <param name="execute">The action to execute.</param>
		public RelayCommand(Action<T> execute) : this(execute, null)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="RelayCommand{T}"/> class.
		/// </summary>
		/// <param name="execute">The action to execute.</param>
		/// <param name="canExecute">The predicate to check whether the function can be executed.</param>
		public RelayCommand(Action<T> execute, Predicate<T> canExecute)
		{
			_execute = execute ?? throw new ArgumentNullException(nameof(execute));
			_canExecute = canExecute;
		}

		/// <summary>
		/// Gets a value indicating whether the command can execute in its current state.
		/// </summary>
		/// <param name="parameter">Data used by the command.</param>
		/// <returns>True if command can be executed; False otherwise.</returns>
		[DebuggerStepThrough]
		public override bool CanExecute(T parameter)
		{
			return _canExecute == null || _canExecute(parameter);
		}

		/// <summary>
		/// Defines the method to be called when the command is invoked.
		/// </summary>
		/// <param name="parameter">Data used by the command.</param>
		public override void Execute(T parameter)
		{
			_execute(parameter);
		}
	}
}
