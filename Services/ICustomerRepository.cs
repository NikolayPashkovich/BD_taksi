using BD_taksi.Entities;

namespace BD_taksi.Services
{
	public interface ICustomerRepository : IRepository<Customer>
	{
		Task<IEnumerable<Customer>> SearchAsync(string? searchTerm);
		Task<IEnumerable<Customer>> GetActiveCustomersAsync();
	}
} 