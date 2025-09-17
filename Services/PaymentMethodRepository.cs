using Microsoft.EntityFrameworkCore;
using BD_taksi.Entities;
using BD_taksi.Data;

namespace BD_taksi.Services
{
	public class PaymentMethodRepository : Repository<PaymentMethod>, IPaymentMethodRepository
	{
		public PaymentMethodRepository(AppDbContext context) : base(context)
		{
		}

		public async Task<IEnumerable<PaymentMethod>> SearchAsync(string? searchTerm)
		{
			if (string.IsNullOrWhiteSpace(searchTerm))
				return await GetAllAsync();

			return await _dbSet
				.Where(pm => pm.Name.Contains(searchTerm))
				.ToListAsync();
		}
	}
}
