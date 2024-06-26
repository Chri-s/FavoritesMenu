﻿using System.Windows;
using System.Windows.Controls;
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
                services.AddSingleton<ISearchPage, SearchPage>();
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
                services.AddSingleton<Services.NavigationService>();
                services.AddSingleton<ItemDataService>();
                services.AddSingleton<HotkeyService>();
            }).Build();
    }

    protected override void OnStartup(StartupEventArgs e)
    {
        this.host.Start();

        Wpf.Ui.Appearance.ApplicationThemeManager.Changed += ApplicationThemeManager_Changed;

        this.taskbarIcon = (H.NotifyIcon.TaskbarIcon)FindResource("TaskbarIcon");

        NotifyIconViewModel notifyVm = this.host.Services.GetRequiredService<NotifyIconViewModel>();
        this.taskbarIcon.DataContext = notifyVm;

        ItemDataService itemDataService = this.host.Services.GetRequiredService<ItemDataService>();

        try
        {
            itemDataService.UpdateItems(SettingsViewModel.GetToolbarPath());
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Could not open the toolbar path \"{SettingsViewModel.GetToolbarPath()}\": {ex.Message}\r\n\r\nPlease select a new path in the settings.", "Favorites menu", MessageBoxButton.OK, MessageBoxImage.Error);
            this.host.Services.GetRequiredService<NotifyIconViewModel>().ShowSettingsCommand.Execute(null);
        }

        this.host.Services.GetRequiredService<SettingsViewModel>().InitHotkeys();

        this.taskbarIcon.ForceCreate();
    }

    private void ApplicationThemeManager_Changed(Wpf.Ui.Appearance.ApplicationTheme currentApplicationTheme, Color systemAccent)
    {
        ContextMenu contextMenu = this.taskbarIcon.ContextMenu;
        Wpf.Ui.Appearance.ApplicationThemeManager.Apply(contextMenu);

        contextMenu = Behaviors.TaskbarIconBehaviors.GetTrayRightClickContextMenu(this.taskbarIcon);
        Wpf.Ui.Appearance.ApplicationThemeManager.Apply(contextMenu);
    }

    protected override void OnExit(ExitEventArgs e)
    {
        using (this.host)
            host.StopAsync().Wait();

        base.OnExit(e);
    }
}

