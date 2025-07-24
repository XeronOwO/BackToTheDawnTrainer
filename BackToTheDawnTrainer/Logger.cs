using Microsoft.Extensions.Logging;
using System;
using System.Windows.Media;

namespace BackToTheDawnTrainer;

internal class Logger(
	string providerTypeFullName,
	string category,
	Func<LoggerFilterOptions> getCurrentConfig,
	ILogHandlerProvider handlerProvider
	) : ILogger
{
	public IDisposable? BeginScope<TState>(TState state) where TState : notnull
		=> default;

	public bool IsEnabled(LogLevel level)
	{
		var config = getCurrentConfig();
		foreach (var rule in config.Rules)
		{
			if (level < config.MinLevel)
			{
				continue;
			}

			if (rule.Filter != null)
			{
				if (rule.Filter(providerTypeFullName, category, level))
				{
					return true;
				}
				continue;
			}
		}

		return true;
	}

	public void Log<TState>(
		LogLevel logLevel,
		EventId eventId,
		TState state,
		Exception? exception,
		Func<TState, Exception?, string> formatter)
	{
		if (!IsEnabled(logLevel))
		{
			return;
		}

		if (handlerProvider.Handler is not { } handler)
		{
			return;
		}

		handler(GetColor(logLevel), LogFormatter.Format(
			category,
			DateTime.Now,
			logLevel,
			eventId,
			state,
			exception,
			formatter
		));
	}

	private static Color GetColor(LogLevel logLevel)
	{
		return logLevel switch
		{
			LogLevel.Trace => Colors.Gray,
			LogLevel.Debug => Colors.Blue,
			LogLevel.Information => Colors.Green,
			LogLevel.Warning => Colors.Yellow,
			LogLevel.Error => Colors.Red,
			LogLevel.Critical => Colors.Magenta,
			_ => Colors.Black
		};
	}
}
