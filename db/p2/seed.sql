USE TrailRent_CatalogDB;
GO

SET IDENTITY_INSERT dbo.EquipmentCategories ON;
INSERT INTO dbo.EquipmentCategories (CategoryId, CategoryName, Description) VALUES
 (1, N'Намети', N'Трисезонні й гірські намети для ночівлі на маршруті'),
 (2, N'Рюкзаки', N'Туристичні рюкзаки різного об''єму'),
 (3, N'Спальні системи', N'Спальники та килимки для холодних ночівель'),
 (4, N'Кухня в поході', N'Пальники, посуд і системи приготування їжі'),
 (5, N'Трекінгове спорядження', N'Палиці, ліхтарі та інші аксесуари');
SET IDENTITY_INSERT dbo.EquipmentCategories OFF;
GO

SET IDENTITY_INSERT dbo.Equipment ON;
INSERT INTO dbo.Equipment (EquipmentId, CategoryId, EquipmentName, DailyRate, DepositAmount, AvailableUnits, IsRentable) VALUES
 (1, 1, N'Намет Alpine Duo 2P', 420, 5000, 8, 1),
 (2, 2, N'Рюкзак Ridge 65L', 250, 2500, 12, 1),
 (3, 3, N'Спальник FrostLine -5°C', 210, 1800, 16, 1),
 (4, 4, N'Пальник FireTrail Compact', 160, 1200, 10, 1),
 (5, 5, N'Трекінгові палиці Summit Pro', 140, 1500, 20, 1);
SET IDENTITY_INSERT dbo.Equipment OFF;
GO

INSERT INTO dbo.EquipmentSpecs (EquipmentId, Brand, Model, WeightGrams, Material, SeasonRating, CapacityValue, CapacityUnit) VALUES
 (1, N'NorthPeak', N'Alpine Duo', 2100, N'Ripstop nylon', N'3 сезони', 2, N'persons'),
 (2, N'TrailForge', N'Ridge 65', 1750, N'Cordura', NULL, 65, N'litres'),
 (3, N'NorthPeak', N'FrostLine', 1350, N'Пухове волокно', N'до -5°C', NULL, NULL),
 (4, N'CampCraft', N'FireTrail', 180, N'Алюміній і сталь', NULL, NULL, NULL),
 (5, N'SummitWorks', N'Summit Pro', 520, N'Карбон', NULL, 135, N'centimetres');
GO

INSERT INTO dbo.MaintenancePolicies (EquipmentId, InspectionIntervalDays, CleaningInstruction, RequiresSpecialist) VALUES
 (1, 30, N'Просушити тент і внутрішній намет перед складанням.', 0),
 (2, 45, N'Очистити фурнітуру та перевірити шви.', 0),
 (3, 20, N'Провітрити спальник; не зберігати стисненим.', 0),
 (4, 14, N'Перевірити пальник на витік газу та очистити форсунку.', 1),
 (5, 60, N'Перевірити замки секцій і наконечники.', 0);
GO

INSERT INTO dbo.EquipmentPhotos (EquipmentId, ImageUrl, IsMain) VALUES
 (1, N'https://images.example.com/alpine-duo-main.jpg', 1),
 (1, N'https://images.example.com/alpine-duo-inside.jpg', 0),
 (2, N'https://images.example.com/ridge-65-main.jpg', 1),
 (3, N'https://images.example.com/frostline-main.jpg', 1),
 (4, N'https://images.example.com/firetrail-main.jpg', 1),
 (5, N'https://images.example.com/summit-pro-main.jpg', 1);
GO

SET IDENTITY_INSERT dbo.Features ON;
INSERT INTO dbo.Features (FeatureId, FeatureName) VALUES
 (1, N'Водостійке'), (2, N'Для двох осіб'), (3, N'Надлегке'), (4, N'Зимове'), (5, N'Компактне');
SET IDENTITY_INSERT dbo.Features OFF;
GO

INSERT INTO dbo.EquipmentFeatures (EquipmentId, FeatureId) VALUES
 (1, 1), (1, 2), (2, 1), (3, 4), (3, 3), (4, 5), (5, 3), (5, 5);
GO
