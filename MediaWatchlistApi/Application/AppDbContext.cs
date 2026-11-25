using Microsoft.EntityFrameworkCore;
using MediaWatchlistApi.Models;

namespace MediaWatchlistApi.Application;
public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options) { }
    public DbSet<Movie> Movies => Set<Movie>();
}