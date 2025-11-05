-- ========================================
-- BaseDatos.sql - Script de Base de Datos
-- Sistema Bancario - Ejercicio Técnico Devsu
-- ========================================

USE master;
GO

-- Crear base de datos si no existe
IF NOT EXISTS (SELECT * FROM sys.databases WHERE name = 'BancoDB')
BEGIN
    CREATE DATABASE BancoDB;
    PRINT 'Base de datos BancoDB creada exitosamente';
END
GO

USE BancoDB;
GO

-- ========================================
-- TABLAS
-- ========================================

-- Tabla Personas (base para herencia TPT)
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Personas')
BEGIN
    CREATE TABLE Personas (
        PersonaId INT PRIMARY KEY IDENTITY(1,1),
        Nombre NVARCHAR(100) NOT NULL,
        Genero NVARCHAR(10) NOT NULL,
        Edad INT NOT NULL,
        Identificacion NVARCHAR(20) NOT NULL,
        Direccion NVARCHAR(200) NOT NULL,
        Telefono NVARCHAR(15) NOT NULL,
        CONSTRAINT CHK_Personas_Edad CHECK (Edad >= 18 AND Edad <= 120)
    );
    
    CREATE UNIQUE INDEX IX_Personas_Identificacion ON Personas(Identificacion);
    PRINT 'Tabla Personas creada';
END
GO

-- Tabla Clientes (hereda de Personas - TPT - Table Per Type)
-- IMPORTANTE: En TPT, la tabla derivada comparte la misma PK con la tabla base
-- PersonaId es tanto PK como FK hacia Personas
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Clientes')
BEGIN
    CREATE TABLE Clientes (
        PersonaId INT PRIMARY KEY,  -- PK que también es FK hacia Personas
        Contrasena NVARCHAR(100) NOT NULL,
        Estado BIT NOT NULL DEFAULT 1,
        CONSTRAINT FK_Clientes_Personas FOREIGN KEY (PersonaId) REFERENCES Personas(PersonaId) ON DELETE CASCADE
    );
    PRINT 'Tabla Clientes creada con patrón TPT (Table-per-Type)';
END
GO

-- Tabla Cuentas
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Cuentas')
BEGIN
    CREATE TABLE Cuentas (
        CuentaId INT PRIMARY KEY IDENTITY(1,1),
        NumeroCuenta NVARCHAR(20) NOT NULL,
        TipoCuenta NVARCHAR(20) NOT NULL,
        SaldoInicial DECIMAL(18,2) NOT NULL,
        Estado BIT NOT NULL DEFAULT 1,
        ClienteId INT NOT NULL,  -- FK que referencia a Clientes(PersonaId)
        FechaCreacion DATETIME NOT NULL DEFAULT GETDATE(),
        FechaActualizacion DATETIME NULL,
        CONSTRAINT FK_Cuentas_Clientes FOREIGN KEY (ClienteId) REFERENCES Clientes(PersonaId) ON DELETE NO ACTION,
        CONSTRAINT CHK_Cuentas_SaldoInicial CHECK (SaldoInicial >= 0)
    );

    CREATE UNIQUE INDEX IX_Cuentas_NumeroCuenta ON Cuentas(NumeroCuenta);
    CREATE INDEX IX_Cuentas_ClienteId ON Cuentas(ClienteId);
    PRINT 'Tabla Cuentas creada';
END
GO

-- Tabla Movimientos
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Movimientos')
BEGIN
    CREATE TABLE Movimientos (
        MovimientoId INT PRIMARY KEY IDENTITY(1,1),
        Fecha DATETIME NOT NULL DEFAULT GETDATE(),
        TipoMovimiento NVARCHAR(20) NOT NULL,
        Valor DECIMAL(18,2) NOT NULL,
        Saldo DECIMAL(18,2) NOT NULL,
        CuentaId INT NOT NULL,
        CONSTRAINT FK_Movimientos_Cuentas FOREIGN KEY (CuentaId) REFERENCES Cuentas(CuentaId) ON DELETE NO ACTION,
        CONSTRAINT CHK_Movimientos_Saldo CHECK (Saldo >= 0)
    );
    
    CREATE INDEX IX_Movimientos_CuentaId ON Movimientos(CuentaId);
    CREATE INDEX IX_Movimientos_Fecha ON Movimientos(Fecha);
    CREATE INDEX IX_Movimientos_CuentaId_Fecha ON Movimientos(CuentaId, Fecha);
    PRINT 'Tabla Movimientos creada';
END
GO

-- ========================================
-- DATOS DE EJEMPLO (Casos de uso del documento)
-- ========================================

PRINT 'Insertando datos de ejemplo...';

-- 1. CREACIÓN DE USUARIOS (Personas + Clientes)


-- Jose Lema
IF NOT EXISTS (SELECT * FROM Personas WHERE Identificacion = '1234567890')
BEGIN
    INSERT INTO Personas (Nombre, Genero, Edad, Identificacion, Direccion, Telefono)
    VALUES ('Jose Lema', 'M', 35, '1234567890', 'Otavalo sn y principal', '098254785');
    
    DECLARE @PersonaId1 INT = SCOPE_IDENTITY();
    
    INSERT INTO Clientes (PersonaId, Contrasena, Estado)
    VALUES (@PersonaId1, '1234', 1);
    
    PRINT 'Cliente Jose Lema creado';
END
GO

-- Marianela Montalvo
IF NOT EXISTS (SELECT * FROM Personas WHERE Identificacion = '0987654321')
BEGIN
    INSERT INTO Personas (Nombre, Genero, Edad, Identificacion, Direccion, Telefono)
    VALUES ('Marianela Montalvo', 'F', 28, '0987654321', 'Amazonas y NNUU', '097548965');
    
    DECLARE @PersonaId2 INT = SCOPE_IDENTITY();
    
    INSERT INTO Clientes (PersonaId, Contrasena, Estado)
    VALUES (@PersonaId2, '5678', 1);
    
    PRINT 'Cliente Marianela Montalvo creado';
END
GO

-- Juan Osorio
IF NOT EXISTS (SELECT * FROM Personas WHERE Identificacion = '1122334455')
BEGIN
    INSERT INTO Personas (Nombre, Genero, Edad, Identificacion, Direccion, Telefono)
    VALUES ('Juan Osorio', 'M', 42, '1122334455', '13 junio y Equinoccial', '098874587');
    
    DECLARE @PersonaId3 INT = SCOPE_IDENTITY();
    
    INSERT INTO Clientes (PersonaId, Contrasena, Estado)
    VALUES (@PersonaId3, '1245', 1);
    
    PRINT 'Cliente Juan Osorio creado';
END
GO

-- 2. CREACIÓN DE CUENTAS
-- En TPT, PersonaId es la PK de Cliente, por lo que ClienteId = PersonaId

DECLARE @ClienteId1 INT = (SELECT c.PersonaId FROM Clientes c JOIN Personas p ON c.PersonaId = p.PersonaId WHERE p.Identificacion = '1234567890');
DECLARE @ClienteId2 INT = (SELECT c.PersonaId FROM Clientes c JOIN Personas p ON c.PersonaId = p.PersonaId WHERE p.Identificacion = '0987654321');
DECLARE @ClienteId3 INT = (SELECT c.PersonaId FROM Clientes c JOIN Personas p ON c.PersonaId = p.PersonaId WHERE p.Identificacion = '1122334455');

-- Cuenta 478758 - Jose Lema - Ahorros
IF NOT EXISTS (SELECT * FROM Cuentas WHERE NumeroCuenta = '478758')
BEGIN
    INSERT INTO Cuentas (NumeroCuenta, TipoCuenta, SaldoInicial, Estado, ClienteId)
    VALUES ('478758', 'Ahorros', 2000, 1, @ClienteId1);
    PRINT 'Cuenta 478758 creada para Jose Lema';
END

-- Cuenta 225487 - Marianela Montalvo - Corriente
IF NOT EXISTS (SELECT * FROM Cuentas WHERE NumeroCuenta = '225487')
BEGIN
    INSERT INTO Cuentas (NumeroCuenta, TipoCuenta, SaldoInicial, Estado, ClienteId)
    VALUES ('225487', 'Corriente', 100, 1, @ClienteId2);
    PRINT 'Cuenta 225487 creada para Marianela Montalvo';
END

-- Cuenta 495878 - Juan Osorio - Ahorros
IF NOT EXISTS (SELECT * FROM Cuentas WHERE NumeroCuenta = '495878')
BEGIN
    INSERT INTO Cuentas (NumeroCuenta, TipoCuenta, SaldoInicial, Estado, ClienteId)
    VALUES ('495878', 'Ahorros', 0, 1, @ClienteId3);
    PRINT 'Cuenta 495878 creada para Juan Osorio';
END

-- Cuenta 496825 - Marianela Montalvo - Ahorros
IF NOT EXISTS (SELECT * FROM Cuentas WHERE NumeroCuenta = '496825')
BEGIN
    INSERT INTO Cuentas (NumeroCuenta, TipoCuenta, SaldoInicial, Estado, ClienteId)
    VALUES ('496825', 'Ahorros', 540, 1, @ClienteId2);
    PRINT 'Cuenta 496825 creada para Marianela Montalvo';
END

-- 3. Nueva cuenta corriente para Jose Lema (Caso de uso 3 del documento)
IF NOT EXISTS (SELECT * FROM Cuentas WHERE NumeroCuenta = '585545')
BEGIN
    INSERT INTO Cuentas (NumeroCuenta, TipoCuenta, SaldoInicial, Estado, ClienteId)
    VALUES ('585545', 'Corriente', 1000, 1, @ClienteId1);
    PRINT 'Cuenta 585545 creada para Jose Lema';
END
GO

-- 4. MOVIMIENTOS


DECLARE @CuentaId1 INT = (SELECT CuentaId FROM Cuentas WHERE NumeroCuenta = '478758');
DECLARE @CuentaId2 INT = (SELECT CuentaId FROM Cuentas WHERE NumeroCuenta = '225487');
DECLARE @CuentaId3 INT = (SELECT CuentaId FROM Cuentas WHERE NumeroCuenta = '495878');
DECLARE @CuentaId4 INT = (SELECT CuentaId FROM Cuentas WHERE NumeroCuenta = '496825');

-- Retiro de 575 en cuenta 478758 (Jose Lema)
IF NOT EXISTS (SELECT * FROM Movimientos WHERE CuentaId = @CuentaId1)
BEGIN
    INSERT INTO Movimientos (Fecha, TipoMovimiento, Valor, Saldo, CuentaId)
    VALUES (GETDATE(), 'Retiro', -575, 1425, @CuentaId1);
    PRINT 'Movimiento: Retiro de 575 en cuenta 478758';
END

-- Deposito de 600 en cuenta 225487 (Marianela Montalvo)
IF NOT EXISTS (SELECT * FROM Movimientos WHERE CuentaId = @CuentaId2)
BEGIN
    INSERT INTO Movimientos (Fecha, TipoMovimiento, Valor, Saldo, CuentaId)
    VALUES (GETDATE(), 'Deposito', 600, 700, @CuentaId2);
    PRINT 'Movimiento: Deposito de 600 en cuenta 225487';
END

-- Deposito de 150 en cuenta 495878 (Juan Osorio)
IF NOT EXISTS (SELECT * FROM Movimientos WHERE CuentaId = @CuentaId3)
BEGIN
    INSERT INTO Movimientos (Fecha, TipoMovimiento, Valor, Saldo, CuentaId)
    VALUES (GETDATE(), 'Deposito', 150, 150, @CuentaId3);
    PRINT 'Movimiento: Deposito de 150 en cuenta 495878';
END

-- Retiro de 540 en cuenta 496825 (Marianela Montalvo)
IF NOT EXISTS (SELECT * FROM Movimientos WHERE CuentaId = @CuentaId4)
BEGIN
    INSERT INTO Movimientos (Fecha, TipoMovimiento, Valor, Saldo, CuentaId)
    VALUES (GETDATE(), 'Retiro', -540, 0, @CuentaId4);
    PRINT 'Movimiento: Retiro de 540 en cuenta 496825';
END
GO

-- ========================================
-- VERIFICACIÓN
-- ========================================

PRINT '';
PRINT '========================================';
PRINT 'BASE DE DATOS CREADA EXITOSAMENTE';
PRINT '========================================';
PRINT '';
PRINT 'Resumen de datos:';
SELECT 'Personas' AS Tabla, COUNT(*) AS Total FROM Personas
UNION ALL
SELECT 'Clientes', COUNT(*) FROM Clientes
UNION ALL
SELECT 'Cuentas', COUNT(*) FROM Cuentas
UNION ALL
SELECT 'Movimientos', COUNT(*) FROM Movimientos;

PRINT '';
PRINT 'Clientes creados:';
SELECT p.Nombre, p.Identificacion, c.Estado
FROM Clientes c
JOIN Personas p ON c.PersonaId = p.PersonaId;

PRINT '';
PRINT 'Cuentas creadas:';
SELECT c.NumeroCuenta, c.TipoCuenta, c.SaldoInicial, p.Nombre AS Cliente
FROM Cuentas c
JOIN Clientes cl ON c.ClienteId = cl.PersonaId
JOIN Personas p ON cl.PersonaId = p.PersonaId;

PRINT '';
PRINT '========================================';
PRINT 'Script completado exitosamente';
PRINT '========================================';
GO
