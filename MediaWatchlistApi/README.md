

For local development, store a random signing key in .NET User Secrets:

```bash
dotnet user-secrets set "Jwt:SecretKey" "$(openssl rand -base64 32)" --project MediaWatchlistApi/MediaWatchlistApi.csproj
dotnet run --project MediaWatchlistApi/MediaWatchlistApi.csproj
```

For Docker Compose, set `JWT_SECRET` to a random value of at least 32 bytes before starting the API:

```bash
export JWT_SECRET="$(openssl rand -base64 32)"
docker compose up -d --force-recreate server
```