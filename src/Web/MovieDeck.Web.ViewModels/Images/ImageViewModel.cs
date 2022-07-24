namespace MovieDeck.Web.ViewModels.Images
{
    using AutoMapper;

    using MovieDeck.Common;
    using MovieDeck.Data.Models;
    using MovieDeck.Services.Mapping;

    public class ImageViewModel : IMapFrom<Image>, IHaveCustomMappings
    {
        public string PhotoUrl { get; set; }

        public void CreateMappings(IProfileExpression configuration)
        {
            configuration.CreateMap<Image, ImageViewModel>()
                .ForMember(x => x.PhotoUrl, opt =>
                    opt.MapFrom(x => x.RemoteImageUrl != null ?
                        string.Format(GlobalConstants.RemoteImagesUrl, x.RemoteImageUrl) :
                       $"/images/recipes/{x.Id}.{x.Extension}"));
        }
    }
}
