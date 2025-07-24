using System;
using System.Diagnostics;

namespace BackToTheDawnTrainer;

public class GameMonitorEventArgs(Process process) : EventArgs
{
	public Process Process { get; } = process;
}
