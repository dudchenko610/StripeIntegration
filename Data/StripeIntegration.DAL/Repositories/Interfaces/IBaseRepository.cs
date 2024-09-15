using System.Linq.Expressions;
using StripeIntegration.Entities.Interface;

namespace StripeIntegration.DAL.Repositories.Interfaces;

public interface IBaseRepository<TEntity> where TEntity : class, IBaseEntity
{
    public Task<TEntity> GetByIdAsync(Guid id);
    public Task UpdateAsync(TEntity item);
    public Task CreateAsync(TEntity item);
    public Task<IEnumerable<TEntity>> GetAllAsync();
    public Task<List<TEntity>> GetAsync(Expression<Func<TEntity, bool>> predicate);
    public Task DeleteAsync(TEntity item);
    public Task CreateRangeAsync(IEnumerable<TEntity> item);
    public Task UpdateRangeAsync(IEnumerable<TEntity> item);
    public Task RemoveRangeAsync(IList<TEntity> items);
    public Task<int> GetCountAsync();
}