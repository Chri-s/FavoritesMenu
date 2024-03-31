using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Interop;
using FavoritesMenu.Services;
using FavoritesMenu.ViewModels;
using Windows.Win32;
using Wpf.Ui;

namespace FavoritesMenu.Views;

internal partial class MainWindow : IMainWindow
{
    private readonly MainWindowViewModel viewModel;
    private readonly Services.NavigationService navigationService;

    public nint Handle => this.InteropHelper.Handle;

    public event EventHandler<HotkeyEventArgs>? HotkeyPressed;

    public MainWindow(MainWindowViewModel viewModel, IPageService pageService, Services.NavigationService navigationService, IServiceProvider serviceProvider)
    {
        this.viewModel = viewModel;
        this.navigationService = navigationService;

        InitializeComponent();

        this.InteropHelper.EnsureHandle();
        HwndSource.FromHwnd(this.InteropHelper.Handle).AddHook(WndProc);

        this.DataContext = viewModel;

        this.NavigationView.SetPageService(pageService);
        this.NavigationView.SetServiceProvider(serviceProvider);
        this.navigationService.SetNavigationControl(this.NavigationView);

        this.IsVisibleChanged += MainWindow_IsVisibleChanged;
    }

    private void MainWindow_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
    {
        if ((bool)e.NewValue)
            this.navigationService.NotifyCurrentPageActivated();
    }

    private IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
    {
        if (msg == PInvoke.WM_HOTKEY)
        {
            this.OnHotkeyPressed(new HotkeyEventArgs(wParam.ToInt32()));
            handled = true;
        }

        return IntPtr.Zero;
    }

    protected override void OnClosing(CancelEventArgs e)
    {
        // If the window is closed, it can't be opened again.
        // So we hide the window instead.
        this.Hide();
        e.Cancel = true;

        base.OnClosing(e);
    }

    private void NavigationView_SelectionChanged(Wpf.Ui.Controls.NavigationView sender, RoutedEventArgs args)
    {
        this.viewModel.SelectedNavigationViewItem = this.NavigationView.SelectedItem!;
    }

    protected virtual void OnHotkeyPressed(HotkeyEventArgs e) => this.HotkeyPressed?.Invoke(this, e);
}
