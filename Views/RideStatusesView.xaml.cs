using System.Windows.Controls;
using Microsoft.Extensions.DependencyInjection;
using BD_taksi.ViewModels;

namespace BD_taksi.Views
{
    /// <summary>
    /// Логика взаимодействия для RideStatusesView.xaml
    /// </summary>
    public partial class RideStatusesView : UserControl
    {
        public RideStatusesView()
        {
            InitializeComponent();
            DataContext = ((App)App.Current).Services.GetService<RideStatusViewModel>();
        }
    }
}
