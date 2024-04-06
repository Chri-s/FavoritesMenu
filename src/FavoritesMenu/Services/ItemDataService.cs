using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using Windows.Win32.Foundation;

namespace FavoritesMenu.Services;

internal partial class ItemDataService : ObservableObject
{
    private List<ItemData>? rootItems;

    public List<ItemData>? RootItems
    {
        get => rootItems;
        private set => this.SetProperty(ref this.rootItems, value);
    }


    private List<ItemData>? allItems;

    public List<ItemData>? AllItems
    {
        get => allItems;
        private set => this.SetProperty(ref this.allItems, value);
    }

    private string? path;
    public string? Path
    {
        get => path;
        private set => this.SetProperty(ref this.path, value);
    }

    public void UpdateItems() => this.UpdateItems(this.Path ?? throw new InvalidOperationException("Path has not been initialized. Call UpdateItems(string path) for the first time."));

    public void UpdateItems(string path)
    {
        this.Path = path;
        List<ItemData> allItems = new();
        var rootItems = AddDirectory(path, string.Empty, allItems);

        this.AllItems = allItems;
        this.RootItems = rootItems;
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

    public void StartItem(ItemData item, bool runElevated)
    {
        ProcessStartInfo psi = new ProcessStartInfo(item.FullPath) { UseShellExecute = true };

        if (runElevated)
            psi.Verb = "runas";

        try
        {
            Process.Start(psi);
        }
        catch (Win32Exception ex) when (ex.NativeErrorCode == (int)WIN32_ERROR.ERROR_NO_ASSOCIATION)
        {
            // This happens if we start the process with the verb "runas" but the file can not be opened with runas.
            // Perhaps because it is not an executable.
            // Then we start it without a verb
            psi.Verb = string.Empty;

            try
            {
                Process.Start(psi);
            }
            catch
            {
                // Do nothing
            }
        }
        catch
        {
            // Do nothing
        }
    }
}
