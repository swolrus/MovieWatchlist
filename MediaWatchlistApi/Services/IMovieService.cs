using MediaWatchlistApi.Dtos;
using MediaWatchlistApi.Models;

namespace MediaWatchlistApi.Services
{
	public interface IMovieService
	{
		Task<IEnumerable<Movie>> GetAsync(CancellationToken cancellationToken);
		Task<Movie> CreateAsync(MovieCreateDto movie, CancellationToken cancellationToken);
		Task<Movie?> GetByIdAsync(int id, CancellationToken cancellationToken);
		Task<Movie?> DeleteAsync(int id, CancellationToken cancellationToken);
		Task<Movie?> UpdateAsync(int id, MovieUpdateDto movie, CancellationToken cancellationToken);
	}
}