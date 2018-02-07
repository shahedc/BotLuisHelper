# BotLuisHelper
Helper utilities for Bot Framework projects with LUIS utterances. 


Replace placeholder info in the following files. Make sure you move config info to config files. 

Better yet: Here's an old blog post that talks about how to move config info out of config files to easily avoid committing config info to source control
http://wakeupandcode.com/all-your-database-are-belong-to-us 

# Project: LuisConsoleApp
## File: Program.cs 

``` 
static string appID = "<APP_ID>";
static string appVersion = "0.1";
static string key = "<APP_KEY>";
``` 

# Project: CosmosUtteranceHelper
## File: Program.cs

``` 
private const string EndpointUrl = "https://<COSMOS_DB>.documents.azure.com:443/";
private const string PrimaryKey = "<COSMOS_KEY>";
private string dbName = "<COSMOS_DB>";
private string collName = "<COSMOS_COLL>";
``` 

# Project: LuisWebApi
## File: TrainController.cs

``` 
static string appID = "<APP_ID>";
static string appVersion = "0.1";
static string key = "<APP_KEY>";
``` 








