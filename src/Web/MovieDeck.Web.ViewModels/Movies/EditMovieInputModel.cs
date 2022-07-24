namespace MovieDeck.Web.ViewModels.Movies
{
    using System.Collections.Generic;
    using System.Linq;

    using AutoMapper;

    using MovieDeck.Data.Models;
    using MovieDeck.Services.Mapping;

    public class EditMovieInputModel : BaseMovieInputModel, IMapFrom<Movie>, IHaveCustomMappings
    {
        public int Id { get; set; }

        //public List<MovieActorInputModel> ActorsList { get; set; }

        public void CreateMappings(IProfileExpression configuration)
        {
            configuration.CreateMap<Movie, EditMovieInputModel>()
                .ForMember(x => x.GenresIds, opt =>
                    opt.MapFrom(x => x.Genres.Select(x => x.GenreId)))
                .ForMember(x => x.DirectorsIds, opt =>
                    opt.MapFrom(x => x.Directors.Select(x => x.DirectorId)))
                .ForMember(x => x.CompaniesIds, opt =>
                    opt.MapFrom(x => x.Companies.Select(x => x.CompanyId)));
        }
    }
}
