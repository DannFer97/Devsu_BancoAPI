# ========================================
# Dockerfile para Frontend AngularJS
# Usa Nginx para servir archivos estáticos
# ========================================

FROM nginx:1.25-alpine

# Copiar archivos del frontend al directorio de nginx
COPY . /usr/share/nginx/html

# Copiar configuración personalizada de nginx
COPY nginx.conf /etc/nginx/conf.d/default.conf

# Exponer puerto 80
EXPOSE 80

# Comando por defecto (nginx se inicia automáticamente)
CMD ["nginx", "-g", "daemon off;"]
