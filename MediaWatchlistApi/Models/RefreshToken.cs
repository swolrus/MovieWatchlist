namespace MediaWatchlistApi.Models;
public sealed class RefreshToken
{
    public Guid Id { get; set; }
    public string Token { get; set; } = string.Empty;
    public DateTime ExpiresAtUtc { get; set; }
    public Guid UserId { get; set; }
    public User User { get; set; } = null!;
}