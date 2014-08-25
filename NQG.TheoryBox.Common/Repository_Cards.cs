namespace NQG.TheoryBox
{
    using System;
    using System.Configuration;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.Azure.Documents;
    using Microsoft.Azure.Documents.Linq;
    using NQG.TheoryBox.DomainModel;

    public partial class Repository
    {
        private static DocumentCollection s_cardCollection;

        public static string CardCollectionId
        {
            get
            {
                return ConfigurationManager.AppSettings.Get("TheoryBox_CardCollection");
            }
        }

        public static async Task<DocumentCollection> ReadOrCreateCardCollectionAsync()
        {
            var database = await ReadOrCreateGathererDatabaseAsync();

            if (s_cardCollection != null)
                return s_cardCollection;

            if (s_cardCollection == null)
            {
                s_cardCollection = Client.CreateDocumentCollectionQuery(database.SelfLink)
                    .ToArray()
                    .FirstOrDefault(col => col.Id == CardCollectionId);

                // ReSharper disable once ConvertIfStatementToNullCoalescingExpression
                if (s_cardCollection == null)
                {
                    s_cardCollection = await Client.CreateDocumentCollectionAsync(database.SelfLink,
                        new DocumentCollection { Id = CardCollectionId });
                }
            }

            return s_cardCollection;
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
