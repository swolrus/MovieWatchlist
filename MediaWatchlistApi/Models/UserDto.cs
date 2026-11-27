namespace MediaWatchlistApi.Dtos
{
    public record UserCreateRequestDto (
        string Email,
        string Password
    );
    public record UserRefreshRequestDto (
        string RefreshToken
    );
}