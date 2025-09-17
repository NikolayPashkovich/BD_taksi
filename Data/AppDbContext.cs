using Microsoft.EntityFrameworkCore;
using System;

namespace BD_taksi.Data
{
	public class AppDbContext : DbContext
	{
		public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
		{
		}

		public DbSet<Entities.Driver> Drivers => Set<Entities.Driver>();
		public DbSet<Entities.License> Licenses => Set<Entities.License>();
		public DbSet<Entities.VehicleMake> VehicleMakes => Set<Entities.VehicleMake>();
		public DbSet<Entities.VehicleModel> VehicleModels => Set<Entities.VehicleModel>();
		public DbSet<Entities.Vehicle> Vehicles => Set<Entities.Vehicle>();
		public DbSet<Entities.Customer> Customers => Set<Entities.Customer>();
		public DbSet<Entities.RideStatus> RideStatuses => Set<Entities.RideStatus>();
		public DbSet<Entities.Tariff> Tariffs => Set<Entities.Tariff>();
		public DbSet<Entities.Promotion> Promotions => Set<Entities.Promotion>();
		public DbSet<Entities.Ride> Rides => Set<Entities.Ride>();
		public DbSet<Entities.PaymentMethod> PaymentMethods => Set<Entities.PaymentMethod>();
		public DbSet<Entities.Payment> Payments => Set<Entities.Payment>();
		public DbSet<Entities.Shift> Shifts => Set<Entities.Shift>();
		public DbSet<Entities.Maintenance> Maintenances => Set<Entities.Maintenance>();
		public DbSet<Entities.Incident> Incidents => Set<Entities.Incident>();

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			// one-to-one Driver-License
			modelBuilder.Entity<Entities.Driver>()
				.HasOne(d => d.License)
				.WithOne(l => l.Driver)
				.HasForeignKey<Entities.License>(l => l.DriverId)
				.OnDelete(DeleteBehavior.Cascade);

			// VehicleMake -> VehicleModel (1-many)
			modelBuilder.Entity<Entities.VehicleModel>()
				.HasOne(vm => vm.Make)
				.WithMany(m => m.Models)
				.HasForeignKey(vm => vm.MakeId)
				.OnDelete(DeleteBehavior.Restrict);

			// VehicleModel -> Vehicle (1-many)
			modelBuilder.Entity<Entities.Vehicle>()
				.HasOne(v => v.Model)
				.WithMany(m => m.Vehicles)
				.HasForeignKey(v => v.ModelId)
				.OnDelete(DeleteBehavior.Restrict);

			// Driver -> Ride (1-many)
			modelBuilder.Entity<Entities.Ride>()
				.HasOne(r => r.Driver)
				.WithMany(d => d.Rides)
				.HasForeignKey(r => r.DriverId)
				.OnDelete(DeleteBehavior.Restrict);

			// Vehicle -> Ride (1-many)
			modelBuilder.Entity<Entities.Ride>()
				.HasOne(r => r.Vehicle)
				.WithMany(v => v.Rides)
				.HasForeignKey(r => r.VehicleId)
				.OnDelete(DeleteBehavior.Restrict);

			// Customer -> Ride (1-many)
			modelBuilder.Entity<Entities.Ride>()
				.HasOne(r => r.Customer)
				.WithMany(c => c.Rides)
				.HasForeignKey(r => r.CustomerId)
				.OnDelete(DeleteBehavior.Restrict);

			// RideStatus -> Ride (1-many)
			modelBuilder.Entity<Entities.Ride>()
				.HasOne(r => r.Status)
				.WithMany(s => s.Rides)
				.HasForeignKey(r => r.StatusId)
				.OnDelete(DeleteBehavior.Restrict);

			// Tariff -> Ride (1-many)
			modelBuilder.Entity<Entities.Ride>()
				.HasOne(r => r.Tariff)
				.WithMany(t => t.Rides)
				.HasForeignKey(r => r.TariffId)
				.OnDelete(DeleteBehavior.Restrict);

			// Promotion -> Ride (1-many, optional)
			modelBuilder.Entity<Entities.Ride>()
				.HasOne(r => r.Promotion)
				.WithMany(p => p.Rides)
				.HasForeignKey(r => r.PromotionId)
				.OnDelete(DeleteBehavior.SetNull);

			// Payment relations
			modelBuilder.Entity<Entities.Payment>()
				.HasOne(p => p.Ride)
				.WithMany(r => r.Payments)
				.HasForeignKey(p => p.RideId)
				.OnDelete(DeleteBehavior.Cascade);

			modelBuilder.Entity<Entities.Payment>()
				.HasOne(p => p.Customer)
				.WithMany(c => c.Payments)
				.HasForeignKey(p => p.CustomerId)
				.OnDelete(DeleteBehavior.Restrict);

			modelBuilder.Entity<Entities.Payment>()
				.HasOne(p => p.Method)
				.WithMany(m => m.Payments)
				.HasForeignKey(p => p.MethodId)
				.OnDelete(DeleteBehavior.Restrict);

			// Shift relations
			modelBuilder.Entity<Entities.Shift>()
				.HasOne(s => s.Driver)
				.WithMany(d => d.Shifts)
				.HasForeignKey(s => s.DriverId)
				.OnDelete(DeleteBehavior.Restrict);

			modelBuilder.Entity<Entities.Shift>()
				.HasOne(s => s.Vehicle)
				.WithMany(v => v.Shifts)
				.HasForeignKey(s => s.VehicleId)
				.OnDelete(DeleteBehavior.Restrict);

			// Maintenance relations
			modelBuilder.Entity<Entities.Maintenance>()
				.HasOne(m => m.Vehicle)
				.WithMany(v => v.Maintenances)
				.HasForeignKey(m => m.VehicleId)
				.OnDelete(DeleteBehavior.Cascade);

			// Incident relations
			modelBuilder.Entity<Entities.Incident>()
				.HasOne(i => i.Vehicle)
				.WithMany(v => v.Incidents)
				.HasForeignKey(i => i.VehicleId)
				.OnDelete(DeleteBehavior.Restrict);

			modelBuilder.Entity<Entities.Incident>()
				.HasOne(i => i.Driver)
				.WithMany(d => d.Incidents)
				.HasForeignKey(i => i.DriverId)
				.OnDelete(DeleteBehavior.Restrict);

			// decimal precision for money (RUB)
			modelBuilder.Entity<Entities.Tariff>().Property(p => p.BaseFare).HasPrecision(18, 2);
			modelBuilder.Entity<Entities.Tariff>().Property(p => p.PricePerKm).HasPrecision(18, 2);
			modelBuilder.Entity<Entities.Tariff>().Property(p => p.PricePerMinute).HasPrecision(18, 2);
			modelBuilder.Entity<Entities.Promotion>().Property(p => p.Value).HasPrecision(18, 2);
			modelBuilder.Entity<Entities.Ride>().Property(p => p.Cost).HasPrecision(18, 2);
			modelBuilder.Entity<Entities.Payment>().Property(p => p.Amount).HasPrecision(18, 2);
			modelBuilder.Entity<Entities.Maintenance>().Property(p => p.Cost).HasPrecision(18, 2);
			modelBuilder.Entity<Entities.Incident>().Property(p => p.DamageCost).HasPrecision(18, 2);

			base.OnModelCreating(modelBuilder);
		}
	}
} 