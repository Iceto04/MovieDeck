namespace MovieDeck.Web.ViewModels.Movies
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;

    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;

    using MovieDeck.Web.ModelBinders;

    public class CreateMovieInputModel : BaseMovieInputModel
    {
        [Required]
        public IFormFile Poster { get; set; }

        [Required]
        public IFormFile Backdrop { get; set; }

        [Required]
        public IEnumerable<IFormFile> Images { get; set; }
    }
}
