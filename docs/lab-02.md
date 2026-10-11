# Lab 02: архітектурні рішення

## Відповідність вимогам

| Вимога | Реалізація |
|---|---|
| Чистий ADO.NET | `CustomerRepository`: `SqlCommand`, типізовані параметри, `SqlDataReader`, `await using`. |
| Dapper | `EquipmentRepository` та `RentalRepository`; останній використовує multi-mapping для `Rental` + `RentalItem` і викликає `dbo.usp_ConfirmRental`. |
| Generic Repository | `BaseDapperRepository<T>` містить спільні читання та soft delete; конкретні Dapper-репозиторії додають вузькі операції. |
| Unit of Work | Один `SqlConnection` і поточний `SqlTransaction` передаються в усі репозиторії; lifetime — scoped HTTP-запит. |
| Транзакція | `RentalService.ConfirmAsync`: процедура переводить `Rentals.Status` у `AwaitingPickup`, після чого зменшується `Equipment.AvailableUnits`. Невдача будь-якого кроку викликає rollback. |
| DTO й мапінг | Контракти в `TrailRent.Bll/Contracts`, централізований `TrailRentProfile`. |
| API | Attribute routing, async + `CancellationToken`, `CreatedAtAction`, Swagger. |
| Помилки | `DomainExceptionHandler` перетворює domain exceptions на RFC ProblemDetails: 400, 404, 409. |
| Aspire | `TrailRent.AppHost` піднімає SQL Server і API; connection-string ресурс має назву `BookingDb`. |

## Ізоляція та конкурентність

Підтвердження бронювання використовує `ReadCommitted`: це баланс між читанням незакомічених даних і кількістю блокувань. В оновленні доступних одиниць є умова `AvailableUnits >= @Quantity`, тому паралельний запит не зробить залишок від'ємним. Для повного optimistic-сценарію таблиця `Rentals` також має `rowversion`; він буде застосований для PUT у наступній ітерації.

## Перевірка rollback

1. Створи бронювання з `equipmentId: 1`, `quantity: 99` — створення поверне 409, бо BLL бачить недостатній залишок.
2. Для демонстрації відкату створи звичайну чернетку, у SQL Server перед викликом confirm зменш доступну кількість до нуля.
3. `POST /api/rentals/{id}/confirm` поверне 409. Процедура вже встигла змінити статус, але `UnitOfWork.RollbackAsync` поверне його до `Draft`.

## Контракт API

| Метод | Шлях | Відповідь |
|---|---|---|
| GET | `/api/customers`, `/api/customers/{id}` | 200 або 404 |
| POST | `/api/customers` | 201 + Location; 400/409 |
| PUT / DELETE | `/api/customers/{id}` | 204 або 404 |
| GET | `/api/rentals`, `/api/rentals/{id}` | 200 або 404 |
| POST | `/api/rentals` | 201 + Location; 400/404/409 |
| POST | `/api/rentals/{id}/confirm` | 204; 404/409 у ProblemDetails |
