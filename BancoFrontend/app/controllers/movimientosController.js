/* 
   Controlador para gestión de Movimientos
= */

(function() {
    'use strict';

    angular.module('bancoApp')
        .controller('MovimientosController', MovimientosController);

    MovimientosController.$inject = ['$scope', 'MovimientoService', 'CuentaService', 'ClienteService', 'ModalService'];

    function MovimientosController($scope, MovimientoService, CuentaService, ClienteService, ModalService) {
        // Variables
        $scope.movimientos = [];
        $scope.movimientosFiltrados = [];
        $scope.cuentas = [];
        $scope.clientes = [];
        $scope.cuentaSeleccionada = null;
        $scope.nuevoMovimiento = {};
        $scope.busqueda = '';
        $scope.loading = false;
        $scope.mensaje = null;
        $scope.error = null;
        $scope.modalActivo = false;
        $scope.infoCuenta = null;
        $scope.saldoCalculado = 0;

        // Funciones públicas
        $scope.cargarMovimientos = cargarMovimientos;
        $scope.cargarCuentas = cargarCuentas;
        $scope.cargarClientes = cargarClientes;
        $scope.buscarMovimientos = buscarMovimientos;
        $scope.abrirModalNuevo = abrirModalNuevo;
        $scope.cerrarModal = cerrarModal;
        $scope.guardarMovimiento = guardarMovimiento;
        $scope.eliminarMovimiento = eliminarMovimiento;
        $scope.limpiarMensajes = limpiarMensajes;
        $scope.obtenerNombreCliente = obtenerNombreCliente;
        $scope.obtenerNumeroCuenta = obtenerNumeroCuenta;
        $scope.onCuentaChange = onCuentaChange;
        $scope.onValorChange = onValorChange;
        $scope.esCredito = esCredito;

        // Inicialización
        init();

      

        function init() {
            cargarClientes();
            cargarCuentas();
            cargarMovimientos();
        }

        // Cargar clientes
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
            CuentaService.getAll()
                .then(function(data) {
                    $scope.cuentas = data;
                })
                .catch(function(error) {
                    console.error('Error al cargar cuentas:', error);
                });
        }

        // Cargar lista de movimientos
        function cargarMovimientos() {
            $scope.loading = true;
            $scope.limpiarMensajes();

            MovimientoService.getAll()
                .then(function(data) {
                    $scope.movimientos = data;
                    $scope.movimientosFiltrados = data;
                    $scope.loading = false;
                })
                .catch(function(error) {
                    $scope.error = 'Error al cargar los movimientos. Verifique que el servidor esté funcionando.';
                    $scope.loading = false;
                    console.error('Error:', error);
                });
        }

        // Búsqueda rápida 
        function buscarMovimientos() {
            if (!$scope.busqueda || $scope.busqueda.trim() === '') {
                $scope.movimientosFiltrados = $scope.movimientos;
                return;
            }

            var termino = $scope.busqueda.toLowerCase();
            
            $scope.movimientosFiltrados = $scope.movimientos.filter(function(mov) {
                var numeroCuenta = obtenerNumeroCuenta(mov.cuentaId).toLowerCase();
                var nombreCliente = obtenerNombreCliente(mov.cuentaId).toLowerCase();
                return (mov.tipoMovimiento && mov.tipoMovimiento.toLowerCase().indexOf(termino) > -1) ||
                       (numeroCuenta.indexOf(termino) > -1) ||
                       (nombreCliente.indexOf(termino) > -1);
            });
        }

        // Obtener nombre del cliente por cuenta ID
        function obtenerNombreCliente(cuentaId) {
            var cuenta = $scope.cuentas.find(function(c) {
                return c.cuentaId === cuentaId;
            });
            if (!cuenta) return 'Desconocido';

            var cliente = $scope.clientes.find(function(cl) {
                return cl.clienteId === cuenta.clienteId;
            });
            return cliente ? cliente.nombre : 'Desconocido';
        }

        // Obtener número de cuenta por ID
        function obtenerNumeroCuenta(cuentaId) {
            var cuenta = $scope.cuentas.find(function(c) {
                return c.cuentaId === cuentaId;
            });
            return cuenta ? cuenta.numeroCuenta : 'N/A';
        }

        // Cuando cambia la cuenta seleccionada
        function onCuentaChange() {
            if (!$scope.nuevoMovimiento.cuentaId) {
                $scope.infoCuenta = null;
                return;
            }

            var cuenta = $scope.cuentas.find(function(c) {
                return c.cuentaId == $scope.nuevoMovimiento.cuentaId;
            });

            if (cuenta) {
                // Obtener el saldo actual de la cuenta
                CuentaService.getById(cuenta.cuentaId)
                    .then(function(data) {
                        $scope.infoCuenta = {
                            numeroCuenta: data.numeroCuenta,
                            tipoCuenta: data.tipoCuenta,
                            saldoActual: data.saldoActual, // Saldo actual del último movimiento
                            nombreCliente: obtenerNombreCliente(data.cuentaId)
                        };
                        $scope.onValorChange(); // Recalcular
                    })
                    .catch(function(error) {
                        console.error('Error al obtener cuenta:', error);
                    });
            }
        }

        // Cuando cambia el valor (para calcular saldo resultante)
        function onValorChange() {
            if (!$scope.infoCuenta || !$scope.nuevoMovimiento.valor || !$scope.nuevoMovimiento.tipoMovimiento) {
                $scope.saldoCalculado = $scope.infoCuenta ? $scope.infoCuenta.saldoActual : 0;
                return;
            }

            var valor = parseFloat($scope.nuevoMovimiento.valor) || 0;

            // Determinar si es retiro o depósito
            var esRetiro = $scope.nuevoMovimiento.tipoMovimiento.toLowerCase().indexOf('retiro') > -1 ||
                           $scope.nuevoMovimiento.tipoMovimiento.toLowerCase().indexOf('debito') > -1 ||
                           $scope.nuevoMovimiento.tipoMovimiento.toLowerCase().indexOf('débito') > -1;

            // Calcular saldo: si es retiro, restar; si es depósito, sumar
            if (esRetiro) {
                $scope.saldoCalculado = $scope.infoCuenta.saldoActual - Math.abs(valor);
            } else {
                $scope.saldoCalculado = $scope.infoCuenta.saldoActual + Math.abs(valor);
            }
        }

        // Verificar si es crédito (valor positivo)
        function esCredito(valor) {
            return valor && parseFloat(valor) > 0;
        }

        // Abrir modal para nuevo movimiento
        function abrirModalNuevo() {
            $scope.nuevoMovimiento = {
                cuentaId: null,
                tipoMovimiento: '',
                valor: 0
            };
            $scope.infoCuenta = null;
            $scope.saldoCalculado = 0;
            $scope.modalActivo = true;
            $scope.limpiarMensajes();
        }

        // Cerrar modal
        function cerrarModal() {
            $scope.modalActivo = false;
            $scope.nuevoMovimiento = {};
            $scope.infoCuenta = null;
            $scope.saldoCalculado = 0;
            $scope.limpiarMensajes();
        }

        // Guardar movimiento
        function guardarMovimiento() {
            // Validaciones básicas
            if (!validarMovimiento($scope.nuevoMovimiento)) {
                return;
            }

            $scope.loading = true;
            $scope.limpiarMensajes();

            // Preparar el movimiento: convertir retiros a negativos automáticamente
            var movimientoParaEnviar = angular.copy($scope.nuevoMovimiento);
            var esRetiro = movimientoParaEnviar.tipoMovimiento.toLowerCase().indexOf('retiro') > -1 ||
                           movimientoParaEnviar.tipoMovimiento.toLowerCase().indexOf('debito') > -1 ||
                           movimientoParaEnviar.tipoMovimiento.toLowerCase().indexOf('débito') > -1;

            // Si es retiro y el valor es positivo, convertirlo a negativo
            if (esRetiro && movimientoParaEnviar.valor > 0) {
                movimientoParaEnviar.valor = movimientoParaEnviar.valor * -1;
            }

            // Crear movimiento
            // AQUÍ SE APLICAN LAS VALIDACIONES DEL BACKEND:
            // - "Saldo no disponible"
            // - "Cupo diario Excedido"
            MovimientoService.create(movimientoParaEnviar)
                .then(function(response) {
                    $scope.mensaje = 'Movimiento registrado exitosamente';
                    $scope.cerrarModal();
                    $scope.cargarMovimientos();
                    $scope.loading = false;
                })
                .catch(function(error) {
                    // El servicio ya extrae el mensaje de error
                    $scope.error = error.mensaje || obtenerMensajeError(error);
                    $scope.loading = false;
                    
                    // Mostrar mensaje específico según el error
                    if ($scope.error.indexOf('Saldo no disponible') > -1) {
                        $scope.error =  $scope.error + ' - La cuenta no tiene fondos suficientes para este retiro.';
                    } else if ($scope.error.indexOf('Cupo diario Excedido') > -1) {
                        $scope.error = $scope.error + ' - Ha alcanzado el límite de retiros de $1,000 por día.';
                    }
                });
        }

        // Eliminar movimiento
        function eliminarMovimiento(movimiento) {
            var mensaje = '¿Está seguro de eliminar este movimiento de $' + movimiento.valor + '? Esta acción no se puede deshacer.';

            ModalService.confirm(mensaje, 'Confirmar Eliminación')
                .then(function(confirmado) {
                    if (!confirmado) {
                        return;
                    }

                    $scope.loading = true;
                    $scope.limpiarMensajes();

                    MovimientoService.remove(movimiento.movimientoId)
                        .then(function() {
                            ModalService.success('Movimiento eliminado exitosamente');
                            $scope.mensaje = 'Movimiento eliminado exitosamente';
                            $scope.cargarMovimientos();
                        })
                        .catch(function(error) {
                            $scope.error = obtenerMensajeError(error);
                            ModalService.error($scope.error);
                            $scope.loading = false;
                        });
                });
        }

        // Validar datos del movimiento
        function validarMovimiento(movimiento) {
            if (!movimiento.cuentaId) {
                $scope.error = 'Debe seleccionar una cuenta';
                return false;
            }

            if (!movimiento.tipoMovimiento || movimiento.tipoMovimiento.trim() === '') {
                $scope.error = 'El tipo de movimiento es obligatorio';
                return false;
            }

            if (movimiento.valor === null || movimiento.valor === undefined || movimiento.valor === 0) {
                $scope.error = 'El valor del movimiento es obligatorio y debe ser mayor a cero';
                return false;
            }

            // Validación: el valor debe ser positivo (el sistema lo convertirá automáticamente)
            var valor = parseFloat(movimiento.valor);
            if (valor < 0) {
                $scope.error = 'Por favor ingrese un valor positivo. El sistema lo convertirá automáticamente según el tipo de movimiento.';
                return false;
            }

            return true;
        }

        // Obtener mensaje de error del backend
        function obtenerMensajeError(error) {
            if (error.data && error.data.error) {
                return error.data.error;
            } else if (error.data && error.data.title) {
                return error.data.title;
            } else if (error.status === 404) {
                return 'Cuenta no encontrada';
            } else if (error.status === 400) {
                return 'Error de validación. Verifique los datos.';
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
                $scope.buscarMovimientos();
            }
        });

        // Watch para recalcular saldo cuando cambie el tipo de movimiento
        $scope.$watch('nuevoMovimiento.tipoMovimiento', function(newVal, oldVal) {
            if (newVal !== oldVal) {
                $scope.onValorChange();
            }
        });
    }

})();