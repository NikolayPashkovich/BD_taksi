using Microsoft.EntityFrameworkCore;
using BD_taksi.Entities;
using BD_taksi.Data;

namespace BD_taksi.Services
{
	public class PaymentRepository : Repository<Payment>, IPaymentRepository
	{
		public PaymentRepository(AppDbContext context) : base(context)
		{
		}

		public async Task<IEnumerable<Payment>> SearchAsync(string? searchTerm)
		{
			if (string.IsNullOrWhiteSpace(searchTerm))
				return await _dbSet
					.Include(p => p.Ride)
					.Include(p => p.Customer)
					.Include(p => p.Method)
					.ToListAsync();

			return await _dbSet
				.Include(p => p.Ride)
				.Include(p => p.Customer)
				.Include(p => p.Method)
				.Where(p => p.Customer.FullName.Contains(searchTerm) ||
						   p.Method.Name.Contains(searchTerm))
				.ToListAsync();
		}

		public async Task<IEnumerable<Payment>> GetByRideIdAsync(int rideId)
		{
			return await _dbSet
				.Include(p => p.Ride)
				.Include(p => p.Customer)
				.Include(p => p.Method)
				.Where(p => p.RideId == rideId)
				.ToListAsync();
		}

		public async Task<IEnumerable<Payment>> GetByCustomerIdAsync(int customerId)
		{
			return await _dbSet
				.Include(p => p.Ride)
				.Include(p => p.Customer)
				.Include(p => p.Method)
				.Where(p => p.CustomerId == customerId)
				.ToListAsync();
		}

		public async Task<IEnumerable<Payment>> GetByStatusAsync(PaymentStatus status)
		{
			return await _dbSet
				.Include(p => p.Ride)
				.Include(p => p.Customer)
				.Include(p => p.Method)
				.Where(p => p.Status == status)
				.ToListAsync();
		}

		public async Task<IEnumerable<Payment>> GetByDateRangeAsync(DateTime startDate, DateTime endDate)
		{
			return await _dbSet
				.Include(p => p.Ride)
				.Include(p => p.Customer)
				.Include(p => p.Method)
				.Where(p => p.Date >= startDate && p.Date <= endDate)
				.ToListAsync();
		}
	}
}
