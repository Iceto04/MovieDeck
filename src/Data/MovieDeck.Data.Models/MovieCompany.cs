namespace MovieDeck.Data.Models
{
    public class MovieCompany
    {
        public int Id { get; set; }

        public int MovieId { get; set; }

        public virtual Movie Movie { get; set; }

        public int CompanyId { get; set; }

        public virtual ProductionCompany Company { get; set; }
    }
}
