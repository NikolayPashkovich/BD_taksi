using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using BD_taksi.Entities;
using BD_taksi.Services;
using System.Collections.ObjectModel;
using System.Windows;

namespace BD_taksi.ViewModels
{
	public partial class RideViewModel : BaseViewModel, IActivatableViewModel
	{
		private readonly IRideRepository _rideRepository;
		private readonly IDriverRepository _driverRepository;
		private readonly ICustomerRepository _customerRepository;
		private readonly IVehicleRepository _vehicleRepository;
		private readonly ITariffRepository _tariffRepository;
		private readonly IPromotionRepository _promotionRepository;
		private readonly IRideStatusRepository _rideStatusRepository;

		[ObservableProperty]
		private ObservableCollection<Ride> _rides = new();

		[ObservableProperty]
		private Ride? _selectedRide;

		[ObservableProperty]
		private string _searchTerm = string.Empty;

		[ObservableProperty]
		private bool _isEditMode;

		// Для редактирования
		[ObservableProperty]
		private DateTime _startTime = DateTime.Now;

		[ObservableProperty]
		private DateTime? _endTime;

		[ObservableProperty]
		private double _distanceKm;

		[ObservableProperty]
		private decimal _cost;

		[ObservableProperty]
		private string _comment = string.Empty;

		[ObservableProperty]
		private int _driverId;

		[ObservableProperty]
		private int _vehicleId;

		[ObservableProperty]
		private int _customerId;

		[ObservableProperty]
		private int _statusId;

		[ObservableProperty]
		private int _tariffId;

		[ObservableProperty]
		private int? _promotionId;

		[ObservableProperty]
		private ObservableCollection<Driver> _drivers = new();

		[ObservableProperty]
		private ObservableCollection<Customer> _customers = new();

		[ObservableProperty]
		private ObservableCollection<Vehicle> _vehicles = new();

		[ObservableProperty]
		private ObservableCollection<Tariff> _tariffs = new();

		[ObservableProperty]
		private ObservableCollection<Promotion> _promotions = new();

		[ObservableProperty]
		private ObservableCollection<RideStatus> _rideStatuses = new();

		public RideViewModel(IRideRepository rideRepository, IDriverRepository driverRepository, 
			ICustomerRepository customerRepository, IVehicleRepository vehicleRepository,
			ITariffRepository tariffRepository, IPromotionRepository promotionRepository,
			IRideStatusRepository rideStatusRepository)
		{
			_rideRepository = rideRepository;
			_driverRepository = driverRepository;
			_customerRepository = customerRepository;
			_vehicleRepository = vehicleRepository;
			_tariffRepository = tariffRepository;
			_promotionRepository = promotionRepository;
			_rideStatusRepository = rideStatusRepository;
			_ = LoadRidesAsync();
			_ = LoadDriversAsync();
			_ = LoadCustomersAsync();
			_ = LoadVehiclesAsync();
			_ = LoadTariffsAsync();
			_ = LoadPromotionsAsync();
			_ = LoadRideStatusesAsync();
		}

		[RelayCommand]
		private async Task LoadRidesAsync()
		{
			IsBusy = true;
			try
			{
				var rides = await _rideRepository.SearchAsync(SearchTerm);
				Rides.Clear();
				foreach (var ride in rides)
				{
					Rides.Add(ride);
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
		private async Task LoadCustomersAsync()
		{
			try
			{
				var customers = await _customerRepository.GetAllAsync();
				Customers.Clear();
				foreach (var customer in customers)
				{
					Customers.Add(customer);
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show($"Ошибка при загрузке клиентов: {ex.Message}", "Ошибка", 
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
		private async Task LoadTariffsAsync()
		{
			try
			{
				var tariffs = await _tariffRepository.GetAllAsync();
				Tariffs.Clear();
				foreach (var tariff in tariffs)
				{
					Tariffs.Add(tariff);
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show($"Ошибка при загрузке тарифов: {ex.Message}", "Ошибка", 
					MessageBoxButton.OK, MessageBoxImage.Error);
			}
		}

		[RelayCommand]
		private async Task LoadPromotionsAsync()
		{
			try
			{
				var promotions = await _promotionRepository.GetActivePromotionsAsync();
				Promotions.Clear();
				foreach (var promotion in promotions)
				{
					Promotions.Add(promotion);
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show($"Ошибка при загрузке акций: {ex.Message}", "Ошибка", 
					MessageBoxButton.OK, MessageBoxImage.Error);
			}
		}

		[RelayCommand]
		private async Task LoadRideStatusesAsync()
		{
			try
			{
				var rideStatuses = await _rideStatusRepository.GetAllAsync();
				RideStatuses.Clear();
				foreach (var rideStatus in rideStatuses)
				{
					RideStatuses.Add(rideStatus);
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show($"Ошибка при загрузке статусов: {ex.Message}", "Ошибка", 
					MessageBoxButton.OK, MessageBoxImage.Error);
			}
		}

		[RelayCommand]
		private async Task SearchAsync()
		{
			await LoadRidesAsync();
		}

		[RelayCommand]
		private void AddNew()
		{
			IsEditMode = true;
			SelectedRide = null;
			StartTime = DateTime.Now;
			EndTime = null;
			DistanceKm = 0;
			Cost = 0;
			Comment = string.Empty;
			DriverId = 0;
			VehicleId = 0;
			CustomerId = 0;
			StatusId = 0;
			TariffId = 0;
			PromotionId = null;
		}

		[RelayCommand]
		private void Edit()
		{
			if (SelectedRide == null)
			{
				MessageBox.Show("Выберите поездку для редактирования!", "Предупреждение", 
					MessageBoxButton.OK, MessageBoxImage.Information);
				return;
			}

			IsEditMode = true;
			StartTime = SelectedRide.StartTime;
			EndTime = SelectedRide.EndTime;
			DistanceKm = SelectedRide.DistanceKm;
			Cost = SelectedRide.Cost;
			Comment = SelectedRide.Comment ?? string.Empty;
			DriverId = SelectedRide.DriverId;
			VehicleId = SelectedRide.VehicleId;
			CustomerId = SelectedRide.CustomerId;
			StatusId = SelectedRide.StatusId;
			TariffId = SelectedRide.TariffId;
			PromotionId = SelectedRide.PromotionId;
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

			if (CustomerId == 0)
			{
				MessageBox.Show("Выберите клиента!", "Ошибка валидации", 
					MessageBoxButton.OK, MessageBoxImage.Warning);
				return;
			}

			if (StatusId == 0)
			{
				MessageBox.Show("Выберите статус!", "Ошибка валидации", 
					MessageBoxButton.OK, MessageBoxImage.Warning);
				return;
			}

			if (TariffId == 0)
			{
				MessageBox.Show("Выберите тариф!", "Ошибка валидации", 
					MessageBoxButton.OK, MessageBoxImage.Warning);
				return;
			}

			if (DistanceKm < 0)
			{
				MessageBox.Show("Расстояние не может быть отрицательным!", "Ошибка валидации", 
					MessageBoxButton.OK, MessageBoxImage.Warning);
				return;
			}

			if (Cost < 0)
			{
				MessageBox.Show("Стоимость не может быть отрицательной!", "Ошибка валидации", 
					MessageBoxButton.OK, MessageBoxImage.Warning);
				return;
			}

			IsBusy = true;
			try
			{
				if (SelectedRide == null)
				{
					// Добавление
					var newRide = new Ride
					{
						StartTime = StartTime,
						EndTime = EndTime,
						DistanceKm = DistanceKm,
						Cost = Cost,
						Comment = Comment,
						DriverId = DriverId,
						VehicleId = VehicleId,
						CustomerId = CustomerId,
						StatusId = StatusId,
						TariffId = TariffId,
						PromotionId = PromotionId
					};

					await _rideRepository.AddAsync(newRide);
				}
				else
				{
					// Редактирование
					SelectedRide.StartTime = StartTime;
					SelectedRide.EndTime = EndTime;
					SelectedRide.DistanceKm = DistanceKm;
					SelectedRide.Cost = Cost;
					SelectedRide.Comment = Comment;
					SelectedRide.DriverId = DriverId;
					SelectedRide.VehicleId = VehicleId;
					SelectedRide.CustomerId = CustomerId;
					SelectedRide.StatusId = StatusId;
					SelectedRide.TariffId = TariffId;
					SelectedRide.PromotionId = PromotionId;

					await _rideRepository.UpdateAsync(SelectedRide);
				}

				await _rideRepository.SaveChangesAsync();
				IsEditMode = false;
				await LoadRidesAsync();
				
				var message = SelectedRide == null ? "Поездка успешно добавлена!" : "Поездка успешно обновлена!";
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
			SelectedRide = null;
		}

		[RelayCommand]
		private async Task DeleteAsync()
		{
			if (SelectedRide == null)
			{
				MessageBox.Show("Выберите поездку для удаления!", "Предупреждение", 
					MessageBoxButton.OK, MessageBoxImage.Information);
				return;
			}

			var result = MessageBox.Show($"Вы уверены, что хотите удалить поездку?", 
				"Подтверждение удаления", MessageBoxButton.YesNo, MessageBoxImage.Question);
			
			if (result != MessageBoxResult.Yes) return;

			IsBusy = true;
			try
			{
				await _rideRepository.DeleteAsync(SelectedRide.Id);
				await _rideRepository.SaveChangesAsync();
				await LoadRidesAsync();
				
				MessageBox.Show("Поездка успешно удалена!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
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
			await LoadCustomersAsync();
			await LoadVehiclesAsync();
			await LoadTariffsAsync();
			await LoadPromotionsAsync();
			await LoadRideStatusesAsync();
		}
	}
}
