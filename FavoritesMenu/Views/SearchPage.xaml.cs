using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using FavoritesMenu.ViewModels;

namespace FavoritesMenu.Views;

internal partial class SearchPage : Page, INotifyNavigated
{
    public SearchPage(SearchViewModel viewModel)
    {
        InitializeComponent();

        this.DataContext = viewModel;

        viewModel.Source.View.CollectionChanged += View_CollectionChanged;
    }

    private async void View_CollectionChanged(object? sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
    {
        // Dispatch ScrollIntoView because the resultListView hasn't applied the changed collection yet.
        // If we invoke it after rendering, the resultListView has made the changes from the collection and scrolls to the correct position.
        await Dispatcher.InvokeAsync(() => this.resultListView.ScrollIntoView(this.resultListView.SelectedItem), System.Windows.Threading.DispatcherPriority.Background);
    }

    public void Navigated()
    {
        this.searchTextBox.Focus();
        this.searchTextBox.SelectionStart = this.searchTextBox.Text.Length;
    }

    private void searchTextBox_PreviewKeyDown(object sender, KeyEventArgs e)
    {
        if (Keyboard.Modifiers != ModifierKeys.None)
            return;

        if (e.Key == Key.Down)
        {
            // Select the next item in the list (as if the user pressed key down on the ListView).
            e.Handled = true;

            int newIndex = this.resultListView.SelectedIndex + 1;

            if (this.resultListView.Items.Count > newIndex)
                this.resultListView.SelectedIndex = newIndex;

            this.resultListView.ScrollIntoView(this.resultListView.Items[this.resultListView.SelectedIndex]);
        }
        else if (e.Key == Key.Up)
        {
            // Select the previous item in the list (as if the user pressed key up on the ListView).
            e.Handled = true;

            if (this.resultListView.SelectedIndex > 0)
                this.resultListView.SelectedIndex--;

            this.resultListView.ScrollIntoView(this.resultListView.Items[this.resultListView.SelectedIndex]);
        }
    }

    private void ListViewItem_MouseDoubleClick(object sender, MouseButtonEventArgs e)
    {

    }
}
