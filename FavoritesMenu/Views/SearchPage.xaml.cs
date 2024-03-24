using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using FavoritesMenu.ViewModels;

namespace FavoritesMenu.Views;

internal partial class SearchPage : Page, INotifyNavigated
{
    public SearchPage(SearchViewModel viewModel)
    {
        InitializeComponent();

        this.DataContext = viewModel;
    }

    public void Navigated()
    {
        this.searchTextBox.Focus();
        this.searchTextBox.SelectionStart = this.searchTextBox.Text.Length;
    }
}
