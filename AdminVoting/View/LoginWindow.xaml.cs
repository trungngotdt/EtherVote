using System.Windows;
using System.Windows.Controls;
using AdminVoting.ViewModel;

namespace AdminVoting.View
{
    /// <summary>
    /// Interaction logic for Login.xaml
    /// </summary>
    public partial class Login : Page
    {
        public Login()
        {
            InitializeComponent();
            //Closing += (s, e) => ViewModelLocator.Cleanup();
        }
    }
}
