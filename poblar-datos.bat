@echo off
echo ========================================
echo Poblar Base de Datos con Datos de Ejemplo
echo ========================================

echo.
echo Verificando que Docker esté corriendo...
docker ps | findstr banco-sqlserver >nul
if %ERRORLEVEL% NEQ 0 (
    echo ERROR: Docker SQL Server no está corriendo
    echo Por favor ejecuta: docker-compose up -d
    pause
    exit /b 1
)

echo.
echo 1. Ejecutando script SQL dentro de un contenedor temporal...
docker run --rm ^
    --network devsu_banco-network ^
    -v "%cd%":/scripts ^
    mcr.microsoft.com/mssql-tools ^
    /opt/mssql-tools/bin/sqlcmd -S banco-sqlserver -U sa -P 1234@Devsu -i /scripts/BaseDatos.sql

if %ERRORLEVEL% NEQ 0 (
    echo.
    echo  ERROR al ejecutar el script SQL.
    pause
    exit /b 1
)

echo.
echo ========================================
echo  Base de datos poblada exitosamente!
echo ========================================

echo.
echo Puedes verificar los datos en:
echo - Swagger: http://localhost:5000
echo - Endpoint: http://localhost:5000/api/clientes
echo.
echo ========================================

pause
