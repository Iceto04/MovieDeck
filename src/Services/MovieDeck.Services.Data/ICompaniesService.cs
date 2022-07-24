namespace MovieDeck.Services.Data
{
    using System.Threading.Tasks;

    using MovieDeck.Data.Models;
    using MovieDeck.Web.ViewModels.Companies;

    public interface ICompaniesService : IGetAllAsKeyValuePairs
    {
        ProductionCompany GetById(int id);

        Task CreateAsync(CreateCompanyInputModel input);
    }
}
