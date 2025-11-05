using BancoAPI.Domain.Interfaces;
using BancoAPI.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace BancoAPI.Infrastructure.Repositories
{
    /// <summary>
    /// Implementación genérica del patrón Repository
    /// </summary>

    public class Repository<T> : IRepository<T> where T : class
    {
        protected readonly BancoDbContext _context;
        protected readonly DbSet<T> _dbSet;

        public Repository(BancoDbContext context)
        {
            _context = context;
            _dbSet = context.Set<T>();
        }

        /// Obtiene todas las entidades - Uso de async/await 
        public virtual async Task<IEnumerable<T>> GetAllAsync()
        {
            return await _dbSet.ToListAsync();
        }

        /// Obtiene una entidad por su ID
        public virtual async Task<T?> GetByIdAsync(int id)
        {
            return await _dbSet.FindAsync(id);
        }

        /// Busca entidades 
        public virtual async Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate)
        {
            return await _dbSet.Where(predicate).ToListAsync();
        }

        /// Obtiene la primera entidad que cumpla
        public virtual async Task<T?> FirstOrDefaultAsync(Expression<Func<T, bool>> predicate)
        {
            return await _dbSet.FirstOrDefaultAsync(predicate);
        }

        /// Agrega una nueva entidad
        public virtual async Task<T> AddAsync(T entity)
        {
            await _dbSet.AddAsync(entity);
            return entity;
        }

        /// Actualiza una entidad existente
        public virtual Task UpdateAsync(T entity)
        {
            _dbSet.Update(entity);
            return Task.CompletedTask;
        }

        /// Elimina una entidad
        public virtual Task DeleteAsync(T entity)
        {
            _dbSet.Remove(entity);
            return Task.CompletedTask;
        }

        /// Verifica si existe alguna entidad que cumpla
        public virtual async Task<bool> AnyAsync(Expression<Func<T, bool>> predicate)
        {
            return await _dbSet.AnyAsync(predicate);
        }

        /// Cuenta cuántas entidades cumplan
        public virtual async Task<int> CountAsync(Expression<Func<T, bool>> predicate)
        {
            return await _dbSet.CountAsync(predicate);
        }
    }
}
