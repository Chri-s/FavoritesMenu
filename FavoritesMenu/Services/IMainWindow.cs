using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace FavoritesMenu.Services
{
    interface IMainWindow
    {
        void Show();

        void Hide();

        bool Activate();

        Visibility Visibility { get; }
    }
}
