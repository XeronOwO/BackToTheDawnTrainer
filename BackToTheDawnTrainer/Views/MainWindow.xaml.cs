using BackToTheDawnTrainer.Resources;
using BackToTheDawnTrainer.ViewModels;
using Microsoft.Extensions.Logging;
using System;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;

namespace BackToTheDawnTrainer.Views;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
	private readonly ILogger<MainWindow> _logger;

	private readonly ILogHandlerProvider _logHandlerProvider;

	private readonly IGameMonitor _monitor;

	private readonly IIl2CppLocator _locator;

	private readonly ITrainer _trainer;

	private readonly DiceCheatView _diceCheatView;

	private readonly GambleCheatView _gambleCheatView;

	private readonly CancellationTokenSource _cts = new();

	public new MainWindowViewModel DataContext
	{
		get => (MainWindowViewModel)base.DataContext;
		set => base.DataContext = value;
	}

	public MainWindow(
		ILogger<MainWindow> logger,
		ILogHandlerProvider logHandlerProvider,
		IGameMonitor monitor,
		IIl2CppLocator locator,
		ITrainer trainer,
		DiceCheatView diceCheatView,
		GambleCheatView gambleCheatView,
		MainWindowViewModel viewModel)
	{
		_logger = logger;
		_logHandlerProvider = logHandlerProvider;
		_monitor = monitor;
		_locator = locator;
		_trainer = trainer;
		DataContext = viewModel;
		InitializeComponent();
		_contentControlDiceCheat.Content = _diceCheatView = diceCheatView;
		_contentControlGambleCheat.Content = _gambleCheatView = gambleCheatView;

		try
		{
			var restoreBounds = Settings.Default.MainWindow_RestoreBounds;
			Left = restoreBounds.Left;
			Top = restoreBounds.Top;
			Width = restoreBounds.Width;
			Height = restoreBounds.Height;
			WindowState = Settings.Default.MainWindow_WindowState;
			_grid.RowDefinitions[0].Height = Settings.Default.MainWindow_Splitter_UpRowHeight;
			_grid.RowDefinitions[2].Height = Settings.Default.MainWindow_Splitter_DownRowHeight;
			_diceCheatView.Dice1 = Settings.Default.Dice1Value;
			_diceCheatView.Dice2 = Settings.Default.Dice2Value;
			for (int i = 0; i < Settings.Default.GambleValues.Length; i++)
			{
				var value = Settings.Default.GambleValues[i];
				_gambleCheatView.Cards[i].IsLocked = value.IsLocked;
				_gambleCheatView.Cards[i].Suit = value.Suit;
				_gambleCheatView.Cards[i].Rank = value.Rank;
			}
		}
		catch
		{
		}

		_monitor.GameStarted += Monitor_GameStarted;
		_monitor.GameExited += Monitor_GameExited;
		_diceCheatView.ValuesChanged += DiceCheatView_ValuesChanged;
		_gambleCheatView.CardChanged += GambleCheatView_CardChanged;
		_trainer.GambleCardsReceived += Trainer_GambleCardsReceived;
	}

	private bool _isScriptInitialized = false;

	private void MainWindow_Initialized(object? sender, EventArgs e)
	{
		_richTextBoxLog.Document.Blocks.Clear();
		_logHandlerProvider.Handler = OnLog;

		_logger.LogInformation("MainWindow initialized.");

		if (_monitor.IsGameRunning)
		{
			_logger.LogInformation("Game detected. PID: {PID}.", _monitor.GameProcess.Id);
			_buttonEnable.IsEnabled = true;
			_labelGameNotRunning.Visibility = Visibility.Collapsed;
			_labelGameRunning.Visibility = Visibility.Visible;
		}
	}

	private void MainWindow_Closing(object? sender, CancelEventArgs e)
	{
		if (_isTrainerEnabled)
		{
			if (MessageBoxResult.No == MessageBox.Show(this, Lang.MainWindow_ClosingConfirm_Text, Lang.MainWindow_ClosingConfirm_Caption, MessageBoxButton.YesNo, MessageBoxImage.Question))
			{
				e.Cancel = true;
				return;
			}

			OnDisableTrainer();
		}

		_cts.Cancel();
		_logHandlerProvider.Handler = null;

		Settings.Default.MainWindow_RestoreBounds = RestoreBounds;
		Settings.Default.MainWindow_WindowState = WindowState;
		Settings.Default.MainWindow_Splitter_UpRowHeight = _grid.RowDefinitions[0].Height;
		Settings.Default.MainWindow_Splitter_DownRowHeight = _grid.RowDefinitions[2].Height;
		Settings.Default.Dice1Value = _diceCheatView.Dice1;
		Settings.Default.Dice2Value = _diceCheatView.Dice2;
		Settings.Default.GambleValues = [.. _gambleCheatView.Cards.Select(c => new PokeCardConfig
		{
			IsLocked = c.IsLocked,
			Suit = c.Suit,
			Rank = c.Rank
		})];
		Settings.Default.Save();
	}

	private void OnLog(Color color, string message)
	{
		if (!_richTextBoxLog.CheckAccess())
		{
			Dispatcher.Invoke(() => OnLog(color, message));
			return;
		}

		if (_richTextBoxLog.Document.Blocks.Count > 100)
		{
			_richTextBoxLog.Document.Blocks.Remove(_richTextBoxLog.Document.Blocks.FirstBlock);
		}

		var paragraph = new Paragraph
		{
			Inlines =
			{
				new Run(message)
				{
					Foreground = new SolidColorBrush(Color.FromArgb(color.A, color.R, color.G, color.B))
				}
			},
			LineHeight = 1.5,
		};
		_richTextBoxLog.Document.Blocks.Add(paragraph);

		_richTextBoxLog.ScrollToEnd();
	}

	private async void ButtonEnable_Click(object sender, RoutedEventArgs e)
		=> await OnEnableTrainerAsync(_cts.Token);

	private void ButtonDisable_Click(object sender, RoutedEventArgs e)
		=> OnDisableTrainer();

	private bool _isTrainerEnabled = false;

	private async Task OnEnableTrainerAsync(CancellationToken token)
	{
		if (!CheckAccess())
		{
			Dispatcher.Invoke(OnEnableTrainerAsync);
			return;
		}

		try
		{
			_logger.LogInformation("Enabling trainer...");
			_buttonEnable.IsEnabled = false;

			DateTime? recordedTime;
			try
			{
				recordedTime = Settings.Default.GameFileLastWriteTime;
			}
			catch
			{
				recordedTime = null;
			}

			if (_locator.CheckRequiresDump(recordedTime, out var newTime))
			{
				_logger.LogInformation("Dump file not exists or game updated. Dumping...");

				await _locator.DumpAsync(token);
				Settings.Default.GameFileLastWriteTime = newTime.Value;
				Settings.Default.Save();
			}
			await _locator.LocateAsync(token);

			await _trainer.AttachAsync(token);
			_isScriptInitialized = true;
			SetDiceCheat(_diceCheatView.Dice1, _diceCheatView.Dice2);
			SetGambleCheat(_gambleCheatView.Cards);

			_isTrainerEnabled = true;
			_tableControlCheating.IsEnabled = true;
			_buttonDisable.IsEnabled = true;
			_logger.LogInformation("Trainer enabled.");
		}
		catch (OperationCanceledException)
		{
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, "Failed to enable trainer.");
			_buttonEnable.IsEnabled = true;
		}
	}

	private void OnDisableTrainer()
	{
		if (!CheckAccess())
		{
			Dispatcher.Invoke(OnDisableTrainer);
			return;
		}

		try
		{
			_logger.LogInformation("Disabling trainer...");
			_buttonDisable.IsEnabled = false;

			_trainer.Detach();

			_isTrainerEnabled = false;
			_logger.LogInformation("Trainer disabled.");
			_tableControlCheating.IsEnabled = false;
			_buttonEnable.IsEnabled = _monitor.IsGameRunning;
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, "Failed to disable trainer.");
			_buttonDisable.IsEnabled = true;
		}
	}

	private void Monitor_GameStarted(object? sender, GameMonitorEventArgs e)
		=> Dispatcher.Invoke(() =>
		{
			_buttonEnable.IsEnabled = true;
			_labelGameNotRunning.Visibility = Visibility.Collapsed;
			_labelGameRunning.Visibility = Visibility.Visible;
		});

	private void Monitor_GameExited(object? sender, EventArgs e)
		=> Dispatcher.Invoke(() =>
		{
			OnDisableTrainer();
			_labelGameNotRunning.Visibility = Visibility.Visible;
			_labelGameRunning.Visibility = Visibility.Collapsed;
		});

	private void DiceCheatView_ValuesChanged(object? sender, DiceCheatView.DiceEventArgs e)
		=> SetDiceCheat(e.Dice1, e.Dice2);

	private void SetDiceCheat(DiceType dice1, DiceType dice2)
	{
		if (!_isScriptInitialized)
		{
			return;
		}

		var value1 = dice1.GetValue();
		var value2 = dice2.GetValue();
		_logger.LogInformation("Setting dice values: {Value1}, {Value2}.", value1, value2);
		_trainer.SetDiceValues([value1, value2]);
	}

	private void GambleCheatView_CardChanged(object? sender, EventArgs e)
		=> SetGambleCheat(_gambleCheatView.Cards);

	private void SetGambleCheat(PokeCard[] cards)
	{
		if (!_isScriptInitialized)
		{
			return;
		}

		int?[] values = [.. cards.Select(c => c.OutgoingValue)];
		_logger.LogInformation("Setting gamble values: {Values}.", string.Join(", ", values));
		_trainer.SetGambleValues(values);
	}

	private void Trainer_GambleCardsReceived(object? sender, GambleEventArgs e)
	{
		_logger.LogInformation("Gamble card values: {Values}.", string.Join(", ", e.Values));

		for (int i = 0; i < _gambleCheatView.Cards.Length; i++)
		{
			var card = _gambleCheatView.Cards[i];
			if (i < e.Values.Length)
			{
				card.SetValue(e.Values[i]);
			}
			else
			{
				card.SetValue(null);
			}
		}
	}
}