/* 
   Controlador para gestión de Cuentas
*/

(function() {
    'use strict';

    angular.module('bancoApp')
        .controller('CuentasController', CuentasController);

    CuentasController.$inject = ['$scope', 'CuentaService', 'ClienteService', 'ModalService'];

    function CuentasController($scope, CuentaService, ClienteService, ModalService) {
        // Variables
        $scope.cuentas = [];
        $scope.cuentasFiltradas = [];
        $scope.clientes = [];
        $scope.cuentaSeleccionada = null;
        $scope.nuevaCuenta = {};
        $scope.modoEdicion = false;
        $scope.busqueda = '';
        $scope.loading = false;
        $scope.mensaje = null;
        $scope.error = null;
        $scope.modalActivo = false;

        // Funciones públicas
        $scope.cargarCuentas = cargarCuentas;
        $scope.cargarClientes = cargarClientes;
        $scope.buscarCuentas = buscarCuentas;
        $scope.abrirModalNuevo = abrirModalNuevo;
        $scope.abrirModalEditar = abrirModalEditar;
        $scope.cerrarModal = cerrarModal;
        $scope.guardarCuenta = guardarCuenta;
        $scope.eliminarCuenta = eliminarCuenta;
        $scope.limpiarMensajes = limpiarMensajes;
        $scope.obtenerNombreCliente = obtenerNombreCliente;

        // Inicialización
        init();

       
        function init() {
            cargarClientes();
            cargarCuentas();
        }

        // Cargar lista de clientes para el dropdown
        function cargarClientes() {
            ClienteService.getAll()
                .then(function(data) {
                    $scope.clientes = data;
                })
                .catch(function(error) {
                    console.error('Error al cargar clientes:', error);
                });
        }

        // Cargar lista de cuentas
        function cargarCuentas() {
            $scope.loading = true;
            $scope.limpiarMensajes();

            CuentaService.getAll()
                .then(function(data) {
                    $scope.cuentas = data;
                    $scope.cuentasFiltradas = data;
                    $scope.loading = false;
                })
                .catch(function(error) {
                    $scope.error = 'Error al cargar las cuentas. Verifique que el servidor esté funcionando.';
                    $scope.loading = false;
                    console.error('Error:', error);
                });
        }

        // Búsqueda rápida 
        function buscarCuentas() {
            if (!$scope.busqueda || $scope.busqueda.trim() === '') {
                $scope.cuentasFiltradas = $scope.cuentas;
                return;
            }

            var termino = $scope.busqueda.toLowerCase();
            
            $scope.cuentasFiltradas = $scope.cuentas.filter(function(cuenta) {
                var nombreCliente = obtenerNombreCliente(cuenta.clienteId).toLowerCase();
                return (cuenta.numeroCuenta && cuenta.numeroCuenta.indexOf(termino) > -1) ||
                       (cuenta.tipoCuenta && cuenta.tipoCuenta.toLowerCase().indexOf(termino) > -1) ||
                       (nombreCliente.indexOf(termino) > -1);
            });
        }

        // Obtener nombre del cliente por ID
        function obtenerNombreCliente(clienteId) {
            var cliente = $scope.clientes.find(function(c) {
                return c.clienteId === clienteId;
            });
            return cliente ? cliente.nombre : 'Desconocido';
        }

        // Abrir modal para nueva cuenta
        function abrirModalNuevo() {
            $scope.modoEdicion = false;
            $scope.nuevaCuenta = {
                estado: true,
                tipoCuenta: 'Ahorros',
                saldoInicial: 0,
                clienteId: null
            };
            $scope.modalActivo = true;
            $scope.limpiarMensajes();
        }

        // Abrir modal para editar cuenta
        function abrirModalEditar(cuenta) {
            $scope.modoEdicion = true;
            $scope.cuentaSeleccionada = cuenta;
            
            // Copiar datos para edición
            $scope.nuevaCuenta = {
                numeroCuenta: cuenta.numeroCuenta,
                tipoCuenta: cuenta.tipoCuenta,
                saldoInicial: cuenta.saldoInicial,
                estado: cuenta.estado,
                clienteId: cuenta.clienteId
            };
            
            $scope.modalActivo = true;
            $scope.limpiarMensajes();
        }

        // Cerrar modal
        function cerrarModal() {
            $scope.modalActivo = false;
            $scope.nuevaCuenta = {};
            $scope.cuentaSeleccionada = null;
            $scope.limpiarMensajes();
        }

        // Guardar cuenta (crear o actualizar)
        function guardarCuenta() {
            // Validaciones básicas
            if (!validarCuenta($scope.nuevaCuenta)) {
                return;
            }

            $scope.loading = true;
            $scope.limpiarMensajes();

            if ($scope.modoEdicion) {
                // Actualizar cuenta existente
                CuentaService.update($scope.cuentaSeleccionada.cuentaId, $scope.nuevaCuenta)
                    .then(function(response) {
                        $scope.mensaje = 'Cuenta actualizada exitosamente';
                        $scope.cerrarModal();
                        $scope.cargarCuentas();
                    })
                    .catch(function(error) {
                        $scope.error = obtenerMensajeError(error);
                        $scope.loading = false;
                    });
            } else {
                // Crear nueva cuenta
                CuentaService.create($scope.nuevaCuenta)
                    .then(function(response) {
                        $scope.mensaje = 'Cuenta creada exitosamente';
                        $scope.cerrarModal();
                        $scope.cargarCuentas();
                    })
                    .catch(function(error) {
                        $scope.error = obtenerMensajeError(error);
                        $scope.loading = false;
                    });
            }
        }

        // Eliminar cuenta
        function eliminarCuenta(cuenta) {
            var mensaje = '¿Está seguro de eliminar la cuenta "' + cuenta.numeroCuenta + '"? Esta acción no se puede deshacer.';

            ModalService.confirm(mensaje, 'Confirmar Eliminación')
                .then(function(confirmado) {
                    if (!confirmado) {
                        return;
                    }

                    $scope.loading = true;
                    $scope.limpiarMensajes();

                    CuentaService.remove(cuenta.cuentaId)
                        .then(function() {
                            ModalService.success('Cuenta "' + cuenta.numeroCuenta + '" eliminada exitosamente');
                            $scope.mensaje = 'Cuenta eliminada exitosamente';
                            $scope.cargarCuentas();
                        })
                        .catch(function(error) {
                            $scope.error = obtenerMensajeError(error);
                            ModalService.error($scope.error);
                            $scope.loading = false;
                        });
                });
        }

        // Validar datos de la cuenta
        function validarCuenta(cuenta) {
            if (!cuenta.numeroCuenta || cuenta.numeroCuenta.trim() === '') {
                $scope.error = 'El número de cuenta es obligatorio';
                return false;
            }

            if (!cuenta.tipoCuenta || cuenta.tipoCuenta.trim() === '') {
                $scope.error = 'El tipo de cuenta es obligatorio';
                return false;
            }

            if (cuenta.saldoInicial === null || cuenta.saldoInicial === undefined) {
                $scope.error = 'El saldo inicial es obligatorio';
                return false;
            }

            if (cuenta.saldoInicial < 0) {
                $scope.error = 'El saldo inicial no puede ser negativo';
                return false;
            }

            if (!cuenta.clienteId) {
                $scope.error = 'Debe seleccionar un cliente';
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
                return 'Cuenta no encontrada';
            } else if (error.status === 409) {
                return 'Ya existe una cuenta con ese número';
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
                $scope.buscarCuentas();
            }
        });
    }

})();