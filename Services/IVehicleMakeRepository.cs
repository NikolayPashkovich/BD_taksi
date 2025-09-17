using BD_taksi.Entities;

namespace BD_taksi.Services
{
	public interface IVehicleMakeRepository : IRepository<VehicleMake>
	{
		Task<IEnumerable<VehicleMake>> SearchAsync(string? searchTerm);
	}
}
