using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BancoAPI.Domain.Exceptions
{
    /// <summary>
    /// Excepción base para todas las excepciones de dominio del sistema bancario.
    /// </summary>
    [Serializable]
    public abstract class DomainException : Exception
    {
        protected DomainException(string message) : base(message) { }
        public virtual string CodigoError => "ERROR_DOMINIO";
    }

    /// <summary>
    /// Excepción cuando el saldo de la cuenta es insuficiente.
    /// </summary>
    public class SaldoInsuficienteException : DomainException
    {
        public SaldoInsuficienteException()
            : base("Saldo no disponible") { }

        public SaldoInsuficienteException(decimal saldoActual, decimal montoRequerido)
            : base($"Saldo no disponible. Saldo actual: ${saldoActual:F2}, Monto requerido: ${montoRequerido:F2}") { }

        public override string CodigoError => "SALDO_INSUFICIENTE";
    }

    /// <summary>
    /// Excepción cuando se excede el límite diario de retiros.
    /// </summary>
    public class CupoDiarioExcedidoException : DomainException
    {
        public CupoDiarioExcedidoException()
            : base("Cupo diario excedido") { }

        public CupoDiarioExcedidoException(decimal montoRetiradoHoy, decimal limiteRetiro)
            : base($"Cupo diario excedido. Retirado hoy: ${montoRetiradoHoy:F2}, Límite: ${limiteRetiro:F2}") { }

        public override string CodigoError => "CUPO_DIARIO_EXCEDIDO";
    }

    /// <summary>
    /// Excepción cuando no se encuentra una entidad solicitada.
    /// </summary>
    public class EntidadNoEncontradaException : DomainException
    {
        public EntidadNoEncontradaException(string entidad, object id)
            : base($"{entidad} con ID {id} no fue encontrada") { }

        public override string CodigoError => "ENTIDAD_NO_ENCONTRADA";
    }

    /// <summary>
    /// Excepción cuando se intenta crear una entidad duplicada.
    /// </summary>
    public class EntidadDuplicadaException : DomainException
    {
        public EntidadDuplicadaException(string entidad, string campo, object valor)
            : base($"{entidad} con {campo} '{valor}' ya existe en el sistema") { }

        public override string CodigoError => "ENTIDAD_DUPLICADA";
    }

    /// <summary>
    /// Excepción para operaciones de negocio inválidas.
    /// </summary>
    public class OperacionInvalidaException : DomainException
    {
        public OperacionInvalidaException(string mensaje) : base(mensaje) { }
        public override string CodigoError => "OPERACION_INVALIDA";
    }
}
