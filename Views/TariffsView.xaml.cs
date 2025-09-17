using System.Windows.Controls;
using Microsoft.Extensions.DependencyInjection;
using BD_taksi.ViewModels;

namespace BD_taksi.Views
{
    public partial class TariffsView : UserControl
    {
        public TariffsView()
        {
            InitializeComponent();
            DataContext = ((App)App.Current).Services.GetService<TariffViewModel>();
        }
    }
}
