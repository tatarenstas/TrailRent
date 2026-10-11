USE master;
GO

IF DB_ID(N'TrailRent_CatalogDB') IS NOT NULL
BEGIN
    ALTER DATABASE TrailRent_CatalogDB SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
    DROP DATABASE TrailRent_CatalogDB;
END
GO

CREATE DATABASE TrailRent_CatalogDB;
GO
USE TrailRent_CatalogDB;
GO

CREATE TABLE dbo.EquipmentCategories (
    CategoryId bigint IDENTITY(1,1) NOT NULL CONSTRAINT PK_EquipmentCategories PRIMARY KEY,
    CategoryName nvarchar(100) NOT NULL CONSTRAINT UQ_EquipmentCategories_Name UNIQUE,
    Description nvarchar(500) NULL,
    CONSTRAINT CK_EquipmentCategories_Name CHECK (LEN(LTRIM(RTRIM(CategoryName))) > 0)
);
GO

CREATE TABLE dbo.Equipment (
    EquipmentId bigint IDENTITY(1,1) NOT NULL CONSTRAINT PK_CatalogEquipment PRIMARY KEY,
    CategoryId bigint NOT NULL,
    EquipmentName nvarchar(200) NOT NULL,
    DailyRate decimal(18,2) NOT NULL,
    DepositAmount decimal(18,2) NOT NULL CONSTRAINT DF_Equipment_Deposit DEFAULT 0,
    AvailableUnits int NOT NULL CONSTRAINT DF_Equipment_AvailableUnits DEFAULT 0,
    IsRentable bit NOT NULL CONSTRAINT DF_Equipment_IsRentable DEFAULT 1,
    CONSTRAINT UQ_Equipment_Name UNIQUE (EquipmentName),
    CONSTRAINT CK_Equipment_Name CHECK (LEN(LTRIM(RTRIM(EquipmentName))) > 0),
    CONSTRAINT CK_Equipment_DailyRate CHECK (DailyRate > 0),
    CONSTRAINT CK_Equipment_Deposit CHECK (DepositAmount >= 0),
    CONSTRAINT CK_Equipment_AvailableUnits CHECK (AvailableUnits >= 0),
    CONSTRAINT FK_Equipment_Categories FOREIGN KEY (CategoryId) REFERENCES dbo.EquipmentCategories(CategoryId)
);
GO

CREATE TABLE dbo.EquipmentSpecs (
    EquipmentId bigint NOT NULL CONSTRAINT PK_EquipmentSpecs PRIMARY KEY,
    Brand nvarchar(100) NOT NULL,
    Model nvarchar(100) NOT NULL,
    WeightGrams int NOT NULL,
    Material nvarchar(100) NULL,
    SeasonRating nvarchar(50) NULL,
    CapacityValue decimal(10,2) NULL,
    CapacityUnit nvarchar(20) NULL,
    CONSTRAINT CK_EquipmentSpecs_Weight CHECK (WeightGrams > 0),
    CONSTRAINT CK_EquipmentSpecs_Capacity CHECK ((CapacityValue IS NULL AND CapacityUnit IS NULL) OR (CapacityValue > 0 AND CapacityUnit IS NOT NULL)),
    CONSTRAINT FK_EquipmentSpecs_Equipment FOREIGN KEY (EquipmentId) REFERENCES dbo.Equipment(EquipmentId) ON DELETE CASCADE
);
GO

CREATE TABLE dbo.MaintenancePolicies (
    EquipmentId bigint NOT NULL CONSTRAINT PK_MaintenancePolicies PRIMARY KEY,
    InspectionIntervalDays int NOT NULL,
    CleaningInstruction nvarchar(1000) NOT NULL,
    RequiresSpecialist bit NOT NULL CONSTRAINT DF_MaintenancePolicies_Specialist DEFAULT 0,
    CONSTRAINT CK_MaintenancePolicies_Interval CHECK (InspectionIntervalDays > 0),
    CONSTRAINT FK_MaintenancePolicies_Equipment FOREIGN KEY (EquipmentId) REFERENCES dbo.Equipment(EquipmentId) ON DELETE CASCADE
);
GO

CREATE TABLE dbo.EquipmentPhotos (
    PhotoId bigint IDENTITY(1,1) NOT NULL CONSTRAINT PK_EquipmentPhotos PRIMARY KEY,
    EquipmentId bigint NOT NULL,
    ImageUrl nvarchar(500) NOT NULL,
    IsMain bit NOT NULL CONSTRAINT DF_EquipmentPhotos_IsMain DEFAULT 0,
    CONSTRAINT CK_EquipmentPhotos_Url CHECK (LEN(LTRIM(RTRIM(ImageUrl))) > 0),
    CONSTRAINT FK_EquipmentPhotos_Equipment FOREIGN KEY (EquipmentId) REFERENCES dbo.Equipment(EquipmentId) ON DELETE CASCADE
);
GO

CREATE TABLE dbo.Features (
    FeatureId bigint IDENTITY(1,1) NOT NULL CONSTRAINT PK_Features PRIMARY KEY,
    FeatureName nvarchar(80) NOT NULL CONSTRAINT UQ_Features_Name UNIQUE,
    CONSTRAINT CK_Features_Name CHECK (LEN(LTRIM(RTRIM(FeatureName))) > 0)
);
GO

CREATE TABLE dbo.EquipmentFeatures (
    EquipmentId bigint NOT NULL,
    FeatureId bigint NOT NULL,
    AddedAt datetime2 NOT NULL CONSTRAINT DF_EquipmentFeatures_AddedAt DEFAULT SYSUTCDATETIME(),
    CONSTRAINT PK_EquipmentFeatures PRIMARY KEY (EquipmentId, FeatureId),
    CONSTRAINT FK_EquipmentFeatures_Equipment FOREIGN KEY (EquipmentId) REFERENCES dbo.Equipment(EquipmentId) ON DELETE CASCADE,
    CONSTRAINT FK_EquipmentFeatures_Features FOREIGN KEY (FeatureId) REFERENCES dbo.Features(FeatureId) ON DELETE CASCADE
);
GO

CREATE INDEX IX_Equipment_Category_Rentable ON dbo.Equipment(CategoryId, IsRentable) INCLUDE (EquipmentName, DailyRate, AvailableUnits);
CREATE INDEX IX_Equipment_Name ON dbo.Equipment(EquipmentName);
CREATE INDEX IX_EquipmentPhotos_Equipment ON dbo.EquipmentPhotos(EquipmentId);
CREATE UNIQUE INDEX UX_EquipmentPhotos_Main ON dbo.EquipmentPhotos(EquipmentId) WHERE IsMain = 1;
CREATE INDEX IX_EquipmentFeatures_Feature ON dbo.EquipmentFeatures(FeatureId);
GO

ALTER TABLE dbo.EquipmentCategories ADD
    CreatedAt datetime2 NOT NULL CONSTRAINT DF_EquipmentCategories_CreatedAt DEFAULT SYSUTCDATETIME(),
    CreatedBy nvarchar(100) NULL,
    UpdatedAt datetime2 NULL,
    UpdatedBy nvarchar(100) NULL,
    IsDeleted bit NOT NULL CONSTRAINT DF_EquipmentCategories_IsDeleted DEFAULT 0;
GO
ALTER TABLE dbo.Equipment ADD
    CreatedAt datetime2 NOT NULL CONSTRAINT DF_CatalogEquipment_CreatedAt DEFAULT SYSUTCDATETIME(),
    CreatedBy nvarchar(100) NULL,
    UpdatedAt datetime2 NULL,
    UpdatedBy nvarchar(100) NULL,
    IsDeleted bit NOT NULL CONSTRAINT DF_CatalogEquipment_IsDeleted DEFAULT 0,
    RowVersion rowversion;
GO

CREATE INDEX IX_EquipmentCategories_Active ON dbo.EquipmentCategories(CategoryName) WHERE IsDeleted = 0;
CREATE INDEX IX_CatalogEquipment_Active ON dbo.Equipment(CategoryId) WHERE IsDeleted = 0;
GO

CREATE OR ALTER VIEW dbo.vw_EquipmentCatalog AS
SELECT e.EquipmentId, e.EquipmentName, c.CategoryName, e.DailyRate, e.DepositAmount,
       e.AvailableUnits, e.IsRentable, s.Brand, s.Model, s.WeightGrams, s.Material,
       s.SeasonRating, s.CapacityValue, s.CapacityUnit, m.InspectionIntervalDays,
       m.RequiresSpecialist, p.ImageUrl AS MainImageUrl
FROM dbo.Equipment AS e
JOIN dbo.EquipmentCategories AS c ON c.CategoryId = e.CategoryId
LEFT JOIN dbo.EquipmentSpecs AS s ON s.EquipmentId = e.EquipmentId
LEFT JOIN dbo.MaintenancePolicies AS m ON m.EquipmentId = e.EquipmentId
LEFT JOIN dbo.EquipmentPhotos AS p ON p.EquipmentId = e.EquipmentId AND p.IsMain = 1
WHERE e.IsDeleted = 0 AND c.IsDeleted = 0;
GO
