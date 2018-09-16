using EthereumVoting.ViewModel;
using System.Windows;
using System.Windows.Controls;

namespace EthereumVoting.View
{
    /// <summary>
    /// Interaction logic for ShellWindow.xaml
    /// </summary>
    public partial class ShellWindow : Page
    {
        public ShellWindow()
        {
            InitializeComponent();
            //MainFrame.NavigationUIVisibility = System.Windows.Navigation.NavigationUIVisibility.Hidden;
            //MainFrame.ContentRendered += (obj, e) => { MainFrame.NavigationUIVisibility = System.Windows.Navigation.NavigationUIVisibility.Hidden; };
            //MainFrame.RemoveBackEntry();
            //Closing += (s, e) => ViewModelLocator.Cleanup();
        }
    }
}
