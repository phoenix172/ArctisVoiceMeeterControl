using System.Windows.Controls;
using ArctisVoiceMeeter.ViewModels;

namespace ArctisVoiceMeeter.Views
{
    /// <summary>
    /// Interaction logic for ChannelBindingView.xaml
    /// </summary>
    public partial class ChannelBindingView : UserControl
    {
        public ChannelBindingView() { InitializeComponent(); }

        public ChannelBindingView(ChannelBindingViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
        }
    }
}
