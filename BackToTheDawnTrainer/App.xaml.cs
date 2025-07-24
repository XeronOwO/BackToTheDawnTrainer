using BackToTheDawnTrainer.Resources;
using BackToTheDawnTrainer.ViewModels;
using BackToTheDawnTrainer.Views;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Configuration;
using System;
using System.IO;
using System.Windows;

namespace BackToTheDawnTrainer;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
	public new static App Current => (App)Application.Current;

	private static readonly IHost _host = Host.CreateDefaultBuilder()
		.ConfigureAppConfiguration(configuration =>
		{
			configuration.SetBasePath(
				Path.GetDirectoryName(AppContext.BaseDirectory)
				?? throw new DirectoryNotFoundException(Lang.Exception_UnableToFindBaseDirectory)
				);
		})
		.ConfigureLogging(builder =>
		{
			builder.ClearProviders();
			builder.AddConfiguration();
			builder.Services.TryAddEnumerable(
				ServiceDescriptor.Singleton<ILoggerProvider, LoggerProvider>()
			);
			LoggerProviderOptions.RegisterProviderOptions<LoggerFilterOptions, LoggerProvider>(builder.Services);
		})
		.ConfigureServices(
			(context, services) =>
			{
				services.AddHostedService<ApplicationHostService>();

				services.AddSingleton<IGameMonitor, GameMonitor>();
				services.AddSingleton<IIl2CppLocator, Il2CppLocator>();
				services.AddSingleton<ILogHandlerProvider, LogHandlerProvider>();
				services.AddSingleton<ITrainer, Trainer>();
				services.AddSingleton<MainWindow>();

				services.AddTransient<DiceCheatView>();
				services.AddTransient<DiceCheatViewModel>();
				services.AddTransient<DiceValueView>();
				services.AddTransient<DiceValueViewModel>();
				services.AddTransient<GambleCardConfigureWindow>();
				services.AddTransient<GambleCardConfigureWindowViewModel>();
				services.AddTransient<GambleCardView>();
				services.AddTransient<GambleCardViewModel>();
				services.AddTransient<GambleCardsView>();
				services.AddTransient<GambleCardsViewModel>();
				services.AddTransient<GambleCheatView>();
				services.AddTransient<GambleCheatViewModel>();
				services.AddTransient<MainWindowViewModel>();
			}
		)
		.Build();

	public static IServiceProvider Services
		=> _host.Services;

	private async void OnStartup(object sender, StartupEventArgs e)
	{
		await _host.StartAsync();
	}

	private async void OnExit(object sender, ExitEventArgs e)
	{
		await _host.StopAsync();

		_host.Dispose();
	}
}
