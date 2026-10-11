USE TrailRent_BookingDB;
GO

SET IDENTITY_INSERT dbo.Customers ON;
INSERT INTO dbo.Customers (CustomerId, FirstName, LastName, Email, PhoneNumber, EmergencyContact)
VALUES
 (1, N'Олена', N'Коваль', N'olena.koval@example.com', N'+380501112233', N'Ігор Коваль +380671112233'),
 (2, N'Максим', N'Бойко', N'maksym.boyko@example.com', N'+380674445566', N'Олена Бойко +380934445566'),
 (3, N'Софія', N'Мельник', N'sofiia.melnyk@example.com', N'+380637778899', NULL),
 (4, N'Артем', N'Іваненко', N'artem.ivanenko@example.com', N'+380503332211', NULL),
 (5, N'Марія', N'Ткачук', N'mariia.tkachuk@example.com', N'+380935554433', N'Тарас Ткачук +380675554433');
SET IDENTITY_INSERT dbo.Customers OFF;
GO

SET IDENTITY_INSERT dbo.MembershipPlans ON;
INSERT INTO dbo.MembershipPlans (MembershipPlanId, PlanCode, PlanName, DiscountPercent, StartsOn, EndsOn)
VALUES
 (1, N'WEEKEND10', N'Вікенд-мандрівник', 10, '2026-01-01', '2026-12-31'),
 (2, N'MOUNTAIN15', N'Гірський сезон', 15, '2026-04-01', '2026-10-31'),
 (3, N'STUDENT8', N'Студентський', 8, '2026-01-01', '2026-12-31');
SET IDENTITY_INSERT dbo.MembershipPlans OFF;
GO

INSERT INTO dbo.Equipment (EquipmentId, EquipmentName, DailyRate, IsRentable) VALUES
 (1, N'Намет Alpine Duo 2P', 420, 1),
 (2, N'Рюкзак Ridge 65L', 250, 1),
 (3, N'Спальник FrostLine -5°C', 210, 1),
 (4, N'Пальник FireTrail Compact', 160, 1),
 (5, N'Трекінгові палиці Summit Pro', 140, 1);
GO

SET IDENTITY_INSERT dbo.Rentals ON;
INSERT INTO dbo.Rentals (RentalId, CustomerId, MembershipPlanId, StartDate, EndDate, RentalStatus, TotalAmount, PickupLocation)
VALUES
 (1, 1, 1, '2026-10-17', '2026-10-20', N'Draft', 2484, N'Чернівці, вул. Героїв Майдану, 1'),
 (2, 2, NULL, '2026-10-10', '2026-10-13', N'AwaitingPickup', 1740, N'Львів, вул. Городоцька, 15'),
 (3, 3, 2, '2026-10-03', '2026-10-06', N'Active', 1785, N'Івано-Франківськ, вул. Незалежності, 8'),
 (4, 4, NULL, '2026-09-20', '2026-09-23', N'Returned', 1380, N'Чернівці, вул. Героїв Майдану, 1'),
 (5, 5, 3, '2026-09-14', '2026-09-16', N'Cancelled', 846.40, N'Львів, вул. Городоцька, 15');
SET IDENTITY_INSERT dbo.Rentals OFF;
GO

INSERT INTO dbo.RentalItems (RentalId, EquipmentId, EquipmentName, DailyRate, Quantity) VALUES
 (1, 1, N'Намет Alpine Duo 2P', 420, 1), (1, 2, N'Рюкзак Ridge 65L', 250, 2),
 (2, 3, N'Спальник FrostLine -5°C', 210, 2), (2, 4, N'Пальник FireTrail Compact', 160, 1),
 (3, 1, N'Намет Alpine Duo 2P', 420, 1), (3, 5, N'Трекінгові палиці Summit Pro', 140, 2),
 (4, 2, N'Рюкзак Ridge 65L', 250, 1), (4, 3, N'Спальник FrostLine -5°C', 210, 1),
 (5, 4, N'Пальник FireTrail Compact', 160, 2), (5, 5, N'Трекінгові палиці Summit Pro', 140, 1);
GO

INSERT INTO dbo.PaymentRecords (RentalId, PaymentMethod, Amount, PaymentStatus) VALUES
 (1, N'Card', 2484, N'Pending'), (2, N'ApplePay', 1740, N'Pending'),
 (3, N'Card', 1785, N'Completed'), (4, N'Cash', 1380, N'Completed'), (5, N'Card', 846.40, N'Failed');
GO
