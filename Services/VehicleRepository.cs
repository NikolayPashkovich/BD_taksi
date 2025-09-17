using Microsoft.EntityFrameworkCore;
using BD_taksi.Entities;
using BD_taksi.Data;

namespace BD_taksi.Services
{
	public class VehicleRepository : Repository<Vehicle>, IVehicleRepository
	{
		public VehicleRepository(AppDbContext context) : base(context)
		{
		}

		public async Task<IEnumerable<Vehicle>> SearchAsync(string? searchTerm)
		{
			if (string.IsNullOrWhiteSpace(searchTerm))
				return await _dbSet.Include(v => v.Model).ThenInclude(m => m.Make).ToListAsync();

			return await _dbSet
				.Include(v => v.Model).ThenInclude(m => m.Make)
				.Where(v => v.PlateNumber.Contains(searchTerm) || 
						   v.Vin.Contains(searchTerm) ||
						   v.Color.Contains(searchTerm) ||
						   v.Model.Name.Contains(searchTerm) ||
						   v.Model.Make.Name.Contains(searchTerm))
				.ToListAsync();
		}

		public async Task<IEnumerable<Vehicle>> GetActiveVehiclesAsync()
		{
			return await _dbSet
				.Include(v => v.Model).ThenInclude(m => m.Make)
				.ToListAsync();
		}

		public async Task<IEnumerable<Vehicle>> GetByModelIdAsync(int modelId)
		{
			return await _dbSet
				.Include(v => v.Model).ThenInclude(m => m.Make)
				.Where(v => v.ModelId == modelId)
				.ToListAsync();
		}

		public async Task<IEnumerable<Vehicle>> GetByYearAsync(int year)
		{
			return await _dbSet
				.Include(v => v.Model).ThenInclude(m => m.Make)
				.Where(v => v.Year == year)
				.ToListAsync();
		}
	}
}
