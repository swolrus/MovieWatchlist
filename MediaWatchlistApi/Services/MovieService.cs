using Microsoft.EntityFrameworkCore;

using MediaWatchlistApi.Models;
using MediaWatchlistApi.Dtos;
using MediaWatchlistApi.Application;

namespace MediaWatchlistApi.Services;
public class MovieService : IMovieService
{
	private readonly AppDbContext _context;

	public MovieService(AppDbContext context)
	{
		_context = context;
	}

	public async Task<IEnumerable<Movie>> GetAsync(
		CancellationToken cancellationToken
	) {
		var movies = await _context.Movies.ToListAsync(cancellationToken);
		return movies;
	}

	public async Task<Movie> CreateAsync(
		MovieCreateDto dto,
		CancellationToken cancellationToken)
	{
		var movie = new Movie
		{
			Name        = dto.Name,
			Watched     = dto.Watched,
			Rating      = dto.Rating,
			Comment     = dto.Comment,
			Created		= DateTime.UtcNow,
			LastUpdated = DateTime.UtcNow
		};

		await _context.AddAsync(movie, cancellationToken);
		await _context.SaveChangesAsync(cancellationToken);

		return movie;
	}

	public async Task<Movie?> GetByIdAsync(int id, CancellationToken cancellationToken)
	{
		var movie = await _context.Movies.FindAsync(id, cancellationToken);

		if (movie == null)
		{
			return null;
		}
		return movie;
	}
	public async Task<Movie?> DeleteAsync(int id, CancellationToken cancellationToken)
	{
		var movie = await _context.Movies.FindAsync(id, cancellationToken);

		if (movie == null)
		{
			return null;
		}
		_context.Movies.Remove(movie);
		await _context.SaveChangesAsync();
		
		return movie;
	}
	public async Task<Movie?> UpdateAsync(int id, MovieUpdateDto dto, CancellationToken cancellationToken)
	{
		var movie = await _context.Movies.FindAsync(id, cancellationToken);

		if (movie == null)
		{
			return null;
		}
		movie.Id		= id;
		movie.Name     	= dto.Name ?? movie.Name;
		movie.Watched   = dto.Watched ?? movie.Watched;
		movie.Rating    = dto.Rating ?? movie.Rating;
		movie.Comment   = dto.Comment ?? movie.Comment;
		movie.LastUpdated = DateTime.UtcNow;

		await _context.SaveChangesAsync(cancellationToken);

		return movie;
	}
	
}