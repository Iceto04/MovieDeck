namespace MovieDeck.Data.Models
{
    using System.Collections.Generic;

    using MovieDeck.Data.Common.Models;

    public class Watchlist : BaseDeletableModel<int>
    {
        public Watchlist()
        {
            this.Movies = new HashSet<MovieWatchlist>();
        }

        public string Name { get; set; }

        public string OwnerId { get; set; }

        public virtual ApplicationUser Owner { get; set; }

        public virtual ICollection<MovieWatchlist> Movies { get; set; }
    }
}
