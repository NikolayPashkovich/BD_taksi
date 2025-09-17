using Microsoft.EntityFrameworkCore;
using BD_taksi.Entities;
using BD_taksi.Data;

namespace BD_taksi.Services
{
	public class VehicleModelRepository : Repository<VehicleModel>, IVehicleModelRepository
	{
		public VehicleModelRepository(AppDbContext context) : base(context)
		{
		}

		public async Task<IEnumerable<VehicleModel>> SearchAsync(string? searchTerm)
		{
			if (string.IsNullOrWhiteSpace(searchTerm))
				return await _dbSet.Include(vm => vm.Make).ToListAsync();

			return await _dbSet
				.Include(vm => vm.Make)
				.Where(vm => vm.Name.Contains(searchTerm) || 
							vm.Class.Contains(searchTerm) ||
							vm.Make.Name.Contains(searchTerm))
				.ToListAsync();
		}

		public async Task<IEnumerable<VehicleModel>> GetByMakeIdAsync(int makeId)
		{
			return await _dbSet
				.Include(vm => vm.Make)
				.Where(vm => vm.MakeId == makeId)
				.ToListAsync();
		}

		public async Task<IEnumerable<VehicleModel>> GetByClassAsync(string vehicleClass)
		{
			return await _dbSet
				.Include(vm => vm.Make)
				.Where(vm => vm.Class == vehicleClass)
				.ToListAsync();
		}
	}
}
