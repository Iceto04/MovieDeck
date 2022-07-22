namespace MovieDeck.Services.Data
{
    using MovieDeck.Data.Models;

    public interface ICompaniesService : IGetAllAsKeyValuePairs
    {
        ProductionCompany GetById(int id);
    }
}
