using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows;

namespace FavoritesMenu;

public class MenuItemStyleSelector : StyleSelector
{
    public Style DefaultMenuItemStyle { get; set; } = null!;

    public Style SearchMenuItemStyle { get; set; } = null!;

    public Style SeparatorStyle { get; set; } = null!;

    public override Style SelectStyle(object item, DependencyObject container)
    {
        return item switch
        {
            ItemData => DefaultMenuItemStyle,
            Separator => SeparatorStyle,
            _ => SearchMenuItemStyle,
        };
    }
}