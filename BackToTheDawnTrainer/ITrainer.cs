using System;
using System.Threading;
using System.Threading.Tasks;

namespace BackToTheDawnTrainer;

public interface ITrainer
{
	event EventHandler<GambleEventArgs>? GambleCardsReceived;

	Task AttachAsync(CancellationToken token);

	void Detach();

	void SetDiceValues(int?[] values);

	void SetGambleValues(int?[] values);
}
