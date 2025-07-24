using BackToTheDawnTrainer.Resources;
using BackToTheDawnTrainer.Views;
using Frida;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;

namespace BackToTheDawnTrainer;

internal class Trainer(ILogger<Trainer> logger, IGameMonitor monitor, IIl2CppLocator locator, IServiceProvider serviceProvider) : ITrainer
{
	private Script? _script;

	private Session? _session;

	private TaskCompletionSource? _tcsReady;

	public Task AttachAsync(CancellationToken token)
		=> Task.Run(async () =>
		{
			if (!monitor.IsGameRunning)
			{
				throw new InvalidOperationException(Lang.Exception_GameNotRunning);
			}

			logger.LogInformation("Loading frida device...");

			var deviceManager = new DeviceManager(serviceProvider.GetRequiredService<MainWindow>().Dispatcher);
			var device = deviceManager.EnumerateDevices().Where(d => d.Type == DeviceType.Local).FirstOrDefault()
				?? throw new InvalidOperationException(Lang.Trainer_NoLocalDeviceFound);
			var process = device.EnumerateProcesses().Where(p => p.Pid == monitor.GameProcess.Id).FirstOrDefault()
				?? throw new InvalidOperationException(string.Format(Lang.Trainer_ProcessWithPidNotFound, monitor.GameProcess.Id));

			logger.LogInformation("Attaching to game process...");
			_session = device.Attach(process.Pid);

			logger.LogInformation("Loading script...");
			var scriptPath = Path.Combine(Environment.CurrentDirectory, "trainer.js");
			var scriptContent = await File.ReadAllTextAsync(scriptPath);
			_script = _session.CreateScript(scriptContent, scriptPath);
			_script.Message += HandleMessage;
			_script.Load();

			_tcsReady = new();
			await _tcsReady.Task;
		}, token);

	private class IncomingPayloadEntity
	{
		[JsonPropertyName("type")]
		public string Type { get; set; } = null!;

		[JsonPropertyName("data")]
		public JsonNode? Data { get; set; }
	}

	private class OutgoingMessageEntity
	{
		[JsonPropertyName("type")]
		public required string Type { get; set; }
	}

	private class OutgoingMessageEntity<T> : OutgoingMessageEntity
		where T : notnull
	{
		[JsonPropertyName("payload")]
		public required T Payload { get; set; }
	}

	private void HandleMessage(object sender, ScriptMessageEventArgs e)
	{
		try
		{
			var message = (JsonObject)JsonNode.Parse(e.Message)!;
			switch (message["type"]!.GetValue<string>())
			{
				case "send":
					OnSend(message["payload"]!.Deserialize<IncomingPayloadEntity>()!);
					break;
				case "log":
					logger.LogInformation("[Frida] {Message}", message["payload"]!.ToString());
					break;
				case "error":
					logger.LogError($"[Frida] Error: {{Description}}\r\n{{Stack}}", message["description"]!.ToString(), message["stack"]!.ToString());
					break;
				default:
					break;
			}
		}
		catch (Exception ex)
		{
			logger.LogError(ex, "Error processing script message: {Message}", e.Message);
		}
	}

	private void OnSend(IncomingPayloadEntity payload)
	{
		switch (payload.Type)
		{
			case "Init":
				OnInit();
				break;
			case "Ready":
				OnReady();
				break;
			case "GambleCardValues":
				OnGambleCardValues(payload.Data);
				break;
			default:
				break;
		}
	}

	private void OnInit()
	{
		logger.LogInformation("Script is initialized.");

		_script?.Post(JsonSerializer.Serialize(new OutgoingMessageEntity<Dictionary<string, long>>
		{
			Type = "MethodRVAs",
			Payload = locator.MethodRVAs,
		}));
	}

	private void OnReady()
	{
		logger.LogInformation("Script is ready.");

		_tcsReady?.TrySetResult();
		_tcsReady = null;
	}

	public event EventHandler<GambleEventArgs>? GambleCardsReceived;

	private void OnGambleCardValues(JsonNode? node)
	{
		if (node is null)
		{
			return;
		}

		var values = node.Deserialize<int[]>() ?? [];
		GambleCardsReceived?.Invoke(this, new GambleEventArgs
		{
			Values = values,
		});
	}

	public void Detach()
	{
		logger.LogInformation("Detaching from game process...");

		_script?.Post(JsonSerializer.Serialize(new OutgoingMessageEntity
		{
			Type = "Exit",
		}));

		if (monitor.IsGameRunning)
		{
			_script?.Unload();
			_session?.Detach();

			logger.LogInformation("Trainer detached.");
		}
		else
		{
			logger.LogInformation("Game process exited. Skipped detaching.");
		}

		_script?.Dispose();
		_script = null;
		_session?.Dispose();
		_session = null;
	}

	public void SetDiceValues(int?[] values)
	{
		_script?.Post(JsonSerializer.Serialize(new OutgoingMessageEntity<int?[]>
		{
			Type = "DiceCheatValues",
			Payload = values,
		}));
	}

	public void SetGambleValues(int?[] values)
	{
		_script?.Post(JsonSerializer.Serialize(new OutgoingMessageEntity<int?[]>
		{
			Type = "GambleCheatValues",
			Payload = values,
		}));
	}
}
