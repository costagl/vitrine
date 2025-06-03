using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using VitrineApi.Data;
using VitrineApi.Interfaces;

public class RepositoryBase<T> : IRepositoryBase<T>, IDisposable where T : class
{
    protected readonly VitrineDBContext _context;
    protected readonly DbSet<T> _dbSet;
    private bool _disposed = false;

    public RepositoryBase(VitrineDBContext context)
    {
        _context = context;
        _dbSet = context.Set<T>();
    }

    public async Task<T> IncluirAsync(T entidade)
    {
        await _dbSet.AddAsync(entidade);
        await _context.SaveChangesAsync();
        return entidade;
    }

    public async Task<T?> BuscarPorIdAsync(int id)
    {
        return await _dbSet.FindAsync(id);
    }

    public async Task<IEnumerable<T>> ListarAsync(Expression<Func<T, bool>>? filtro = null)
    {
        if (filtro != null)
            return await _dbSet.Where(filtro).ToListAsync();

        return await _dbSet.ToListAsync();
    }

    public async Task AtualizarAsync(T entidade)
    {
        _dbSet.Update(entidade);
        await _context.SaveChangesAsync();
    }

    public async Task RemoverAsync(T entidade)
    {
        _dbSet.Remove(entidade);
        await _context.SaveChangesAsync();
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposed)
        {
            if (disposing)
            {
                _context.Dispose();
            }
            _disposed = true;
        }
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }
}
