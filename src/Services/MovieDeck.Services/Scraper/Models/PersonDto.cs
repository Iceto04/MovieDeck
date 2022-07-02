namespace MovieDeck.Services.Scraper.Models
{
    using System;

    public class PersonDto
    {
        public string FullName { get; set; }

        public DateTime? BirthDate { get; set; }

        public string Biography { get; set; }

        public string PhotoUrl { get; set; }
    }
}
