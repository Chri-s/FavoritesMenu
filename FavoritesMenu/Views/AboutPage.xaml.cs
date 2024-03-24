using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using FavoritesMenu.ViewModels;

namespace FavoritesMenu.Views;

partial class AboutPage : Page
{
    public AboutPage(AboutViewModel viewModel)
    {
        InitializeComponent();

        this.DataContext = viewModel;
    }
}
