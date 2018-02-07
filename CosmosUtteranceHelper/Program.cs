using System;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Newtonsoft.Json;

namespace CosmosUtteranceHelper
{
    class Program
    {
        // Cosmos DB info
        private const string EndpointUrl = "https://<COSMOS_DB>.documents.azure.com:443/";
        private const string PrimaryKey = "<COSMOS_KEY>";
        private DocumentClient client;
        private string dbName = "<COSMOS_DB>";
        private string collName = "<COSMOS_COLL>";

        // Sample utterance data
        string uId1 = "Utterance.001";
        string uText1 = "Some utterance text";
        string uIntent1 = "SomeIntent";
        string uEntity1 = "EntityName1";
        string uId2 = "Utterance.002";
        string uText2 = "Some utterance text";
        string uIntent2 = "SomeIntent2";
        string uEntity2 = "EntityName2";
        string uIntentUpdated = "NewIntent";

        static void Main(string[] args)
        {
            try
            {
                Program p = new Program();
                p.ProcessUtterances().Wait();
            }
            catch (DocumentClientException de)
            {
                Exception baseException = de.GetBaseException();
                Console.WriteLine("{0} error occurred: {1}, Message: {2}", de.StatusCode, de.Message, baseException.Message);
            }
            catch (Exception e)
            {
                Exception baseException = e.GetBaseException();
                Console.WriteLine("Error: {0}, Message: {1}", e.Message, baseException.Message);
            }
            finally
            {
                Console.WriteLine("End of program, press any key to exit.");
                Console.ReadKey();
            }
        }

        private async Task ProcessUtterances()
        {
            this.client = new DocumentClient(new Uri(EndpointUrl), PrimaryKey);

            await this.client.CreateDatabaseIfNotExistsAsync(new Database { Id = dbName });

            await this.client.CreateDocumentCollectionIfNotExistsAsync(UriFactory.CreateDatabaseUri(dbName), new DocumentCollection { Id = collName });

            // create sample utterance models
            UtteranceModel utteranceModel1 = new UtteranceModel
            {
                Id = uId1,
                Utterances = new Utterance[]
                {
                    new Utterance
                    {
                        text = uText1,
                        intentName = uIntent1,
                        entityLabels = new EntityLabel[]
                        {
                            new EntityLabel
                            {
                                entityName = uEntity1,
                                startCharIndex = 0,
                                endCharIndex = 1
                            }
                        }
                    }
                }
            };

            // add sample utterance model
            await this.CreateUtteranceDocumentIfNotExists(dbName, collName, utteranceModel1);

            UtteranceModel utteranceModel2 = new UtteranceModel
            {
                Id = uId2,
                Utterances = new Utterance[]
                {
                    new Utterance
                    {
                        text = uText2,
                        intentName = uIntent2,
                        entityLabels = new EntityLabel[]
                        {
                            new EntityLabel
                            {
                                entityName = uEntity2,
                                startCharIndex = 0,
                                endCharIndex = 1
                            }
                        }
                    }
                }
            };

            // add sample utterance model
            await this.CreateUtteranceDocumentIfNotExists(dbName, collName, utteranceModel2);

            // retrieve sample data
            this.ExecuteSimpleQuery(dbName, collName);


            // Update the intent
            utteranceModel2.Utterances[0].intentName = uIntentUpdated;

            await this.ReplaceUtteranceDocument(dbName, collName, uId2, utteranceModel2);

            this.ExecuteSimpleQuery(dbName, collName);
            
            // delete sample document
            //await this.DeleteUtteranceDocument(dbName, collName, "Utterance.002");
            
            // Clean up/delete the database
            //await this.client.DeleteDatabaseAsync(UriFactory.CreateDatabaseUri(dbName));
        }

        private void WriteToConsoleAndPromptToContinue(string format, params object[] args)
        {
            Console.WriteLine(format, args);
            Console.WriteLine("Press any key to continue ...");
            Console.ReadKey();
        }


        private async Task CreateUtteranceDocumentIfNotExists(string databaseName, string collectionName, UtteranceModel utteranceModels)
        {
            try
            {
                await this.client.ReadDocumentAsync(UriFactory.CreateDocumentUri(databaseName, collectionName, utteranceModels.Id));
                this.WriteToConsoleAndPromptToContinue("Found {0}", utteranceModels.Id);
            }
            catch (DocumentClientException de)
            {
                if (de.StatusCode == HttpStatusCode.NotFound)
                {
                    await this.client.CreateDocumentAsync(UriFactory.CreateDocumentCollectionUri(databaseName, collectionName), utteranceModels);
                    this.WriteToConsoleAndPromptToContinue("Created Utterance {0}", utteranceModels.Id);
                }
                else
                {
                    throw;
                }
            }
        }
        
        private void ExecuteSimpleQuery(string databaseName, string collectionName)
        {
            // Set some common query options
            FeedOptions queryOptions = new FeedOptions { MaxItemCount = -1 };

            // Here we find the Andersen family via its LastName
            IQueryable<UtteranceModel> utteranceQuery = this.client.CreateDocumentQuery<UtteranceModel>(
                    UriFactory.CreateDocumentCollectionUri(databaseName, collectionName), queryOptions)
                    .Where(u => u.Id == uId2);

            // The query is executed synchronously here, but can also be executed asynchronously via the IDocumentQuery<T> interface
            Console.WriteLine("Running LINQ query...");
            foreach (UtteranceModel utteranceModel in utteranceQuery)
            {
                Console.WriteLine("\tRead {0}", utteranceModel);
            }

            // Now execute the same query via direct SQL
            IQueryable<UtteranceModel> utteranceQueryInSql = this.client.CreateDocumentQuery<UtteranceModel>(
                    UriFactory.CreateDocumentCollectionUri(databaseName, collectionName),
                    "SELECT * FROM UtteranceModel WHERE UtteranceModel.Id = '" + uId2 + "'",
                    queryOptions);

            Console.WriteLine("Running direct SQL query...");
            foreach (UtteranceModel utteranceModel in utteranceQueryInSql)
            {
                Console.WriteLine("\tRead {0}", utteranceModel);
            }

            Console.WriteLine("Press any key to continue ...");
            Console.ReadKey();
        }

        private async Task ReplaceUtteranceDocument(string databaseName, string collectionName, string utteranceText, UtteranceModel updateUtteranceModel)
        {
            await this.client.ReplaceDocumentAsync(UriFactory.CreateDocumentUri(databaseName, collectionName, utteranceText), updateUtteranceModel);
            this.WriteToConsoleAndPromptToContinue("Replaced Utterance {0}", utteranceText);
        }

        private async Task DeleteUtteranceDocument(string databaseName, string collectionName, string documentName)
        {
            await this.client.DeleteDocumentAsync(UriFactory.CreateDocumentUri(databaseName, collectionName, documentName));
            Console.WriteLine("Deleted Utterance {0}", documentName);
        }
    }


}
