using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace CustomEditor.Controls
{
	public class FilteredTextBox : TextBox
	{
		public event Action ValidateTextCompleted;

		public FilteredTextBox()
		{
			TextChanged += OnTextChanged;
		}

		public int MinimumDigit
		{
			get { return (int)GetValue(MinimumDigitProperty); }
			set { SetValue(MinimumDigitProperty, value); }
		}

		public static readonly DependencyProperty MinimumDigitProperty = DependencyProperty.Register(
			nameof(MinimumDigit),
			typeof(int),
			typeof(FilteredTextBox),
			new FrameworkPropertyMetadata(-1));

		public int MaximumDigit
		{
			get { return (int)GetValue(MaximumDigitProperty); }
			set { SetValue(MaximumDigitProperty, value); }
		}

		public static readonly DependencyProperty MaximumDigitProperty = DependencyProperty.Register(
			nameof(MaximumDigit),
			typeof(int),
			typeof(FilteredTextBox),
			new FrameworkPropertyMetadata(-1));

		protected override void OnPreviewTextInput(TextCompositionEventArgs eventArgs)
		{
			base.OnPreviewTextInput(eventArgs);

			var letterOrDigit = Convert.ToChar(eventArgs.Text);

			if (!char.IsDigit(letterOrDigit))
				eventArgs.Handled = true;

			var newText = Text + eventArgs.Text;
			var successParsing = int.TryParse(newText, out var newDigitValue);
			if (MinimumDigit > -1 && successParsing && newDigitValue < MinimumDigit)
				eventArgs.Handled = true;

			if (MaximumDigit > -1 && successParsing && newDigitValue > MaximumDigit)
				eventArgs.Handled = true;
		}

		private void OnTextChanged(object sender, TextChangedEventArgs textChangedEventArgs)
		{
			if (string.IsNullOrEmpty(Text))
				SetValue(TextProperty, null);

			// var value = (textChangedEventArgs.Source as FilteredTextBox)?.Text;
			// var validatedText = double.TryParse(value, NumberStyles.Any, CultureInfo.InvariantCulture, out var validatedValue);

			ValidateTextCompleted?.Invoke();
		}
	}
}
