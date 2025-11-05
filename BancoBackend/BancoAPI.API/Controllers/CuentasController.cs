using BancoAPI.Application.DTOs;
using BancoAPI.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace BancoAPI.API.Controllers
{
    /// <summary>
    /// Controller REST para gestión de Cuentas
    /// Endpoints: GET, POST, PUT, PATCH, DELETE
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class CuentasController : ControllerBase
    {
        private readonly ICuentaService _cuentaService;
        private readonly ILogger<CuentasController> _logger;

        public CuentasController(ICuentaService cuentaService, ILogger<CuentasController> logger)
        {
            _cuentaService = cuentaService;
            _logger = logger;
        }

        /// <summary>
        /// Obtiene todas las cuentas
        /// </summary>
             
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<CuentaDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<CuentaDto>>> GetCuentas()
        {
            _logger.LogInformation("GET /api/cuentas - Obteniendo todas las cuentas");
            var cuentas = await _cuentaService.GetAllCuentasAsync();
            return Ok(cuentas);
        }

        /// <summary>
        /// Obtiene una cuenta por ID
        /// </summary>
                             
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(CuentaDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<CuentaDto>> GetCuenta(int id)
        {
            _logger.LogInformation("GET /api/cuentas/{Id} - Obteniendo cuenta", id);
            var cuenta = await _cuentaService.GetCuentaByIdAsync(id);
            return Ok(cuenta);
        }

        /// <summary>
        /// Busca una cuenta por número de cuenta
        /// </summary>

        [HttpGet("numero/{numeroCuenta}")]
        [ProducesResponseType(typeof(CuentaDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<CuentaDto>> GetCuentaByNumero(string numeroCuenta)
        {
            _logger.LogInformation("GET /api/cuentas/numero/{NumeroCuenta}", numeroCuenta);
            var cuenta = await _cuentaService.GetCuentaByNumeroAsync(numeroCuenta);

            if (cuenta == null)
                return NotFound(new { error = $"Cuenta {numeroCuenta} no encontrada" });

            return Ok(cuenta);
        }

        /// <summary>
        /// Obtiene todas las cuentas de un cliente
        /// </summary>
 
        [HttpGet("cliente/{clienteId}")]
        [ProducesResponseType(typeof(IEnumerable<CuentaDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<CuentaDto>>> GetCuentasByCliente(int clienteId)
        {
            _logger.LogInformation("GET /api/cuentas/cliente/{ClienteId}", clienteId);
            var cuentas = await _cuentaService.GetCuentasByClienteIdAsync(clienteId);
            return Ok(cuentas);
        }

        /// <summary>
        /// Crea una nueva cuenta
        /// </summary>
  
        [HttpPost]
        [ProducesResponseType(typeof(CuentaDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<ActionResult<CuentaDto>> CreateCuenta([FromBody] CuentaCreateDto cuentaDto)
        {
            _logger.LogInformation("POST /api/cuentas - Creando nueva cuenta");

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var cuenta = await _cuentaService.CreateCuentaAsync(cuentaDto);

            return CreatedAtAction(
                nameof(GetCuenta),
                new { id = cuenta.CuentaId },
                cuenta);
        }

        /// <summary>
        /// Actualiza una cuenta existente (PUT)
        /// </summary>
 
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(CuentaDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<CuentaDto>> UpdateCuenta(int id, [FromBody] CuentaUpdateDto cuentaDto)
        {
            _logger.LogInformation("PUT /api/cuentas/{Id} - Actualizando cuenta", id);

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var cuenta = await _cuentaService.UpdateCuentaAsync(id, cuentaDto);
            return Ok(cuenta);
        }

        /// <summary>
        /// Actualiza parcialmente una cuenta (PATCH)
        /// </summary>

        [HttpPatch("{id}")]
        [ProducesResponseType(typeof(CuentaDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<CuentaDto>> PatchCuenta(int id, [FromBody] CuentaUpdateDto cuentaDto)
        {
            _logger.LogInformation("PATCH /api/cuentas/{Id} - Actualizando parcialmente cuenta", id);

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var cuenta = await _cuentaService.UpdateCuentaAsync(id, cuentaDto);
            return Ok(cuenta);
        }

        /// <summary>
        /// Elimina una cuenta
        /// </summary>

        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> DeleteCuenta(int id)
        {
            _logger.LogInformation("DELETE /api/cuentas/{Id} - Eliminando cuenta", id);

            await _cuentaService.DeleteCuentaAsync(id);
            return NoContent();
        }
    }
}
