using System.Windows;
using System.Windows.Media;
using FavoritesMenu.Services;
using FavoritesMenu.ViewModels;
using FavoritesMenu.Views;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Wpf.Ui;

namespace FavoritesMenu;

public partial class App : Application
{
    private H.NotifyIcon.TaskbarIcon taskbarIcon = null!;

    private IHost host;

    public App()
    {
        this.host = new HostBuilder()
            .ConfigureServices((hostContext, services) =>
            {
                // Register UI
                services.AddSingleton<IMainWindow, MainWindow>();
                services.AddSingleton<AboutPage>();
                services.AddSingleton<SearchPage>();
                services.AddSingleton<SettingsPage>();

                // Register View Models
                services.AddSingleton<MainWindowViewModel>();
                services.AddSingleton<AboutViewModel>();
                services.AddSingleton<SettingsViewModel>();
                services.AddSingleton<SearchViewModel>();
                services.AddSingleton<SearchItemViewModel>();
                services.AddSingleton<NotifyIconViewModel>();

                // Services
                services.AddSingleton<IPageService, PageService>();
                services.AddSingleton<INavigationService, Services.NavigationService>();
                services.AddSingleton<ItemDataService>();
                services.AddSingleton<HotkeyService>();
            }).Build();
    }

    protected override void OnStartup(StartupEventArgs e)
    {
        this.host.Start();

        this.taskbarIcon = (H.NotifyIcon.TaskbarIcon)FindResource("TaskbarIcon");

        NotifyIconViewModel notifyVm = this.host.Services.GetRequiredService<NotifyIconViewModel>();
        this.taskbarIcon.DataContext = notifyVm;

        ItemDataService itemDataService = this.host.Services.GetRequiredService<ItemDataService>();

        itemDataService.UpdateItems(SettingsViewModel.GetToolbarPath());

        this.host.Services.GetRequiredService<SettingsViewModel>().InitHotkeys();

        this.taskbarIcon.ForceCreate();
    }

    protected override void OnExit(ExitEventArgs e)
    {
        using (this.host)
            host.StopAsync().Wait();

        base.OnExit(e);
    }
}

