namespace MovieDeck.Web.ViewModels.Movies
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;

    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Rendering;

    using MovieDeck.Data.Models;
    using MovieDeck.Web.ModelBinders;
    using MovieDeck.Web.ViewModels.Actors;
    using MovieDeck.Web.ViewModels.Companies;
    using MovieDeck.Web.ViewModels.Directors;

    public class AddMovieInputModel
    {
        [Required]
        public string Title { get; set; }

        [Required]
        [MinLength(100)]
        public string Plot { get; set; }

        [DataType(DataType.Date)]
        public DateTime ReleaseDate { get; set; }

        [ModelBinder(typeof(AddMovieModelBinder))]
        public TimeSpan Runtime { get; set; }

        [DisplayName("Genres")]
        public int[] GenresIds { get; set; }

        //// public IEnumerable<AddActorInputModel> Actors { get; set; }

        //// public IEnumerable<AddDirectorInputModel> Directors { get; set; }

        //// public IEnumerable<AddCompanyInputModel> Companies { get; set; }

        public IEnumerable<SelectListItem> GenresItems { get; set; }
    }
}
