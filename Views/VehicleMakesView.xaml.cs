using System.Windows.Controls;
using Microsoft.Extensions.DependencyInjection;
using BD_taksi.ViewModels;

namespace BD_taksi.Views
{
    public partial class VehicleMakesView : UserControl
    {
        public VehicleMakesView()
        {
            InitializeComponent();
            DataContext = ((App)App.Current).Services.GetService<VehicleMakeViewModel>();
        }
    }
}
