using BackToTheDawnTrainer.ViewModels;
using System;
using System.Windows.Controls;

namespace BackToTheDawnTrainer.Views;

/// <summary>
/// GamblePlayerView.xaml 的交互逻辑
/// </summary>
public partial class GambleCardsView : UserControl
{
	public new GambleCardsViewModel DataContext
	{
		get => (GambleCardsViewModel)base.DataContext;
		set => base.DataContext = value;
	}

	public PokeCard[] Cards { get; }

	public GambleCardsView(
		GambleCardView gambleCardView1,
		GambleCardView gambleCardView2,
		GambleCardView gambleCardView3,
		GambleCardsViewModel viewModel)
	{
		DataContext = viewModel;
		InitializeComponent();
		_contentControl1.Content = gambleCardView1;
		_contentControl2.Content = gambleCardView2;
		_contentControl3.Content = gambleCardView3;
		Cards =
		[
			gambleCardView1.Card,
			gambleCardView2.Card,
			gambleCardView3.Card,
		];

		gambleCardView1.CardChanged += GambleCardView_CardChanged;
		gambleCardView2.CardChanged += GambleCardView_CardChanged;
		gambleCardView3.CardChanged += GambleCardView_CardChanged;
	}

	public event EventHandler? CardChanged;

	private void GambleCardView_CardChanged(object? sender, EventArgs e)
		=> CardChanged?.Invoke(this, EventArgs.Empty);
}
