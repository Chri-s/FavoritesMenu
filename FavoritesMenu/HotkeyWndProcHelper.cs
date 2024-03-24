using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Interop;
using Windows.Win32;

namespace FavoritesMenu;

class HotkeyWndProcHelper : IDisposable
{
    private readonly Action<int> hotkeyCallback;
    private HwndSource hwndSource;

    public HotkeyWndProcHelper(Action<int> hotkeyCallback)
    {
        this.hotkeyCallback = hotkeyCallback;

        this.hwndSource = new HwndSource(0, 0, 0, 0, 0, "HOTKEY_WINDOW", -3);
        this.hwndSource.AddHook(this.WndProc);
    }

    public nint Hwnd { get => this.hwndSource.Handle; }

    public void Dispose()
    {
        this.hwndSource.RemoveHook(this.WndProc);
        this.hwndSource.Dispose();
    }

    private IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
    {
        if (msg == PInvoke.WM_HOTKEY)
        {
            hotkeyCallback(wParam.ToInt32());
            handled = true;
        }

        return IntPtr.Zero;
    }
}
