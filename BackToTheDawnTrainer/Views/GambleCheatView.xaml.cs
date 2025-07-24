using BackToTheDawnTrainer.ViewModels;
using System;
using System.Windows.Controls;

namespace BackToTheDawnTrainer.Views;

/// <summary>
/// GambleCheatView.xaml 的交互逻辑
/// </summary>
public partial class GambleCheatView : UserControl
{
	public new GambleCheatViewModel DataContext
	{
		get => (GambleCheatViewModel)base.DataContext;
		set => base.DataContext = value;
	}

	public PokeCard[] Cards { get; }

	public GambleCheatView(
		GambleCardsView gambleCardsView1,
		GambleCardsView gambleCardsView2,
		GambleCardsView gambleCardsView3,
		GambleCardsView gambleCardsView4,
		GambleCheatViewModel viewModel)
	{
		DataContext = viewModel;
		InitializeComponent();
		_contentControl1.Content = gambleCardsView1;
		_contentControl2.Content = gambleCardsView2;
		_contentControl3.Content = gambleCardsView3;
		_contentControl4.Content = gambleCardsView4;
		Cards =
		[
			..gambleCardsView1.Cards,
			..gambleCardsView2.Cards,
			..gambleCardsView3.Cards,
			..gambleCardsView4.Cards,
		];

		gambleCardsView1.CardChanged += GambleCardsView_CardChanged;
		gambleCardsView2.CardChanged += GambleCardsView_CardChanged;
		gambleCardsView3.CardChanged += GambleCardsView_CardChanged;
		gambleCardsView4.CardChanged += GambleCardsView_CardChanged;
	}

	public event EventHandler? CardChanged;

	private void GambleCardsView_CardChanged(object? sender, EventArgs e)
		=> CardChanged?.Invoke(this, EventArgs.Empty);
}
