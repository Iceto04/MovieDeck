namespace MovieDeck.Web.ViewModels.Companies
{
    using System.ComponentModel.DataAnnotations;

    public class CreateCompanyInputModel
    {
        [Required]
        [MinLength(2)]
        public string Name { get; set; }
    }
}
