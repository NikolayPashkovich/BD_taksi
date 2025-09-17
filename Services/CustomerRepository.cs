using Microsoft.EntityFrameworkCore;
using BD_taksi.Entities;
using BD_taksi.Data;

namespace BD_taksi.Services
{
	public class CustomerRepository : Repository<Customer>, ICustomerRepository
	{
		public CustomerRepository(AppDbContext context) : base(context)
		{
		}

		public async Task<IEnumerable<Customer>> SearchAsync(string? searchTerm)
		{
			if (string.IsNullOrWhiteSpace(searchTerm))
				return await GetAllAsync();

			return await _dbSet
				.Where(c => c.Name.Contains(searchTerm) || c.Phone.Contains(searchTerm) || 
				           (c.Email != null && c.Email.Contains(searchTerm)))
				.ToListAsync();
		}

		public async Task<IEnumerable<Customer>> GetActiveCustomersAsync()
		{
			return await _dbSet.ToListAsync(); // Все клиенты активны по умолчанию
		}
	}
} 