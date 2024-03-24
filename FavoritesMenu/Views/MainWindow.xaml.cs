using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows;
using FavoritesMenu.Services;
using FavoritesMenu.ViewModels;
using Wpf.Ui;

namespace FavoritesMenu.Views;

internal partial class MainWindow : IMainWindow
{
    private readonly MainWindowViewModel viewModel;

    public MainWindow(MainWindowViewModel viewModel, IPageService pageService, INavigationService navigationService, IServiceProvider serviceProvider)
    {
        this.viewModel = viewModel;

        InitializeComponent();

        this.DataContext = viewModel;

        this.NavigationView.SetPageService(pageService);
        this.NavigationView.SetServiceProvider(serviceProvider);
        navigationService.SetNavigationControl(this.NavigationView);
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
}
