using Microsoft.EntityFrameworkCore;
using BD_taksi.Entities;
using BD_taksi.Data;

namespace BD_taksi.Services
{
	public class LicenseRepository : Repository<License>, ILicenseRepository
	{
		public LicenseRepository(AppDbContext context) : base(context)
		{
		}

		public async Task<IEnumerable<License>> SearchAsync(string? searchTerm)
		{
			if (string.IsNullOrWhiteSpace(searchTerm))
				return await _dbSet.Include(l => l.Driver).ToListAsync();

			return await _dbSet
				.Include(l => l.Driver)
				.Where(l => l.Number.Contains(searchTerm) || 
						   l.Category.Contains(searchTerm) ||
						   l.Driver.FullName.Contains(searchTerm))
				.ToListAsync();
		}

		public async Task<IEnumerable<License>> GetByDriverIdAsync(int driverId)
		{
			return await _dbSet
				.Include(l => l.Driver)
				.Where(l => l.DriverId == driverId)
				.ToListAsync();
		}

		public async Task<IEnumerable<License>> GetExpiringLicensesAsync(DateTime expiryDate)
		{
			return await _dbSet
				.Include(l => l.Driver)
				.Where(l => l.ExpiryDate <= expiryDate)
				.ToListAsync();
		}
	}
}
