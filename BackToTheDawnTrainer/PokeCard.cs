using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Windows.Media;

namespace BackToTheDawnTrainer;

public partial class PokeCard : ObservableObject
{
	[ObservableProperty]
	public partial PokeSuit Suit { get; set; } = PokeSuit.Unknown;

	[ObservableProperty]
	public partial int Rank { get; set; } = 1;

	[ObservableProperty]
	public partial bool IsLocked { get; set; } = false;

	public int? OutgoingValue
	{
		get
		{
			if (!IsLocked || Suit == PokeSuit.Unknown)
			{
				return null;
			}

			return (int)Suit * 100 + Rank;
		}
	}

	public string Text
	{
		get
		{
			if (Suit == PokeSuit.Unknown || Rank < 1 || Rank > 13)
				return "🂠";

			var start = 0x1F0A0;
			var skip = Suit switch
			{
				PokeSuit.Spade => 0,
				PokeSuit.Heart => 1,
				PokeSuit.Diamond => 2,
				PokeSuit.Club => 3,
				_ => throw new ArgumentOutOfRangeException(nameof(Suit), Suit, null)
			};
			var index = start + skip * 0x10 + Rank;
			if (Rank >= 12)
			{
				++index;
			}
			return char.ConvertFromUtf32(index);
		}
	}

	public void SetValue(int? value)
	{
		if (value is null)
		{
			Suit = PokeSuit.Unknown;
			Rank = 1;
			return;
		}

		Suit = (PokeSuit)(value / 100);
		Rank = value.Value % 100;
	}

	private static readonly SolidColorBrush _blackBrush = new(Colors.Black);

	private static readonly SolidColorBrush _redBrush = new(Colors.Red);

	public SolidColorBrush Foreground
		=> Suit == PokeSuit.Unknown || Suit == PokeSuit.Spade || Suit == PokeSuit.Club ? _blackBrush : _redBrush;

	public static PokeCard Default { get; } = new();
}
