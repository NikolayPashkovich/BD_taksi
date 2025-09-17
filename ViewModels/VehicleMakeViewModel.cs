using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using BD_taksi.Entities;
using BD_taksi.Services;
using System.Collections.ObjectModel;
using System.Windows;

namespace BD_taksi.ViewModels
{
	public partial class VehicleMakeViewModel : BaseViewModel
	{
		private readonly IVehicleMakeRepository _vehicleMakeRepository;
		private readonly INotificationService _notificationService;

		[ObservableProperty]
		private ObservableCollection<VehicleMake> _vehicleMakes = new();

		[ObservableProperty]
		private VehicleMake? _selectedVehicleMake;

		[ObservableProperty]
		private string _searchTerm = string.Empty;

		[ObservableProperty]
		private bool _isEditMode;

		// Для редактирования
		[ObservableProperty]
		private string _name = string.Empty;

		public VehicleMakeViewModel(IVehicleMakeRepository vehicleMakeRepository, INotificationService notificationService)
		{
			_vehicleMakeRepository = vehicleMakeRepository;
			_notificationService = notificationService;
			_ = LoadVehicleMakesAsync();
		}

		[RelayCommand]
		private async Task LoadVehicleMakesAsync()
		{
			IsBusy = true;
			try
			{
				var vehicleMakes = await _vehicleMakeRepository.SearchAsync(SearchTerm);
				VehicleMakes.Clear();
				foreach (var vehicleMake in vehicleMakes)
				{
					VehicleMakes.Add(vehicleMake);
				}
			}
			finally
			{
				IsBusy = false;
			}
		}

		[RelayCommand]
		private async Task SearchAsync()
		{
			await LoadVehicleMakesAsync();
		}

		[RelayCommand]
		private void AddNew()
		{
			IsEditMode = true;
			SelectedVehicleMake = null;
			Name = string.Empty;
		}

		[RelayCommand]
		private void Edit()
		{
			if (SelectedVehicleMake == null)
			{
				MessageBox.Show("Выберите марку для редактирования!", "Предупреждение", 
					MessageBoxButton.OK, MessageBoxImage.Information);
				return;
			}

			IsEditMode = true;
			Name = SelectedVehicleMake.Name;
		}

		[RelayCommand]
		private async Task SaveAsync()
		{
			if (string.IsNullOrWhiteSpace(Name))
			{
				MessageBox.Show("Поле 'Название' обязательно для заполнения!", "Ошибка валидации", 
					MessageBoxButton.OK, MessageBoxImage.Warning);
				return;
			}

			IsBusy = true;
			try
			{
				if (SelectedVehicleMake == null)
				{
					// Добавление
					var newVehicleMake = new VehicleMake
					{
						Name = Name
					};

					await _vehicleMakeRepository.AddAsync(newVehicleMake);
				}
				else
				{
					// Редактирование
					SelectedVehicleMake.Name = Name;
					await _vehicleMakeRepository.UpdateAsync(SelectedVehicleMake);
				}

				await _vehicleMakeRepository.SaveChangesAsync();
				IsEditMode = false;
				await LoadVehicleMakesAsync();
				
				// Уведомляем другие ViewModels об изменении данных
				_notificationService.NotifyDataChanged("VehicleMake");
				
				var message = SelectedVehicleMake == null ? "Марка успешно добавлена!" : "Марка успешно обновлена!";
				MessageBox.Show(message, "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
			}
			catch (Exception ex)
			{
				MessageBox.Show($"Ошибка при сохранении: {ex.Message}", "Ошибка", 
					MessageBoxButton.OK, MessageBoxImage.Error);
			}
			finally
			{
				IsBusy = false;
			}
		}

		[RelayCommand]
		private void Cancel()
		{
			IsEditMode = false;
			SelectedVehicleMake = null;
		}

		[RelayCommand]
		private async Task DeleteAsync()
		{
			if (SelectedVehicleMake == null)
			{
				MessageBox.Show("Выберите марку для удаления!", "Предупреждение", 
					MessageBoxButton.OK, MessageBoxImage.Information);
				return;
			}

			var result = MessageBox.Show($"Вы уверены, что хотите удалить марку '{SelectedVehicleMake.Name}'?", 
				"Подтверждение удаления", MessageBoxButton.YesNo, MessageBoxImage.Question);
			
			if (result != MessageBoxResult.Yes) return;

			IsBusy = true;
			try
			{
				await _vehicleMakeRepository.DeleteAsync(SelectedVehicleMake.Id);
				await _vehicleMakeRepository.SaveChangesAsync();
				await LoadVehicleMakesAsync();
				
				MessageBox.Show("Марка успешно удалена!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
			}
			catch (Exception ex)
			{
				MessageBox.Show($"Ошибка при удалении: {ex.Message}", "Ошибка", 
					MessageBoxButton.OK, MessageBoxImage.Error);
			}
			finally
			{
				IsBusy = false;
			}
		}

		partial void OnSearchTermChanged(string value)
		{
			_ = SearchAsync();
		}
	}
}
