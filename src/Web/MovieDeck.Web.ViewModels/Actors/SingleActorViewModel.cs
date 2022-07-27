namespace MovieDeck.Web.ViewModels.Actors
{
    using System;
    using System.Collections.Generic;

    using AutoMapper;

    using MovieDeck.Common;
    using MovieDeck.Data.Models;
    using MovieDeck.Services.Mapping;
    using MovieDeck.Web.ViewModels.Images;

    public class SingleActorViewModel : IMapFrom<Actor>, IHaveCustomMappings
    {
        public int Id { get; set; }

        public string FullName { get; set; }

        public DateTime? BirthDate { get; set; }

        public string Biography { get; set; }

        public string BiographyShort =>
            this.Biography != null ?
            this.Biography.Length > 300
                ? this.Biography.Substring(0, 300) + "..."
                : this.Biography
            : null;

        public string PhotoUrl { get; set; }

        public int OriginalId { get; set; }

        public IEnumerable<ActorMovieViewModel> Movies { get; set; }

        public IEnumerable<ActorImageViewModel> Images { get; set; }

        public void CreateMappings(IProfileExpression configuration)
        {
            configuration.CreateMap<Actor, SingleActorViewModel>()
                .ForMember(x => x.PhotoUrl, opt =>
                    opt.MapFrom(x => x.PhotoPath == null ?
                        GlobalConstants.BlankProfilePicture :
                            x.PhotoPath.Contains("-") ?
                            $"/images/{GlobalConstants.ActorsImagesFolder}/" + x.PhotoPath :
                            string.Format(GlobalConstants.RemoteImagesUrl, x.PhotoPath)));
        }
    }
}
