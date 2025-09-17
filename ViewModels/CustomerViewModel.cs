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
	public partial class CustomerViewModel : BaseViewModel
	{
		private readonly ICustomerRepository _customerRepository;

		[ObservableProperty]
		private ObservableCollection<Customer> _customers = new();

		[ObservableProperty]
		private Customer? _selectedCustomer;

		[ObservableProperty]
		private string _searchTerm = string.Empty;

		[ObservableProperty]
		private bool _isEditMode;

		// Для редактирования
		[ObservableProperty]
		private string _name = string.Empty;

		[ObservableProperty]
		private string _phone = string.Empty;

		[ObservableProperty]
		private string _email = string.Empty;

		[ObservableProperty]
		private double _rating = 0;

		public CustomerViewModel(ICustomerRepository customerRepository)
		{
			_customerRepository = customerRepository;
			_ = LoadCustomersAsync();
		}

		[RelayCommand]
		private async Task LoadCustomersAsync()
		{
			IsBusy = true;
			try
			{
				var customers = await _customerRepository.SearchAsync(SearchTerm);
				Customers.Clear();
				foreach (var customer in customers)
				{
					Customers.Add(customer);
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
			await LoadCustomersAsync();
		}

		[RelayCommand]
		private void AddNew()
		{
			IsEditMode = true;
			SelectedCustomer = null;
			Name = string.Empty;
			Phone = string.Empty;
			Email = string.Empty;
			Rating = 0;
		}

		[RelayCommand]
		private void Edit()
		{
			if (SelectedCustomer == null)
			{
				MessageBox.Show("Выберите клиента для редактирования!", "Предупреждение", 
					MessageBoxButton.OK, MessageBoxImage.Information);
				return;
			}

			IsEditMode = true;
			Name = SelectedCustomer.Name;
			Phone = SelectedCustomer.Phone;
			Email = SelectedCustomer.Email ?? string.Empty;
			Rating = SelectedCustomer.Rating;
		}

		[RelayCommand]
		private async Task SaveAsync()
		{
			if (string.IsNullOrWhiteSpace(Name))
			{
				MessageBox.Show("Поле 'Имя' обязательно для заполнения!", "Ошибка валидации", 
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

			if (!string.IsNullOrWhiteSpace(Email) && !IsValidEmail(Email))
			{
				MessageBox.Show("Неверный формат email!", "Ошибка валидации", 
					MessageBoxButton.OK, MessageBoxImage.Warning);
				return;
			}

			IsBusy = true;
			try
			{
				if (SelectedCustomer == null)
				{
					// Добавление
					var newCustomer = new Customer
					{
						Name = Name,
						Phone = Phone,
						Email = string.IsNullOrWhiteSpace(Email) ? null : Email,
						Rating = Rating
					};

					await _customerRepository.AddAsync(newCustomer);
				}
				else
				{
					// Редактирование
					SelectedCustomer.Name = Name;
					SelectedCustomer.Phone = Phone;
					SelectedCustomer.Email = string.IsNullOrWhiteSpace(Email) ? null : Email;
					SelectedCustomer.Rating = Rating;

					await _customerRepository.UpdateAsync(SelectedCustomer);
				}

				await _customerRepository.SaveChangesAsync();
				IsEditMode = false;
				await LoadCustomersAsync();
				
				var message = SelectedCustomer == null ? "Клиент успешно добавлен!" : "Клиент успешно обновлен!";
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
			SelectedCustomer = null;
		}

		[RelayCommand]
		private async Task DeleteAsync()
		{
			if (SelectedCustomer == null)
			{
				MessageBox.Show("Выберите клиента для удаления!", "Предупреждение", 
					MessageBoxButton.OK, MessageBoxImage.Information);
				return;
			}

			var result = MessageBox.Show($"Вы уверены, что хотите удалить клиента '{SelectedCustomer.Name}'?", 
				"Подтверждение удаления", MessageBoxButton.YesNo, MessageBoxImage.Question);
			
			if (result != MessageBoxResult.Yes) return;

			IsBusy = true;
			try
			{
				await _customerRepository.DeleteAsync(SelectedCustomer.Id);
				await _customerRepository.SaveChangesAsync();
				await LoadCustomersAsync();
				
				MessageBox.Show("Клиент успешно удален!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
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
			var regex = new Regex(@"^[\d\s\(\)\-\+]+$");
			return regex.IsMatch(phone) && phone.Length >= 10;
		}

		private bool IsValidEmail(string email)
		{
			try
			{
				var addr = new System.Net.Mail.MailAddress(email);
				return addr.Address == email;
			}
			catch
			{
				return false;
			}
		}
	}
} 