using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using BD_taksi.Entities;
using BD_taksi.Services;
using System.Collections.Generic;

namespace BD_taksi.ViewModels
{
    public partial class RideStatusViewModel : BaseViewModel
    {
        private readonly IRideStatusRepository _rideStatusRepository;

        [ObservableProperty]
        private ObservableCollection<RideStatus> _rideStatuses;

        [ObservableProperty]
        private RideStatus _selectedRideStatus;

        [ObservableProperty]
        private string _searchTerm;

        [ObservableProperty]
        private bool _isEditMode;

        [ObservableProperty]
        private string _name;

        [ObservableProperty]
        private string _description;

        [ObservableProperty]
        private bool _isActive;

        public RideStatusViewModel(IRideStatusRepository rideStatusRepository)
        {
            _rideStatusRepository = rideStatusRepository;
            RideStatuses = new ObservableCollection<RideStatus>();
            _ = LoadRideStatusesAsync();
            _ = EnsureDefaultStatusesExistAsync();
        }

        [RelayCommand]
        private async Task LoadRideStatusesAsync()
        {
            try
            {
                IsBusy = true;
                var rideStatuses = await _rideStatusRepository.GetAllAsync();
                RideStatuses.Clear();
                foreach (var status in rideStatuses)
                {
                    RideStatuses.Add(status);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке статусов поездок: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                IsBusy = false;
            }
        }

        [RelayCommand]
        private void Search()
        {
            if (string.IsNullOrWhiteSpace(SearchTerm))
            {
                _ = LoadRideStatusesAsync();
                return;
            }

            var filtered = RideStatuses.Where(s => 
                s.Name.Contains(SearchTerm, StringComparison.OrdinalIgnoreCase) ||
                s.Description.Contains(SearchTerm, StringComparison.OrdinalIgnoreCase)).ToList();

            RideStatuses.Clear();
            foreach (var status in filtered)
            {
                RideStatuses.Add(status);
            }
        }

        [RelayCommand]
        private void AddNew()
        {
            IsEditMode = true;
            Name = string.Empty;
            Description = string.Empty;
            IsActive = true;
        }

        [RelayCommand]
        private void Edit()
        {
            if (SelectedRideStatus == null)
            {
                MessageBox.Show("Выберите статус поездки для редактирования", "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            IsEditMode = true;
            Name = SelectedRideStatus.Name;
            Description = SelectedRideStatus.Description;
            IsActive = SelectedRideStatus.IsActive;
        }

        [RelayCommand]
        private async Task SaveAsync()
        {
            if (string.IsNullOrWhiteSpace(Name))
            {
                MessageBox.Show("Введите название статуса", "Ошибка валидации", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                IsBusy = true;

                if (SelectedRideStatus == null)
                {
                    // Добавление нового статуса
                    var newStatus = new RideStatus
                    {
                        Name = Name,
                        Description = Description,
                        IsActive = IsActive
                    };

                    await _rideStatusRepository.AddAsync(newStatus);
                    await _rideStatusRepository.SaveChangesAsync();
                    RideStatuses.Add(newStatus);
                }
                else
                {
                    // Обновление существующего статуса
                    SelectedRideStatus.Name = Name;
                    SelectedRideStatus.Description = Description;
                    SelectedRideStatus.IsActive = IsActive;

                    await _rideStatusRepository.UpdateAsync(SelectedRideStatus);
                    await _rideStatusRepository.SaveChangesAsync();
                }

                IsEditMode = false;
                SelectedRideStatus = null;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при сохранении: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
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
            SelectedRideStatus = null;
            Name = string.Empty;
            Description = string.Empty;
            IsActive = true;
        }

        [RelayCommand]
        private async Task DeleteAsync()
        {
            if (SelectedRideStatus == null)
            {
                MessageBox.Show("Выберите статус поездки для удаления", "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var result = MessageBox.Show($"Вы уверены, что хотите удалить статус '{SelectedRideStatus.Name}'?", 
                "Подтверждение удаления", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    IsBusy = true;
                    await _rideStatusRepository.DeleteAsync(SelectedRideStatus.Id);
                    await _rideStatusRepository.SaveChangesAsync();
                    RideStatuses.Remove(SelectedRideStatus);
                    SelectedRideStatus = null;
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при удалении: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                finally
                {
                    IsBusy = false;
                }
            }
        }

        partial void OnSearchTermChanged(string value)
        {
            Search();
        }

        private async Task EnsureDefaultStatusesExistAsync()
        {
            try
            {
                var existingStatuses = await _rideStatusRepository.GetAllAsync();
                if (!existingStatuses.Any())
                {
                    // Создаем базовые статусы поездок
                    var defaultStatuses = new List<RideStatus>
                    {
                        new RideStatus { Name = "Заказана", Description = "Поездка заказана клиентом", IsActive = true },
                        new RideStatus { Name = "В пути", Description = "Водитель едет к клиенту", IsActive = true },
                        new RideStatus { Name = "Выполняется", Description = "Поездка выполняется", IsActive = true },
                        new RideStatus { Name = "Завершена", Description = "Поездка завершена", IsActive = true },
                        new RideStatus { Name = "Отменена", Description = "Поездка отменена", IsActive = true }
                    };

                    foreach (var status in defaultStatuses)
                    {
                        await _rideStatusRepository.AddAsync(status);
                    }
                    await _rideStatusRepository.SaveChangesAsync();
                    
                    // Обновляем список
                    await LoadRideStatusesAsync();
                }
            }
            catch (Exception ex)
            {
                // Логируем ошибку, но не показываем пользователю
                System.Diagnostics.Debug.WriteLine($"Ошибка при создании базовых статусов: {ex.Message}");
            }
        }
    }
}
