namespace MediaWatchlistApi.Models
{
    public class Movie
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public bool Watched { get; set; }
        public double Rating { get; set; }
        public string Comment { get; set; } = string.Empty;
        public DateTime Created { get; set; }
        public DateTime LastUpdated { get; set; }
    }
}