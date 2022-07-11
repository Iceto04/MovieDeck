namespace MovieDeck.Services.Models
{
    using System;

    public class PersonDto
    {
        public string FullName { get; set; }

        public DateTime? BirthDate { get; set; }

        public string Biography { get; set; }

        public string PhotoPath { get; set; }
    }
}
