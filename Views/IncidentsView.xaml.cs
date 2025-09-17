using System.Windows.Controls;
using Microsoft.Extensions.DependencyInjection;
using BD_taksi.ViewModels;

namespace BD_taksi.Views
{
    /// <summary>
    /// Логика взаимодействия для IncidentsView.xaml
    /// </summary>
    public partial class IncidentsView : UserControl
    {
        private IncidentViewModel? _viewModel;

        public IncidentsView()
        {
            InitializeComponent();
            _viewModel = ((App)App.Current).Services.GetService<IncidentViewModel>();
            DataContext = _viewModel;
        }

        private async void UserControl_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            if (_viewModel is IActivatableViewModel activatableViewModel)
            {
                await activatableViewModel.OnActivatedAsync();
            }
        }
    }
}
