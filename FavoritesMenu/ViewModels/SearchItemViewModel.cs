using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using CommunityToolkit.Mvvm.ComponentModel;
using FavoritesMenu.Views;

namespace FavoritesMenu.ViewModels;

internal partial class SearchItemViewModel : ObservableObject
{
    [ObservableProperty]
    private List<ItemData> items = new();

    public SearchItemViewModel()
    {
        this.Source.SortDescriptions.Add(new SortDescription(nameof(ItemData.DisplayName), ListSortDirection.Ascending));
        this.Source.Filter += Source_Filter;
    }

    private void Source_Filter(object sender, FilterEventArgs e)
    {
        if (string.IsNullOrEmpty(this.SearchString))
            e.Accepted = true;
        else
            e.Accepted = ((ItemData)e.Item).DisplayName.Contains(this.SearchString, StringComparison.OrdinalIgnoreCase);
    }

    [ObservableProperty]
    private string searchString = string.Empty;

    public CollectionViewSource Source { get; init; } = new CollectionViewSource();

    protected override void OnPropertyChanged(PropertyChangedEventArgs e)
    {
        base.OnPropertyChanged(e);

        if (e.PropertyName == nameof(SearchString))
        {
            //if (!string.IsNullOrEmpty(SearchString) && !searchWindow.IsVisible)
            //{
            //    searchWindow.Show();
            //    searchWindow.FocusSearchTextBox();
            //}

            this.Source.View.Refresh();
        }
        else if (e.PropertyName == nameof(Items))
        {
            this.Source.Source = this.Items;
        }
    }
}
