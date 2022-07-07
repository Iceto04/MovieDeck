namespace MovieDeck.Web.ViewModels.Home
{
    using System.Collections.Generic;

    using MovieDeck.Web.ViewModels.Movies;

    public class IndexListViewModel
    {
        public IEnumerable<MovieViewModel> Movies { get; set; }
    }
}
