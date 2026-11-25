# MediaWatchlist

MediaWatchlist is a movie watchlist application with an Angular frontend, an
ASP.NET Core minimal API, and PostgreSQL storage.

## Start the application

The easiest way to run the full stack is with Docker Compose. Docker and Docker
Compose are required.

1. Set `POSTGRES_PASSWORD` in your shell or in a root `.env` file. For example:

   ```dotenv
   POSTGRES_PASSWORD=replace-with-a-strong-password
   ```

   Keep the value consistent with the password used to initialize an existing
   PostgreSQL data directory. The `.env` file is ignored by Git.

2. From the repository root, build and start the services:

   ```sh
   docker compose up --build
   ```

   To run in the background, use `docker compose up --build -d`. Stop the
   services with `docker compose down`. The PostgreSQL files persist in
   `postgres_data/`.

### Service URLs

| Service | URL | Notes |
| --- | --- | --- |
| Angular frontend | [http://localhost:4200](http://localhost:4200) | Development server with live reload |
| Backend API | [http://localhost:8080](http://localhost:8080) | ASP.NET Core API |
| Scalar API reference | [http://localhost:8080/scalar/v1](http://localhost:8080/scalar/v1) | Available while the API runs in Development |
| OpenAPI schema | [http://localhost:8080/openapi/v1.json](http://localhost:8080/openapi/v1.json) | Generated API document; available in Development |
| PostgreSQL | `localhost:5432` | Database `app_db`, user `app_user` |

The frontend's `/api` requests are proxied to the backend by the Angular
development server. The backend applies its EF Core migrations when it starts.

## Backend API

All movie endpoints use the `/api/movies` route. JSON responses use camel-case
property names.

| Method | Path | Description | Success |
| --- | --- | --- | --- |
| `GET` | `/api/movies/` | List movies | `200 OK` |
| `GET` | `/api/movies/{id}` | Get one movie | `200 OK`, or `404 Not Found` |
| `POST` | `/api/movies/` | Create a movie | `201 Created`, or `400 Bad Request` |
| `PUT` | `/api/movies/{id}` | Update supplied movie fields | `200 OK`, `404 Not Found`, or `400 Bad Request` |
| `DELETE` | `/api/movies/{id}` | Delete a movie | `200 OK`, or `404 Not Found` |

Example create request:

```sh
curl -i http://localhost:8080/api/movies/ \
  -H 'Content-Type: application/json' \
  -d '{
    "name": "Arrival",
    "watched": true,
    "rating": 4.5,
    "comment": "Thought-provoking"
  }'
```

`name` must be 2–100 characters, `comment` must be 2–500 characters, and
`rating` must be non-empty and between 0 and 5. `watched` is a boolean. Updates
accept any subset of `name`, `watched`, `rating`, and `comment`.

Successful responses are wrapped in an envelope:

```json
{
  "success": true,
  "message": null,
  "data": {
    "id": 1,
    "name": "Arrival",
    "watched": true,
    "rating": 4.5,
    "comment": "Thought-provoking",
    "created": "2026-11-25T10:00:00Z",
    "lastUpdated": "2026-11-25T10:00:00Z"
  }
}
```

List responses put an array in `data`. Errors use `success: false` and include
a message; validation and not-found errors can also include `errors` and
`traceId`.

For the complete request and response schemas, use the
[Scalar API reference](http://localhost:8080/scalar/v1) or fetch the
[OpenAPI JSON document](http://localhost:8080/openapi/v1.json) while the API is
running in Development.

## Frontend development

The Compose setup runs Angular in a container at `http://localhost:4200`, with
source files mounted for live reload.

To run Angular directly on the host instead, install Node.js 24 or later and
Corepack, then from `Frontend/` run:

```sh
yarn install
yarn start
```

The host-run Angular dev server uses `Frontend/proxy.local.conf.json`, which
targets `http://localhost:8080`. The Compose frontend uses
`Frontend/proxy.conf.json`, which targets the API service at `http://server:8080`.

## Project layout

- `MediaWatchlistApi/` — ASP.NET Core API, EF Core context, and migrations.
- `Frontend/` — Angular application and development proxy.
- `compose.yaml` — frontend, API, and PostgreSQL service definitions.