using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using BD_taksi.Entities;
using BD_taksi.Services;
using System.Collections.ObjectModel;
using System.Windows;

namespace BD_taksi.ViewModels
{
	public partial class PromotionViewModel : BaseViewModel
	{
		private readonly IPromotionRepository _promotionRepository;

		[ObservableProperty]
		private ObservableCollection<Promotion> _promotions = new();

		[ObservableProperty]
		private Promotion? _selectedPromotion;

		[ObservableProperty]
		private string _searchTerm = string.Empty;

		[ObservableProperty]
		private bool _isEditMode;

		// Для редактирования
		[ObservableProperty]
		private string _code = string.Empty;

		[ObservableProperty]
		private PromotionType _type = PromotionType.Percent;

		[ObservableProperty]
		private decimal _value;

		[ObservableProperty]
		private DateTime _startDate = DateTime.Today;

		[ObservableProperty]
		private DateTime _endDate = DateTime.Today.AddMonths(1);

		[ObservableProperty]
		private bool _isActive = true;

		public PromotionViewModel(IPromotionRepository promotionRepository)
		{
			_promotionRepository = promotionRepository;
			_ = LoadPromotionsAsync();
		}

		[RelayCommand]
		private async Task LoadPromotionsAsync()
		{
			IsBusy = true;
			try
			{
				var promotions = await _promotionRepository.SearchAsync(SearchTerm);
				Promotions.Clear();
				foreach (var promotion in promotions)
				{
					Promotions.Add(promotion);
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
			await LoadPromotionsAsync();
		}

		[RelayCommand]
		private void AddNew()
		{
			IsEditMode = true;
			SelectedPromotion = null;
			Code = string.Empty;
			Type = PromotionType.Percent;
			Value = 0;
			StartDate = DateTime.Today;
			EndDate = DateTime.Today.AddMonths(1);
			IsActive = true;
		}

		[RelayCommand]
		private void Edit()
		{
			if (SelectedPromotion == null)
			{
				MessageBox.Show("Выберите акцию для редактирования!", "Предупреждение", 
					MessageBoxButton.OK, MessageBoxImage.Information);
				return;
			}

			IsEditMode = true;
			Code = SelectedPromotion.Code;
			Type = SelectedPromotion.Type;
			Value = SelectedPromotion.Value;
			StartDate = SelectedPromotion.StartDate;
			EndDate = SelectedPromotion.EndDate;
			IsActive = SelectedPromotion.IsActive;
		}

		[RelayCommand]
		private async Task SaveAsync()
		{
			if (string.IsNullOrWhiteSpace(Code))
			{
				MessageBox.Show("Поле 'Код' обязательно для заполнения!", "Ошибка валидации", 
					MessageBoxButton.OK, MessageBoxImage.Warning);
				return;
			}

			if (Value < 0)
			{
				MessageBox.Show("Значение не может быть отрицательным!", "Ошибка валидации", 
					MessageBoxButton.OK, MessageBoxImage.Warning);
				return;
			}

			if (Type == PromotionType.Percent && Value > 100)
			{
				MessageBox.Show("Процент не может быть больше 100%!", "Ошибка валидации", 
					MessageBoxButton.OK, MessageBoxImage.Warning);
				return;
			}

			if (EndDate <= StartDate)
			{
				MessageBox.Show("Дата окончания должна быть позже даты начала!", "Ошибка валидации", 
					MessageBoxButton.OK, MessageBoxImage.Warning);
				return;
			}

			IsBusy = true;
			try
			{
				if (SelectedPromotion == null)
				{
					// Добавление
					var newPromotion = new Promotion
					{
						Code = Code,
						Type = Type,
						Value = Value,
						StartDate = StartDate,
						EndDate = EndDate,
						IsActive = IsActive
					};

					await _promotionRepository.AddAsync(newPromotion);
				}
				else
				{
					// Редактирование
					SelectedPromotion.Code = Code;
					SelectedPromotion.Type = Type;
					SelectedPromotion.Value = Value;
					SelectedPromotion.StartDate = StartDate;
					SelectedPromotion.EndDate = EndDate;
					SelectedPromotion.IsActive = IsActive;

					await _promotionRepository.UpdateAsync(SelectedPromotion);
				}

				await _promotionRepository.SaveChangesAsync();
				IsEditMode = false;
				await LoadPromotionsAsync();
				
				var message = SelectedPromotion == null ? "Акция успешно добавлена!" : "Акция успешно обновлена!";
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
			SelectedPromotion = null;
		}

		[RelayCommand]
		private async Task DeleteAsync()
		{
			if (SelectedPromotion == null)
			{
				MessageBox.Show("Выберите акцию для удаления!", "Предупреждение", 
					MessageBoxButton.OK, MessageBoxImage.Information);
				return;
			}

			var result = MessageBox.Show($"Вы уверены, что хотите удалить акцию '{SelectedPromotion.Code}'?", 
				"Подтверждение удаления", MessageBoxButton.YesNo, MessageBoxImage.Question);
			
			if (result != MessageBoxResult.Yes) return;

			IsBusy = true;
			try
			{
				await _promotionRepository.DeleteAsync(SelectedPromotion.Id);
				await _promotionRepository.SaveChangesAsync();
				await LoadPromotionsAsync();
				
				MessageBox.Show("Акция успешно удалена!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
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
