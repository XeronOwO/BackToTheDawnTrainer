using System;
using System.Windows.Markup;

namespace BackToTheDawnTrainer.Extensions;

public sealed class Int32Extension(int value) : MarkupExtension
{
	public int Value { get; set; } = value;

	public override object ProvideValue(IServiceProvider sp) => Value;
};
