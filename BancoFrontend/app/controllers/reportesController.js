/* 
   Controlador para generación de Reportes
   CON DESCARGA PDF 
  */

(function() {
    'use strict';

    angular.module('bancoApp')
        .controller('ReportesController', ReportesController);

    ReportesController.$inject = ['$scope', 'ReporteService', 'ClienteService', '$window'];

    function ReportesController($scope, ReporteService, ClienteService, $window) {
        // Variables
        $scope.clientes = [];
        $scope.filtros = {
            clienteId: null,
            fechaInicio: '',
            fechaFin: ''
        };
        $scope.reporte = null;
        $scope.loading = false;
        $scope.mensaje = null;
        $scope.error = null;
        $scope.reporteGenerado = false;

        // Funciones públicas
        $scope.cargarClientes = cargarClientes;
        $scope.generarReporte = generarReporte;
        $scope.descargarPDF = descargarPDF;
        $scope.limpiarReporte = limpiarReporte;
        $scope.limpiarMensajes = limpiarMensajes;

        // Inicialización
        init();

       
        // WATCHERS - Detectar cambios en filtros
  

        // Si cambia el cliente, limpiar el reporte
        $scope.$watch('filtros.clienteId', function(newVal, oldVal) {
            if (newVal !== oldVal && oldVal !== null) {
                limpiarReporte();
            }
        });

        // Si cambia la fecha de inicio, limpiar el reporte
        $scope.$watch('filtros.fechaInicio', function(newVal, oldVal) {
            if (newVal !== oldVal && oldVal !== '') {
                limpiarReporte();
            }
        });

        // Si cambia la fecha de fin, limpiar el reporte
        $scope.$watch('filtros.fechaFin', function(newVal, oldVal) {
            if (newVal !== oldVal && oldVal !== '') {
                limpiarReporte();
            }
        });

  

        function init() {
            cargarClientes();
            establecerFechasPorDefecto();
        }

        // Establecer fechas por defecto (último mes)
        function establecerFechasPorDefecto() {
            var hoy = new Date();
            var hace30Dias = new Date();
            hace30Dias.setDate(hoy.getDate() - 30);

            $scope.filtros.fechaFin = hoy;
            $scope.filtros.fechaInicio = hace30Dias;
        }

        // Cargar lista de clientes
        function cargarClientes() {
            ClienteService.getAll()
                .then(function(data) {
                    $scope.clientes = data;
                })
                .catch(function(error) {
                    console.error('Error al cargar clientes:', error);
                    $scope.error = 'Error al cargar la lista de clientes';
                });
        }

        // Generar reporte 
        function generarReporte() {
            // Validaciones
            if (!validarFiltros()) {
                return;
            }

            $scope.loading = true;
            $scope.limpiarMensajes();
            $scope.reporte = null;
            $scope.reporteGenerado = false;

            var fechaInicioStr = convertirFechaAString($scope.filtros.fechaInicio);
            var fechaFinStr = convertirFechaAString($scope.filtros.fechaFin);

            ReporteService.generarReporte(
                $scope.filtros.clienteId,
                fechaInicioStr,
                fechaFinStr
            )
                .then(function(data) {
                    $scope.reporte = data;
                    $scope.reporteGenerado = true;
                    $scope.loading = false;
                    $scope.today = new Date(); 

                    if (!data.movimientos || data.movimientos.length === 0) {
                        $scope.mensaje = 'No se encontraron movimientos para el período seleccionado';
                    } else {
                        $scope.mensaje = 'Reporte generado exitosamente. Total: ' +
                                        data.movimientos.length + ' movimiento(s)';
                    }
                })
                .catch(function(error) {
                    $scope.error = obtenerMensajeError(error);
                    $scope.loading = false;
                });
        }

        // Descargar reporte en PDF 
        function descargarPDF() {
            if (!$scope.reporteGenerado || !$scope.reporte) {
                $scope.error = 'Debe generar el reporte antes de descargarlo';
                return;
            }

            var fechaInicioStr = convertirFechaAString($scope.filtros.fechaInicio);
            var fechaFinStr = convertirFechaAString($scope.filtros.fechaFin);

            try {
                ReporteService.descargarPDF(
                    $scope.filtros.clienteId,
                    fechaInicioStr,
                    fechaFinStr
                );
                $scope.mensaje = 'PDF descargado';
            } catch (error) {
            
                console.log('Endpoint PDF no disponible, generando del lado del cliente');
                imprimirReporte();
            }
        }


        // Limpiar reporte
        function limpiarReporte() {
            $scope.reporte = null;
            $scope.reporteGenerado = false;
            $scope.limpiarMensajes();
        }

        // Validar filtros
        function validarFiltros() {
            if (!$scope.filtros.clienteId) {
                $scope.error = 'Debe seleccionar un cliente';
                return false;
            }

            if (!$scope.filtros.fechaInicio) {
                $scope.error = 'Debe seleccionar una fecha de inicio';
                return false;
            }

            if (!$scope.filtros.fechaFin) {
                $scope.error = 'Debe seleccionar una fecha de fin';
                return false;
            }

            // Validar que fecha inicio sea menor que fecha fin
            var inicio = new Date($scope.filtros.fechaInicio);
            var fin = new Date($scope.filtros.fechaFin);

            if (inicio > fin) {
                $scope.error = 'La fecha de inicio debe ser menor a la fecha de fin';
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
                return 'No se encontraron datos para el cliente seleccionado';
            } else {
                return 'Error al generar el reporte. Intente nuevamente.';
            }
        }

        // Formatear fecha para display (dd/mm/yyyy)
        function formatearFechaDisplay(fecha) {
            if (!fecha) return 'N/A';
            var d = new Date(fecha);
            var dia = ('0' + d.getDate()).slice(-2);
            var mes = ('0' + (d.getMonth() + 1)).slice(-2);
            var anio = d.getFullYear();
            return dia + '/' + mes + '/' + anio;
        }

        // Formatear número con 2 decimales
        function formatearNumero(numero) {
            if (numero === null || numero === undefined) return '0.00';
            return parseFloat(numero).toFixed(2).replace(/\B(?=(\d{3})+(?!\d))/g, ',');
        }

        // Convertir Date object a string YYYY-MM-DD para enviar al backend
        function convertirFechaAString(fecha) {
            if (!fecha) return '';
            var d = fecha instanceof Date ? fecha : new Date(fecha);
            var year = d.getFullYear();
            var month = ('0' + (d.getMonth() + 1)).slice(-2);
            var day = ('0' + d.getDate()).slice(-2);
            return year + '-' + month + '-' + day;
        }

        // Limpiar mensajes
        function limpiarMensajes() {
            $scope.mensaje = null;
            $scope.error = null;
        }
    }

})();