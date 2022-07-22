namespace MovieDeck.Services.Data
{
    using System.Collections.Generic;
    using System.Linq;

    using MovieDeck.Data.Common.Repositories;
    using MovieDeck.Data.Models;

    public class ActorsService : IActorsService
    {
        private readonly IDeletableEntityRepository<Actor> actorsRepository;

        public ActorsService(IDeletableEntityRepository<Actor> actorsRepository)
        {
            this.actorsRepository = actorsRepository;
        }

        public Actor GetById(int id)
        {
            return this.actorsRepository.All().FirstOrDefault(x => x.Id == id);
        }

        public IEnumerable<KeyValuePair<string, string>> GetAllAsKeyValuePairs()
        {
            return this.actorsRepository.AllAsNoTracking()
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
