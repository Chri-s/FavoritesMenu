using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using H.NotifyIcon;
using Windows.Win32;

namespace FavoritesMenu.Behaviors;

internal class TaskbarIconBehaviors : DependencyObject
{
    public static ContextMenu GetTrayRightClickContextMenu(DependencyObject obj) => (ContextMenu)obj.GetValue(TrayRightClickContextMenuProperty);

    public static void SetTrayRightClickContextMenu(DependencyObject obj, ContextMenu value) => obj.SetValue(TrayRightClickContextMenuProperty, value);

    public static readonly DependencyProperty TrayRightClickContextMenuProperty =
        DependencyProperty.RegisterAttached("TrayRightClickContextMenu", typeof(ContextMenu), typeof(TaskbarIconBehaviors), new UIPropertyMetadata(null, TrayRightClickContextMenu_Changed));

    private static void TrayRightClickContextMenu_Changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (!(d is TaskbarIcon taskbarIcon))
            throw new Exception("RightClickBehaviors.Command can only be set on TaskbarIcon.");

        if (e.OldValue == null && e.NewValue != null)
            taskbarIcon.TrayRightMouseUp += TaskbarIcon_TrayRightMouseUp;
        else if (e.OldValue != null && e.NewValue == null)
            taskbarIcon.PreviewMouseUp -= TaskbarIcon_TrayRightMouseUp;
    }

    private static void TaskbarIcon_TrayRightMouseUp(object sender, RoutedEventArgs e)
    {
        if (!(e.Source is TaskbarIcon taskbarIcon))
            throw new Exception("Wrong source for TrayRightMouseUp: " + e.Source.GetType().FullName);

        ContextMenu? menu = GetTrayRightClickContextMenu(taskbarIcon);

        if (menu != null)
        {
            UpdateDataContext(menu, taskbarIcon.DataContext);

            // Create a message window and set it to foreground.
            // Otherwise the context menu won't be closed if the user clicks somewhere outside of the menu
            ContextMenuWndProcHelper hwndHelper = new ContextMenuWndProcHelper();
            menu.Tag = hwndHelper;
            PInvoke.SetForegroundWindow(new Windows.Win32.Foundation.HWND(hwndHelper.Hwnd));
            menu.Closed += DisposeHwndHelper;

            menu.IsOpen = true;
        }
    }

    private static void UpdateDataContext(FrameworkElement? target, object? newDataContextValue)
    {
        //if there is no target or it's data context is determined through a binding
        //of its own, keep it
        if (target == null || target.GetBindingExpression(FrameworkElement.DataContextProperty) != null)
            return;

        target.DataContext = newDataContextValue;
    }

    /// <summary>
    /// Dispose the ContextMenuWndProcHelper created in <see cref="TaskbarIcon_TrayRightMouseUp"/>.
    /// </summary>
    private static void DisposeHwndHelper(object sender, RoutedEventArgs e)
    {
        ContextMenu menu = (ContextMenu)sender;
        menu.Closed -= DisposeHwndHelper;

        ContextMenuWndProcHelper hwndHelper = (ContextMenuWndProcHelper)menu.Tag;
        menu.Tag = null;

        hwndHelper.Dispose();
    }
}
