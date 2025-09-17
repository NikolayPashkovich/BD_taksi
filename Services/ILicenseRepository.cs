using BD_taksi.Entities;

namespace BD_taksi.Services
{
	public interface ILicenseRepository : IRepository<License>
	{
		Task<IEnumerable<License>> SearchAsync(string searchTerm);
		Task<IEnumerable<License>> GetByDriverIdAsync(int driverId);
		Task<IEnumerable<License>> GetExpiringLicensesAsync(DateTime expiryDate);
	}
}
