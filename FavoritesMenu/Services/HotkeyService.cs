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
    private readonly IMainWindow mainWindow;

    public event EventHandler? SearchHotkeyPressed;

    public event EventHandler? OpenMenuHotkeyPressed;

    private const int SearchHotKeyId = 0xB123;
    private const int OpenMenuHotKeyId = 0xB124;

    private bool isSearchHotkeyRegistered;
    private bool isOpenMenuHotkeyRegistered;

    public HotkeyService(IMainWindow mainWindow)
    {
        this.mainWindow = mainWindow;

        this.mainWindow.HotkeyPressed += MainWindow_HotkeyPressed;
    }

    public void Dispose()
    {
        if (this.isSearchHotkeyRegistered)
        {
            PInvoke.UnregisterHotKey(new HWND(this.mainWindow.Handle), SearchHotKeyId);
            this.isSearchHotkeyRegistered = false;
        }

        if (this.isOpenMenuHotkeyRegistered)
        {
            PInvoke.UnregisterHotKey(new HWND(this.mainWindow.Handle), OpenMenuHotKeyId);
            this.isOpenMenuHotkeyRegistered = false;
        }
    }

    public bool SetSearchHotkey(HotkeyConverter? hotkey) => this.ChangeHotkey(ref this.isSearchHotkeyRegistered, SearchHotKeyId, hotkey);

    public bool SetOpenMenuHotkey(HotkeyConverter? hotkey) => this.ChangeHotkey(ref this.isOpenMenuHotkeyRegistered, OpenMenuHotKeyId, hotkey);

    private bool ChangeHotkey(ref bool isRegistered, int hotkeyId, HotkeyConverter? hotkey)
    {
        if (isRegistered)
        {
            PInvoke.UnregisterHotKey(new HWND(this.mainWindow.Handle), hotkeyId);
            isRegistered = false;
        }

        if (hotkey != null)
        {
            bool success = PInvoke.RegisterHotKey(new HWND(this.mainWindow.Handle), hotkeyId, hotkey.HotKeyModifiers, hotkey.VirtualKey);
            isRegistered = success;
            return success;
        }

        return true;
    }

    private void MainWindow_HotkeyPressed(object? sender, HotkeyEventArgs e)
    {
        if (e.HotkeyId == SearchHotKeyId)
            this.SearchHotkeyPressed?.Invoke(this, EventArgs.Empty);
        else if (e.HotkeyId == OpenMenuHotKeyId)
            this.OpenMenuHotkeyPressed?.Invoke(this, EventArgs.Empty);
    }
}
