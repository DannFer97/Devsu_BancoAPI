using BancoAPI.Application.DTOs;
using BancoAPI.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace BancoAPI.API.Controllers
{
    /// <summary>
    /// Controller REST para gestión de Movimientos (Transacciones)
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class MovimientosController : ControllerBase
    {
        private readonly IMovimientoService _movimientoService;
        private readonly ILogger<MovimientosController> _logger;

        public MovimientosController(IMovimientoService movimientoService, ILogger<MovimientosController> logger)
        {
            _movimientoService = movimientoService;
            _logger = logger;
        }

        /// <summary>
        /// Obtiene todos los movimientos
        /// </summary>
 
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<MovimientoDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<MovimientoDto>>> GetMovimientos()
        {
            _logger.LogInformation("GET /api/movimientos - Obteniendo todos los movimientos");
            var movimientos = await _movimientoService.GetAllMovimientosAsync();
            return Ok(movimientos);
        }

        /// <summary>
        /// Obtiene un movimiento por ID
        /// </summary>

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(MovimientoDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<MovimientoDto>> GetMovimiento(int id)
        {
            _logger.LogInformation("GET /api/movimientos/{Id} - Obteniendo movimiento", id);
            var movimiento = await _movimientoService.GetMovimientoByIdAsync(id);
            return Ok(movimiento);
        }

        /// <summary>
        /// Obtiene todos los movimientos de una cuenta
        /// </summary>

        [HttpGet("cuenta/{cuentaId}")]
        [ProducesResponseType(typeof(IEnumerable<MovimientoDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<MovimientoDto>>> GetMovimientosByCuenta(int cuentaId)
        {
            _logger.LogInformation("GET /api/movimientos/cuenta/{CuentaId}", cuentaId);
            var movimientos = await _movimientoService.GetMovimientosByCuentaAsync(cuentaId);
            return Ok(movimientos);
        }

        /// <summary>
        /// Crea un nuevo movimiento (transacción)
        /// 
        /// </summary>

        [HttpPost]
        [ProducesResponseType(typeof(MovimientoDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<MovimientoDto>> CreateMovimiento([FromBody] MovimientoCreateDto movimientoDto)
        {
            _logger.LogInformation("POST /api/movimientos - Creando nuevo movimiento");
            _logger.LogInformation("CuentaId: {CuentaId}, Tipo: {Tipo}, Valor: {Valor}",
                movimientoDto.CuentaId,
                movimientoDto.TipoMovimiento,
                movimientoDto.Valor);

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var movimiento = await _movimientoService.CreateMovimientoAsync(movimientoDto);

            _logger.LogInformation("Movimiento creado exitosamente. Saldo resultante: {Saldo}", movimiento.Saldo);

            return CreatedAtAction(
                nameof(GetMovimiento),
                new { id = movimiento.MovimientoId },
                movimiento);
        }

        /// <summary>
        /// Elimina un movimiento
        /// Solo permite eliminar el último movimiento de la cuenta
        /// </summary>
 
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> DeleteMovimiento(int id)
        {
            _logger.LogInformation("DELETE /api/movimientos/{Id} - Eliminando movimiento", id);

            await _movimientoService.DeleteMovimientoAsync(id);
            return NoContent();
        }
    }
}
