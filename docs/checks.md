# Перевірочні запити

```sql
-- Booking: читання, пошук і пагінація
USE TrailRent_BookingDB;
SELECT * FROM dbo.vw_RentalSummary;
SELECT CustomerId, FirstName, LastName FROM dbo.Customers WHERE Email LIKE N'%@gmail.com';
SELECT RentalId, StartDate, RentalStatus, TotalAmount FROM dbo.Rentals
ORDER BY StartDate DESC OFFSET 0 ROWS FETCH NEXT 2 ROWS ONLY;

-- CRUD та перевірка обмеження
INSERT INTO dbo.Customers (FirstName, LastName, Email) VALUES (N'Test', N'User', N'test@trailrent.ua');
UPDATE dbo.Customers SET PhoneNumber = N'+380000000000', UpdatedAt = SYSUTCDATETIME() WHERE Email = N'test@trailrent.ua';
UPDATE dbo.Customers SET IsDeleted = 1 WHERE Email = N'test@trailrent.ua';
UPDATE dbo.Rentals SET RentalStatus = N'Completed' WHERE RentalId = 1; -- має завершитися помилкою CHECK
```

```sql
-- Catalog: пошук, M:N та обмеження
USE TrailRent_CatalogDB;
SELECT * FROM dbo.vw_EquipmentCatalog;
SELECT EquipmentId, EquipmentName, DailyRate FROM dbo.Equipment WHERE EquipmentName LIKE N'%намет%';
SELECT e.EquipmentName FROM dbo.Equipment e
JOIN dbo.EquipmentFeatures ef ON ef.EquipmentId = e.EquipmentId
JOIN dbo.Features f ON f.FeatureId = ef.FeatureId WHERE f.FeatureName = N'Для двох осіб';
INSERT INTO dbo.Equipment (CategoryId, EquipmentName, DailyRate, AvailableUnits) VALUES (1, N'Некоректне', -1, 1); -- CHECK
```

```javascript
// MongoDB: фільтр, пагінація, агрегація та оновлення
db.equipment_reviews.find({ equipmentId: 1 });
db.equipment_reviews.find().sort({ createdAt: -1 }).skip(0).limit(2);
db.equipment_reviews.aggregate([{ $group: { _id: "$equipmentName", averageRating: { $avg: "$rating" } } }]);
db.equipment_reviews.updateOne({ _id: ObjectId("66f0d1000000000000000002") }, { $set: { rating: 5 } });
```
