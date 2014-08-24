namespace NQG.TheoryBox
{
    using System.Collections;
    using Microsoft.Azure.Documents;
    using Microsoft.Azure.Documents.Client;
    using Microsoft.Azure.Documents.Linq;
    using System;
    using System.Configuration;
    using System.Linq;
    using System.Threading.Tasks;
    using NQG.TheoryBox.DomainModel;

    public sealed class Repository
    {
        private static readonly object SyncRoot = new object();

        private static Database s_database;
        private static DocumentCollection s_collection;
        private static DocumentClient s_client;

        public static String DatabaseId
        {
            get
            {
                return ConfigurationManager.AppSettings.Get("TheoryBox_Database");
            }
        }


        public static String CardCollectionId
        {
            get
            {
                return ConfigurationManager.AppSettings.Get("TheoryBox_CardCollection");
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

        public static async Task<DocumentCollection> ReadOrCreateCardCollectionAsync()
        {
            var database = await ReadOrCreateGathererDatabaseAsync();

            if (s_collection != null)
                return s_collection;

            if (s_collection == null)
            {
                s_collection = Client.CreateDocumentCollectionQuery(database.SelfLink)
                    .ToArray()
                    .FirstOrDefault(col => col.Id == CardCollectionId);

                if (s_collection == null)
                {
                    s_collection = await Client.CreateDocumentCollectionAsync(database.SelfLink,
                        new DocumentCollection { Id = CardCollectionId });
                }
            }

            return s_collection;
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

        public static async Task DeleteCardCollectionAsync()
        {
            var collection = await ReadOrCreateCardCollectionAsync();
            await Client.DeleteDocumentCollectionAsync(collection.SelfLink);
        }

        public static async Task<Card> GetCard(string multiverseId)
        {
            var collection = await ReadOrCreateCardCollectionAsync();

            var existingCard =
                Client.CreateDocumentQuery<Card>(collection.SelfLink)
                    .Where(c => c.MultiverseId == multiverseId)
                    .ToArray()
                    .FirstOrDefault();

            return existingCard;
        }

        public static async Task<IOrderedQueryable<Card>> GetAllCards()
        {
            var collection = await ReadOrCreateCardCollectionAsync();

            var existingCards =
                Client.CreateDocumentQuery<Card>(collection.SelfLink);

            return existingCards;
        }

        public static async Task<Card> CreateOrUpdateCardAsync(Card card)
        {
            var collection = await ReadOrCreateCardCollectionAsync();

            var existingCard =
                Client.CreateDocumentQuery(collection.SelfLink)
                    .Where(c => c.Id == card.MultiverseId)
                    .ToArray()
                    .FirstOrDefault();

            if (existingCard == null)
            {
                await Client.CreateDocumentAsync(collection.SelfLink, card);
            }
            else
            {
                await Client.ReplaceDocumentAsync(existingCard.SelfLink, card);
            }

            return card;
        }
         
    }
}
