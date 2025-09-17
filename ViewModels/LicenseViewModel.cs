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
	public partial class LicenseViewModel : BaseViewModel, IActivatableViewModel
	{
		private readonly ILicenseRepository _licenseRepository;
		private readonly IDriverRepository _driverRepository;
		private readonly INotificationService _notificationService;

		[ObservableProperty]
		private ObservableCollection<License> _licenses = new();

		[ObservableProperty]
		private License? _selectedLicense;

		[ObservableProperty]
		private string _searchTerm = string.Empty;

		[ObservableProperty]
		private bool _isEditMode;

		// Для редактирования
		[ObservableProperty]
		private int _driverId;

		[ObservableProperty]
		private string _number = string.Empty;

		[ObservableProperty]
		private string _category = string.Empty;

		[ObservableProperty]
		private DateTime _issueDate = DateTime.Today;

		[ObservableProperty]
		private DateTime _expiryDate = DateTime.Today.AddYears(1);

		[ObservableProperty]
		private ObservableCollection<Driver> _drivers = new();

		public LicenseViewModel(ILicenseRepository licenseRepository, IDriverRepository driverRepository, INotificationService notificationService)
		{
			_licenseRepository = licenseRepository;
			_driverRepository = driverRepository;
			_notificationService = notificationService;
			
			// Подписываемся на уведомления об изменениях
			_notificationService.DataChanged += OnDataChanged;
			
			_ = LoadLicensesAsync();
			_ = LoadDriversAsync();
		}

		[RelayCommand]
		private async Task LoadLicensesAsync()
		{
			IsBusy = true;
			try
			{
				var licenses = await _licenseRepository.SearchAsync(SearchTerm);
				Licenses.Clear();
				foreach (var license in licenses)
				{
					Licenses.Add(license);
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
		private async Task SearchAsync()
		{
			await LoadLicensesAsync();
		}

		[RelayCommand]
		private void AddNew()
		{
			IsEditMode = true;
			SelectedLicense = null;
			DriverId = 0;
			Number = string.Empty;
			Category = string.Empty;
			IssueDate = DateTime.Today;
			ExpiryDate = DateTime.Today.AddYears(1);
		}

		[RelayCommand]
		private void Edit()
		{
			if (SelectedLicense == null)
			{
				MessageBox.Show("Выберите лицензию для редактирования!", "Предупреждение", 
					MessageBoxButton.OK, MessageBoxImage.Information);
				return;
			}

			IsEditMode = true;
			DriverId = SelectedLicense.DriverId;
			Number = SelectedLicense.Number;
			Category = SelectedLicense.Category;
			IssueDate = SelectedLicense.IssueDate;
			ExpiryDate = SelectedLicense.ExpiryDate;
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

			if (string.IsNullOrWhiteSpace(Number))
			{
				MessageBox.Show("Поле 'Номер лицензии' обязательно для заполнения!", "Ошибка валидации", 
					MessageBoxButton.OK, MessageBoxImage.Warning);
				return;
			}

			if (string.IsNullOrWhiteSpace(Category))
			{
				MessageBox.Show("Поле 'Категория' обязательно для заполнения!", "Ошибка валидации", 
					MessageBoxButton.OK, MessageBoxImage.Warning);
				return;
			}

			if (ExpiryDate <= IssueDate)
			{
				MessageBox.Show("Дата окончания должна быть позже даты выдачи!", "Ошибка валидации", 
					MessageBoxButton.OK, MessageBoxImage.Warning);
				return;
			}

			IsBusy = true;
			try
			{
				if (SelectedLicense == null)
				{
					// Добавление
					var newLicense = new License
					{
						DriverId = DriverId,
						Number = Number,
						Category = Category,
						IssueDate = IssueDate,
						ExpiryDate = ExpiryDate
					};

					await _licenseRepository.AddAsync(newLicense);
				}
				else
				{
					// Редактирование
					SelectedLicense.DriverId = DriverId;
					SelectedLicense.Number = Number;
					SelectedLicense.Category = Category;
					SelectedLicense.IssueDate = IssueDate;
					SelectedLicense.ExpiryDate = ExpiryDate;

					await _licenseRepository.UpdateAsync(SelectedLicense);
				}

				await _licenseRepository.SaveChangesAsync();
				IsEditMode = false;
				await LoadLicensesAsync();
				
				var message = SelectedLicense == null ? "Лицензия успешно добавлена!" : "Лицензия успешно обновлена!";
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
			SelectedLicense = null;
		}

		[RelayCommand]
		private async Task DeleteAsync()
		{
			if (SelectedLicense == null)
			{
				MessageBox.Show("Выберите лицензию для удаления!", "Предупреждение", 
					MessageBoxButton.OK, MessageBoxImage.Information);
				return;
			}

			var result = MessageBox.Show($"Вы уверены, что хотите удалить лицензию '{SelectedLicense.Number}'?", 
				"Подтверждение удаления", MessageBoxButton.YesNo, MessageBoxImage.Question);
			
			if (result != MessageBoxResult.Yes) return;

			IsBusy = true;
			try
			{
				await _licenseRepository.DeleteAsync(SelectedLicense.Id);
				await _licenseRepository.SaveChangesAsync();
				await LoadLicensesAsync();
				
				MessageBox.Show("Лицензия успешно удалена!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
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
			if (entityType == "Driver")
			{
				await LoadDriversAsync();
			}
		}

		public async Task OnActivatedAsync()
		{
			await LoadDriversAsync();
		}
	}
}
