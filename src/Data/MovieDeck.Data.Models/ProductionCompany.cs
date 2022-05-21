namespace MovieDeck.Data.Models
{
    using System.Collections.Generic;

    using MovieDeck.Data.Common.Models;

    public class ProductionCompany : BaseDeletableModel<int>
    {
        public ProductionCompany()
        {
            this.Movies = new HashSet<MovieCompany>();
        }

        public string Name { get; set; }

        public virtual ICollection<MovieCompany> Movies { get; set; }
    }
}
