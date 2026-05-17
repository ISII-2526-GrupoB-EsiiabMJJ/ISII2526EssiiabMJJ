-----------------------------------------------------------
-- LIMPIEZA DE DATOS EXISTENTES
-----------------------------------------------------------
DELETE FROM [dbo].[RentDevices];
DELETE FROM [dbo].[Rental];

DELETE FROM [dbo].[PurchaseItems];
DELETE FROM [dbo].[Purchases];

DELETE FROM [dbo].[ReviewItems];
DELETE FROM [dbo].[Review];
DELETE FROM [dbo].[Device];

DELETE FROM [dbo].[AspNetUserRoles];
DELETE FROM [dbo].[AspNetRoles];
DELETE FROM [dbo].[AspNetUsers];

DELETE FROM [dbo].[Model];

-----------------------------------------------------------
-- MODELOS
-----------------------------------------------------------
SET IDENTITY_INSERT [dbo].[Model] ON;

INSERT INTO [dbo].[Model] ([Id], [NameModel])
VALUES
    (1, 'NVIDIA GeForce RTX 5090'),
    (2, 'NVIDIA GeForce RTX 5080'),
    (3, 'NVIDIA GeForce RTX 4080 Ti'),
    (4, 'NVIDIA GeForce RTX 4090'),

    -- Modelos necesarios para CU ComprarDispositivo
    (5, 'Samsung Galaxy S24'),
    (6, 'Apple iPhone 15'),
    (7, 'Google Pixel 8');

SET IDENTITY_INSERT [dbo].[Model] OFF;

-----------------------------------------------------------
-- USUARIOS (AspNetUsers)
-----------------------------------------------------------
INSERT INTO [dbo].[AspNetUsers]
([Id], [Name], [Surname], [UserName], [NormalizedUserName],
 [Email], [NormalizedEmail], [AccessFailedCount], [EmailConfirmed],
 [PhoneNumberConfirmed], [TwoFactorEnabled], [LockoutEnabled],
 [PasswordHash], [SecurityStamp], [ConcurrencyStamp])
VALUES
('1', 'Petru', 'Vlad', 'rumanoloKo', 'RUMANOLOKO',
 'petru@email.com', 'PETRU@EMAIL.COM', 1, 1,
 1, 1, 1,
 NULL, NEWID(), NEWID()),

('2', 'Vlad', 'Vladislav', 'vladis', 'VLADIS',
 'vlad@email.com', 'VLAD@EMAIL.COM', 1, 1,
 1, 1, 1,
 NULL, NEWID(), NEWID()),

('3', 'Mihai', 'Varcea', 'varicia', 'VARICIA',
 'mihai@email.com', 'MIHAI@EMAIL.COM', 1, 1,
 1, 1, 1,
 NULL, NEWID(), NEWID()),

('4', 'Jaime', 'de los campos', 'Jaime de los campos', 'JAIME DE LOS CAMPOS',
 'jaime@email.com', 'JAIME@EMAIL.COM', 1, 1,
 1, 1, 1,
 NULL, NEWID(), NEWID()),

 ('5', 'Maria', 'Test', 'maria_test@uclm.es', 'MARIA_TEST@UCLM.ES',
 'maria_test@uclm.es', 'MARIA_TEST@UCLM.ES', 1, 1,
 1, 1, 1,
NULL, NEWID(), NEWID()),

('6', 'Maria', 'Test', 'maria@uclm.es', 'MARIA@UCLM.ES',
 'maria@uclm.es', 'MARIA@UCLM.ES', 0, 1,
 0, 0, 1,
 'AQAAAAIAAYagAAAAENLmWIcV18tkCAePy46+UJUSbMRb3CZ3Shuxn8LdzW9wvi7uL56TSDDXG+3od69L7g==',
 NEWID(), NEWID());

-----------------------------------------------------------
-- ROLES
-----------------------------------------------------------
INSERT INTO [dbo].[AspNetRoles]
([Id], [Name], [NormalizedName], [ConcurrencyStamp])
VALUES
('1', 'Cliente', 'CLIENTE', NEWID()),
('2', 'Administrador', 'ADMINISTRADOR', NEWID());

-----------------------------------------------------------
-- USUARIOS - ROLES
-----------------------------------------------------------
INSERT INTO [dbo].[AspNetUserRoles]
([UserId], [RoleId])
VALUES
('6', '1'),
('5','2');

-----------------------------------------------------------
-- RESEÑAS
-----------------------------------------------------------
SET IDENTITY_INSERT [dbo].[Review] ON;

INSERT INTO [dbo].[Review] (
    [ReviewId],
    [CustomerId],
    [ApplicationUserId], -- ahora FK a AspNetUsers.Id
    [DateOfReview],
    [OverallRating],
    [ReviewTitle],
    [CustomerCountry]
)
VALUES
    (1, '1', '2', '2025-09-10 10:00:00', 5, 'Máximo rendimiento en IA y gaming extremo', 'España'),
    (2, '2', '2', '2025-09-11 12:30:00', 4, 'Gran potencia, pero requiere buena ventilación', 'Alemania'),
    (3, '3', '3', '2025-09-12 16:45:00', 5, 'Silencio y eficiencia en largas sesiones', 'Francia');

SET IDENTITY_INSERT [dbo].[Review] OFF;

-----------------------------------------------------------
-- DISPOSITIVOS
-----------------------------------------------------------
SET IDENTITY_INSERT [dbo].[Device] ON;

INSERT INTO [dbo].[Device] (
    [Id],
    [ModelId],
    [Year],
    [Quality],
    [quantityForRent],
    [quantityForPurchase],
    [priceForRent],
    [priceForPurchase],
    [Name],
    [Color],
    [Brand],
    [Description]
)
VALUES
    (1, 1, 2025, 3, 5, 15, 59.99, 2199.99, 'RTX 5090 Founders Edition', 'Negro', 'NVIDIA', 'Máximo rendimiento en gaming 8K y tareas de IA'),
    (2, 2, 2025, 2, 8, 22, 49.99, 1699.99, 'RTX 5080 Gaming Pro', 'Plata', 'MSI', 'Ideal para gaming 4K con DLSS 4.0'),
    (3, 3, 2024, 2, 6, 19, 39.99, 1299.99, 'RTX 4080 Ti Dual Fan', 'Blanco', 'Gigabyte', 'Excelente rendimiento en 1440p y eficiencia térmica'),
    (4, 1, 2025, 3, 3, 12, 64.99, 2299.99, 'RTX 5090 OC Edition', 'Rojo', 'Gigabyte', 'Versión overclockeada con triple ventilador'),
    (5, 4, 2024, 2, 6, 19, 54.99, 1899.99, 'RTX 4090 Dual Ultimate', 'Negro', 'MSI', 'Excelente rendimiento en 4K y eficiencia persistente'),
	    -- Dispositivos necesarios para CU ComprarDispositivo
    (6, 5, 2024, 3, 0, 8, 0, 899.99, 'Galaxy S24', 'Azul', 'Samsung', 'Smartphone Samsung Galaxy S24 disponible para compra'),
    (7, 6, 2024, 3, 0, 10, 0, 999.99, 'iPhone 15', 'Negro', 'Apple', 'Smartphone Apple iPhone 15 disponible para compra'),
    (8, 7, 2024, 3, 0, 6, 0, 799.99, 'Pixel 8', 'Blanco', 'Google', 'Smartphone Google Pixel 8 disponible para compra');
SET IDENTITY_INSERT [dbo].[Device] OFF;

-----------------------------------------------------------
-- RESEÑAS DE DISPOSITIVOS
-----------------------------------------------------------
SET IDENTITY_INSERT [dbo].[ReviewItems] ON;

INSERT INTO [dbo].[ReviewItems] (
    [Id],
    [Rating],
    [DeviceId],
    [ReviewId],
    [Comment]
)
VALUES
    (1, 5, 1, 1, 'Potencia brutal, ideal para IA y gaming extremo'),
    (2, 4, 1, 2, 'Requiere buena ventilación, pero rinde excelente'),
    (3, 5, 2, 3, 'Silenciosa y eficiente en cargas largas'),
    (4, 4, 3, 2, 'Buen rendimiento en 1440p, drivers estables');

SET IDENTITY_INSERT [dbo].[ReviewItems] OFF;

-----------------------------------------------------------
-- RENTAL
-----------------------------------------------------------
SET IDENTITY_INSERT [dbo].[Rental] ON;

INSERT INTO [dbo].[Rental] (
    [Id],
    [ApplicationUserId], -- FK a AspNetUsers.Id
    [Name],
    [Surname],
    [DeliveryAddress],
    [RentalDate],
    [PaymentMethod],
    [RentalDateFrom],
    [RentalDateTo],
    [TotalPrice]
)
VALUES
(
    1,
    (SELECT Id FROM [dbo].[AspNetUsers] WHERE UserName = 'Jaime de los campos'),
    'Jaime',
    'de los campos',
    'Av. españa 31',
    GETDATE(),
    0, -- Cash
    '2025-11-30',
    '2026-12-05',
    49.99 + 39.99 + 64.99
);

SET IDENTITY_INSERT [dbo].[Rental] OFF;

-----------------------------------------------------------
-- RENTDEVICE
-----------------------------------------------------------
INSERT INTO [dbo].[RentDevices] (
    [DeviceId],
    [RentalId],
    [Price],
    [Quantity],
    [Description]
)
VALUES
    (2, 1, 49.99, 1, 'Tarjeta gráfica de última generación ideal para gaming 4K y renderizado profesional.'),
    (3, 1, 39.99, 1, 'Excelente rendimiento para videojuegos exigentes y aplicaciones de diseño 3D.'),
    (4, 1, 64.99, 1, 'Modelo tope de gama con gran eficiencia energética y potencia para IA y gráficos avanzados.');
