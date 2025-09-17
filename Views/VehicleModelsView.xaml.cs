using System.Windows.Controls;
using Microsoft.Extensions.DependencyInjection;
using BD_taksi.ViewModels;

namespace BD_taksi.Views
{
	public partial class VehicleModelsView : UserControl
	{
		private VehicleModelViewModel? _viewModel;

		public VehicleModelsView()
		{
			InitializeComponent();
			_viewModel = ((App)App.Current).Services.GetService<VehicleModelViewModel>();
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
