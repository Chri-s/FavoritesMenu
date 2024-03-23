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
using FavoritesMenu.Views;


namespace FavoritesMenu.ViewModels;

internal partial class NotifyIconViewModel : ObservableObject
{
    private SettingsWindow? settingsWindow;

    private List<object> rootItems = null!;

    private SearchItemViewModel searchVm = new SearchItemViewModel();

    public NotifyIconViewModel()
    {
        this.searchVm.PropertyChanged += SearchVm_PropertyChanged;
    }

    private void SearchVm_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
        if (e.PropertyName != nameof(searchVm.SearchString))
            return;

        if (string.IsNullOrEmpty(searchVm.SearchString))
        {
            this.Items = rootItems;
        }
        else
        {
            List<object> items = this.rootItems.OfType<ItemData>()
                                              .Where(i => i.DisplayName.Contains(searchVm.SearchString, StringComparison.OrdinalIgnoreCase))
                                              .Cast<object>()
                                              .Append(this.searchVm)
                                              .ToList();

            this.Items = items;
        }
    }

    [ObservableProperty]
    private List<object> items = new();

    [RelayCommand]
    public void RefreshItems()
    {
        string path = SettingsViewModel.GetToolbarPath();

        try
        {
            List<ItemData> allItems = new();
            List<ItemData> itemData = AddDirectory(path, string.Empty, allItems);
            this.rootItems = itemData.Cast<object>().Append(searchVm).ToList();
            this.Items = rootItems;

            this.searchVm.Items = allItems;
        }
        catch
        {
            this.ShowSettings(true);
        }
    }

    private List<ItemData> AddDirectory(string path, string folderPath, List<ItemData> allItems)
    {
        var menuItems = (from d in new DirectoryInfo(path).GetFileSystemInfos("*", new EnumerationOptions() { IgnoreInaccessible = true })
                         where !d.Attributes.HasFlag(FileAttributes.Hidden)
                         let i = new ItemData(Shell.GetDisplayName(d.FullName), d.FullName, folderPath, d is FileInfo, Shell.GetFileIcon(d.FullName), Shell.GetLargeFileIcon(d.FullName))
                         select i).ToList();

        menuItems.Sort((x, y) =>
        {
            int result = x.IsFile.CompareTo(y.IsFile);
            if (result != 0)
                return result;

            return string.Compare(x.DisplayName, y.DisplayName, true);
        });

        string folderPathWithSeparator = folderPath + ((folderPath.Length > 0) ? " > " : string.Empty);

        foreach (var directoryItem in menuItems)
        {
            if (!directoryItem.IsFile)
            {
                string displayName = Shell.GetDisplayName(directoryItem.FullPath);
                string itemFolderPath = folderPathWithSeparator + displayName;
                directoryItem.SubItems.AddRange(AddDirectory(directoryItem.FullPath, itemFolderPath, allItems));
            }
            else
            {
                allItems.Add(directoryItem);
            }
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
