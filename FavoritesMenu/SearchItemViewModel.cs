using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using CommunityToolkit.Mvvm.ComponentModel;

namespace FavoritesMenu;

internal partial class SearchItemViewModel : ObservableObject
{
    private readonly SearchWindow searchWindow = new();

    [ObservableProperty]
    private List<ItemData> items = new();

    public SearchItemViewModel()
    {
        this.searchWindow.DataContext = this;

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
            if (!string.IsNullOrEmpty(SearchString) && !searchWindow.IsVisible)
            {
                searchWindow.Show();
                searchWindow.FocusSearchTextBox();
            }

            this.Source.View.Refresh();
        }
        else if (e.PropertyName == nameof(Items))
        {
            this.Source.Source = this.Items;
        }
    }
}
