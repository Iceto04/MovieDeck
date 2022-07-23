namespace MovieDeck.Web.ViewModels.Movies
{
    using System.Collections.Generic;
    using System.ComponentModel;

    using Microsoft.AspNetCore.Mvc.Rendering;

    public class MovieActorInputModel
    {
        [DisplayName("Actor")]
        public int ActorId { get; set; }

        public string CharacterName { get; set; }
    }
}
