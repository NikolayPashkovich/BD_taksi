using Microsoft.EntityFrameworkCore;
using BD_taksi.Entities;
using BD_taksi.Data;

namespace BD_taksi.Services
{
	public class IncidentRepository : Repository<Incident>, IIncidentRepository
	{
		public IncidentRepository(AppDbContext context) : base(context)
		{
		}

		public async Task<IEnumerable<Incident>> SearchAsync(string? searchTerm)
		{
			if (string.IsNullOrWhiteSpace(searchTerm))
				return await _dbSet
					.Include(i => i.Vehicle).ThenInclude(v => v.Model).ThenInclude(m => m.Make)
					.Include(i => i.Driver)
					.ToListAsync();

			return await _dbSet
				.Include(i => i.Vehicle).ThenInclude(v => v.Model).ThenInclude(m => m.Make)
				.Include(i => i.Driver)
				.Where(i => i.Description.Contains(searchTerm) ||
						   i.Vehicle.PlateNumber.Contains(searchTerm) ||
						   i.Driver.FullName.Contains(searchTerm))
				.ToListAsync();
		}

		public async Task<IEnumerable<Incident>> GetByVehicleIdAsync(int vehicleId)
		{
			return await _dbSet
				.Include(i => i.Vehicle).ThenInclude(v => v.Model).ThenInclude(m => m.Make)
				.Include(i => i.Driver)
				.Where(i => i.VehicleId == vehicleId)
				.ToListAsync();
		}

		public async Task<IEnumerable<Incident>> GetByDriverIdAsync(int driverId)
		{
			return await _dbSet
				.Include(i => i.Vehicle).ThenInclude(v => v.Model).ThenInclude(m => m.Make)
				.Include(i => i.Driver)
				.Where(i => i.DriverId == driverId)
				.ToListAsync();
		}

		public async Task<IEnumerable<Incident>> GetByDateRangeAsync(DateTime startDate, DateTime endDate)
		{
			return await _dbSet
				.Include(i => i.Vehicle).ThenInclude(v => v.Model).ThenInclude(m => m.Make)
				.Include(i => i.Driver)
				.Where(i => i.Date >= startDate && i.Date <= endDate)
				.ToListAsync();
		}
	}
}
