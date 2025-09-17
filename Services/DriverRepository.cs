using Microsoft.EntityFrameworkCore;
using BD_taksi.Entities;
using BD_taksi.Data;

namespace BD_taksi.Services
{
	public class DriverRepository : Repository<Driver>, IDriverRepository
	{
		public DriverRepository(AppDbContext context) : base(context)
		{
		}

		public async Task<IEnumerable<Driver>> SearchAsync(string? searchTerm)
		{
			if (string.IsNullOrWhiteSpace(searchTerm))
				return await GetAllAsync();

			return await _dbSet
				.Where(d => d.FullName.Contains(searchTerm) || d.Phone.Contains(searchTerm))
				.ToListAsync();
		}

		public async Task<IEnumerable<Driver>> GetActiveDriversAsync()
		{
			return await _dbSet
				.ToListAsync();
		}
	}
} 