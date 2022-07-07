namespace MovieDeck.Web.ViewModels.Movies
{
    using System;

    public class MovieViewModel
    {
        public string Title { get; set; }

        public string Plot { get; set; }

        public DateTime? ReleaseDate { get; set; }

        public TimeSpan Runtime { get; set; }

        public string ImdbRating { get; set; }

        public string OriginalUrl { get; set; }

        public string PosterUrl { get; set; }

        public string BannerUrl { get; set; }
    }
}
