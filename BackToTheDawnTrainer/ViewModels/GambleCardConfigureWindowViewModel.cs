using CommunityToolkit.Mvvm.ComponentModel;

namespace BackToTheDawnTrainer.ViewModels;

public partial class GambleCardConfigureWindowViewModel : ObservableObject
{
	[ObservableProperty]
	public partial bool IsLocked { get; set; } = false;

	[ObservableProperty]
	public partial PokeSuit Suit { get; set; } = PokeSuit.Spade;

	[ObservableProperty]
	public partial int Rank { get; set; } = 1;
}
