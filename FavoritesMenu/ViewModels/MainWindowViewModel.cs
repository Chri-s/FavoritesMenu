using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using FavoritesMenu.Views;
using Microsoft.Extensions.DependencyInjection;
using Wpf.Ui;
using Wpf.Ui.Controls;

namespace FavoritesMenu.ViewModels;

internal partial class MainWindowViewModel : ObservableObject
{
    private readonly INavigationService navigationService;
    private readonly IServiceProvider serviceProvider;
    private MainWindow? mainWindow;

    public INavigationViewItem SearchViewItem { get; init; } = new NavigationViewItem("Search", SymbolRegular.Search16, typeof(SearchPage));

    public INavigationViewItem SettingsViewItem { get; init; } = new NavigationViewItem("Settings", SymbolRegular.Settings24, typeof(SettingsPage));

    public INavigationViewItem AboutViewItem { get; init; } = new NavigationViewItem("About", SymbolRegular.Info12, typeof(AboutPage));

    [ObservableProperty]
    private string applicationTitle = "Favorites Menu";

    [ObservableProperty]
    private List<INavigationViewItem> navigationViewItems;

    [ObservableProperty]
    private INavigationViewItem selectedNavigationViewItem = null!;

    public MainWindowViewModel(INavigationService navigationService, IServiceProvider serviceProvider)
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
        if (this.mainWindow == null)
            this.mainWindow = this.serviceProvider.GetRequiredService<MainWindow>();

        this.mainWindow.Show();
    }

    partial void OnSelectedNavigationViewItemChanged(INavigationViewItem value)
    {
        if (value != null && this.navigationService.GetNavigationControl().SelectedItem != value)
            this.navigationService.Navigate(value.TargetPageType!);
    }
}
