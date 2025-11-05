/* 
   Servicio para mostrar modales personalizados
   Reemplaza alert() y confirm() nativos del navegador
 */

(function() {
    'use strict';

    angular.module('bancoApp')
        .factory('ModalService', ModalService);

    ModalService.$inject = ['$q', '$timeout'];

    function ModalService($q, $timeout) {
        var service = {
            confirm: confirm,
            alert: alert,
            success: success,
            error: error
        };

        return service;

        /**
         * Modal de confirmación 
         * @param {string} mensaje - Mensaje a mostrar
         * @param {string} titulo - 
         * @returns {Promise} - Resuelve con true si acepta, false si cancela
         */
        function confirm(mensaje, titulo) {
            var deferred = $q.defer();
            titulo = titulo || '¿Está seguro?';

            // Crear overlay
            var overlay = crearOverlay();

            // Crear modal
            var modal = document.createElement('div');
            modal.className = 'custom-modal custom-modal-confirm';
            modal.innerHTML = `
                <div class="custom-modal-content">
                    <div class="custom-modal-header">
                        <h3 class="custom-modal-title"> ${titulo}</h3>
                    </div>
                    <div class="custom-modal-body">
                        <p>${mensaje}</p>
                    </div>
                    <div class="custom-modal-footer">
                        <button class="btn btn-secondary modal-btn-cancel">Cancelar</button>
                        <button class="btn btn-danger modal-btn-confirm">Confirmar</button>
                    </div>
                </div>
            `;

            document.body.appendChild(modal);

            // Animación de entrada
            $timeout(function() {
                overlay.classList.add('active');
                modal.classList.add('active');
            }, 10);

            // Event listeners
            var btnCancel = modal.querySelector('.modal-btn-cancel');
            var btnConfirm = modal.querySelector('.modal-btn-confirm');

            btnCancel.onclick = function() {
                cerrarModal(overlay, modal);
                deferred.resolve(false);
            };

            btnConfirm.onclick = function() {
                cerrarModal(overlay, modal);
                deferred.resolve(true);
            };

            // Cerrar con ESC
            var escHandler = function(e) {
                if (e.key === 'Escape') {
                    cerrarModal(overlay, modal);
                    deferred.resolve(false);
                    document.removeEventListener('keydown', escHandler);
                }
            };
            document.addEventListener('keydown', escHandler);

            // Cerrar al hacer clic fuera del modal
            overlay.onclick = function(e) {
                if (e.target === overlay) {
                    cerrarModal(overlay, modal);
                    deferred.resolve(false);
                }
            };

            return deferred.promise;
        }

        /**
         * Modal de alerta (reemplaza window.alert)
         * @param {string} mensaje - Mensaje a mostrar
         * @param {string} titulo - 
         * @returns {Promise}
         */
        function alert(mensaje, titulo) {
            return mostrarModal(mensaje, titulo || 'Información', 'info');
        }

        /**
         * Modal de éxito
         * @param {string} mensaje - Mensaje a mostrar
         * @param {string} titulo - 
         * @returns {Promise}
         */
        function success(mensaje, titulo) {
            return mostrarModal(mensaje, titulo || 'Éxito', 'success');
        }

        /**
         * Modal de error
         * @param {string} mensaje - Mensaje a mostrar
         * @param {string} titulo - 
         * @returns {Promise}
         */
        function error(mensaje, titulo) {
            return mostrarModal(mensaje, titulo || 'Error', 'error');
        }

        /**
         * Función genérica para mostrar modales informativos
         */
        function mostrarModal(mensaje, titulo, tipo) {
            var deferred = $q.defer();


            var clases = {
                'info': 'custom-modal-info',
                'success': 'custom-modal-success',
                'error': 'custom-modal-error'
            };

            var overlay = crearOverlay();

            var modal = document.createElement('div');
            modal.className = 'custom-modal ' + clases[tipo];
            modal.innerHTML = `
                <div class="custom-modal-content">
                    <div class="custom-modal-header">
                        <h3 class="custom-modal-title">${titulo}</h3>
                    </div>
                    <div class="custom-modal-body">
                        <p>${mensaje}</p>
                    </div>
                    <div class="custom-modal-footer">
                        <button class="btn btn-primary modal-btn-ok">Aceptar</button>
                    </div>
                </div>
            `;

            document.body.appendChild(modal);

            $timeout(function() {
                overlay.classList.add('active');
                modal.classList.add('active');
            }, 10);

            var btnOk = modal.querySelector('.modal-btn-ok');
            btnOk.onclick = function() {
                cerrarModal(overlay, modal);
                deferred.resolve(true);
            };

            // Cerrar con ESC o Enter
            var keyHandler = function(e) {
                if (e.key === 'Escape' || e.key === 'Enter') {
                    cerrarModal(overlay, modal);
                    deferred.resolve(true);
                    document.removeEventListener('keydown', keyHandler);
                }
            };
            document.addEventListener('keydown', keyHandler);

            return deferred.promise;
        }

        /**
         * Crear overlay de fondo
         */
        function crearOverlay() {
            var overlay = document.createElement('div');
            overlay.className = 'custom-modal-overlay';
            document.body.appendChild(overlay);
            return overlay;
        }

        /**
         * Cerrar y eliminar modal
         */
        function cerrarModal(overlay, modal) {
            overlay.classList.remove('active');
            modal.classList.remove('active');

            $timeout(function() {
                if (overlay && overlay.parentNode) {
                    overlay.parentNode.removeChild(overlay);
                }
                if (modal && modal.parentNode) {
                    modal.parentNode.removeChild(modal);
                }
            }, 300); // Esperar animación de salida
        }
    }

})();
