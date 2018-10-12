using GalaSoft.MvvmLight.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace CommonLibraryUtilities
{
    public interface IFrameNavigationService: INavigationService
    {
        object Parameter { get; }

        FrameworkElement GetDescendantFromName(DependencyObject parent, string name);
    }
}
