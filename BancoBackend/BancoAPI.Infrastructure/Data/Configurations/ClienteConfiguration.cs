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
    /// Configuración de Fluent API para la entidad Cliente
    /// </summary>
    public class ClienteConfiguration : IEntityTypeConfiguration<Cliente>
    {
        public void Configure(EntityTypeBuilder<Cliente> builder)
        {
            // Nombre de la tabla
            builder.ToTable("Clientes");

            // Propiedades específicas de Cliente
            builder.Property(c => c.Contrasena)
                .IsRequired()
                .HasMaxLength(100)
                .HasColumnName("Contrasena");

            builder.Property(c => c.Estado)
                .IsRequired()
                .HasDefaultValue(true)
                .HasColumnName("Estado");

        }
    }
}
