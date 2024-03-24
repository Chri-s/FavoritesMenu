using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Windows.Data;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FavoritesMenu.Services;
using Windows.Win32.System.Com;

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
        this.Source.View.CollectionChanged += View_CollectionChanged;
        this.SelectedItem = this.Source.View.Cast<ItemData>().FirstOrDefault();

        this.itemDataService.PropertyChanged += ItemDataService_PropertyChanged;
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

    [RelayCommand(CanExecute = nameof(CanOpenItem))]
    private void OpenItem(ItemData item)
    {
        ProcessStartInfo psi = new ProcessStartInfo(item.FullPath) { UseShellExecute = true };
        try
        {
            Process.Start(psi);
        }
        catch
        {
            // Do nothing
        }
    }

    private bool CanOpenItem(ItemData item) => item != null;

    [RelayCommand(CanExecute = nameof(CanOpenContextMenu))]
    private void OpenContextMenu(ItemData item)
    {
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
