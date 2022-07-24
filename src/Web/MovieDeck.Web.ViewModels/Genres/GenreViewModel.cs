namespace MovieDeck.Web.ViewModels.Genres
{
    using MovieDeck.Data.Models;
    using MovieDeck.Services.Mapping;

    public class GenreViewModel : IMapFrom<MovieGenre>
    {
        public string GenreName { get; set; }
    }
}
