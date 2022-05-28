namespace MovieDeck.Services.Data
{
    using System.Collections.Generic;

    public interface IGenresService
    {
        IEnumerable<KeyValuePair<string, string>> GetAllAsKeyValuePairs();
    }
}
