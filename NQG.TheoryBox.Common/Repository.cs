namespace NQG.TheoryBox
{
    using Microsoft.Azure.Documents;
    using Microsoft.Azure.Documents.Client;
    using Microsoft.Azure.Documents.Linq;
    using System;
    using System.Configuration;
    using System.Linq;
    using System.Threading.Tasks;

    public static partial class Repository
    {
        private static readonly object SyncRoot = new object();

        private static Database s_database;
        private static DocumentClient s_client;

        public static String DatabaseId
        {
            get
            {
                return ConfigurationManager.AppSettings.Get("TheoryBox_Database");
            }
        }

        public static DocumentClient Client
        {
            get
            {
                if (s_client == null)
                {
                    lock (SyncRoot)
                    {
                        if (s_client == null)
                        {
                            var endpoint = ConfigurationManager.AppSettings.Get("TheoryBox_EndpointUrl");
                            var authKey = ConfigurationManager.AppSettings.Get("TheoryBox_AuthorizationKey");
                            var endpointUri = new Uri(endpoint);
                            s_client = new DocumentClient(endpointUri, authKey);
                        }
                    }
                }

                return s_client;
            }
        }

        public static async Task<Database> ReadOrCreateGathererDatabaseAsync()
        {
            if (s_database != null)
                return s_database;

            s_database = Client
                .CreateDatabaseQuery()
                .ToArray()
                .FirstOrDefault(db => db.Id == DatabaseId);

            if (s_database == null)
            {
                var database = new Database { Id = DatabaseId };
                s_database = await Client.CreateDatabaseAsync(database);
            }

            return s_database;
        }
    }
}
