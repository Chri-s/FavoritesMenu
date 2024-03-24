using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FavoritesMenu.Views;

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
