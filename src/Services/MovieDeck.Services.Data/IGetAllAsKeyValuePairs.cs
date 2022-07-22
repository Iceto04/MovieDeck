namespace MovieDeck.Services.Data
{
    using System.Collections.Generic;

    public interface IGetAllAsKeyValuePairs
    {
        IEnumerable<KeyValuePair<string, string>> GetAllAsKeyValuePairs();
    }
}
