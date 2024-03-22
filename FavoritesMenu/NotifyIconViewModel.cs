using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;


namespace FavoritesMenu;

internal partial class NotifyIconViewModel : ObservableObject
{
    private SettingsWindow? settingsWindow;

    [ObservableProperty]
    private List<ItemData> items = new();

    [RelayCommand]
    public void RefreshItems()
    {
        string path = SettingsViewModel.GetToolbarPath();

        try
        {
            this.Items = AddDirectory(path);
        }
        catch
        {
            this.ShowSettings(true);
        }
    }

    private List<ItemData> AddDirectory(string path)
    {
        var menuItems = (from d in new DirectoryInfo(path).GetFileSystemInfos("*", new EnumerationOptions() { IgnoreInaccessible = true })
                         where !d.Attributes.HasFlag(FileAttributes.Hidden)
                         let i = new ItemData(Shell.GetDisplayName(d.FullName), d.FullName, d is FileInfo, Shell.GetFileIcon(d.FullName))
                         select i).ToList();

        menuItems.Sort((x, y) =>
        {
            int result = x.IsFile.CompareTo(y.IsFile);
            if (result != 0)
                return result;

            return string.Compare(x.DisplayName, y.DisplayName, true);
        });

        foreach (var directoryItem in menuItems)
        {
            if (!directoryItem.IsFile)
                directoryItem.SubItems.AddRange(AddDirectory(directoryItem.FullPath));
        }

        return menuItems;
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
        if (this.settingsWindow != null)
        {
            this.settingsWindow.Focus();
            return;
        }

        this.settingsWindow = new();
        SettingsViewModel vm = new SettingsViewModel(this.settingsWindow);
        vm.ToolbarPathIsInvalid = toolbarPathIsInvalid;
        settingsWindow.DataContext = vm;

        settingsWindow.Closed += delegate { this.settingsWindow = null; };

        if (!(settingsWindow.ShowDialog() ?? false))
            return;

        this.RefreshItems();
    }

    [RelayCommand]
    private void ShowAboutWindow()
    {
        new AboutWindow().Show();
    }

    [RelayCommand]
    private void ExitApplication()
    {
        Application.Current.Shutdown();
    }
}
