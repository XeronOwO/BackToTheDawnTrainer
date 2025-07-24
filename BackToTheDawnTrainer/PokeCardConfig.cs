namespace BackToTheDawnTrainer;

public class PokeCardConfig
{
	public bool IsLocked { get; set; } = false;

	public PokeSuit Suit { get; set; } = PokeSuit.Unknown;

	public int Rank { get; set; } = 1;
}
