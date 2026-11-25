
using Microsoft.EntityFrameworkCore;
using MediaWatchlistApi.Services;
using FluentValidation;

namespace MediaWatchlistApi.Application;
public static class ConfigureServices
{
	extension (IServiceCollection services)
	{
		public IServiceCollection AddAppDbContext(IConfiguration configuration)
		{
            services.AddDbContext<ApplicationDbContext>(options =>
    			options.UseNpgsql(configuration.GetConnectionString("DefaultConnection")));
			return services;
		}

		public IServiceCollection AddServices()
		{
			services.AddScoped<IMovieService, MovieService>();
			return services;
		}

		public IServiceCollection AddValidators()
		{
			services.AddValidatorsFromAssemblyContaining<Program>();
			return services;
		}
	}
}
