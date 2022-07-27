namespace MovieDeck.Web.ViewModels.Actors
{
    using System;
using System.Collections.Generic;

    using AutoMapper;

    using MovieDeck.Common;
    using MovieDeck.Data.Models;
    using MovieDeck.Services.Mapping;
    using MovieDeck.Web.ViewModels.Images;

    public class ActorMovieViewModel : IMapFrom<MovieActor>, IHaveCustomMappings
    {
        public int MovieId { get; set; }

        public string MovieTitle { get; set; }

        public DateTime MovieReleaseDate { get; set; }

        public string MoviePosterPath { get; set; }

        public string MoviePosterUrl { get; set; }

        public string CharacterName { get; set; }

        public void CreateMappings(IProfileExpression configuration)
        {
            configuration.CreateMap<MovieActor, ActorMovieViewModel>()
                .ForMember(x => x.MoviePosterUrl, opt =>
                    opt.MapFrom(x => x.Movie.PosterPath.Contains("-") ?
                        $"/images/{GlobalConstants.MoviesImagesFolder}/" + x.Movie.PosterPath :
                        string.Format(GlobalConstants.RemoteImagesUrl, x.Movie.PosterPath)));
        }
    }
}