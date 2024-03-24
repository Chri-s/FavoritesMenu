﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FavoritesMenu.Services;


namespace FavoritesMenu.ViewModels;

internal partial class NotifyIconViewModel : ObservableObject
{
    private readonly MainWindowViewModel mainWindowViewModel;
    private readonly ItemDataService itemDataService;
    private readonly SearchItemViewModel searchVm;

    public NotifyIconViewModel(MainWindowViewModel mainWindowViewModel, ItemDataService itemDataService, SearchItemViewModel searchItemViewModel)
    {
        this.mainWindowViewModel = mainWindowViewModel;
        this.itemDataService = itemDataService;
        this.searchVm = searchItemViewModel;

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
    private List<object> items = new();

    [RelayCommand]
    public void RefreshItems()
    {
        this.itemDataService.UpdateItems();
    }

    [RelayCommand]
    private void OpenItem(ItemData item)
    {
        ProcessStartInfo psi = new ProcessStartInfo(item.FullPath) { UseShellExecute = true };
        try
        {
            Process.Start(psi);
        }
        catch
        {
            // Do nothing
        }
    }

    [RelayCommand]
    private void OpenContextMenu(ItemData item)
    {
        Shell.OpenContextMenu(item.FullPath);
    }

    [RelayCommand]
    private void ShowSettings() => ShowSettings(false);

    private void ShowSettings(bool toolbarPathIsInvalid)
    {
        this.mainWindowViewModel.SelectedNavigationViewItem = this.mainWindowViewModel.SettingsViewItem;
        this.mainWindowViewModel.ShowMainWindow();
        //if (this.settingsWindow != null)
        //{
        //    this.settingsWindow.Focus();
        //    return;
        //}

        //this.settingsWindow = new();
        //SettingsViewModel vm = new SettingsViewModel();
        //settingsWindow.DataContext = vm;

        //settingsWindow.Closed += delegate { this.settingsWindow = null; };

        //if (!(settingsWindow.ShowDialog() ?? false))
        //    return;

        //this.RefreshItems();
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
