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
        private static DocumentCollection s_logsCollection;

        public static String LogsCollectionId
        {
            get
            {
                return ConfigurationManager.AppSettings.Get("TheoryBox_LogsCollection");
            }
        }

        public static async Task<DocumentCollection> ReadOrCreateLogsCollectionAsync()
        {
            var database = await ReadOrCreateGathererDatabaseAsync();

            if (s_logsCollection != null)
                return s_logsCollection;

            if (s_logsCollection == null)
            {
                s_logsCollection = Client.CreateDocumentCollectionQuery(database.SelfLink)
                    .ToArray()
                    .FirstOrDefault(col => col.Id == CardCollectionId);

                // ReSharper disable once ConvertIfStatementToNullCoalescingExpression
                if (s_logsCollection == null)
                {
                    s_logsCollection = await Client.CreateDocumentCollectionAsync(database.SelfLink,
                        new DocumentCollection { Id = LogsCollectionId });
                }
            }

            return s_logsCollection;
        }

        public static async Task<CardLog> GetLog(string multiverseId)
        {
            var collection = await ReadOrCreateLogsCollectionAsync();

            var existingLog =
                Client.CreateDocumentQuery<CardLog>(collection.SelfLink)
                    .Where(c => c.MultiverseId == multiverseId)
                    .ToArray()
                    .FirstOrDefault();

            return existingLog;
        }

        public static async Task<IOrderedQueryable<CardLog>> GetAllCardLogs()
        {
            var collection = await ReadOrCreateLogsCollectionAsync();

            var logs =
                Client.CreateDocumentQuery<CardLog>(collection.SelfLink);

            return logs;
        }

        public static async Task<CardLog> CreateOrUpdateCardLogAsync(CardLog cardLog)
        {
            var collection = await ReadOrCreateLogsCollectionAsync();

            var existingLog =
                Client.CreateDocumentQuery(collection.SelfLink)
                    .Where(c => c.Id == cardLog.MultiverseId)
                    .ToArray()
                    .FirstOrDefault();

            if (existingLog == null)
            {
                await Client.CreateDocumentAsync(collection.SelfLink, cardLog);
            }
            else
            {
                await Client.ReplaceDocumentAsync(existingLog.SelfLink, cardLog);
            }

            return cardLog;
        }

    }
}
