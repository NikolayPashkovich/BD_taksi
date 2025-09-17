using Microsoft.EntityFrameworkCore;
using BD_taksi.Entities;
using BD_taksi.Data;

namespace BD_taksi.Services
{
	public class VehicleMakeRepository : Repository<VehicleMake>, IVehicleMakeRepository
	{
		public VehicleMakeRepository(AppDbContext context) : base(context)
		{
		}

		public async Task<IEnumerable<VehicleMake>> SearchAsync(string? searchTerm)
		{
			if (string.IsNullOrWhiteSpace(searchTerm))
				return await GetAllAsync();

			return await _dbSet
				.Where(vm => vm.Name.Contains(searchTerm))
				.ToListAsync();
		}
	}
}
