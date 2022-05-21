namespace MovieDeck.Data.Models
{
    public class MovieWatchlist
    {
        public int Id { get; set; }

        public int MovieId { get; set; }

        public virtual Movie Movie { get; set; }

        public int WatchlistId { get; set; }

        public virtual Watchlist Watchlist { get; set; }
    }
}
