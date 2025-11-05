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
    /// Configuración de Fluent API para la entidad Cuenta
    /// </summary>
    public class CuentaConfiguration : IEntityTypeConfiguration<Cuenta>
    {
        public void Configure(EntityTypeBuilder<Cuenta> builder)
        {
            // Nombre de la tabla
            builder.ToTable("Cuentas");

            // Clave primaria
            builder.HasKey(c => c.CuentaId);

            // Propiedades
            builder.Property(c => c.NumeroCuenta)
                .IsRequired()
                .HasMaxLength(20);

            builder.Property(c => c.TipoCuenta)
                .IsRequired()
                .HasMaxLength(20)
                .HasConversion<string>();  

            builder.Property(c => c.SaldoInicial)
                .IsRequired()
                .HasPrecision(18, 2);

            builder.Property(c => c.Estado)
                .IsRequired()
                .HasDefaultValue(true);

            // Índices
            builder.HasIndex(c => c.NumeroCuenta)
                .IsUnique()
                .HasDatabaseName("IX_Cuentas_NumeroCuenta");

            builder.HasIndex(c => c.ClienteId)
                .HasDatabaseName("IX_Cuentas_ClienteId");

            // Relaciones: Una cuenta pertenece a un cliente
            // ClienteId es FK que referencia a PersonaId (PK de Cliente en herencia TPT)
            builder.HasOne(c => c.Cliente)
                .WithMany(cl => cl.Cuentas)
                .HasForeignKey(c => c.ClienteId)
                .HasPrincipalKey(cl => cl.PersonaId)  
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(c => c.Movimientos)
                .WithOne(m => m.Cuenta)
                .HasForeignKey(m => m.CuentaId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
