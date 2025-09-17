using BD_taksi.Entities;

namespace BD_taksi.Services
{
	public interface IDriverRepository : IRepository<Driver>
	{
		Task<IEnumerable<Driver>> SearchAsync(string? searchTerm);
		Task<IEnumerable<Driver>> GetActiveDriversAsync();
	}
} 