namespace MovieDeck.Services.Data
{
    using System.Collections.Generic;
    using System.Linq;

    using MovieDeck.Data.Common.Repositories;
    using MovieDeck.Data.Models;

    public class DirectorsService : IDirectorsService
    {
        private readonly IDeletableEntityRepository<Director> directorsRepository;

        public DirectorsService(IDeletableEntityRepository<Director> directorsRepository)
        {
            this.directorsRepository = directorsRepository;
        }

        public Director GetById(int id)
        {
            return this.directorsRepository.All().FirstOrDefault(x => x.Id == id);
        }

        public IEnumerable<KeyValuePair<string, string>> GetAllAsKeyValuePairs()
        {
            return this.directorsRepository.AllAsNoTracking()
                .Select(x => new
                {
                    x.Id,
                    x.FullName,
                })
                .OrderBy(x => x.FullName)
                .ToList().Select(x => new KeyValuePair<string, string>(x.Id.ToString(), x.FullName));
        }
    }
}
