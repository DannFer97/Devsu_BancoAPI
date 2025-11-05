/*
   Controlador para gestión de Clientes
  */

(function() {
    'use strict';

    angular.module('bancoApp')
        .controller('ClientesController', ClientesController);

    ClientesController.$inject = ['$scope', 'ClienteService', 'ModalService'];

    function ClientesController($scope, ClienteService, ModalService) {
        // Variables
        $scope.clientes = [];
        $scope.clientesFiltrados = [];
        $scope.clienteSeleccionado = null;
        $scope.nuevoCliente = {};
        $scope.modoEdicion = false;
        $scope.busqueda = '';
        $scope.loading = false;
        $scope.mensaje = null;
        $scope.error = null;
        $scope.modalActivo = false;

        // Funciones públicas
        $scope.cargarClientes = cargarClientes;
        $scope.buscarClientes = buscarClientes;
        $scope.abrirModalNuevo = abrirModalNuevo;
        $scope.abrirModalEditar = abrirModalEditar;
        $scope.cerrarModal = cerrarModal;
        $scope.guardarCliente = guardarCliente;
        $scope.eliminarCliente = eliminarCliente;
        $scope.limpiarMensajes = limpiarMensajes;

        // Inicialización
        init();


        function init() {
            cargarClientes();
        }

        // Cargar lista de clientes
        function cargarClientes() {
            $scope.loading = true;
            $scope.limpiarMensajes();

            ClienteService.getAll()
                .then(function(data) {
                    $scope.clientes = data;
                    $scope.clientesFiltrados = data;
                    $scope.loading = false;
                })
                .catch(function(error) {
                    $scope.error = 'Error al cargar los clientes. Verifique que el servidor esté funcionando.';
                    $scope.loading = false;
                    console.error('Error:', error);
                });
        }

        // Búsqueda rápida 
        function buscarClientes() {
            if (!$scope.busqueda || $scope.busqueda.trim() === '') {
                $scope.clientesFiltrados = $scope.clientes;
                return;
            }

            var termino = $scope.busqueda.toLowerCase();
            
            $scope.clientesFiltrados = $scope.clientes.filter(function(cliente) {
                return (cliente.nombre && cliente.nombre.toLowerCase().indexOf(termino) > -1) ||
                       (cliente.identificacion && cliente.identificacion.indexOf(termino) > -1) ||
                       (cliente.direccion && cliente.direccion.toLowerCase().indexOf(termino) > -1) ||
                       (cliente.telefono && cliente.telefono.indexOf(termino) > -1);
            });
        }

        // Abrir modal para nuevo cliente
        function abrirModalNuevo() {
            $scope.modoEdicion = false;
            $scope.nuevoCliente = {
                estado: true,
                genero: 'M',
                edad: 18
            };
            $scope.modalActivo = true;
            $scope.limpiarMensajes();
        }

        // Abrir modal para editar cliente
        function abrirModalEditar(cliente) {
            $scope.modoEdicion = true;
            $scope.clienteSeleccionado = cliente;
            
            // Copiar datos para edición
            $scope.nuevoCliente = {
                nombre: cliente.nombre,
                genero: cliente.genero,
                edad: cliente.edad,
                identificacion: cliente.identificacion,
                direccion: cliente.direccion,
                telefono: cliente.telefono,
                contrasena: cliente.contrasena,
                estado: cliente.estado
            };
            
            $scope.modalActivo = true;
            $scope.limpiarMensajes();
        }

        // Cerrar modal
        function cerrarModal() {
            $scope.modalActivo = false;
            $scope.nuevoCliente = {};
            $scope.clienteSeleccionado = null;
            $scope.limpiarMensajes();
        }

        // Guardar cliente (crear o actualizar)
        function guardarCliente() {
            // Validaciones básicas
            if (!validarCliente($scope.nuevoCliente)) {
                return;
            }

            $scope.loading = true;
            $scope.limpiarMensajes();

            if ($scope.modoEdicion) {
                // Actualizar cliente existente
                ClienteService.update($scope.clienteSeleccionado.clienteId, $scope.nuevoCliente)
                    .then(function(response) {
                        $scope.mensaje = 'Cliente actualizado exitosamente';
                        $scope.cerrarModal();
                        $scope.cargarClientes();
                    })
                    .catch(function(error) {
                        $scope.error = obtenerMensajeError(error);
                        $scope.loading = false;
                    });
            } else {
                // Crear nuevo cliente
                ClienteService.create($scope.nuevoCliente)
                    .then(function(response) {
                        $scope.mensaje = 'Cliente creado exitosamente';
                        $scope.cerrarModal();
                        $scope.cargarClientes();
                    })
                    .catch(function(error) {
                        $scope.error = obtenerMensajeError(error);
                        $scope.loading = false;
                    });
            }
        }

        // Eliminar cliente
        function eliminarCliente(cliente) {
            var mensaje = '¿Está seguro de eliminar el cliente "' + cliente.nombre + '"? Esta acción no se puede deshacer.';

            ModalService.confirm(mensaje, 'Confirmar Eliminación')
                .then(function(confirmado) {
                    if (!confirmado) {
                        return;
                    }

                    $scope.loading = true;
                    $scope.limpiarMensajes();

                    ClienteService.remove(cliente.clienteId)
                        .then(function() {
                            ModalService.success('Cliente "' + cliente.nombre + '" eliminado exitosamente');
                            $scope.mensaje = 'Cliente eliminado exitosamente';
                            $scope.cargarClientes();
                        })
                        .catch(function(error) {
                            $scope.error = obtenerMensajeError(error);
                            ModalService.error($scope.error);
                            $scope.loading = false;
                        });
                });
        }

        // Validar datos del cliente
        function validarCliente(cliente) {
            if (!cliente.nombre || cliente.nombre.trim() === '') {
                $scope.error = 'El nombre es obligatorio';
                return false;
            }

            if (!cliente.genero) {
                $scope.error = 'El género es obligatorio';
                return false;
            }

            if (!cliente.edad || cliente.edad < 18 || cliente.edad > 120) {
                $scope.error = 'La edad debe estar entre 18 y 120 años';
                return false;
            }

            if (!cliente.identificacion || cliente.identificacion.trim() === '') {
                $scope.error = 'La identificación es obligatoria';
                return false;
            }

            if (!cliente.direccion || cliente.direccion.trim() === '') {
                $scope.error = 'La dirección es obligatoria';
                return false;
            }

            if (!cliente.telefono || cliente.telefono.trim() === '') {
                $scope.error = 'El teléfono es obligatorio';
                return false;
            }

            if (!$scope.modoEdicion && (!cliente.contrasena || cliente.contrasena.trim() === '')) {
                $scope.error = 'La contraseña es obligatoria';
                return false;
            }

            return true;
        }

        // Obtener mensaje de error del backend
        function obtenerMensajeError(error) {
            // Verificar si hay errores de validación específicos
            if (error.data && error.data.errors) {
                var mensajes = [];

                // Iterar sobre cada campo con errores
                for (var campo in error.data.errors) {
                    if (error.data.errors.hasOwnProperty(campo)) {
                        var erroresCampo = error.data.errors[campo];

                        // Agregar cada mensaje de error del campo
                        for (var i = 0; i < erroresCampo.length; i++) {
                            mensajes.push('• ' + erroresCampo[i]);
                        }
                    }
                }

                // Retornar todos los mensajes unidos con saltos de línea
                if (mensajes.length > 0) {
                    return mensajes.join('\n');
                }
            }

            // Si no hay errores específicos, usar otros mensajes
            if (error.data && error.data.error) {
                return error.data.error;
            } else if (error.data && error.data.title) {
                return error.data.title;
            } else if (error.status === 404) {
                return 'Cliente no encontrado';
            } else if (error.status === 409) {
                return 'Ya existe un cliente con esa identificación';
            } else {
                return 'Error al procesar la solicitud. Verifique los datos e intente nuevamente.';
            }
        }

        // Limpiar mensajes
        function limpiarMensajes() {
            $scope.mensaje = null;
            $scope.error = null;
        }

        // Watch para búsqueda en tiempo real
        $scope.$watch('busqueda', function(newVal, oldVal) {
            if (newVal !== oldVal) {
                $scope.buscarClientes();
            }
        });
    }

})();