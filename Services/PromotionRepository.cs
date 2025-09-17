using Microsoft.EntityFrameworkCore;
using BD_taksi.Entities;
using BD_taksi.Data;

namespace BD_taksi.Services
{
	public class PromotionRepository : Repository<Promotion>, IPromotionRepository
	{
		public PromotionRepository(AppDbContext context) : base(context)
		{
		}

		public async Task<IEnumerable<Promotion>> SearchAsync(string? searchTerm)
		{
			if (string.IsNullOrWhiteSpace(searchTerm))
				return await GetAllAsync();

			return await _dbSet
				.Where(p => p.Code.Contains(searchTerm))
				.ToListAsync();
		}

		public async Task<IEnumerable<Promotion>> GetActivePromotionsAsync()
		{
			return await _dbSet
				.ToListAsync();
		}

		public async Task<IEnumerable<Promotion>> GetByDateRangeAsync(DateTime startDate, DateTime endDate)
		{
			return await _dbSet
				.Where(p => p.StartDate <= endDate && p.EndDate >= startDate)
				.ToListAsync();
		}
	}
}
