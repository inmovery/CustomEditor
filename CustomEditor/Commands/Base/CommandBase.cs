using System;
using System.Diagnostics;
using System.Windows.Input;

namespace CustomEditor.Commands.Base
{
	/// <summary>
	/// Provides a base implementation of the <see cref="ICommand"/> interface.
	/// </summary>
	public abstract class CommandBase : ICommand
	{
		/// <summary>
		/// Occurs when changes occur that affect whether or not the command should execute.
		/// </summary>
		public event EventHandler CanExecuteChanged
		{
			add => CommandManager.RequerySuggested += value;
			remove => CommandManager.RequerySuggested -= value;
		}

		/// <summary>
		/// Tries to execute the command by checking the <see cref="CanExecute"/> property
		/// and executes the command only when it can be executed.
		/// </summary>
		/// <returns>True if command has been executed; False otherwise.</returns>
		public bool TryExecute()
		{
			if (!CanExecute())
				return false;

			Execute();
			return true;
		}

		void ICommand.Execute(object parameter)
		{
			Execute();
		}

		bool ICommand.CanExecute(object parameter)
		{
			return CanExecute();
		}

		/// <summary>
		/// Defines the method that determines whether the command can execute in its current state.
		/// </summary>
		/// <returns>True if command can be executed; False otherwise.</returns>
		public abstract bool CanExecute();

		/// <summary>
		/// Defines the method to be called when the command is invoked.
		/// </summary>
		public abstract void Execute();
	}

	/// <summary>
	/// Provides an implementation of the <see cref="ICommand"/> interface.
	/// </summary>
	/// <typeparam name="T">The type of the command parameter.</typeparam>
	public abstract class CommandBase<T> : ICommand
	{
		/// <summary>
		/// Occurs when changes occur that affect whether or not the command should execute.
		/// </summary>
		public event EventHandler CanExecuteChanged
		{
			add => CommandManager.RequerySuggested += value;
			remove => CommandManager.RequerySuggested -= value;
		}

		/// <summary>
		/// Tries to execute the command by calling the <see cref="CanExecute"/> method
		/// and executes the command only when it can be executed.
		/// </summary>
		/// <param name="parameter">Data used by the command.</param>
		/// <returns>True if command has been executed; False otherwise.</returns>
		public bool TryExecute(T parameter)
		{
			if (!CanExecute(parameter))
				return false;

			Execute(parameter);

			return true;
		}

		[DebuggerStepThrough]
		bool ICommand.CanExecute(object parameter)
		{
			return CanExecute((T)parameter);
		}

		void ICommand.Execute(object parameter)
		{
			Execute((T)parameter);
		}

		/// <summary>
		/// Gets a value indicating whether the command can execute in its current state.
		/// </summary>
		/// <param name="parameter">Data used by the command.</param>
		/// <returns>True if command can be executed; False otherwise.</returns>
		[DebuggerStepThrough]
		public abstract bool CanExecute(T parameter);

		/// <summary>
		/// Defines the method to be called when the command is invoked.
		/// </summary>
		/// <param name="parameter">Data used by the command.</param>
		public abstract void Execute(T parameter);
	}
}
