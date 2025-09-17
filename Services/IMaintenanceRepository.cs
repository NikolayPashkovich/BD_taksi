using BD_taksi.Entities;

namespace BD_taksi.Services
{
	public interface IMaintenanceRepository : IRepository<Maintenance>
	{
		Task<IEnumerable<Maintenance>> SearchAsync(string? searchTerm);
		Task<IEnumerable<Maintenance>> GetByVehicleIdAsync(int vehicleId);
		Task<IEnumerable<Maintenance>> GetByTypeAsync(string type);
		Task<IEnumerable<Maintenance>> GetByDateRangeAsync(DateTime startDate, DateTime endDate);
	}
}
