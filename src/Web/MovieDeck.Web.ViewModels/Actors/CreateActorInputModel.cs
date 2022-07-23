namespace MovieDeck.Web.ViewModels.Actors
{
    using System;
    using System.ComponentModel.DataAnnotations;

    using Microsoft.AspNetCore.Http;

    public class CreateActorInputModel
    {
        [Required]
        [MinLength(2)]
        [MaxLength(250)]
        public string FullName { get; set; }

        [DataType(DataType.Date)]
        public DateTime? BirthDate { get; set; }

        [Required]
        [MinLength(100)]
        public string Biography { get; set; }

        public IFormFile Photo { get; set; }
    }
}
