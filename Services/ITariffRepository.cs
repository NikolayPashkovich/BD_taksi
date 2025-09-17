using BD_taksi.Entities;

namespace BD_taksi.Services
{
	public interface ITariffRepository : IRepository<Tariff>
	{
		Task<IEnumerable<Tariff>> SearchAsync(string? searchTerm);
	}
}
