namespace MovieDeck.Web.ViewModels.Movies
{
    using System;
    using System.Collections.Generic;

    using MovieDeck.Data.Models;
    using MovieDeck.Services.Mapping;
    using MovieDeck.Web.ViewModels.Actors;
    using MovieDeck.Web.ViewModels.Directors;
    using MovieDeck.Web.ViewModels.Genres;
    using MovieDeck.Web.ViewModels.Images;

    public class SingleMovieViewModel : IMapFrom<Movie>
{
        public string Title { get; set; }

        public string Plot { get; set; }

        public DateTime? ReleaseDate { get; set; }

        public TimeSpan Runtime { get; set; }

        public string ImdbRating { get; set; }

        public string PosterUrl { get; set; }

        public IEnumerable<DirectorViewModel> Directors { get; set; }

        public IEnumerable<ActorViewModel> Actors { get; set; }

        public IEnumerable<GenreViewModel> Genres { get; set; }

        public IEnumerable<ImageViewModel> Images { get; set; }
    }
}
