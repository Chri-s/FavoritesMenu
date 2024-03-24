using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Wpf.Ui;
using Wpf.Ui.Controls;

namespace FavoritesMenu.Services;

class NavigationService : INavigationService
{
    private INavigationView navigationView = null!;
    private IPageService pageService;

    public NavigationService(IPageService pageService)
    {
        this.pageService = pageService;
    }

    public INavigationView GetNavigationControl() => this.navigationView;

    public bool GoBack() => this.navigationView.GoBack();

    public bool Navigate(Type pageType) => this.navigationView.Navigate(pageType);

    public bool Navigate(Type pageType, object? dataContext) => this.navigationView.Navigate(pageType, dataContext);

    public bool Navigate(string pageIdOrTargetTag) => this.navigationView.Navigate(pageIdOrTargetTag);

    public bool Navigate(string pageIdOrTargetTag, object? dataContext) => this.navigationView.Navigate(pageIdOrTargetTag, dataContext);

    public bool NavigateWithHierarchy(Type pageType) => this.navigationView.NavigateWithHierarchy(pageType);

    public bool NavigateWithHierarchy(Type pageType, object? dataContext) => this.navigationView.NavigateWithHierarchy(pageType, dataContext);

    public void SetNavigationControl(INavigationView navigation)
    {
        if (this.navigationView != null)
            this.navigationView.Navigated -= NavigationView_Navigated;

        this.navigationView = navigation;

        this.navigationView.Navigated += NavigationView_Navigated;
    }

    private void NavigationView_Navigated(NavigationView sender, NavigatedEventArgs args)
    {
        (args.Page as INotifyNavigated)?.Navigated();
    }

    public void SetPageService(IPageService pageService) => this.pageService = pageService;
}
