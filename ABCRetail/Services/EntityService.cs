using ABCRetail.Models;
using Azure.Data.Tables;

namespace ABCRetail.Services
{
    public class EntityService
    {
        //Gets access to Connection String
        private readonly IConfiguration _configuration;
        
        public EntityService(IConfiguration configuration)
        {
          _configuration = configuration;
        }

        //Create table Client
        private async Task<TableClient> GetTableClient(string tableName)
        { 
            //Access Storage 
            var ServiceClient = new TableServiceClient(_configuration["AzureStorage:ConnectionString"]);
            //Acesss Table
            var tableClient = ServiceClient.GetTableClient(tableName);

            //Creates table if it doesnt exists
            await tableClient.CreateIfNotExistsAsync();
            return tableClient;
        }

        //Returns all entities in a Table
        public async IAsyncEnumerable<T> GetAllEntityAsync<T>(string tableName) where T : class, ITableEntity, new()
        {
            var tableClient = await GetTableClient(tableName);

            await foreach (var customer in tableClient.QueryAsync<T>())
            {
                yield return customer;
            }
        }

        //Search for an Entity
        public async Task<T> GetEntityAsync<T>(string tableName, string PartitionKey, string RowKey) where T : class, ITableEntity, new()
        {
            var tableClient = await GetTableClient(tableName);

            try
            {
                var response = await tableClient.GetEntityAsync<T>(PartitionKey, RowKey);
                return response.Value;
            }
            catch (Azure.RequestFailedException ex) when (ex.Status == 404)
            {
                // Entity not found
                return null;
            }
        }
       

        //Insect/update entity
        public async Task<T> InsertUpdateAsync<T>(T entity, string tableName) where T : class, ITableEntity, new()
        {
            var tableClient = await GetTableClient(tableName);

            //updates or adds entity if it does not exist
            await tableClient.UpsertEntityAsync(entity);
            return entity;
        }

        //Delete the entity
        public async Task DeleteEntityAsync(string tableName,string PartitionKey, string RowKey)
        {
            var tableClient = await GetTableClient(tableName);
            await tableClient.DeleteEntityAsync(PartitionKey, RowKey);

        }
    } 
}
