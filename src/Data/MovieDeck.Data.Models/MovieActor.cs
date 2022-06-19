namespace MovieDeck.Data.Models
{
    public class MovieActor
    {
        public int Id { get; set; }

        public int MovieId { get; set; }

        public virtual Movie Movie { get; set; }

        public int ActorId { get; set; }

        public virtual Actor Actor { get; set; }

        public string CharacterName { get; set; }
    }
}
