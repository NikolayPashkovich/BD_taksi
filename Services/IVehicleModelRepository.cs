using BD_taksi.Entities;

namespace BD_taksi.Services
{
	public interface IVehicleModelRepository : IRepository<VehicleModel>
	{
		Task<IEnumerable<VehicleModel>> SearchAsync(string? searchTerm);
		Task<IEnumerable<VehicleModel>> GetByMakeIdAsync(int makeId);
		Task<IEnumerable<VehicleModel>> GetByClassAsync(string vehicleClass);
	}
}
