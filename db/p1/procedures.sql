USE TrailRent_BookingDB;
GO

CREATE OR ALTER PROCEDURE dbo.usp_CreateCustomer
    @FirstName nvarchar(50),
    @LastName nvarchar(50),
    @Email nvarchar(100),
    @PhoneNumber nvarchar(20) = NULL,
    @EmergencyContact nvarchar(100) = NULL,
    @NewCustomerId bigint OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    IF EXISTS (SELECT 1 FROM dbo.Customers WHERE Email = @Email)
        THROW 51001, 'A customer with this email already exists.', 1;

    INSERT INTO dbo.Customers (FirstName, LastName, Email, PhoneNumber, EmergencyContact)
    VALUES (@FirstName, @LastName, @Email, @PhoneNumber, @EmergencyContact);
    SET @NewCustomerId = SCOPE_IDENTITY();
END;
GO

CREATE OR ALTER PROCEDURE dbo.usp_CreateMembershipPlan
    @PlanCode nvarchar(30),
    @PlanName nvarchar(100),
    @DiscountPercent decimal(5,2),
    @StartsOn date,
    @EndsOn date,
    @NewMembershipPlanId bigint OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    IF @DiscountPercent < 0 OR @DiscountPercent > 100
        THROW 51002, 'Discount percent must be between 0 and 100.', 1;
    IF @EndsOn <= @StartsOn
        THROW 51003, 'Plan end date must be later than start date.', 1;
    IF EXISTS (SELECT 1 FROM dbo.MembershipPlans WHERE PlanCode = @PlanCode)
        THROW 51004, 'Membership plan code already exists.', 1;

    INSERT INTO dbo.MembershipPlans (PlanCode, PlanName, DiscountPercent, StartsOn, EndsOn)
    VALUES (@PlanCode, @PlanName, @DiscountPercent, @StartsOn, @EndsOn);
    SET @NewMembershipPlanId = SCOPE_IDENTITY();
END;
GO

CREATE OR ALTER PROCEDURE dbo.usp_ConfirmRental
    @RentalId bigint
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;
    BEGIN TRANSACTION;

    DECLARE @Status nvarchar(30);
    SELECT @Status = RentalStatus FROM dbo.Rentals WITH (UPDLOCK)
    WHERE RentalId = @RentalId AND IsDeleted = 0;

    IF @Status IS NULL THROW 51010, 'Rental not found.', 1;
    IF @Status <> N'Draft' THROW 51011, 'Only a draft rental can be confirmed.', 1;
    IF NOT EXISTS (SELECT 1 FROM dbo.RentalItems WHERE RentalId = @RentalId)
        THROW 51012, 'A rental must contain at least one item.', 1;

    UPDATE dbo.Rentals SET RentalStatus = N'AwaitingPickup', UpdatedAt = SYSUTCDATETIME()
    WHERE RentalId = @RentalId;
    COMMIT TRANSACTION;
END;
GO

CREATE OR ALTER PROCEDURE dbo.usp_StartRental
    @RentalId bigint,
    @PaymentMethod nvarchar(50),
    @Amount decimal(18,2)
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;
    BEGIN TRANSACTION;

    DECLARE @Status nvarchar(30), @Total decimal(18,2);
    SELECT @Status = RentalStatus, @Total = TotalAmount FROM dbo.Rentals WITH (UPDLOCK)
    WHERE RentalId = @RentalId AND IsDeleted = 0;

    IF @Status IS NULL THROW 51020, 'Rental not found.', 1;
    IF @Status <> N'AwaitingPickup' THROW 51021, 'Rental must be confirmed before pickup.', 1;
    IF @Amount < @Total THROW 51022, 'Payment amount is less than the rental total.', 1;

    IF EXISTS (SELECT 1 FROM dbo.PaymentRecords WHERE RentalId = @RentalId)
        UPDATE dbo.PaymentRecords SET PaymentMethod = @PaymentMethod, Amount = @Amount,
            PaymentStatus = N'Completed', TransactionDate = SYSUTCDATETIME() WHERE RentalId = @RentalId;
    ELSE
        INSERT INTO dbo.PaymentRecords (RentalId, PaymentMethod, Amount, PaymentStatus)
        VALUES (@RentalId, @PaymentMethod, @Amount, N'Completed');

    UPDATE dbo.Rentals SET RentalStatus = N'Active', UpdatedAt = SYSUTCDATETIME() WHERE RentalId = @RentalId;
    COMMIT TRANSACTION;
END;
GO

CREATE OR ALTER PROCEDURE dbo.usp_ReturnRental
    @RentalId bigint
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;
    BEGIN TRANSACTION;

    DECLARE @Status nvarchar(30);
    SELECT @Status = RentalStatus FROM dbo.Rentals WITH (UPDLOCK)
    WHERE RentalId = @RentalId AND IsDeleted = 0;
    IF @Status IS NULL THROW 51030, 'Rental not found.', 1;
    IF @Status <> N'Active' THROW 51031, 'Only active rental can be returned.', 1;

    UPDATE dbo.Rentals SET RentalStatus = N'Returned', UpdatedAt = SYSUTCDATETIME() WHERE RentalId = @RentalId;
    COMMIT TRANSACTION;
END;
GO

CREATE OR ALTER PROCEDURE dbo.usp_CancelRental
    @RentalId bigint
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;
    BEGIN TRANSACTION;

    DECLARE @Status nvarchar(30);
    SELECT @Status = RentalStatus FROM dbo.Rentals WITH (UPDLOCK)
    WHERE RentalId = @RentalId AND IsDeleted = 0;
    IF @Status IS NULL THROW 51040, 'Rental not found.', 1;
    IF @Status NOT IN (N'Draft', N'AwaitingPickup')
        THROW 51041, 'Rental can be cancelled only before pickup.', 1;

    UPDATE dbo.Rentals SET RentalStatus = N'Cancelled', UpdatedAt = SYSUTCDATETIME() WHERE RentalId = @RentalId;
    COMMIT TRANSACTION;
END;
GO
