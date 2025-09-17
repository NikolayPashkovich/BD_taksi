using BD_taksi.Entities;

namespace BD_taksi.Services
{
	public interface IPaymentMethodRepository : IRepository<PaymentMethod>
	{
		Task<IEnumerable<PaymentMethod>> SearchAsync(string? searchTerm);
	}
}
