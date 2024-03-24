using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Win32;
using Windows.Win32.Foundation;

namespace FavoritesMenu.Services;

internal class HotkeyService : IDisposable
{
    private readonly HotkeyWndProcHelper hotkeyWindow;

    public event EventHandler? SearchHotkeyPressed;

    public event EventHandler? OpenMenuHotkeyPressed;

    private const int SearchHotKeyId = 0xB123;
    private const int OpenMenuHotKeyId = 0xB124;

    private bool isSearchHotkeyRegistered;
    private bool isOpenMenuHotkeyRegistered;

    public HotkeyService()
    {
        this.hotkeyWindow = new HotkeyWndProcHelper(this.HotkeyPressed);
    }

    public void Dispose()
    {
        this.hotkeyWindow.Dispose();
    }

    public bool SetSearchHotkey(HotkeyConverter? hotkey) => this.ChangeHotkey(ref this.isSearchHotkeyRegistered, SearchHotKeyId, hotkey);

    public bool SetOpenMenuHotkey(HotkeyConverter? hotkey) => this.ChangeHotkey(ref this.isOpenMenuHotkeyRegistered, OpenMenuHotKeyId, hotkey);

    private bool ChangeHotkey(ref bool isRegistered, int hotkeyId, HotkeyConverter? hotkey)
    {
        if (isRegistered)
        {
            PInvoke.UnregisterHotKey(new HWND(this.hotkeyWindow.Hwnd), hotkeyId);
            isRegistered = false;
        }

        if (hotkey != null)
        {
            bool success = PInvoke.RegisterHotKey(new HWND(this.hotkeyWindow.Hwnd), hotkeyId, hotkey.HotKeyModifiers, hotkey.VirtualKey);
            isRegistered = success;
            return success;
        }

        return true;
    }

    private void HotkeyPressed(int id)
    {
        if (id == SearchHotKeyId)
            this.SearchHotkeyPressed?.Invoke(this, EventArgs.Empty);
        else if (id == OpenMenuHotKeyId)
            this.OpenMenuHotkeyPressed?.Invoke(this, EventArgs.Empty);
    }
}
