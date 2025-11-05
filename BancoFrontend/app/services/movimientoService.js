/* 
   Servicio para CRUD de Movimientos
 */

(function() {
    'use strict';

    angular.module('bancoApp')
        .factory('MovimientoService', MovimientoService);

    MovimientoService.$inject = ['$http', 'API_URL'];

    function MovimientoService($http, API_URL) {
        var service = {
            getAll: getAll,
            getById: getById,
            getByCuenta: getByCuenta,
            create: create,
            remove: remove
        };

        return service;

        // Obtener todos los movimientos
        function getAll() {
            return $http.get(API_URL + '/movimientos')
                .then(function(response) {
                    return response.data;
                })
                .catch(function(error) {
                    console.error('Error al obtener movimientos:', error);
                    throw error;
                });
        }

        // Obtener movimiento por ID
        function getById(id) {
            return $http.get(API_URL + '/movimientos/' + id)
                .then(function(response) {
                    return response.data;
                })
                .catch(function(error) {
                    console.error('Error al obtener movimiento:', error);
                    throw error;
                });
        }

        // Obtener movimientos de una cuenta
        function getByCuenta(cuentaId) {
            return $http.get(API_URL + '/movimientos/cuenta/' + cuentaId)
                .then(function(response) {
                    return response.data;
                })
                .catch(function(error) {
                    console.error('Error al obtener movimientos de la cuenta:', error);
                    throw error;
                });
        }

        // Crear nuevo movimiento
        // Aquí se aplican las validaciones del backend

        function create(movimiento) {
            return $http.post(API_URL + '/movimientos', movimiento)
                .then(function(response) {
                    return response.data;
                })
                .catch(function(error) {
                    console.error('Error al crear movimiento:', error);
                    // Extraer mensaje de error del backend
                    var mensaje = 'Error al crear movimiento';
                    if (error.data && error.data.error) {
                        mensaje = error.data.error;
                    } else if (error.data && error.data.title) {
                        mensaje = error.data.title;
                    }
                    error.mensaje = mensaje;
                    throw error;
                });
        }

        // Eliminar movimiento
        function remove(id) {
            return $http.delete(API_URL + '/movimientos/' + id)
                .then(function(response) {
                    return response.data;
                })
                .catch(function(error) {
                    console.error('Error al eliminar movimiento:', error);
                    throw error;
                });
        }
    }

})();