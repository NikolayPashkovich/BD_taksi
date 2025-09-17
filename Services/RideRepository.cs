using Microsoft.EntityFrameworkCore;
using BD_taksi.Entities;
using BD_taksi.Data;

namespace BD_taksi.Services
{
	public class RideRepository : Repository<Ride>, IRideRepository
	{
		public RideRepository(AppDbContext context) : base(context)
		{
		}

		public async Task<IEnumerable<Ride>> SearchAsync(string? searchTerm)
		{
			if (string.IsNullOrWhiteSpace(searchTerm))
				return await _dbSet
					.Include(r => r.Driver)
					.Include(r => r.Customer)
					.Include(r => r.Vehicle).ThenInclude(v => v.Model).ThenInclude(m => m.Make)
					.Include(r => r.Status)
					.Include(r => r.Tariff)
					.Include(r => r.Promotion)
					.ToListAsync();

			return await _dbSet
				.Include(r => r.Driver)
				.Include(r => r.Customer)
				.Include(r => r.Vehicle).ThenInclude(v => v.Model).ThenInclude(m => m.Make)
				.Include(r => r.Status)
				.Include(r => r.Tariff)
				.Include(r => r.Promotion)
				.Where(r => r.Driver.FullName.Contains(searchTerm) ||
						   r.Customer.FullName.Contains(searchTerm) ||
						   r.Vehicle.PlateNumber.Contains(searchTerm) ||
						   r.Status.Name.Contains(searchTerm) ||
						   r.Comment.Contains(searchTerm))
				.ToListAsync();
		}

		public async Task<IEnumerable<Ride>> GetByDriverIdAsync(int driverId)
		{
			return await _dbSet
				.Include(r => r.Driver)
				.Include(r => r.Customer)
				.Include(r => r.Vehicle).ThenInclude(v => v.Model).ThenInclude(m => m.Make)
				.Include(r => r.Status)
				.Include(r => r.Tariff)
				.Include(r => r.Promotion)
				.Where(r => r.DriverId == driverId)
				.ToListAsync();
		}

		public async Task<IEnumerable<Ride>> GetByCustomerIdAsync(int customerId)
		{
			return await _dbSet
				.Include(r => r.Driver)
				.Include(r => r.Customer)
				.Include(r => r.Vehicle).ThenInclude(v => v.Model).ThenInclude(m => m.Make)
				.Include(r => r.Status)
				.Include(r => r.Tariff)
				.Include(r => r.Promotion)
				.Where(r => r.CustomerId == customerId)
				.ToListAsync();
		}

		public async Task<IEnumerable<Ride>> GetByStatusIdAsync(int statusId)
		{
			return await _dbSet
				.Include(r => r.Driver)
				.Include(r => r.Customer)
				.Include(r => r.Vehicle).ThenInclude(v => v.Model).ThenInclude(m => m.Make)
				.Include(r => r.Status)
				.Include(r => r.Tariff)
				.Include(r => r.Promotion)
				.Where(r => r.StatusId == statusId)
				.ToListAsync();
		}

		public async Task<IEnumerable<Ride>> GetByDateRangeAsync(DateTime startDate, DateTime endDate)
		{
			return await _dbSet
				.Include(r => r.Driver)
				.Include(r => r.Customer)
				.Include(r => r.Vehicle).ThenInclude(v => v.Model).ThenInclude(m => m.Make)
				.Include(r => r.Status)
				.Include(r => r.Tariff)
				.Include(r => r.Promotion)
				.Where(r => r.StartTime >= startDate && r.StartTime <= endDate)
				.ToListAsync();
		}
	}
}
