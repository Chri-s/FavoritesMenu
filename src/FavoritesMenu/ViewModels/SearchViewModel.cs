using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Windows.Data;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FavoritesMenu.Services;

namespace FavoritesMenu.ViewModels;

internal partial class SearchViewModel : ObservableObject
{
    private readonly ItemDataService itemDataService;
    private readonly MainWindowViewModel mainWindowViewModel;

    private bool closeAfterSearch = false;

    private bool isNavigatingToSearchPageFromContextMenu = false;

    public SearchViewModel(ItemDataService itemDataService, MainWindowViewModel mainWindowViewModel, HotkeyService hotkeyService)
    {
        this.itemDataService = itemDataService;
        this.mainWindowViewModel = mainWindowViewModel;

        hotkeyService.SearchHotkeyPressed += delegate { this.OpenSearchWindowForOneSearch(); };

        this.Source.SortDescriptions.Add(new SortDescription(nameof(ItemData.DisplayName), ListSortDirection.Ascending));
        this.Source.Filter += Source_Filter;
        this.Source.Source = itemDataService.AllItems ?? new List<ItemData>();
        this.Source.View.CollectionChanged += View_CollectionChanged;
        this.SelectedItem = this.Source.View.Cast<ItemData>().FirstOrDefault();

        this.itemDataService.PropertyChanged += ItemDataService_PropertyChanged;
    }

    public void OpenSearchWindowForOneSearch()
    {
        this.SearchString = string.Empty;

        if (this.mainWindowViewModel.IsMainWindowShown)
        {
            this.mainWindowViewModel.SelectedNavigationViewItem = this.mainWindowViewModel.SearchViewItem;
            this.mainWindowViewModel.ActivateMainWindow();
            return;
        }

        /// Open the main window and set the flag <see cref="closeAfterSearch"/> to true
        /// in <see cref="SearchPageActivated"/>. This closes the main window after the search.
        this.isNavigatingToSearchPageFromContextMenu = true;
        this.mainWindowViewModel.SelectedNavigationViewItem = this.mainWindowViewModel.SearchViewItem;
        this.mainWindowViewModel.ShowMainWindow();

        this.isNavigatingToSearchPageFromContextMenu = false;
    }

    /// <summary>
    /// This method should only be called by <see cref="Views.SearchPage"/> is it is navigated.
    /// </summary>
    internal void SearchPageActivated()
    {
        if (this.isNavigatingToSearchPageFromContextMenu)
            this.closeAfterSearch = true;
        else
            this.closeAfterSearch = false;
    }

    private void View_CollectionChanged(object? sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
    {
        if (this.SelectedItem == null)
            this.SelectedItem = this.Source.View.Cast<ItemData>().FirstOrDefault();
    }

    private void ItemDataService_PropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(itemDataService.AllItems))
            this.Source.Source = itemDataService.AllItems;
    }

    public CollectionViewSource Source { get; init; } = new CollectionViewSource();

    [ObservableProperty]
    private string searchString = string.Empty;

    partial void OnSearchStringChanged(string value)
    {
        this.Source.View.Refresh();
    }

    [ObservableProperty]
    private ItemData? selectedItem;

    [RelayCommand]
    private void HideMainWindow()
    {
        this.mainWindowViewModel.HideMainWindow();
    }

    [RelayCommand(CanExecute = nameof(CanOpenItem))]
    private void OpenItem(ItemData item)
    {
        if (closeAfterSearch)
            this.mainWindowViewModel.HideMainWindow();

        this.itemDataService.StartItem(item, Keyboard.Modifiers == (ModifierKeys.Shift | ModifierKeys.Control));
    }

    private bool CanOpenItem(ItemData item) => item != null;

    [RelayCommand(CanExecute = nameof(CanOpenContextMenu))]
    private void OpenContextMenu(ItemData item)
    {
        if (closeAfterSearch)
            this.mainWindowViewModel.HideMainWindow();

        Shell.OpenContextMenu(item.FullPath);
    }

    private bool CanOpenContextMenu(ItemData item) => item != null;

    private void Source_Filter(object sender, FilterEventArgs e)
    {
        if (string.IsNullOrEmpty(this.SearchString))
            e.Accepted = true;
        else
            e.Accepted = ((ItemData)e.Item).DisplayName.Contains(this.SearchString, StringComparison.OrdinalIgnoreCase);
    }
}
