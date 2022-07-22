namespace MovieDeck.Web.ViewModels.Movies
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;

    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Rendering;

    using MovieDeck.Data.Models;
    using MovieDeck.Web.ModelBinders;
    using MovieDeck.Web.ViewModels.Actors;
    using MovieDeck.Web.ViewModels.Companies;
    using MovieDeck.Web.ViewModels.Directors;

    public class CreateMovieInputModel
    {
        [Required]
        public string Title { get; set; }

        [Required]
        [MinLength(100)]
        public string Plot { get; set; }

        [DataType(DataType.Date)]
        public DateTime? ReleaseDate { get; set; }

        [ModelBinder(typeof(AddMovieModelBinder))]
        public TimeSpan Runtime { get; set; }

        [DisplayName("Genres")]
        public int[] GenresIds { get; set; }

        public IFormFile Poster { get; set; }

        public IFormFile Backdrop { get; set; }

        public IEnumerable<IFormFile> Images { get; set; }

        [DisplayName("Actors")]
        public int[] ActorsIds { get; set; }

        [DisplayName("Directors")]
        public int[] DirectorsIds { get; set; }

        [DisplayName("Production Companies")]
        public int[] CompaniesIds { get; set; }

        public IEnumerable<SelectListItem> ActorsItems { get; set; }

        public IEnumerable<SelectListItem> DirectorsItems { get; set; }

        public IEnumerable<SelectListItem> CompaniesItems { get; set; }

        public IEnumerable<SelectListItem> GenresItems { get; set; }
    }
}
