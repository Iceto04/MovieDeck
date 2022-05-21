namespace MovieDeck.Data.Models
{
    using System;

    using MovieDeck.Data.Common.Models;
    using MovieDeck.Data.Models.Enums;

    public class BasePersonModel<T> : BaseDeletableModel<T>
    {
        public string FirstName { get; set; }

        public string LastName { get; set; }

        public DateTime BirthDate { get; set; }

        public Gender Gender { get; set; }

        public string Biography { get; set; }
    }
}
