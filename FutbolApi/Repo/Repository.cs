using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Numerics;
using System.Threading.Tasks;
using FutbolApi.Data;
using Microsoft.EntityFrameworkCore;

namespace FutbolApi.Repositories
{
    public class Repository<T> : IRepository<T> where T : class
    {
    private readonly ApplicationDbContext _context;
    private readonly DbSet<T> _dbSet;

    public Repository(ApplicationDbContext context)
    {
        _context = context;
        _dbSet = _context.Set<T>();
    }

    public async Task<T> GetByIdAsync(int id)
    {
        return await _dbSet.FindAsync(id);
    }

    public async Task<IEnumerable<T>> GetAllAsync()
    {
        return await _dbSet.ToListAsync();
    }

    public async Task AddAsync(T entity)
    {
        await _dbSet.AddAsync(entity);
    }

    public async Task AddRangeAsync(IEnumerable<T> entities) // Toplu ekleme metodu
    {
        await _dbSet.AddRangeAsync(entities);
    }

    public async Task Update(T entity)
    {
        _dbSet.Update(entity);
    }

    public async Task UpdateRange(List<T> entitites)
    {
        _dbSet.UpdateRange(entitites);
    }

    public IQueryable<T> GetDb()
    {
        return _dbSet;
    }

    public async Task Remove(T entity)
    {
        _dbSet.Remove(entity);
    }

    public async Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate)
    {
        return await _dbSet.Where(predicate).ToListAsync();
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}

    }

