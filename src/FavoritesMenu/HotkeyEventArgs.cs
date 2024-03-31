using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FavoritesMenu;

internal class HotkeyEventArgs : EventArgs
{
    public HotkeyEventArgs(int hotkeyId)
    {
        HotkeyId = hotkeyId;
    }

    public int HotkeyId { get; private init; }
}
