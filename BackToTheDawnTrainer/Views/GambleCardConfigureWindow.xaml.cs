using BackToTheDawnTrainer.Resources;
using BackToTheDawnTrainer.ViewModels;
using System.Windows;

namespace BackToTheDawnTrainer.Views;

/// <summary>
/// GambleCardConfigureWindow.xaml 的交互逻辑
/// </summary>
public partial class GambleCardConfigureWindow : Window
{
	public new GambleCardConfigureWindowViewModel DataContext
	{
		get => (GambleCardConfigureWindowViewModel)base.DataContext;
		set => base.DataContext = value;
	}

	public GambleCardConfigureWindow(GambleCardConfigureWindowViewModel viewModel)
	{
		DataContext = viewModel;
		InitializeComponent();
	}

	public bool IsLocked => DataContext.IsLocked;

	public PokeSuit Suit => DataContext.Suit;

	public int Rank => DataContext.Rank;

	private bool _result = false;

	public PokeCard? Card { get; set; }

	public bool ShowDialog(Window owner)
	{
		Owner = owner;
		if (Card != null)
		{
			DataContext.IsLocked = Card.IsLocked;
			DataContext.Suit = Card.Suit != PokeSuit.Unknown ? Card.Suit : PokeSuit.Spade;
			DataContext.Rank = Card.Rank;
		}

		ShowDialog();
		return _result;
	}

	private void ButtonConfirm_Click(object sender, RoutedEventArgs e)
	{
		if (Suit == PokeSuit.Unknown)
		{
			MessageBox.Show(this, Lang.GambleCardConfigureWindow_SuitNotSelected_Text, Lang.GambleCardConfigureWindow_SuitNotSelected_Caption, MessageBoxButton.OK, MessageBoxImage.Warning);
			return;
		}

		_result = true;
		Close();
	}

	private void ButtonCancel_Click(object sender, RoutedEventArgs e)
	{
		_result = false;
		Close();
	}
}
