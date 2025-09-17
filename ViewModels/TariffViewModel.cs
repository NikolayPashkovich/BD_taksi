using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using BD_taksi.Entities;
using BD_taksi.Services;
using System.Collections.ObjectModel;
using System.Windows;

namespace BD_taksi.ViewModels
{
	public partial class TariffViewModel : BaseViewModel
	{
		private readonly ITariffRepository _tariffRepository;

		[ObservableProperty]
		private ObservableCollection<Tariff> _tariffs = new();

		[ObservableProperty]
		private Tariff? _selectedTariff;

		[ObservableProperty]
		private string _searchTerm = string.Empty;

		[ObservableProperty]
		private bool _isEditMode;

		// Для редактирования
		[ObservableProperty]
		private string _name = string.Empty;

		[ObservableProperty]
		private decimal _baseFare;

		[ObservableProperty]
		private decimal _pricePerKm;

		[ObservableProperty]
		private decimal _pricePerMinute;

		public TariffViewModel(ITariffRepository tariffRepository)
		{
			_tariffRepository = tariffRepository;
			_ = LoadTariffsAsync();
		}

		[RelayCommand]
		private async Task LoadTariffsAsync()
		{
			IsBusy = true;
			try
			{
				var tariffs = await _tariffRepository.SearchAsync(SearchTerm);
				Tariffs.Clear();
				foreach (var tariff in tariffs)
				{
					Tariffs.Add(tariff);
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
			await LoadTariffsAsync();
		}

		[RelayCommand]
		private void AddNew()
		{
			IsEditMode = true;
			SelectedTariff = null;
			Name = string.Empty;
			BaseFare = 0;
			PricePerKm = 0;
			PricePerMinute = 0;
		}

		[RelayCommand]
		private void Edit()
		{
			if (SelectedTariff == null)
			{
				MessageBox.Show("Выберите тариф для редактирования!", "Предупреждение", 
					MessageBoxButton.OK, MessageBoxImage.Information);
				return;
			}

			IsEditMode = true;
			Name = SelectedTariff.Name;
			BaseFare = SelectedTariff.BaseFare;
			PricePerKm = SelectedTariff.PricePerKm;
			PricePerMinute = SelectedTariff.PricePerMinute;
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

			if (BaseFare < 0)
			{
				MessageBox.Show("Базовая стоимость не может быть отрицательной!", "Ошибка валидации", 
					MessageBoxButton.OK, MessageBoxImage.Warning);
				return;
			}

			if (PricePerKm < 0)
			{
				MessageBox.Show("Стоимость за км не может быть отрицательной!", "Ошибка валидации", 
					MessageBoxButton.OK, MessageBoxImage.Warning);
				return;
			}

			if (PricePerMinute < 0)
			{
				MessageBox.Show("Стоимость за минуту не может быть отрицательной!", "Ошибка валидации", 
					MessageBoxButton.OK, MessageBoxImage.Warning);
				return;
			}

			IsBusy = true;
			try
			{
				if (SelectedTariff == null)
				{
					// Добавление
					var newTariff = new Tariff
					{
						Name = Name,
						BaseFare = BaseFare,
						PricePerKm = PricePerKm,
						PricePerMinute = PricePerMinute
					};

					await _tariffRepository.AddAsync(newTariff);
				}
				else
				{
					// Редактирование
					SelectedTariff.Name = Name;
					SelectedTariff.BaseFare = BaseFare;
					SelectedTariff.PricePerKm = PricePerKm;
					SelectedTariff.PricePerMinute = PricePerMinute;

					await _tariffRepository.UpdateAsync(SelectedTariff);
				}

				await _tariffRepository.SaveChangesAsync();
				IsEditMode = false;
				await LoadTariffsAsync();
				
				var message = SelectedTariff == null ? "Тариф успешно добавлен!" : "Тариф успешно обновлен!";
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
			SelectedTariff = null;
		}

		[RelayCommand]
		private async Task DeleteAsync()
		{
			if (SelectedTariff == null)
			{
				MessageBox.Show("Выберите тариф для удаления!", "Предупреждение", 
					MessageBoxButton.OK, MessageBoxImage.Information);
				return;
			}

			var result = MessageBox.Show($"Вы уверены, что хотите удалить тариф '{SelectedTariff.Name}'?", 
				"Подтверждение удаления", MessageBoxButton.YesNo, MessageBoxImage.Question);
			
			if (result != MessageBoxResult.Yes) return;

			IsBusy = true;
			try
			{
				await _tariffRepository.DeleteAsync(SelectedTariff.Id);
				await _tariffRepository.SaveChangesAsync();
				await LoadTariffsAsync();
				
				MessageBox.Show("Тариф успешно удален!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
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
