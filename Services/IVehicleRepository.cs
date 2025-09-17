using BD_taksi.Entities;

namespace BD_taksi.Services
{
	public interface IVehicleRepository : IRepository<Vehicle>
	{
		Task<IEnumerable<Vehicle>> SearchAsync(string? searchTerm);
		Task<IEnumerable<Vehicle>> GetActiveVehiclesAsync();
		Task<IEnumerable<Vehicle>> GetByModelIdAsync(int modelId);
		Task<IEnumerable<Vehicle>> GetByYearAsync(int year);
	}
}
