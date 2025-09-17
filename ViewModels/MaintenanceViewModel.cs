using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using BD_taksi.Entities;
using BD_taksi.Services;
using System.Collections.ObjectModel;
using System.Windows;

namespace BD_taksi.ViewModels
{
	public partial class MaintenanceViewModel : BaseViewModel, IActivatableViewModel
	{
		private readonly IMaintenanceRepository _maintenanceRepository;
		private readonly INotificationService _notificationService;
		private readonly IVehicleRepository _vehicleRepository;

		[ObservableProperty]
		private ObservableCollection<Maintenance> _maintenances = new();

		[ObservableProperty]
		private Maintenance? _selectedMaintenance;

		[ObservableProperty]
		private string _searchTerm = string.Empty;

		[ObservableProperty]
		private bool _isEditMode;

		// Для редактирования
		[ObservableProperty]
		private DateTime _date = DateTime.Today;

		[ObservableProperty]
		private string _type = string.Empty;

		[ObservableProperty]
		private decimal _cost;

		[ObservableProperty]
		private int _odometer;

		[ObservableProperty]
		private string _comment = string.Empty;

		[ObservableProperty]
		private int _vehicleId;

		[ObservableProperty]
		private ObservableCollection<Vehicle> _vehicles = new();

		public MaintenanceViewModel(IMaintenanceRepository maintenanceRepository, IVehicleRepository vehicleRepository, INotificationService notificationService)
		{
			_maintenanceRepository = maintenanceRepository;
			_vehicleRepository = vehicleRepository;
			_notificationService = notificationService;
			
			// Подписываемся на уведомления об изменениях
			_notificationService.DataChanged += OnDataChanged;
			
			_ = LoadMaintenancesAsync();
			_ = LoadVehiclesAsync();
		}

		[RelayCommand]
		private async Task LoadMaintenancesAsync()
		{
			IsBusy = true;
			try
			{
				var maintenances = await _maintenanceRepository.SearchAsync(SearchTerm);
				Maintenances.Clear();
				foreach (var maintenance in maintenances)
				{
					Maintenances.Add(maintenance);
				}
			}
			finally
			{
				IsBusy = false;
			}
		}

		[RelayCommand]
		private async Task LoadVehiclesAsync()
		{
			try
			{
				var vehicles = await _vehicleRepository.GetActiveVehiclesAsync();
				Vehicles.Clear();
				foreach (var vehicle in vehicles)
				{
					Vehicles.Add(vehicle);
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show($"Ошибка при загрузке автомобилей: {ex.Message}", "Ошибка", 
					MessageBoxButton.OK, MessageBoxImage.Error);
			}
		}

		[RelayCommand]
		private async Task SearchAsync()
		{
			await LoadMaintenancesAsync();
		}

		[RelayCommand]
		private void AddNew()
		{
			IsEditMode = true;
			SelectedMaintenance = null;
			Date = DateTime.Today;
			Type = string.Empty;
			Cost = 0;
			Odometer = 0;
			Comment = string.Empty;
			VehicleId = 0;
		}

		[RelayCommand]
		private void Edit()
		{
			if (SelectedMaintenance == null)
			{
				MessageBox.Show("Выберите ТО/ремонт для редактирования!", "Предупреждение", 
					MessageBoxButton.OK, MessageBoxImage.Information);
				return;
			}

			IsEditMode = true;
			Date = SelectedMaintenance.Date;
			Type = SelectedMaintenance.Type;
			Cost = SelectedMaintenance.Cost;
			Odometer = SelectedMaintenance.Odometer;
			Comment = SelectedMaintenance.Comment ?? string.Empty;
			VehicleId = SelectedMaintenance.VehicleId;
		}

		[RelayCommand]
		private async Task SaveAsync()
		{
			if (VehicleId == 0)
			{
				MessageBox.Show("Выберите автомобиль!", "Ошибка валидации", 
					MessageBoxButton.OK, MessageBoxImage.Warning);
				return;
			}

			if (string.IsNullOrWhiteSpace(Type))
			{
				MessageBox.Show("Поле 'Тип' обязательно для заполнения!", "Ошибка валидации", 
					MessageBoxButton.OK, MessageBoxImage.Warning);
				return;
			}

			if (Cost < 0)
			{
				MessageBox.Show("Стоимость не может быть отрицательной!", "Ошибка валидации", 
					MessageBoxButton.OK, MessageBoxImage.Warning);
				return;
			}

			if (Odometer < 0)
			{
				MessageBox.Show("Пробег не может быть отрицательным!", "Ошибка валидации", 
					MessageBoxButton.OK, MessageBoxImage.Warning);
				return;
			}

			IsBusy = true;
			try
			{
				if (SelectedMaintenance == null)
				{
					// Добавление
					var newMaintenance = new Maintenance
					{
						Date = Date,
						Type = Type,
						Cost = Cost,
						Odometer = Odometer,
						Comment = Comment,
						VehicleId = VehicleId
					};

					await _maintenanceRepository.AddAsync(newMaintenance);
				}
				else
				{
					// Редактирование
					SelectedMaintenance.Date = Date;
					SelectedMaintenance.Type = Type;
					SelectedMaintenance.Cost = Cost;
					SelectedMaintenance.Odometer = Odometer;
					SelectedMaintenance.Comment = Comment;
					SelectedMaintenance.VehicleId = VehicleId;

					await _maintenanceRepository.UpdateAsync(SelectedMaintenance);
				}

				await _maintenanceRepository.SaveChangesAsync();
				IsEditMode = false;
				await LoadMaintenancesAsync();
				
				var message = SelectedMaintenance == null ? "ТО/ремонт успешно добавлен!" : "ТО/ремонт успешно обновлен!";
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
			SelectedMaintenance = null;
		}

		[RelayCommand]
		private async Task DeleteAsync()
		{
			if (SelectedMaintenance == null)
			{
				MessageBox.Show("Выберите ТО/ремонт для удаления!", "Предупреждение", 
					MessageBoxButton.OK, MessageBoxImage.Information);
				return;
			}

			var result = MessageBox.Show($"Вы уверены, что хотите удалить ТО/ремонт?", 
				"Подтверждение удаления", MessageBoxButton.YesNo, MessageBoxImage.Question);
			
			if (result != MessageBoxResult.Yes) return;

			IsBusy = true;
			try
			{
				await _maintenanceRepository.DeleteAsync(SelectedMaintenance.Id);
				await _maintenanceRepository.SaveChangesAsync();
				await LoadMaintenancesAsync();
				
				MessageBox.Show("ТО/ремонт успешно удален!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
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
			await LoadVehiclesAsync();
		}

		private async void OnDataChanged(string entityType)
		{
			if (entityType == "Vehicle")
			{
				await LoadVehiclesAsync();
			}
		}
	}
}
