using BD_taksi.Entities;

namespace BD_taksi.Services
{
	public interface IPaymentRepository : IRepository<Payment>
	{
		Task<IEnumerable<Payment>> SearchAsync(string? searchTerm);
		Task<IEnumerable<Payment>> GetByRideIdAsync(int rideId);
		Task<IEnumerable<Payment>> GetByCustomerIdAsync(int customerId);
		Task<IEnumerable<Payment>> GetByStatusAsync(PaymentStatus status);
		Task<IEnumerable<Payment>> GetByDateRangeAsync(DateTime startDate, DateTime endDate);
	}
}
