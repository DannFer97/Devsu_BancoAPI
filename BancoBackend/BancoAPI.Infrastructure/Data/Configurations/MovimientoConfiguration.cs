using BancoAPI.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BancoAPI.Infrastructure.Data.Configurations
{
    /// <summary>
    /// Configuración de Fluent API para la entidad Movimiento
    /// </summary>
    public class MovimientoConfiguration : IEntityTypeConfiguration<Movimiento>
    {
        public void Configure(EntityTypeBuilder<Movimiento> builder)
        {
            // Nombre de la tabla
            builder.ToTable("Movimientos");

            // Clave primaria
            builder.HasKey(m => m.MovimientoId);

            // Propiedades
            builder.Property(m => m.Fecha)
                .IsRequired()
                .HasDefaultValueSql("GETDATE()"); // SQL Server

            builder.Property(m => m.TipoMovimiento)
                .IsRequired()
                .HasMaxLength(20)
                .HasConversion<string>(); 

            builder.Property(m => m.Valor)
                .IsRequired()
                .HasPrecision(18, 2);

            builder.Property(m => m.Saldo)
                .IsRequired()
                .HasPrecision(18, 2);

            // Índices para optimizar consultas
            builder.HasIndex(m => m.CuentaId)
                .HasDatabaseName("IX_Movimientos_CuentaId");

            builder.HasIndex(m => m.Fecha)
                .HasDatabaseName("IX_Movimientos_Fecha");

            // Índice compuesto para consultas de reportes
            builder.HasIndex(m => new { m.CuentaId, m.Fecha })
                .HasDatabaseName("IX_Movimientos_CuentaId_Fecha");

            // Relaciones
            builder.HasOne(m => m.Cuenta)
                .WithMany(c => c.Movimientos)
                .HasForeignKey(m => m.CuentaId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
