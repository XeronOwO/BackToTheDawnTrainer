using CommunityToolkit.Mvvm.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace BackToTheDawnTrainer.ViewModels;

public partial class DiceValueViewModel : ObservableValidator
{
	[ObservableProperty]
	public partial string GroupName { get; set; } = string.Empty;

	[Range(0, 6, ErrorMessage = "Dice value must be between 0 and 6.")]
	[ObservableProperty]
	public partial DiceType Value { get; set; } = DiceType.Random;
}
