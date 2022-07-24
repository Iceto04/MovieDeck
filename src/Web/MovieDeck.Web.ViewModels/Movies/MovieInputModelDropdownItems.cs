namespace MovieDeck.Web.ViewModels.Movies
{
    using System.Collections.Generic;

    using Microsoft.AspNetCore.Mvc.Rendering;

    public class MovieInputModelDropdownItems
    {
        public IEnumerable<SelectListItem> ActorsItems { get; set; }

        public IEnumerable<SelectListItem> DirectorsItems { get; set; }

        public IEnumerable<SelectListItem> CompaniesItems { get; set; }

        public IEnumerable<SelectListItem> GenresItems { get; set; }
    }
}