using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Task2Tracker.Application.Common.Interfaces;
using Task2Tracker.Domain.Common;

namespace Task2Tracker.Infrastructure.Persistence.Repositories;

public class GenericRepository<T> : IGenericRepository<T> where T : BaseEntity
{
    protected readonly ApplicationDbContext _context;
    protected readonly DbSet<T> _dbSet;

    public GenericRepository(ApplicationDbContext context)
    {
        _context = context;
        _dbSet = context.Set<T>();
    }

    public async Task<T?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _dbSet.FindAsync(new object[] { id }, cancellationToken);
    }

    public async Task<IReadOnlyList<T>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var list = await _dbSet.ToListAsync(cancellationToken);
        return list.AsReadOnly();
    }

    // DOĞRU KOD: Arayüzün beklediği asenkron Task<IReadOnlyList<T>> yapısına tamamen eşitlendi.
    public async Task<IReadOnlyList<T>> GetWhere(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default)
    {
        var list = await _dbSet.Where(predicate).ToListAsync(cancellationToken);
        return list.AsReadOnly();
    }

    public async Task AddAsync(T entity, CancellationToken cancellationToken = default)
    {
        await _dbSet.AddAsync(entity, cancellationToken);
    }

    public void Update(T entity)
    {
        _dbSet.Update(entity);
    }

    public void Delete(T entity)
    {
        _dbSet.Remove(entity);
    }
}