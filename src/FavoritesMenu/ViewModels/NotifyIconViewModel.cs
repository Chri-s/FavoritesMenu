using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FavoritesMenu.Services;
using Windows.Win32.Foundation;


namespace FavoritesMenu.ViewModels;

internal partial class NotifyIconViewModel : ObservableObject
{
    private readonly MainWindowViewModel mainWindowViewModel;
    private readonly ItemDataService itemDataService;
    private readonly SearchItemViewModel searchVm;
    private readonly IMainWindow mainWindow;

    public NotifyIconViewModel(MainWindowViewModel mainWindowViewModel, ItemDataService itemDataService, SearchItemViewModel searchItemViewModel, HotkeyService hotkeyService, IMainWindow mainWindow)
    {
        this.mainWindow = mainWindow;
        this.mainWindowViewModel = mainWindowViewModel;
        this.itemDataService = itemDataService;
        this.searchVm = searchItemViewModel;

        hotkeyService.OpenMenuHotkeyPressed += delegate
        {
            this.ShouldOpenContextMenu = true;
            this.ShouldOpenContextMenu = false;
        };

        this.itemDataService.PropertyChanged += ItemDataService_PropertyChanged;
    }

    private void ItemDataService_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(ItemDataService.RootItems))
        {
            List<object> menuItems = new(this.itemDataService.RootItems ?? Enumerable.Empty<object>());
            menuItems.Add(new System.Windows.Controls.Separator());
            menuItems.Add(searchVm);

            this.Items = menuItems;
        }
    }

    [ObservableProperty]
    private bool shouldOpenContextMenu;

    [ObservableProperty]
    private List<object> items = new();

    [RelayCommand]
    public void RefreshItems()
    {
        try
        {
            this.itemDataService.UpdateItems();
        }
        catch (Exception ex)
        {
            // The message box closes instantly if the window is not specified.
            MessageBox.Show((Window)this.mainWindow, $"Could not open the toolbar path \"{SettingsViewModel.GetToolbarPath()}\": {ex.Message}\r\n\r\nPlease select a new path in the settings.", "Favorites menu", MessageBoxButton.OK, MessageBoxImage.Error);
            this.ShowSettings();
        }
    }

    [RelayCommand]
    private void OpenItem(ItemData item)
    {
        this.itemDataService.StartItem(item, Keyboard.Modifiers == (ModifierKeys.Shift | ModifierKeys.Control));
    }

    [RelayCommand]
    private void OpenContextMenu(ItemData item)
    {
        Shell.OpenContextMenu(item.FullPath);
    }

    [RelayCommand]
    private void ShowSettings()
    {
        this.mainWindowViewModel.SelectedNavigationViewItem = this.mainWindowViewModel.SettingsViewItem;
        this.mainWindowViewModel.ShowMainWindow();
    }

    [RelayCommand]
    private void ShowAboutWindow()
    {
        this.mainWindowViewModel.SelectedNavigationViewItem = this.mainWindowViewModel.AboutViewItem;
        this.mainWindowViewModel.ShowMainWindow();
    }

    [RelayCommand]
    private void ExitApplication()
    {
        Application.Current.Shutdown();
    }
}
