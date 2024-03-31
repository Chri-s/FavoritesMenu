using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using Wpf.Ui;

namespace FavoritesMenu.Services;

internal class PageService : IPageService
{
    private IServiceProvider serviceProvider;

    public PageService(IServiceProvider serviceProvider)
    {
        this.serviceProvider = serviceProvider;
    }

    public T? GetPage<T>() where T : class
    {
        return this.serviceProvider.GetService<T>();
    }

    public FrameworkElement? GetPage(Type pageType)
    {
        return (FrameworkElement?)this.serviceProvider.GetService(pageType);
    }
}
