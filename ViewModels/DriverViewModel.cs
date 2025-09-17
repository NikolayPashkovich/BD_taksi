using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using BD_taksi.Entities;
using BD_taksi.Services;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;
using System.Windows;
using System;

namespace BD_taksi.ViewModels
{
	public partial class DriverViewModel : BaseViewModel
	{
		private readonly IDriverRepository _driverRepository;
		private readonly INotificationService _notificationService;

		[ObservableProperty]
		private ObservableCollection<Driver> _drivers = new();

		[ObservableProperty]
		private Driver? _selectedDriver;

		[ObservableProperty]
		private string _searchTerm = string.Empty;

		[ObservableProperty]
		private bool _isEditMode;

		// Для редактирования
		[ObservableProperty]
		private string _fullName = string.Empty;

		[ObservableProperty]
		private string _phone = string.Empty;

		[ObservableProperty]
		private DateTime _hireDate = DateTime.Today;

		[ObservableProperty]
		private bool _isActive = true;

		public DriverViewModel(IDriverRepository driverRepository, INotificationService notificationService)
		{
			_driverRepository = driverRepository;
			_notificationService = notificationService;
			_ = LoadDriversAsync();
		}

		[RelayCommand]
		private async Task LoadDriversAsync()
		{
			IsBusy = true;
			try
			{
				var drivers = await _driverRepository.SearchAsync(SearchTerm);
				Drivers.Clear();
				foreach (var driver in drivers)
				{
					Drivers.Add(driver);
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
			await LoadDriversAsync();
		}

		[RelayCommand]
		private void AddNew()
		{
			IsEditMode = true;
			SelectedDriver = null;
			FullName = string.Empty;
			Phone = string.Empty;
			HireDate = DateTime.Today;
			IsActive = true;
		}

		[RelayCommand]
		private void Edit()
		{
			if (SelectedDriver == null)
			{
				MessageBox.Show("Выберите водителя для редактирования!", "Предупреждение", 
					MessageBoxButton.OK, MessageBoxImage.Information);
				return;
			}

			IsEditMode = true;
			FullName = SelectedDriver.FullName;
			Phone = SelectedDriver.Phone;
			HireDate = SelectedDriver.HireDate;
			IsActive = SelectedDriver.IsActive;
		}

		[RelayCommand]
		private async Task SaveAsync()
		{
			if (string.IsNullOrWhiteSpace(FullName))
			{
				MessageBox.Show("Поле 'ФИО' обязательно для заполнения!", "Ошибка валидации", 
					MessageBoxButton.OK, MessageBoxImage.Warning);
				return;
			}

			if (string.IsNullOrWhiteSpace(Phone))
			{
				MessageBox.Show("Поле 'Телефон' обязательно для заполнения!", "Ошибка валидации", 
					MessageBoxButton.OK, MessageBoxImage.Warning);
				return;
			}

			if (!IsValidPhone(Phone))
			{
				MessageBox.Show("Неверный формат телефона! Используйте только цифры, пробелы, скобки, дефисы и +. Минимум 10 символов.", 
					"Ошибка валидации", MessageBoxButton.OK, MessageBoxImage.Warning);
				return;
			}

			IsBusy = true;
			try
			{
				if (SelectedDriver == null)
				{
					// Добавление
					var newDriver = new Driver
					{
						FullName = FullName,
						Phone = Phone,
						HireDate = HireDate,
						IsActive = IsActive
					};

					await _driverRepository.AddAsync(newDriver);
				}
				else
				{
					// Редактирование
					SelectedDriver.FullName = FullName;
					SelectedDriver.Phone = Phone;
					SelectedDriver.HireDate = HireDate;
					SelectedDriver.IsActive = IsActive;

					await _driverRepository.UpdateAsync(SelectedDriver);
				}

				await _driverRepository.SaveChangesAsync();
				IsEditMode = false;
				await LoadDriversAsync();
				
				// Уведомляем другие ViewModels об изменении данных
				_notificationService.NotifyDataChanged("Driver");
				
				var message = SelectedDriver == null ? "Водитель успешно добавлен!" : "Водитель успешно обновлен!";
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
			SelectedDriver = null;
		}

		[RelayCommand]
		private async Task DeleteAsync()
		{
			if (SelectedDriver == null)
			{
				MessageBox.Show("Выберите водителя для удаления!", "Предупреждение", 
					MessageBoxButton.OK, MessageBoxImage.Information);
				return;
			}

			var result = MessageBox.Show($"Вы уверены, что хотите удалить водителя '{SelectedDriver.FullName}'?", 
				"Подтверждение удаления", MessageBoxButton.YesNo, MessageBoxImage.Question);
			
			if (result != MessageBoxResult.Yes) return;

			IsBusy = true;
			try
			{
				await _driverRepository.DeleteAsync(SelectedDriver.Id);
				await _driverRepository.SaveChangesAsync();
				await LoadDriversAsync();
				
				MessageBox.Show("Водитель успешно удален!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
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

		private bool IsValidPhone(string phone)
		{
			// Проверяем, что телефон содержит только цифры, пробелы, скобки, дефисы и +
			var regex = new Regex(@"^[\d\s\(\)\-\+]+$");
			return regex.IsMatch(phone) && phone.Length >= 10;
		}
	}
} 