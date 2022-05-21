namespace MovieDeck.Data.Models
{
    public class MovieGenre
    {
        public int Id { get; set; }

        public int MovieId { get; set; }

        public virtual Movie Movie { get; set; }

        public int GenreId { get; set; }

        public virtual Genre Genre { get; set; }
    }
}
