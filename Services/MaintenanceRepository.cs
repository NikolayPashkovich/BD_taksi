using Microsoft.EntityFrameworkCore;
using BD_taksi.Entities;
using BD_taksi.Data;

namespace BD_taksi.Services
{
	public class MaintenanceRepository : Repository<Maintenance>, IMaintenanceRepository
	{
		public MaintenanceRepository(AppDbContext context) : base(context)
		{
		}

		public async Task<IEnumerable<Maintenance>> SearchAsync(string? searchTerm)
		{
			if (string.IsNullOrWhiteSpace(searchTerm))
				return await _dbSet
					.Include(m => m.Vehicle).ThenInclude(v => v.Model).ThenInclude(m => m.Make)
					.ToListAsync();

			return await _dbSet
				.Include(m => m.Vehicle).ThenInclude(v => v.Model).ThenInclude(m => m.Make)
				.Where(m => m.Type.Contains(searchTerm) ||
						   m.Vehicle.PlateNumber.Contains(searchTerm) ||
						   m.Comment.Contains(searchTerm))
				.ToListAsync();
		}

		public async Task<IEnumerable<Maintenance>> GetByVehicleIdAsync(int vehicleId)
		{
			return await _dbSet
				.Include(m => m.Vehicle).ThenInclude(v => v.Model).ThenInclude(m => m.Make)
				.Where(m => m.VehicleId == vehicleId)
				.ToListAsync();
		}

		public async Task<IEnumerable<Maintenance>> GetByTypeAsync(string type)
		{
			return await _dbSet
				.Include(m => m.Vehicle).ThenInclude(v => v.Model).ThenInclude(m => m.Make)
				.Where(m => m.Type == type)
				.ToListAsync();
		}

		public async Task<IEnumerable<Maintenance>> GetByDateRangeAsync(DateTime startDate, DateTime endDate)
		{
			return await _dbSet
				.Include(m => m.Vehicle).ThenInclude(v => v.Model).ThenInclude(m => m.Make)
				.Where(m => m.Date >= startDate && m.Date <= endDate)
				.ToListAsync();
		}
	}
}
