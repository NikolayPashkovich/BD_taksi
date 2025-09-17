using System.Linq.Expressions;

namespace BD_taksi.Services
{
	public interface IRepository<T> where T : class
	{
		Task<IEnumerable<T>> GetAllAsync();
		Task<IEnumerable<T>> GetAsync(Expression<Func<T, bool>> predicate);
		Task<T?> GetByIdAsync(int id);
		Task<T> AddAsync(T entity);
		Task UpdateAsync(T entity);
		Task DeleteAsync(int id);
		Task SaveChangesAsync();
	}
} 