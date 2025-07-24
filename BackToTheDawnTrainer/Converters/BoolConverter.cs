using System;
using System.Globalization;
using System.Windows.Data;

namespace BackToTheDawnTrainer.Converters;

public class BoolConverter : IValueConverter
{
	public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
	{
		return value?.Equals(parameter) ?? false;
	}

	public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
	{
		return (value is bool isChecked && isChecked) ? parameter : Binding.DoNothing;
	}
}
