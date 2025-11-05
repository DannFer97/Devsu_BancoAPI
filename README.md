# Sistema Bancario - Gestión de Cuentas y Movimientos

Sistema web completo para la gestión de clientes bancarios, cuentas y movimientos transaccionales. Desarrollado con arquitectura de capas en .NET Core 6 y frontend en AngularJS.

## Tabla de Contenidos

- [Descripción](#descripción)
- [Arquitectura](#arquitectura)
- [Tecnologías Utilizadas](#tecnologías-utilizadas)
- [Requisitos Previos](#requisitos-previos)
- [Instalación y Despliegue](#instalación-y-despliegue)
- [Estructura del Proyecto](#estructura-del-proyecto)
- [API Endpoints](#api-endpoints)
- [Reglas de Negocio](#reglas-de-negocio)
- [Pruebas](#pruebas)

## Descripción

Sistema bancario que permite gestionar clientes, cuentas bancarias y movimientos transaccionales con las siguientes funcionalidades:

- CRUD completo de Clientes, Cuentas y Movimientos
- Gestión de transacciones bancarias (débitos y créditos)
- Validación de saldo disponible
- Límite diario de retiros ($1000)
- Generación de reportes de estado de cuenta por rango de fechas
- Exportación de reportes en formato PDF

## Arquitectura

El proyecto implementa una arquitectura de Clean Architecture con las siguientes capas:

### Backend (BancoBackend)

- **BancoAPI.API**: Capa de presentación con Controllers REST
- **BancoAPI.Application**: Lógica de negocio, servicios y DTOs
- **BancoAPI.Domain**: Entidades de dominio e interfaces
- **BancoAPI.Infrastructure**: Acceso a datos, repositorios y DbContext
- **BancoAPI.Tests**: Pruebas unitarias

### Frontend (BancoFrontend)

- Aplicación web desarrollada en AngularJS 1.x
- Arquitectura MVC
- Sin frameworks de estilos (CSS personalizado)

### Patrones Implementados

- Repository Pattern
- Unit of Work Pattern
- Dependency Injection
- DTO Pattern
- Exception Handling centralizado
- Programación funcional con LINQ y expresiones lambda

## Tecnologías Utilizadas

### Backend

- .NET Core 6.0
- Entity Framework Core 6.0.25
- SQL Server 2022 Express
- AutoMapper
- Swagger/OpenAPI
- QuestPDF (generación de reportes)
- xUnit (pruebas unitarias)

### Frontend

- AngularJS 1.x
- HTML5/CSS3
- JavaScript ES6

### DevOps

- Docker & Docker Compose
- Multi-stage Docker builds

## Requisitos Previos

- Docker Desktop instalado
- Git
- Windows/Linux/macOS

## Instalación y Despliegue

### 1. Clonar el repositorio

```bash
git clone <URL_DEL_REPOSITORIO>
cd Devsu
```

### 2. Configurar variables de entorno

Crear un archivo `.env` en la raíz del proyecto con el siguiente contenido:

```env
# SQL Server Configuration
SA_PASSWORD=YourStrong@Passw0rd

# Connection String para la API
CONNECTION_STRING=Server=sqlserver,1433;Database=BancoDB;User Id=sa;Password=YourStrong@Passw0rd;TrustServerCertificate=True;Encrypt=False;
```

**IMPORTANTE**: La contraseña de SQL Server debe cumplir con los siguientes requisitos:
- Mínimo 8 caracteres
- Contener mayúsculas, minúsculas, números y símbolos especiales

### 3. Construir las imágenes Docker

```bash
docker-compose build
```

### 4. Levantar los contenedores

```bash
docker-compose up -d
```

Este comando levantará 3 servicios:
- **banco-sqlserver**: SQL Server 2022 en puerto 1433
- **banco-api**: API REST en puerto 5000
- **banco-frontend**: Aplicación web en puerto 8080

### 5. Verificar que los contenedores estén corriendo

```bash
docker-compose ps
```

Todos los servicios deberían mostrar el estado "Up".

### 6. Poblar la base de datos con datos de ejemplo

**IMPORTANTE**: Después de que los contenedores estén corriendo, ejecutar el siguiente script para crear las tablas y poblar la base de datos:

```bash
poblar-datos.bat
```

Este script:
- Verifica que SQL Server esté corriendo
- Ejecuta el script `BaseDatos.sql` que crea las tablas
- Inserta datos de ejemplo (clientes, cuentas y movimientos)

### 7. Acceder a la aplicación

Una vez completados los pasos anteriores:

- **Frontend**: http://localhost:8080
- **API Backend**: http://localhost:5000
- **Swagger UI**: http://localhost:5000/swagger

## Estructura del Proyecto

```
Devsu/
├── BancoBackend/
│   ├── BancoAPI.API/              # Controllers y configuración
│   ├── BancoAPI.Application/      # Servicios, DTOs y lógica de negocio
│   ├── BancoAPI.Domain/           # Entidades e interfaces
│   ├── BancoAPI.Infrastructure/   # Repositorios y DbContext
│   └── BancoAPI.Tests/            # Pruebas unitarias
├── BancoFrontend/
│   ├── app/
│   │   ├── controllers/           # Controladores AngularJS
│   │   ├── services/              # Servicios HTTP
│   │   └── views/                 # Vistas HTML
│   ├── css/                       # Estilos personalizados
│   └── index.html                 # Punto de entrada
├── docker-compose.yml             # Orquestación de contenedores
├── Dockerfile                     # Dockerfile del backend
├── BaseDatos.sql                  # Script de creación de BD
├── poblar-datos.bat               # Script para poblar datos
└── .env                           # Variables de entorno
```

## API Endpoints

### Clientes

- `GET /api/clientes` - Obtener todos los clientes
- `GET /api/clientes/{id}` - Obtener cliente por ID
- `POST /api/clientes` - Crear nuevo cliente
- `PUT /api/clientes/{id}` - Actualizar cliente completo
- `PATCH /api/clientes/{id}` - Actualizar cliente parcialmente
- `DELETE /api/clientes/{id}` - Eliminar cliente

### Cuentas

- `GET /api/cuentas` - Obtener todas las cuentas
- `GET /api/cuentas/{id}` - Obtener cuenta por ID
- `GET /api/cuentas/numero/{numeroCuenta}` - Buscar por número de cuenta
- `GET /api/cuentas/cliente/{clienteId}` - Obtener cuentas de un cliente
- `POST /api/cuentas` - Crear nueva cuenta
- `PUT /api/cuentas/{id}` - Actualizar cuenta
- `PATCH /api/cuentas/{id}` - Actualizar cuenta parcialmente
- `DELETE /api/cuentas/{id}` - Eliminar cuenta

### Movimientos

- `GET /api/movimientos` - Obtener todos los movimientos
- `GET /api/movimientos/{id}` - Obtener movimiento por ID
- `GET /api/movimientos/cuenta/{cuentaId}` - Obtener movimientos de una cuenta
- `POST /api/movimientos` - Crear nuevo movimiento
- `PUT /api/movimientos/{id}` - Actualizar movimiento
- `DELETE /api/movimientos/{id}` - Eliminar movimiento

### Reportes

- `GET /api/reportes?clienteId={id}&fechaInicio={fecha}&fechaFin={fecha}` - Estado de cuenta en JSON
- `GET /api/reportes/pdf?clienteId={id}&fechaInicio={fecha}&fechaFin={fecha}` - Estado de cuenta en PDF (base64)

**Ejemplo de consulta de reporte:**
```
GET /api/reportes?clienteId=1&fechaInicio=2024-01-01&fechaFin=2024-12-31
```

## Reglas de Negocio

### Movimientos

1. **Créditos y Débitos**
   - Los créditos son valores positivos
   - Los débitos son valores negativos

2. **Validación de Saldo**
   - No se permite realizar un débito si el saldo es insuficiente
   - Mensaje de error: "Saldo no disponible"

3. **Límite Diario de Retiros**
   - Límite máximo: $1000 USD por día
   - Se acumulan los débitos del día
   - Mensaje de error: "Cupo diario Excedido"

4. **Cálculo de Saldo**
   - Cada movimiento almacena el saldo resultante después de la transacción
   - El saldo se calcula: `SaldoAnterior + Valor` (donde Valor es positivo para créditos y negativo para débitos)

### Cuentas

- El número de cuenta debe ser único
- El saldo inicial no puede modificarse después de la creación
- No se puede eliminar una cuenta con movimientos asociados

### Clientes

- La identificación debe ser única
- No se puede eliminar un cliente con cuentas asociadas

## Pruebas

### Ejecutar Pruebas Unitarias

```bash
cd BancoBackend
dotnet test
```

### Pruebas del Frontend

```bash
cd BancoFrontend
npm test
```

### Probar Endpoints con Swagger

1. Acceder a http://localhost:5000/swagger
2. Expandir el endpoint deseado
3. Hacer clic en "Try it out"
4. Ingresar los parámetros requeridos
5. Ejecutar y revisar la respuesta

### Colección Postman

Se incluye una colección de Postman con todos los endpoints configurados para facilitar las pruebas.

## Datos de Ejemplo

Después de ejecutar `poblar-datos.bat`, la base de datos contendrá:

### Clientes

| Nombre | Identificación | Teléfono | Contraseña |
|--------|---------------|----------|------------|
| Jose Lema | 098254785 | 098254785 | 1234 |
| Marianela Montalvo | 09754865 | 097548965 | 5678 |
| Juan Osorio | 098874587 | 098874587 | 1245 |

### Cuentas

| Número Cuenta | Tipo | Saldo Inicial | Cliente |
|---------------|------|---------------|---------|
| 478758 | Ahorros | $2000 | Jose Lema |
| 225487 | Corriente | $100 | Marianela Montalvo |
| 495878 | Ahorros | $0 | Juan Osorio |
| 496825 | Ahorros | $540 | Marianela Montalvo |

### Movimientos Iniciales

Se incluyen movimientos de ejemplo para demostrar débitos y créditos.

## Características Técnicas Destacadas

- **Clean Architecture**: Separación clara de responsabilidades
- **SOLID Principles**: Código mantenible y escalable
- **Entity Framework Core**: Code-First con migraciones
- **Exception Handling**: Manejo centralizado de errores con middlewares
- **AutoMapper**: Mapeo automático entre entidades y DTOs
- **Validación de Modelos**: Data Annotations en DTOs
- **Eager Loading**: Optimización de consultas con Include
- **Async/Await**: Operaciones asíncronas en toda la aplicación
- **Expresiones Lambda**: Queries dinámicos y programación funcional
- **Docker Multi-stage**: Optimización de imágenes Docker
- **Health Checks**: Verificación de dependencias en docker-compose

## Comandos Útiles

### Docker

```bash
# Ver logs de todos los servicios
docker-compose logs -f

# Ver logs de un servicio específico
docker-compose logs -f banco-api

# Detener todos los servicios
docker-compose down

# Detener y eliminar volúmenes (resetea la BD)
docker-compose down -v

# Reconstruir solo un servicio
docker-compose build banco-api
docker-compose up -d banco-api
```

### Acceso a SQL Server

```bash
docker exec -it banco-sqlserver /opt/mssql-tools/bin/sqlcmd -S localhost -U sa -P YourStrong@Passw0rd
```

## Solución de Problemas

### Error: "SA_PASSWORD variable is not set"
- Asegurarse de que el archivo `.env` existe en la raíz del proyecto
- Verificar que la contraseña cumple con los requisitos de complejidad

### Error: "Container banco-sqlserver Error"
- Verificar que Docker Desktop está corriendo
- Revisar los logs: `docker-compose logs banco-sqlserver`
- Verificar la contraseña de SQL Server en `.env`

### Error al ejecutar poblar-datos.bat
- Asegurarse de que los contenedores están corriendo: `docker-compose ps`
- Esperar unos segundos después de `docker-compose up -d` para que SQL Server se inicialice completamente

### El frontend no carga
- Verificar que el contenedor está corriendo: `docker ps`
- Revisar logs: `docker-compose logs banco-frontend`
- Verificar que el puerto 8080 no está siendo usado por otra aplicación

### La API no responde
- Verificar que SQL Server está listo (healthcheck)
- Revisar logs: `docker-compose logs banco-api`
- Verificar la cadena de conexión en `.env`

## Licencia

Este proyecto fue desarrollado como parte de una evaluación técnica para Devsu.
