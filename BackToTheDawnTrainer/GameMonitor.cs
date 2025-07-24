using BackToTheDawnTrainer.Resources;
using Microsoft.Extensions.Logging;
using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace BackToTheDawnTrainer;

internal class GameMonitor : IGameMonitor
{
	private readonly CancellationTokenSource _cts = new();

	private readonly ILogger<GameMonitor> _logger;

	public GameMonitor(ILogger<GameMonitor> logger)
	{
		_logger = logger;

		_ = StartMonitorLoop(_cts.Token);
	}

	public bool IsGameRunning => _gameProcess is not null;

	private Process? _gameProcess;

	public Process GameProcess => _gameProcess ?? throw new InvalidOperationException(Lang.Exception_GameNotRunning);

	public event EventHandler<GameMonitorEventArgs>? GameStarted;

	public event EventHandler? GameExited;

	private Task StartMonitorLoop(CancellationToken token)
		=> Task.Run(async () =>
		{
			_logger.LogInformation("Game monitor started.");

			while (!token.IsCancellationRequested)
			{
				try
				{
					if (_gameProcess is null)
					{
						_gameProcess ??= Process.GetProcessesByName("Back To The Dawn").FirstOrDefault();
						if (_gameProcess is not null)
						{
							_logger.LogInformation("Game launch detected. PID: {PID}.", _gameProcess.Id);
							GameStarted?.Invoke(this, new GameMonitorEventArgs(_gameProcess));
						}
					}
					else
					{
						if (_gameProcess.HasExited)
						{
							_logger.LogInformation("Game exit detected.");
							_gameProcess = null;
							GameExited?.Invoke(this, EventArgs.Empty);
						}
					}

					await Task.Delay(1000, token);
				}
				catch (OperationCanceledException)
				{
				}
				catch (Exception ex)
				{
					_logger.LogError(ex, "Error while checking game status.");
				}
			}
		}, token);

	#region Dispose

	private bool disposedValue;

	protected virtual void Dispose(bool disposing)
	{
		if (!disposedValue)
		{
			_cts.Cancel();

			if (disposing)
			{
				_cts.Dispose();
			}

			disposedValue = true;
		}
	}

	~GameMonitor()
	{
		Dispose(disposing: false);
	}

	public void Dispose()
	{
		Dispose(disposing: true);
		GC.SuppressFinalize(this);
	}

	#endregion
}
