using BancoAPI.Application.DTOs;
using BancoAPI.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace BancoAPI.API.Controllers
{
    /// <summary>
    /// Controller REST para gestión de Clientes
    /// Endpoints: GET, POST, PUT, PATCH, DELETE
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class ClientesController : ControllerBase
    {
        private readonly IClienteService _clienteService;
        private readonly ILogger<ClientesController> _logger;

        public ClientesController(IClienteService clienteService, ILogger<ClientesController> logger)
        {
            _clienteService = clienteService;
            _logger = logger;
        }

        /// <summary>
        /// Obtiene todos los clientes
        /// </summary>

        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<ClienteDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<ClienteDto>>> GetClientes()
        {
            _logger.LogInformation("GET /api/clientes - Obteniendo todos los clientes");
            var clientes = await _clienteService.GetAllClientesAsync();
            return Ok(clientes);
        }

        /// <summary>
        /// Obtiene un cliente por ID
        /// </summary>
 
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ClienteDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ClienteDto>> GetCliente(int id)
        {
            _logger.LogInformation("GET /api/clientes/{Id} - Obteniendo cliente", id);
            var cliente = await _clienteService.GetClienteByIdAsync(id);
            return Ok(cliente);
        }

        /// <summary>
        /// Busca un cliente por identificación
        /// </summary>
  
        [HttpGet("identificacion/{identificacion}")]
        [ProducesResponseType(typeof(ClienteDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ClienteDto>> GetClienteByIdentificacion(string identificacion)
        {
            _logger.LogInformation("GET /api/clientes/identificacion/{Identificacion}", identificacion);
            var cliente = await _clienteService.GetClienteByIdentificacionAsync(identificacion);

            if (cliente == null)
                return NotFound(new { error = $"Cliente con identificación {identificacion} no encontrado" });

            return Ok(cliente);
        }

        /// <summary>
        /// Crea un nuevo cliente
        /// </summary>
 
        [HttpPost]
        [ProducesResponseType(typeof(ClienteDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<ActionResult<ClienteDto>> CreateCliente([FromBody] ClienteCreateDto clienteDto)
        {
            _logger.LogInformation("POST /api/clientes - Creando nuevo cliente");

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var cliente = await _clienteService.CreateClienteAsync(clienteDto);

            return CreatedAtAction(
                nameof(GetCliente),
                new { id = cliente.ClienteId },
                cliente);
        }

        /// <summary>
        /// Actualiza un cliente existente (PUT)
        /// </summary>

        [HttpPut("{id}")]
        [ProducesResponseType(typeof(ClienteDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ClienteDto>> UpdateCliente(int id, [FromBody] ClienteUpdateDto clienteDto)
        {
            _logger.LogInformation("PUT /api/clientes/{Id} - Actualizando cliente", id);

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var cliente = await _clienteService.UpdateClienteAsync(id, clienteDto);
            return Ok(cliente);
        }

        /// <summary>
        /// Actualiza parcialmente un cliente (PATCH)
        /// </summary>
   
        [HttpPatch("{id}")]
        [ProducesResponseType(typeof(ClienteDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ClienteDto>> PatchCliente(int id, [FromBody] ClienteUpdateDto clienteDto)
        {
            _logger.LogInformation("PATCH /api/clientes/{Id} - Actualizando parcialmente cliente", id);

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var cliente = await _clienteService.UpdateClienteAsync(id, clienteDto);
            return Ok(cliente);
        }

        /// <summary>
        /// Elimina un cliente
        /// </summary>

        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> DeleteCliente(int id)
        {
            _logger.LogInformation("DELETE /api/clientes/{Id} - Eliminando cliente", id);

            await _clienteService.DeleteClienteAsync(id);
            return NoContent();
        }
    }
}
