namespace MovieDeck.Data.Models
{
    using System;
    using System.Collections.Generic;

    using MovieDeck.Data.Common.Models;
    using MovieDeck.Data.Models.Enums;

    public class Actor : BasePersonModel<int>
    {
        public Actor()
        {
            this.Movies = new HashSet<MovieActor>();
        }

        public virtual ICollection<MovieActor> Movies { get; set; }
    }
}
