using BancoAPI.Domain.Interfaces;
using BancoAPI.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Storage;

namespace BancoAPI.Infrastructure.Repositories
{
    /// <summary>
    /// Implementación del patrón Unit of Work 
    /// Coordina el trabajo de múltiples repositorios y maneja transacciones
    /// </summary>
    public class UnitOfWork : IUnitOfWork
    {
        private readonly BancoDbContext _context;
        private IDbContextTransaction? _transaction;

        // Lazy initialization de repositorios
        private IClienteRepository? _clientes;
        private ICuentaRepository? _cuentas;
        private IMovimientoRepository? _movimientos;

        public UnitOfWork(BancoDbContext context)
        {
            _context = context;
        }

        /// Repositorio de Clientes 
        public IClienteRepository Clientes
        {
            get
            {
                _clientes ??= new ClienteRepository(_context);
                return _clientes;
            }
        }
 
        /// Repositorio de Cuentas 
        public ICuentaRepository Cuentas
        {
            get
            {
                _cuentas ??= new CuentaRepository(_context);
                return _cuentas;
            }
        }

        /// Repositorio de Movimientos 
        public IMovimientoRepository Movimientos
        {
            get
            {
                _movimientos ??= new MovimientoRepository(_context);
                return _movimientos;
            }
        }

        /// Guarda todos los cambios realizados en la unidad de trabajo
        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }

        /// Inicia una transacción de base de datos
        public async Task BeginTransactionAsync()
        {
            _transaction = await _context.Database.BeginTransactionAsync();
        }

        /// Confirma la transacción actual
        public async Task CommitTransactionAsync()
        {
            try
            {
                await _context.SaveChangesAsync();

                if (_transaction != null)
                {
                    await _transaction.CommitAsync();
                }
            }
            catch
            {
                await RollbackTransactionAsync();
                throw;
            }
            finally
            {
                if (_transaction != null)
                {
                    await _transaction.DisposeAsync();
                    _transaction = null;
                }
            }
        }

        /// Revierte la transacción actual
        public async Task RollbackTransactionAsync()
        {
            if (_transaction != null)
            {
                await _transaction.RollbackAsync();
                await _transaction.DisposeAsync();
                _transaction = null;
            }
        }

        /// Libera los recursos utilizados
        public void Dispose()
        {
            _transaction?.Dispose();
            _context.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}
