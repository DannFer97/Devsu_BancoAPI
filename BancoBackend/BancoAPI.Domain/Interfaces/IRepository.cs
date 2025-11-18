using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;


namespace BancoAPI.Domain.Interfaces
{
    /// <summary>
    /// Interfaz genérica para operaciones de repositorio CRUD
    /// Implementa el patrón Repository con uso de expresiones lambda 
    /// </summary>

    public interface IRepository<T> where T : class
    {

        /// Obtiene todas las entidades
        Task<IEnumerable<T>> GetAllAsync();

        /// Obtiene una entidad por su ID
        Task<T?> GetByIdAsync(int id);

        /// Busca entidades que cumplan 
        Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate);

        /// Obtiene la primera entidad que cumplan
        Task<T?> FirstOrDefaultAsync(Expression<Func<T, bool>> predicate);

        /// Agrega una nueva entidad
        Task<T> AddAsync(T entity);

        /// Actualiza una entidad existente
        Task UpdateAsync(T entity);

        /// Elimina una entidad
        Task DeleteAsync(T entity);

        /// Verifica si existe alguna entidad que cumplan
        Task<bool> AnyAsync(Expression<Func<T, bool>> predicate);

        /// Cuenta cuántas entidades cumplan
        Task<int> CountAsync(Expression<Func<T, bool>> predicate);
    }
}
