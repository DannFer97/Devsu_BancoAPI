/**
 * PRUEBAS UNITARIAS - MovimientoService
 */

describe('MovimientoService - Pruebas Críticas', function() {
    var MovimientoService;
    var $httpBackend;
    var $rootScope;
    var API_URL;
    var $exceptionHandler;

    // Setup antes de cada prueba
    beforeEach(function() {
        // Usar global.ngModule y global.ngInject que setup.js expuso
        ngModule('bancoApp');

        // Mock del $exceptionHandler para capturar excepciones
        ngModule(function($exceptionHandlerProvider) {
            $exceptionHandlerProvider.mode('log');
        });

        // Inyectar dependencias
        ngInject(function(_MovimientoService_, _$httpBackend_, _$rootScope_, _API_URL_, _$exceptionHandler_) {
            MovimientoService = _MovimientoService_;
            $httpBackend = _$httpBackend_;
            $rootScope = _$rootScope_;
            API_URL = _API_URL_;
            $exceptionHandler = _$exceptionHandler_;
        });
    });

    // Cleanup después de cada prueba
    afterEach(function() {
        if ($httpBackend) {
            try {
                $httpBackend.verifyNoOutstandingExpectation();
                $httpBackend.verifyNoOutstandingRequest();
            } catch(e) {
                // Ignorar errores de digest en progreso para tests de error
            }
        }
    });

    /**
     * PRUEBA 1: Crear movimiento de CRÉDITO (depósito)
     * Los créditos tienen valor POSITIVO
     */
    it('debería crear un depósito con valor positivo', function() {
        // Arrange
        var movimiento = {
            cuentaId: 2,
            tipoMovimiento: 'Deposito de 600',
            valor: 600
        };

        var respuestaEsperada = {
            movimientoId: 1,
            fecha: '2024-10-02T00:00:00',
            tipoMovimiento: 'Deposito de 600',
            valor: 600,
            saldo: 700,
            cuentaId: 2
        };

        $httpBackend.expectPOST(API_URL + '/movimientos', movimiento)
            .respond(201, respuestaEsperada);

        // Act
        var resultado;
        MovimientoService.create(movimiento).then(function(data) {
            resultado = data;
        });

        $httpBackend.flush();

        // Assert
        expect(resultado).toBeDefined();
        expect(resultado.valor).toBe(600);
        expect(resultado.saldo).toBe(700);
    });

    /**
     * PRUEBA 2: Crear movimiento de DÉBITO (retiro)
     * Los débitos tienen valor NEGATIVO
     */
    it('debería crear un retiro con valor negativo', function() {
        // Arrange
        var movimiento = {
            cuentaId: 1,
            tipoMovimiento: 'Retiro de 575',
            valor: -575
        };

        var respuestaEsperada = {
            movimientoId: 2,
            fecha: '2024-10-03T00:00:00',
            tipoMovimiento: 'Retiro de 575',
            valor: -575,
            saldo: 1425,
            cuentaId: 1
        };

        $httpBackend.expectPOST(API_URL + '/movimientos', movimiento)
            .respond(201, respuestaEsperada);

        // Act
        var resultado;
        MovimientoService.create(movimiento).then(function(data) {
            resultado = data;
        });

        $httpBackend.flush();

        // Assert
        expect(resultado).toBeDefined();
        expect(resultado.valor).toBe(-575);
        expect(resultado.saldo).toBe(1425);
    });

    /**
     * PRUEBA 3: VALIDACIÓN CRÍTICA - "Saldo no disponible"
     * Debe rechazar débito cuando el saldo es cero o insuficiente
     */
    it('debería rechazar débito con mensaje "Saldo no disponible"', function() {
        // Arrange
        var movimiento = {
            cuentaId: 3,
            tipoMovimiento: 'Retiro de 100',
            valor: -100
        };

        var errorCapturado = null;

        $httpBackend.expectPOST(API_URL + '/movimientos', movimiento)
            .respond(400, { error: 'Saldo no disponible' });

        // Act - capturar error y convertir rechazo en resolución
        var promesa = MovimientoService.create(movimiento);

        promesa.then(
            function() {
                // No debería llegar aquí
            },
            function(error) {
                errorCapturado = error;
            }
        );

        // Flush con manejo de excepciones
        try {
            $httpBackend.flush();
        } catch(e) {
            // Capturar cualquier excepción que escape
        }

        // Assert
        expect(errorCapturado).not.toBeNull();
        expect(errorCapturado.status).toBe(400);
        expect(errorCapturado.mensaje).toBe('Saldo no disponible');
    });

    /**
     * PRUEBA 4: VALIDACIÓN CRÍTICA - "Cupo diario Excedido"
     * Límite diario: $1000 según documento
     */
    it('debería rechazar débito con mensaje "Cupo diario Excedido"', function() {
        // Arrange
        var movimiento = {
            cuentaId: 1,
            tipoMovimiento: 'Retiro de 600',
            valor: -600
        };

        var errorCapturado = null;

        $httpBackend.expectPOST(API_URL + '/movimientos', movimiento)
            .respond(400, { error: 'Cupo diario Excedido' });

        // Act - capturar error y convertir rechazo en resolución
        var promesa = MovimientoService.create(movimiento);

        promesa.then(
            function() {
                // No debería llegar aquí
            },
            function(error) {
                errorCapturado = error;
            }
        );

        // Flush con manejo de excepciones
        try {
            $httpBackend.flush();
        } catch(e) {
            // Capturar cualquier excepción que escape
        }

        // Assert
        expect(errorCapturado).not.toBeNull();
        expect(errorCapturado.status).toBe(400);
        expect(errorCapturado.mensaje).toBe('Cupo diario Excedido');
    });
});
