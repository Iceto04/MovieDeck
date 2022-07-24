namespace MovieDeck.Web.ViewModels.Movies
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;

    using Microsoft.AspNetCore.Mvc;
    using MovieDeck.Web.ModelBinders;

    public class BaseMovieInputModel : MovieInputModelDropdownItems
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

        [DisplayName("Directors")]
        public int[] DirectorsIds { get; set; }

        [DisplayName("Production Companies")]
        public int[] CompaniesIds { get; set; }

        public List<MovieActorInputModel> Actors { get; set; }
    }
}
