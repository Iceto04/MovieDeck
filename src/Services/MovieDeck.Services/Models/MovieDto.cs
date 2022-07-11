namespace MovieDeck.Services.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations.Schema;

    using Microsoft.EntityFrameworkCore.Metadata.Internal;
    using MovieDeck.Data.Models;

    public class MovieDto
    {
        public MovieDto()
        {
            this.Actors = new List<ActorDto>();
            this.Directors = new List<PersonDto>();
            this.Genres = new List<string>();
            this.Companies = new List<string>();
            this.Images = new List<string>();
        }

        public string Title { get; set; }

        public string Plot { get; set; }

        public DateTime? ReleaseDate { get; set; }

        public TimeSpan Runtime { get; set; }

        public string ImdbRating { get; set; }

        public string PosterPath { get; set; }

        public string BackdropPath { get; set; }

        public int OriginalId { get; set; }

        //// public string AddedByUserId { get; set; }

        public List<ActorDto> Actors { get; set; }

        public List<PersonDto> Directors { get; set; }

        public List<string> Genres { get; set; }

        public List<string> Companies { get; set; }

        public List<string> Images { get; set; }
    }
}
