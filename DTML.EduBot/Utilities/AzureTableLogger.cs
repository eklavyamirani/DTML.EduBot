using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace DTML.EduBot.Utilities
{
    public class AzureTableLogger : ILogger
    {
        private CloudStorageAccount storageAccount;
        private CloudTableClient tableClient;
        private CloudTable table;

        public AzureTableLogger()
        {
            this.storageAccount = CloudStorageAccount.Parse(ConfigurationManager.AppSettings["StorageConnectionString"]);
            this.tableClient = storageAccount.CreateCloudTableClient();
            this.table = tableClient.GetTableReference("botconversationlogs");
            this.table.CreateIfNotExists();
        }

        public async Task Log(string userId, string message, string eventType)
        {
            try
            {
                LogEntry log = new LogEntry(userId);
                log.message = message;
                log.eventType = eventType;
                log.date = DateTime.Now.ToShortDateString();
                // Create the TableOperation that inserts the customer entity.
                var insertOperation = TableOperation.Insert(log);
                // Execute the insert operation.
                await table.ExecuteAsync(insertOperation);

            }
            catch (Exception)
            {
             // Do nothing...
            }
        }

        public async Task Log(LogEntry log)
        {
            if (log != null)
            {
                try
                {
                    var insertOperation = TableOperation.Insert(log);
                    await table.ExecuteAsync(insertOperation);

                }
                catch (Exception)
                {
                    // Do nothing...
                }
            }
        }

        public Task Log(Exception exception)
        {
            throw new NotImplementedException();
        }

        [Serializable]
        public class LogEntry : TableEntity
        {
            public string message { get; set; }
            public string eventType { get; set; }
            public string detectedLanguage { get; set; }
            public string userId { get; set; }

            public string date { get; set; }

            public LogEntry(string rowKey)
            {
                var key = DateTime.Now;
                PartitionKey = key.Day.ToString(CultureInfo.InvariantCulture) + key.Month.ToString(CultureInfo.InvariantCulture) + key.Year.ToString(CultureInfo.InvariantCulture);
                RowKey = rowKey;
            }

            public LogEntry() { }
        }

    }
}
