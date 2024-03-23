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
using System.Windows.Shapes;

namespace FavoritesMenu;
/// <summary>
/// Interaction logic for SearchWindow.xaml
/// </summary>
partial class SearchWindow
{
    public SearchWindow()
    {
        InitializeComponent();
    }

    public void FocusSearchTextBox()
    {
        this.searchTextBox.Focus();
        this.searchTextBox.SelectionStart = this.searchTextBox.Text.Length;
    }
}
