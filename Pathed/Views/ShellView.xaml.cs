using Pathed.ViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

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
