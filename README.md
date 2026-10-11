# TrailRent Reviews — Lab 04

MongoDB microservice for TrailRent equipment reviews (`TrailRent_CommunityDB.equipment_reviews` from Lab 01), built as Clean Architecture + CQRS with MediatR. The work is intentionally isolated in branch `lab-04`.

```text
WebApi → Application → Domain
   ↓            ↑
Infrastructure ─┘
```

`Domain` has no project dependency (only `MongoDB.Bson` for `ObjectId`). `Application` knows only Domain; the MongoDB driver exists only in `Infrastructure`.

## Run

```bash
docker run --name trailrent-mongo -p 27017:27017 -d mongo:8
dotnet run --project TrailRent.Reviews.WebApi
```

Swagger: `http://localhost:5104/swagger`. On Development startup the app creates the compound, text and unique indexes, then idempotently seeds `reviews`.

## Demonstration

```bash
curl http://localhost:5104/api/reviews
curl -X POST http://localhost:5104/api/reviews -H 'content-type: application/json' -d '{"productId":101,"productName":"Alpine tent","authorId":"u-new","authorName":"Тарас","rating":5,"text":"Чудово"}'
# rating: 10 is stopped by ValidationBehavior before its handler runs (400)
```

`GET /api/reviews/{id}` returns `ETag`; send the same value with `If-Match` when updating. A stale version or duplicate author/product pair returns 409 ProblemDetails.

For the requested evidence, use Compass to capture the embedded `author`/`comments` document and run `db.equipment_reviews.getIndexes()` in mongosh. Detailed checklist: [docs/lab-04/checklist.md](docs/lab-04/checklist.md).
