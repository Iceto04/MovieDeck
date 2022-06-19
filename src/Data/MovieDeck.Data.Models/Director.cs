namespace MovieDeck.Data.Models
{
    using System;
    using System.Collections.Generic;

    public class Director : BasePersonModel<int>
    {
        public Director()
        {
            this.Movies = new HashSet<MovieDirector>();
        }

        public virtual ICollection<MovieDirector> Movies { get; set; }
    }
}
