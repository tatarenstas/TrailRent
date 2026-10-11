USE master;
GO

IF DB_ID(N'TrailRent_BookingDB') IS NOT NULL
BEGIN
    ALTER DATABASE TrailRent_BookingDB SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
    DROP DATABASE TrailRent_BookingDB;
END
GO

CREATE DATABASE TrailRent_BookingDB;
GO
USE TrailRent_BookingDB;
GO

CREATE TABLE dbo.Customers (
    CustomerId      bigint IDENTITY(1,1) NOT NULL CONSTRAINT PK_Customers PRIMARY KEY,
    FirstName       nvarchar(50) NOT NULL,
    LastName        nvarchar(50) NOT NULL,
    Email           nvarchar(100) NOT NULL CONSTRAINT UQ_Customers_Email UNIQUE,
    PhoneNumber     nvarchar(20) NULL,
    EmergencyContact nvarchar(100) NULL,
    IsActive        bit NOT NULL CONSTRAINT DF_Customers_IsActive DEFAULT 1,
    CONSTRAINT CK_Customers_Name CHECK (LEN(LTRIM(RTRIM(FirstName))) > 0 AND LEN(LTRIM(RTRIM(LastName))) > 0),
    CONSTRAINT CK_Customers_Email CHECK (Email LIKE N'%_@_%._%')
);
GO

CREATE TABLE dbo.MembershipPlans (
    MembershipPlanId bigint IDENTITY(1,1) NOT NULL CONSTRAINT PK_MembershipPlans PRIMARY KEY,
    PlanCode         nvarchar(30) NOT NULL CONSTRAINT UQ_MembershipPlans_PlanCode UNIQUE,
    PlanName         nvarchar(100) NOT NULL,
    DiscountPercent  decimal(5,2) NOT NULL,
    StartsOn         date NOT NULL,
    EndsOn           date NOT NULL,
    IsActive         bit NOT NULL CONSTRAINT DF_MembershipPlans_IsActive DEFAULT 1,
    CONSTRAINT CK_MembershipPlans_Discount CHECK (DiscountPercent >= 0 AND DiscountPercent <= 100),
    CONSTRAINT CK_MembershipPlans_Dates CHECK (EndsOn > StartsOn)
);
GO

-- Локальна read-модель: власник назви та тарифу — TrailRent_CatalogDB.
CREATE TABLE dbo.Equipment (
    EquipmentId   bigint NOT NULL CONSTRAINT PK_Equipment PRIMARY KEY,
    EquipmentName nvarchar(200) NOT NULL,
    DailyRate     decimal(18,2) NOT NULL,
    IsRentable    bit NOT NULL CONSTRAINT DF_BookingEquipment_IsRentable DEFAULT 1,
    CONSTRAINT CK_BookingEquipment_Name CHECK (LEN(LTRIM(RTRIM(EquipmentName))) > 0),
    CONSTRAINT CK_BookingEquipment_DailyRate CHECK (DailyRate > 0)
);
GO

CREATE TABLE dbo.Rentals (
    RentalId         bigint IDENTITY(1,1) NOT NULL CONSTRAINT PK_Rentals PRIMARY KEY,
    CustomerId       bigint NOT NULL,
    MembershipPlanId bigint NULL,
    StartDate        date NOT NULL,
    EndDate          date NOT NULL,
    RentalStatus     nvarchar(30) NOT NULL CONSTRAINT DF_Rentals_Status DEFAULT N'Draft',
    TotalAmount      decimal(18,2) NOT NULL,
    PickupLocation   nvarchar(150) NOT NULL,
    Notes            nvarchar(500) NULL,
    CONSTRAINT CK_Rentals_Dates CHECK (EndDate > StartDate),
    CONSTRAINT CK_Rentals_Status CHECK (RentalStatus IN (N'Draft', N'AwaitingPickup', N'Active', N'Returned', N'Cancelled')),
    CONSTRAINT CK_Rentals_TotalAmount CHECK (TotalAmount >= 0),
    CONSTRAINT FK_Rentals_Customers FOREIGN KEY (CustomerId) REFERENCES dbo.Customers(CustomerId),
    CONSTRAINT FK_Rentals_MembershipPlans FOREIGN KEY (MembershipPlanId) REFERENCES dbo.MembershipPlans(MembershipPlanId) ON DELETE SET NULL
);
GO

CREATE TABLE dbo.RentalItems (
    RentalId      bigint NOT NULL,
    EquipmentId   bigint NOT NULL,
    EquipmentName nvarchar(200) NOT NULL,
    DailyRate     decimal(18,2) NOT NULL,
    Quantity      int NOT NULL,
    CONSTRAINT PK_RentalItems PRIMARY KEY (RentalId, EquipmentId),
    CONSTRAINT CK_RentalItems_DailyRate CHECK (DailyRate >= 0),
    CONSTRAINT CK_RentalItems_Quantity CHECK (Quantity > 0),
    CONSTRAINT FK_RentalItems_Rentals FOREIGN KEY (RentalId) REFERENCES dbo.Rentals(RentalId) ON DELETE CASCADE,
    CONSTRAINT FK_RentalItems_Equipment FOREIGN KEY (EquipmentId) REFERENCES dbo.Equipment(EquipmentId)
);
GO

CREATE TABLE dbo.PaymentRecords (
    RentalId         bigint NOT NULL CONSTRAINT PK_PaymentRecords PRIMARY KEY,
    PaymentMethod    nvarchar(50) NOT NULL,
    Amount           decimal(18,2) NOT NULL,
    PaymentStatus    nvarchar(30) NOT NULL CONSTRAINT DF_PaymentRecords_Status DEFAULT N'Pending',
    TransactionDate  datetime2 NOT NULL CONSTRAINT DF_PaymentRecords_Date DEFAULT SYSUTCDATETIME(),
    CONSTRAINT CK_PaymentRecords_Amount CHECK (Amount >= 0),
    CONSTRAINT CK_PaymentRecords_Status CHECK (PaymentStatus IN (N'Pending', N'Completed', N'Failed', N'Refunded')),
    CONSTRAINT FK_PaymentRecords_Rentals FOREIGN KEY (RentalId) REFERENCES dbo.Rentals(RentalId) ON DELETE CASCADE
);
GO

CREATE INDEX IX_Rentals_Customer_Status ON dbo.Rentals(CustomerId, RentalStatus) INCLUDE (StartDate, EndDate, TotalAmount);
CREATE INDEX IX_Rentals_Active ON dbo.Rentals(StartDate, EndDate) WHERE RentalStatus = N'Active';
CREATE INDEX IX_RentalItems_EquipmentId ON dbo.RentalItems(EquipmentId);
CREATE INDEX IX_PaymentRecords_Pending ON dbo.PaymentRecords(TransactionDate) WHERE PaymentStatus = N'Pending';
GO

ALTER TABLE dbo.Customers ADD
    CreatedAt datetime2 NOT NULL CONSTRAINT DF_Customers_CreatedAt DEFAULT SYSUTCDATETIME(),
    CreatedBy nvarchar(100) NULL,
    UpdatedAt datetime2 NULL,
    UpdatedBy nvarchar(100) NULL,
    IsDeleted bit NOT NULL CONSTRAINT DF_Customers_IsDeleted DEFAULT 0;
GO
ALTER TABLE dbo.MembershipPlans ADD
    CreatedAt datetime2 NOT NULL CONSTRAINT DF_MembershipPlans_CreatedAt DEFAULT SYSUTCDATETIME(),
    CreatedBy nvarchar(100) NULL,
    UpdatedAt datetime2 NULL,
    UpdatedBy nvarchar(100) NULL,
    IsDeleted bit NOT NULL CONSTRAINT DF_MembershipPlans_IsDeleted DEFAULT 0;
GO
ALTER TABLE dbo.Rentals ADD
    CreatedAt datetime2 NOT NULL CONSTRAINT DF_Rentals_CreatedAt DEFAULT SYSUTCDATETIME(),
    CreatedBy nvarchar(100) NULL,
    UpdatedAt datetime2 NULL,
    UpdatedBy nvarchar(100) NULL,
    IsDeleted bit NOT NULL CONSTRAINT DF_Rentals_IsDeleted DEFAULT 0,
    RowVersion rowversion;
GO

CREATE INDEX IX_Customers_Active ON dbo.Customers(LastName, FirstName) WHERE IsDeleted = 0;
CREATE INDEX IX_Rentals_NotDeleted ON dbo.Rentals(CustomerId) WHERE IsDeleted = 0;
GO

CREATE OR ALTER VIEW dbo.vw_RentalSummary AS
SELECT r.RentalId, r.StartDate, r.EndDate, r.RentalStatus, r.TotalAmount, r.PickupLocation,
       c.CustomerId, CONCAT(c.FirstName, N' ', c.LastName) AS CustomerName, c.Email,
       m.PlanCode, m.DiscountPercent,
       p.PaymentMethod, p.PaymentStatus
FROM dbo.Rentals AS r
JOIN dbo.Customers AS c ON c.CustomerId = r.CustomerId
LEFT JOIN dbo.MembershipPlans AS m ON m.MembershipPlanId = r.MembershipPlanId
LEFT JOIN dbo.PaymentRecords AS p ON p.RentalId = r.RentalId
WHERE r.IsDeleted = 0;
GO
