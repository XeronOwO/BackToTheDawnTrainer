using System;
using System.Diagnostics;

namespace BackToTheDawnTrainer;

public interface IGameMonitor : IDisposable
{
	bool IsGameRunning { get; }

	Process GameProcess { get; }

	event EventHandler<GameMonitorEventArgs>? GameStarted;

	event EventHandler? GameExited;
}
