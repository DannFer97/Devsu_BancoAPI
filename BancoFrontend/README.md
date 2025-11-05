# BancoFrontend - Sistema Bancario

Frontend AngularJS 1.3.2 para el sistema bancario.

## 🚀 Ejecutar con Docker

### Opción 1: Desde el directorio raíz (Recomendado)
```bash
cd C:\Users\USUARIO\Documents\Devsu
docker-compose up -d bancofrontend
```

Esto levantará:
- SQL Server (puerto 1433)
- BancoAPI (puerto 5000)
- BancoFrontend (puerto 8080)

### Opción 2: Rebuild completo
```bash
cd C:\Users\USUARIO\Documents\Devsu
docker-compose up -d --build
```

## 🌐 Acceder a la Aplicación

- **Frontend**: http://localhost:8080
- **Backend API**: http://localhost:5000
- **Swagger**: http://localhost:5000

## 📁 Estructura del Proyecto

```
BancoFrontend/
├── index.html              # Página principal
├── app/
│   ├── app.js             # Configuración AngularJS
│   ├── controllers/       # Controladores (pendiente)
│   ├── services/          # Servicios (pendiente)
│   └── views/             # Vistas HTML (pendiente)
├── css/
│   ├── layout.css         # Estilos de layout
│   └── styles.css         # Estilos generales
├── Dockerfile             # Configuración Docker
└── nginx.conf             # Configuración Nginx
```

## 🔧 Configuración

### API URL
La URL de la API está configurada en `app/app.js`:

```javascript
app.constant('API_URL', '/api');
```

En Docker, Nginx hace proxy reverso automáticamente:
- Frontend: `http://localhost:8080`
- API calls: `http://localhost:8080/api/*` → `http://bancoapi:80/api/*`

### Para desarrollo local (sin Docker):
1. Cambiar API_URL a `'http://localhost:5000/api'`
2. Ejecutar: `npm run serve`
3. Abrir: http://localhost:8080

## 🐳 Comandos Docker Útiles

```bash
# Ver logs del frontend
docker logs -f banco-frontend

# Reiniciar solo el frontend
docker-compose restart bancofrontend

# Reconstruir el frontend
docker-compose up -d --build bancofrontend

# Detener todo
docker-compose down

# Detener y eliminar volúmenes
docker-compose down -v
```

## 📝 Tecnologías

- **AngularJS**: 1.3.2
- **Nginx**: 1.25-alpine
- **Docker**: Container runtime
