namespace MovieDeck.Web.ViewModels.Images
{
    using AutoMapper;

    using MovieDeck.Common;
    using MovieDeck.Data.Models;
    using MovieDeck.Services.Mapping;

    public class ActorImageViewModel : IMapFrom<Image>, IHaveCustomMappings
    {
        public string PhotoUrl { get; set; }

        public void CreateMappings(IProfileExpression configuration)
        {
            configuration.CreateMap<Image, ActorImageViewModel>()
                .ForMember(x => x.PhotoUrl, opt =>
                    opt.MapFrom(x => x.RemoteImageUrl != null ?
                        string.Format(GlobalConstants.RemoteImagesUrl, x.RemoteImageUrl) :
                       $"/images/{GlobalConstants.ActorsImagesFolder}/{x.Id}.{x.Extension}"));
        }
    }
}
