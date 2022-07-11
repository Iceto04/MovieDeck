namespace MovieDeck.Data.Models
{
    using System;
    using System.ComponentModel.DataAnnotations.Schema;

    using MovieDeck.Data.Common.Models;
    using MovieDeck.Data.Models.Enums;

    public class BasePersonModel<T> : BaseDeletableModel<T>
    {
        public string FullName { get; set; }

        [Column(TypeName = "Date")]
        public DateTime? BirthDate { get; set; }

        public string Biography { get; set; }

        public string PhotoPath { get; set; }
    }
}
