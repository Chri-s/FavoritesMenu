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
    public Style DefaultMenuItemStyle { get; set; }

    public Style SearchMenuItemStyle { get; set; }

    public override Style SelectStyle(object item, DependencyObject container)
    {
        if (item is ItemData)
            return DefaultMenuItemStyle;

        return SearchMenuItemStyle;
    }
}