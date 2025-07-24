using System;

namespace BackToTheDawnTrainer;

public class GambleEventArgs : EventArgs
{
	public required int[] Values { get; init; }
}
