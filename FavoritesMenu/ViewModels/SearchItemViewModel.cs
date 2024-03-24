using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FavoritesMenu.Views;

namespace FavoritesMenu.ViewModels;

internal partial class SearchItemViewModel : ObservableObject
{
    private readonly SearchViewModel searchViewModel;

    public SearchItemViewModel(SearchViewModel searchViewModel)
    {
        this.searchViewModel = searchViewModel;
    }

    [RelayCommand]
    private void OpenSearch() => this.searchViewModel.OpenSearchWindowForOneSearch();
}
