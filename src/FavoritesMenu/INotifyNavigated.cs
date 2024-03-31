using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FavoritesMenu;

interface INotifyNavigated
{
    /// <summary>
    /// Gets called if the current page was activated by the navigation.
    /// </summary>
    void Navigated();
}
