using Microsoft.EntityFrameworkCore;
using BD_taksi.Entities;
using BD_taksi.Data;

namespace BD_taksi.Services
{
	public class TariffRepository : Repository<Tariff>, ITariffRepository
	{
		public TariffRepository(AppDbContext context) : base(context)
		{
		}

		public async Task<IEnumerable<Tariff>> SearchAsync(string? searchTerm)
		{
			if (string.IsNullOrWhiteSpace(searchTerm))
				return await GetAllAsync();

			return await _dbSet
				.Where(t => t.Name.Contains(searchTerm))
				.ToListAsync();
		}
	}
}
