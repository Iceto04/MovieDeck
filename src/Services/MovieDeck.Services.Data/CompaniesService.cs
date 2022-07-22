namespace MovieDeck.Services.Data
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using MovieDeck.Data.Common.Repositories;
    using MovieDeck.Data.Models;

    public class CompaniesService : ICompaniesService
    {
        private readonly IDeletableEntityRepository<ProductionCompany> companiesRepository;

        public CompaniesService(IDeletableEntityRepository<ProductionCompany> companiesRepository)
        {
            this.companiesRepository = companiesRepository;
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
    }
}
