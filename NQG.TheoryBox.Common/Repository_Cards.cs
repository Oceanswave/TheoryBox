namespace NQG.TheoryBox
{
    using MyCouch;
    using MyCouch.Requests;
    using NQG.TheoryBox.DomainModel;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Linq;
    using System.Threading.Tasks;

    public partial class Repository
    {
        public static string CardsDatabaseName
        {
            get
            {
                return ConfigurationManager.AppSettings.Get("TheoryBox_Database_Cards");
            }
        }

        public static async Task<bool> CardExistsAsync(string multiverseId)
        {
            using (var store = new MyCouchStore(GetDbClient(CardsDatabaseName)))
            {
                var existingCardResponse =
                    await store.ExistsAsync(multiverseId);

                return existingCardResponse;
            }
            
        }

        public static async Task<Card> GetCardAsync(string multiverseId)
        {
            using (var store = new MyCouchStore(GetDbClient(CardsDatabaseName)))
            {
                return await store.GetByIdAsync<Card>(multiverseId);
            }
        }

        public static async Task<IList<Card>> GetAllCardsAsync()
        {
            using (var client = GetDbClient(CardsDatabaseName))
            {
                var query2 = new QueryViewRequest(SystemViewIdentity.AllDocs);

                var qi = await client.Views.QueryAsync<Card>(query2);
                var result = qi.Rows.OrderBy(r => r.Value.Details.Name).Select(r => r.Value).ToList();
                return result;
            }
        }

        public static async Task<Card> CreateOrUpdateCardAsync(Card card)
        {
            using (var store = new MyCouchStore(GetDbClient(CardsDatabaseName)))
            {
                var setResponse = await store.SetAsync(card);
                return setResponse;
            }
        }
    }
}
