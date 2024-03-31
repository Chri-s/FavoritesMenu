using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace FavoritesMenu.Services;

internal interface IMainWindow
{
    event EventHandler<HotkeyEventArgs>? HotkeyPressed;

    void Show();

    void Hide();

    bool Activate();

    Visibility Visibility { get; }

    nint Handle { get; }
}
