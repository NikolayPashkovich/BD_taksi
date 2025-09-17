using Microsoft.EntityFrameworkCore;
using BD_taksi.Entities;
using BD_taksi.Data;

namespace BD_taksi.Services
{
	public class RideStatusRepository : Repository<RideStatus>, IRideStatusRepository
	{
		public RideStatusRepository(AppDbContext context) : base(context)
		{
		}

		public async Task<IEnumerable<RideStatus>> SearchAsync(string? searchTerm)
		{
			if (string.IsNullOrWhiteSpace(searchTerm))
				return await GetAllAsync();

			return await _dbSet
				.Where(rs => rs.Name.Contains(searchTerm))
				.ToListAsync();
		}
	}
}
