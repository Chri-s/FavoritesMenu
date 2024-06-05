using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows.Input;
using FavoritesMenu.Services;
using FavoritesMenu.ViewModels;

namespace FavoritesMenu.Views;

internal partial class SearchPage : Page, INotifyNavigated, ISearchPage
{
    private readonly SearchViewModel viewModel;
    public SearchPage(SearchViewModel viewModel)
    {
        this.viewModel = viewModel;

        InitializeComponent();

        this.DataContext = viewModel;

        viewModel.Source.View.CollectionChanged += View_CollectionChanged;
    }

    private async void View_CollectionChanged(object? sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
    {
        // Dispatch ScrollIntoView because the resultListView hasn't applied the changed collection yet.
        // If we invoke it after rendering, the resultListView has made the changes from the collection and scrolls to the correct position.
        await Dispatcher.InvokeAsync(() =>
        {
            if (this.resultListView.SelectedIndex == -1 && this.resultListView.Items.Count > 0)
                this.resultListView.SelectedIndex = 0;

            this.resultListView.ScrollIntoView(this.resultListView.SelectedItem);
        }, System.Windows.Threading.DispatcherPriority.Background);
    }

    public void Navigated()
    {
        this.viewModel.SearchPageActivated();
        this.searchTextBox.Focus();

        // Dispatch ScrollIntoView because the resultListView hasn't applied the changed collection yet.
        // If we invoke it after rendering, the resultListView has made the changes from the collection and scrolls to the correct position.
        Dispatcher.InvokeAsync(() =>
        {
            if (this.resultListView.Items.Count > 0)
                this.resultListView.SelectedIndex = 0;

            this.resultListView.ScrollIntoView(this.resultListView.SelectedItem);
        }, System.Windows.Threading.DispatcherPriority.Background).Wait();
    }

    public void FocusSearchTextBox()
    {
        this.searchTextBox.Focus();
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
}
