using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.ChangeFeedProcessor;
using Microsoft.Azure.Documents.Client;
using Microsoft.Extensions.Configuration;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace ChangeFeedProcessor
{
    class Program
    {
        private static readonly CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
        private static readonly ChangeFeedProcessorBuilder builder = new ChangeFeedProcessorBuilder();
        private static CosmosSettings cosmosSettings;
        private static readonly string hostName = Guid.NewGuid().ToString();

        public static IConfiguration Configuration { get; set; }
        public static object ConfigurationManager { get; private set; }

        public static void Main(string[] args)
        {
            cosmosSettings = GetCosmosConfiguration();
            MainAsync().Wait();
        }

        /// <summary>
        /// Main Async function; checks for or creates monitored/lease
        /// collections and runs Change Feed Host
        /// </summary>
        /// <returns>A Task to allow asynchronous execution</returns>
        private static async Task MainAsync()
        {
            await RunChangeFeedHostAsync();
        }

        private static async Task RunChangeFeedHostAsync()
        {
            // monitored collection info 
            DocumentCollectionInfo documentCollectionInfo = new DocumentCollectionInfo
            {
                Uri = new Uri(cosmosSettings.DbUrl),
                MasterKey = cosmosSettings.AuthorizationKey,
                DatabaseName = cosmosSettings.DbName,
                CollectionName = cosmosSettings.CollectionName
            };

            DocumentCollectionInfo leaseCollectionInfo = new DocumentCollectionInfo
            {
                Uri = new Uri(cosmosSettings.DbUrl),
                MasterKey = cosmosSettings.AuthorizationKey,
                DatabaseName = cosmosSettings.DbName,
                CollectionName = cosmosSettings.LeaseCollectionName
            };
            DocumentFeedObserverFactory docObserverFactory = new DocumentFeedObserverFactory();
            ChangeFeedOptions feedOptions = new ChangeFeedOptions
            {
                /* ie customize StartFromBeginning so change feed reads from beginning
                    can customize MaxItemCount, PartitonKeyRangeId, RequestContinuation, SessionToken and StartFromBeginning
                */
                StartFromBeginning = true
            };

            ChangeFeedProcessorOptions feedProcessorOptions = new ChangeFeedProcessorOptions();

            // ie. customizing lease renewal interval to 15 seconds
            // can customize LeaseRenewInterval, LeaseAcquireInterval, LeaseExpirationInterval, FeedPollDelay 
            feedProcessorOptions.LeaseRenewInterval = TimeSpan.FromSeconds(15);
            ChangeFeedProcessorBuilder builder = new ChangeFeedProcessorBuilder();

            builder
                .WithHostName(hostName)
                .WithFeedCollection(documentCollectionInfo)
                .WithLeaseCollection(leaseCollectionInfo)
                .WithProcessorOptions(feedProcessorOptions)
                .WithObserverFactory(new DocumentFeedObserverFactory());
            //.WithObserver<DocumentFeedObserver>();  If no factory then just pass an observer

            var result = await builder.BuildAsync();
            await result.StartAsync();
            Console.Read();
            await result.StopAsync();
        }


        #region "Configuration"
        private static CosmosSettings GetCosmosConfiguration()
        {
            var builder = new ConfigurationBuilder()
               .SetBasePath(Directory.GetCurrentDirectory())
               .AddJsonFile("appsettings.json");

            Configuration = builder.Build();

            var cosmosSettings = new CosmosSettings()
            {
                DbUrl = Configuration.GetSection("cosmos")["DbUrl"],
                DbName = Configuration.GetSection("cosmos")["DbName"],
                CollectionName = Configuration.GetSection("cosmos")["CollectionName"],
                LeaseCollectionName = Configuration.GetSection("cosmos")["LeaseCollectionName"],
                AuthorizationKey = Configuration.GetSection("cosmos")["AuthorizationKey"],
                MaxConnectionLimit = Convert.ToInt32(Configuration.GetSection("cosmos")["MaxConnectionLimit"]),
                OfferThroughput = Convert.ToInt32(Configuration.GetSection("cosmos")["OfferThroughput"])
            };

            return cosmosSettings;
        }
        #endregion
    }
}