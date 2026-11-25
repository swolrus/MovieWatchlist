using Microsoft.AspNetCore.Http.HttpResults;

using MediaWatchlistApi.Dtos;
using MediaWatchlistApi.Models;
using MediaWatchlistApi.Services;
using MediaWatchlistApi.Application;


namespace MediaWatchlistApi.Endpoints;
public static class MoviesEndpoints
{
	extension (WebApplication app)
	{
		public WebApplication MapMoviesEndpoints()
		{
			var group = app.MapGroup("/api/movies");

			group.MapGet("/", GetAsync);
			group.MapPost("/", CreateAsync)
				.AddEndpointFilter<ValidationFilter<MovieCreateDto>>()
				.Produces<ApiResponseError>(StatusCodes.Status400BadRequest);
			group.MapGet("{id:int}", GetByIdAsync);
			group.MapDelete("{id:int}", DeleteAsync);
			group.MapPut("{id:int}", UpdateAsync)
				.AddEndpointFilter<ValidationFilter<MovieUpdateDto>>()
				.Produces<ApiResponseError>(StatusCodes.Status400BadRequest);

			return app;
		}
	}

	// GET "/" returns list of movies or empty array if none
	public static async Task<Ok<ApiResponseSuccess<IEnumerable<Movie>>>>
	GetAsync(
		IMovieService movieService)
	{
		var movies = await movieService.GetAsync(CancellationToken.None);

		return TypedResults.Ok(ApiResponseSuccess<IEnumerable<Movie>>.Ok(movies));
	}

	// GET "/{id}" returns ok containing movie at id else notfound
	public static async Task<Results<Ok<ApiResponseSuccess<Movie>>, NotFound<ApiResponseError>>> 
    GetByIdAsync(
            int id,
            IMovieService movieService,
			HttpContext context)
    {
		var movie = await movieService.GetByIdAsync(
			id, CancellationToken.None);

		if (movie == null)
		{
			var error = new ApiError("MOVIE_NOT_FOUND", $"Movie with ID {id} does not exist.");
			return TypedResults.NotFound(
            	ApiResponseError.Fail("Movie not found.", [error], context.TraceIdentifier)
			);
		}
		return TypedResults.Ok(ApiResponseSuccess<Movie>.Ok(movie, $"Movie with ID {id} found."));
	}

	// POST "/" creates movie from body
	public static async Task<Created<ApiResponseSuccess<Movie>>> 
	CreateAsync(MovieCreateDto dto,
        IMovieService movieService)
	{
		var created = await movieService.CreateAsync(dto, CancellationToken.None);
		return TypedResults.Created(string.Empty, ApiResponseSuccess<Movie>.Ok(created));
	}

	// DELETE "/{id}" Deletes movie at id and returns NoContent on success
    public static async Task<Results<Ok<ApiResponseSuccess<Movie>>, NotFound<ApiResponseError>>>
    DeleteAsync(int id,
        IMovieService movieService,
		HttpContext context)
	{
		var deleted = await movieService.DeleteAsync(id, CancellationToken.None);
		if (deleted == null)
        {
			var error = new ApiError("MOVIE_NOT_FOUND", $"Movie with ID {id} not found.");
			return TypedResults.NotFound(
            	ApiResponseError.Fail("Movie not found.", [error], context.TraceIdentifier)
			);
        }

		return TypedResults.Ok(ApiResponseSuccess<Movie>.Ok(deleted, $"Movie with ID {id} deleted."));
	}

	// PUT "/{id}" returns ok containing movie at id else notfound
	public static async Task<Results<Ok<ApiResponseSuccess<Movie>>, NotFound<ApiResponseError>>>
    UpdateAsync(
		int id,
		MovieUpdateDto dto,
		IMovieService movieService,
		HttpContext context)
    {
		var updated = await movieService.UpdateAsync(
			id, dto, CancellationToken.None);

		if (updated == null)
		{
			var error = new ApiError("MOVIE_NOT_FOUND", $"Movie with ID {id} not found.");
			return TypedResults.NotFound(
            	ApiResponseError.Fail("Movie not found.", [error], context.TraceIdentifier)
			);
		}
		
		return TypedResults.Ok(ApiResponseSuccess<Movie>.Ok(updated, $"Movie with ID {id} updated."));
	}
}