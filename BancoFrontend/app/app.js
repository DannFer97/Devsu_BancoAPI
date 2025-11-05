/* 
    Configuracion Principal 
    */

(function() {
    'use strict';

    // Definir módulo principal
    var app = angular.module('bancoApp', ['ngRoute']);

    // Configuración de rutas
    app.config(['$routeProvider', '$locationProvider', function($routeProvider, $locationProvider) {
        
        $routeProvider
            // Ruta de Clientes
            .when('/clientes', {
                templateUrl: 'app/views/clientes.html',
                controller: 'ClientesController'
            })
            
            // Ruta de Cuentas
            .when('/cuentas', {
                templateUrl: 'app/views/cuentas.html',
                controller: 'CuentasController'
            })
            
            // Ruta de Movimientos
            .when('/movimientos', {
                templateUrl: 'app/views/movimientos.html',
                controller: 'MovimientosController'
            })
            
            // Ruta de Reportes
            .when('/reportes', {
                templateUrl: 'app/views/reportes.html',
                controller: 'ReportesController'
            })
            
            // Ruta por defecto
            .otherwise({
                redirectTo: '/clientes'
            });
    }]);

    // Constantes de configuración
    // En Docker, Nginx hace proxy a /api -> bancoapi:80/api
    // En local, puedes cambiar a 'http://localhost:5000/api'
    app.constant('API_URL', '/api');

    // Controller principal para el menú
    app.controller('MainController', ['$scope', '$location', 
        function($scope, $location) {
            // Detectar página actual para menú activo
            $scope.$on('$routeChangeSuccess', function() {
                var path = $location.path();
                $scope.currentPage = path.split('/')[1];
            });
        }
    ]);

    // Run: Configuración inicial
    app.run(['$rootScope', function($rootScope) {
        console.log('BancoApp AngularJS 1.3.2 iniciado correctamente');
    }]);

})();
