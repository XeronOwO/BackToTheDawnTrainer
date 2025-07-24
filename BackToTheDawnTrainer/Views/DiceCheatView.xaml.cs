using BackToTheDawnTrainer.ViewModels;
using System;
using System.Windows.Controls;

namespace BackToTheDawnTrainer.Views;

/// <summary>
/// DiceCheatView.xaml 的交互逻辑
/// </summary>
public partial class DiceCheatView : UserControl
{
	public new DiceCheatViewModel DataContext
	{
		get => (DiceCheatViewModel)base.DataContext;
		set => base.DataContext = value;
	}

	private readonly DiceValueView _diceValueView1;

	private readonly DiceValueView _diceValueView2;

	public DiceCheatView(DiceCheatViewModel viewModel, DiceValueView diceValueView1, DiceValueView diceValueView2)
	{
		DataContext = viewModel;
		InitializeComponent();
		_contentControl1.Content = _diceValueView1 = diceValueView1;
		_contentControl2.Content = _diceValueView2 = diceValueView2;

		_diceValueView1.ValueChanged += DiceValueView_ValueChanged;
		_diceValueView2.ValueChanged += DiceValueView_ValueChanged;
	}

	private void DiceValueView_ValueChanged(object? sender, DiceValueView.DiceEventArgs e)
	{
		ValuesChanged?.Invoke(this, new()
		{
			Dice1 = _diceValueView1.Value,
			Dice2 = _diceValueView2.Value,
		});
	}

	public class DiceEventArgs : EventArgs
	{
		public required DiceType Dice1 { get; init; }

		public required DiceType Dice2 { get; init; }
	}

	public event EventHandler<DiceEventArgs>? ValuesChanged;

	public DiceType Dice1
	{
		get => _diceValueView1.Value;
		set => _diceValueView1.Value = value;
	}

	public DiceType Dice2
	{
		get => _diceValueView2.Value;
		set => _diceValueView2.Value = value;
	}
}
