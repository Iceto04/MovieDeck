namespace MovieDeck.Data.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations.Schema;

    using Microsoft.EntityFrameworkCore.Metadata.Internal;

    using MovieDeck.Data.Common.Models;

    public class Movie : BaseDeletableModel<int>
    {
        public Movie()
        {
            this.Actors = new HashSet<MovieActor>();
            this.Directors = new HashSet<MovieDirector>();
            this.Genres = new HashSet<MovieGenre>();
            this.Watchlists = new HashSet<MovieWatchlist>();
            this.Images = new HashSet<Image>();
            this.Companies = new HashSet<MovieCompany>();
            this.Ratings = new HashSet<Rating>();
        }

        public string Title { get; set; }

        public string Plot { get; set; }

        [Column(TypeName = "Date")]
        public DateTime? ReleaseDate { get; set; }

        public TimeSpan Runtime { get; set; }

        public double ImdbRating { get; set; }

        public long RatingsCount { get; set; }

        public int OriginalId { get; set; }

        public string PosterPath { get; set; }

        public string BackdropPath { get; set; }

        public string AddedByUserId { get; set; }

        public virtual ApplicationUser AddedByUser { get; set; }

        public virtual ICollection<MovieActor> Actors { get; set; }

        public virtual ICollection<MovieDirector> Directors { get; set; }

        public virtual ICollection<MovieGenre> Genres { get; set; }

        public virtual ICollection<MovieWatchlist> Watchlists { get; set; }

        public virtual ICollection<Image> Images { get; set; }

        public virtual ICollection<MovieCompany> Companies { get; set; }

        public virtual ICollection<Rating> Ratings { get; set; }
    }
}
