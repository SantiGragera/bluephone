-- Blue Phone - Script de base de datos (SQL Server)
-- Ejecutar en SSMS conectado a la instancia local (localhost).

IF DB_ID('BluePhone') IS NULL
    CREATE DATABASE BluePhone;
GO

USE BluePhone;
GO

-- Se recrean las tablas para poder ejecutar el script más de una vez.
IF OBJECT_ID('dbo.productos', 'U') IS NOT NULL DROP TABLE dbo.productos;
IF OBJECT_ID('dbo.categorias', 'U') IS NOT NULL DROP TABLE dbo.categorias;
GO

CREATE TABLE categorias (
    idCategoria INT IDENTITY(1,1) PRIMARY KEY,
    descripcion VARCHAR(50) NOT NULL
);

CREATE TABLE productos (
    idProducto  INT IDENTITY(1,1) PRIMARY KEY,
    nombre      VARCHAR(100) NOT NULL,
    precio      DECIMAL(10,2) NOT NULL,
    idCategoria INT NOT NULL,
    CONSTRAINT FK_productos_categorias
        FOREIGN KEY (idCategoria) REFERENCES categorias(idCategoria)
);
GO

INSERT INTO categorias (descripcion) VALUES
('iPhone'), ('Mac'), ('iPad'), ('Apple Watch'), ('AirPods y Accesorios');

INSERT INTO productos (nombre, precio, idCategoria) VALUES
('iPhone 16 Pro 256GB', 1199.00, 1),
('iPhone 16 128GB', 799.00, 1),
('MacBook Air M3 13"', 1099.00, 2),
('MacBook Pro M3 14"', 1599.00, 2),
('iPad Air 11"', 599.00, 3),
('iPad 10ª generación', 349.00, 3),
('Apple Watch Series 10', 399.00, 4),
('Apple Watch SE', 249.00, 4),
('AirPods Pro 2', 249.00, 5),
('Cargador MagSafe', 39.00, 5);
GO

-- Verificación (JOIN entre las dos tablas)
SELECT p.idProducto, p.nombre, p.precio, c.descripcion
FROM productos p
INNER JOIN categorias c ON p.idCategoria = c.idCategoria;
GO
