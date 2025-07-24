using BackToTheDawnTrainer.ViewModels;
using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;

namespace BackToTheDawnTrainer.Views;

/// <summary>
/// DiceValueView.xaml 的交互逻辑
/// </summary>
public partial class DiceValueView : UserControl
{
	public static readonly DependencyProperty GroupNameProperty = DependencyProperty.Register(
		"GroupName", typeof(string), typeof(DiceValueView), new PropertyMetadata(string.Empty, OnGroupNameChanged));

	private static void OnGroupNameChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
	{
		if (d is DiceValueView view)
		{
			view.DataContext.GroupName = (string)e.NewValue;
		}
	}

	public string GroupName
	{
		get => (string)GetValue(GroupNameProperty);
		set => SetValue(GroupNameProperty, value);
	}

	public new DiceValueViewModel DataContext
	{
		get => (DiceValueViewModel)base.DataContext;
		set => base.DataContext = value;
	}

	public DiceValueView(DiceValueViewModel viewModel)
	{
		DataContext = viewModel;
		InitializeComponent();

		DataContext.PropertyChanged += DataContext_PropertyChanged;
	}

	private void DataContext_PropertyChanged(object? sender, PropertyChangedEventArgs e)
	{
		if (e.PropertyName == nameof(DiceValueViewModel.Value))
		{
			ValueChanged?.Invoke(this, new() { Value = DataContext.Value });
		}
	}

	public class DiceEventArgs : EventArgs
	{
		public required DiceType Value { get; init; }
	}

	public event EventHandler<DiceEventArgs>? ValueChanged;

	public DiceType Value
	{
		get => DataContext.Value;
		set => DataContext.Value = value;
	}
}
