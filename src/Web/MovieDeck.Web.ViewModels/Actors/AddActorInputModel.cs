﻿namespace MovieDeck.Web.ViewModels.Actors
{
    using System;
    using System.ComponentModel.DataAnnotations;

    using MovieDeck.Data.Models.Enums;

    public class AddActorInputModel
    {
        [Required]
        [MinLength(2)]
        [MaxLength(250)]
        public string FirstName { get; set; }

        [Required]
        [MinLength(2)]
        [MaxLength(250)]
        public string LastName { get; set; }

        public DateTime BirthDate { get; set; }

        public Gender Gender { get; set; }

        [Required]
        [MinLength(100)]
        public string Biography { get; set; }
    }
}
