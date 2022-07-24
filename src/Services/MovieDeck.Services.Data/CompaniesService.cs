namespace MovieDeck.Services.Data
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using MovieDeck.Data.Common.Repositories;
    using MovieDeck.Data.Models;
    using MovieDeck.Web.ViewModels.Companies;

    public class CompaniesService : ICompaniesService
    {
        private readonly IDeletableEntityRepository<ProductionCompany> companiesRepository;
        private readonly IRepository<MovieCompany> movieCompanyRepository;

        public CompaniesService(
            IDeletableEntityRepository<ProductionCompany> companiesRepository,
            IRepository<MovieCompany> movieCompanyRepository)
        {
            this.companiesRepository = companiesRepository;
            this.movieCompanyRepository = movieCompanyRepository;
        }

        public async Task CreateAsync(CreateCompanyInputModel input)
        {
            var company = new ProductionCompany
            {
                Name = input.Name,
            };

            await this.companiesRepository.AddAsync(company);
            await this.companiesRepository.SaveChangesAsync();
        }

        public ProductionCompany GetById(int id)
        {
            return this.companiesRepository.All().FirstOrDefault(x => x.Id == id);
        }

        public IEnumerable<KeyValuePair<string, string>> GetAllAsKeyValuePairs()
        {
            return this.companiesRepository.AllAsNoTracking()
                .Select(x => new
                {
                    x.Id,
                    x.Name,
                })
                .OrderBy(x => x.Name)
                .ToList().Select(x => new KeyValuePair<string, string>(x.Id.ToString(), x.Name));
        }

        public async Task RemoveAllMovieCompaniesForMovieAsync(int id)
        {
            var movieCompanies = this.movieCompanyRepository.All().Where(x => x.MovieId == id);
            foreach (var movieCompany in movieCompanies)
            {
                this.movieCompanyRepository.Delete(movieCompany);
            }

            await this.movieCompanyRepository.SaveChangesAsync();
        }
    }
}
