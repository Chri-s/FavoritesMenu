using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace FavoritesMenu;

internal static class Extensions
{
    public static bool HasVisualParent(this DependencyObject? o, object itemToCheck)
    {
        if (o == null)
            return false;

        DependencyObject? parent = VisualTreeHelper.GetParent(o);
        while (parent != null)
        {
            if (parent == itemToCheck)
                return true;

            parent = VisualTreeHelper.GetParent(parent);
        }

        return false;
    }
}
