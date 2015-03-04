using System.ComponentModel.Composition;
using System.Windows;
using Pathed.ViewModels;

namespace Pathed.Views
{
    [Export, PartCreationPolicy(CreationPolicy.Shared)]
    public partial class ShellView : Window
    {
        [Import]
        public ShellViewModel ViewModel
        {
            get { return (ShellViewModel)DataContext; }
            set { DataContext = value; }
        }

        [ImportingConstructor]
        public ShellView()
        {
            InitializeComponent();
        }
    }
}
