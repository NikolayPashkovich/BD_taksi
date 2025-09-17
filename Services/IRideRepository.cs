using BD_taksi.Entities;

namespace BD_taksi.Services
{
	public interface IRideRepository : IRepository<Ride>
	{
		Task<IEnumerable<Ride>> SearchAsync(string? searchTerm);
		Task<IEnumerable<Ride>> GetByDriverIdAsync(int driverId);
		Task<IEnumerable<Ride>> GetByCustomerIdAsync(int customerId);
		Task<IEnumerable<Ride>> GetByStatusIdAsync(int statusId);
		Task<IEnumerable<Ride>> GetByDateRangeAsync(DateTime startDate, DateTime endDate);
	}
}
