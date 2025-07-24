using Microsoft.Extensions.Logging;
using System;
using System.Text;

#pragma warning disable IDE0079
#pragma warning disable IDE0060

namespace BackToTheDawnTrainer;

internal static class LogFormatter
{
	public static string Format<TState>(
		string name,
		DateTime time,
		LogLevel logLevel,
		EventId eventId,
		TState state,
		Exception? exception,
		Func<TState, Exception?, string> formatter)
		=> Format(name, time, logLevel, eventId, exception, formatter(state, exception));

	private static string Format(
		string name,
		DateTime time,
		LogLevel logLevel,
		EventId eventId,
		Exception? exception,
		string message)
	{
		var sb = new StringBuilder();
		var logLevelString = logLevel.GetString();
		var indent = new string(' ', logLevelString.Length + 2);
		sb.AppendLine($"{logLevelString}: {name}[{eventId.Id}] @ {time}");
		sb.Append($"{indent}{message}");
		if (exception is not null)
		{
			sb.AppendLine();
			sb.Append(exception.ToString());
		}

		return sb.ToString();
	}

	private static string GetString(this LogLevel logLevel) => logLevel switch
	{
		LogLevel.Trace => "[TRACE]",
		LogLevel.Debug => "[DEBUG]",
		LogLevel.Information => "[INFO]",
		LogLevel.Warning => "[WARN]",
		LogLevel.Error => "[ERROR]",
		LogLevel.Critical => "[FATAL]",
		_ => throw new ArgumentOutOfRangeException(nameof(logLevel))
	};
}
