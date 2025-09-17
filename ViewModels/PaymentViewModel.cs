using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using BD_taksi.Entities;
using BD_taksi.Services;
using System.Collections.ObjectModel;
using System.Windows;

namespace BD_taksi.ViewModels
{
	public partial class PaymentViewModel : BaseViewModel
	{
		private readonly IPaymentRepository _paymentRepository;
		private readonly IRideRepository _rideRepository;
		private readonly ICustomerRepository _customerRepository;
		private readonly IPaymentMethodRepository _paymentMethodRepository;

		[ObservableProperty]
		private ObservableCollection<Payment> _payments = new();

		[ObservableProperty]
		private Payment? _selectedPayment;

		[ObservableProperty]
		private string _searchTerm = string.Empty;

		[ObservableProperty]
		private bool _isEditMode;

		// Для редактирования
		[ObservableProperty]
		private decimal _amount;

		[ObservableProperty]
		private DateTime _date = DateTime.Now;

		[ObservableProperty]
		private PaymentStatus _status = PaymentStatus.Pending;

		[ObservableProperty]
		private int _rideId;

		[ObservableProperty]
		private int _customerId;

		[ObservableProperty]
		private int _methodId;

		[ObservableProperty]
		private ObservableCollection<Ride> _rides = new();

		[ObservableProperty]
		private ObservableCollection<Customer> _customers = new();

		[ObservableProperty]
		private ObservableCollection<PaymentMethod> _paymentMethods = new();

		public PaymentViewModel(IPaymentRepository paymentRepository, IRideRepository rideRepository,
			ICustomerRepository customerRepository, IPaymentMethodRepository paymentMethodRepository)
		{
			_paymentRepository = paymentRepository;
			_rideRepository = rideRepository;
			_customerRepository = customerRepository;
			_paymentMethodRepository = paymentMethodRepository;
			_ = LoadPaymentsAsync();
			_ = LoadRidesAsync();
			_ = LoadCustomersAsync();
			_ = LoadPaymentMethodsAsync();
		}

		[RelayCommand]
		private async Task LoadPaymentsAsync()
		{
			IsBusy = true;
			try
			{
				var payments = await _paymentRepository.SearchAsync(SearchTerm);
				Payments.Clear();
				foreach (var payment in payments)
				{
					Payments.Add(payment);
				}
			}
			finally
			{
				IsBusy = false;
			}
		}

		[RelayCommand]
		private async Task LoadRidesAsync()
		{
			try
			{
				var rides = await _rideRepository.GetAllAsync();
				Rides.Clear();
				foreach (var ride in rides)
				{
					Rides.Add(ride);
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show($"Ошибка при загрузке поездок: {ex.Message}", "Ошибка", 
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
		private async Task LoadPaymentMethodsAsync()
		{
			try
			{
				var paymentMethods = await _paymentMethodRepository.GetAllAsync();
				PaymentMethods.Clear();
				foreach (var paymentMethod in paymentMethods)
				{
					PaymentMethods.Add(paymentMethod);
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show($"Ошибка при загрузке способов оплаты: {ex.Message}", "Ошибка", 
					MessageBoxButton.OK, MessageBoxImage.Error);
			}
		}

		[RelayCommand]
		private async Task SearchAsync()
		{
			await LoadPaymentsAsync();
		}

		[RelayCommand]
		private void AddNew()
		{
			IsEditMode = true;
			SelectedPayment = null;
			Amount = 0;
			Date = DateTime.Now;
			Status = PaymentStatus.Pending;
			RideId = 0;
			CustomerId = 0;
			MethodId = 0;
		}

		[RelayCommand]
		private void Edit()
		{
			if (SelectedPayment == null)
			{
				MessageBox.Show("Выберите платеж для редактирования!", "Предупреждение", 
					MessageBoxButton.OK, MessageBoxImage.Information);
				return;
			}

			IsEditMode = true;
			Amount = SelectedPayment.Amount;
			Date = SelectedPayment.Date;
			Status = SelectedPayment.Status;
			RideId = SelectedPayment.RideId;
			CustomerId = SelectedPayment.CustomerId;
			MethodId = SelectedPayment.MethodId;
		}

		[RelayCommand]
		private async Task SaveAsync()
		{
			if (RideId == 0)
			{
				MessageBox.Show("Выберите поездку!", "Ошибка валидации", 
					MessageBoxButton.OK, MessageBoxImage.Warning);
				return;
			}

			if (CustomerId == 0)
			{
				MessageBox.Show("Выберите клиента!", "Ошибка валидации", 
					MessageBoxButton.OK, MessageBoxImage.Warning);
				return;
			}

			if (MethodId == 0)
			{
				MessageBox.Show("Выберите способ оплаты!", "Ошибка валидации", 
					MessageBoxButton.OK, MessageBoxImage.Warning);
				return;
			}

			if (Amount <= 0)
			{
				MessageBox.Show("Сумма должна быть больше нуля!", "Ошибка валидации", 
					MessageBoxButton.OK, MessageBoxImage.Warning);
				return;
			}

			IsBusy = true;
			try
			{
				if (SelectedPayment == null)
				{
					// Добавление
					var newPayment = new Payment
					{
						Amount = Amount,
						Date = Date,
						Status = Status,
						RideId = RideId,
						CustomerId = CustomerId,
						MethodId = MethodId
					};

					await _paymentRepository.AddAsync(newPayment);
				}
				else
				{
					// Редактирование
					SelectedPayment.Amount = Amount;
					SelectedPayment.Date = Date;
					SelectedPayment.Status = Status;
					SelectedPayment.RideId = RideId;
					SelectedPayment.CustomerId = CustomerId;
					SelectedPayment.MethodId = MethodId;

					await _paymentRepository.UpdateAsync(SelectedPayment);
				}

				await _paymentRepository.SaveChangesAsync();
				IsEditMode = false;
				await LoadPaymentsAsync();
				
				var message = SelectedPayment == null ? "Платеж успешно добавлен!" : "Платеж успешно обновлен!";
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
			SelectedPayment = null;
		}

		[RelayCommand]
		private async Task DeleteAsync()
		{
			if (SelectedPayment == null)
			{
				MessageBox.Show("Выберите платеж для удаления!", "Предупреждение", 
					MessageBoxButton.OK, MessageBoxImage.Information);
				return;
			}

			var result = MessageBox.Show($"Вы уверены, что хотите удалить платеж?", 
				"Подтверждение удаления", MessageBoxButton.YesNo, MessageBoxImage.Question);
			
			if (result != MessageBoxResult.Yes) return;

			IsBusy = true;
			try
			{
				await _paymentRepository.DeleteAsync(SelectedPayment.Id);
				await _paymentRepository.SaveChangesAsync();
				await LoadPaymentsAsync();
				
				MessageBox.Show("Платеж успешно удален!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
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
