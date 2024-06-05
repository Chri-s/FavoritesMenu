using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CommunityToolkit.Mvvm.ComponentModel;
using FavoritesMenu.Services;
using FavoritesMenu.Views;
using Microsoft.Extensions.DependencyInjection;
using Windows.Win32;
using Wpf.Ui;
using Wpf.Ui.Controls;

namespace FavoritesMenu.ViewModels;

internal partial class MainWindowViewModel : ObservableObject
{
    private readonly Services.NavigationService navigationService;
    private readonly IServiceProvider serviceProvider;
    private IMainWindow? mainWindow;

    public INavigationViewItem SearchViewItem { get; init; } = new NavigationViewItem("Search", SymbolRegular.Search16, typeof(ISearchPage));

    public INavigationViewItem SettingsViewItem { get; init; } = new NavigationViewItem("Settings", SymbolRegular.Settings24, typeof(SettingsPage));

    public INavigationViewItem AboutViewItem { get; init; } = new NavigationViewItem("About", SymbolRegular.Info12, typeof(AboutPage));

    [ObservableProperty]
    private string applicationTitle = "Favorites Menu";

    [ObservableProperty]
    private List<INavigationViewItem> navigationViewItems;

    [ObservableProperty]
    private INavigationViewItem selectedNavigationViewItem = null!;

    public MainWindowViewModel(Services.NavigationService navigationService, IServiceProvider serviceProvider)
    {
        this.navigationService = navigationService;
        this.serviceProvider = serviceProvider;

        this.navigationViewItems = new List<INavigationViewItem>()
        {
            this.SearchViewItem,
            this.SettingsViewItem,
            this.AboutViewItem,
        };
    }

    public void ShowMainWindow()
    {
        this.MainWindow.Show();
        PInvoke.SetForegroundWindow(new Windows.Win32.Foundation.HWND(this.MainWindow.Handle));
    }

    public void HideMainWindow() => this.MainWindow.Hide();

    public void ActivateMainWindow()
    {
        this.MainWindow.Activate();
        PInvoke.SetForegroundWindow(new Windows.Win32.Foundation.HWND(this.MainWindow.Handle));
    }

    private IMainWindow MainWindow
    {
        get
        {
            if (this.mainWindow == null)
                this.mainWindow = this.serviceProvider.GetRequiredService<IMainWindow>();

            return this.mainWindow;
        }
    }

    public bool IsMainWindowShown { get => this.MainWindow.Visibility == System.Windows.Visibility.Visible; }

    partial void OnSelectedNavigationViewItemChanged(INavigationViewItem value)
    {
        // The NavigationView crashes if we navigate to a page and the MainWindow has not yet been shown. 
        if (!this.IsMainWindowShown)
            this.MainWindow.Show();

        if (value != null && this.navigationService.GetNavigationControl().SelectedItem != value)
            this.navigationService.Navigate(value.TargetPageType!);
    }
}
