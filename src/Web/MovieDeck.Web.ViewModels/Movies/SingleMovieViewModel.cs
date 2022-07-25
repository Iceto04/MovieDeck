namespace MovieDeck.Web.ViewModels.Movies
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using AutoMapper;
    using MovieDeck.Common;
    using MovieDeck.Data.Models;
    using MovieDeck.Services.Mapping;
    using MovieDeck.Web.ViewModels.Actors;
    using MovieDeck.Web.ViewModels.Directors;
    using MovieDeck.Web.ViewModels.Genres;
    using MovieDeck.Web.ViewModels.Images;

    public class SingleMovieViewModel : IMapFrom<Movie>, IHaveCustomMappings
    {
        public int Id { get; set; }

        public string Title { get; set; }

        public string Plot { get; set; }

        public DateTime? ReleaseDate { get; set; }

        public TimeSpan Runtime { get; set; }

        public double AverageRating { get; set; }

        public long RatingsCount { get; set; }

        public string PosterUrl { get; set; }

        public string TrailerUrl { get; set; }

        public byte UserRating { get; set; }

        public IEnumerable<DirectorViewModel> Directors { get; set; }

        public IEnumerable<ActorViewModel> Actors { get; set; }

        public IEnumerable<GenreViewModel> Genres { get; set; }

        public IEnumerable<ImageViewModel> Images { get; set; }

        public void CreateMappings(IProfileExpression configuration)
        {
            configuration.CreateMap<Movie, SingleMovieViewModel>()
                .ForMember(x => x.PosterUrl, opt =>
                    opt.MapFrom(x => x.PosterPath.Contains("-") ?
                        "/images/recipes/" + x.PosterPath :
                        string.Format(GlobalConstants.RemoteImagesUrl, x.PosterPath)))
                /// If the movie has no API ratings and no one has rated it,
                /// the view should display 0 and the database cannot be divided by zero,
                /// so you should have this check
                .ForMember(x => x.AverageRating, opt =>
                    opt.MapFrom(x => x.RatingsCount + x.Ratings.Count == 0
                        ? 0
                        : ((x.ImdbRating * x.RatingsCount)
                            + x.Ratings.Sum(x => x.Value)) / (x.RatingsCount + x.Ratings.Count)))
                .ForMember(x => x.RatingsCount, opt =>
                    opt.MapFrom(x => x.RatingsCount + x.Ratings.Count))
                .ForMember(x => x.TrailerUrl, opt =>
                    opt.MapFrom(x => x.TrailerKey != null ?
                        string.Format(GlobalConstants.YoutubeEmbedVideoUrl, x.TrailerKey) : "#"));
        }
    }
}
