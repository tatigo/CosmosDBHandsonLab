using Azure.CosmosSQL.Models;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.Documents.Linq;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Azure.CosmosSQL
{
    class Program
    {
        public static IConfiguration Configuration { get; set; }
        private static CosmosSettings cosmosSettings;
        private static DocumentClient client;

        static void Main(string[] args)
        {
            cosmosSettings = GetCosmosConfiguration();
            client = new DocumentClient(new Uri(cosmosSettings.DbUrl), cosmosSettings.AuthorizationKey);

            try
            {
                /****** Create a Document **********************************/
                
                var tweet = TweetManager.CreateTweet("#Cosmo SQL API Level 500 training at #Mississauga");
                CreateDocument(tweet).ConfigureAwait(false);

                /*********Create a Document**********************************

                /******* Bulk Uploading Documents**************/

                //UploadBulkUploadDocuments().ConfigureAwait(false);

                /*******  Bulk Uploading Documents***********/


                /*******  Query Document By Document Id*****************/

                //string documentId = "acb113cb-ece6-4ad7-b53b-e269872c2db2";
                //var response = GetDocument(documentId);

                //Console.WriteLine(response.Result.User + " " + response.Result.Text);
                //Console.WriteLine("1.2 - Query Document By Document Id - Complete");

                /*******  Query Document By Document Id*****************/

                /*******  UPDATE / REPLACE DOCUMENTS********************************/

                //Console.WriteLine("1.3 - Update/Replace Documents - Start");
                //string documentId = "acb113cb-ece6-4ad7-b53b-e269872c2db2";
                //var response = GetDocument(documentId);
                //var tweetToModify = response.Result;
                //tweetToModify.User.screenName = "Azure Specialist";
                //UpdateDocument(tweetToModify).ConfigureAwait(false);
                //Console.WriteLine("1.3 - Update/Replace Documents - Complete");

                /*******  UPDATE / REPLACE DOCUMENTS********************************/


                /********* UPSERT DOCUMENTS********************************/

                // Create a new Tweet
                //var tweet = TweetManager.CreateTweet("Welcome to #Cosmo SQL API Level 500");
                //UpsertDocument(tweet).ConfigureAwait(false);

                /********* UPSERT DOCUMENTS********************************/

                /********************* DELETE DOCUMENT ****************************/
                var documentToDelete = "8685a809-83f9-48e6-a8b5-c189e35060c4";
                DeleteDocument(documentToDelete).ConfigureAwait(false);

                /*************************************************************/

                /*******************************************************************/
                //Console.WriteLine("1.4 - Query Documents (Tweets) By HashTag Start");
                //var tweets = GetTweetByHashTag("#Azure");

                //foreach(var tweet in tweets)
                //{
                //    Console.WriteLine(tweet.UserName + "-" + tweet.Message + "\n");
                //}

                //Console.WriteLine("1.2 - Query Documents (Tweets) By HashTag Complete");

            }
            catch (ArgumentException aex)
            {
                Console.WriteLine($"Caught ArgumentException: {aex.Message}");
            }

            Console.ReadKey();
        }

        #region "Bulk Upload Documents"
        private static async Task UploadBulkUploadDocuments()
        {
            List<Tweet> tweets = TweetManager.BullkCreateTweets();

            foreach (var tweet in tweets)
            {
                await CreateDocument(tweet);
            }
        }
        #endregion

        #region "Create Document"
        /// <summary>
        /// Create Document in Cosmos DB.
        /// </summary>
        /// <param name="tweet">Represents Tweet object.</param>
        private static async Task CreateDocument(Tweet tweet)
        {
            try
            {
                var response = await client.CreateDocumentAsync(
                                            GetCollectionSelfLink(),
                                            tweet,
                                            new RequestOptions
                                            {
                                                JsonSerializerSettings = new JsonSerializerSettings
                                                {
                                                    ContractResolver = new CamelCasePropertyNamesContractResolver()
                                                },
                                                OfferThroughput = cosmosSettings.OfferThroughput
                                            },false);

                Console.WriteLine($"Status code of Create Document Operation: {response.StatusCode}");
            }
            catch(Exception ex)
            {
                throw new Exception(ex.Message);
            }

        }
        #endregion

        #region "Read Document By Id"
        /// <summary>
        /// Reads Document from Cosmos DB based on the Document Id.
        /// </summary>
        /// <param name="documentProvider"></param>
        private static async Task<Tweet> GetDocument(string documentId)
        {
            var documentUri = UriFactory.CreateDocumentUri(cosmosSettings.DbName, cosmosSettings.CollectionName, documentId);
            return await client.ReadDocumentAsync<Tweet>(documentUri);
        }
        #endregion

        #region "Update Document"
        /// <summary>
        /// Uploads the tweet to Cosmos DB.
        /// </summary>
        /// <param name="documentProvider"></param>
        private static async Task UpdateDocument(Tweet tweet)
        {
            var documentUri = UriFactory.CreateDocumentUri(cosmosSettings.DbName, cosmosSettings.CollectionName, tweet.Id.ToString());

            try
            {
                var response = await client.ReplaceDocumentAsync(documentUri,
                                                tweet,
                                                new RequestOptions
                                                {
                                                    JsonSerializerSettings = new JsonSerializerSettings
                                                    {
                                                        ContractResolver = new CamelCasePropertyNamesContractResolver()
                                                    },
                                                    OfferThroughput = cosmosSettings.OfferThroughput
                                                });

                Console.WriteLine($"Status code of Update/Replace Document Operation: {response.StatusCode}");
            }
            catch(Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        #endregion

        #region "Upsert Document"

        /// <summary>
        /// Upserts document.
        /// Case 1. Upsert on a new object => document is created.
        //  Case 2. If you update a property on the existing document then upsert on this document => Replaces the document
        /// </summary>
        /// <param name="documentProvider"></param>
        private static async Task UpsertDocument(Tweet tweet)
        {
            try
            {
                var response = await client.UpsertDocumentAsync(GetCollectionSelfLink(), tweet, new RequestOptions
                {
                    JsonSerializerSettings = new JsonSerializerSettings
                    {
                        ContractResolver = new CamelCasePropertyNamesContractResolver()
                    },
                    OfferThroughput = cosmosSettings.OfferThroughput
                });

                Console.WriteLine($"Status code of Upsert Operation: {response.StatusCode}");
            }
            catch(Exception ex)
            {
                throw new Exception(ex.Message);
            }
            
        }

        #endregion
        
        #region "Delete Document"
        /// <summary>
        /// Reads Document from Cosmos DB based on the Document Id.
        /// </summary>
        /// <param name="documentProvider"></param>
        private static async Task DeleteDocument(string documentId)
        {
            try
            {
                var documentUri = UriFactory.CreateDocumentUri(cosmosSettings.DbName, cosmosSettings.CollectionName, documentId);
                var response = await client.DeleteDocumentAsync(documentUri);
            }
            catch(Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        #endregion

        #region "Advanced SQL Queries - JOIN"
        /// <summary>
        /// Uploads the tweet to Cosmos DB.
        /// </summary>
        /// <param name="documentProvider"></param>
        private static List<TweetSummary> GetTweetByHashTag(string hashTag)
        {
            var sql = new SqlQuerySpec
            {
                QueryText = "SELECT value  { \"UserName\": t.User.name, \"Message\" : t.Text} FROM t JOIN h IN t.HashTags WHERE h.Text = @hashTag",
                            Parameters = new SqlParameterCollection {
                                new SqlParameter { Name = "@hashTag", Value = hashTag.ToLower() }
                            }
            };

            var result = client.CreateDocumentQuery<TweetSummary>(
                                        GetCollectionSelfLink(),
                                        sql,
                                        new FeedOptions { EnableCrossPartitionQuery = true }
                                       ).ToList();

            return result;
        }
        #endregion

        /// <summary>
        /// Retrun Collection Self Link
        /// </summary>
        /// <returns></returns>
        private static string GetCollectionSelfLink()
        {
            return UriFactory.CreateDocumentCollectionUri(cosmosSettings.DbName, cosmosSettings.CollectionName).ToString();
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
                AuthorizationKey = Configuration.GetSection("cosmos")["AuthorizationKey"],
                MaxConnectionLimit = Convert.ToInt32(Configuration.GetSection("cosmos")["MaxConnectionLimit"]),
                OfferThroughput = Convert.ToInt32(Configuration.GetSection("cosmos")["OfferThroughput"])
            };

            return cosmosSettings;
        }

        #endregion



    }
}
