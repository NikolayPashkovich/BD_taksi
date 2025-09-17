using System.Windows.Controls;
using System.Windows.Input;
using System.Text.RegularExpressions;
using Microsoft.Extensions.DependencyInjection;
using BD_taksi.ViewModels;

namespace BD_taksi.Views
{
	/// <summary>
	/// Interaction logic for DriversView.xaml
	/// </summary>
	public partial class DriversView : UserControl
	{
		public DriversView()
		{
			InitializeComponent();
			DataContext = ((App)App.Current).Services.GetService<DriverViewModel>();
		}

		private void PhoneTextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
		{
			// Разрешаем только цифры, пробелы, скобки, дефисы и +
			var regex = new Regex(@"^[\d\s\(\)\-\+]+$");
			e.Handled = !regex.IsMatch(e.Text);
		}
	}
} 