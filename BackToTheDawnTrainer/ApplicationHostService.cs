using BackToTheDawnTrainer.Views;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using WPFLocalizeExtension.Engine;

namespace BackToTheDawnTrainer;

public class ApplicationHostService(IServiceProvider serviceProvider) : IHostedService
{
	public Task StartAsync(CancellationToken cancellationToken)
	{
		LocalizeDictionary.Instance.Culture = CultureInfo.CurrentUICulture;

		var window = serviceProvider.GetRequiredService<MainWindow>();
		App.Current.MainWindow = window;
		window.Show();

		return Task.CompletedTask;
	}

	public Task StopAsync(CancellationToken cancellationToken)
	{
		return Task.CompletedTask;
	}
}
