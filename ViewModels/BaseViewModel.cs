using CommunityToolkit.Mvvm.ComponentModel;

namespace BD_taksi.ViewModels
{
	public abstract class BaseViewModel : ObservableObject
	{
		private bool _isBusy;
		public bool IsBusy
		{
			get => _isBusy;
			set => SetProperty(ref _isBusy, value);
		}
	}
} 