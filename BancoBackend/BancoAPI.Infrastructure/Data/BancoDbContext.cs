using BancoAPI.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;


namespace BancoAPI.Infrastructure.Data
{
    /// <summary>
    /// Contexto de base de datos para el sistema bancario
    /// </summary>
    public class BancoDbContext : DbContext
    {
        public BancoDbContext(DbContextOptions<BancoDbContext> options) : base(options)
        {
        }

        /// DbSet de Personas
        public DbSet<Persona> Personas { get; set; } = null!;

        /// DbSet de Clientes
        public DbSet<Cliente> Clientes { get; set; } = null!;

        /// DbSet de Cuentas
        public DbSet<Cuenta> Cuentas { get; set; } = null!;

        /// DbSet de Movimientos
        public DbSet<Movimiento> Movimientos { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configuración de herencia: Table-per-Type (TPT) - Mejores prácticas EF Core
            // La tabla Personas contiene la PK y propiedades base
            // La tabla Clientes comparte la misma PK como FK hacia Personas
            modelBuilder.Entity<Persona>()
                .ToTable("Personas")
                .HasKey(p => p.PersonaId);

            modelBuilder.Entity<Cliente>()
                .ToTable("Clientes");

            // Aplicar todas las configuraciones de Fluent API
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(BancoDbContext).Assembly);

            // Índices únicos para mejorar rendimiento y garantizar unicidad
            modelBuilder.Entity<Persona>()
                .HasIndex(p => p.Identificacion)
                .IsUnique()
                .HasDatabaseName("IX_Personas_Identificacion");

            modelBuilder.Entity<Cuenta>()
                .HasIndex(c => c.NumeroCuenta)
                .IsUnique()
                .HasDatabaseName("IX_Cuentas_NumeroCuenta");

            // Configurar precisión de decimales para evitar problemas de redondeo
            modelBuilder.Entity<Cuenta>()
                .Property(c => c.SaldoInicial)
                .HasPrecision(18, 2);

            modelBuilder.Entity<Movimiento>()
                .Property(m => m.Valor)
                .HasPrecision(18, 2);

            modelBuilder.Entity<Movimiento>()
                .Property(m => m.Saldo)
                .HasPrecision(18, 2);

        }
    }
}
