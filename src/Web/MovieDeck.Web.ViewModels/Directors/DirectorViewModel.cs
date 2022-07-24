namespace MovieDeck.Web.ViewModels.Directors
{
    using AutoMapper;

    using MovieDeck.Common;
    using MovieDeck.Data.Models;
    using MovieDeck.Services.Mapping;

    public class DirectorViewModel : IMapFrom<MovieDirector>, IHaveCustomMappings
    {
        public string DirectorFullName { get; set; }

        public string DirectorPhotoPath { get; set; }

        public string PhotoUrl { get; set; }

        public void CreateMappings(IProfileExpression configuration)
        {
            configuration.CreateMap<MovieDirector, DirectorViewModel>()
                .ForMember(x => x.PhotoUrl, opt =>
                    opt.MapFrom(x => x.Director.PhotoPath.Contains("-") ?
                        "/images/recipes/" + x.Director.PhotoPath :
                        string.Format(GlobalConstants.RemoteImagesUrl, x.Director.PhotoPath)));
        }
    }
}
