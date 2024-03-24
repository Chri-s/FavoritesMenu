using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows.Data;
using CommunityToolkit.Mvvm.ComponentModel;
using FavoritesMenu.Services;

namespace FavoritesMenu.ViewModels;

internal partial class SearchViewModel : ObservableObject
{
    private readonly ItemDataService itemDataService;

    public SearchViewModel(ItemDataService itemDataService)
    {
        this.itemDataService = itemDataService;

        this.Source.SortDescriptions.Add(new SortDescription(nameof(ItemData.DisplayName), ListSortDirection.Ascending));
        this.Source.Filter += Source_Filter;
        this.Source.Source = itemDataService.AllItems;

        this.itemDataService.PropertyChanged += ItemDataService_PropertyChanged;
    }

    private void ItemDataService_PropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(itemDataService.AllItems))
            this.Source.Source = itemDataService.AllItems;
    }

    public CollectionViewSource Source { get; init; } = new CollectionViewSource();

    [ObservableProperty]
    private string searchString = string.Empty;

    private void Source_Filter(object sender, FilterEventArgs e)
    {
        if (string.IsNullOrEmpty(this.SearchString))
            e.Accepted = true;
        else
            e.Accepted = ((ItemData)e.Item).DisplayName.Contains(this.SearchString, StringComparison.OrdinalIgnoreCase);
    }

    protected override void OnPropertyChanged(PropertyChangedEventArgs e)
    {
        base.OnPropertyChanged(e);

        if (e.PropertyName == nameof(SearchString))
        {
            this.Source.View.Refresh();
        }
    }
}
