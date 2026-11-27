
using Microsoft.EntityFrameworkCore;
using MediaWatchlistApi.Services;
using FluentValidation;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using MediaWatchlistApi.Models;

namespace MediaWatchlistApi.Application;
public static class ConfigureServices
{
	extension (IServiceCollection services)
	{
		public IServiceCollection AddAppDbContext(IConfiguration configuration)
		{
            services.AddDbContext<AppDbContext>(options =>
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

		public IServiceCollection AddHasher(WebApplicationBuilder builder)
		{
			var secretKey = builder.Configuration["Jwt:SecretKey"];
			if (string.IsNullOrWhiteSpace(secretKey))
			{
				throw new InvalidOperationException(
					"Missing configuration 'Jwt:SecretKey'. Set it with .NET User Secrets or the Jwt__SecretKey environment variable.");
			}

			var secretKeyBytes = Encoding.UTF8.GetBytes(secretKey);
			if (secretKeyBytes.Length < 32)
			{
				throw new InvalidOperationException(
					"Configuration 'Jwt:SecretKey' must be at least 32 UTF-8 bytes for HS256.");
			}

			services.AddScoped<IPasswordHasher<User>, PasswordHasher<User>>();
			services.AddSingleton<TokenProvider>();
			services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
				.AddJwtBearer(options =>
				{
					options.MapInboundClaims = false;
					options.TokenValidationParameters = new TokenValidationParameters
					{
						IssuerSigningKey = new SymmetricSecurityKey(secretKeyBytes),
						ValidIssuer = builder.Configuration["Jwt:Issuer"],
						ValidAudience = builder.Configuration["Jwt:Audience"],
						ClockSkew = TimeSpan.Zero
					};
				});
			return services;
		}
	}
}
