using Microsoft.EntityFrameworkCore;
using BD_taksi.Entities;
using BD_taksi.Data;

namespace BD_taksi.Services
{
	public class ShiftRepository : Repository<Shift>, IShiftRepository
	{
		public ShiftRepository(AppDbContext context) : base(context)
		{
		}

		public async Task<IEnumerable<Shift>> SearchAsync(string? searchTerm)
		{
			if (string.IsNullOrWhiteSpace(searchTerm))
				return await _dbSet
					.Include(s => s.Driver)
					.Include(s => s.Vehicle).ThenInclude(v => v.Model).ThenInclude(m => m.Make)
					.ToListAsync();

			return await _dbSet
				.Include(s => s.Driver)
				.Include(s => s.Vehicle).ThenInclude(v => v.Model).ThenInclude(m => m.Make)
				.Where(s => s.Driver.FullName.Contains(searchTerm) ||
						   s.Vehicle.PlateNumber.Contains(searchTerm))
				.ToListAsync();
		}

		public async Task<IEnumerable<Shift>> GetByDriverIdAsync(int driverId)
		{
			return await _dbSet
				.Include(s => s.Driver)
				.Include(s => s.Vehicle).ThenInclude(v => v.Model).ThenInclude(m => m.Make)
				.Where(s => s.DriverId == driverId)
				.ToListAsync();
		}

		public async Task<IEnumerable<Shift>> GetByVehicleIdAsync(int vehicleId)
		{
			return await _dbSet
				.Include(s => s.Driver)
				.Include(s => s.Vehicle).ThenInclude(v => v.Model).ThenInclude(m => m.Make)
				.Where(s => s.VehicleId == vehicleId)
				.ToListAsync();
		}

		public async Task<IEnumerable<Shift>> GetByDateRangeAsync(DateTime startDate, DateTime endDate)
		{
			return await _dbSet
				.Include(s => s.Driver)
				.Include(s => s.Vehicle).ThenInclude(v => v.Model).ThenInclude(m => m.Make)
				.Where(s => s.Start >= startDate && s.Start <= endDate)
				.ToListAsync();
		}

		public async Task<IEnumerable<Shift>> GetActiveShiftsAsync()
		{
			return await _dbSet
				.Include(s => s.Driver)
				.Include(s => s.Vehicle).ThenInclude(v => v.Model).ThenInclude(m => m.Make)
				.Where(s => s.End == null)
				.ToListAsync();
		}
	}
}
