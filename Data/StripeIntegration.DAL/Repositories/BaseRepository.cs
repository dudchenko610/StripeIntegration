using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using StripeIntegration.DAL.Repositories.Interfaces;
using StripeIntegration.Entities.Interface;

namespace StripeIntegration.DAL.Repositories;

public abstract class BaseRepository<TEntity> : IBaseRepository<TEntity>
    where TEntity : class, IBaseEntity
{
    protected readonly DatabaseContext _context;
    protected readonly DbSet<TEntity> _dbSet;

    public BaseRepository(DatabaseContext context)
    {
        _context = context;
        _dbSet = context.Set<TEntity>();
    }

    public virtual async Task CreateAsync(TEntity item)
    {
        await _dbSet.AddAsync(item);
        await _context.SaveChangesAsync();
    }

    public virtual async Task DeleteAsync(TEntity item)
    {
        _dbSet.Remove(item);
        await _context.SaveChangesAsync();
    }

    public virtual async Task<IEnumerable<TEntity>> GetAllAsync()
    {
        return await _dbSet.ToListAsync();
    }

    public virtual async Task UpdateAsync(TEntity item)
    {
        _context.Entry(item).State = EntityState.Modified;
        await _context.SaveChangesAsync();
    }

    public virtual async Task CreateRangeAsync(IEnumerable<TEntity> item)
    {
        await _dbSet.AddRangeAsync(item);
        await _context.SaveChangesAsync();
    }

    public virtual async Task UpdateRangeAsync(IEnumerable<TEntity> item)
    {
        _dbSet.UpdateRange(item);
        await _context.SaveChangesAsync();
    }

    public virtual Task<TEntity> GetByIdAsync(Guid id)
    {
        var res = _dbSet.FirstOrDefaultAsync(x => x.Id == id);
        return res!;
    }

    public virtual async Task RemoveRangeAsync(IList<TEntity> items)
    {
        _dbSet.RemoveRange(items);
        await _context.SaveChangesAsync();
    }

    public virtual Task<int> GetCountAsync()
    {
        return _dbSet.CountAsync();
    }

    public Task<List<TEntity>> GetAsync(Expression<Func<TEntity, bool>> predicate)
    {
        return _dbSet.Where(predicate).ToListAsync();
    }
}
