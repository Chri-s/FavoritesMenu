using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Win32;

namespace FavoritesMenu;

internal partial class SettingsViewModel : ObservableObject
{
    private const string RunKeyName = @"Software\Microsoft\Windows\CurrentVersion\Run";
    private const string RunValueName = "FavoritesMenu";

    private const string SettingsKeyName = @"Software\chri-s\FavoritesMenu";
    private const string MenuPathValueName = "MenuPath";

    private readonly string ValueForStartup = $"\"{Environment.ProcessPath}\"";

    public SettingsViewModel(SettingsWindow window)
    {
        this.window = window;

        using var runKey = Registry.CurrentUser.CreateSubKey(RunKeyName, false);

        object? value = runKey.GetValue(RunValueName);

        this.StartWithWindows = (value is string s && string.Equals(s, ValueForStartup, StringComparison.OrdinalIgnoreCase));
        this.ToolbarPath = GetToolbarPath();
    }

    private readonly SettingsWindow window;

    [ObservableProperty]
    private bool startWithWindows;

    [ObservableProperty]
    private bool toolbarPathIsInvalid;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(SaveCommand))]
    private string toolbarPath = string.Empty;

    [RelayCommand]
    private void BrowseFolder()
    {
        OpenFolderDialog openFolderDialog = new OpenFolderDialog();

        openFolderDialog.Title = "Select path for menu";
        openFolderDialog.InitialDirectory = this.ToolbarPath;

        if (openFolderDialog.ShowDialog() ?? false)
        {
            this.ToolbarPath = openFolderDialog.FolderName;
            this.ToolbarPathIsInvalid = false;
        }
    }

    [RelayCommand]
    private void Cancel()
    {
        this.window.DialogResult = false;
    }

    private bool CanSave()
    {
        bool isValid;
        try
        {
            isValid = Path.Exists(this.ToolbarPath);
        }
        catch
        {
            isValid = false;
        }

        this.ToolbarPathIsInvalid = !isValid;

        return isValid;
    }

    [RelayCommand(CanExecute = nameof(CanSave))]
    private void Save()
    {
        this.window.DialogResult = true;

        try
        {
            using (var runKey = Registry.CurrentUser.CreateSubKey(RunKeyName))
            {
                if (this.StartWithWindows)
                    runKey.SetValue(RunValueName, ValueForStartup);
                else
                    runKey.DeleteValue(RunValueName, false);
            }

            using (var softwareKey = Registry.CurrentUser.CreateSubKey(SettingsKeyName))
                softwareKey.SetValue(MenuPathValueName, this.ToolbarPath);
        }
        catch (Exception ex)
        {
            MessageBox.Show("An error occured while saving the settings: " + ex.Message, "Settings - Favorites Menu");
        }
    }

    public static string GetToolbarPath()
    {
        using (var softwareKey = Registry.CurrentUser.CreateSubKey(SettingsKeyName, false))
        {
            object? value = softwareKey.GetValue(MenuPathValueName);

            return value as string ?? string.Empty;
        }
    }
}
