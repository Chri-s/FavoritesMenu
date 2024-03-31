using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Wpf.Ui.Appearance;

namespace FavoritesMenu.ViewModels;

internal partial class ThemeViewModel
{
    public string Name { get; private init; }

    public string DisplayName { get; private init; }

    public bool IsSystemDefault { get; private init; }

    private readonly ApplicationTheme theme;

    public ThemeViewModel(string name, string displayName, ApplicationTheme? applicationTheme)
    {
        this.Name = name;
        this.DisplayName = displayName;
        this.IsSystemDefault = (applicationTheme == null);
        this.theme = applicationTheme ?? ApplicationTheme.Unknown;
    }

    public void Apply(Window? window)
    {
        if (this.IsSystemDefault)
        {
            SystemThemeWatcher.Watch(window);
            ApplicationThemeManager.ApplySystemTheme();
        }
        else
        {
            ApplicationThemeManager.Apply(this.theme);
        }
    }

    public void Revoke(Window? window)
    {
        if (this.IsSystemDefault)
        {
            SystemThemeWatcher.UnWatch(window);
        }
    }
}
