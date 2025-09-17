using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using BD_taksi.Entities;
using BD_taksi.Services;
using System.Collections.ObjectModel;
using System.Windows;
using System.Text.RegularExpressions;

namespace BD_taksi.ViewModels
{
	public partial class VehicleViewModel : BaseViewModel, IActivatableViewModel
	{
		private readonly IVehicleRepository _vehicleRepository;
		private readonly IVehicleModelRepository _vehicleModelRepository;
		private readonly INotificationService _notificationService;

		[ObservableProperty]
		private ObservableCollection<Vehicle> _vehicles = new();

		[ObservableProperty]
		private Vehicle? _selectedVehicle;

		[ObservableProperty]
		private string _searchTerm = string.Empty;

		[ObservableProperty]
		private bool _isEditMode;

		// Для редактирования
		[ObservableProperty]
		private string _plateNumber = string.Empty;

		[ObservableProperty]
		private string _vin = string.Empty;

		[ObservableProperty]
		private int _year = DateTime.Now.Year;

		[ObservableProperty]
		private string _color = string.Empty;

		[ObservableProperty]
		private bool _isActive = true;

		[ObservableProperty]
		private int _modelId;

		[ObservableProperty]
		private ObservableCollection<VehicleModel> _vehicleModels = new();

		public VehicleViewModel(IVehicleRepository vehicleRepository, IVehicleModelRepository vehicleModelRepository, INotificationService notificationService)
		{
			_vehicleRepository = vehicleRepository;
			_vehicleModelRepository = vehicleModelRepository;
			_notificationService = notificationService;
			_ = LoadVehiclesAsync();
			_ = LoadVehicleModelsAsync();
		}

		[RelayCommand]
		private async Task LoadVehiclesAsync()
		{
			IsBusy = true;
			try
			{
				var vehicles = await _vehicleRepository.SearchAsync(SearchTerm);
				Vehicles.Clear();
				foreach (var vehicle in vehicles)
				{
					Vehicles.Add(vehicle);
				}
			}
			finally
			{
				IsBusy = false;
			}
		}

		[RelayCommand]
		private async Task LoadVehicleModelsAsync()
		{
			try
			{
				var vehicleModels = await _vehicleModelRepository.GetAllAsync();
				VehicleModels.Clear();
				foreach (var vehicleModel in vehicleModels)
				{
					VehicleModels.Add(vehicleModel);
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show($"Ошибка при загрузке моделей: {ex.Message}", "Ошибка", 
					MessageBoxButton.OK, MessageBoxImage.Error);
			}
		}

		[RelayCommand]
		private async Task SearchAsync()
		{
			await LoadVehiclesAsync();
		}

		[RelayCommand]
		private void AddNew()
		{
			IsEditMode = true;
			SelectedVehicle = null;
			PlateNumber = string.Empty;
			Vin = string.Empty;
			Year = DateTime.Now.Year;
			Color = string.Empty;
			IsActive = true;
			ModelId = 0;
		}

		[RelayCommand]
		private void Edit()
		{
			if (SelectedVehicle == null)
			{
				MessageBox.Show("Выберите автомобиль для редактирования!", "Предупреждение", 
					MessageBoxButton.OK, MessageBoxImage.Information);
				return;
			}

			IsEditMode = true;
			PlateNumber = SelectedVehicle.PlateNumber;
			Vin = SelectedVehicle.Vin;
			Year = SelectedVehicle.Year;
			Color = SelectedVehicle.Color;
			IsActive = SelectedVehicle.IsActive;
			ModelId = SelectedVehicle.ModelId;
		}

		[RelayCommand]
		private async Task SaveAsync()
		{
			if (ModelId == 0)
			{
				MessageBox.Show("Выберите модель!", "Ошибка валидации", 
					MessageBoxButton.OK, MessageBoxImage.Warning);
				return;
			}

			if (string.IsNullOrWhiteSpace(PlateNumber))
			{
				MessageBox.Show("Поле 'Гос. номер' обязательно для заполнения!", "Ошибка валидации", 
					MessageBoxButton.OK, MessageBoxImage.Warning);
				return;
			}

			if (string.IsNullOrWhiteSpace(Vin))
			{
				MessageBox.Show("Поле 'VIN' обязательно для заполнения!", "Ошибка валидации", 
					MessageBoxButton.OK, MessageBoxImage.Warning);
				return;
			}

			if (Vin.Length < 11 || Vin.Length > 17)
			{
				MessageBox.Show("VIN должен содержать от 11 до 17 символов!", "Ошибка валидации", 
					MessageBoxButton.OK, MessageBoxImage.Warning);
				return;
			}

			if (Year < 1990 || Year > 2100)
			{
				MessageBox.Show("Год должен быть в диапазоне 1990-2100!", "Ошибка валидации", 
					MessageBoxButton.OK, MessageBoxImage.Warning);
				return;
			}

			IsBusy = true;
			try
			{
				if (SelectedVehicle == null)
				{
					// Добавление
					var newVehicle = new Vehicle
					{
						PlateNumber = PlateNumber,
						Vin = Vin,
						Year = Year,
						Color = Color,
						IsActive = IsActive,
						ModelId = ModelId
					};

					await _vehicleRepository.AddAsync(newVehicle);
				}
				else
				{
					// Редактирование
					SelectedVehicle.PlateNumber = PlateNumber;
					SelectedVehicle.Vin = Vin;
					SelectedVehicle.Year = Year;
					SelectedVehicle.Color = Color;
					SelectedVehicle.IsActive = IsActive;
					SelectedVehicle.ModelId = ModelId;

					await _vehicleRepository.UpdateAsync(SelectedVehicle);
				}

				await _vehicleRepository.SaveChangesAsync();
				IsEditMode = false;
				await LoadVehiclesAsync();
				
				// Уведомляем другие ViewModels об изменении данных
				_notificationService.NotifyDataChanged("Vehicle");
				
				var message = SelectedVehicle == null ? "Автомобиль успешно добавлен!" : "Автомобиль успешно обновлен!";
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
			SelectedVehicle = null;
		}

		[RelayCommand]
		private async Task DeleteAsync()
		{
			if (SelectedVehicle == null)
			{
				MessageBox.Show("Выберите автомобиль для удаления!", "Предупреждение", 
					MessageBoxButton.OK, MessageBoxImage.Information);
				return;
			}

			var result = MessageBox.Show($"Вы уверены, что хотите удалить автомобиль '{SelectedVehicle.PlateNumber}'?", 
				"Подтверждение удаления", MessageBoxButton.YesNo, MessageBoxImage.Question);
			
			if (result != MessageBoxResult.Yes) return;

			IsBusy = true;
			try
			{
				await _vehicleRepository.DeleteAsync(SelectedVehicle.Id);
				await _vehicleRepository.SaveChangesAsync();
				await LoadVehiclesAsync();
				
				// Уведомляем другие ViewModels об изменении данных
				_notificationService.NotifyDataChanged("Vehicle");
				
				MessageBox.Show("Автомобиль успешно удален!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
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

		public async Task OnActivatedAsync()
		{
			await LoadVehicleModelsAsync();
		}
	}
}
