using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using H.NotifyIcon;
using Windows.Win32;

namespace FavoritesMenu.Behaviors;

internal class TaskbarIconBehaviors : DependencyObject
{
    #region TrayRightClickContextMenu

    public static ContextMenu GetTrayRightClickContextMenu(DependencyObject obj) => (ContextMenu)obj.GetValue(TrayRightClickContextMenuProperty);

    public static void SetTrayRightClickContextMenu(DependencyObject obj, ContextMenu value) => obj.SetValue(TrayRightClickContextMenuProperty, value);

    public static readonly DependencyProperty TrayRightClickContextMenuProperty =
        DependencyProperty.RegisterAttached("TrayRightClickContextMenu", typeof(ContextMenu), typeof(TaskbarIconBehaviors), new UIPropertyMetadata(null, TrayRightClickContextMenu_Changed));

    private static void TrayRightClickContextMenu_Changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (!(d is TaskbarIcon taskbarIcon))
            throw new Exception("TaskbarIconBehaviors.TrayRightClickContextMenu can only be set on TaskbarIcon.");

        if (e.OldValue == null && e.NewValue != null)
            taskbarIcon.TrayRightMouseUp += TaskbarIcon_TrayRightMouseUp;
        else if (e.OldValue != null && e.NewValue == null)
            taskbarIcon.TrayRightMouseUp -= TaskbarIcon_TrayRightMouseUp;
    }

    private static void TaskbarIcon_TrayRightMouseUp(object sender, RoutedEventArgs e)
    {
        if (!(e.Source is TaskbarIcon taskbarIcon))
            throw new Exception("Wrong source for TrayRightMouseUp: " + e.Source.GetType().FullName);

        ContextMenu? menu = GetTrayRightClickContextMenu(taskbarIcon);

        ShowContextMenu(taskbarIcon, menu);
    }
    #endregion

    #region TrayLeftClickContextMenu

    public static ContextMenu? GetTrayLeftClickContextMenu(DependencyObject obj) => (ContextMenu)obj.GetValue(TrayLeftClickContextMenuProperty);

    public static void SetTrayLeftClickContextMenu(DependencyObject obj, ContextMenu? value) => obj.SetValue(TrayLeftClickContextMenuProperty, value);

    public static readonly DependencyProperty TrayLeftClickContextMenuProperty =
        DependencyProperty.RegisterAttached("TrayLeftClickContextMenu", typeof(ContextMenu), typeof(TaskbarIconBehaviors), new UIPropertyMetadata(null, TrayLeftClickContextMenu_Changed));

    private static void TrayLeftClickContextMenu_Changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (!(d is TaskbarIcon taskbarIcon))
            throw new Exception("TaskbarIconBehaviors.TrayLeftClickContextMenu can only be set on TaskbarIcon.");

        if (e.OldValue == null && e.NewValue != null)
            taskbarIcon.TrayLeftMouseUp += TaskbarIcon_TrayLeftMouseUp;
        else if (e.OldValue != null && e.NewValue == null)
            taskbarIcon.TrayLeftMouseUp -= TaskbarIcon_TrayLeftMouseUp;
    }

    private static void TaskbarIcon_TrayLeftMouseUp(object sender, RoutedEventArgs e)
    {
        if (!(e.Source is TaskbarIcon taskbarIcon))
            throw new Exception("Wrong source for TrayLeftMouseUp: " + e.Source.GetType().FullName);

        ContextMenu? menu = GetTrayLeftClickContextMenu(taskbarIcon);

        ShowContextMenu(taskbarIcon, menu);
    }
    #endregion

    #region ShouldOpenLeftClickContextMenu
    public static bool GetShouldOpenLeftClickContextMenu(DependencyObject obj) => (bool)obj.GetValue(ShouldOpenLeftClickContextMenuProperty);

    public static void SetShouldOpenLeftClickContextMenu(DependencyObject obj, bool value) => obj.SetValue(ShouldOpenLeftClickContextMenuProperty, value);

    public static readonly DependencyProperty ShouldOpenLeftClickContextMenuProperty =
        DependencyProperty.RegisterAttached("ShouldOpenLeftClickContextMenu", typeof(bool), typeof(TaskbarIconBehaviors), new FrameworkPropertyMetadata(false, ShouldOpenLeftClickContextMenu_Changed));

    private static void ShouldOpenLeftClickContextMenu_Changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (!(bool)e.NewValue)
            return;

        if (!(d is TaskbarIcon taskbarIcon))
            throw new Exception("TaskbarIconBehaviors.ShouldOpenLeftClickContextMenu can only be set on TaskbarIcon.");

        ContextMenu? menu = GetTrayLeftClickContextMenu(taskbarIcon);

        ShowContextMenu(taskbarIcon, menu);
    }
    #endregion

    private static void ShowContextMenu(TaskbarIcon taskbarIcon, ContextMenu? menu)
    {
        if (menu == null)
            return;

        // This happens when the context menu is open and the user clicks the tray icon again.
        if (menu.Tag is ContextMenuWndProcHelper helper)
        {
            PInvoke.SetForegroundWindow(new Windows.Win32.Foundation.HWND(helper.Hwnd));
        }
        else if (menu.Tag == null)
        {
            UpdateDataContext(menu, taskbarIcon.DataContext);

            // Create a message window and set it to foreground.
            // Otherwise the context menu won't be closed if the user clicks somewhere outside of the menu
            ContextMenuWndProcHelper hwndHelper = new ContextMenuWndProcHelper();
            menu.Tag = hwndHelper;
            PInvoke.SetForegroundWindow(new Windows.Win32.Foundation.HWND(hwndHelper.Hwnd));
            menu.Closed += DisposeHwndHelper;
        }

        menu.IsOpen = true;
    }

    private static void UpdateDataContext(FrameworkElement? target, object? newDataContextValue)
    {
        // If there is no target or it's data context is determined through a binding
        // of its own, keep it
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
