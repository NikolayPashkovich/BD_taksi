using BD_taksi.Entities;

namespace BD_taksi.Services
{
	public interface IRideStatusRepository : IRepository<RideStatus>
	{
		Task<IEnumerable<RideStatus>> SearchAsync(string? searchTerm);
	}
}
