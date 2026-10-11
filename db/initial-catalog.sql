IF OBJECT_ID(N'[__EFMigrationsHistory]') IS NULL
BEGIN
    CREATE TABLE [__EFMigrationsHistory] (
        [MigrationId] nvarchar(150) NOT NULL,
        [ProductVersion] nvarchar(32) NOT NULL,
        CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY ([MigrationId])
    );
END;
GO

BEGIN TRANSACTION;
IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261011210247_InitialCatalogCreate'
)
BEGIN
    CREATE TABLE [EquipmentCategories] (
        [Id] int NOT NULL IDENTITY,
        [Name] nvarchar(100) NOT NULL,
        [Description] nvarchar(500) NOT NULL,
        CONSTRAINT [PK_EquipmentCategories] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261011210247_InitialCatalogCreate'
)
BEGIN
    CREATE TABLE [Features] (
        [Id] int NOT NULL IDENTITY,
        [Name] nvarchar(80) NOT NULL,
        CONSTRAINT [PK_Features] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261011210247_InitialCatalogCreate'
)
BEGIN
    CREATE TABLE [Equipment] (
        [Id] int NOT NULL IDENTITY,
        [Sku] nvarchar(32) NOT NULL,
        [Name] nvarchar(160) NOT NULL,
        [DailyRate] decimal(18,2) NOT NULL,
        [AvailableUnits] int NOT NULL,
        [IsRentable] bit NOT NULL,
        [CategoryId] int NOT NULL,
        CONSTRAINT [PK_Equipment] PRIMARY KEY ([Id]),
        CONSTRAINT [CK_Equipment_DailyRate] CHECK ([DailyRate] > 0),
        CONSTRAINT [FK_Equipment_EquipmentCategories_CategoryId] FOREIGN KEY ([CategoryId]) REFERENCES [EquipmentCategories] ([Id]) ON DELETE NO ACTION
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261011210247_InitialCatalogCreate'
)
BEGIN
    CREATE TABLE [EquipmentFeatures] (
        [EquipmentId] int NOT NULL,
        [FeatureId] int NOT NULL,
        [AddedAt] datetime2 NOT NULL,
        CONSTRAINT [PK_EquipmentFeatures] PRIMARY KEY ([EquipmentId], [FeatureId]),
        CONSTRAINT [FK_EquipmentFeatures_Equipment_EquipmentId] FOREIGN KEY ([EquipmentId]) REFERENCES [Equipment] ([Id]) ON DELETE CASCADE,
        CONSTRAINT [FK_EquipmentFeatures_Features_FeatureId] FOREIGN KEY ([FeatureId]) REFERENCES [Features] ([Id]) ON DELETE CASCADE
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261011210247_InitialCatalogCreate'
)
BEGIN
    CREATE TABLE [EquipmentPhotos] (
        [Id] int NOT NULL IDENTITY,
        [EquipmentId] int NOT NULL,
        [Url] nvarchar(500) NOT NULL,
        [SortOrder] int NOT NULL,
        CONSTRAINT [PK_EquipmentPhotos] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_EquipmentPhotos_Equipment_EquipmentId] FOREIGN KEY ([EquipmentId]) REFERENCES [Equipment] ([Id]) ON DELETE CASCADE
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261011210247_InitialCatalogCreate'
)
BEGIN
    CREATE TABLE [EquipmentSpecs] (
        [EquipmentId] int NOT NULL,
        [WeightKg] decimal(18,2) NOT NULL,
        [Dimensions] nvarchar(80) NOT NULL,
        [Material] nvarchar(80) NOT NULL,
        CONSTRAINT [PK_EquipmentSpecs] PRIMARY KEY ([EquipmentId]),
        CONSTRAINT [CK_EquipmentSpecs_Weight] CHECK ([WeightKg] > 0),
        CONSTRAINT [FK_EquipmentSpecs_Equipment_EquipmentId] FOREIGN KEY ([EquipmentId]) REFERENCES [Equipment] ([Id]) ON DELETE CASCADE
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261011210247_InitialCatalogCreate'
)
BEGIN
    CREATE TABLE [MaintenancePolicies] (
        [EquipmentId] int NOT NULL,
        [ServiceIntervalDays] int NOT NULL,
        [Instructions] nvarchar(600) NOT NULL,
        CONSTRAINT [PK_MaintenancePolicies] PRIMARY KEY ([EquipmentId]),
        CONSTRAINT [CK_Maintenance_Interval] CHECK ([ServiceIntervalDays] > 0),
        CONSTRAINT [FK_MaintenancePolicies_Equipment_EquipmentId] FOREIGN KEY ([EquipmentId]) REFERENCES [Equipment] ([Id]) ON DELETE CASCADE
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261011210247_InitialCatalogCreate'
)
BEGIN
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'Description', N'Name') AND [object_id] = OBJECT_ID(N'[EquipmentCategories]'))
        SET IDENTITY_INSERT [EquipmentCategories] ON;
    EXEC(N'INSERT INTO [EquipmentCategories] ([Id], [Description], [Name])
    VALUES (1, N''Tents and camp furniture'', N''Camping''),
    (2, N''Trail equipment'', N''Hiking'')');
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'Description', N'Name') AND [object_id] = OBJECT_ID(N'[EquipmentCategories]'))
        SET IDENTITY_INSERT [EquipmentCategories] OFF;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261011210247_InitialCatalogCreate'
)
BEGIN
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'Name') AND [object_id] = OBJECT_ID(N'[Features]'))
        SET IDENTITY_INSERT [Features] ON;
    EXEC(N'INSERT INTO [Features] ([Id], [Name])
    VALUES (1, N''Waterproof''),
    (2, N''Ultralight''),
    (3, N''Four-season'')');
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'Name') AND [object_id] = OBJECT_ID(N'[Features]'))
        SET IDENTITY_INSERT [Features] OFF;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261011210247_InitialCatalogCreate'
)
BEGIN
    CREATE INDEX [IX_Equipment_CategoryId] ON [Equipment] ([CategoryId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261011210247_InitialCatalogCreate'
)
BEGIN
    CREATE UNIQUE INDEX [IX_Equipment_Sku] ON [Equipment] ([Sku]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261011210247_InitialCatalogCreate'
)
BEGIN
    CREATE UNIQUE INDEX [IX_EquipmentCategories_Name] ON [EquipmentCategories] ([Name]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261011210247_InitialCatalogCreate'
)
BEGIN
    CREATE INDEX [IX_EquipmentFeatures_FeatureId] ON [EquipmentFeatures] ([FeatureId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261011210247_InitialCatalogCreate'
)
BEGIN
    CREATE UNIQUE INDEX [IX_EquipmentPhotos_EquipmentId_SortOrder] ON [EquipmentPhotos] ([EquipmentId], [SortOrder]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261011210247_InitialCatalogCreate'
)
BEGIN
    CREATE UNIQUE INDEX [IX_Features_Name] ON [Features] ([Name]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261011210247_InitialCatalogCreate'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20261011210247_InitialCatalogCreate', N'9.0.0');
END;

COMMIT;
GO

