using System.Collections.ObjectModel;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using FavoritesMenu.ViewModels;
using FavoritesMenu.Views;

namespace FavoritesMenu;
/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
    private H.NotifyIcon.TaskbarIcon taskbarIcon = null!;

    internal static App CurrentApp { get; private set; } = null!;

    protected override void OnStartup(StartupEventArgs e)
    {
        App.CurrentApp = this;

        this.taskbarIcon = (H.NotifyIcon.TaskbarIcon)FindResource("TaskbarIcon");

        ((NotifyIconViewModel)this.taskbarIcon.DataContext).RefreshItems();

        this.taskbarIcon.ForceCreate();

        new MainWindow() { DataContext = new MainWindowViewModel() }.Show();
    }

    //private List<ItemData> AddDirectory(string path)
    //{
    //    var menuItems = (from d in new DirectoryInfo(path).GetFileSystemInfos("*", new EnumerationOptions() { IgnoreInaccessible = true })
    //                     where !d.Attributes.HasFlag(FileAttributes.Hidden)
    //                     let i = new ItemData(Shell.GetDisplayName(d.FullName), d.FullName, d is FileInfo, Shell.GetFileIcon(d.FullName))
    //                     select i).ToList();

    //    menuItems.Sort((x, y) =>
    //    {
    //        int result = x.IsFile.CompareTo(y.IsFile);
    //        if (result != 0)
    //            return result;

    //        return string.Compare(x.DisplayName, y.DisplayName, true);
    //    });

    //    foreach (var directoryItem in menuItems)
    //    {
    //        if (!directoryItem.IsFile)
    //            directoryItem.SubItems.AddRange(AddDirectory(directoryItem.FullPath));
    //    }

    //    return menuItems;
    //}

    //private void RefreshItems()
    //{
    //    string path = SettingsViewModel.GetToolbarPath();

    //    try
    //    {
    //        var items = new ObservableCollection<ItemData>(AddDirectory(path));

    //        this.taskbarIcon.ContextMenu.ItemsSource = items;
    //    }
    //    catch
    //    {
    //        NotifyIconViewModel vm = (NotifyIconViewModel)this.taskbarIcon.DataContext;
    //        vm.ShowSettings(true);
    //    }
    //}
}

