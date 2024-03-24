using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace FavoritesMenu.Behaviors;

internal class ControlBehaviors : DependencyObject
{
    #region DoubleClickCommand
    public static ICommand? GetDoubleClickCommand(DependencyObject obj) => (ICommand?)obj.GetValue(DoubleClickCommandProperty);

    public static void SetDoubleClickCommand(DependencyObject obj, ICommand? value) => obj.SetValue(DoubleClickCommandProperty, value);

    public static readonly DependencyProperty DoubleClickCommandProperty =
        DependencyProperty.RegisterAttached("DoubleClickCommand", typeof(ICommand), typeof(ControlBehaviors), new UIPropertyMetadata(null, DoubleClickCommand_Changed));


    public static object? GetDoubleClickCommandParameter(DependencyObject obj) => (object?)obj.GetValue(DoubleClickCommandParameterProperty);

    public static void SetDoubleClickCommandParameter(DependencyObject obj, object? value) => obj.SetValue(DoubleClickCommandParameterProperty, value);

    public static readonly DependencyProperty DoubleClickCommandParameterProperty =
        DependencyProperty.RegisterAttached("DoubleClickCommandParameter", typeof(object), typeof(ControlBehaviors), new PropertyMetadata(null));


    private static void DoubleClickCommand_Changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        Control control = (Control)d;

        if (e.NewValue == null && e.OldValue != null)
            control.MouseDoubleClick -= Control_MouseDoubleClick;
        else if (e.NewValue != null && e.OldValue == null)
            control.MouseDoubleClick += Control_MouseDoubleClick;
    }

    private static void Control_MouseDoubleClick(object sender, MouseButtonEventArgs e)
    {
        if (e.ChangedButton != MouseButton.Left)
            return;

        Control control = (Control)sender;

        ICommand? cmd = GetDoubleClickCommand(control);
        if (cmd == null)
            return;

        object? parameter = GetDoubleClickCommandParameter(control);

        if (cmd.CanExecute(parameter))
            cmd.Execute(parameter);
    }
    #endregion

    #region RightClickCommand


    public static ICommand? GetRightClickCommand(DependencyObject obj) => (ICommand?)obj.GetValue(RightClickCommandProperty);

    public static void SetRightClickCommand(DependencyObject obj, ICommand? value) => obj.SetValue(RightClickCommandProperty, value);

    public static readonly DependencyProperty RightClickCommandProperty =
        DependencyProperty.RegisterAttached("RightClickCommand", typeof(ICommand), typeof(ControlBehaviors), new UIPropertyMetadata(null, RightClickCommand_Changed));


    public static object? GetRightClickCommandParameter(DependencyObject obj) => (object?)obj.GetValue(RightClickCommandParameterProperty);

    public static void SetRightClickCommandParameter(DependencyObject obj, object? value) => obj.SetValue(RightClickCommandParameterProperty, value);

    public static readonly DependencyProperty RightClickCommandParameterProperty =
        DependencyProperty.RegisterAttached("RightClickCommandParameter", typeof(object), typeof(ControlBehaviors), new PropertyMetadata(null));


    private static void RightClickCommand_Changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        Control control = (Control)d;

        if (e.NewValue == null && e.OldValue != null)
            control.MouseRightButtonUp -= Control_MouseRightButtonUp;
        else if (e.NewValue != null && e.OldValue == null)
            control.MouseRightButtonUp += Control_MouseRightButtonUp;
    }

    private static void Control_MouseRightButtonUp(object sender, MouseButtonEventArgs e)
    {
        Control control = (Control)sender;

        ICommand? cmd = GetRightClickCommand(control);
        if (cmd == null)
            return;

        object? parameter = GetRightClickCommandParameter(control);

        if (cmd.CanExecute(parameter))
            cmd.Execute(parameter);
    }
    #endregion

    #region PreviewRightClickCommand
    public static ICommand? GetPreviewRightClickCommand(DependencyObject obj) => (ICommand?)obj.GetValue(PreviewRightClickCommandProperty);

    public static void SetPreviewRightClickCommand(DependencyObject obj, ICommand? value) => obj.SetValue(PreviewRightClickCommandProperty, value);

    public static readonly DependencyProperty PreviewRightClickCommandProperty =
        DependencyProperty.RegisterAttached("PreviewRightClickCommand", typeof(ICommand), typeof(ControlBehaviors), new UIPropertyMetadata(null, PreviewRightClickCommand_Changed));

    public static object GetPreviewRightClickCommandParameter(DependencyObject obj) => (object)obj.GetValue(PreviewRightClickCommandParameterProperty);

    public static void SetPreviewRightClickCommandParameter(DependencyObject obj, object value) => obj.SetValue(PreviewRightClickCommandParameterProperty, value);

    public static readonly DependencyProperty PreviewRightClickCommandParameterProperty =
        DependencyProperty.RegisterAttached("PreviewRightClickCommandParameter", typeof(object), typeof(ControlBehaviors), new PropertyMetadata(null));

    private static void PreviewRightClickCommand_Changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (!(d is Control control))
            throw new Exception("ControlBehaviors.PreviewRightClickCommand can only be set on Control.");

        if (e.OldValue == null && e.NewValue != null)
            control.PreviewMouseUp += UiElement_PreviewMouseUp_ForCommand;
        else if (e.OldValue != null && e.NewValue == null)
            control.PreviewMouseUp -= UiElement_PreviewMouseUp_ForCommand;
    }

    private static void UiElement_PreviewMouseUp_ForCommand(object control, MouseButtonEventArgs e)
    {
        if (e.ChangedButton != MouseButton.Right)
            return;

        Control uiElement = (Control)control;
        ICommand? cmd = GetPreviewRightClickCommand(uiElement);
        if (cmd == null)
            return;

        object param = GetPreviewRightClickCommandParameter(uiElement);

        if (cmd.CanExecute(param))
            cmd.Execute(param);

        e.Handled = true;
    }
    #endregion
}
