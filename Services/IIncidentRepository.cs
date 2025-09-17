using BD_taksi.Entities;

namespace BD_taksi.Services
{
	public interface IIncidentRepository : IRepository<Incident>
	{
		Task<IEnumerable<Incident>> SearchAsync(string? searchTerm);
		Task<IEnumerable<Incident>> GetByVehicleIdAsync(int vehicleId);
		Task<IEnumerable<Incident>> GetByDriverIdAsync(int driverId);
		Task<IEnumerable<Incident>> GetByDateRangeAsync(DateTime startDate, DateTime endDate);
	}
}
