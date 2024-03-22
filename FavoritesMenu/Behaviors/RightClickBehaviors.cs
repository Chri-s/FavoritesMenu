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

internal class RightClickBehaviors : DependencyObject
{
    public static ICommand? GetCommand(DependencyObject obj) => (ICommand?)obj.GetValue(CommandProperty);

    public static void SetCommand(DependencyObject obj, ICommand? value) => obj.SetValue(CommandProperty, value);

    public static readonly DependencyProperty CommandProperty =
        DependencyProperty.RegisterAttached("Command", typeof(ICommand), typeof(RightClickBehaviors), new UIPropertyMetadata(null, Command_Changed));

    public static object GetCommandParameter(DependencyObject obj) => (object)obj.GetValue(CommandParameterProperty);

    public static void SetCommandParameter(DependencyObject obj, object value) => obj.SetValue(CommandParameterProperty, value);

    public static readonly DependencyProperty CommandParameterProperty =
        DependencyProperty.RegisterAttached("CommandParameter", typeof(object), typeof(RightClickBehaviors), new PropertyMetadata(null));

    private static void Command_Changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (!(d is UIElement uiElement))
            throw new Exception("RightClickBehaviors.Command can only be set on UIElement.");

        if (e.OldValue == null && e.NewValue != null)
            uiElement.PreviewMouseUp += UiElement_PreviewMouseUp_ForCommand;
        else if (e.OldValue != null && e.NewValue == null)
            uiElement.PreviewMouseUp -= UiElement_PreviewMouseUp_ForCommand;
    }

    private static void UiElement_PreviewMouseUp_ForCommand(object sender, MouseButtonEventArgs e)
    {
        if (e.ChangedButton != MouseButton.Right)
            return;

        UIElement uiElement = (UIElement)sender;
        ICommand? cmd = GetCommand(uiElement);
        if (cmd == null)
            return;

        object param = GetCommandParameter(uiElement);

        if (cmd.CanExecute(param))
            cmd.Execute(param);

        e.Handled = true;
    }




    public static ContextMenu GetTrayRightClickContextMenu(DependencyObject obj) => (ContextMenu)obj.GetValue(TrayRightClickContextMenuProperty);

    public static void SetTrayRightClickContextMenu(DependencyObject obj, ContextMenu value) => obj.SetValue(TrayRightClickContextMenuProperty, value);

    public static readonly DependencyProperty TrayRightClickContextMenuProperty =
        DependencyProperty.RegisterAttached("TrayRightClickContextMenu", typeof(ContextMenu), typeof(RightClickBehaviors), new UIPropertyMetadata(null, TrayRightClickContextMenu_Changed));

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
