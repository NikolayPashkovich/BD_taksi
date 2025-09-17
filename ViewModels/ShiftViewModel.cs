using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using BD_taksi.Entities;
using BD_taksi.Services;
using System.Collections.ObjectModel;
using System.Windows;

namespace BD_taksi.ViewModels
{
	public partial class ShiftViewModel : BaseViewModel, IActivatableViewModel
	{
		private readonly IShiftRepository _shiftRepository;
		private readonly INotificationService _notificationService;
		private readonly IDriverRepository _driverRepository;
		private readonly IVehicleRepository _vehicleRepository;

		[ObservableProperty]
		private ObservableCollection<Shift> _shifts = new();

		[ObservableProperty]
		private Shift? _selectedShift;

		[ObservableProperty]
		private string _searchTerm = string.Empty;

		[ObservableProperty]
		private bool _isEditMode;

		// Для редактирования
		[ObservableProperty]
		private DateTime _start = DateTime.Now;

		[ObservableProperty]
		private DateTime? _end;

		[ObservableProperty]
		private double _distanceKm;

		[ObservableProperty]
		private int _driverId;

		[ObservableProperty]
		private int _vehicleId;

		[ObservableProperty]
		private ObservableCollection<Driver> _drivers = new();

		[ObservableProperty]
		private ObservableCollection<Vehicle> _vehicles = new();

		public ShiftViewModel(IShiftRepository shiftRepository, IDriverRepository driverRepository, IVehicleRepository vehicleRepository, INotificationService notificationService)
		{
			_shiftRepository = shiftRepository;
			_driverRepository = driverRepository;
			_vehicleRepository = vehicleRepository;
			_notificationService = notificationService;
			
			// Подписываемся на уведомления об изменениях
			_notificationService.DataChanged += OnDataChanged;
			
			_ = LoadShiftsAsync();
			_ = LoadDriversAsync();
			_ = LoadVehiclesAsync();
		}

		[RelayCommand]
		private async Task LoadShiftsAsync()
		{
			IsBusy = true;
			try
			{
				var shifts = await _shiftRepository.SearchAsync(SearchTerm);
				Shifts.Clear();
				foreach (var shift in shifts)
				{
					Shifts.Add(shift);
				}
			}
			finally
			{
				IsBusy = false;
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
			await LoadShiftsAsync();
		}

		[RelayCommand]
		private void AddNew()
		{
			IsEditMode = true;
			SelectedShift = null;
			Start = DateTime.Now;
			End = null;
			DistanceKm = 0;
			DriverId = 0;
			VehicleId = 0;
		}

		[RelayCommand]
		private void Edit()
		{
			if (SelectedShift == null)
			{
				MessageBox.Show("Выберите смену для редактирования!", "Предупреждение", 
					MessageBoxButton.OK, MessageBoxImage.Information);
				return;
			}

			IsEditMode = true;
			Start = SelectedShift.Start;
			End = SelectedShift.End;
			DistanceKm = SelectedShift.DistanceKm;
			DriverId = SelectedShift.DriverId;
			VehicleId = SelectedShift.VehicleId;
		}

		[RelayCommand]
		private async Task SaveAsync()
		{
			if (DriverId == 0)
			{
				MessageBox.Show("Выберите водителя!", "Ошибка валидации", 
					MessageBoxButton.OK, MessageBoxImage.Warning);
				return;
			}

			if (VehicleId == 0)
			{
				MessageBox.Show("Выберите автомобиль!", "Ошибка валидации", 
					MessageBoxButton.OK, MessageBoxImage.Warning);
				return;
			}

			if (DistanceKm < 0)
			{
				MessageBox.Show("Расстояние не может быть отрицательным!", "Ошибка валидации", 
					MessageBoxButton.OK, MessageBoxImage.Warning);
				return;
			}

			if (End.HasValue && End <= Start)
			{
				MessageBox.Show("Время окончания должно быть позже времени начала!", "Ошибка валидации", 
					MessageBoxButton.OK, MessageBoxImage.Warning);
				return;
			}

			IsBusy = true;
			try
			{
				if (SelectedShift == null)
				{
					// Добавление
					var newShift = new Shift
					{
						Start = Start,
						End = End,
						DistanceKm = DistanceKm,
						DriverId = DriverId,
						VehicleId = VehicleId
					};

					await _shiftRepository.AddAsync(newShift);
				}
				else
				{
					// Редактирование
					SelectedShift.Start = Start;
					SelectedShift.End = End;
					SelectedShift.DistanceKm = DistanceKm;
					SelectedShift.DriverId = DriverId;
					SelectedShift.VehicleId = VehicleId;

					await _shiftRepository.UpdateAsync(SelectedShift);
				}

				await _shiftRepository.SaveChangesAsync();
				IsEditMode = false;
				await LoadShiftsAsync();
				
				var message = SelectedShift == null ? "Смена успешно добавлена!" : "Смена успешно обновлена!";
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
			SelectedShift = null;
		}

		[RelayCommand]
		private async Task DeleteAsync()
		{
			if (SelectedShift == null)
			{
				MessageBox.Show("Выберите смену для удаления!", "Предупреждение", 
					MessageBoxButton.OK, MessageBoxImage.Information);
				return;
			}

			var result = MessageBox.Show($"Вы уверены, что хотите удалить смену?", 
				"Подтверждение удаления", MessageBoxButton.YesNo, MessageBoxImage.Question);
			
			if (result != MessageBoxResult.Yes) return;

			IsBusy = true;
			try
			{
				await _shiftRepository.DeleteAsync(SelectedShift.Id);
				await _shiftRepository.SaveChangesAsync();
				await LoadShiftsAsync();
				
				MessageBox.Show("Смена успешно удалена!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
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
