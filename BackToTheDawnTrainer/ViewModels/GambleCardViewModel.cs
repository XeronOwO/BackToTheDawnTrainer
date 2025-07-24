using CommunityToolkit.Mvvm.ComponentModel;
using System.Windows.Media;

namespace BackToTheDawnTrainer.ViewModels;

public partial class GambleCardViewModel : ObservableObject
{
	[ObservableProperty]
	public partial string Text { get; set; } = PokeCard.Default.Text;

	[ObservableProperty]
	public partial SolidColorBrush Foreground { get; set; } = PokeCard.Default.Foreground;

	[ObservableProperty]
	public partial bool IsLocked { get; set; } = false;
}
