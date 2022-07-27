namespace MovieDeck.Web.ViewModels.Movies
{
    using System;

    using AutoMapper;

    using MovieDeck.Common;
    using MovieDeck.Data.Models;
    using MovieDeck.Services.Mapping;

    public class MovieViewModel : IMapFrom<Movie>, IHaveCustomMappings
    {
        public int Id { get; set; }

        public string Title { get; set; }

        public string Plot { get; set; }

        public DateTime? ReleaseDate { get; set; }

        public TimeSpan Runtime { get; set; }

        public string ImdbRating { get; set; }

        public string PosterUrl { get; set; }

        public string BackdropUrl { get; set; }

        public void CreateMappings(IProfileExpression configuration)
        {
            configuration.CreateMap<Movie, MovieViewModel>()
                .ForMember(x => x.PosterUrl, opt =>
                    opt.MapFrom(x => x.PosterPath.Contains("-") ?
                        $"/images/{GlobalConstants.MoviesImagesFolder}/" + x.PosterPath :
                        string.Format(GlobalConstants.RemoteImagesUrl, x.PosterPath)))
                .ForMember(x => x.BackdropUrl, opt =>
                    opt.MapFrom(x => x.BackdropPath.Contains("-") ?
                        $"/images/{GlobalConstants.MoviesImagesFolder}/" + x.BackdropPath :
                        string.Format(GlobalConstants.RemoteImagesUrl, x.BackdropPath)));
        }
    }
}
