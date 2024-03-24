using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace FavoritesMenu.Controls;

internal partial class HotkeyEditor : UserControl
{
    public HotkeyConverter? Hotkey
    {
        get { return (HotkeyConverter?)GetValue(HotkeyProperty); }
        set { SetValue(HotkeyProperty, value); }
    }

    public static readonly DependencyProperty HotkeyProperty =
        DependencyProperty.Register("Hotkey", typeof(HotkeyConverter), typeof(HotkeyEditor), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, Hotkey_Changed));

    private static void Hotkey_Changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        HotkeyEditor editor = (HotkeyEditor)d;

        editor.removeButton.IsEnabled = (e.NewValue != null);
    }

    public HotkeyEditor()
    {
        InitializeComponent();
    }

    private void TextBox_PreviewKeyDown(object sender, KeyEventArgs e)
    {
        e.Handled = true;

        ModifierKeys modifiers = e.KeyboardDevice.Modifiers;

        if (Keyboard.IsKeyDown(Key.LWin) || Keyboard.IsKeyDown(Key.RWin))
            modifiers |= ModifierKeys.Windows;

        // If Key == Key.System, the real key is in e.SystemKey
        Key key = (e.Key == Key.System ? e.SystemKey : e.Key);

        // Pressing delete, backspace or escape without modifiers clears the current value
        if (modifiers == ModifierKeys.None &&
            (key == Key.Delete || key == Key.Back || key == Key.Escape))
        {
            this.Hotkey = null;
            return;
        }

        // If no actual key was pressed - return
        if (key == Key.LeftCtrl ||
            key == Key.RightCtrl ||
            key == Key.LeftAlt ||
            key == Key.RightAlt ||
            key == Key.LeftShift ||
            key == Key.RightShift ||
            key == Key.LWin ||
            key == Key.RWin ||
            key == Key.Clear ||
            key == Key.OemClear ||
            key == Key.Apps)
            return;

        this.Hotkey = new HotkeyConverter(key, modifiers, true);
    }

    private void removeButton_Click(object sender, RoutedEventArgs e)
    {
        this.Hotkey = null;
    }
}
