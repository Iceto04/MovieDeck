namespace MovieDeck.Data.Models
{
    using System.Collections.Generic;

    using MovieDeck.Data.Common.Models;

    public class Genre : BaseDeletableModel<int>
    {
        public Genre()
        {
            this.Movies = new HashSet<MovieGenre>();
        }

        public string Name { get; set; }

        public virtual ICollection<MovieGenre> Movies { get; set; }
    }
}
