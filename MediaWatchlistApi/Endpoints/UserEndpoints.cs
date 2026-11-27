using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.JsonWebTokens;

using MediaWatchlistApi.Dtos;
using MediaWatchlistApi.Models;
using MediaWatchlistApi.Application;


namespace MediaWatchlistApi.Endpoints;
public static class UserEndpoints
{
    extension (WebApplication app)
	{
		public WebApplication MapUserEndpoints()
		{
			var group = app.MapGroup("/api/user");

			group.MapPost("/register", RegisterAsync);
			group.MapPost("/login", LoginAsync);
			group.MapPost("/refresh", RefreshAsync);
			group.MapGet("/identify", IdentifyAsync)
                .RequireAuthorization();

			return app;
		}
	}
    public static async Task<Results<Conflict<string>, Created<object>>> 
    RegisterAsync(
        UserCreateRequestDto dto,
        AppDbContext db,
        IPasswordHasher<User> hasher)
    {
        if (await db.Users.AnyAsync(u => u.Email == dto.Email))
            return TypedResults.Conflict("Email already registered.");

        var user = new User
        {
            Id = Guid.NewGuid(),
            Email = dto.Email
        };
        user.PasswordHash = hasher.HashPassword(user, dto.Password);

        db.Users.Add(user);
        await db.SaveChangesAsync();

        return TypedResults.Created<object>($"/users/{user.Id}", new { user.Id, user.Email });
    }

    // Login Endpoint
    public static async Task<Results<Ok<object>, UnauthorizedHttpResult>> 
    LoginAsync(
        UserCreateRequestDto request,
        AppDbContext db,
        IPasswordHasher<User> hasher,
        TokenProvider tokenProvider)
    {
        var user = await db.Users.SingleOrDefaultAsync(u => u.Email == request.Email);
        if (user is null)
            return TypedResults.Unauthorized();

        var verificationResult = hasher.VerifyHashedPassword(user, user.PasswordHash, request.Password);
        if (verificationResult == PasswordVerificationResult.Failed)
            return TypedResults.Unauthorized();

        var accessToken = tokenProvider.CreateAccessToken(user);
        var refreshToken = new RefreshToken
        {
            Id = Guid.NewGuid(),
            Token = tokenProvider.GenerateRefreshToken(),
            ExpiresAtUtc = DateTime.UtcNow.AddDays(7),
            UserId = user.Id
        };

        db.RefreshTokens.Add(refreshToken);
        await db.SaveChangesAsync();

        return TypedResults.Ok<object>(new
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken.Token
        });
    }

    // Refresh Token Rotation Endpoint
    public static async Task<Results<Ok<object>, UnauthorizedHttpResult>>
    RefreshAsync(
        UserRefreshRequestDto request,
        AppDbContext db,
        TokenProvider tokenProvider)
    {
        var existingToken = await db.RefreshTokens
            .Include(r => r.User)
            .SingleOrDefaultAsync(r => r.Token == request.RefreshToken);

        if (existingToken is null || existingToken.ExpiresAtUtc < DateTime.UtcNow)
            return TypedResults.Unauthorized();

        // Rotate: remove old token and issue a new pair
        db.RefreshTokens.Remove(existingToken);

        var newRefreshToken = new RefreshToken
        {
            Id = Guid.NewGuid(),
            Token = tokenProvider.GenerateRefreshToken(),
            ExpiresAtUtc = DateTime.UtcNow.AddDays(7),
            UserId = existingToken.UserId
        };

        db.RefreshTokens.Add(newRefreshToken);
        await db.SaveChangesAsync();

        var newAccessToken = tokenProvider.CreateAccessToken(existingToken.User);

        return TypedResults.Ok<object>(new
        {
            AccessToken = newAccessToken,
            RefreshToken = newRefreshToken.Token
        });
    }

    // Protected Endpoint using EF Core
    public static async Task<Results<Ok<object>, UnauthorizedHttpResult, NotFound>>
    IdentifyAsync(
        ClaimsPrincipal principal, 
        AppDbContext db)
    {
        if (!Guid.TryParse(principal.FindFirstValue(JwtRegisteredClaimNames.Sub), out var userId))
            return TypedResults.Unauthorized();

        var user = await db.Users
            .Where(u => u.Id == userId)
            .Select(u => new { u.Id, u.Email, u.Role })
            .SingleOrDefaultAsync();

        return user is not null ? TypedResults.Ok<object>(user) : TypedResults.NotFound();
    }
}