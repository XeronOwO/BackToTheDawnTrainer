using System;

namespace BackToTheDawnTrainer;

public enum DiceType
{
	Random,
	One,
	Two,
	Three,
	Four,
	Five,
	Six,
}

public static class DiceTypeExtensions
{
	public static int? GetValue(this DiceType type)
	{
		return type switch
		{
			DiceType.Random => null,
			DiceType.One => 1,
			DiceType.Two => 2,
			DiceType.Three => 3,
			DiceType.Four => 4,
			DiceType.Five => 5,
			DiceType.Six => 6,
			_ => throw new ArgumentOutOfRangeException(nameof(type), type, null),
		};
	}
}
