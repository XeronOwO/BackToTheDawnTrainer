using CommunityToolkit.Mvvm.ComponentModel;

namespace BackToTheDawnTrainer.ViewModels;

public partial class GambleCardsViewModel : ObservableObject
{
	private const string PokeCardDefault = "🂡";

	[ObservableProperty]
	public partial string Card1 { get; set; } = PokeCardDefault;

	[ObservableProperty]
	public partial string Card2 { get; set; } = PokeCardDefault;

	[ObservableProperty]
	public partial string Card3 { get; set; } = PokeCardDefault;
}
