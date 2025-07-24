using BackToTheDawnTrainer.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;

namespace BackToTheDawnTrainer.Views;

/// <summary>
/// GambleCardView.xaml 的交互逻辑
/// </summary>
public partial class GambleCardView : UserControl
{
	private readonly IServiceProvider _serviceProvider;

	public new GambleCardViewModel DataContext
	{
		get => (GambleCardViewModel)base.DataContext;
		set => base.DataContext = value;
	}

	public PokeCard Card { get; } = new();

	public GambleCardView(IServiceProvider serviceProvider, GambleCardViewModel viewModel)
	{
		_serviceProvider = serviceProvider;
		DataContext = viewModel;
		InitializeComponent();

		Card.PropertyChanged += Card_PropertyChanged;
	}

	private void Card_PropertyChanged(object? sender, PropertyChangedEventArgs e)
	{
		DataContext.IsLocked = Card.IsLocked;
		DataContext.Text = Card.Text;
		DataContext.Foreground = Card.Foreground;
	}

	public event EventHandler? CardChanged;

	private void Button_Click(object sender, RoutedEventArgs e)
	{
		var window = _serviceProvider.GetRequiredService<GambleCardConfigureWindow>();
		window.Card = Card;
		if (!window.ShowDialog(Window.GetWindow(this)))
		{
			return;
		}

		Card.IsLocked = window.IsLocked;
		Card.Suit = window.Suit;
		Card.Rank = window.Rank;

		CardChanged?.Invoke(this, EventArgs.Empty);
	}
}
