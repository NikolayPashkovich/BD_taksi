using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using BD_taksi.Entities;
using BD_taksi.Services;
using System.Collections.ObjectModel;
using System.Windows;

namespace BD_taksi.ViewModels
{
	public partial class PaymentMethodViewModel : BaseViewModel
	{
		private readonly IPaymentMethodRepository _paymentMethodRepository;

		[ObservableProperty]
		private ObservableCollection<PaymentMethod> _paymentMethods = new();

		[ObservableProperty]
		private PaymentMethod? _selectedPaymentMethod;

		[ObservableProperty]
		private string _searchTerm = string.Empty;

		[ObservableProperty]
		private bool _isEditMode;

		// Для редактирования
		[ObservableProperty]
		private string _name = string.Empty;

		public PaymentMethodViewModel(IPaymentMethodRepository paymentMethodRepository)
		{
			_paymentMethodRepository = paymentMethodRepository;
			_ = LoadPaymentMethodsAsync();
		}

		[RelayCommand]
		private async Task LoadPaymentMethodsAsync()
		{
			IsBusy = true;
			try
			{
				var paymentMethods = await _paymentMethodRepository.SearchAsync(SearchTerm);
				PaymentMethods.Clear();
				foreach (var paymentMethod in paymentMethods)
				{
					PaymentMethods.Add(paymentMethod);
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
			await LoadPaymentMethodsAsync();
		}

		[RelayCommand]
		private void AddNew()
		{
			IsEditMode = true;
			SelectedPaymentMethod = null;
			Name = string.Empty;
		}

		[RelayCommand]
		private void Edit()
		{
			if (SelectedPaymentMethod == null)
			{
				MessageBox.Show("Выберите способ оплаты для редактирования!", "Предупреждение", 
					MessageBoxButton.OK, MessageBoxImage.Information);
				return;
			}

			IsEditMode = true;
			Name = SelectedPaymentMethod.Name;
		}

		[RelayCommand]
		private async Task SaveAsync()
		{
			if (string.IsNullOrWhiteSpace(Name))
			{
				MessageBox.Show("Поле 'Название' обязательно для заполнения!", "Ошибка валидации", 
					MessageBoxButton.OK, MessageBoxImage.Warning);
				return;
			}

			IsBusy = true;
			try
			{
				if (SelectedPaymentMethod == null)
				{
					// Добавление
					var newPaymentMethod = new PaymentMethod
					{
						Name = Name
					};

					await _paymentMethodRepository.AddAsync(newPaymentMethod);
				}
				else
				{
					// Редактирование
					SelectedPaymentMethod.Name = Name;
					await _paymentMethodRepository.UpdateAsync(SelectedPaymentMethod);
				}

				await _paymentMethodRepository.SaveChangesAsync();
				IsEditMode = false;
				await LoadPaymentMethodsAsync();
				
				var message = SelectedPaymentMethod == null ? "Способ оплаты успешно добавлен!" : "Способ оплаты успешно обновлен!";
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
			SelectedPaymentMethod = null;
		}

		[RelayCommand]
		private async Task DeleteAsync()
		{
			if (SelectedPaymentMethod == null)
			{
				MessageBox.Show("Выберите способ оплаты для удаления!", "Предупреждение", 
					MessageBoxButton.OK, MessageBoxImage.Information);
				return;
			}

			var result = MessageBox.Show($"Вы уверены, что хотите удалить способ оплаты '{SelectedPaymentMethod.Name}'?", 
				"Подтверждение удаления", MessageBoxButton.YesNo, MessageBoxImage.Question);
			
			if (result != MessageBoxResult.Yes) return;

			IsBusy = true;
			try
			{
				await _paymentMethodRepository.DeleteAsync(SelectedPaymentMethod.Id);
				await _paymentMethodRepository.SaveChangesAsync();
				await LoadPaymentMethodsAsync();
				
				MessageBox.Show("Способ оплаты успешно удален!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
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
