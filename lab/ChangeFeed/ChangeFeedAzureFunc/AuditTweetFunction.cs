using System.Collections.Generic;
using Microsoft.Azure.Documents;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;

namespace ChangeFeedAzureFunc
{
    public static class AuditTweetFunction
    {
        [FunctionName("AuditTweetFunction")]
        public static void Run([CosmosDBTrigger(
            databaseName: "TweetsDb",
            collectionName: "Tweets",
            ConnectionStringSetting = "DBConnection",
            LeaseCollectionName = "leases")]IReadOnlyList<Document> input, ILogger log)
        {
            if (input != null && input.Count > 0)
            {
                log.LogInformation("Total Documents modified " + input.Count);

                foreach (var doc in input)
                {
                    log.LogInformation("Document Id " + doc.Id);
                }
                
            }
        }
    }
}
