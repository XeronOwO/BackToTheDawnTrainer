using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Concurrent;

namespace BackToTheDawnTrainer;

internal class LoggerProvider : ILoggerProvider
{
	private static readonly string _providerTypeFullName = typeof(LoggerProvider).FullName!;

	private readonly IDisposable? _onChangeToken;

	private LoggerFilterOptions _currentConfig;

	private readonly ConcurrentDictionary<string, Logger> _loggers =
		new(StringComparer.OrdinalIgnoreCase);

	private readonly ILogHandlerProvider _handlerProvider;

	public LoggerProvider(
		IOptionsMonitor<LoggerFilterOptions> config,
		ILogHandlerProvider executor)
	{
		_currentConfig = config.CurrentValue;
		_onChangeToken = config.OnChange(updatedConfig => _currentConfig = updatedConfig);
		_handlerProvider = executor;
	}

	public ILogger CreateLogger(string categoryName)
		=> _loggers.GetOrAdd(
			categoryName,
			name => new Logger(_providerTypeFullName, name, GetCurrentConfig, _handlerProvider)
			);

	private LoggerFilterOptions GetCurrentConfig() => _currentConfig;

	#region Dispose

	private bool disposedValue;

	protected virtual void Dispose(bool disposing)
	{
		if (!disposedValue)
		{
			if (disposing)
			{
				_onChangeToken?.Dispose();
			}

			disposedValue = true;
		}
	}

	~LoggerProvider()
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
