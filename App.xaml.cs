using System.Configuration;
using System.Data;
using System.Windows;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using BD_taksi.Data;
using BD_taksi.Services;
using BD_taksi.ViewModels;
using BD_taksi.Views;

namespace BD_taksi
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    	public partial class App : Application
	{
		private ServiceProvider _serviceProvider;
		public ServiceProvider Services => _serviceProvider;

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            var services = new ServiceCollection();
            ConfigureServices(services);
            _serviceProvider = services.BuildServiceProvider();

            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseSqlServer("Server=.\\SQLEXPRESS;Database=BD_Taksi;Trusted_Connection=True;TrustServerCertificate=True;MultipleActiveResultSets=True")
                .Options;

            using (var db = new AppDbContext(options))
            {
                db.Database.Migrate();
            }

            var mainWindow = _serviceProvider.GetService<MainWindow>();
            mainWindow?.Show();
        }

        private void ConfigureServices(ServiceCollection services)
        {
            // DbContext
            services.AddDbContext<AppDbContext>(options =>
                options.UseSqlServer("Server=.\\SQLEXPRESS;Database=BD_Taksi;Trusted_Connection=True;TrustServerCertificate=True;MultipleActiveResultSets=True"), ServiceLifetime.Transient);

            // Services
            services.AddSingleton<INotificationService, NotificationService>();

            // Repositories
            services.AddTransient<IDriverRepository, DriverRepository>();
            services.AddTransient<ICustomerRepository, CustomerRepository>();
            services.AddTransient<ILicenseRepository, LicenseRepository>();
            services.AddTransient<IVehicleMakeRepository, VehicleMakeRepository>();
            services.AddTransient<IVehicleModelRepository, VehicleModelRepository>();
            services.AddTransient<IVehicleRepository, VehicleRepository>();
            services.AddTransient<ITariffRepository, TariffRepository>();
            services.AddTransient<IPromotionRepository, PromotionRepository>();
            services.AddTransient<IRideRepository, RideRepository>();
            services.AddTransient<IPaymentMethodRepository, PaymentMethodRepository>();
            services.AddTransient<IPaymentRepository, PaymentRepository>();
            services.AddTransient<IShiftRepository, ShiftRepository>();
            services.AddTransient<IMaintenanceRepository, MaintenanceRepository>();
            services.AddTransient<IIncidentRepository, IncidentRepository>();
            services.AddTransient<IRideStatusRepository, RideStatusRepository>();

            // ViewModels
            services.AddTransient<DriverViewModel>();
            services.AddTransient<CustomerViewModel>();
            services.AddTransient<LicenseViewModel>();
            services.AddTransient<VehicleMakeViewModel>();
            services.AddTransient<VehicleModelViewModel>();
            services.AddTransient<VehicleViewModel>();
            services.AddTransient<TariffViewModel>();
            services.AddTransient<PromotionViewModel>();
            services.AddTransient<RideViewModel>();
            services.AddTransient<PaymentMethodViewModel>();
            services.AddTransient<PaymentViewModel>();
            services.AddTransient<ShiftViewModel>();
            services.AddTransient<MaintenanceViewModel>();
            services.AddTransient<IncidentViewModel>();
            services.AddTransient<RideStatusViewModel>();

            // Views
            services.AddTransient<MainWindow>();
            services.AddTransient<DriversView>();
            services.AddTransient<CustomersView>();
            services.AddTransient<LicensesView>();
            services.AddTransient<VehicleMakesView>();
            services.AddTransient<VehicleModelsView>();
            services.AddTransient<VehiclesView>();
            services.AddTransient<TariffsView>();
            services.AddTransient<PromotionsView>();
            services.AddTransient<RidesView>();
            services.AddTransient<PaymentMethodsView>();
            services.AddTransient<PaymentsView>();
            services.AddTransient<ShiftsView>();
            services.AddTransient<MaintenanceView>();
            services.AddTransient<IncidentsView>();
            services.AddTransient<RideStatusesView>();
        }
    }
}
