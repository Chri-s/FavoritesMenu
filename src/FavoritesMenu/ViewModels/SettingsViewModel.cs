﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection.Emit;
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
    private const string ThemeValueName = "Theme";

    private readonly string ValueForStartup = $"\"{Environment.ProcessPath}\"";

    private bool isInitializing = true;

    private readonly ItemDataService itemDataService;
    private readonly HotkeyService hotkeyService;
    private readonly IMainWindow mainWindow;

    public SettingsViewModel(ItemDataService itemDataService, HotkeyService hotkeyService, IMainWindow mainWindow)
    {
        this.itemDataService = itemDataService;
        this.hotkeyService = hotkeyService;
        this.mainWindow = mainWindow;

        using var runKey = Registry.CurrentUser.CreateSubKey(RunKeyName, false);

        object? value = runKey.GetValue(RunValueName);

        this.StartWithWindows = (value is string s && string.Equals(s, ValueForStartup, StringComparison.OrdinalIgnoreCase));

        using var settingsKey = GetSettingsRegistryKey(false);
        this.ToolbarPath = GetToolbarPath(settingsKey);
        this.SearchHotKey = GetHotkey(settingsKey, SearchHotkeyValueName);
        this.MenuHotKey = GetHotkey(settingsKey, MenuHotkeyValueName);
        this.SelectedTheme = GetTheme(settingsKey);

        this.isInitializing = false;
    }

    public void InitHotkeys()
    {
        if (!this.hotkeyService.SetSearchHotkey(this.SearchHotKey))
            MessageBox.Show($"Could not register the hotkey \"{this.SearchHotKey}\" for the search. It is probably already registered by another application.", "Favorites Menu", MessageBoxButton.OK, MessageBoxImage.Warning);

        if (!this.hotkeyService.SetOpenMenuHotkey(this.MenuHotKey))
            MessageBox.Show($"Could not register the hotkey \"{this.MenuHotKey}\" to open the toolbar. It is probably already registered by another application.", "Favorites Menu", MessageBoxButton.OK, MessageBoxImage.Warning);
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

        SaveToolbarPath(value);

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
    private List<ThemeViewModel> themes = new List<ThemeViewModel>()
    {
        new("Light", "Light", Wpf.Ui.Appearance.ApplicationTheme.Light),
        new("Dark", "Dark", Wpf.Ui.Appearance.ApplicationTheme.Dark),
        new("HighContrast", "High contrast", Wpf.Ui.Appearance.ApplicationTheme.HighContrast),
        new("SystemDefault", "System Default", null),
    };

    [ObservableProperty]
    private ThemeViewModel selectedTheme;

    partial void OnSelectedThemeChanged(ThemeViewModel? oldValue, ThemeViewModel newValue)
    {
        oldValue?.Revoke((Window)this.mainWindow);
        newValue.Apply((Window)this.mainWindow);

        if (this.isInitializing)
            return;

        SaveTheme(newValue.Name);
    }

    [ObservableProperty]
    private HotkeyConverter? searchHotKey;

    partial void OnSearchHotKeyChanged(HotkeyConverter? oldValue, HotkeyConverter? newValue)
    {
        if (this.isInitializing)
            return;

        SetHotkey(SearchHotkeyValueName, newValue);
        this.InitHotkeys();
    }

    [ObservableProperty]
    private HotkeyConverter? menuHotKey;

    partial void OnMenuHotKeyChanged(HotkeyConverter? oldValue, HotkeyConverter? newValue)
    {
        if (this.isInitializing)
            return;

        SetHotkey(MenuHotkeyValueName, newValue);
        this.InitHotkeys();
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

    [RelayCommand]
    private void OpenFolder()
    {
        try
        {
            string explorerPath = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Windows), "explorer.exe");
            Process.Start(new ProcessStartInfo(explorerPath, @$"""{this.ToolbarPath}"""));
        }
        catch (Exception ex)
        {
            MessageBox.Show("Could not open folder: " + ex.Message, "Favorites Menu - Open folder");
        }
    }

    public static string GetToolbarPath()
    {
        using var settingsKey = GetSettingsRegistryKey(false);
        return GetToolbarPath(settingsKey);
    }

    private static void SaveToolbarPath(string toolbarPath)
    {
        try
        {
            using (var softwareKey = Registry.CurrentUser.CreateSubKey(SettingsKeyName))
                softwareKey.SetValue(MenuPathValueName, toolbarPath);
        }
        catch (Exception ex)
        {
            MessageBox.Show("An error occured while saving the toolbar path: " + ex.Message, "Settings - Favorites Menu");
        }
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

    private ThemeViewModel GetTheme(RegistryKey settingsKey)
    {
        if (!(settingsKey.GetValue(ThemeValueName) is string value))
            return this.Themes.First(t => t.IsSystemDefault);

        ThemeViewModel? theme = this.Themes.FirstOrDefault(t => t.Name == value);
        if (theme == null)
            theme = this.Themes.First(t => t.IsSystemDefault);

        return theme;
    }

    private static void SaveTheme(string name)
    {
        try
        {
            using (var softwareKey = Registry.CurrentUser.CreateSubKey(SettingsKeyName))
                softwareKey.SetValue(ThemeValueName, name);
        }
        catch (Exception ex)
        {
            MessageBox.Show("An error occured while saving the themes: " + ex.Message, "Settings - Favorites Menu");
        }
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
