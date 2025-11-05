/*
   Servicio para CRUD de Clientes
    */

(function() {
    'use strict';

    angular.module('bancoApp')
        .factory('ClienteService', ClienteService);

    ClienteService.$inject = ['$http', 'API_URL'];

    function ClienteService($http, API_URL) {
        var service = {
            getAll: getAll,
            getById: getById,
            getByIdentificacion: getByIdentificacion,
            create: create,
            update: update,
            updateParcial: updateParcial,
            remove: remove
        };

        return service;

        // Obtener todos los clientes
        function getAll() {
            return $http.get(API_URL + '/clientes')
                .then(function(response) {
                    return response.data;
                })
                .catch(function(error) {
                    console.error('Error al obtener clientes:', error);
                    throw error;
                });
        }

        // Obtener cliente por ID
        function getById(id) {
            return $http.get(API_URL + '/clientes/' + id)
                .then(function(response) {
                    return response.data;
                })
                .catch(function(error) {
                    console.error('Error al obtener cliente:', error);
                    throw error;
                });
        }

        // Buscar cliente por identificación
        function getByIdentificacion(identificacion) {
            return $http.get(API_URL + '/clientes/identificacion/' + identificacion)
                .then(function(response) {
                    return response.data;
                })
                .catch(function(error) {
                    console.error('Error al buscar cliente:', error);
                    throw error;
                });
        }

        // Crear nuevo cliente
        function create(cliente) {
            return $http.post(API_URL + '/clientes', cliente)
                .then(function(response) {
                    return response.data;
                })
                .catch(function(error) {
                    console.error('Error al crear cliente:', error);
                    throw error;
                });
        }

        // Actualizar cliente completo (PUT)
        function update(id, cliente) {
            return $http.put(API_URL + '/clientes/' + id, cliente)
                .then(function(response) {
                    return response.data;
                })
                .catch(function(error) {
                    console.error('Error al actualizar cliente:', error);
                    throw error;
                });
        }

        // Actualizar cliente parcial (PATCH)
        function updateParcial(id, datos) {
            return $http.patch(API_URL + '/clientes/' + id, datos)
                .then(function(response) {
                    return response.data;
                })
                .catch(function(error) {
                    console.error('Error al actualizar cliente:', error);
                    throw error;
                });
        }

        // Eliminar cliente
        function remove(id) {
            return $http.delete(API_URL + '/clientes/' + id)
                .then(function(response) {
                    return response.data;
                })
                .catch(function(error) {
                    console.error('Error al eliminar cliente:', error);
                    throw error;
                });
        }
    }

})();