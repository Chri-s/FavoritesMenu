using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FavoritesMenu.Services;
using Microsoft.Win32;

namespace FavoritesMenu.ViewModels;

internal partial class SettingsViewModel : ObservableObject
{
    private const string RunKeyName = @"Software\Microsoft\Windows\CurrentVersion\Run";
    private const string RunValueName = "FavoritesMenu";

    private const string SettingsKeyName = @"Software\chri-s\FavoritesMenu";
    private const string MenuPathValueName = "MenuPath";
    private const string SearchHotkeyValueName = "SearchHotkey";
    private const string MenuHotkeyValueName = "MenuHotkey";

    private readonly string ValueForStartup = $"\"{Environment.ProcessPath}\"";

    private bool isInitializing = true;

    private readonly ItemDataService itemDataService;

    public SettingsViewModel(ItemDataService itemDataService)
    {
        this.itemDataService = itemDataService;

        using var runKey = Registry.CurrentUser.CreateSubKey(RunKeyName, false);

        object? value = runKey.GetValue(RunValueName);

        this.StartWithWindows = (value is string s && string.Equals(s, ValueForStartup, StringComparison.OrdinalIgnoreCase));

        using var settingsKey = GetSettingsRegistryKey(false);
        this.ToolbarPath = GetToolbarPath(settingsKey);
        this.SearchHotKey = GetHotkey(settingsKey, SearchHotkeyValueName);
        this.MenuHotKey = GetHotkey(settingsKey, MenuHotkeyValueName);

        this.isInitializing = false;
    }

    [ObservableProperty]
    private bool startWithWindows;

    partial void OnStartWithWindowsChanged(bool value)
    {
        if (this.isInitializing)
            return;

        try
        {
            using (var runKey = Registry.CurrentUser.CreateSubKey(RunKeyName))
            {
                if (this.StartWithWindows)
                    runKey.SetValue(RunValueName, ValueForStartup);
                else
                    runKey.DeleteValue(RunValueName, false);
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show("An error occurred while changing the \"Start with Windows\"-option: " + ex.Message, "Error while saving");
        }
    }

    [ObservableProperty]
    private string toolbarPath = string.Empty;

    partial void OnToolbarPathChanged(string value)
    {
        if (this.isInitializing)
            return;

        try
        {
            using (var softwareKey = Registry.CurrentUser.CreateSubKey(SettingsKeyName))
                softwareKey.SetValue(MenuPathValueName, this.ToolbarPath);
        }
        catch (Exception ex)
        {
            MessageBox.Show("An error occured while saving the toolbar path: " + ex.Message, "Settings - Favorites Menu");
        }

        try
        {
            this.itemDataService.UpdateItems(this.ToolbarPath);
        }
        catch (Exception ex)
        {
            MessageBox.Show("An error occured while updating the toolbar: " + ex.Message, "Settings - Favorites Menu");
        }
    }

    [ObservableProperty]
    private HotkeyConverter? searchHotKey;

    partial void OnSearchHotKeyChanged(HotkeyConverter? oldValue, HotkeyConverter? newValue)
    {
        if (this.isInitializing)
            return;

        SetHotkey(SearchHotkeyValueName, newValue);
    }

    [ObservableProperty]
    private HotkeyConverter? menuHotKey;

    partial void OnMenuHotKeyChanged(HotkeyConverter? oldValue, HotkeyConverter? newValue)
    {
        if (this.isInitializing)
            return;

        SetHotkey(MenuHotkeyValueName, newValue);
    }

    [RelayCommand]
    private void BrowseFolder()
    {
        OpenFolderDialog openFolderDialog = new OpenFolderDialog();

        openFolderDialog.Title = "Select path for menu";
        openFolderDialog.InitialDirectory = this.ToolbarPath;

        if (openFolderDialog.ShowDialog() ?? false)
        {
            this.ToolbarPath = openFolderDialog.FolderName;
        }
    }

    public static string GetToolbarPath()
    {
        using var settingsKey = GetSettingsRegistryKey(false);
        return GetToolbarPath(settingsKey);
    }

    private static RegistryKey GetSettingsRegistryKey(bool writable) => Registry.CurrentUser.CreateSubKey(SettingsKeyName, writable);

    private static string GetToolbarPath(RegistryKey settingsKey)
    {
        if (settingsKey.GetValue(MenuPathValueName) is string value)
            return value;

        return string.Empty;
    }

    private static HotkeyConverter? GetHotkey(RegistryKey settingsKey, string valueName)
    {
        if (settingsKey.GetValue(valueName) is int value)
            return new HotkeyConverter((uint)value);

        return null;
    }

    private static void SetHotkey(string valueName, HotkeyConverter? hotkey)
    {
        using var settingsKey = GetSettingsRegistryKey(true);

        if (hotkey == null)
            settingsKey.DeleteValue(valueName, false);
        else
            settingsKey.SetValue(valueName, hotkey.GetSettingsFormat(), RegistryValueKind.DWord);
    }
}
