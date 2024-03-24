using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;
using Windows.Win32.UI.Input.KeyboardAndMouse;

namespace FavoritesMenu;

internal class HotkeyConverter
{
    public Key Key { get; init; }

    public ModifierKeys ModifierKeys { get; init; }

    public HOT_KEY_MODIFIERS HotKeyModifiers { get; init; }

    public uint VirtualKey { get; init; }

    public bool NoRepeat { get; init; }

    public HotkeyConverter(Key key, ModifierKeys modifierKeys, bool noRepeat)
    {
        this.Key = key;
        this.ModifierKeys = modifierKeys;

        this.HotKeyModifiers = 0;

        if (modifierKeys.HasFlag(ModifierKeys.Control))
            this.HotKeyModifiers |= HOT_KEY_MODIFIERS.MOD_CONTROL;

        if (modifierKeys.HasFlag(ModifierKeys.Shift))
            this.HotKeyModifiers |= HOT_KEY_MODIFIERS.MOD_SHIFT;

        if (modifierKeys.HasFlag(ModifierKeys.Alt))
            this.HotKeyModifiers |= HOT_KEY_MODIFIERS.MOD_ALT;

        if (modifierKeys.HasFlag(ModifierKeys.Windows))
            this.HotKeyModifiers |= HOT_KEY_MODIFIERS.MOD_WIN;

        if (noRepeat)
            this.HotKeyModifiers |= HOT_KEY_MODIFIERS.MOD_NOREPEAT;

        this.VirtualKey = (uint)KeyInterop.VirtualKeyFromKey(key);
    }

    public HotkeyConverter(uint settingsFormat)
    {
        this.HotKeyModifiers = (HOT_KEY_MODIFIERS)(settingsFormat >> 16);

        this.ModifierKeys = ModifierKeys.None;

        if (this.HotKeyModifiers.HasFlag(HOT_KEY_MODIFIERS.MOD_CONTROL))
            this.ModifierKeys |= ModifierKeys.Control;

        if (this.HotKeyModifiers.HasFlag(HOT_KEY_MODIFIERS.MOD_SHIFT))
            this.ModifierKeys |= ModifierKeys.Shift;

        if (this.HotKeyModifiers.HasFlag(HOT_KEY_MODIFIERS.MOD_ALT))
            this.ModifierKeys |= ModifierKeys.Alt;

        if (this.HotKeyModifiers.HasFlag(HOT_KEY_MODIFIERS.MOD_WIN))
            this.ModifierKeys |= ModifierKeys.Windows;

        this.NoRepeat = this.HotKeyModifiers.HasFlag(HOT_KEY_MODIFIERS.MOD_NOREPEAT);

        this.VirtualKey = settingsFormat & 0xFFFF;

        this.Key = KeyInterop.KeyFromVirtualKey((int)this.VirtualKey);
    }

    public uint GetSettingsFormat()
    {
        uint format = (uint)this.HotKeyModifiers;
        format <<= 16;

        format |= VirtualKey;

        return format;
    }

    public override string ToString()
    {
        StringBuilder sb = new StringBuilder();

        if (this.ModifierKeys.HasFlag(ModifierKeys.Windows))
            sb.Append("Win + ");
        if (this.ModifierKeys.HasFlag(ModifierKeys.Control))
            sb.Append("Ctrl + ");
        if (this.ModifierKeys.HasFlag(ModifierKeys.Alt))
            sb.Append("Alt + ");
        if (this.ModifierKeys.HasFlag(ModifierKeys.Shift))
            sb.Append("Shift + ");

        sb.Append(this.Key);

        return sb.ToString();
    }
}
