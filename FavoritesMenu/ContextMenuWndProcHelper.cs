using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Interop;
using FavoritesMenu.My;

namespace FavoritesMenu;

/// <summary>
/// This class registers a window message hook which is needed for the shell context menu.
/// </summary>
internal class ContextMenuWndProcHelper : IDisposable
{
    private HwndSource hwndSource;

    public IContextMenu2? ContextMenu2 { get; set; }
    public IContextMenu3? ContextMenu3 { get; set; }

    public nint Hwnd { get => this.hwndSource.Handle; }

    public ContextMenuWndProcHelper()
    {
        this.hwndSource = new HwndSource(0, 0, 0, 0, 0, "HWND_WINDOW", -3);
        this.hwndSource.AddHook(this.WndProc);
    }

    public void Dispose()
    {
        hwndSource.Dispose();
    }

    private IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
    {
        if (ContextMenu3 != null)
        {
            if (ContextMenu3.HandleMenuMsg2(msg, wParam, lParam, out nint lpResult) == 0)
            {
                handled = true;
                return lpResult;
            }
        }
        if (ContextMenu2 != null)
        {
            if (ContextMenu2.HandleMenuMsg(msg, wParam, lParam) == 0)
            {
                handled = true;
                return IntPtr.Zero;
            }
        }

        return IntPtr.Zero;
    }
}
