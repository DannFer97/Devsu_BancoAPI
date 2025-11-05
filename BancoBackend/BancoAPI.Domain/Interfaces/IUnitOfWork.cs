using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BancoAPI.Domain.Interfaces
{
    /// <summary>
    /// Interfaz Unit of Work para coordinar transacciones y persistencia
    /// </summary>
    public interface IUnitOfWork : IDisposable
    {
        /// Repositorio de Clientes
        IClienteRepository Clientes { get; }

        /// Repositorio de Cuentas
        ICuentaRepository Cuentas { get; }

        /// Repositorio de Movimientos
        IMovimientoRepository Movimientos { get; }

        /// Guarda todos los cambios realizados en la unidad de trabajo
        Task<int> SaveChangesAsync();

        /// Inicia una transacción de base de datos
        Task BeginTransactionAsync();

        /// Confirma la transacción actual
        Task CommitTransactionAsync();

        /// Revierte la transacción actual
        Task RollbackTransactionAsync();
    }
}
