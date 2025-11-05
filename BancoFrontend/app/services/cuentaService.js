/* 
   Servicio para CRUD de Cuentas
    */

(function() {
    'use strict';

    angular.module('bancoApp')
        .factory('CuentaService', CuentaService);

    CuentaService.$inject = ['$http', 'API_URL'];

    function CuentaService($http, API_URL) {
        var service = {
            getAll: getAll,
            getById: getById,
            getByNumero: getByNumero,
            getByCliente: getByCliente,
            create: create,
            update: update,
            remove: remove
        };

        return service;

        // Obtener todas las cuentas
        function getAll() {
            return $http.get(API_URL + '/cuentas')
                .then(function(response) {
                    return response.data;
                })
                .catch(function(error) {
                    console.error('Error al obtener cuentas:', error);
                    throw error;
                });
        }

        // Obtener cuenta por ID
        function getById(id) {
            return $http.get(API_URL + '/cuentas/' + id)
                .then(function(response) {
                    return response.data;
                })
                .catch(function(error) {
                    console.error('Error al obtener cuenta:', error);
                    throw error;
                });
        }

        // Buscar cuenta por número
        function getByNumero(numero) {
            return $http.get(API_URL + '/cuentas/numero/' + numero)
                .then(function(response) {
                    return response.data;
                })
                .catch(function(error) {
                    console.error('Error al buscar cuenta:', error);
                    throw error;
                });
        }

        // Obtener cuentas de un cliente
        function getByCliente(clienteId) {
            return $http.get(API_URL + '/cuentas/cliente/' + clienteId)
                .then(function(response) {
                    return response.data;
                })
                .catch(function(error) {
                    console.error('Error al obtener cuentas del cliente:', error);
                    throw error;
                });
        }

        // Crear nueva cuenta
        function create(cuenta) {
            return $http.post(API_URL + '/cuentas', cuenta)
                .then(function(response) {
                    return response.data;
                })
                .catch(function(error) {
                    console.error('Error al crear cuenta:', error);
                    throw error;
                });
        }

        // Actualizar cuenta (PUT)
        function update(id, cuenta) {
            return $http.put(API_URL + '/cuentas/' + id, cuenta)
                .then(function(response) {
                    return response.data;
                })
                .catch(function(error) {
                    console.error('Error al actualizar cuenta:', error);
                    throw error;
                });
        }

        // Eliminar cuenta
        function remove(id) {
            return $http.delete(API_URL + '/cuentas/' + id)
                .then(function(response) {
                    return response.data;
                })
                .catch(function(error) {
                    console.error('Error al eliminar cuenta:', error);
                    throw error;
                });
        }
    }

})();