using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using FavoritesMenu.Views;
using Wpf.Ui.Controls;

namespace FavoritesMenu.ViewModels;

internal partial class MainWindowViewModel : ObservableObject
{
    [ObservableProperty]
    private string applicationTitle = "Favorites Menu";

    [ObservableProperty]
    private List<NavigationViewItem> navigationViewItems = new List<NavigationViewItem>()
    {
        new NavigationViewItem("Search", SymbolRegular.Search16, typeof(SearchPage)),
        new NavigationViewItem("Settings", SymbolRegular.Settings24, typeof(SettingsPage)),
        new NavigationViewItem("About", SymbolRegular.Info12, typeof(AboutPage)),
    };


}
