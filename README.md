# TrailRent Catalog — Lab 03

EF Core Code First microservice for the TrailRent equipment catalogue. It is an independent implementation of the Project #2 catalogue from Lab 01 and intentionally lives only in branch `lab-03`.

## Structure

| Project | Responsibility |
| --- | --- |
| `TrailRent.Catalog.Domain` | aggregate entities, invariants and domain exceptions |
| `TrailRent.Catalog.Dal` | `DbContext`, Fluent API configurations, repositories, UoW, specifications and migrations |
| `TrailRent.Catalog.Bll` | DTOs, AutoMapper profile, validators and services |
| `TrailRent.Catalog.Api` | controllers, Swagger, ProblemDetails, Serilog and startup seed |

## Run locally

Start SQL Server (Docker Desktop):

```bash
docker run --name trailrent-sql -e ACCEPT_EULA=Y -e MSSQL_SA_PASSWORD='TrailRent!2026' -p 1433:1433 -d mcr.microsoft.com/mssql/server:2022-latest
dotnet run --project TrailRent.Catalog.Api
```

The development startup executes `MigrateAsync()` and the idempotent code seed. Swagger opens at `http://localhost:5103/swagger`.

To apply migrations manually:

```bash
dotnet tool install --global dotnet-ef
dotnet ef database update --project TrailRent.Catalog.Dal --startup-project TrailRent.Catalog.Api
```

The generated initial SQL script is [db/initial-catalog.sql](db/initial-catalog.sql).

## API examples

```bash
# Query-level filter, sort and paging
curl 'http://localhost:5103/api/equipment?search=tent&rentable=true&sort=rate_desc&page=1&pageSize=10'

# Eager Include/ThenInclude endpoint
curl http://localhost:5103/api/equipment/1

# Explicit loading endpoint
curl http://localhost:5103/api/categories/1

# M:N query through EquipmentFeature
curl 'http://localhost:5103/api/categories/feature/Waterproof?since=2026-01-01'

# FluentValidation + DataAnnotations: returns ValidationProblemDetails (400)
curl -X POST http://localhost:5103/api/equipment -H 'content-type: application/json' -d '{"sku":"bad sku","name":"x","dailyRate":0,"availableUnits":-1,"categoryId":0}'
```

Posting the same `sku` twice produces a `409 ProblemDetails`; SQL commands and the request correlation ID are written by Serilog.
