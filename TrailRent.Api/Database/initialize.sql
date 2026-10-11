IF OBJECT_ID(N'dbo.Customers', N'U') IS NULL
CREATE TABLE dbo.Customers (
    Id bigint IDENTITY(1,1) PRIMARY KEY, FirstName nvarchar(50) NOT NULL, LastName nvarchar(50) NOT NULL,
    Email nvarchar(100) NOT NULL UNIQUE, PhoneNumber nvarchar(20) NULL, IsActive bit NOT NULL DEFAULT 1,
    IsDeleted bit NOT NULL DEFAULT 0, UpdatedAt datetime2 NULL,
    CONSTRAINT CK_Customers_Email CHECK (Email LIKE N'%_@_%._%')
);
GO
IF OBJECT_ID(N'dbo.Equipment', N'U') IS NULL
CREATE TABLE dbo.Equipment (
    Id bigint IDENTITY(1,1) PRIMARY KEY, Name nvarchar(200) NOT NULL UNIQUE, DailyRate decimal(18,2) NOT NULL,
    AvailableUnits int NOT NULL, IsRentable bit NOT NULL DEFAULT 1, IsDeleted bit NOT NULL DEFAULT 0, UpdatedAt datetime2 NULL,
    CONSTRAINT CK_Equipment_Rate CHECK (DailyRate > 0), CONSTRAINT CK_Equipment_Units CHECK (AvailableUnits >= 0)
);
GO
IF OBJECT_ID(N'dbo.Rentals', N'U') IS NULL
CREATE TABLE dbo.Rentals (
    Id bigint IDENTITY(1,1) PRIMARY KEY, CustomerId bigint NOT NULL, StartDate date NOT NULL, EndDate date NOT NULL,
    Status nvarchar(30) NOT NULL DEFAULT N'Draft', TotalAmount decimal(18,2) NOT NULL, PickupLocation nvarchar(150) NOT NULL,
    IsDeleted bit NOT NULL DEFAULT 0, UpdatedAt datetime2 NULL, RowVersion rowversion,
    CONSTRAINT FK_Rentals_Customers FOREIGN KEY (CustomerId) REFERENCES dbo.Customers(Id),
    CONSTRAINT CK_Rentals_Dates CHECK (EndDate > StartDate),
    CONSTRAINT CK_Rentals_Status CHECK (Status IN (N'Draft', N'AwaitingPickup', N'Active', N'Returned', N'Cancelled'))
);
GO
IF OBJECT_ID(N'dbo.RentalItems', N'U') IS NULL
CREATE TABLE dbo.RentalItems (
    RentalId bigint NOT NULL, EquipmentId bigint NOT NULL, EquipmentName nvarchar(200) NOT NULL,
    DailyRate decimal(18,2) NOT NULL, Quantity int NOT NULL,
    CONSTRAINT PK_RentalItems PRIMARY KEY (RentalId, EquipmentId),
    CONSTRAINT FK_RentalItems_Rentals FOREIGN KEY (RentalId) REFERENCES dbo.Rentals(Id) ON DELETE CASCADE,
    CONSTRAINT FK_RentalItems_Equipment FOREIGN KEY (EquipmentId) REFERENCES dbo.Equipment(Id),
    CONSTRAINT CK_RentalItems_Quantity CHECK (Quantity > 0)
);
GO
CREATE OR ALTER PROCEDURE dbo.usp_ConfirmRental @RentalId bigint AS
BEGIN
    SET NOCOUNT ON;
    DECLARE @Status nvarchar(30);
    SELECT @Status = Status FROM dbo.Rentals WITH (UPDLOCK) WHERE Id = @RentalId AND IsDeleted = 0;
    IF @Status IS NULL THROW 51001, 'Rental not found.', 1;
    IF @Status <> N'Draft' THROW 51002, 'Only a draft rental can be confirmed.', 1;
    UPDATE dbo.Rentals SET Status = N'AwaitingPickup', UpdatedAt = SYSUTCDATETIME() WHERE Id = @RentalId;
END;
GO
IF NOT EXISTS (SELECT 1 FROM dbo.Customers)
INSERT dbo.Customers (FirstName, LastName, Email, PhoneNumber) VALUES
 (N'Олена', N'Коваль', N'olena@example.com', N'+380501112233'),
 (N'Максим', N'Бойко', N'maksym@example.com', N'+380674445566');
IF NOT EXISTS (SELECT 1 FROM dbo.Equipment)
INSERT dbo.Equipment (Name, DailyRate, AvailableUnits) VALUES
 (N'Намет Alpine Duo 2P', 420, 8), (N'Рюкзак Ridge 65L', 250, 12),
 (N'Спальник FrostLine -5°C', 210, 16), (N'Пальник FireTrail Compact', 160, 10);
GO
