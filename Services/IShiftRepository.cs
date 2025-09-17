using BD_taksi.Entities;

namespace BD_taksi.Services
{
	public interface IShiftRepository : IRepository<Shift>
	{
		Task<IEnumerable<Shift>> SearchAsync(string? searchTerm);
		Task<IEnumerable<Shift>> GetByDriverIdAsync(int driverId);
		Task<IEnumerable<Shift>> GetByVehicleIdAsync(int vehicleId);
		Task<IEnumerable<Shift>> GetByDateRangeAsync(DateTime startDate, DateTime endDate);
		Task<IEnumerable<Shift>> GetActiveShiftsAsync();
	}
}
