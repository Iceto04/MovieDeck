namespace MovieDeck.Web.ViewModels.Movies
{
    using System.ComponentModel;

    using MovieDeck.Data.Models;
    using MovieDeck.Services.Mapping;

    public class MovieActorInputModel : IMapFrom<MovieActor>
    {
        [DisplayName("Actor")]
        public int ActorId { get; set; }

        public string CharacterName { get; set; }
    }
}
