using BD_taksi.Entities;

namespace BD_taksi.Services
{
	public interface IPromotionRepository : IRepository<Promotion>
	{
		Task<IEnumerable<Promotion>> SearchAsync(string? searchTerm);
		Task<IEnumerable<Promotion>> GetActivePromotionsAsync();
		Task<IEnumerable<Promotion>> GetByDateRangeAsync(DateTime startDate, DateTime endDate);
	}
}
