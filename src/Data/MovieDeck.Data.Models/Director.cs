namespace MovieDeck.Data.Models
{
    using System;
    using System.Collections.Generic;

    public class Director : BasePersonModel<string>
    {
        public Director()
        {
            this.Id = Guid.NewGuid().ToString();

            this.Movies = new HashSet<MovieDirector>();
        }

        public virtual ICollection<MovieDirector> Movies { get; set; }
    }
}
