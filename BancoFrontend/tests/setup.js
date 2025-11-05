/**
 * Setup para Jest + AngularJS 1.3.2
 * Configuración simple siguiendo mejores prácticas
 */

// 1. Cargar Angular y angular-route
require('angular');
require('angular-route');

// 2. Simular entorno Jasmine para que angular-mocks se registre
if (!global.jasmine) {
    global.jasmine = {
        getEnv: function() {
            return {
                beforeEach: beforeEach,
                afterEach: afterEach
            };
        }
    };
}

// 3. Cargar angular-mocks (ahora detectará "Jasmine")
require('angular-mocks');

// 4. Exponer angular globalmente
global.angular = window.angular;

// 5. Exponer module e inject para que los tests puedan usarlos
global.ngModule = window.module;
global.ngInject = window.inject;

// 6. Cargar la aplicación y servicios
require('../app/app.js');
require('../app/services/movimientoService.js');
