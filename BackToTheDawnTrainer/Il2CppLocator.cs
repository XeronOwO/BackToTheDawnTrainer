using BackToTheDawnTrainer.Resources;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace BackToTheDawnTrainer;

internal class Il2CppLocator(ILogger<Il2CppLocator> logger, IGameMonitor monitor) : IIl2CppLocator
{
	private const string DumpDirectoryName = "Dump";

	private static readonly string _dumpDirectory = Path.Combine(Environment.CurrentDirectory, DumpDirectoryName);

	private static readonly string _scriptPath = Path.Combine(_dumpDirectory, "script.json");

	public bool CheckRequiresDump(DateTime? oldTime, [NotNullWhen(true)] out DateTime? newTime)
	{
		var gameDirectory = Path.GetDirectoryName(monitor.GameProcess.MainModule!.FileName)!;
		var il2CppPath = Path.Combine(gameDirectory, "GameAssembly.dll");

		var currentTime = File.GetLastWriteTime(il2CppPath);
		if (oldTime is null)
		{
			newTime = currentTime;
			return true;
		}

		if (currentTime > oldTime + TimeSpan.FromSeconds(1))
		{
			newTime = currentTime;
			return true;
		}

		if (!File.Exists(_scriptPath))
		{
			newTime = currentTime;
			return true;
		}

		newTime = null;
		return false;
	}

	private static string EscapeArguments(params string[] args)
	{
		if (args == null || args.Length == 0)
		{
			return "";
		}

		var stringBuilder = new StringBuilder();

		for (int i = 0; i < args.Length; i++)
		{
			if (i > 0)
			{
				stringBuilder.Append(' ');
			}

			var arg = args[i];

			if (arg.Contains(' ') || arg == "")
			{
				stringBuilder.Append('"').Append(arg).Append('"');
			}
			else
			{
				stringBuilder.Append(arg);
			}
		}

		return stringBuilder.ToString();
	}

	public async Task DumpAsync(CancellationToken token)
	{
		const string dumperFileName = $"{nameof(Il2CppDumper)}.exe";

		var dumperExecutablePath = Path.Combine(Environment.CurrentDirectory, dumperFileName);
		var gameDirectory = Path.GetDirectoryName(monitor.GameProcess.MainModule!.FileName)!;
		var il2CppPath = Path.Combine(gameDirectory, "GameAssembly.dll");
		var metadataPath = Path.Combine(gameDirectory, "Back To The Dawn_Data", "il2cpp_data", "Metadata", "global-metadata.dat");
		Directory.CreateDirectory(_dumpDirectory);

		logger.LogInformation($"Starting process {dumperFileName}...");
		using var process = new Process
		{
			StartInfo = new ProcessStartInfo
			{
				FileName = dumperExecutablePath,
				Arguments = EscapeArguments(il2CppPath, metadataPath, _dumpDirectory),
				CreateNoWindow = true,
				UseShellExecute = false,
				RedirectStandardOutput = true,
				RedirectStandardError = true,
				WorkingDirectory = Environment.CurrentDirectory,
			},
		};
		process.Start();
		logger.LogInformation($"Process {dumperFileName} started with PID: {{PID}}.", process.Id);
		_ = StartProcessOutputLoop(token);

		logger.LogInformation("Waiting for process to exit...");
		await process.WaitForExitAsync(token);
		logger.LogInformation($"Process {dumperFileName} exited with code: {{ExitCode}}.", process.ExitCode);
		if (process.ExitCode != 0)
		{
			throw new InvalidOperationException(string.Format(Lang.Exception_DumperExitedNonZero, process.ExitCode));
		}

		Task StartProcessOutputLoop(CancellationToken token)
			=> Task.Run(async () =>
			{
				while (!token.IsCancellationRequested)
				{
					try
					{
						var line = await process.StandardOutput.ReadLineAsync();
						if (line == null)
						{
							break;
						}
						logger.LogInformation($"[{nameof(Il2CppDumper)}.exe] {{Line}}", line);
					}
					catch (Exception ex)
					{
						logger.LogError(ex, $"Error reading standard output of {nameof(Il2CppDumper)}.");
					}
				}
			}, token);
	}

	private const string RandomTwoDiceValueMethodSignature = "void ThrowDiceRangePoint___RandomTwoDiceValue (int32_t* dice1Value, int32_t* dice2value, bool isChallengeDice, const MethodInfo* method);";

	private const string RandomOneIntValueMethodSignature = "int32_t CommonManage__RandomOneIntValue (int32_t minValue, int32_t maxValue, const MethodInfo* method);";

	private const string SendGamblerCardMethodSignature = "void GambleRound__SendGamblerCard (System_Collections_Generic_List_Gambler__o* specifyGamblerList, const MethodInfo* method);";

	private const string RandomOneIntFromListMethodSignature = "int32_t CommonManage__RandomOneIntFromList (System_Collections_Generic_List_int__o* list, const MethodInfo* method);";

	private static readonly string[] _methodSignatures =
	[
		RandomTwoDiceValueMethodSignature,
		RandomOneIntValueMethodSignature,
		SendGamblerCardMethodSignature,
		RandomOneIntFromListMethodSignature,
	];

	public Dictionary<string, long> MethodRVAs { get; } = [];

	public async Task LocateAsync(CancellationToken token)
	{
		logger.LogInformation("Reading script.json...");
		using var file = File.OpenRead(_scriptPath);
		var info = await JsonSerializer.DeserializeAsync<Il2CppInfo>(file, cancellationToken: token);

		MethodRVAs.Clear();
		foreach (var method in info.ScriptMethod)
		{
			if (_methodSignatures.Contains(method.Signature))
			{
				MethodRVAs[method.Signature] = method.Address;
				logger.LogInformation("Found method: {Signature} at RVA: {RVA}", method.Signature, method.Address.ToString("X"));
			}
		}

		if (MethodRVAs.Count != _methodSignatures.Length)
		{
			foreach (var signature in _methodSignatures)
			{
				if (!MethodRVAs.ContainsKey(signature))
				{
					logger.LogWarning("Method signature not found: {Signature}", signature);
				}
			}
			logger.LogWarning("Some method signatures were not found in the script.json file. " +
				"Some functions may not work correctly. ");
		}
	}
}
