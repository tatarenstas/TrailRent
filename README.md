# TrailRent · Lab 02

Web API для сервісу прокату туристичного спорядження. Рішення продовжує контекст бронювань із Lab 01 і реалізує вертикаль **DAL → BLL → API** на ADO.NET, Dapper та .NET Aspire.

## Структура

```text
TrailRent.Domain   моделі, статуси й доменні винятки
TrailRent.Dal      ADO.NET/Dapper-репозиторії та Unit of Work
TrailRent.Bll      DTO, AutoMapper, сервіси й бізнес-правила
TrailRent.Api      контролери, ProblemDetails, Swagger, ініціалізація БД
TrailRent.AppHost  Aspire: SQL Server + API однією командою
```

Залежності: `Api → Bll → Dal → Domain`; контролери спілкуються лише із сервісами.

## Запуск

Потрібні .NET 9 SDK і Docker Desktop.

```bash
dotnet restore
dotnet run --project TrailRent.AppHost
```

Aspire створює SQL Server та передає API рядок підключення `ConnectionStrings__BookingDb`. Під час старту API застосовує ідемпотентний скрипт `TrailRent.Api/Database/initialize.sql`, який створює таблиці, процедуру `dbo.usp_ConfirmRental` і тестові дані. Swagger доступний за URL API в Aspire Dashboard (`/swagger`). Для локального SQL Server задай `ConnectionStrings__BookingDb` або зміни `appsettings.json`.

## Основні запити

```bash
# Клієнти: CRUD
curl http://localhost:5000/api/customers
curl -X POST http://localhost:5000/api/customers \
  -H 'Content-Type: application/json' \
  -d '{"firstName":"O\u0027Brien","lastName":"Test","email":"obrien@example.com"}'

# Бронювання і транзакційне підтвердження
curl -X POST http://localhost:5000/api/rentals \
  -H 'Content-Type: application/json' \
  -d '{"customerId":1,"startDate":"2026-10-20","endDate":"2026-10-23","pickupLocation":"Чернівці","items":[{"equipmentId":1,"quantity":1}]}'
curl -X POST http://localhost:5000/api/rentals/1/confirm
```

`POST /api/rentals/{id}/confirm` виконується в одній транзакції: процедура змінює статус бронювання, а Dapper-репозиторій зменшує доступну кількість кожної позиції. Якщо другої операції не можна виконати, Unit of Work відкочує процедуру. Помилки валідації, відсутній ресурс і конфлікт повертаються як `400`, `404`, `409` у форматі `ProblemDetails`.

Деталі архітектури та перевірочні сценарії: [docs/lab-02.md](docs/lab-02.md).
