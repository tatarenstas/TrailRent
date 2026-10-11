# Lab 04 checklist

- Four projects implement the dependency rule; `Domain` is free of external persistence APIs.
- `Review` includes embedded `Author`, `ProductReference`, and private `_comments`; BSON class maps reside only in Infrastructure.
- MediatR separates create/update/delete/comment commands from read queries. Validation, structured logging and performance monitoring are open pipeline behaviors.
- Mongo repository uses version filtering for optimistic concurrency. Query implementation applies `$match`/text search plus sorting/paging in MongoDB.
- Startup creates `product_recent`, `review_text`, and `unique_author_product` indexes, then runs idempotent bulk seed data.
- Global ProblemDetails maps FluentValidation, domain, Mongo duplicate-key and connection errors to the required HTTP statuses.

Screenshots to make after local launch: Swagger, an `equipment_reviews` document in Compass, `db.equipment_reviews.getIndexes()`, and a 400 invalid-rating request with the handler absent from logs.
