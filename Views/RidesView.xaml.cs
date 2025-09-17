using System.Windows.Controls;
using Microsoft.Extensions.DependencyInjection;
using BD_taksi.ViewModels;

namespace BD_taksi.Views
{
	public partial class RidesView : UserControl
	{
		private RideViewModel? _viewModel;

		public RidesView()
		{
			InitializeComponent();
			_viewModel = ((App)App.Current).Services.GetService<RideViewModel>();
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
