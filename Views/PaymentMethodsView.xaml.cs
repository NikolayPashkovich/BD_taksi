using System.Windows.Controls;
using Microsoft.Extensions.DependencyInjection;
using BD_taksi.ViewModels;

namespace BD_taksi.Views
{
    public partial class PaymentMethodsView : UserControl
    {
        public PaymentMethodsView()
        {
            InitializeComponent();
            DataContext = ((App)App.Current).Services.GetService<PaymentMethodViewModel>();
        }
    }
}
