using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace FavoritesMenu;

internal partial class AboutWindowViewModel
{
    public AboutWindow? AboutWindow { get; set; }

    public string Version => Assembly.GetExecutingAssembly().GetCustomAttribute<AssemblyFileVersionAttribute>()?.Version ?? string.Empty;

    [RelayCommand]
    private void OpenLink(Uri uri)
    {
        try
        {
            Process.Start(new ProcessStartInfo(uri.ToString()) { UseShellExecute = true });
        }
        catch
        {
            // Do nothing
        }
    }

    [RelayCommand]
    private void CloseWindow()
    {
        this.AboutWindow?.Close();
    }
}
