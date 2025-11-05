using AutoMapper;
using BancoAPI.Application.DTOs;
using BancoAPI.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BancoAPI.Application.Mappings
{
    /// <summary>
    /// Perfiles de AutoMapper para mapeo entre entidades y DTOs
    /// </summary>
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {

            // Mapeos de Cliente
            CreateMap<Cliente, ClienteDto>()
                .ForMember(dest => dest.ClienteId, opt => opt.MapFrom(src => src.PersonaId))  // Mapear PersonaId a ClienteId
                .ForMember(dest => dest.Cuentas,
                    opt => opt.MapFrom(src => src.Cuentas));

            CreateMap<ClienteCreateDto, Cliente>()
                .ForMember(dest => dest.PersonaId, opt => opt.Ignore())  // PK generada por DB
                .ForMember(dest => dest.Cuentas, opt => opt.Ignore());

            CreateMap<ClienteUpdateDto, Cliente>()
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

            // Mapeos de Cuenta

            CreateMap<Cuenta, CuentaDto>()
                .ForMember(dest => dest.NombreCliente,
                    opt => opt.MapFrom(src => src.Cliente != null ? src.Cliente.Nombre : null))
                .ForMember(dest => dest.Movimientos,
                    opt => opt.MapFrom(src => src.Movimientos));

            CreateMap<CuentaCreateDto, Cuenta>()
                .ForMember(dest => dest.CuentaId, opt => opt.Ignore())
                .ForMember(dest => dest.Cliente, opt => opt.Ignore())
                .ForMember(dest => dest.Movimientos, opt => opt.Ignore());

            CreateMap<CuentaUpdateDto, Cuenta>()
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

            // Mapeos de Movimiento

            CreateMap<Movimiento, MovimientoDto>()
                .ForMember(dest => dest.NumeroCuenta,
                    opt => opt.MapFrom(src => src.Cuenta != null ? src.Cuenta.NumeroCuenta : null))
                .ForMember(dest => dest.NombreCliente,
                    opt => opt.MapFrom(src => src.Cuenta != null && src.Cuenta.Cliente != null
                        ? src.Cuenta.Cliente.Nombre
                        : null));

            CreateMap<MovimientoCreateDto, Movimiento>()
                .ForMember(dest => dest.MovimientoId, opt => opt.Ignore())
                .ForMember(dest => dest.Fecha, opt => opt.Ignore())
                .ForMember(dest => dest.Saldo, opt => opt.Ignore())
                .ForMember(dest => dest.Cuenta, opt => opt.Ignore());
        }
    }
}
