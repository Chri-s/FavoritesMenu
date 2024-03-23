using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;

namespace FavoritesMenu;

public class ItemData
{
    public ItemData(string displayName, string fullPath, string folderPath, bool isFile, ImageSource? imageSource, ImageSource? largeImageSource)
    {
        this.DisplayName = displayName;
        this.FullPath = fullPath;
        this.FolderPath = folderPath;
        this.IsFile = isFile;
        this.ImageSource = imageSource;
        this.LargeImageSource = largeImageSource;
    }

    public string DisplayName { get; init; }

    public string FolderPath { get; init; }

    public string FullPath { get; init; }

    public bool IsFile { get; init; }

    public ImageSource? ImageSource { get; init; }
    public ImageSource? LargeImageSource { get; init; }

    public List<ItemData> SubItems { get; } = new();
}
