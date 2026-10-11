# Версіонування схеми

## EF Core для Catalog

У наступній ітерації `TrailRent_CatalogDB` описується C#-класами та Fluent API. Кожна зміна потрапляє в окрему міграцію з методами `Up` і `Down`:

```bash
dotnet ef migrations add AddEquipmentSku
dotnet ef database update
```

## SQL-скрипти для Booking

Для ADO.NET/Dapper схема `TrailRent_BookingDB` зберігається версіонованими SQL-скриптами, бо тут потрібен точний контроль процедур і транзакцій.

| Версія | Зміна |
|---|---|
| v1 | Базові таблиці клієнтів, бронювань, позицій та платежів. |
| v2 | Обмеження, індекси, каскадні залежності й `vw_RentalSummary`. |
| v3 | Аудит, м'яке видалення та `rowversion` для бронювання. |

## MongoDB

MongoDB допускає гнучку структуру, але для production-версії колекції варто захистити JSON Schema Validation. Наприклад, рейтинг має бути цілим від 1 до 5:

```javascript
db.runCommand({
  collMod: "equipment_reviews",
  validator: { $jsonSchema: {
    bsonType: "object",
    required: ["equipmentId", "rating", "author"],
    properties: { rating: { bsonType: "int", minimum: 1, maximum: 5 } }
  }}
});
```
