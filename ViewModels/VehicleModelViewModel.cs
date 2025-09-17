using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using BD_taksi.Entities;
using BD_taksi.Services;
using System.Collections.ObjectModel;
using System.Windows;

namespace BD_taksi.ViewModels
{
	public partial class VehicleModelViewModel : BaseViewModel, IActivatableViewModel
	{
		private readonly IVehicleModelRepository _vehicleModelRepository;
		private readonly IVehicleMakeRepository _vehicleMakeRepository;
		private readonly INotificationService _notificationService;

		[ObservableProperty]
		private ObservableCollection<VehicleModel> _vehicleModels = new();

		[ObservableProperty]
		private VehicleModel? _selectedVehicleModel;

		[ObservableProperty]
		private string _searchTerm = string.Empty;

		[ObservableProperty]
		private bool _isEditMode;

		// Для редактирования
		[ObservableProperty]
		private string _name = string.Empty;

		[ObservableProperty]
		private string _class = "Economy";

		[ObservableProperty]
		private int _makeId;

		[ObservableProperty]
		private ObservableCollection<VehicleMake> _vehicleMakes = new();

		public VehicleModelViewModel(IVehicleModelRepository vehicleModelRepository, IVehicleMakeRepository vehicleMakeRepository, INotificationService notificationService)
		{
			_vehicleModelRepository = vehicleModelRepository;
			_vehicleMakeRepository = vehicleMakeRepository;
			_notificationService = notificationService;
			
			// Подписываемся на уведомления об изменениях
			_notificationService.DataChanged += OnDataChanged;
			
			_ = LoadVehicleModelsAsync();
			_ = LoadVehicleMakesAsync();
		}

		[RelayCommand]
		private async Task LoadVehicleModelsAsync()
		{
			IsBusy = true;
			try
			{
				var vehicleModels = await _vehicleModelRepository.SearchAsync(SearchTerm);
				VehicleModels.Clear();
				foreach (var vehicleModel in vehicleModels)
				{
					VehicleModels.Add(vehicleModel);
				}
			}
			finally
			{
				IsBusy = false;
			}
		}

		[RelayCommand]
		private async Task LoadVehicleMakesAsync()
		{
			try
			{
				var vehicleMakes = await _vehicleMakeRepository.GetAllAsync();
				VehicleMakes.Clear();
				foreach (var vehicleMake in vehicleMakes)
				{
					VehicleMakes.Add(vehicleMake);
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show($"Ошибка при загрузке марок: {ex.Message}", "Ошибка", 
					MessageBoxButton.OK, MessageBoxImage.Error);
			}
		}

		[RelayCommand]
		private async Task SearchAsync()
		{
			await LoadVehicleModelsAsync();
		}

		[RelayCommand]
		private void AddNew()
		{
			IsEditMode = true;
			SelectedVehicleModel = null;
			Name = string.Empty;
			Class = "Economy";
			MakeId = 0;
		}

		[RelayCommand]
		private void Edit()
		{
			if (SelectedVehicleModel == null)
			{
				MessageBox.Show("Выберите модель для редактирования!", "Предупреждение", 
					MessageBoxButton.OK, MessageBoxImage.Information);
				return;
			}

			IsEditMode = true;
			Name = SelectedVehicleModel.Name;
			Class = SelectedVehicleModel.Class;
			MakeId = SelectedVehicleModel.MakeId;
		}

		[RelayCommand]
		private async Task SaveAsync()
		{
			if (MakeId == 0)
			{
				MessageBox.Show("Выберите марку!", "Ошибка валидации", 
					MessageBoxButton.OK, MessageBoxImage.Warning);
				return;
			}

			if (string.IsNullOrWhiteSpace(Name))
			{
				MessageBox.Show("Поле 'Название' обязательно для заполнения!", "Ошибка валидации", 
					MessageBoxButton.OK, MessageBoxImage.Warning);
				return;
			}

			if (string.IsNullOrWhiteSpace(Class))
			{
				MessageBox.Show("Поле 'Класс' обязательно для заполнения!", "Ошибка валидации", 
					MessageBoxButton.OK, MessageBoxImage.Warning);
				return;
			}

			IsBusy = true;
			try
			{
				if (SelectedVehicleModel == null)
				{
					// Добавление
					var newVehicleModel = new VehicleModel
					{
						Name = Name,
						Class = Class,
						MakeId = MakeId
					};

					await _vehicleModelRepository.AddAsync(newVehicleModel);
				}
				else
				{
					// Редактирование
					SelectedVehicleModel.Name = Name;
					SelectedVehicleModel.Class = Class;
					SelectedVehicleModel.MakeId = MakeId;

					await _vehicleModelRepository.UpdateAsync(SelectedVehicleModel);
				}

				await _vehicleModelRepository.SaveChangesAsync();
				IsEditMode = false;
				await LoadVehicleModelsAsync();
				await LoadVehicleMakesAsync(); // Обновляем список марок
				
				var message = SelectedVehicleModel == null ? "Модель успешно добавлена!" : "Модель успешно обновлена!";
				MessageBox.Show(message, "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
			}
			catch (Exception ex)
			{
				var errorMessage = $"Ошибка при сохранении: {ex.Message}";
				if (ex.InnerException != null)
				{
					errorMessage += $"\n\nДетали: {ex.InnerException.Message}";
				}
				MessageBox.Show(errorMessage, "Ошибка", 
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
			SelectedVehicleModel = null;
		}

		[RelayCommand]
		private async Task DeleteAsync()
		{
			if (SelectedVehicleModel == null)
			{
				MessageBox.Show("Выберите модель для удаления!", "Предупреждение", 
					MessageBoxButton.OK, MessageBoxImage.Information);
				return;
			}

			var result = MessageBox.Show($"Вы уверены, что хотите удалить модель '{SelectedVehicleModel.Name}'?", 
				"Подтверждение удаления", MessageBoxButton.YesNo, MessageBoxImage.Question);
			
			if (result != MessageBoxResult.Yes) return;

			IsBusy = true;
			try
			{
				await _vehicleModelRepository.DeleteAsync(SelectedVehicleModel.Id);
				await _vehicleModelRepository.SaveChangesAsync();
				await LoadVehicleModelsAsync();
				
				MessageBox.Show("Модель успешно удалена!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
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

		private async void OnDataChanged(string entityType)
		{
			if (entityType == "VehicleMake")
			{
				await LoadVehicleMakesAsync();
			}
		}

		public async Task OnActivatedAsync()
		{
			await LoadVehicleMakesAsync();
		}
	}
}
