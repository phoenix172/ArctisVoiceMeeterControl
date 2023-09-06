using ArctisVoiceMeeter.ViewModels;
using System;
using System.Collections.Generic;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ArctisVoiceMeeter
{
    /// <summary>
    /// Interaction logic for ChannelBindingView.xaml
    /// </summary>
    public partial class ChannelBindingView : UserControl
    {
        public ChannelBindingView(ChannelBindingViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
        }
    }
}
