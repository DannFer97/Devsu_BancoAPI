/* 
   Servicio para Generación de Reportes
 */

(function() {
    'use strict';

    angular.module('bancoApp')
        .factory('ReporteService', ReporteService);

    ReporteService.$inject = ['$http', 'API_URL', '$window'];

    function ReporteService($http, API_URL, $window) {
        var service = {
            generarReporte: generarReporte,
            descargarPDF: descargarPDF
        };

        return service;

        // Generar reporte de estado de cuenta
        function generarReporte(clienteId, fechaInicio, fechaFin) {
            var params = {
                clienteId: clienteId,
                fechaInicio: fechaInicio,
                fechaFin: fechaFin
            };

            return $http.get(API_URL + '/reportes', { params: params })
                .then(function(response) {
                    return response.data;
                })
                .catch(function(error) {
                    console.error('Error al generar reporte:', error);
                    throw error;
                });
        }

        // Descargar reporte en PDF

        function descargarPDF(clienteId, fechaInicio, fechaFin) {
            // Construir URL con parámetros
            var url = API_URL + '/reportes/pdf?clienteId=' + clienteId + 
                      '&fechaInicio=' + fechaInicio + 
                      '&fechaFin=' + fechaFin;

            $window.open(url, '_blank');
        }


        // Función auxiliar para formatear fechas
        function formatearFecha(fecha) {
            var d = new Date(fecha);
            var dia = ('0' + d.getDate()).slice(-2);
            var mes = ('0' + (d.getMonth() + 1)).slice(-2);
            var anio = d.getFullYear();
            return dia + '/' + mes + '/' + anio;
        }
    }

})();