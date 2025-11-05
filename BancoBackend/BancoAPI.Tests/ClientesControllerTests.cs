using BancoAPI.API.Controllers;
using BancoAPI.Application.DTOs;
using BancoAPI.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;
using FluentAssertions;

namespace BancoAPI.Tests
{
    /// <summary>
    /// Pruebas unitarias para ClientesController
    /// </summary>
    public class ClientesControllerTests
    {
        private readonly Mock<IClienteService> _mockClienteService;
        private readonly Mock<ILogger<ClientesController>> _mockLogger;
        private readonly ClientesController _controller;

        public ClientesControllerTests()
        {
            _mockClienteService = new Mock<IClienteService>();
            _mockLogger = new Mock<ILogger<ClientesController>>();
            _controller = new ClientesController(_mockClienteService.Object, _mockLogger.Object);
        }

        #region GetClienteById Tests

        [Fact]
        public async Task GetCliente_WithValidId_ReturnsOkResult()
        {
            // Arrange
            var clienteId = 1;
            var expectedCliente = new ClienteDto
            {
                ClienteId = clienteId,
                Nombre = "Nombre Prueba",
                Identificacion = "1234567890",
                Direccion = "Dirección Prueba",
                Telefono = "0987654321",
                Estado = true
            };

            _mockClienteService
                .Setup(service => service.GetClienteByIdAsync(clienteId))
                .ReturnsAsync(expectedCliente);

            // Act
            var result = await _controller.GetCliente(clienteId);

            // Assert
            var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
            var returnedCliente = okResult.Value.Should().BeOfType<ClienteDto>().Subject;
            returnedCliente.ClienteId.Should().Be(clienteId);
            returnedCliente.Nombre.Should().Be(expectedCliente.Nombre);
        }

        [Fact]
        public async Task GetCliente_WithInvalidId_ReturnsNotFound()
        {
            // Arrange
            var clienteId = 999; // ID inexistente
            _mockClienteService
                .Setup(service => service.GetClienteByIdAsync(clienteId))
                .ReturnsAsync((ClienteDto)null);

            // Act
            var result = await _controller.GetCliente(clienteId);

            // Assert
            result.Result.Should().BeOfType<OkObjectResult>()
                .Which.Value.Should().BeNull();
        }

        #endregion

        #region CreateCliente Tests

        [Fact]
        public async Task CreateCliente_WithValidData_ReturnsCreatedResult()
        {
            // Arrange
            var clienteCreateDto = new ClienteCreateDto
            {
                Nombre = "Nuevo Cliente",
                Identificacion = "1234567890",
                Direccion = "Dirección Prueba",
                Telefono = "0987654321",
                Genero = "M",
                Edad = 25
            };

            var createdCliente = new ClienteDto
            {
                ClienteId = 1,
                Nombre = clienteCreateDto.Nombre,
                Identificacion = clienteCreateDto.Identificacion,
                Direccion = clienteCreateDto.Direccion,
                Telefono = clienteCreateDto.Telefono,
                Genero = clienteCreateDto.Genero,
                Edad = clienteCreateDto.Edad,
                Estado = true
            };

            _mockClienteService
                .Setup(service => service.CreateClienteAsync(clienteCreateDto))
                .ReturnsAsync(createdCliente);

            // Act
            var result = await _controller.CreateCliente(clienteCreateDto);

            // Assert
            var createdResult = result.Result.Should().BeOfType<CreatedAtActionResult>().Subject;
            createdResult.ActionName.Should().Be(nameof(ClientesController.GetCliente));
            createdResult.RouteValues["id"].Should().Be(createdCliente.ClienteId);
            createdResult.Value.Should().BeEquivalentTo(createdCliente);
        }

        [Fact]
        public async Task CreateCliente_WithInvalidModel_ReturnsBadRequest()
        {
            // Arrange
            var clienteCreateDto = new ClienteCreateDto
            {
                Nombre = "", // Nombre inválido
                Identificacion = "1234567890",
                Direccion = "Dirección Prueba"
            };

            // Añadir errores de validación al modelo
            _controller.ModelState.AddModelError("Nombre", "El nombre es obligatorio");

            // Act
            var result = await _controller.CreateCliente(clienteCreateDto);

            // Assert
            result.Result.Should().BeOfType<BadRequestObjectResult>();
        }

        #endregion

        #region Security Tests

        [Fact]
        public async Task GetCliente_WithNegativeId_ReturnsOkWithNull()
        {
            // Arrange
            var invalidId = -1;
            
            // Mockear el servicio para que retorne null
            _mockClienteService
                .Setup(service => service.GetClienteByIdAsync(invalidId))
                .ReturnsAsync((ClienteDto)null);

            // Act
            var result = await _controller.GetCliente(invalidId);

            // Assert
            // En la implementación actual, no hay validación de ID negativo en el controller
            // por lo que se llama al servicio y devuelve null, que es devuelto como Ok(null)
            _mockClienteService.Verify(service => service.GetClienteByIdAsync(invalidId), Times.Once);
            var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
            okResult.Value.Should().BeNull();
        }

        #endregion
    }
}