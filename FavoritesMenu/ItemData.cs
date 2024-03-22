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
    public ItemData(string displayName, string fullPath, bool isFile, ImageSource? imageSource)
    {
        this.DisplayName = displayName;
        this.FullPath = fullPath;
        this.IsFile = isFile;
        this.ImageSource = imageSource;
    }

    public string DisplayName { get; set; }

    public string FullPath { get; set; }

    public bool IsFile { get; set; }

    public ImageSource? ImageSource { get; set; }

    public List<ItemData> SubItems { get; } = new();
}
