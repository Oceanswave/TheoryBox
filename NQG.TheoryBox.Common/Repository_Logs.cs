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
        public static string LogsDatabaseName
        {
            get
            {
                return ConfigurationManager.AppSettings.Get("TheoryBox_Database_Logs");
            }
        }

        public static async Task<bool> LogExistsAsync(string multiverseId)
        {
            using (var store = new MyCouchStore(GetDbClient(LogsDatabaseName)))
            {
                var existingCardResponse =
                    await store.ExistsAsync(multiverseId);

                return existingCardResponse;
            }
        }

        public static async Task<CardLog> GetLog(string multiverseId)
        {
            using (var store = new MyCouchStore(GetDbClient(LogsDatabaseName)))
            {
                return await store.GetByIdAsync<CardLog>(multiverseId);
            }
        }

        public static async Task<IList<CardLog>> GetAllCardLogs()
        {
            using (var client = GetDbClient(LogsDatabaseName))
            {
                var query2 = new QueryViewRequest(SystemViewIdentity.AllDocs);

                var qi = await client.Views.QueryAsync<CardLog>(query2);
                var result = qi.Rows.OrderBy(r => r.Id).Select(r => r.Value).ToList();
                return result;
            }
        }

        public static async Task<CardLog> CreateOrUpdateCardLogAsync(CardLog cardLog)
        {
            using (var store = new MyCouchStore(GetDbClient(LogsDatabaseName)))
            {
                var setResponse = await store.SetAsync(cardLog);
                return setResponse;
            }
        }
    }
}
