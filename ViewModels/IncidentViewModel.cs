using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using BD_taksi.Entities;
using BD_taksi.Services;
using System.Collections.ObjectModel;
using System.Windows;

namespace BD_taksi.ViewModels
{
	public partial class IncidentViewModel : BaseViewModel, IActivatableViewModel
	{
		private readonly IIncidentRepository _incidentRepository;
		private readonly INotificationService _notificationService;
		private readonly IVehicleRepository _vehicleRepository;
		private readonly IDriverRepository _driverRepository;

		[ObservableProperty]
		private ObservableCollection<Incident> _incidents = new();

		[ObservableProperty]
		private Incident? _selectedIncident;

		[ObservableProperty]
		private string _searchTerm = string.Empty;

		[ObservableProperty]
		private bool _isEditMode;

		// Для редактирования
		[ObservableProperty]
		private DateTime _date = DateTime.Today;

		[ObservableProperty]
		private string _description = string.Empty;

		[ObservableProperty]
		private decimal _damageCost;

		[ObservableProperty]
		private int _vehicleId;

		[ObservableProperty]
		private int _driverId;

		[ObservableProperty]
		private ObservableCollection<Vehicle> _vehicles = new();

		[ObservableProperty]
		private ObservableCollection<Driver> _drivers = new();

		public IncidentViewModel(IIncidentRepository incidentRepository, IVehicleRepository vehicleRepository, IDriverRepository driverRepository, INotificationService notificationService)
		{
			_incidentRepository = incidentRepository;
			_vehicleRepository = vehicleRepository;
			_driverRepository = driverRepository;
			_notificationService = notificationService;
			
			// Подписываемся на уведомления об изменениях
			_notificationService.DataChanged += OnDataChanged;
			
			_ = LoadIncidentsAsync();
			_ = LoadVehiclesAsync();
			_ = LoadDriversAsync();
		}

		[RelayCommand]
		private async Task LoadIncidentsAsync()
		{
			IsBusy = true;
			try
			{
				var incidents = await _incidentRepository.SearchAsync(SearchTerm);
				Incidents.Clear();
				foreach (var incident in incidents)
				{
					Incidents.Add(incident);
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
		private async Task LoadDriversAsync()
		{
			try
			{
				var drivers = await _driverRepository.GetActiveDriversAsync();
				Drivers.Clear();
				foreach (var driver in drivers)
				{
					Drivers.Add(driver);
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show($"Ошибка при загрузке водителей: {ex.Message}", "Ошибка", 
					MessageBoxButton.OK, MessageBoxImage.Error);
			}
		}

		[RelayCommand]
		private async Task SearchAsync()
		{
			await LoadIncidentsAsync();
		}

		[RelayCommand]
		private void AddNew()
		{
			IsEditMode = true;
			SelectedIncident = null;
			Date = DateTime.Today;
			Description = string.Empty;
			DamageCost = 0;
			VehicleId = 0;
			DriverId = 0;
		}

		[RelayCommand]
		private void Edit()
		{
			if (SelectedIncident == null)
			{
				MessageBox.Show("Выберите инцидент для редактирования!", "Предупреждение", 
					MessageBoxButton.OK, MessageBoxImage.Information);
				return;
			}

			IsEditMode = true;
			Date = SelectedIncident.Date;
			Description = SelectedIncident.Description;
			DamageCost = SelectedIncident.DamageCost;
			VehicleId = SelectedIncident.VehicleId;
			DriverId = SelectedIncident.DriverId;
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

			if (DriverId == 0)
			{
				MessageBox.Show("Выберите водителя!", "Ошибка валидации", 
					MessageBoxButton.OK, MessageBoxImage.Warning);
				return;
			}

			if (string.IsNullOrWhiteSpace(Description))
			{
				MessageBox.Show("Поле 'Описание' обязательно для заполнения!", "Ошибка валидации", 
					MessageBoxButton.OK, MessageBoxImage.Warning);
				return;
			}

			if (DamageCost < 0)
			{
				MessageBox.Show("Стоимость ущерба не может быть отрицательной!", "Ошибка валидации", 
					MessageBoxButton.OK, MessageBoxImage.Warning);
				return;
			}

			IsBusy = true;
			try
			{
				if (SelectedIncident == null)
				{
					// Добавление
					var newIncident = new Incident
					{
						Date = Date,
						Description = Description,
						DamageCost = DamageCost,
						VehicleId = VehicleId,
						DriverId = DriverId
					};

					await _incidentRepository.AddAsync(newIncident);
				}
				else
				{
					// Редактирование
					SelectedIncident.Date = Date;
					SelectedIncident.Description = Description;
					SelectedIncident.DamageCost = DamageCost;
					SelectedIncident.VehicleId = VehicleId;
					SelectedIncident.DriverId = DriverId;

					await _incidentRepository.UpdateAsync(SelectedIncident);
				}

				await _incidentRepository.SaveChangesAsync();
				IsEditMode = false;
				await LoadIncidentsAsync();
				
				var message = SelectedIncident == null ? "Инцидент успешно добавлен!" : "Инцидент успешно обновлен!";
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
			SelectedIncident = null;
		}

		[RelayCommand]
		private async Task DeleteAsync()
		{
			if (SelectedIncident == null)
			{
				MessageBox.Show("Выберите инцидент для удаления!", "Предупреждение", 
					MessageBoxButton.OK, MessageBoxImage.Information);
				return;
			}

			var result = MessageBox.Show($"Вы уверены, что хотите удалить инцидент?", 
				"Подтверждение удаления", MessageBoxButton.YesNo, MessageBoxImage.Question);
			
			if (result != MessageBoxResult.Yes) return;

			IsBusy = true;
			try
			{
				await _incidentRepository.DeleteAsync(SelectedIncident.Id);
				await _incidentRepository.SaveChangesAsync();
				await LoadIncidentsAsync();
				
				MessageBox.Show("Инцидент успешно удален!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
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
			await LoadDriversAsync();
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
