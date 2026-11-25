### Building and running your application

When you're ready, start your application by running:
`docker compose up --build`.

Set `POSTGRES_PASSWORD` before starting Compose. You can export it in your shell
or put it in a root `.env` file (which is ignored by Git):

```sh
POSTGRES_PASSWORD=choose-a-strong-password
```

The API will be available at http://localhost:8080. Opening the root URL redirects
to the movie collection at http://localhost:8080/api/movies/.
The Compose service runs in Development mode, with the Scalar API reference at
http://localhost:8080/scalar and the OpenAPI document at
http://localhost:8080/openapi/v1.json.
PostgreSQL is available on localhost:5432 for host-run EF Core commands. Inside
Compose, the API connects to the database using the `db` service name, and waits
for PostgreSQL to become healthy.

### Deploying your application to the cloud

First, build your image, e.g.: `docker build -t myapp .`.
If your cloud uses a different CPU architecture than your development
machine (e.g., you are on a Mac M1 and your cloud provider is amd64),
you'll want to build the image for that platform, e.g.:
`docker build --platform=linux/amd64 -t myapp .`.

Then, push it to your registry, e.g. `docker push myregistry.com/myapp`.

Consult Docker's [getting started](https://docs.docker.com/go/get-started-sharing/)
docs for more detail on building and pushing.

### References
* [Docker's .NET guide](https://docs.docker.com/language/dotnet/)
* The [dotnet-docker](https://github.com/dotnet/dotnet-docker/tree/main/samples)
  repository has many relevant samples and docs.