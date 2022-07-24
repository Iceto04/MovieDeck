namespace MovieDeck.Web.ViewModels.Actors
{
    using AutoMapper;
    using MovieDeck.Common;
    using MovieDeck.Data.Models;
    using MovieDeck.Services.Mapping;

    public class ActorViewModel : IMapFrom<MovieActor>, IHaveCustomMappings
    {
        public string ActorFullName { get; set; }

        public string ActorPhotoPath { get; set; }

        public string PhotoUrl { get; set; }

        public string CharacterName { get; set; }

        public void CreateMappings(IProfileExpression configuration)
        {
            configuration.CreateMap<MovieActor, ActorViewModel>()
                .ForMember(x => x.PhotoUrl, opt =>
                    opt.MapFrom(x => x.Actor.PhotoPath.Contains("-") ?
                        "/images/recipes/" + x.Actor.PhotoPath :
                        string.Format(GlobalConstants.RemoteImagesUrl, x.Actor.PhotoPath)));
        }
    }
}
